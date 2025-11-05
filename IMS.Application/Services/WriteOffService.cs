using IMS.Application.DTOs;
using IMS.Application.Exceptions;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class WriteOffService : IWriteOffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<WriteOffService> _logger;
        private readonly IApprovalService _approvalService;
        private readonly IStockService _stockService;

        public WriteOffService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            IActivityLogService activityLogService,
            INotificationService notificationService,
            IApprovalService approvalService,
            IFileService fileService,
            UserManager<User> userManager,
            IStockService stockService,
            ILogger<WriteOffService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _approvalService = approvalService;
            _stockService = stockService;
        }

        // Find and Report Damaged/Expired Items
        public async Task<DamageReportDto> ReportDamagedItemsAsync(DamageReportDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate stock availability
                foreach (var item in dto.Items)
                {
                    var currentStock = await _stockService.GetAvailableStockAsync(dto.StoreId, item.ItemId);
                    if (currentStock < item.DamagedQuantity)
                    {
                        throw new InvalidOperationException($"Damaged quantity exceeds available stock for item {item.ItemName}");
                    }
                }

                // Create damage report
                var damageReport = new DamageReport
                {
                    ReportNo = await GenerateDamageReportNoAsync(),
                    StoreId = dto.StoreId,
                    ReportDate = dto.ReportDate,
                    ReportedBy = dto.ReportedBy,
                    Status = DamageStatus.Reported,
                    TotalValue = 0,
                    Items = new List<DamageReportItem>()
                };

                foreach (var itemDto in dto.Items)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                    var itemValue = item.UnitPrice * itemDto.DamagedQuantity;

                    damageReport.Items.Add(new DamageReportItem
                    {
                        ItemId = itemDto.ItemId,
                        DamagedQuantity = itemDto.DamagedQuantity,
                        DamageType = itemDto.DamageType, // Physical, Water, Fire, Expired, Contaminated
                        DamageDate = itemDto.DamageDate,
                        DiscoveredDate = itemDto.DiscoveredDate,
                        DamageDescription = itemDto.DamageDescription,
                        EstimatedValue = (decimal)itemValue,
                        PhotoUrls = string.Join(",", itemDto.PhotoUrls ?? new List<string>()),
                        BatchNo = itemDto.BatchNo,
                        Remarks = itemDto.Remarks
                    });

                    damageReport.TotalValue += (decimal)itemValue;
                }

                await _unitOfWork.DamageReports.AddAsync(damageReport);
                await _unitOfWork.CompleteAsync();

                // Create write-off request if total value exceeds threshold
                if (damageReport.TotalValue > 10000) // Configurable threshold
                {
                    await CreateWriteOffRequestAsync(damageReport);
                }

                // Send notification
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    Title = "Damage Report Created",
                    Message = $"Damage report {damageReport.ReportNo} has been created for review",
                    Type = "DamageReport",
                    Priority = "High",
                    CreatedAt = DateTime.Now
                });

                await _unitOfWork.CommitTransactionAsync();

                dto.Id = damageReport.Id;
                dto.Status = damageReport.Status;
                dto.TotalValue = damageReport.TotalValue;
                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error reporting damaged items");
                throw;
            }
        }

        // Create Write-off Request
        public async Task<WriteOffRequestDto> CreateWriteOffRequestAsync(DamageReport damageReport)
        {
            try
            {
                var writeOffRequest = new WriteOffRequest
                {
                    RequestNo = await GenerateWriteOffRequestNoAsync(),
                    DamageReportId = damageReport.Id,
                    DamageReportNo = damageReport.ReportNo,
                    StoreId = damageReport.StoreId,
                    RequestDate = DateTime.Now,
                    RequestedBy = damageReport.ReportedBy,
                    TotalValue = damageReport.TotalValue,
                    Status = WriteOffStatus.Pending,
                    Justification = "Items damaged beyond repair or expired",
                    Items = new List<WriteOffItem>()
                };

                foreach (var damageItem in damageReport.Items)
                {
                    writeOffRequest.Items.Add(new WriteOffItem
                    {
                        ItemId = damageItem.ItemId,
                        Quantity = damageItem.DamagedQuantity,
                        Value = damageItem.EstimatedValue,
                        Reason = damageItem.DamageType,
                        BatchNo = damageItem.BatchNo
                    });
                }

                await _unitOfWork.WriteOffRequests.AddAsync(writeOffRequest);
                await _unitOfWork.CompleteAsync();

                // Create approval workflow based on value
                var approvalLevels = GetApprovalLevels(writeOffRequest.TotalValue);

                foreach (var level in approvalLevels)
                {
                    await _approvalService.CreateApprovalRequestAsync(new ApprovalRequestDto
                    {
                        EntityType = "WriteOff",
                        EntityId = writeOffRequest.Id,
                        RequestedBy = writeOffRequest.RequestedBy,
                        RequestedDate = DateTime.Now,
                        Amount = writeOffRequest.TotalValue,
                        ApprovalLevel = level,
                        Description = $"Write-off Request: {writeOffRequest.RequestNo}"
                    });
                }

                return new WriteOffRequestDto
                {
                    Id = writeOffRequest.Id,
                    RequestNo = writeOffRequest.RequestNo,
                    Status = writeOffRequest.Status,
                    TotalValue = writeOffRequest.TotalValue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating write-off request");
                throw;
            }
        }

        // Get Approval from Authority
        public async Task<bool> ApproveWriteOffAsync(int writeOffId, WriteOffApprovalDto approvalDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var writeOffRequest = await _unitOfWork.WriteOffRequests.Query()
                    .Include(w => w.Items)
                    .Include(w => w.DamageReport)
                    .FirstOrDefaultAsync(w => w.Id == writeOffId);

                if (writeOffRequest == null)
                    throw new InvalidOperationException("Write-off request not found");

                if (writeOffRequest.Status != WriteOffStatus.Pending)
                    throw new InvalidOperationException("Write-off request is not pending");

                // Check if all required approval levels are complete
                var requiredLevels = GetApprovalLevels(writeOffRequest.TotalValue);
                var completedApprovals = await _approvalService.GetCompletedApprovalsAsync("WriteOff", writeOffId);

                if (completedApprovals.Count < requiredLevels.Count)
                {
                    // Record this approval
                    await _approvalService.ApproveRequestAsync(new ApprovalActionDto
                    {
                        EntityType = "WriteOff",
                        EntityId = writeOffId,
                        ApprovedBy = approvalDto.ApprovedBy,
                        ApprovedDate = DateTime.Now,
                        ApprovalLevel = approvalDto.ApprovalLevel,
                        Remarks = approvalDto.Remarks
                    });

                    // Check if all approvals are now complete
                    completedApprovals = await _approvalService.GetCompletedApprovalsAsync("WriteOff", writeOffId);

                    if (completedApprovals.Count < requiredLevels.Count)
                    {
                        // Still waiting for more approvals
                        await _unitOfWork.CommitTransactionAsync();
                        return true;
                    }
                }

                // All approvals complete - proceed with write-off
                writeOffRequest.Status = WriteOffStatus.Approved;
                writeOffRequest.ApprovedBy = approvalDto.ApprovedBy;
                writeOffRequest.ApprovedDate = DateTime.Now;
                writeOffRequest.ApprovalRemarks = approvalDto.Remarks;
                writeOffRequest.ApprovalReference = approvalDto.ApprovalReference;

                // Update damage report status
                if (writeOffRequest.DamageReport != null)
                {
                    writeOffRequest.DamageReport.Status = DamageStatus.Approved;
                }

                await _unitOfWork.CompleteAsync();

                // Send notification
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = writeOffRequest.RequestedBy,
                    Title = "Write-off Request Approved",
                    Message = $"Write-off request {writeOffRequest.RequestNo} has been approved",
                    Type = "Approval",
                    CreatedAt = DateTime.Now
                });

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving write-off");
                throw;
            }
        }

        // Remove from Stock and Dispose
        public async Task<bool> ExecuteWriteOffAsync(int writeOffId, WriteOffExecutionDto executionDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var writeOffRequest = await _unitOfWork.WriteOffRequests.Query()
                    .Include(w => w.Items)
                    .FirstOrDefaultAsync(w => w.Id == writeOffId);

                if (writeOffRequest == null)
                    throw new InvalidOperationException("Write-off request not found");

                if (writeOffRequest.Status != WriteOffStatus.Approved)
                    throw new InvalidOperationException("Write-off request is not approved");

                // Process each item for write-off
                foreach (var item in writeOffRequest.Items)
                {
                    // Remove from stock
                    await _stockService.WriteOffStockAsync(
                        writeOffRequest.StoreId,
                        item.ItemId,
                        item.Quantity,
                        $"Write-off: {writeOffRequest.RequestNo}",
                        executionDto.ExecutedBy
                    );

                    // Create stock movement record
                    var stockMovement = new StockMovement
                    {
                        ItemId = item.ItemId,
                        StoreId = writeOffRequest.StoreId,
                        MovementType = StockMovementType.WriteOff.ToString(),
                        Quantity = -item.Quantity, // Negative for removal
                        ReferenceType = "WriteOff",
                        ReferenceNo = writeOffRequest.RequestNo,
                        MovementDate = DateTime.Now,
                        CreatedBy = executionDto.ExecutedBy,
                        Remarks = item.Reason
                    };

                    await _unitOfWork.StockMovements.AddAsync(stockMovement);

                    // Create disposal record
                    var disposal = new DisposalRecord
                    {
                        WriteOffId = writeOffId,
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                        DisposalMethod = executionDto.DisposalMethod, // Destroy, Recycle, Donate, Auction
                        DisposalDate = executionDto.DisposalDate,
                        DisposalLocation = executionDto.DisposalLocation,
                        DisposalCompany = executionDto.DisposalCompany,
                        DisposalCertificateNo = executionDto.CertificateNo,
                        DisposedBy = executionDto.ExecutedBy,
                        WitnessedBy = executionDto.WitnessedBy,
                        PhotoUrls = string.Join(",", executionDto.PhotoUrls ?? new List<string>()),
                        Remarks = executionDto.Remarks
                    };

                    await _unitOfWork.DisposalRecords.AddAsync(disposal);
                }

                // Update write-off status
                writeOffRequest.Status = WriteOffStatus.Executed;
                writeOffRequest.ExecutedBy = executionDto.ExecutedBy;
                writeOffRequest.ExecutedDate = DateTime.Now;
                writeOffRequest.DisposalMethod = executionDto.DisposalMethod;
                writeOffRequest.DisposalRemarks = executionDto.Remarks;

                await _unitOfWork.CompleteAsync();

                // Generate disposal certificate
                var certificate = await GenerateDisposalCertificateAsync(writeOffRequest, executionDto);

                // Send notifications
                await SendWriteOffCompletionNotificationsAsync(writeOffRequest, certificate);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error executing write-off");
                throw;
            }
        }

        // Monitor Expiry Dates
        public async Task<IEnumerable<ExpiryAlertDto>> GetExpiryAlertsAsync(int storeId, int daysBeforeExpiry = 30)
        {
            try
            {
                var expiryDate = DateTime.Now.AddDays(daysBeforeExpiry);

                var expiringItems = await _unitOfWork.BatchTrackings.Query()
                    .Include(bt => bt.Item)
                    .Where(bt => bt.StoreId == storeId &&
                                bt.ExpiryDate != null &&
                                bt.ExpiryDate <= expiryDate &&
                                bt.RemainingQuantity > 0)
                    .Select(bt => new ExpiryAlertDto
                    {
                        ItemId = bt.ItemId,
                        ItemName = bt.Item.Name,
                        ItemCode = bt.Item.ItemCode,
                        BatchNo = bt.BatchNumber,
                        ExpiryDate = bt.ExpiryDate.Value,
                        DaysToExpiry = (bt.ExpiryDate.Value - DateTime.Now).Days,
                        Quantity = bt.RemainingQuantity,
                        EstimatedValue = bt.RemainingQuantity * bt.Item.UnitPrice,
                        AlertLevel = (bt.ExpiryDate.Value - DateTime.Now).Days <= 7 ? "Critical" :
                                    (bt.ExpiryDate.Value - DateTime.Now).Days <= 15 ? "High" : "Medium"
                    })
                    .OrderBy(ea => ea.ExpiryDate)
                    .ToListAsync();

                // Send alerts for critical items
                foreach (var item in expiringItems.Where(i => i.AlertLevel == "Critical"))
                {
                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        Title = "Critical Expiry Alert",
                        Message = $"Item {item.ItemName} (Batch: {item.BatchNo}) expires in {item.DaysToExpiry} days",
                        Type = "ExpiryAlert",
                        Priority = "Critical",
                        CreatedAt = DateTime.Now
                    });
                }

                return expiringItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiry alerts");
                throw;
            }
        }

        // Helper Methods
        private List<string> GetApprovalLevels(decimal value)
        {
            var levels = new List<string>();

            if (value <= 10000)
                levels.Add("StoreManager");
            else if (value <= 50000)
            {
                levels.Add("StoreManager");
                levels.Add("DepartmentHead");
            }
            else if (value <= 100000)
            {
                levels.Add("StoreManager");
                levels.Add("DepartmentHead");
                levels.Add("FinanceManager");
            }
            else
            {
                levels.Add("StoreManager");
                levels.Add("DepartmentHead");
                levels.Add("FinanceManager");
                levels.Add("Director");
            }

            return levels;
        }

        public async Task<string> GenerateDamageReportNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"DMG-{date:yyyyMM}";

            var lastReport = await _unitOfWork.DamageReports.Query()
                .Where(d => d.ReportNo.StartsWith(prefix))
                .OrderByDescending(d => d.ReportNo)
                .FirstOrDefaultAsync();

            if (lastReport == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastReport.ReportNo.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }

        public async Task<string> GenerateWriteOffRequestNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"WO-{date:yyyyMM}";

            var lastRequest = await _unitOfWork.WriteOffRequests.Query()
                .Where(w => w.RequestNo.StartsWith(prefix))
                .OrderByDescending(w => w.RequestNo)
                .FirstOrDefaultAsync();

            if (lastRequest == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastRequest.RequestNo.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }

        private async Task<DisposalCertificateDto> GenerateDisposalCertificateAsync(
            WriteOffRequest writeOffRequest, WriteOffExecutionDto executionDto)
        {
            var certificate = new DisposalCertificateDto
            {
                CertificateNo = $"DC-{DateTime.Now:yyyyMMdd}-{writeOffRequest.Id:D4}",
                WriteOffRequestNo = writeOffRequest.RequestNo,
                DisposalDate = executionDto.DisposalDate,
                DisposalMethod = executionDto.DisposalMethod,
                TotalValue = writeOffRequest.TotalValue,
                ExecutedBy = executionDto.ExecutedBy,
                WitnessedBy = executionDto.WitnessedBy,
                Items = writeOffRequest.Items.Select(i => new DisposalItemDto
                {
                    ItemName = i.Item?.Name,
                    Quantity = i.Quantity,
                    Value = i.Value
                }).ToList()
            };

            return certificate;
        }

        private async Task SendWriteOffCompletionNotificationsAsync(
            WriteOffRequest writeOffRequest, DisposalCertificateDto certificate)
        {
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Write-off Completed",
                Message = $"Write-off {writeOffRequest.RequestNo} has been executed. Certificate: {certificate.CertificateNo}",
                Type = "Completion",
                CreatedAt = DateTime.Now
            });
        }
















        #region CRUD Operations

        public async Task<IEnumerable<WriteOffDto>> GetAllWriteOffsAsync()
        {
            try
            {
                var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync(
                    includes: new[] { "WriteOffItems.Item", "WriteOffItems.Store" },
                    orderBy: q => q.OrderByDescending(w => w.WriteOffDate)
                );

                return await MapToWriteOffDtos(writeOffs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all write-offs");
                throw;
            }
        }

        public async Task<WriteOffDto> GetWriteOffByIdAsync(int id)
        {
            try
            {
                var writeOff = await _unitOfWork.WriteOffs.GetAsync(
                    w => w.Id == id,
                    includes: new[] { "WriteOffItems.Item", "WriteOffItems.Store" }
                );

                if (writeOff == null)
                    return null;

                var dto = await MapToWriteOffDto(writeOff);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving write-off {WriteOffId}", id);
                throw;
            }
        }

        public async Task<WriteOffDto> CreateWriteOffAsync(WriteOffDto dto, List<IFormFile> attachments = null)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Generate write-off number
                var writeOffNo = await GenerateWriteOffNumberAsync();

                // Calculate total value
                decimal totalValue = (decimal)dto.Items.Sum(i => i.Quantity * i.UnitPrice);

                // Get approval requirement
                var approvalReq = await GetApprovalRequirementAsync(totalValue);

                var writeOff = new WriteOff
                {
                    WriteOffNo = writeOffNo,
                    WriteOffDate = dto.WriteOffDate,
                    Reason = dto.Reason,
                    TotalValue = totalValue,
                    Status = approvalReq.RequiresApproval ? "Pending" : "Approved",
                    RequiredApproverRole = approvalReq.ApproverRole,
                    ApprovalThreshold = (int)approvalReq.ThresholdAmount,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                // Handle attachments
                if (attachments != null && attachments.Any())
                {
                    var attachmentPaths = new List<string>();
                    foreach (var file in attachments)
                    {
                        var path = await _fileService.SaveFileAsync(file, "writeoffs");
                        attachmentPaths.Add(path);
                    }
                    writeOff.AttachmentPaths = System.Text.Json.JsonSerializer.Serialize(attachmentPaths);
                }

                // If auto-approved, set approval details
                if (!approvalReq.RequiresApproval)
                {
                    writeOff.ApprovedBy = _userContext.CurrentUserName;
                    writeOff.ApprovedDate = DateTime.UtcNow;
                }

                await _unitOfWork.WriteOffs.AddAsync(writeOff);
                await _unitOfWork.CompleteAsync();

                // Add write-off items
                foreach (var itemDto in dto.Items)
                {
                    var writeOffItem = new WriteOffItem
                    {
                        WriteOffId = writeOff.Id,
                        ItemId = itemDto.ItemId,
                        StoreId = itemDto.StoreId,
                        Quantity = itemDto.Quantity,
                        Reason = itemDto.Reason,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };

                    await _unitOfWork.WriteOffItems.AddAsync(writeOffItem);

                    // Update stock if auto-approved
                    if (writeOff.Status == "Approved")
                    {
                        await UpdateStockForWriteOffAsync(itemDto);
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "WriteOff",
                    writeOff.Id,
                    "Create",
                    $"Created write-off {writeOff.WriteOffNo} for {totalValue:C}",
                    _userContext.CurrentUserName
                );

                // Send notifications if pending
                if (writeOff.Status == "Pending")
                {
                    var requirement = new ApprovalRequirement
                    {
                        RequiresApproval = approvalReq.RequiresApproval,
                        ApproverRole = approvalReq.ApproverRole,
                        ThresholdAmount = approvalReq.ThresholdAmount,
                        Description = approvalReq.Description
                    };
                    await SendWriteOffApprovalNotificationAsync(writeOff, requirement);
                }

                return await GetWriteOffByIdAsync(writeOff.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating write-off");
                throw;
            }
        }

        public async Task<WriteOffDto> UpdateWriteOffAsync(int id, WriteOffDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var writeOff = await _unitOfWork.WriteOffs.GetByIdAsync(id);
                if (writeOff == null)
                    throw new NotFoundException($"Write-off with ID {id} not found");

                if (writeOff.Status != "Draft" && writeOff.Status != "Pending")
                    throw new InvalidOperationException("Cannot update approved or rejected write-offs");

                // Update basic details
                writeOff.WriteOffDate = dto.WriteOffDate;
                writeOff.Reason = dto.Reason;
                writeOff.UpdatedAt = DateTime.UtcNow;
                writeOff.UpdatedBy = _userContext.CurrentUserName;

                // Recalculate total value
                decimal totalValue = (decimal)dto.Items.Sum(i => i.Quantity * i.UnitPrice);
                writeOff.TotalValue = totalValue;

                // Re-evaluate approval requirement
                var approvalReq = await GetApprovalRequirementAsync(totalValue);
                writeOff.RequiredApproverRole = approvalReq.ApproverRole;
                writeOff.ApprovalThreshold = (int)approvalReq.ThresholdAmount;

                _unitOfWork.WriteOffs.Update(writeOff);

                // Remove existing items
                var existingItems = await _unitOfWork.WriteOffItems.GetAllAsync(w => w.WriteOffId == id);
                foreach (var item in existingItems)
                {
                    _unitOfWork.WriteOffItems.Delete(item);
                }

                // Add updated items
                foreach (var itemDto in dto.Items)
                {
                    var writeOffItem = new WriteOffItem
                    {
                        WriteOffId = writeOff.Id,
                        ItemId = itemDto.ItemId,
                        StoreId = itemDto.StoreId,
                        Quantity = itemDto.Quantity,
                        Reason = itemDto.Reason,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };

                    await _unitOfWork.WriteOffItems.AddAsync(writeOffItem);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "WriteOff",
                    writeOff.Id,
                    "Update",
                    $"Updated write-off {writeOff.WriteOffNo}",
                    _userContext.CurrentUserName
                );

                return await GetWriteOffByIdAsync(id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating write-off {WriteOffId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteWriteOffAsync(int id)
        {
            try
            {
                var writeOff = await _unitOfWork.WriteOffs.GetByIdAsync(id);
                if (writeOff == null)
                    return false;

                if (writeOff.Status == "Approved")
                    throw new InvalidOperationException("Cannot delete approved write-offs");

                writeOff.IsActive = false;
                writeOff.UpdatedAt = DateTime.UtcNow;
                writeOff.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.WriteOffs.Update(writeOff);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "WriteOff",
                    writeOff.Id,
                    "Delete",
                    $"Deleted write-off {writeOff.WriteOffNo}",
                    _userContext.CurrentUserName
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting write-off {WriteOffId}", id);
                throw;
            }
        }

        #endregion

        #region Approval Workflow

        public async Task<bool> SubmitForApprovalAsync(int writeOffId)
        {
            try
            {
                var writeOff = await _unitOfWork.WriteOffs.GetByIdAsync(writeOffId);
                if (writeOff == null || writeOff.Status != "Draft")
                    return false;

                writeOff.Status = "Pending";
                writeOff.UpdatedAt = DateTime.UtcNow;
                writeOff.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.WriteOffs.Update(writeOff);
                await _unitOfWork.CompleteAsync();

                // Send approval notification
                var approvalReq = await GetApprovalRequirementAsync(writeOff.TotalValue);

                var requirement = new ApprovalRequirement
                {
                    RequiresApproval = approvalReq.RequiresApproval,
                    ApproverRole = approvalReq.ApproverRole,
                    ThresholdAmount = approvalReq.ThresholdAmount,
                    Description = approvalReq.Description
                };

                await SendWriteOffApprovalNotificationAsync(writeOff, requirement);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting write-off {WriteOffId} for approval", writeOffId);
                throw;
            }
        }

        public async Task<bool> ApproveWriteOffAsync(int writeOffId, string approvedBy, string comments)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var writeOff = await _unitOfWork.WriteOffs.GetAsync(
                    w => w.Id == writeOffId,
                    includes: new[] { "WriteOffItems" }
                );

                if (writeOff == null || writeOff.Status != "Pending")
                    return false;

                // Validate approval authority
                var userRoles = await GetUserRolesAsync(approvedBy);
                if (!userRoles.Contains(writeOff.RequiredApproverRole) && !userRoles.Contains("Admin"))
                {
                    throw new UnauthorizedAccessException(
                        $"User does not have required role: {writeOff.RequiredApproverRole}"
                    );
                }

                // Update write-off status
                writeOff.Status = "Approved";
                writeOff.ApprovedBy = approvedBy;
                writeOff.ApprovedDate = DateTime.UtcNow;
                writeOff.ApprovalComments = comments;
                writeOff.UpdatedAt = DateTime.UtcNow;
                writeOff.UpdatedBy = approvedBy;

                _unitOfWork.WriteOffs.Update(writeOff);

                // Update stock for approved items
                foreach (var item in writeOff.WriteOffItems)
                {
                    var itemDto = new WriteOffItemDto
                    {
                        ItemId = item.ItemId,
                        StoreId = item.StoreId,
                        Quantity = item.Quantity
                    };
                    await UpdateStockForWriteOffAsync(itemDto);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "WriteOff",
                    writeOff.Id,
                    "Approve",
                    $"Approved write-off {writeOff.WriteOffNo}",
                    approvedBy
                );

                // Send notification to creator
                await _notificationService.SendNotificationAsync(
                    writeOff.CreatedBy,
                    "Write-off Approved",
                    $"Your write-off #{writeOff.WriteOffNo} has been approved",
                    "success"
                );

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving write-off {WriteOffId}", writeOffId);
                throw;
            }
        }

        public async Task<bool> RejectWriteOffAsync(int writeOffId, string rejectedBy, string reason)
        {
            try
            {
                var writeOff = await _unitOfWork.WriteOffs.GetByIdAsync(writeOffId);
                if (writeOff == null || writeOff.Status != "Pending")
                    return false;

                writeOff.Status = "Rejected";
                writeOff.RejectedBy = rejectedBy;
                writeOff.RejectionDate = DateTime.UtcNow;
                writeOff.RejectionReason = reason;
                writeOff.UpdatedAt = DateTime.UtcNow;
                writeOff.UpdatedBy = rejectedBy;

                _unitOfWork.WriteOffs.Update(writeOff);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "WriteOff",
                    writeOff.Id,
                    "Reject",
                    $"Rejected write-off {writeOff.WriteOffNo}: {reason}",
                    rejectedBy
                );

                // Send notification to creator
                await _notificationService.SendNotificationAsync(
                    writeOff.CreatedBy,
                    "Write-off Rejected",
                    $"Your write-off #{writeOff.WriteOffNo} has been rejected. Reason: {reason}",
                    "error"
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting write-off {WriteOffId}", writeOffId);
                throw;
            }
        }

        #endregion

        #region Approval Requirements

        private ApprovalRequirement GetApprovalRequirement(decimal amount)
        {
            if (amount < 1000)
            {
                return new ApprovalRequirement
                {
                    RequiresApproval = false,
                    ThresholdAmount = 1000,
                    Description = "Amount below 1,000 BDT - Auto-approved"
                };
            }
            else if (amount < 10000)
            {
                return new ApprovalRequirement
                {
                    RequiresApproval = true,
                    ApproverRole = "StoreManager",
                    ThresholdAmount = 10000,
                    Description = "Amount 1,000-10,000 BDT - Store Manager approval required"
                };
            }
            else if (amount < 50000)
            {
                return new ApprovalRequirement
                {
                    RequiresApproval = true,
                    ApproverRole = "Officer",
                    ThresholdAmount = 50000,
                    Description = "Amount 10,000-50,000 BDT - Officer approval required"
                };
            }
            else
            {
                return new ApprovalRequirement
                {
                    RequiresApproval = true,
                    ApproverRole = "Admin",
                    ThresholdAmount = decimal.MaxValue,
                    Description = "Amount above 50,000 BDT - Admin approval required"
                };
            }
        }

        public async Task<bool> CanUserApproveAsync(string userId, decimal totalValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var requirement = GetApprovalRequirement(totalValue);
            if (!requirement.RequiresApproval) return true;

            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.Contains(requirement.ApproverRole) || userRoles.Contains("Admin");
        }

        #endregion

        #region Search and Filter

        public async Task<IEnumerable<WriteOffDto>> SearchWriteOffsAsync(string searchTerm)
        {
            try
            {
                var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync(
                    w => w.WriteOffNo.Contains(searchTerm) ||
                         w.Reason.Contains(searchTerm) ||
                         w.CreatedBy.Contains(searchTerm),
                    includes: new[] { "WriteOffItems.Item", "WriteOffItems.Store" }
                );

                return await MapToWriteOffDtos(writeOffs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching write-offs with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<WriteOffDto>> GetWriteOffsByStatusAsync(string status)
        {
            try
            {
                var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync(
                    w => w.Status == status,
                    includes: new[] { "WriteOffItems.Item", "WriteOffItems.Store" },
                    orderBy: q => q.OrderByDescending(w => w.WriteOffDate)
                );

                return await MapToWriteOffDtos(writeOffs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving write-offs by status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<WriteOffDto>> GetWriteOffsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync(
                    w => w.WriteOffDate >= startDate && w.WriteOffDate <= endDate,
                    includes: new[] { "WriteOffItems.Item", "WriteOffItems.Store" },
                    orderBy: q => q.OrderByDescending(w => w.WriteOffDate)
                );

                return await MapToWriteOffDtos(writeOffs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving write-offs by date range");
                throw;
            }
        }

        public async Task<IEnumerable<WriteOffDto>> GetPendingApprovalsAsync(string approverRole)
        {
            try
            {
                var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync(
                    w => w.Status == "Pending" && w.RequiredApproverRole == approverRole,
                    includes: new[] { "WriteOffItems.Item", "WriteOffItems.Store" },
                    orderBy: q => q.OrderBy(w => w.CreatedAt)
                );

                return await MapToWriteOffDtos(writeOffs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending approvals for role: {Role}", approverRole);
                throw;
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task<string> GenerateWriteOffNumberAsync()
        {
            var prefix = "WO";
            var year = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month.ToString().PadLeft(2, '0');

            var lastWriteOff = await _unitOfWork.WriteOffs.GetAsync(
                w => w.WriteOffNo.StartsWith($"{prefix}{year}{month}"),
                orderBy: q => q.OrderByDescending(w => w.Id)
            );

            int sequence = 1;
            if (lastWriteOff != null)
            {
                var lastSequence = lastWriteOff.WriteOffNo.Substring(6);
                if (int.TryParse(lastSequence, out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            return $"{prefix}{year}{month}{sequence.ToString().PadLeft(4, '0')}";
        }

        private async Task UpdateStockForWriteOffAsync(WriteOffItemDto item)
        {
            var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                si => si.ItemId == item.ItemId && si.StoreId == item.StoreId
            );

            if (storeItem != null)
            {
                storeItem.Quantity -= item.Quantity;
                if (storeItem.Quantity < 0) storeItem.Quantity = 0;

                storeItem.UpdatedAt = DateTime.UtcNow;
                storeItem.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.StoreItems.Update(storeItem);

                // Check for low stock alert
                await CheckAndCreateStockAlertAsync(storeItem);
            }
        }

        private async Task CheckAndCreateStockAlertAsync(StoreItem storeItem)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
            if (item != null && item.MinimumStock > 0 && storeItem.Quantity <= item.MinimumStock)
            {
                var existingAlert = await _unitOfWork.StockAlerts.SingleOrDefaultAsync(
                    sa => sa.ItemId == item.Id && sa.StoreId == storeItem.StoreId && sa.Status == "Active"
                );

                if (existingAlert == null)
                {
                    var alert = new StockAlert
                    {
                        ItemId = item.Id,
                        StoreId = storeItem.StoreId,
                        AlertType = storeItem.Quantity == 0 ? "OutOfStock" : "LowStock",
                        CurrentStock = storeItem.Quantity,
                        MinimumStock = item.MinimumStock,
                        Status = "Active",
                        AlertDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };

                    await _unitOfWork.StockAlerts.AddAsync(alert);
                    await _unitOfWork.CompleteAsync();

                    // Send notification
                    await SendStockAlertNotificationAsync(alert, item);
                }
            }
        }

        private async Task SendWriteOffApprovalNotificationAsync(WriteOff writeOff, ApprovalRequirement requirement)
        {
            var notification = new NotificationDto
            {
                Title = "Write-off Approval Required",
                Message = $"Write-off {writeOff.WriteOffNo} for {writeOff.TotalValue:C} requires approval",
                Type = NotificationType.Approval.ToString(),
                Priority = NotificationPriority.High.ToString(),
                TargetRole = requirement.ApproverRole,
                RelatedEntity = "WriteOff",
                RelatedEntityId = writeOff.Id
            };

            await _notificationService.CreateNotificationAsync(notification);

            writeOff.NotificationSent = true;
            writeOff.NotificationSentDate = DateTime.UtcNow;
            _unitOfWork.WriteOffs.Update(writeOff);
        }

        private async Task SendStockAlertNotificationAsync(StockAlert alert, Item item)
        {
            var notification = new NotificationDto
            {
                Title = $"{alert.AlertType}: {item.Name}",
                Message = $"Stock level for {item.Name} is {alert.CurrentStock} (Min: {alert.MinimumStock})",
                Type = NotificationType.StockAlert.ToString(),
                Priority = NotificationPriority.High.ToString(),
                TargetRole = "StoreManager",
                RelatedEntity = "StockAlert",
                RelatedEntityId = alert.Id
            };

            await _notificationService.CreateNotificationAsync(notification);
        }

        private async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        #endregion

        #region Mapping Methods

        private async Task<IEnumerable<WriteOffDto>> MapToWriteOffDtos(IEnumerable<WriteOff> writeOffs)
        {
            var dtos = new List<WriteOffDto>();
            foreach (var writeOff in writeOffs)
            {
                var dto = await MapToWriteOffDto(writeOff);
                dtos.Add(dto);
            }
            return dtos;
        }

        private async Task<WriteOffDto> MapToWriteOffDto(WriteOff writeOff)
        {
            var dto = new WriteOffDto
            {
                Id = writeOff.Id,
                WriteOffNo = writeOff.WriteOffNo,
                WriteOffDate = writeOff.WriteOffDate,
                Reason = writeOff.Reason,
                TotalValue = writeOff.TotalValue,
                Status = writeOff.Status,
                ApprovedBy = writeOff.ApprovedBy,
                ApprovedDate = writeOff.ApprovedDate,
                ApprovalComments = writeOff.ApprovalComments,
                RejectedBy = writeOff.RejectedBy,
                RejectionDate = writeOff.RejectionDate,
                RejectionReason = writeOff.RejectionReason,
                CreatedAt = writeOff.CreatedAt,
                CreatedBy = writeOff.CreatedBy,
                UpdatedAt = writeOff.UpdatedAt,
                UpdatedBy = writeOff.UpdatedBy,
                Items = new List<WriteOffItemDto>()
            };

            // Map items
            if (writeOff.WriteOffItems != null)
            {
                foreach (var item in writeOff.WriteOffItems)
                {
                    var itemDto = new WriteOffItemDto
                    {
                        Id = item.Id,
                        ItemId = item.ItemId,
                        ItemCode = item.Item?.ItemCode,
                        ItemName = item.Item?.Name,
                        StoreId = item.StoreId,
                        StoreName = item.Store?.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.Item?.UnitPrice ?? 0,
                        TotalPrice = item.Quantity * (item.Item?.UnitPrice ?? 0),
                        Reason = item.Reason,
                        Unit = item.Item?.Unit
                    };
                    dto.Items.Add(itemDto);
                }
            }

            // Parse attachment paths
            if (!string.IsNullOrEmpty(writeOff.AttachmentPaths))
            {
                try
                {
                    dto.AttachmentPaths = writeOff.AttachmentPaths;
                }
                catch
                {
                    dto.AttachmentPaths = "";
                }
            }

            return dto;
        }
        public async Task<WriteOffDto> CreateWriteOffAsync(WriteOffDto dto)
        {
            // Call the overload with null attachments
            return await CreateWriteOffAsync(dto, null);
        }

        public async Task<bool> ApproveWriteOffAsync(int writeOffId, string approvedBy)
        {
            // Call the overload with empty comments
            return await ApproveWriteOffAsync(writeOffId, approvedBy, string.Empty);
        }

        public async Task<WriteOffDto> CreateWriteOffWithApprovalAsync(WriteOffDto dto, string approvedBy)
        {
            dto.Status = "Approved";
            dto.ApprovedBy = approvedBy;
            dto.ApprovedDate = DateTime.UtcNow;
            return await CreateWriteOffAsync(dto);
        }

        public async Task<IEnumerable<WriteOffDto>> GetPagedWriteOffsAsync(int page, int pageSize)
        {
            var allWriteOffs = await GetAllWriteOffsAsync();
            return allWriteOffs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<bool> WriteOffNoExistsAsync(string writeOffNo)
        {
            var writeOff = await _unitOfWork.WriteOffs.SingleOrDefaultAsync(w => w.WriteOffNo == writeOffNo);
            return writeOff != null;
        }

        public async Task<int> GetWriteOffCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync();

            if (startDate.HasValue)
                writeOffs = writeOffs.Where(w => w.WriteOffDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                writeOffs = writeOffs.Where(w => w.WriteOffDate <= endDate.Value).ToList();

            // Fix: Use Count() method instead of Count property
            return writeOffs.Count();
        }

        public async Task<decimal> GetTotalWriteOffValueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync();
            writeOffs = writeOffs.Where(w => w.Status == "Approved").ToList();

            if (startDate.HasValue)
                writeOffs = writeOffs.Where(w => w.WriteOffDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                writeOffs = writeOffs.Where(w => w.WriteOffDate <= endDate.Value).ToList();

            return writeOffs.Sum(w => w.TotalValue);
        }

        public async Task<IEnumerable<WriteOffDto>> GetWriteOffsByApproverAsync(string approverName)
        {
            var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync(
                w => w.ApprovedBy == approverName,
                includes: new[] { "WriteOffItems.Item", "WriteOffItems.Store" }
            );
            return await MapToWriteOffDtos(writeOffs);
        }

        public async Task<IEnumerable<WriteOffDto>> GetWriteOffsByReasonAsync(string reason)
        {
            var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync(
                w => w.Reason.Contains(reason),
                includes: new[] { "WriteOffItems.Item", "WriteOffItems.Store" }
            );
            return await MapToWriteOffDtos(writeOffs);
        }

        public async Task<ApprovalThresholdDto> GetApprovalRequirementAsync(decimal amount)
        {
            var requirement = GetApprovalRequirement(amount);
            return await Task.FromResult(new ApprovalThresholdDto
            {
                RequiresApproval = requirement.RequiresApproval,
                ApproverRole = requirement.ApproverRole,
                ThresholdAmount = requirement.ThresholdAmount,
                Description = requirement.Description,
                MinValue = amount < 1000 ? 0 : amount < 10000 ? 1000 : amount < 50000 ? 10000 : 50000,
                MaxValue = amount < 1000 ? 999 : amount < 10000 ? 9999 : amount < 50000 ? 49999 : decimal.MaxValue,
                Level = ""
            });
        }
        #endregion

        public async Task<ServiceResult> CreateWriteOffFromDamageAsync(int damageId)
        {
            try
            {
                var damage = await _unitOfWork.Damages
                    .GetAsync(d => d.Id == damageId,
                        includes: new[] { "Item" });

                if (damage == null)
                    return ServiceResult.Failure("Damage report not found");

                // Calculate estimated value based on item price
                var item = damage.Item;
                var estimatedUnitPrice = 100m; // Default value

                // Try to get last purchase price
                var lastPurchase = await _unitOfWork.PurchaseItems
                    .FindAsync(pi => pi.ItemId == damage.ItemId);
                if (lastPurchase.Any())
                {
                    estimatedUnitPrice = lastPurchase.OrderByDescending(pi => pi.CreatedAt).First().UnitPrice;
                }

                var totalValue = damage.Quantity * estimatedUnitPrice;

                var writeOffDto = new WriteOffDto
                {
                    WriteOffNo = await GenerateWriteOffNoAsync(),
                    WriteOffDate = DateTime.Now,
                    Reason = $"Damaged items - {damage.Description}",
                    TotalValue = (decimal)totalValue,
                    Items = new List<WriteOffItemDto>
                {
                    new WriteOffItemDto
                    {
                        ItemId = damage.ItemId,
                        ItemName = item?.Name,
                        Quantity = (decimal)damage.Quantity,
                        UnitPrice = estimatedUnitPrice,
                        TotalPrice = totalValue,
                        Reason = damage.DamageType
                    }
                }
                };

                var result = await CreateWriteOffAsync(writeOffDto);

                if (result != null)
                {
                    damage.Status = "Write-off Created";
                    damage.UpdatedAt = DateTime.Now;
                    damage.UpdatedBy = _userContext.CurrentUserName;
                    _unitOfWork.Damages.Update(damage);
                    await _unitOfWork.CompleteAsync();
                }

                return ServiceResult.SuccessResult("Write-off created from damage report");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating write-off from damage");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<string> GenerateWriteOffNoAsync()
        {
            var prefix = "WO";
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString("D2");

            var lastWriteOff = await _unitOfWork.WriteOffs
                .Query()
                .Where(w => w.WriteOffNo.StartsWith($"{prefix}{year}{month}"))
                .OrderByDescending(w => w.WriteOffNo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastWriteOff != null)
            {
                var lastNumber = lastWriteOff.WriteOffNo.Substring(7);
                nextNumber = int.Parse(lastNumber) + 1;
            }

            return $"{prefix}{year}{month}{nextNumber:D4}";
        }
    }
}