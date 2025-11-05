using IMS.Application.DTOs;
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
    public interface IPersonnelItemLifeService
    {
        Task<PersonnelItemIssueDto> CreatePersonnelIssueAsync(PersonnelItemIssueDto dto);
        Task<IEnumerable<PersonnelItemIssueDto>> GetExpiringItemsAsync(int daysAhead = 30);
        Task<IEnumerable<PersonnelItemIssueDto>> GetByPersonnelAsync(string badgeNo);
        Task<PersonnelItemIssueDto> ReplaceItemAsync(int id, string reason);
        Task<bool> ProcessExpiryAlertsAsync();
        Task<DashboardStatsDto> GetLifeSpanDashboardAsync();
        Task StartLifeTrackingFromReceiveAsync(int receiveId);
        Task<IEnumerable<PersonnelItemIssueDto>> GetExpiredItemsAsync();
    }
    
    public class PersonnelItemLifeService : IPersonnelItemLifeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PersonnelItemLifeService> _logger;
        private readonly IUserContext _userContext;
        
        public PersonnelItemLifeService(
            IUnitOfWork unitOfWork, 
            INotificationService notificationService,
            ILogger<PersonnelItemLifeService> logger,
            IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
            _userContext = userContext;
        }
        
        public async Task<PersonnelItemIssueDto> CreatePersonnelIssueAsync(PersonnelItemIssueDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                
                var item = await _unitOfWork.Items.GetByIdAsync(dto.ItemId);
                
                // Only track controlled items
                if (item.ItemControlType != "Controlled")
                {
                    _logger.LogInformation($"Item {item.ItemCode} is not controlled, skipping life tracking");
                    return null;
                }
                
                // Check authorization
                if (dto.PersonnelType == "Ansar" && !item.IsAnsarAuthorized)
                {
                    throw new InvalidOperationException($"Item {item.Name} is not authorized for Ansar personnel");
                }
                
                if (dto.PersonnelType == "VDP" && !item.IsVDPAuthorized)
                {
                    throw new InvalidOperationException($"Item {item.Name} is not authorized for VDP personnel");
                }
                
                var personnelIssue = new PersonnelItemIssue
                {
                    IssueNo = dto.IssueNo ?? await GenerateIssueNoAsync(),
                    PersonnelId = dto.PersonnelId,
                    PersonnelType = dto.PersonnelType,
                    PersonnelName = dto.PersonnelName,
                    PersonnelBadgeNo = dto.PersonnelBadgeNo,
                    PersonnelUnit = dto.PersonnelUnit,
                    PersonnelDesignation = dto.PersonnelDesignation,
                    PersonnelMobile = dto.PersonnelMobile,
                    ItemId = dto.ItemId,
                    Quantity = dto.Quantity,
                    Unit = item.Unit,
                    IssueDate = dto.IssueDate,
                    ReceivedDate = dto.ReceivedDate,
                    BattalionId = dto.BattalionId,
                    StoreId = dto.StoreId,
                    Status = "Active",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.UserId ?? "System",
                    IsActive = true
                };
                
                // Calculate expiry date if received - Use personnel-specific lifespan
                if (dto.ReceivedDate.HasValue)
                {
                    int? lifeSpanMonths = null;
                    int? alertBeforeDays = null;

                    // Determine lifespan based on personnel type
                    if (dto.PersonnelType == "Ansar")
                    {
                        lifeSpanMonths = item.AnsarLifeSpanMonths ?? item.LifeSpanMonths; // Fallback to legacy field
                        alertBeforeDays = item.AnsarAlertBeforeDays ?? item.AlertBeforeDays ?? 30;
                    }
                    else if (dto.PersonnelType == "VDP")
                    {
                        lifeSpanMonths = item.VDPLifeSpanMonths ?? item.LifeSpanMonths; // Fallback to legacy field
                        alertBeforeDays = item.VDPAlertBeforeDays ?? item.AlertBeforeDays ?? 30;
                    }

                    if (lifeSpanMonths.HasValue)
                    {
                        personnelIssue.LifeExpiryDate = dto.ReceivedDate.Value.AddMonths(lifeSpanMonths.Value);
                        personnelIssue.AlertDate = personnelIssue.LifeExpiryDate.Value.AddDays(-alertBeforeDays.Value);
                        personnelIssue.RemainingDays = (personnelIssue.LifeExpiryDate.Value - DateTime.Now).Days;

                        _logger.LogInformation(
                            "Personnel {PersonnelType} {PersonnelName} - Item {ItemName} - " +
                            "Lifespan: {Months} months, Expiry: {ExpiryDate}, Alert: {AlertDate}",
                            dto.PersonnelType, dto.PersonnelName, item.Name,
                            lifeSpanMonths.Value, personnelIssue.LifeExpiryDate, personnelIssue.AlertDate);
                    }
                }
                
                await _unitOfWork.PersonnelItemIssues.AddAsync(personnelIssue);
                await _unitOfWork.CompleteAsync();
                
                // Create initial notification
                await CreateLifeSpanNotificationAsync(personnelIssue, "New controlled item issued");
                
                await _unitOfWork.CommitTransactionAsync();
                
                dto.Id = personnelIssue.Id;
                dto.Status = personnelIssue.Status;
                dto.LifeExpiryDate = personnelIssue.LifeExpiryDate;
                dto.RemainingDays = personnelIssue.RemainingDays;
                
                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating personnel issue");
                throw;
            }
        }
        
        public async Task<IEnumerable<PersonnelItemIssueDto>> GetExpiringItemsAsync(int daysAhead = 30)
        {
            var expiryDate = DateTime.Now.AddDays(daysAhead);
            
            var expiringItems = await _unitOfWork.PersonnelItemIssues
                .Query()
                .Include(p => p.Item)
                .Include(p => p.Battalion)
                .Include(p => p.Store)
                .Where(p => p.IsActive && 
                           p.Status == "Active" &&
                           p.LifeExpiryDate != null &&
                           p.LifeExpiryDate <= expiryDate)
                .OrderBy(p => p.LifeExpiryDate)
                .ToListAsync();
                
            return expiringItems.Select(MapToDto);
        }
        
        public async Task<IEnumerable<PersonnelItemIssueDto>> GetExpiredItemsAsync()
        {
            var expiredItems = await _unitOfWork.PersonnelItemIssues
                .Query()
                .Include(p => p.Item)
                .Include(p => p.Battalion)
                .Include(p => p.Store)
                .Where(p => p.IsActive && 
                           p.Status == "Active" &&
                           p.LifeExpiryDate != null &&
                           p.LifeExpiryDate <= DateTime.Now)
                .OrderBy(p => p.LifeExpiryDate)
                .ToListAsync();
                
            return expiredItems.Select(MapToDto);
        }
        
        public async Task<IEnumerable<PersonnelItemIssueDto>> GetByPersonnelAsync(string badgeNo)
        {
            var items = await _unitOfWork.PersonnelItemIssues
                .Query()
                .Include(p => p.Item)
                .Include(p => p.Store)
                .Where(p => p.PersonnelBadgeNo == badgeNo && p.IsActive)
                .OrderByDescending(p => p.IssueDate)
                .ToListAsync();
                
            return items.Select(MapToDto);
        }
        
        public async Task<PersonnelItemIssueDto> ReplaceItemAsync(int id, string reason)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                
                var existingIssue = await _unitOfWork.PersonnelItemIssues
                    .Query()
                    .Include(p => p.Item)
                    .FirstOrDefaultAsync(p => p.Id == id);
                    
                if (existingIssue == null)
                    throw new InvalidOperationException("Personnel issue not found");
                    
                // Mark existing as replaced
                existingIssue.Status = "Replaced";
                existingIssue.ReplacedDate = DateTime.Now;
                existingIssue.ReplacementReason = reason;
                existingIssue.UpdatedAt = DateTime.Now;
                existingIssue.UpdatedBy = _userContext.UserId;
                
                _unitOfWork.PersonnelItemIssues.Update(existingIssue);
                
                // Create new issue for replacement
                var newIssue = new PersonnelItemIssue
                {
                    IssueNo = await GenerateIssueNoAsync(),
                    PersonnelId = existingIssue.PersonnelId,
                    PersonnelType = existingIssue.PersonnelType,
                    PersonnelName = existingIssue.PersonnelName,
                    PersonnelBadgeNo = existingIssue.PersonnelBadgeNo,
                    PersonnelUnit = existingIssue.PersonnelUnit,
                    ItemId = existingIssue.ItemId,
                    Quantity = existingIssue.Quantity,
                    Unit = existingIssue.Unit,
                    IssueDate = DateTime.Now,
                    ReceivedDate = DateTime.Now,
                    BattalionId = existingIssue.BattalionId,
                    StoreId = existingIssue.StoreId,
                    Status = "Active",
                    ReplacementIssueId = existingIssue.Id,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.UserId,
                    IsActive = true,
                    Remarks = $"Replacement for {existingIssue.IssueNo} - Reason: {reason}"
                };
                
                // Calculate new expiry - Use personnel-specific lifespan
                int? lifeSpanMonths = null;
                int? alertBeforeDays = null;

                if (existingIssue.PersonnelType == "Ansar")
                {
                    lifeSpanMonths = existingIssue.Item.AnsarLifeSpanMonths ?? existingIssue.Item.LifeSpanMonths;
                    alertBeforeDays = existingIssue.Item.AnsarAlertBeforeDays ?? existingIssue.Item.AlertBeforeDays ?? 30;
                }
                else if (existingIssue.PersonnelType == "VDP")
                {
                    lifeSpanMonths = existingIssue.Item.VDPLifeSpanMonths ?? existingIssue.Item.LifeSpanMonths;
                    alertBeforeDays = existingIssue.Item.VDPAlertBeforeDays ?? existingIssue.Item.AlertBeforeDays ?? 30;
                }

                if (lifeSpanMonths.HasValue)
                {
                    newIssue.LifeExpiryDate = DateTime.Now.AddMonths(lifeSpanMonths.Value);
                    newIssue.AlertDate = newIssue.LifeExpiryDate.Value.AddDays(-alertBeforeDays.Value);
                    newIssue.RemainingDays = (newIssue.LifeExpiryDate.Value - DateTime.Now).Days;
                }
                
                await _unitOfWork.PersonnelItemIssues.AddAsync(newIssue);
                await _unitOfWork.CompleteAsync();
                
                await _unitOfWork.CommitTransactionAsync();
                
                return MapToDto(newIssue);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error replacing item");
                throw;
            }
        }
        
        public async Task<bool> ProcessExpiryAlertsAsync()
        {
            try
            {
                var expiringItems = await _unitOfWork.PersonnelItemIssues
                    .Query()
                    .Include(p => p.Item)
                    .Include(p => p.Battalion)
                    .Include(p => p.Store)
                    .Where(p => p.IsActive && 
                               p.Status == "Active" &&
                               p.LifeExpiryDate != null &&
                               p.LifeExpiryDate <= DateTime.Now.AddDays(30))
                    .ToListAsync();
                    
                foreach (var item in expiringItems)
                {
                    // Update remaining days
                    if (item.LifeExpiryDate.HasValue)
                    {
                        item.RemainingDays = (item.LifeExpiryDate.Value - DateTime.Now).Days;
                        
                        // Check if alert needed
                        bool shouldSendAlert = false;
                        string alertType = "";
                        
                        if (item.RemainingDays <= 0)
                        {
                            item.Status = "Expired";
                            shouldSendAlert = true;
                            alertType = "Expired";
                        }
                        else if (item.RemainingDays <= 7 && item.AlertCount < 3)
                        {
                            shouldSendAlert = true;
                            alertType = "Critical";
                        }
                        else if (item.RemainingDays <= 30 && !item.IsAlertSent)
                        {
                            shouldSendAlert = true;
                            alertType = "Warning";
                        }
                        
                        if (shouldSendAlert)
                        {
                            await SendLifeSpanAlertAsync(item, alertType);
                            item.IsAlertSent = true;
                            item.LastAlertDate = DateTime.Now;
                            item.AlertCount++;
                        }
                        
                        _unitOfWork.PersonnelItemIssues.Update(item);
                    }
                }
                
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expiry alerts");
                return false;
            }
        }
        
        public async Task StartLifeTrackingFromReceiveAsync(int receiveId)
        {
            var receive = await _unitOfWork.Receives
                .Query()
                .Include(r => r.ReceiveItems)
                .FirstOrDefaultAsync(r => r.Id == receiveId);
                
            if (receive == null)
                return;
                
            // Check if this is a Battalion/VDP receive
            if (receive.ReceiveType != "Battalion" && receive.ReceiveType != "VDP")
                return;

            foreach (var receiveItem in receive.ReceiveItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(receiveItem.ItemId);
                
                // Only track controlled items
                if (item.ItemControlType != "Controlled")
                    continue;
                    
                // If individual name is specified, create personnel issue
                if (!string.IsNullOrEmpty(receive.ReceivedFromIndividualName))
                {
                    await CreatePersonnelIssueAsync(new PersonnelItemIssueDto
                    {
                        IssueNo = receive.OriginalIssueNo ?? receive.ReceiveNo,
                        PersonnelName = receive.ReceivedFromIndividualName,
                        PersonnelBadgeNo = receive.ReceivedFromIndividualBadgeNo,
                        PersonnelType = receive.ReceiveType == "Battalion" ? "Ansar" : "VDP",
                        ItemId = receiveItem.ItemId,
                        Quantity = receiveItem.Quantity,
                        IssueDate = receive.ReceiveDate,
                        ReceivedDate = receive.ReceiveDate,
                        BattalionId = receive.ReceivedFromBattalionId,
                        StoreId = receive.StoreId
                    });
                }
            }
        }
        
        public async Task<DashboardStatsDto> GetLifeSpanDashboardAsync()
        {
            var activeItems = await _unitOfWork.PersonnelItemIssues
                .Query()
                .Where(p => p.IsActive && p.Status == "Active")
                .ToListAsync();
                
            var expiredItems = await GetExpiredItemsAsync();
            
            var expiringIn7Days = activeItems
                .Where(i => i.RemainingDays > 0 && i.RemainingDays <= 7)
                .Count();
                
            var expiringIn30Days = activeItems
                .Where(i => i.RemainingDays > 0 && i.RemainingDays <= 30)
                .Count();
                
            return new DashboardStatsDto
            {
                TotalActiveItems = activeItems.Count,
                ExpiredItems = expiredItems.Count(),
                ExpiringIn7Days = expiringIn7Days,
                ExpiringIn30Days = expiringIn30Days,
                TotalPersonnel = activeItems.Select(i => i.PersonnelBadgeNo).Distinct().Count(),
                AnsarItems = activeItems.Count(i => i.PersonnelType == "Ansar"),
                VDPItems = activeItems.Count(i => i.PersonnelType == "VDP")
            };
        }
        
        // Helper Methods
        private async Task<string> GenerateIssueNoAsync()
        {
            var prefix = "PLI"; // Personnel Life Issue
            var date = DateTime.Now.ToString("yyyyMMdd");
            var count = await _unitOfWork.PersonnelItemIssues
                .CountAsync(p => p.CreatedAt.Date == DateTime.Today) + 1;
                
            return $"{prefix}-{date}-{count:D4}";
        }
        
        private async Task CreateLifeSpanNotificationAsync(PersonnelItemIssue issue, string message)
        {
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Life Span Tracking",
                Message = $"{message} - {issue.Item?.Name} for {issue.PersonnelName}",
                Type = "Info",
                Priority = "Normal",
                Category = "LifeSpan",
                ReferenceType = "PersonnelItemIssue",
                ReferenceId = issue.Id.ToString()
            });
        }
        
        private async Task SendLifeSpanAlertAsync(PersonnelItemIssue issue, string alertType)
        {
            var priority = alertType switch
            {
                "Expired" => "Critical",
                "Critical" => "High",
                _ => "Normal"
            };
            
            var message = alertType switch
            {
                "Expired" => $"{issue.Item?.Name} for {issue.PersonnelName} has expired",
                "Critical" => $"{issue.Item?.Name} for {issue.PersonnelName} expires in {issue.RemainingDays} days",
                _ => $"{issue.Item?.Name} for {issue.PersonnelName} will expire on {issue.LifeExpiryDate:dd/MM/yyyy}"
            };
            
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = $"Life Span Alert - {alertType}",
                Message = message,
                Type = "Alert",
                Priority = priority,
                Category = "LifeSpan",
                ReferenceType = "PersonnelItemIssue",
                ReferenceId = issue.Id.ToString(),
                ActionUrl = $"/PersonnelLifeTracking/Details/{issue.Id}"
            });
        }
        
        private PersonnelItemIssueDto MapToDto(PersonnelItemIssue entity)
        {
            return new PersonnelItemIssueDto
            {
                Id = entity.Id,
                IssueNo = entity.IssueNo,
                PersonnelId = entity.PersonnelId,
                PersonnelType = entity.PersonnelType,
                PersonnelName = entity.PersonnelName,
                PersonnelBadgeNo = entity.PersonnelBadgeNo,
                PersonnelUnit = entity.PersonnelUnit,
                PersonnelDesignation = entity.PersonnelDesignation,
                ItemId = entity.ItemId,
                ItemName = entity.Item?.Name,
                ItemCode = entity.Item?.ItemCode,
                Quantity = entity.Quantity,
                Unit = entity.Unit,
                LifeSpanMonths = entity.Item?.LifeSpanMonths,
                IssueDate = entity.IssueDate,
                ReceivedDate = entity.ReceivedDate,
                LifeExpiryDate = entity.LifeExpiryDate,
                AlertDate = entity.AlertDate,
                RemainingDays = entity.RemainingDays,
                Status = entity.Status,
                BattalionId = entity.BattalionId,
                BattalionName = entity.Battalion?.Name,
                StoreId = entity.StoreId,
                StoreName = entity.Store?.Name,
                IsAlertSent = entity.IsAlertSent,
                LastAlertDate = entity.LastAlertDate,
                AlertCount = entity.AlertCount,
                Remarks = entity.Remarks
            };
        }
    }
}