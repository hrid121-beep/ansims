// REMOVED: using AutoMapper - now using manual mapping
using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;

namespace IMS.Application.Services
{
    public class ReturnService : IReturnService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        // REMOVED: IMapper - using manual mapping instead
        private readonly IWriteOffService _writeOffService;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReturnService> _logger;
        private readonly IApprovalService _approvalService;
        private readonly IStockService _stockService;

        public ReturnService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            // REMOVED: IMapper mapper - no longer needed
            IWriteOffService writeOffService,
            IActivityLogService activityLogService,
            INotificationService notificationService,
            IApprovalService approvalService,
            IStockService stockService,
            ILogger<ReturnService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            // REMOVED: _mapper assignment
            _writeOffService = writeOffService;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
            _approvalService = approvalService;
            _stockService = stockService;
            _logger = logger;
        }

        // Create Return Request
        public async Task<ReturnDto> CreateReturnRequestAsync(ReturnDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate original issue
                var originalIssue = await _unitOfWork.Issues.Query()
                    .Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.Id == dto.OriginalIssueId);

                if (originalIssue == null)
                    throw new InvalidOperationException("Original issue not found");

                if (originalIssue.Status != IssueStatus.Issued.ToString())
                    throw new InvalidOperationException("Original issue is not in issued status");

                // Validate return quantities
                foreach (var returnItem in dto.Items)
                {
                    var issueItem = originalIssue.Items.FirstOrDefault(i => i.ItemId == returnItem.ItemId);
                    if (issueItem == null)
                        throw new InvalidOperationException($"Item {returnItem.ItemId} was not part of original issue");

                    var previousReturns = await GetPreviousReturnQuantityAsync(dto.OriginalIssueId, returnItem.ItemId);
                    var maxReturnQuantity = issueItem.IssuedQuantity - previousReturns;

                    if (returnItem.ReturnQuantity > maxReturnQuantity)
                        throw new InvalidOperationException($"Return quantity exceeds issued quantity for item {returnItem.ItemName}");
                }

                // Create return record
                var returnEntity = new Return
                {
                    ReturnNo = await GenerateReturnNoAsync(),
                    OriginalIssueId = dto.OriginalIssueId,
                    OriginalIssueNo = originalIssue.IssueNo,
                    ReturnDate = dto.ReturnDate,
                    ReturnType = dto.ReturnType, // Normal, Damaged, Expired
                    Reason = dto.Reason,
                    Status = ReturnStatus.Pending,
                    RequestedBy = dto.RequestedBy,
                    RequestedDate = DateTime.Now,

                    // Store information
                    ToStoreId = originalIssue.FromStoreId ?? 0,
                    FromEntityType = originalIssue.ToEntityType,
                    FromEntityId = originalIssue.ToEntityId,

                    Items = new List<ReturnItem>()
                };

                foreach (var itemDto in dto.Items)
                {
                    returnEntity.Items.Add(new ReturnItem
                    {
                        ItemId = itemDto.ItemId,
                        ReturnQuantity = itemDto.ReturnQuantity,
                        Condition = itemDto.Condition, // Good, Damaged, Expired
                        ReturnReason = itemDto.ReturnReason,
                        BatchNo = itemDto.BatchNo,
                        Remarks = itemDto.Remarks
                    });
                }

                await _unitOfWork.Returns.AddAsync(returnEntity);
                await _unitOfWork.CompleteAsync();

                // Create approval request if needed
                if (dto.ReturnType == "Damaged" || dto.ReturnType == "Expired")
                {
                    await _approvalService.CreateApprovalRequestAsync(new ApprovalRequestDto
                    {
                        EntityType = "Return",
                        EntityId = returnEntity.Id,
                        RequestedBy = dto.RequestedBy,
                        RequestedDate = DateTime.Now,
                        Description = $"Return Request: {returnEntity.ReturnNo} - {dto.ReturnType}",
                        Priority = "Normal"
                    });
                }

                // Send notification
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    Title = "New Return Request",
                    Message = $"Return request {returnEntity.ReturnNo} has been created",
                    Type = "Return",
                    CreatedAt = DateTime.Now
                });

                await _unitOfWork.CommitTransactionAsync();

                dto.Id = returnEntity.Id;
                dto.Status = returnEntity.Status;
                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating return request");
                throw;
            }
        }

        // Check Item Condition
        public async Task<bool> CheckReturnConditionAsync(int returnId, ConditionCheckDto checkDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var returnEntity = await _unitOfWork.Returns.Query()
                    .Include(r => r.Items)
                    .FirstOrDefaultAsync(r => r.Id == returnId);

                if (returnEntity == null)
                    throw new InvalidOperationException("Return not found");

                if (returnEntity.Status != ReturnStatus.Pending)
                    throw new InvalidOperationException("Return is not pending for condition check");

                // Create condition check record
                var conditionCheck = new ConditionCheck
                {
                    ReturnId = returnId,
                    CheckedBy = checkDto.CheckedBy,
                    CheckedDate = DateTime.Now,
                    OverallCondition = checkDto.OverallCondition,
                    Items = new List<ConditionCheckItem>()
                };

                foreach (var checkItem in checkDto.Items)
                {
                    var returnItem = returnEntity.Items.FirstOrDefault(ri => ri.ItemId == checkItem.ItemId);
                    if (returnItem == null) continue;

                    var conditionItem = new ConditionCheckItem
                    {
                        ItemId = checkItem.ItemId,
                        CheckedQuantity = checkItem.CheckedQuantity,
                        GoodQuantity = checkItem.GoodQuantity,
                        DamagedQuantity = checkItem.DamagedQuantity,
                        ExpiredQuantity = checkItem.ExpiredQuantity,
                        Condition = checkItem.Condition,
                        Remarks = checkItem.Remarks,
                        Photos = string.Join(",", checkItem.Photos ?? new List<string>()) // Fixed: Join list to string
                    };

                    conditionCheck.Items.Add(conditionItem);

                    // Update return item condition
                    returnItem.CheckedCondition = checkItem.Condition;
                    returnItem.AcceptedQuantity = checkItem.GoodQuantity;
                    returnItem.RejectedQuantity = checkItem.DamagedQuantity + checkItem.ExpiredQuantity;
                }

                await _unitOfWork.ConditionChecks.AddAsync(conditionCheck);

                // Update return status
                returnEntity.Status = ReturnStatus.ConditionChecked;
                returnEntity.ConditionCheckId = conditionCheck.Id; // Store the condition check ID

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error checking return condition");
                throw;
            }
        }

        // Approve Return
        public async Task<bool> ApproveReturnAsync(int returnId, ReturnApprovalDto approvalDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var returnEntity = await _unitOfWork.Returns.Query()
                    .Include(r => r.Items)
                    //.Include(r => r.ConditionCheck)
                    //.ThenInclude(cc => cc.Items)
                    .FirstOrDefaultAsync(r => r.Id == returnId);

                if (returnEntity == null)
                    throw new InvalidOperationException("Return not found");

                if (returnEntity.Status != ReturnStatus.ConditionChecked)
                    throw new InvalidOperationException("Return condition has not been checked");

                // Update approved quantities
                foreach (var item in returnEntity.Items)
                {
                    var approvedItem = approvalDto.ApprovedItems.FirstOrDefault(ai => ai.ItemId == item.ItemId);
                    if (approvedItem != null)
                    {
                        item.ApprovedQuantity = approvedItem.ApprovedQuantity;
                        item.ApprovalRemarks = approvedItem.Remarks;
                    }
                }

                returnEntity.Status = ReturnStatus.Approved;
                returnEntity.ApprovedBy = approvalDto.ApprovedBy;
                returnEntity.ApprovedDate = DateTime.Now;
                returnEntity.ApprovalRemarks = approvalDto.Remarks;

                // Update approval record
                await _approvalService.ApproveRequestAsync(new ApprovalActionDto
                {
                    EntityType = "Return",
                    EntityId = returnId,
                    ApprovedBy = approvalDto.ApprovedBy,
                    ApprovedDate = DateTime.Now,
                    Remarks = approvalDto.Remarks
                });

                await _unitOfWork.CompleteAsync();

                // Send notification
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = returnEntity.RequestedBy,
                    Title = "Return Request Approved",
                    Message = $"Your return request {returnEntity.ReturnNo} has been approved",
                    Type = "Approval",
                    CreatedAt = DateTime.Now
                });

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving return");
                throw;
            }
        }

        // Receive Return and Update Stock
        public async Task<bool> ReceiveReturnAsync(int returnId, ReceiveReturnDto receiveDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var returnEntity = await _unitOfWork.Returns.Query()
                    .Include(r => r.Items)
                    // REMOVED: .Include(r => r.ConditionCheck)
                    // REMOVED: .ThenInclude(cc => cc.Items)
                    .FirstOrDefaultAsync(r => r.Id == returnId);

                if (returnEntity == null)
                    throw new InvalidOperationException("Return not found");

                if (returnEntity.Status != ReturnStatus.Approved)
                    throw new InvalidOperationException("Return is not approved for receiving");

                // Load condition check separately
                ConditionCheck conditionCheck = null;
                if (returnEntity.ConditionCheckId.HasValue)
                {
                    conditionCheck = await _unitOfWork.ConditionChecks
                        .Query()
                        .Include(cc => cc.Items)
                        .FirstOrDefaultAsync(cc => cc.Id == returnEntity.ConditionCheckId.Value);
                }

                // Process each return item
                foreach (var item in returnEntity.Items.Where(i => i.ApprovedQuantity > 0))
                {
                    var receiveItem = receiveDto.ReceivedItems.FirstOrDefault(ri => ri.ItemId == item.ItemId);
                    if (receiveItem == null) continue;

                    item.ReceivedQuantity = receiveItem.ReceivedQuantity;
                    item.ReceivedDate = DateTime.Now;
                    item.ReceivedBy = receiveDto.ReceivedBy;

                    // Get condition check details - now using the separately loaded conditionCheck
                    var conditionItem = conditionCheck?.Items
                        .FirstOrDefault(ci => ci.ItemId == item.ItemId);

                    // Update stock based on condition
                    if (conditionItem != null)
                    {
                        // Restore good items to stock
                        if (conditionItem.GoodQuantity > 0)
                        {
                            await _stockService.ReceiveStockAsync(
                                returnEntity.ToStoreId,
                                item.ItemId,
                                conditionItem.GoodQuantity,
                                $"Return: {returnEntity.ReturnNo}",
                                receiveDto.ReceivedBy
                            );

                            // Create stock movement for good items
                            await CreateStockMovementAsync(
                                item.ItemId,
                                returnEntity.ToStoreId,
                                conditionItem.GoodQuantity,
                                StockMovementType.Return,
                                returnEntity.ReturnNo,
                                "Good Condition"
                            );
                        }

                        // Handle damaged items
                        if (conditionItem.DamagedQuantity > 0)
                        {
                            await CreateDamageRecordAsync(
                                item.ItemId,
                                returnEntity.ToStoreId,
                                conditionItem.DamagedQuantity,
                                returnEntity.ReturnNo,
                                "Returned Damaged"
                            );
                        }

                        // Handle expired items
                        if (conditionItem.ExpiredQuantity > 0)
                        {
                            await CreateExpiredRecordAsync(
                                item.ItemId,
                                returnEntity.ToStoreId,
                                conditionItem.ExpiredQuantity,
                                returnEntity.ReturnNo
                            );
                        }
                    }
                }

                returnEntity.Status = ReturnStatus.Completed;
                returnEntity.CompletedDate = DateTime.Now;
                returnEntity.ReceivedBy = receiveDto.ReceivedBy;
                returnEntity.ReceivedDate = DateTime.Now;
                returnEntity.ReceiptRemarks = receiveDto.Remarks;

                await _unitOfWork.CompleteAsync();

                // Generate return receipt
                var receipt = await GenerateReturnReceiptAsync(returnEntity);

                // Send notifications
                await SendReturnCompletionNotificationsAsync(returnEntity, receipt);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error receiving return");
                throw;
            }
        }

        // Helper Methods
        private async Task<decimal> GetPreviousReturnQuantityAsync(int? issueId, int itemId)
        {
            var previousReturns = await _unitOfWork.Returns.Query()
                .Include(r => r.Items)
                .Where(r => r.OriginalIssueId == issueId && r.Status == ReturnStatus.Completed)
                .SelectMany(r => r.Items)
                .Where(ri => ri.ItemId == itemId)
                .SumAsync(ri => ri.ReceivedQuantity ?? 0);

            return previousReturns;
        }

        private async Task CreateStockMovementAsync(int itemId, int storeId, decimal quantity,
            StockMovementType type, string referenceNo, string remarks)
        {
            var stockMovement = new StockMovement
            {
                ItemId = itemId,
                StoreId = storeId,
                MovementType = type.ToString(),
                Quantity = quantity,
                ReferenceType = "Return",
                ReferenceNo = referenceNo,
                MovementDate = DateTime.Now,
                Remarks = remarks
            };

            await _unitOfWork.StockMovements.AddAsync(stockMovement);
        }

        private async Task CreateDamageRecordAsync(int itemId, int storeId, decimal quantity,
            string referenceNo, string reason)
        {
            var damage = new DamageRecord
            {
                ItemId = itemId,
                StoreId = storeId,
                DamagedQuantity = quantity,
                DamageDate = DateTime.Now,
                DamageReason = reason,
                ReferenceType = "Return",
                ReferenceNo = referenceNo,
                Status = "Pending"
            };

            await _unitOfWork.DamageRecords.AddAsync(damage);
        }

        private async Task CreateExpiredRecordAsync(int itemId, int storeId, decimal quantity,
            string referenceNo)
        {
            var expired = new ExpiredRecord
            {
                ItemId = itemId,
                StoreId = storeId,
                ExpiredQuantity = quantity,
                ExpiryDate = DateTime.Now,
                ReferenceType = "Return",
                ReferenceNo = referenceNo,
                Status = "Pending"
            };

            await _unitOfWork.ExpiredRecords.AddAsync(expired);
        }

        public async Task<string> GenerateReturnNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"RET-{date:yyyyMM}";

            var lastReturn = await _unitOfWork.Returns.Query()
                .Where(r => r.ReturnNo.StartsWith(prefix))
                .OrderByDescending(r => r.ReturnNo)
                .FirstOrDefaultAsync();

            if (lastReturn == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastReturn.ReturnNo.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }

        private async Task<ReturnReceiptDto> GenerateReturnReceiptAsync(Return returnEntity)
        {
            // Generate receipt implementation
            var receipt = new ReturnReceiptDto
            {
                ReceiptNo = $"RR-{DateTime.Now:yyyyMMdd}-{returnEntity.Id:D4}",
                ReturnNo = returnEntity.ReturnNo,
                ReturnDate = returnEntity.ReturnDate,
                CompletedDate = returnEntity.CompletedDate.Value,
                Items = returnEntity.Items.Select(i => new ReturnReceiptItemDto
                {
                    ItemName = i.Item?.Name,
                    ReturnQuantity = i.ReturnQuantity,
                    ReceivedQuantity = i.ReceivedQuantity ?? 0,
                    Condition = i.CheckedCondition
                }).ToList()
            };

            return receipt;
        }

        private async Task SendReturnCompletionNotificationsAsync(Return returnEntity, ReturnReceiptDto receipt)
        {
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                UserId = returnEntity.RequestedBy,
                Title = "Return Completed",
                Message = $"Return {returnEntity.ReturnNo} has been completed. Receipt: {receipt.ReceiptNo}",
                Type = "Completion",
                CreatedAt = DateTime.Now
            });
        }

        // Interface implementations...
        public async Task<IEnumerable<ReturnDto>> GetAllReturnsAsync()
        {
            _logger.LogInformation("📋 GetAllReturnsAsync called");

            // First get all returns without navigation properties
            var returns = await _unitOfWork.Returns.Query()
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            _logger.LogInformation($"📋 Found {returns.Count} returns in database (without navigation)");

            // Then load navigation properties separately
            foreach (var returnEntity in returns)
            {
                if (returnEntity.ItemId > 0)
                {
                    returnEntity.Item = await _unitOfWork.Items.GetByIdAsync(returnEntity.ItemId);
                }
                if (returnEntity.StoreId.HasValue && returnEntity.StoreId.Value > 0)
                {
                    returnEntity.Store = await _unitOfWork.Stores.GetByIdAsync(returnEntity.StoreId.Value);
                }
            }

            var dtos = returns.Select(r => MapToDto(r)).ToList();

            _logger.LogInformation($"📋 Mapped to {dtos.Count} DTOs");

            return dtos;
        }

        public async Task<ReturnDto> GetReturnByIdAsync(int id)
        {
            var returnEntity = await _unitOfWork.Returns.Query()
                .Include(r => r.Items)
                .ThenInclude(ri => ri.Item)
                .FirstOrDefaultAsync(r => r.Id == id);

            return returnEntity != null ? MapToDto(returnEntity) : null;
        }

        private ReturnDto MapToDto(Return returnEntity)
        {
            return new ReturnDto
            {
                Id = returnEntity.Id,
                ReturnNo = returnEntity.ReturnNo,
                OriginalIssueId = returnEntity.OriginalIssueId,
                OriginalIssueNo = returnEntity.OriginalIssueNo,
                ReturnDate = returnEntity.ReturnDate,
                ReturnType = returnEntity.ReturnType,
                Reason = returnEntity.Reason,
                Status = returnEntity.Status,
                ReturnedBy = returnEntity.ReturnedBy,
                ReturnedByType = returnEntity.ReturnedByType,
                RequestedBy = returnEntity.RequestedBy,

                // Map simple return properties (single item)
                ItemId = returnEntity.ItemId,
                ItemName = returnEntity.Item?.Name,
                StoreId = returnEntity.StoreId,
                StoreName = returnEntity.Store?.Name,
                Quantity = returnEntity.Quantity,
                IsRestocked = returnEntity.IsRestocked,
                Condition = returnEntity.Remarks?.Contains("Condition:") == true
                    ? returnEntity.Remarks.Split("Condition:")[1].Split('.')[0].Trim()
                    : string.Empty,
                Remarks = returnEntity.Remarks,

                // Map complex return properties (multiple items)
                Items = returnEntity.Items?.Select(i => new ReturnItemDto
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    ItemName = i.Item?.Name,
                    ReturnQuantity = i.ReturnQuantity,
                    Condition = i.Condition,
                    ReturnReason = i.ReturnReason
                }).ToList(),

                // Audit fields
                CreatedBy = returnEntity.CreatedBy,
                CreatedAt = returnEntity.CreatedAt,
                UpdatedBy = returnEntity.UpdatedBy,
                UpdatedAt = returnEntity.UpdatedAt
            };
        }



        public async Task<ReturnDto> CreateReturnAsync(ReturnDto returnDto)
        {
            try
            {
                _logger.LogInformation("🔵 CreateReturnAsync START");
                await _unitOfWork.BeginTransactionAsync();
                _logger.LogInformation("🔵 Transaction started");

                // Validation
                if (returnDto.ItemId <= 0)
                    throw new InvalidOperationException("অনুগ্রহ করে একটি Item নির্বাচন করুন। (Please select an item.)");

                if (!returnDto.StoreId.HasValue || returnDto.StoreId <= 0)
                    throw new InvalidOperationException("অনুগ্রহ করে Store নির্বাচন করুন। (Please select a store.)");

                if (returnDto.Quantity <= 0)
                    throw new InvalidOperationException("ফেরত পরিমাণ অবশ্যই শূন্যের চেয়ে বেশি হতে হবে। (Return quantity must be greater than zero.)");

                if (string.IsNullOrEmpty(returnDto.ReturnedBy))
                    throw new InvalidOperationException("ফেরতদাতার নাম লিখুন। (Please enter who is returning the item.)");

                if (string.IsNullOrEmpty(returnDto.Condition))
                    throw new InvalidOperationException("আইটেমের অবস্থা নির্বাচন করুন। (Please select item condition.)");

                // Verify item exists
                var item = await _unitOfWork.Items.GetByIdAsync(returnDto.ItemId);
                if (item == null)
                    throw new InvalidOperationException("নির্বাচিত Item খুঁজে পাওয়া যায়নি। (Selected item not found.)");

                // Verify store exists
                var store = await _unitOfWork.Stores.GetByIdAsync(returnDto.StoreId.Value);
                if (store == null)
                    throw new InvalidOperationException("নির্বাচিত Store খুঁজে পাওয়া যায়নি। (Selected store not found.)");

                // Set default ReturnType if not specified
                if (string.IsNullOrEmpty(returnDto.ReturnType))
                {
                    returnDto.ReturnType = "Normal";
                }

                // Create return record
                var returnItem = new Return
                {
                    ReturnNo = await GenerateReturnNoAsync(),
                    ReturnDate = returnDto.ReturnDate,
                    ReturnedBy = returnDto.ReturnedBy,
                    ReturnedByType = returnDto.ReturnedByType,
                    Reason = returnDto.Reason,
                    ReturnType = returnDto.ReturnType,
                    ItemId = returnDto.ItemId,
                    StoreId = returnDto.StoreId.Value,
                    ToStoreId = returnDto.StoreId.Value, // Store where item is being returned to
                    Quantity = returnDto.Quantity,
                    IsRestocked = returnDto.IsRestocked,
                    Remarks = $"Condition: {returnDto.Condition}. {returnDto.Remarks}".Trim(),
                    OriginalIssueId = returnDto.OriginalIssueId,
                    OriginalIssueNo = returnDto.OriginalIssueNo,
                    Status = ReturnStatus.Pending,
                    RequestedBy = returnDto.CreatedBy ?? "System",
                    RequestedDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = returnDto.CreatedBy ?? "System",
                    IsActive = true // Explicitly set IsActive
                };

                await _unitOfWork.Returns.AddAsync(returnItem);
                _logger.LogInformation("🔵 Return added to context, calling CompleteAsync...");
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"🔵 First CompleteAsync done. Return ID: {returnItem.Id}, ReturnNo: {returnItem.ReturnNo}");

                // Create approval request for Damaged/Expired returns
                if (returnDto.ReturnType == "Damaged" || returnDto.ReturnType == "Expired")
                {
                    try
                    {
                        await _approvalService.CreateApprovalRequestAsync(new ApprovalRequestDto
                        {
                            EntityType = "Return",
                            EntityId = returnItem.Id,
                            RequestedBy = returnDto.CreatedBy ?? "System",
                            RequestedDate = DateTime.Now,
                            Description = $"Return Request: {returnItem.ReturnNo} - {returnDto.ReturnType} ({item.Name})",
                            Priority = returnDto.ReturnType == "Damaged" ? "High" : "Normal",
                            Status = "Pending"
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not create approval request for return {ReturnNo}", returnItem.ReturnNo);
                        // Don't fail the return creation if approval fails
                    }
                }

                // Update store stock ONLY if item is good condition AND auto-restock is enabled
                if (returnDto.IsRestocked && returnDto.Condition == "Good")
                {
                    var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                        si => si.StoreId == returnDto.StoreId && si.ItemId == returnDto.ItemId);

                    if (storeItem != null)
                    {
                        storeItem.Quantity += returnDto.Quantity;
                        storeItem.UpdatedAt = DateTime.Now;
                        storeItem.UpdatedBy = returnDto.CreatedBy ?? "System";
                        _unitOfWork.StoreItems.Update(storeItem);
                    }
                    else
                    {
                        storeItem = new StoreItem
                        {
                            StoreId = returnDto.StoreId.Value,
                            ItemId = returnDto.ItemId,
                            Quantity = returnDto.Quantity,
                            Status = Domain.Enums.ItemStatus.Available,
                            CreatedAt = DateTime.Now,
                            CreatedBy = returnDto.CreatedBy ?? "System"
                        };
                        await _unitOfWork.StoreItems.AddAsync(storeItem);
                    }
                }

                // Create notification
                try
                {
                    var notificationMessage = returnDto.ReturnType == "Damaged" || returnDto.ReturnType == "Expired"
                        ? $"Return {returnItem.ReturnNo} created and sent for approval"
                        : $"Return {returnItem.ReturnNo} created successfully";

                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        Title = "Return Created",
                        Message = notificationMessage,
                        Type = "Return",
                        CreatedAt = DateTime.Now
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not create notification for return {ReturnNo}", returnItem.ReturnNo);
                }

                _logger.LogInformation("🔵 Calling second CompleteAsync...");
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("🔵 Second CompleteAsync done, calling CommitTransactionAsync...");
                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("🔵 Transaction COMMITTED successfully!");

                _logger.LogInformation($"✅ Return {returnItem.ReturnNo} created successfully with ID: {returnItem.Id}, IsActive: {returnItem.IsActive}");

                returnDto.Id = returnItem.Id;
                returnDto.ReturnNo = returnItem.ReturnNo;
                returnDto.Status = returnItem.Status;
                return returnDto;
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ EXCEPTION in CreateReturnAsync: {Message}", ex.Message);
                _logger.LogError("❌ Stack trace: {StackTrace}", ex.StackTrace);
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError("❌ Transaction ROLLED BACK");
                throw; // Re-throw to let controller handle user-friendly message
            }
        }

        public async Task<IEnumerable<ReturnDto>> GetReturnsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var returns = await GetAllReturnsAsync();
            return returns.Where(r => r.ReturnDate >= fromDate && r.ReturnDate <= toDate);
        }

        public async Task<IEnumerable<ReturnDto>> GetReturnsBySourceAsync(string returnedBy)
        {
            var returns = await GetAllReturnsAsync();
            return returns.Where(r => r.ReturnedBy.Contains(returnedBy, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<ReturnDto>> GetReturnsByTypeAsync(string returnedByType)
        {
            var returns = await GetAllReturnsAsync();
            return returns.Where(r => r.ReturnedByType.Equals(returnedByType, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<ReturnDto>> GetRestockedReturnsAsync()
        {
            var returns = await GetAllReturnsAsync();
            return returns.Where(r => r.IsRestocked);
        }

        public async Task<IEnumerable<ReturnDto>> GetPendingRestockReturnsAsync()
        {
            var returns = await GetAllReturnsAsync();
            return returns.Where(r => !r.IsRestocked);
        }

        public async Task<int> GetReturnCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var returns = await _unitOfWork.Returns.GetAllAsync();
            var query = returns.Where(r => r.IsActive);

            if (fromDate.HasValue)
                query = query.Where(r => r.ReturnDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.ReturnDate <= toDate.Value);

            return query.Count();
        }

        public async Task<bool> ReturnNoExistsAsync(string returnNo)
        {
            var returnItem = await _unitOfWork.Returns.FirstOrDefaultAsync(r => r.ReturnNo == returnNo);
            return returnItem != null;
        }

        public async Task<IEnumerable<ReturnDto>> GetPagedReturnsAsync(int pageNumber, int pageSize)
        {
            var allReturns = await GetAllReturnsAsync();
            return allReturns
                .OrderByDescending(r => r.ReturnDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task MarkAsRestockedAsync(int returnId)
        {
            var returnItem = await _unitOfWork.Returns.GetByIdAsync(returnId);
            if (returnItem != null && !returnItem.IsRestocked)
            {
                returnItem.IsRestocked = true;
                returnItem.UpdatedAt = DateTime.Now;
                returnItem.UpdatedBy = "System";
                _unitOfWork.Returns.Update(returnItem);

                // Update store stock
                var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                    si => si.StoreId == returnItem.StoreId && si.ItemId == returnItem.ItemId);

                if (storeItem != null)
                {
                    storeItem.Quantity += returnItem.Quantity;
                    _unitOfWork.StoreItems.Update(storeItem);
                }
                else
                {
                    storeItem = new StoreItem
                    {
                        StoreId = returnItem.StoreId,
                        ItemId = returnItem.ItemId,
                        Quantity = returnItem.Quantity,
                        Status = Domain.Enums.ItemStatus.Available,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System"
                    };
                    await _unitOfWork.StoreItems.AddAsync(storeItem);
                }

                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<ReturnDto> CreateReturnLinkedToIssueAsync(int originalIssueId, ReturnDto returnDto)
        {
            // Validate original issue exists
            var originalIssue = await _unitOfWork.Issues.GetByIdAsync(originalIssueId);
            if (originalIssue == null || !originalIssue.IsActive)
                throw new InvalidOperationException("Original issue not found");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Create return linked to issue
                var stockReturn = new StockReturn
                {
                    OriginalIssueId = originalIssueId,
                    ReturnNumber = await GenerateReturnNoAsync(),
                    ItemId = returnDto.ItemId,
                    Quantity = (int)returnDto.Quantity,
                    Reason = returnDto.Reason,
                    Condition = returnDto.Condition,
                    RestockApproved = false,
                    ReturnDate = DateTime.Now,
                    ReturnedBy = returnDto.ReturnedBy,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.GetCurrentUserName()
                };

                await _unitOfWork.StockReturns.AddAsync(stockReturn);

                // If condition is good and approved, restock
                if (returnDto.Condition == "Good" && returnDto.RestockApproved)
                {
                    var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(si =>
                        si.ItemId == returnDto.ItemId && si.StoreId == returnDto.StoreId);

                    if (storeItem != null)
                    {
                        storeItem.Quantity += returnDto.Quantity;
                        storeItem.UpdatedAt = DateTime.Now;
                        storeItem.UpdatedBy = _userContext.GetCurrentUserName();
                        _unitOfWork.StoreItems.Update(storeItem);
                    }

                    stockReturn.RestockApproved = true;
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                returnDto.Id = stockReturn.Id;
                returnDto.ReturnNo = stockReturn.ReturnNumber;
                return returnDto;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<IssueDto>> GetRecentCompletedIssuesAsync(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            var issues = await _unitOfWork.Issues.FindAsync(i =>
                i.IsActive && i.IssueDate >= cutoffDate);

            var issueDtos = new List<IssueDto>();
            foreach (var issue in issues.OrderByDescending(i => i.IssueDate))
            {
                issueDtos.Add(await GetIssueByIdAsync(issue.Id));
            }

            return issueDtos;
        }

        private async Task<IssueDto> GetIssueByIdAsync(int issueId)
        {
            var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
            if (issue == null) return null;

            // Manual mapping (AutoMapper removed for compatibility)
            return new IssueDto
            {
                Id = issue.Id,
                IssueNo = issue.IssueNo,
                IssueDate = issue.IssueDate,
                IssuedToName = issue.IssuedTo, // Map IssuedTo from entity to IssuedToName in DTO
                Purpose = issue.Purpose,
                CreatedBy = issue.CreatedBy,
                CreatedAt = issue.CreatedAt
            };
        }

        public async Task<IEnumerable<ReturnDto>> GetReturnsByIssueAsync(int issueId)
        {
            var returns = await _unitOfWork.StockReturns.FindAsync(r => r.OriginalIssueId == issueId && r.IsActive);
            var returnDtos = new List<ReturnDto>();

            foreach (var stockReturn in returns)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(stockReturn.ItemId);
                // Remove the .HasValue and .Value - just use OriginalIssueId directly
                var issue = await _unitOfWork.Issues.GetByIdAsync(stockReturn.OriginalIssueId);

                returnDtos.Add(new ReturnDto
                {
                    Id = stockReturn.Id,
                    ReturnNo = stockReturn.ReturnNumber,
                    ReturnDate = stockReturn.ReturnDate,
                    ReturnedBy = stockReturn.ReturnedBy,
                    ItemId = stockReturn.ItemId,
                    ItemName = item?.Name,
                    Quantity = stockReturn.Quantity,
                    Reason = stockReturn.Reason,
                    Condition = stockReturn.Condition,
                    IsRestocked = stockReturn.RestockApproved,
                    RestockApproved = stockReturn.RestockApproved,
                    OriginalIssueId = stockReturn.OriginalIssueId,
                    OriginalIssueNo = issue?.IssueNo,
                    CreatedAt = stockReturn.CreatedAt,
                    CreatedBy = stockReturn.CreatedBy
                });
            }

            return returnDtos.OrderByDescending(r => r.ReturnDate);
        }

        public async Task<ServiceResult> AuthorizeReturnAsync(int returnId, bool isApproved, string reason = null)
        {
            try
            {
                var returnItem = await _unitOfWork.Returns.GetByIdAsync(returnId);
                if (returnItem == null)
                    return ServiceResult.Failure("Return not found");

                // Return entity doesn't have Status, ApprovedBy, etc. - use what's available
                returnItem.RestockApprovalRequired = !isApproved;
                returnItem.Remarks = reason;
                returnItem.UpdatedAt = DateTime.Now;
                returnItem.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Returns.Update(returnItem);
                await _unitOfWork.CompleteAsync();

                // Send notification
                var notification = new NotificationDto
                {
                    Title = $"Return {(isApproved ? "Authorized" : "Rejected")}",
                    Message = $"Return {returnItem.ReturnNo} has been {(isApproved ? "authorized" : "rejected")}",
                    Type = isApproved ? "success" : "warning",
                    UserId = returnItem.CreatedBy
                };
                await _notificationService.CreateNotificationAsync(notification);

                return ServiceResult.SuccessResult($"Return {(isApproved ? "authorized" : "rejected")} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authorizing return");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ProcessReturnRestockingAsync(int returnId, ReturnRestockDto restockDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var returnItem = await _unitOfWork.Returns.GetByIdAsync(returnId);
                if (returnItem == null)
                    return ServiceResult.Failure("Return not found");

                // Get original issue ID from receive
                var receive = await _unitOfWork.Receives.GetByIdAsync(returnItem.ReceiveId);
                if (receive == null)
                    return ServiceResult.Failure("Original receive not found");

                var issueId = receive.OriginalIssueId;
                var issueItems = await _unitOfWork.IssueItems
                    .FindAsync(ii => ii.IssueId == issueId);

                foreach (var restockItem in restockDto.Items)
                {
                    var issueItem = issueItems.FirstOrDefault(ii => ii.ItemId == restockItem.ItemId);
                    if (issueItem == null)
                        continue;

                    if (restockItem.RestockQuantity > 0)
                    {
                        // Add back to store
                        var storeItem = await _unitOfWork.StoreItems
                            .FirstOrDefaultAsync(si => si.StoreId == issueItem.StoreId && si.ItemId == restockItem.ItemId);

                        if (storeItem != null)
                        {
                            storeItem.Quantity += restockItem.RestockQuantity;
                            storeItem.UpdatedAt = DateTime.Now;
                            storeItem.UpdatedBy = _userContext.CurrentUserName;
                            _unitOfWork.StoreItems.Update(storeItem);
                        }

                        // Create stock movement
                        var movement = new StockMovement
                        {
                            StoreId = issueItem.StoreId,
                            ItemId = restockItem.ItemId,
                            MovementType = "IN",
                            MovementDate = DateTime.Now,
                            Quantity = restockItem.RestockQuantity,
                            ReferenceType = "RETURN",
                            ReferenceNo = returnItem.ReturnNo,
                            Remarks = $"Return from receive {receive.ReceiveNo}",
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userContext.CurrentUserName,
                            IsActive = true
                        };
                        await _unitOfWork.StockMovements.AddAsync(movement);
                    }

                    // Handle damaged items
                    if (restockItem.DamagedQuantity > 0)
                    {
                        var damageReport = new DamageReportDto
                        {
                            ReturnId = returnId,
                            ItemId = restockItem.ItemId,
                            Quantity = restockItem.DamagedQuantity,
                            DamageType = restockItem.DamageType,
                            Description = restockItem.DamageDescription,
                            EstimatedValue = restockItem.EstimatedValue
                        };

                        await CreateDamageReportAsync(returnId, damageReport);
                    }
                }

                returnItem.UpdatedAt = DateTime.Now;
                returnItem.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Returns.Update(returnItem);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ServiceResult.SuccessResult("Return restocked successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error processing return restocking");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> CreateDamageReportAsync(int returnId, DamageReportDto damageDto)
        {
            try
            {
                var damage = new Damage
                {
                    ItemId = damageDto.ItemId,
                    Quantity = damageDto.Quantity,
                    DamageType = damageDto.DamageType,
                    Description = damageDto.Description,
                    ReportedBy = _userContext.CurrentUserName,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.Damages.AddAsync(damage);
                await _unitOfWork.CompleteAsync();

                // If damage is significant, create write-off request
                if (damageDto.EstimatedValue > 1000) // Threshold for auto write-off
                {
                    var writeOffDto = new WriteOffDto
                    {
                        Reason = $"Damaged items from return {returnId}",
                        TotalValue = (decimal)damageDto.EstimatedValue,
                        Items = new List<WriteOffItemDto>
                    {
                        new WriteOffItemDto
                        {
                            ItemId = damageDto.ItemId,
                            Quantity = (decimal)damageDto.Quantity,
                            UnitPrice = damageDto.EstimatedValue / damageDto.Quantity,
                            Reason = damageDto.Description
                        }
                    }
                    };

                    await _writeOffService.CreateWriteOffAsync(writeOffDto);
                }

                return ServiceResult.SuccessResult("Damage report created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating damage report");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a return request with comprehensive validation
        /// </summary>
        public async Task<bool> DeleteReturnAsync(int returnId, string deletedBy)
        {
            var returnRequest = await _unitOfWork.Returns
                .Query()
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == returnId);

            if (returnRequest == null)
                throw new InvalidOperationException("Return request not found");

            // 1. Status Check - Only allow deletion of Pending, Rejected, or Cancelled returns
            if (returnRequest.Status != ReturnStatus.Pending &&
                returnRequest.Status != ReturnStatus.Rejected &&
                returnRequest.Status != ReturnStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    "Only Pending, Rejected, or Cancelled returns can be deleted. " +
                    $"Current status: {returnRequest.Status}");
            }

            // 2. CRITICAL: Check StockMovement records
            var hasStockMovements = await _unitOfWork.StockMovements
                .Query()
                .AnyAsync(sm => sm.ReferenceType == "Return" && sm.ReferenceId == returnId);

            if (hasStockMovements)
            {
                throw new InvalidOperationException(
                    "Cannot delete - Stock movement records exist for this return. " +
                    "This return has already impacted inventory. Contact administrator for reversal.");
            }

            // 3. Check if return has been restocked
            if (returnRequest.IsRestocked)
            {
                throw new InvalidOperationException(
                    "Cannot delete - Return has already been restocked into inventory.");
            }

            // 4. Check if already processed (approved, in-process, completed, etc.)
            if (returnRequest.Status == ReturnStatus.Approved ||
                returnRequest.Status == ReturnStatus.ConditionChecked ||
                returnRequest.Status == ReturnStatus.InProcess ||
                returnRequest.Status == ReturnStatus.Completed)
            {
                throw new InvalidOperationException(
                    "Cannot delete processed returns. This violates audit trail integrity.");
            }

            // 5. Soft Delete (NEVER hard delete transactional data!)
            returnRequest.IsActive = false;
            returnRequest.UpdatedBy = deletedBy;
            returnRequest.UpdatedAt = DateTime.Now;

            _unitOfWork.Returns.Update(returnRequest);
            await _unitOfWork.CompleteAsync();

            // 6. Log the deletion
            await _activityLogService.LogActivityAsync(
                "Return",
                returnRequest.Id,
                "Delete",
                $"Return {returnRequest.ReturnNo} deleted by {deletedBy}. " +
                $"Reason: {returnRequest.Reason}, Items: {returnRequest.Items?.Count ?? 0}",
                deletedBy
            );

            return true;
        }
    }
}