using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class AllotmentLetterService : IAllotmentLetterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AllotmentLetterService> _logger;
        private readonly IApprovalService _approvalService;

        public AllotmentLetterService(
            IUnitOfWork unitOfWork,
            ILogger<AllotmentLetterService> logger,
            IApprovalService approvalService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _approvalService = approvalService;
        }

        public async Task<AllotmentLetterDto> GetAllotmentLetterByIdAsync(int id)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetAsync(
                    a => a.Id == id,
                    includes: new[] { "Items.Item", "FromStore", "IssuedToBattalion", "IssuedToRange", "IssuedToZila", "IssuedToUpazila", "Recipients.Items.Item", "Recipients.Range", "Recipients.Battalion", "Recipients.Zila", "Recipients.Upazila", "DistributionList" });

                return MapToDto(allotment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving allotment letter with ID {Id}", id);
                throw;
            }
        }

        public async Task<AllotmentLetterDto> GetAllotmentLetterByNoAsync(string allotmentNo)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetAsync(
                    a => a.AllotmentNo == allotmentNo,
                    includes: new[] { "Items.Item", "FromStore" });

                return MapToDto(allotment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving allotment letter with number {AllotmentNo}", allotmentNo);
                throw;
            }
        }

        public async Task<IEnumerable<AllotmentLetterDto>> GetAllAllotmentLettersAsync()
        {
            try
            {
                var allotments = await _unitOfWork.AllotmentLetters.GetAllAsync(
                    includes: new[] { "FromStore", "IssuedToBattalion", "IssuedToRange", "Items", "Recipients.Items.Item", "DistributionList" });

                return allotments.Select(a => MapToDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all allotment letters");
                throw;
            }
        }

        public async Task<AllotmentLetterDto> CreateAllotmentLetterAsync(AllotmentLetterDto dto)
        {
            try
            {
                var allotmentLetter = MapToEntity(dto);
                allotmentLetter.AllotmentNo = await GenerateAllotmentNoAsync();
                allotmentLetter.Status = "Draft";
                allotmentLetter.AllotmentDate = DateTime.Now;

                // Calculate remaining quantities
                foreach (var item in allotmentLetter.Items)
                {
                    item.RemainingQuantity = item.AllottedQuantity;
                    item.IssuedQuantity = 0;
                }

                await _unitOfWork.AllotmentLetters.AddAsync(allotmentLetter);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Allotment letter {AllotmentNo} created successfully", allotmentLetter.AllotmentNo);

                return await GetAllotmentLetterByIdAsync(allotmentLetter.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating allotment letter");
                throw;
            }
        }

        public async Task<AllotmentLetterDto> UpdateAllotmentLetterAsync(AllotmentLetterDto dto)
        {
            try
            {
                var existing = await _unitOfWork.AllotmentLetters.GetByIdAsync(dto.Id);

                if (existing == null)
                    throw new InvalidOperationException($"Allotment letter with ID {dto.Id} not found");

                if (existing.Status != "Draft")
                    throw new InvalidOperationException("Only draft allotment letters can be updated");

                UpdateEntityFromDto(existing, dto);

                _unitOfWork.AllotmentLetters.Update(existing);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Allotment letter {AllotmentNo} updated successfully", existing.AllotmentNo);

                return await GetAllotmentLetterByIdAsync(existing.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating allotment letter with ID {Id}", dto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAllotmentLetterAsync(int id)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetByIdAsync(id);

                if (allotment == null)
                {
                    _logger.LogWarning("Allotment letter with ID {Id} not found for deletion", id);
                    return false;
                }

                if (allotment.Status != "Draft")
                    throw new InvalidOperationException("Only draft allotment letters can be deleted");

                _unitOfWork.AllotmentLetters.Delete(allotment);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Allotment letter {AllotmentNo} deleted successfully", allotment.AllotmentNo);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting allotment letter with ID {Id}", id);
                throw;
            }
        }

        public async Task<ServiceResult> SubmitForApprovalAsync(int id, string submittedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var allotment = await _unitOfWork.AllotmentLetters
                    .GetAsync(a => a.Id == id, includes: new[] { "Items" });

                if (allotment == null)
                    return ServiceResult.Failure("Allotment letter not found");

                if (allotment.Status != "Draft")
                    return ServiceResult.Failure("Only draft allotment letters can be submitted for approval");

                if (!allotment.Items.Any())
                    return ServiceResult.Failure("Cannot submit allotment letter without items");

                // Calculate total quantity
                var totalQuantity = allotment.Items.Sum(x => x.AllottedQuantity);

                // AllotmentLetter ALWAYS requires DD Provision approval (Provision Store workflow)
                // Check if there's a configured approval requirement, otherwise use default
                var approvalRequired = await _approvalService.GetApprovalRequirementAsync("ALLOTMENT_LETTER", totalQuantity);
                string approverRole = approvalRequired?.ApproverRole ?? "DD Provision";

                allotment.Status = "Pending";

                // Create approval request
                var approval = new Domain.Entities.ApprovalRequest
                {
                    EntityType = "ALLOTMENT_LETTER",
                    EntityId = allotment.Id,
                    RequestedBy = submittedBy,
                    RequestedDate = DateTime.Now,
                    Status = Domain.Enums.ApprovalStatus.Pending.ToString(),
                    Priority = "Normal",
                    Amount = 0, // Allotment letters don't have amount
                    Description = $"Allotment Letter {allotment.AllotmentNo} - {totalQuantity} items to {allotment.IssuedTo}",
                    CurrentLevel = 1,
                    MaxLevel = 1,
                    CreatedAt = DateTime.Now,
                    CreatedBy = submittedBy,
                    IsActive = true
                };

                await _unitOfWork.ApprovalRequests.AddAsync(approval);
                await _unitOfWork.CompleteAsync();

                // Create approval step (DD Provision for Provision Store)
                var approvalStep = new Domain.Entities.ApprovalStep
                {
                    ApprovalRequestId = approval.Id,
                    StepLevel = 1,
                    ApproverRole = approverRole,
                    Status = Domain.Enums.ApprovalStatus.Pending,
                    CreatedAt = DateTime.Now,
                    CreatedBy = submittedBy,
                    IsActive = true
                };

                await _unitOfWork.ApprovalSteps.AddAsync(approvalStep);

                allotment.UpdatedAt = DateTime.Now;
                allotment.UpdatedBy = submittedBy;

                _unitOfWork.AllotmentLetters.Update(allotment);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Allotment letter {AllotmentNo} submitted for approval by {User}",
                    allotment.AllotmentNo, submittedBy);

                return ServiceResult.SuccessResult("Allotment letter submitted for approval");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error submitting allotment letter {Id} for approval", id);
                return ServiceResult.Failure("Error submitting for approval: " + ex.Message);
            }
        }

        public async Task<bool> ApproveAllotmentLetterAsync(int id, string approvedBy)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetByIdAsync(id);

                if (allotment == null)
                {
                    _logger.LogWarning("Allotment letter with ID {Id} not found for approval", id);
                    return false;
                }

                // Can only approve if status is Pending (from Approval Center)
                if (allotment.Status != "Pending")
                {
                    _logger.LogWarning("Allotment letter {AllotmentNo} cannot be approved. Current status: {Status}",
                        allotment.AllotmentNo, allotment.Status);
                    return false;
                }

                allotment.Status = "Active";
                allotment.ApprovedBy = approvedBy;
                allotment.ApprovedDate = DateTime.Now;

                _unitOfWork.AllotmentLetters.Update(allotment);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Allotment letter {AllotmentNo} approved by {User}",
                    allotment.AllotmentNo, approvedBy);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving allotment letter with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> RejectAllotmentLetterAsync(int id, string rejectedBy, string reason)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetByIdAsync(id);

                if (allotment == null)
                {
                    _logger.LogWarning("Allotment letter with ID {Id} not found for rejection", id);
                    return false;
                }

                if (allotment.Status != "Draft")
                {
                    _logger.LogWarning("Allotment letter {AllotmentNo} cannot be rejected. Current status: {Status}",
                        allotment.AllotmentNo, allotment.Status);
                    return false;
                }

                allotment.Status = "Rejected";
                allotment.RejectedBy = rejectedBy;
                allotment.RejectedDate = DateTime.Now;
                allotment.RejectionReason = reason;

                _unitOfWork.AllotmentLetters.Update(allotment);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Allotment letter {AllotmentNo} rejected by {User}. Reason: {Reason}",
                    allotment.AllotmentNo, rejectedBy, reason);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting allotment letter with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> CancelAllotmentLetterAsync(int id, string reason)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetByIdAsync(id);

                if (allotment == null)
                {
                    _logger.LogWarning("Allotment letter with ID {Id} not found for cancellation", id);
                    return false;
                }

                allotment.Status = "Cancelled";
                allotment.Remarks = $"Cancelled: {reason}";

                _unitOfWork.AllotmentLetters.Update(allotment);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Allotment letter {AllotmentNo} cancelled. Reason: {Reason}",
                    allotment.AllotmentNo, reason);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling allotment letter with ID {Id}", id);
                throw;
            }
        }

        public async Task<string> GenerateAllotmentNoAsync()
        {
            try
            {
                var prefix = "AL";
                var year = DateTime.Now.Year.ToString().Substring(2);
                var month = DateTime.Now.Month.ToString("D2");

                var lastAllotment = await _unitOfWork.AllotmentLetters
                    .Query()
                    .Where(a => a.AllotmentNo.StartsWith($"{prefix}{year}{month}"))
                    .OrderByDescending(a => a.AllotmentNo)
                    .FirstOrDefaultAsync();

                int sequence = 1;
                if (lastAllotment != null)
                {
                    var lastSeq = lastAllotment.AllotmentNo.Substring(7);
                    if (int.TryParse(lastSeq, out int lastNum))
                    {
                        sequence = lastNum + 1;
                    }
                }

                return $"{prefix}{year}{month}{sequence:D4}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating allotment number");
                throw;
            }
        }

        public async Task<bool> ValidateAllotmentLetterForIssueAsync(int allotmentLetterId, List<IssueItemDto> items)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetAsync(
                    a => a.Id == allotmentLetterId,
                    includes: new[] { "Items" });

                if (allotment == null)
                {
                    _logger.LogWarning("Allotment letter {Id} not found", allotmentLetterId);
                    return false;
                }

                if (allotment.Status != "Active")
                {
                    _logger.LogWarning("Allotment letter {AllotmentNo} is not active. Status: {Status}",
                        allotment.AllotmentNo, allotment.Status);
                    return false;
                }

                if (allotment.ValidUntil < DateTime.Now)
                {
                    _logger.LogWarning("Allotment letter {AllotmentNo} has expired", allotment.AllotmentNo);
                    return false;
                }

                foreach (var issueItem in items)
                {
                    var allotmentItem = allotment.Items.FirstOrDefault(ai => ai.ItemId == issueItem.ItemId);

                    if (allotmentItem == null)
                    {
                        _logger.LogWarning("Item {ItemId} not found in allotment letter {AllotmentNo}",
                            issueItem.ItemId, allotment.AllotmentNo);
                        return false;
                    }

                    if (allotmentItem.RemainingQuantity < issueItem.Quantity)
                    {
                        _logger.LogWarning("Insufficient quantity for item {ItemId}. Requested: {Requested}, Remaining: {Remaining}",
                            issueItem.ItemId, issueItem.Quantity, allotmentItem.RemainingQuantity);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating allotment letter {Id} for issue", allotmentLetterId);
                throw;
            }
        }

        public async Task<bool> UpdateIssuedQuantitiesAsync(int allotmentLetterId, List<IssueItemDto> items)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetAsync(
                    a => a.Id == allotmentLetterId,
                    includes: new[] { "Items" });

                if (allotment == null)
                {
                    _logger.LogWarning("Allotment letter {Id} not found for updating issued quantities", allotmentLetterId);
                    return false;
                }

                foreach (var issueItem in items)
                {
                    var allotmentItem = allotment.Items.FirstOrDefault(ai => ai.ItemId == issueItem.ItemId);

                    if (allotmentItem != null)
                    {
                        allotmentItem.IssuedQuantity += issueItem.Quantity;
                        allotmentItem.RemainingQuantity = allotmentItem.AllottedQuantity - allotmentItem.IssuedQuantity;
                    }
                }

                // Check if fully issued
                if (allotment.Items.All(i => i.RemainingQuantity == 0))
                {
                    allotment.Status = "Completed";
                    _logger.LogInformation("Allotment letter {AllotmentNo} marked as completed", allotment.AllotmentNo);
                }

                _unitOfWork.AllotmentLetters.Update(allotment);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issued quantities for allotment letter {Id}", allotmentLetterId);
                throw;
            }
        }

        public async Task<IEnumerable<AllotmentLetterDto>> GetActiveAllotmentLettersAsync()
        {
            var allotments = await _unitOfWork.AllotmentLetters
                .Query()
                .Include(a => a.FromStore)
                .Include(a => a.IssuedToBattalion)
                .Include(a => a.Items)
                .Where(a => a.Status == "Active" && a.ValidUntil >= DateTime.Now)
                .ToListAsync();

            return allotments.Select(a => MapToDto(a));
        }

        public async Task<IEnumerable<AllotmentLetterDto>> GetAllotmentLettersByStoreAsync(int storeId)
        {
            var allotments = await _unitOfWork.AllotmentLetters
                .Query()
                .Include(a => a.Items)
                .Include(a => a.IssuedToBattalion)
                .Where(a => a.FromStoreId == storeId)
                .ToListAsync();

            return allotments.Select(a => MapToDto(a));
        }

        public async Task<IEnumerable<AllotmentLetterDto>> GetAllotmentLettersByBattalionAsync(int battalionId)
        {
            var allotments = await _unitOfWork.AllotmentLetters
                .Query()
                .Include(a => a.Items)
                .Include(a => a.FromStore)
                .Where(a => a.IssuedToBattalionId == battalionId)
                .ToListAsync();

            return allotments.Select(a => MapToDto(a));
        }

        // MANUAL MAPPING METHODS

        private AllotmentLetterDto MapToDto(AllotmentLetter entity)
        {
            if (entity == null) return null;

            return new AllotmentLetterDto
            {
                Id = entity.Id,
                AllotmentNo = entity.AllotmentNo,
                AllotmentDate = entity.AllotmentDate,
                ValidFrom = entity.ValidFrom,
                ValidUntil = entity.ValidUntil,
                IssuedTo = entity.IssuedTo,
                IssuedToType = entity.IssuedToType,
                IssuedToBattalionId = entity.IssuedToBattalionId,
                IssuedToBattalionName = entity.IssuedToBattalion?.Name,
                IssuedToRangeId = entity.IssuedToRangeId,
                IssuedToRangeName = entity.IssuedToRange?.Name,
                IssuedToZilaId = entity.IssuedToZilaId,
                IssuedToZilaName = entity.IssuedToZila?.Name,
                IssuedToUpazilaId = entity.IssuedToUpazilaId,
                IssuedToUpazilaName = entity.IssuedToUpazila?.Name,
                FromStoreId = entity.FromStoreId,
                FromStoreName = entity.FromStore?.Name,
                Purpose = entity.Purpose,
                Status = entity.Status,
                ApprovedBy = entity.ApprovedBy,
                ApprovedDate = entity.ApprovedDate,
                RejectedBy = entity.RejectedBy,
                RejectedDate = entity.RejectedDate,
                RejectionReason = entity.RejectionReason,
                DocumentPath = entity.DocumentPath,
                ReferenceNo = entity.ReferenceNo,
                Remarks = entity.Remarks,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy,
                Items = entity.Items?.Select(i => new AllotmentLetterItemDto
                {
                    Id = i.Id,
                    AllotmentLetterId = i.AllotmentLetterId,
                    ItemId = i.ItemId,
                    ItemCode = i.Item?.ItemCode,
                    ItemName = i.Item?.Name,
                    AllottedQuantity = i.AllottedQuantity,
                    IssuedQuantity = i.IssuedQuantity,
                    RemainingQuantity = i.RemainingQuantity,
                    Unit = i.Unit,
                    Remarks = i.Remarks
                }).ToList() ?? new List<AllotmentLetterItemDto>(),

                // NEW: Multiple recipients mapping
                Recipients = entity.Recipients?.Select(r => new AllotmentLetterRecipientDto
                {
                    Id = r.Id,
                    AllotmentLetterId = r.AllotmentLetterId,
                    RecipientType = r.RecipientType,
                    RangeId = r.RangeId,
                    BattalionId = r.BattalionId,
                    ZilaId = r.ZilaId,
                    UpazilaId = r.UpazilaId,
                    RecipientName = r.RecipientName,
                    Remarks = r.Remarks,
                    SerialNo = r.SerialNo,
                    Items = r.Items?.Select(ri => new AllotmentLetterRecipientItemDto
                    {
                        Id = ri.Id,
                        AllotmentLetterRecipientId = ri.AllotmentLetterRecipientId,
                        ItemId = ri.ItemId,
                        ItemCode = ri.Item?.ItemCode,
                        ItemName = ri.Item?.Name,
                        AllottedQuantity = ri.AllottedQuantity,
                        IssuedQuantity = ri.IssuedQuantity,
                        Unit = ri.Unit,
                        Remarks = ri.Remarks
                    }).ToList() ?? new List<AllotmentLetterRecipientItemDto>()
                }).ToList() ?? new List<AllotmentLetterRecipientDto>(),

                // Bengali/Government Format Fields
                Subject = entity.Subject,
                SubjectBn = entity.SubjectBn,
                BodyText = entity.BodyText,
                BodyTextBn = entity.BodyTextBn,
                CollectionDeadline = entity.CollectionDeadline,
                SignatoryName = entity.SignatoryName,
                SignatoryDesignation = entity.SignatoryDesignation,
                SignatoryDesignationBn = entity.SignatoryDesignationBn,
                SignatoryId = entity.SignatoryId,
                SignatoryPhone = entity.SignatoryPhone,
                SignatoryEmail = entity.SignatoryEmail,
                BengaliDate = entity.BengaliDate,

                // Distribution List (অনুলিপি)
                DistributionList = entity.DistributionList?.Select(d => new AllotmentLetterDistributionDto
                {
                    Id = d.Id,
                    AllotmentLetterId = d.AllotmentLetterId,
                    SerialNo = d.SerialNo,
                    RecipientTitle = d.RecipientTitle,
                    RecipientTitleBn = d.RecipientTitleBn,
                    Address = d.Address,
                    AddressBn = d.AddressBn,
                    Purpose = d.Purpose,
                    PurposeBn = d.PurposeBn,
                    DisplayOrder = d.DisplayOrder
                }).OrderBy(d => d.DisplayOrder).ThenBy(d => d.SerialNo).ToList() ?? new List<AllotmentLetterDistributionDto>()
            };
        }

        private AllotmentLetter MapToEntity(AllotmentLetterDto dto)
        {
            return new AllotmentLetter
            {
                Id = dto.Id,
                AllotmentNo = dto.AllotmentNo,
                AllotmentDate = dto.AllotmentDate,
                ValidFrom = dto.ValidFrom,
                ValidUntil = dto.ValidUntil,
                IssuedTo = dto.IssuedTo,
                IssuedToType = dto.IssuedToType,
                IssuedToBattalionId = dto.IssuedToBattalionId,
                IssuedToRangeId = dto.IssuedToRangeId,
                IssuedToZilaId = dto.IssuedToZilaId,
                IssuedToUpazilaId = dto.IssuedToUpazilaId,
                FromStoreId = dto.FromStoreId,
                Purpose = dto.Purpose,
                Status = dto.Status,
                DocumentPath = dto.DocumentPath,
                ReferenceNo = dto.ReferenceNo,
                Remarks = dto.Remarks,
                CreatedBy = dto.CreatedBy,
                Items = dto.Items?.Select(i => new AllotmentLetterItem
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    AllottedQuantity = i.AllottedQuantity,
                    IssuedQuantity = i.IssuedQuantity,
                    RemainingQuantity = i.RemainingQuantity,
                    Unit = i.Unit,
                    Remarks = i.Remarks
                }).ToList() ?? new List<AllotmentLetterItem>(),

                // NEW: Multiple recipients mapping
                Recipients = dto.Recipients?.Select(r => new AllotmentLetterRecipient
                {
                    Id = r.Id,
                    RecipientType = r.RecipientType,
                    RangeId = r.RangeId,
                    BattalionId = r.BattalionId,
                    ZilaId = r.ZilaId,
                    UpazilaId = r.UpazilaId,
                    RecipientName = r.RecipientName,
                    Remarks = r.Remarks,
                    SerialNo = r.SerialNo,
                    Items = r.Items?.Select(ri => new AllotmentLetterRecipientItem
                    {
                        Id = ri.Id,
                        ItemId = ri.ItemId,
                        AllottedQuantity = ri.AllottedQuantity,
                        IssuedQuantity = ri.IssuedQuantity,
                        Unit = ri.Unit,
                        Remarks = ri.Remarks
                    }).ToList() ?? new List<AllotmentLetterRecipientItem>()
                }).ToList() ?? new List<AllotmentLetterRecipient>(),

                // Bengali/Government Format Fields
                Subject = dto.Subject,
                SubjectBn = dto.SubjectBn,
                BodyText = dto.BodyText,
                BodyTextBn = dto.BodyTextBn,
                CollectionDeadline = dto.CollectionDeadline,
                SignatoryName = dto.SignatoryName,
                SignatoryDesignation = dto.SignatoryDesignation,
                SignatoryDesignationBn = dto.SignatoryDesignationBn,
                SignatoryId = dto.SignatoryId,
                SignatoryPhone = dto.SignatoryPhone,
                SignatoryEmail = dto.SignatoryEmail,
                BengaliDate = dto.BengaliDate,

                // Distribution List (অনুলিপি)
                DistributionList = dto.DistributionList?.Select(d => new AllotmentLetterDistribution
                {
                    Id = d.Id,
                    AllotmentLetterId = d.AllotmentLetterId,
                    SerialNo = d.SerialNo,
                    RecipientTitle = d.RecipientTitle,
                    RecipientTitleBn = d.RecipientTitleBn,
                    Address = d.Address,
                    AddressBn = d.AddressBn,
                    Purpose = d.Purpose,
                    PurposeBn = d.PurposeBn,
                    DisplayOrder = d.DisplayOrder
                }).ToList() ?? new List<AllotmentLetterDistribution>()
            };
        }

        private void UpdateEntityFromDto(AllotmentLetter entity, AllotmentLetterDto dto)
        {
            entity.ValidFrom = dto.ValidFrom;
            entity.ValidUntil = dto.ValidUntil;
            entity.IssuedTo = dto.IssuedTo;
            entity.IssuedToType = dto.IssuedToType;
            entity.IssuedToBattalionId = dto.IssuedToBattalionId;
            entity.IssuedToRangeId = dto.IssuedToRangeId;
            entity.IssuedToZilaId = dto.IssuedToZilaId;
            entity.IssuedToUpazilaId = dto.IssuedToUpazilaId;
            entity.FromStoreId = dto.FromStoreId;
            entity.Purpose = dto.Purpose;
            entity.DocumentPath = dto.DocumentPath;
            entity.ReferenceNo = dto.ReferenceNo;
            entity.Remarks = dto.Remarks;
            entity.UpdatedBy = dto.UpdatedBy;
            entity.UpdatedAt = DateTime.Now;

            // Update items if provided
            if (dto.Items != null)
            {
                entity.Items = dto.Items.Select(i => new AllotmentLetterItem
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    AllottedQuantity = i.AllottedQuantity,
                    IssuedQuantity = i.IssuedQuantity,
                    RemainingQuantity = i.RemainingQuantity,
                    Unit = i.Unit,
                    Remarks = i.Remarks
                }).ToList();
            }

            // NEW: Update recipients if provided
            if (dto.Recipients != null)
            {
                entity.Recipients = dto.Recipients.Select(r => new AllotmentLetterRecipient
                {
                    Id = r.Id,
                    RecipientType = r.RecipientType,
                    RangeId = r.RangeId,
                    BattalionId = r.BattalionId,
                    ZilaId = r.ZilaId,
                    UpazilaId = r.UpazilaId,
                    RecipientName = r.RecipientName,
                    Remarks = r.Remarks,
                    SerialNo = r.SerialNo,
                    Items = r.Items?.Select(ri => new AllotmentLetterRecipientItem
                    {
                        Id = ri.Id,
                        ItemId = ri.ItemId,
                        AllottedQuantity = ri.AllottedQuantity,
                        IssuedQuantity = ri.IssuedQuantity,
                        Unit = ri.Unit,
                        Remarks = ri.Remarks
                    }).ToList() ?? new List<AllotmentLetterRecipientItem>()
                }).ToList();
            }
        }
    }
}