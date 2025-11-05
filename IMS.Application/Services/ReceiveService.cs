using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Document = iTextSharp.text.Document;

namespace IMS.Application.Services
{
    public class ReceiveService : IReceiveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IActivityLogService _activityLogService;
        private readonly IPersonnelItemLifeService _personnelItemLifeService;
        private readonly IBarcodeService _barcodeService;
        private readonly IFileService _fileService;
        private readonly IIssueService _issueService;
        private readonly ILogger<ReceiveService> _logger;

        public ReceiveService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            IBarcodeService barcodeService,
            IFileService fileService,
            IIssueService issueService,
            IPersonnelItemLifeService personnelItemLifeService,
            IActivityLogService activityLogService,
            ILogger<ReceiveService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _barcodeService = barcodeService;
            _fileService = fileService;
            _issueService = issueService;
            _activityLogService = activityLogService;
            _logger = logger;
            _personnelItemLifeService = personnelItemLifeService;
        }

        public async Task<IEnumerable<ReceiveDto>> GetAllReceivesAsync()
        {
            var receives = await _unitOfWork.Receives.GetAllAsync();
            var receiveDtos = new List<ReceiveDto>();

            foreach (var receive in receives.Where(r => r.IsActive))
            {
                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receive.Id);

                // Get original issue items if linked
                List<IssueItemDto> issueItems = null;
                if (receive.OriginalIssueId.HasValue)
                {
                    var issueDto = await _issueService.GetIssueByIdAsync(receive.OriginalIssueId.Value);
                    issueItems = issueDto?.Items?.ToList();
                }

                var items = new List<ReceiveItemDto>();
                foreach (var ri in receiveItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(ri.ItemId);
                    var store = await _unitOfWork.Stores.GetByIdAsync(ri.StoreId);

                    // Get issued quantity from original issue if available
                    var issueItem = issueItems?.FirstOrDefault(ii => ii.ItemId == ri.ItemId);

                    items.Add(new ReceiveItemDto
                    {
                        ItemId = ri.ItemId,
                        ItemCode = item?.Code,
                        ItemName = item?.Name,
                        StoreId = ri.StoreId,
                        StoreName = store?.Name,
                        Quantity = ri.Quantity,
                        IssuedQuantity = issueItem?.Quantity ?? ri.Quantity,
                        ReceivedQuantity = ri.ReceivedQuantity ?? ri.Quantity,
                        Condition = ri.Condition,
                        Remarks = ri.Remarks,
                        Unit = item?.Unit,
                        LedgerNo = ri.LedgerNo,
                        PageNo = ri.PageNo,
                        UsableQuantity = ri.UsableQuantity,
                        PartiallyUsableQuantity = ri.PartiallyUsableQuantity,
                        UnusableQuantity = ri.UnusableQuantity
                    });
                }

                // Build receive name based on type
                string receivedFromName = GetReceivedFromName(receive);

                // Get store information
                Store receiveStore = null;
                if (receive.StoreId.HasValue)
                {
                    receiveStore = await _unitOfWork.Stores.GetByIdAsync(receive.StoreId.Value);
                }

                receiveDtos.Add(new ReceiveDto
                {
                    Id = receive.Id,
                    ReceiveNo = receive.ReceiveNo,
                    ReceiveDate = receive.ReceiveDate,
                    ReceiveType = receive.ReceiveType,
                    ReceivedFromBattalionId = receive.ReceivedFromBattalionId,
                    ReceivedFromRangeId = receive.ReceivedFromRangeId,
                    ReceivedFromZilaId = receive.ReceivedFromZilaId,
                    ReceivedFromUpazilaId = receive.ReceivedFromUpazilaId,
                    ReceivedFromIndividualName = receive.ReceivedFromIndividualName,
                    ReceivedFromIndividualBadgeNo = receive.ReceivedFromIndividualBadgeNo,
                    ReceivedFromName = receivedFromName,
                    ReceivedBy = receive.ReceivedBy,
                    StoreId = receive.StoreId,
                    StoreName = receiveStore?.Name,
                    OverallCondition = receive.OverallCondition,
                    AssessmentNotes = receive.AssessmentNotes,
                    OriginalIssueId = receive.OriginalIssueId,
                    OriginalIssueNo = receive.OriginalIssue?.IssueNo,
                    Status = receive.Status,
                    Remarks = receive.Remarks,
                    Items = items,
                    CreatedAt = receive.CreatedAt,
                    CreatedBy = receive.CreatedBy,
                    UpdatedAt = receive.UpdatedAt,
                    UpdatedBy = receive.UpdatedBy
                });
            }

            return receiveDtos;
        }

        public async Task<ReceiveDto> GetReceiveByIdAsync(int id)
        {
            var receive = await _unitOfWork.Receives.GetByIdAsync(id);
            if (receive == null || !receive.IsActive) return null;

            // Include related entities
            if (receive.ReceivedFromBattalionId.HasValue)
                receive.ReceivedFromBattalion = await _unitOfWork.Battalions.GetByIdAsync(receive.ReceivedFromBattalionId.Value);
            if (receive.ReceivedFromRangeId.HasValue)
                receive.ReceivedFromRange = await _unitOfWork.Ranges.GetByIdAsync(receive.ReceivedFromRangeId.Value);
            if (receive.ReceivedFromZilaId.HasValue)
                receive.ReceivedFromZila = await _unitOfWork.Zilas.GetByIdAsync(receive.ReceivedFromZilaId.Value);
            if (receive.ReceivedFromUpazilaId.HasValue)
                receive.ReceivedFromUpazila = await _unitOfWork.Upazilas.GetByIdAsync(receive.ReceivedFromUpazilaId.Value);
            if (receive.OriginalIssueId.HasValue)
                receive.OriginalIssue = await _unitOfWork.Issues.GetByIdAsync(receive.OriginalIssueId.Value);

            var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receive.Id);

            // Get original issue items if linked
            List<IssueItemDto> issueItems = null;
            if (receive.OriginalIssueId.HasValue)
            {
                var issueDto = await _issueService.GetIssueByIdAsync(receive.OriginalIssueId.Value);
                issueItems = issueDto?.Items?.ToList();
            }

            var items = new List<ReceiveItemDto>();
            foreach (var ri in receiveItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(ri.ItemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(ri.StoreId);

                // Get issued quantity from original issue if available
                var issueItem = issueItems?.FirstOrDefault(ii => ii.ItemId == ri.ItemId);

                items.Add(new ReceiveItemDto
                {
                    ItemId = ri.ItemId,
                    ItemCode = item?.Code,
                    ItemName = item?.Name,
                    StoreId = ri.StoreId,
                    StoreName = store?.Name,
                    Quantity = ri.Quantity,
                    IssuedQuantity = issueItem?.Quantity ?? ri.Quantity, // Use original issue qty if available
                    ReceivedQuantity = ri.ReceivedQuantity ?? ri.Quantity,
                    Condition = ri.Condition,
                    Remarks = ri.Remarks,
                    Unit = item?.Unit
                });
            }

            // Get store information
            Store storeEntity = null;
            if (receive.StoreId.HasValue)
            {
                storeEntity = await _unitOfWork.Stores.GetByIdAsync(receive.StoreId.Value);
            }

            return new ReceiveDto
            {
                Id = receive.Id,
                ReceiveNo = receive.ReceiveNo,
                ReceiveDate = receive.ReceiveDate,
                ReceiveType = receive.ReceiveType,
                ReceivedFromBattalionId = receive.ReceivedFromBattalionId,
                ReceivedFromRangeId = receive.ReceivedFromRangeId,
                ReceivedFromZilaId = receive.ReceivedFromZilaId,
                ReceivedFromUpazilaId = receive.ReceivedFromUpazilaId,
                ReceivedFromIndividualName = receive.ReceivedFromIndividualName,
                ReceivedFromIndividualBadgeNo = receive.ReceivedFromIndividualBadgeNo,
                ReceivedFromName = GetReceivedFromName(receive),
                ReceivedBy = receive.ReceivedBy,
                StoreId = receive.StoreId,
                StoreName = storeEntity?.Name,
                OverallCondition = receive.OverallCondition,
                AssessmentNotes = receive.AssessmentNotes,
                AssessedBy = receive.AssessedBy,
                AssessmentDate = receive.AssessmentDate,
                OriginalIssueId = receive.OriginalIssueId,
                OriginalIssueNo = receive.OriginalIssue?.IssueNo,
                Status = receive.Status,
                Remarks = receive.Remarks,
                Items = items,
                CreatedAt = receive.CreatedAt,
                CreatedBy = receive.CreatedBy,
                UpdatedAt = receive.UpdatedAt,
                UpdatedBy = receive.UpdatedBy
            };
        }

        public async Task<ReceiveDto> CreateReceiveAsync(ReceiveDto receiveDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var receive = new Receive
                {
                    ReceiveNo = await GenerateReceiveNoAsync(),
                    ReceiveDate = receiveDto.ReceiveDate,
                    ReceiveType = receiveDto.ReceiveType,
                    ReceivedFromBattalionId = receiveDto.ReceivedFromBattalionId,
                    ReceivedFromRangeId = receiveDto.ReceivedFromRangeId,
                    ReceivedFromZilaId = receiveDto.ReceivedFromZilaId,
                    ReceivedFromUpazilaId = receiveDto.ReceivedFromUpazilaId,
                    ReceivedFromIndividualName = receiveDto.ReceivedFromIndividualName,
                    ReceivedFromIndividualBadgeNo = receiveDto.ReceivedFromIndividualBadgeNo,
                    ReceivedBy = receiveDto.ReceivedBy ?? _userContext.CurrentUserName,
                    OverallCondition = receiveDto.OverallCondition ?? "Good",
                    AssessmentNotes = receiveDto.AssessmentNotes,
                    AssessedBy = receiveDto.ReceivedBy ?? _userContext.CurrentUserName,
                    AssessmentDate = DateTime.UtcNow,
                    Status = receiveDto.Status ?? "Completed",
                    Remarks = receiveDto.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.Receives.AddAsync(receive);
                await _unitOfWork.CompleteAsync();

                // Add receive items and update store stock
                foreach (var itemDto in receiveDto.Items)
                {
                    var receiveItem = new ReceiveItem
                    {
                        ReceiveId = receive.Id,
                        ItemId = itemDto.ItemId,
                        StoreId = itemDto.StoreId,
                        Quantity = itemDto.Quantity,
                        Condition = itemDto.Condition ?? "Good",
                        Remarks = itemDto.Remarks,
                        LedgerNo = itemDto.LedgerNo,
                        PageNo = itemDto.PageNo,
                        UsableQuantity = itemDto.UsableQuantity,
                        PartiallyUsableQuantity = itemDto.PartiallyUsableQuantity,
                        UnusableQuantity = itemDto.UnusableQuantity,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _userContext.CurrentUserName
                    };

                    await _unitOfWork.ReceiveItems.AddAsync(receiveItem);

                    // ⚠️ IMPORTANT: Stock adjustment happens ONLY in CompleteReceiveAsync
                    // Do NOT add stock here to prevent double-counting
                    // If status is "Completed", CompleteReceiveAsync will be called separately
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogActivityAsync(
                    "Receive",
                    receive.Id,
                    "Create",
                    $"Created receive {receive.ReceiveNo}",
                    _userContext.CurrentUserName
                );

                return await GetReceiveByIdAsync(receive.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating receive");
                throw;
            }
        }

        // Create receive from issue
        public async Task<ReceiveDto> CreateReceiveFromIssueAsync(int issueId, ReceiveDto receiveDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
                if (issue == null || issue.Status != "Approved")
                    throw new InvalidOperationException("Issue not found or not approved");

                var receive = new Receive
                {
                    ReceiveNo = await GenerateReceiveNoAsync(),
                    ReceiveDate = receiveDto.ReceiveDate,
                    ReceiveType = issue.IssuedToType,
                    ReceivedFromBattalionId = issue.IssuedToBattalionId,
                    ReceivedFromRangeId = issue.IssuedToRangeId,
                    ReceivedFromZilaId = issue.IssuedToZilaId,
                    ReceivedFromUpazilaId = issue.IssuedToUpazilaId,
                    ReceivedFromIndividualName = issue.IssuedToIndividualName,
                    ReceivedFromIndividualBadgeNo = issue.IssuedToIndividualBadgeNo,
                    ReceivedBy = receiveDto.ReceivedBy ?? _userContext.CurrentUserName,
                    Remarks = receiveDto.Remarks,
                    OriginalIssueId = issueId,
                    OverallCondition = receiveDto.OverallCondition,
                    AssessmentNotes = receiveDto.AssessmentNotes,
                    AssessedBy = _userContext.CurrentUserName,
                    AssessmentDate = DateTime.UtcNow,
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.Receives.AddAsync(receive);
                await _unitOfWork.CompleteAsync();

                // Add receive items with condition assessment
                foreach (var itemDto in receiveDto.Items)
                {
                    var receiveItem = new ReceiveItem
                    {
                        ReceiveId = receive.Id,
                        ItemId = itemDto.ItemId,
                        StoreId = itemDto.StoreId,
                        Quantity = itemDto.Quantity,
                        Condition = itemDto.Condition ?? "Good",
                        Remarks = itemDto.Remarks,
                        LedgerNo = itemDto.LedgerNo,
                        PageNo = itemDto.PageNo,
                        UsableQuantity = itemDto.UsableQuantity,
                        PartiallyUsableQuantity = itemDto.PartiallyUsableQuantity,
                        UnusableQuantity = itemDto.UnusableQuantity,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _userContext.CurrentUserName
                    };

                    await _unitOfWork.ReceiveItems.AddAsync(receiveItem);

                    // Update stock based on condition
                    if (itemDto.Condition != "Damaged")
                    {
                        var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                            si => si.ItemId == itemDto.ItemId && si.StoreId == itemDto.StoreId
                        );

                        if (storeItem != null)
                        {
                            storeItem.Quantity += itemDto.Quantity;
                            storeItem.UpdatedAt = DateTime.UtcNow;
                            storeItem.UpdatedBy = _userContext.CurrentUserName;
                            _unitOfWork.StoreItems.Update(storeItem);
                        }
                        else
                        {
                            // Create new store item if doesn't exist
                            storeItem = new StoreItem
                            {
                                StoreId = itemDto.StoreId,
                                ItemId = itemDto.ItemId,
                                Quantity = itemDto.Quantity,
                                Status = ItemStatus.Available,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = _userContext.CurrentUserName
                            };
                            await _unitOfWork.StoreItems.AddAsync(storeItem);
                        }
                    }
                    else
                    {
                        // Create damage record for damaged items
                        await CreateDamageRecordAsync(itemDto, receive.Id);
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogActivityAsync(
                    "Receive",
                    receive.Id,
                    "CreateFromIssue",
                    $"Created receive {receive.ReceiveNo} from issue #{issue.IssueNo}",
                    _userContext.CurrentUserName
                );

                return await GetReceiveByIdAsync(receive.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating receive from issue");
                throw;
            }
        }

        // Get receives by issue
        public async Task<IEnumerable<ReceiveDto>> GetReceivesByIssueAsync(int issueId)
        {
            var receives = await _unitOfWork.Receives.FindAsync(r => r.OriginalIssueId == issueId && r.IsActive);
            var receiveDtos = new List<ReceiveDto>();

            foreach (var receive in receives)
            {
                receiveDtos.Add(await GetReceiveByIdAsync(receive.Id));
            }

            return receiveDtos;
        }

        public async Task<string> GenerateReceiveNoAsync()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var count = await _unitOfWork.Receives.CountAsync(r => r.CreatedAt.Year == year && r.CreatedAt.Month == month) + 1;
            return $"RCV-{year}{month:D2}-{count:D4}";
        }

        public async Task<IEnumerable<ReceiveDto>> GetReceivesByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var receives = await GetAllReceivesAsync();
            return receives.Where(r => r.ReceiveDate >= fromDate && r.ReceiveDate <= toDate);
        }

        public async Task<IEnumerable<ReceiveDto>> GetReceivesBySourceAsync(string source)
        {
            var receives = await GetAllReceivesAsync();
            return receives.Where(r => r.ReceivedFromName.Contains(source, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<ReceiveDto>> GetReceivesByTypeAsync(string sourceType)
        {
            var receives = await GetAllReceivesAsync();
            return receives.Where(r => r.ReceiveType.Equals(sourceType, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<int> GetReceiveCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var receives = await _unitOfWork.Receives.GetAllAsync();
            var query = receives.Where(r => r.IsActive);

            if (fromDate.HasValue)
                query = query.Where(r => r.ReceiveDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.ReceiveDate <= toDate.Value);

            return query.Count();
        }

        public async Task<decimal> GetTotalReceivedValueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var receives = await _unitOfWork.Receives.GetAllAsync();
            var query = receives.Where(r => r.IsActive);

            if (fromDate.HasValue)
                query = query.Where(r => r.ReceiveDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.ReceiveDate <= toDate.Value);

            var filteredReceives = query.ToList();
            var purchaseItems = await _unitOfWork.PurchaseItems.GetAllAsync();

            decimal totalValue = 0;

            foreach (var receive in filteredReceives)
            {
                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receive.Id);
                foreach (var receiveItem in receiveItems)
                {
                    // Get the last purchase price for this item
                    var lastPurchase = purchaseItems
                        .Where(pi => pi.ItemId == receiveItem.ItemId)
                        .OrderByDescending(pi => pi.CreatedAt)
                        .FirstOrDefault();

                    if (lastPurchase != null && (receiveItem.Condition.ToLower() == "good" || receiveItem.Condition.ToLower() == "serviceable"))
                    {
                        totalValue += (decimal)(receiveItem.Quantity * lastPurchase.UnitPrice);
                    }
                }
            }

            return totalValue;
        }

        public async Task<bool> ReceiveNoExistsAsync(string receiveNo)
        {
            var receive = await _unitOfWork.Receives.FirstOrDefaultAsync(r => r.ReceiveNo == receiveNo);
            return receive != null;
        }

        public async Task<IEnumerable<ReceiveDto>> GetPagedReceivesAsync(int pageNumber, int pageSize)
        {
            var allReceives = await GetAllReceivesAsync();
            return allReceives
                .OrderByDescending(r => r.ReceiveDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        // Export to Excel
        public byte[] ExportToExcel(IEnumerable<ReceiveDto> receives)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Receives");

                // Headers
                worksheet.Cells[1, 1].Value = "Receive No";
                worksheet.Cells[1, 2].Value = "Date";
                worksheet.Cells[1, 3].Value = "Type";
                worksheet.Cells[1, 4].Value = "Received From";
                worksheet.Cells[1, 5].Value = "Total Items";
                worksheet.Cells[1, 6].Value = "Condition";
                worksheet.Cells[1, 7].Value = "Original Issue";
                worksheet.Cells[1, 8].Value = "Status";
                worksheet.Cells[1, 9].Value = "Received By";
                worksheet.Cells[1, 10].Value = "Created Date";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 10])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Data
                int row = 2;
                foreach (var receive in receives)
                {
                    worksheet.Cells[row, 1].Value = receive.ReceiveNo;
                    worksheet.Cells[row, 2].Value = receive.ReceiveDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 3].Value = receive.ReceiveType;
                    worksheet.Cells[row, 4].Value = receive.ReceivedFromName;
                    worksheet.Cells[row, 5].Value = receive.Items.Count;
                    worksheet.Cells[row, 6].Value = receive.OverallCondition;
                    worksheet.Cells[row, 7].Value = receive.OriginalIssueNo ?? "-";
                    worksheet.Cells[row, 8].Value = receive.Status;
                    worksheet.Cells[row, 9].Value = receive.ReceivedBy;
                    worksheet.Cells[row, 10].Value = receive.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        // Export to PDF
        public byte[] ExportToPdf(IEnumerable<ReceiveDto> receives)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4.Rotate());
                var writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Receive Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Table
                var table = new PdfPTable(8);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 15f, 12f, 10f, 20f, 10f, 10f, 13f, 10f });

                // Headers
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                table.AddCell(new PdfPCell(new Phrase("Receive No", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Date", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Type", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Received From", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Items", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Condition", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Issue No", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Status", headerFont)));

                // Data
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                foreach (var receive in receives)
                {
                    table.AddCell(new PdfPCell(new Phrase(receive.ReceiveNo, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(receive.ReceiveDate.ToString("dd/MM/yyyy"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(receive.ReceiveType, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(receive.ReceivedFromName, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(receive.Items.Count.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(receive.OverallCondition, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(receive.OriginalIssueNo ?? "-", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(receive.Status, cellFont)));
                }

                document.Add(table);

                // Footer
                document.Add(new Paragraph("\n"));
                var footer = new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", cellFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                document.Add(footer);

                document.Close();

                return ms.ToArray();
            }
        }

        // Helper method to get received from name
        private string GetReceivedFromName(Receive receive)
        {
            switch (receive.ReceiveType)
            {
                case "Battalion":
                    return receive.ReceivedFromBattalion?.Name ?? $"Battalion ID: {receive.ReceivedFromBattalionId}";
                case "Range":
                    return receive.ReceivedFromRange?.Name ?? $"Range ID: {receive.ReceivedFromRangeId}";
                case "Zila":
                    return receive.ReceivedFromZila?.Name ?? $"Zila ID: {receive.ReceivedFromZilaId}";
                case "Upazila":
                    return receive.ReceivedFromUpazila?.Name ?? $"Upazila ID: {receive.ReceivedFromUpazilaId}";
                case "Individual":
                    return $"{receive.ReceivedFromIndividualName} ({receive.ReceivedFromIndividualBadgeNo})";
                default:
                    return receive.ReceivedFromIndividualName ?? "Unknown";
            }
        }

        // Create damage record for damaged items
        private async Task CreateDamageRecordAsync(ReceiveItemDto item, int receiveId)
        {
            var damage = new Damage
            {
                DamageNo = await GenerateDamageNoAsync(),
                DamageDate = DateTime.Now,
                ItemId = item.ItemId,
                StoreId = item.StoreId,
                Quantity = item.Quantity,
                DamageType = "Received Damaged",
                Remarks = $"Damaged on receipt - Receive No: {receiveId}",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _userContext.CurrentUserName
            };

            await _unitOfWork.Damages.AddAsync(damage);
        }

        // Get stock level for an item in a store
        public async Task<StockLevelDto> GetStockLevelAsync(int itemId, int? storeId)
        {
            var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                si => si.ItemId == itemId && si.StoreId == storeId
            );

            var item = await _unitOfWork.Items.GetByIdAsync(itemId);

            return new StockLevelDto
            {
                ItemId = itemId,
                StoreId = storeId,
                CurrentStock = storeItem?.Quantity ?? 0,
                AvailableStock = storeItem?.Quantity ?? 0,
                MinimumStock = item?.MinimumStock ?? 0,
                ItemName = item?.Name
            };
        }

        public async Task<IssueDto> ScanIssueVoucherAsync(string qrCode)
        {
            try
            {
                // Parse QR code data
                var parts = qrCode.Split('|');
                if (parts.Length < 3 || parts[0] != "ISSUE")
                {
                    throw new InvalidOperationException("Invalid issue voucher QR code");
                }

                var voucherNumber = parts[1];
                var issueNo = parts[2];

                // Find issue by voucher number
                var issue = await _unitOfWork.Issues
                    .FirstOrDefaultAsync(i => i.VoucherNumber == voucherNumber || i.IssueNo == issueNo);

                if (issue == null)
                {
                    throw new InvalidOperationException("Issue not found");
                }

                return await _issueService.GetIssueByIdAsync(issue.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning issue voucher");
                throw;
            }
        }

        private async Task CreateDamageRecordForItemAsync(ReceiveItem receiveItem)
        {
            var damage = new Damage
            {
                ItemId = receiveItem.ItemId,
                StoreId = receiveItem.StoreId,
                Quantity = receiveItem.Quantity,
                DamageType = receiveItem.Condition,
                DamageDate = DateTime.Now,
                ReportedBy = _userContext.CurrentUserName,
                Cause = $"Received in {receiveItem.Condition} condition",
                Description = receiveItem.Remarks,
                EstimatedLoss = 0, // To be calculated
                ActionTaken = "Recorded during receive process",
                Status = "Pending Review",
                CreatedAt = DateTime.Now,
                CreatedBy = _userContext.CurrentUserName,
                IsActive = true
            };

            await _unitOfWork.Damages.AddAsync(damage);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<ReceiveDto> BulkReceiveByBarcodeAsync(List<string> barcodes, string receivedBy)
        {
            var receiveDto = new ReceiveDto
            {
                ReceiveNo = await GenerateReceiveNoAsync(),
                ReceiveDate = DateTime.Now,
                ReceivedBy = receivedBy,
                Status = "Completed",
                Items = new List<ReceiveItemDto>()
            };

            foreach (var barcode in barcodes)
            {
                var barcodeEntity = await _barcodeService.GetBarcodeByNumberAsync(barcode);
                if (barcodeEntity != null)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(barcodeEntity.ItemId);
                    if (item != null)
                    {
                        receiveDto.Items.Add(new ReceiveItemDto
                        {
                            ItemId = item.Id,
                            ItemCode = item.Code,
                            ItemName = item.Name,
                            Quantity = 1,
                            Condition = "Good"
                        });
                    }
                }
            }

            if (receiveDto.Items.Any())
            {
                return await CreateReceiveAsync(receiveDto);
            }

            return receiveDto;
        }

        public async Task<Dictionary<string, object>> GetReceiveAnalyticsAsync(DateTime fromDate, DateTime toDate)
        {
            var receives = await _unitOfWork.Receives.FindAsync(r =>
                r.ReceiveDate >= fromDate && r.ReceiveDate <= toDate && r.IsActive);

            var receiveItems = new List<ReceiveItem>();
            foreach (var receive in receives)
            {
                var items = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receive.Id);
                receiveItems.AddRange(items);
            }

            return new Dictionary<string, object>
            {
                ["TotalReceives"] = receives.Count(),
                ["ByType"] = receives.GroupBy(r => r.ReceiveType)
                    .Select(g => new { Type = g.Key, Count = g.Count() }).ToList(),
                ["TopItems"] = receiveItems.GroupBy(ri => ri.ItemId)
                    .OrderByDescending(g => g.Sum(ri => ri.Quantity))
                    .Take(10)
                    .Select(g => new { ItemId = g.Key, TotalQuantity = g.Sum(ri => ri.Quantity) }).ToList()
            };
        }

        public async Task<IEnumerable<dynamic>> GetDamageReportAsync(DateTime fromDate, DateTime toDate)
        {
            var damages = await _unitOfWork.Damages.FindAsync(d =>
                d.DamageDate >= fromDate && d.DamageDate <= toDate && d.IsActive);

            return damages.Select(d => new
            {
                Id = d.Id,
                DamageNo = d.DamageNo,
                DamageDate = d.DamageDate,
                ItemId = d.ItemId,
                ItemName = d.Item?.Name,
                Quantity = d.Quantity,
                DamageType = d.DamageType,
                Cause = d.Cause,
                StoreId = d.StoreId,
                StoreName = d.Store?.Name
            });
        }

        public async Task<ReceiveDto> CreateReceiveFromVoucherAsync(string voucherNumber)
        {
            try
            {
                // Find issue by voucher number - check multiple fields
                _logger.LogInformation("Looking for issue with voucher number: {VoucherNumber}", voucherNumber);

                var allIssues = await _unitOfWork.Issues.GetAllAsync();
                var issue = allIssues.FirstOrDefault(i =>
                    i.IsActive &&
                    (i.VoucherNo == voucherNumber ||
                     i.VoucherNumber == voucherNumber ||
                     i.IssueNo == voucherNumber));

                if (issue == null)
                {
                    _logger.LogWarning("Issue not found for voucher number: {VoucherNumber}. Searched VoucherNo, VoucherNumber, and IssueNo fields.", voucherNumber);
                    throw new InvalidOperationException($"Issue not found for voucher number: {voucherNumber}. Please verify the voucher number is correct and the issue has been approved.");
                }

                _logger.LogInformation("Found issue ID {IssueId} for voucher {VoucherNumber}", issue.Id, voucherNumber);
                var issueDto = await _issueService.GetIssueByIdAsync(issue.Id);

                if (issueDto == null)
                    throw new InvalidOperationException("Issue details not found for this voucher");

                if (issueDto.Status != "Approved" && issueDto.Status != "Issued")
                {
                    throw new InvalidOperationException($"Issue must be approved or issued before receiving. Current status: {issueDto.Status}");
                }

                // Check if already received
                var existingReceives = await GetReceivesByIssueIdAsync(issue.Id);
                if (existingReceives.Any(r => r.Status == "Completed"))
                {
                    throw new InvalidOperationException("This issue has already been received");
                }

                // Create receive DTO
                var receive = new ReceiveDto
                {
                    ReceiveNo = await GenerateReceiveNoAsync(),
                    ReceiveDate = DateTime.Now,
                    ReceiveType = issueDto.IssuedToType,
                    ReceivedFromBattalionId = issueDto.IssuedToBattalionId,
                    ReceivedFromRangeId = issueDto.IssuedToRangeId,
                    ReceivedFromZilaId = issueDto.IssuedToZilaId,
                    ReceivedFromUpazilaId = issueDto.IssuedToUpazilaId,
                    ReceivedFromIndividualName = issueDto.IssuedToIndividualName,
                    ReceivedFromIndividualBadgeNo = issueDto.IssuedToIndividualBadgeNo,
                    ReceivedFromBadgeNo = issueDto.IssuedToIndividualBadgeNo,
                    OriginalIssueId = issueDto.Id,
                    OriginalIssueNo = issueDto.IssueNo,
                    OriginalVoucherNo = voucherNumber,
                    ReceivedBy = _userContext.CurrentUserName,
                    Status = "Draft",
                    Items = issueDto.Items.Select(i => new ReceiveItemDto
                    {
                        ItemId = i.ItemId,
                        ItemCode = i.ItemCode,
                        ItemName = i.ItemName,
                        CategoryName = i.CategoryName,
                        StoreId = i.StoreId,
                        StoreName = i.StoreName,
                        Quantity = i.Quantity,
                        IssuedQuantity = i.Quantity,
                        ReceivedQuantity = i.Quantity, // Default to full quantity
                        Condition = "Good", // Default condition
                        Unit = i.Unit,
                        BatchNumber = i.BatchNumber
                    }).ToList()
                };

                // Save receive
                var result = await CreateReceiveAsync(receive);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating receive from voucher");
                throw;
            }
        }

        public async Task<ReceiveDto> AssessItemConditionAsync(int receiveId, int itemId, string condition, string notes)
        {
            try
            {
                var receive = await _unitOfWork.Receives.GetByIdAsync(receiveId);
                if (receive == null) throw new InvalidOperationException("Receive not found");

                var receiveItem = await _unitOfWork.ReceiveItems.FirstOrDefaultAsync(
                    ri => ri.ReceiveId == receiveId && ri.ItemId == itemId);

                if (receiveItem == null) throw new InvalidOperationException("Receive item not found");

                // Update condition
                receiveItem.Condition = condition;
                receiveItem.DamageNotes = notes;
                receiveItem.UpdatedAt = DateTime.UtcNow;
                receiveItem.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.ReceiveItems.Update(receiveItem);
                await _unitOfWork.CompleteAsync();

                // If damaged, create damage record
                if (condition == "Damaged")
                {
                    await CreateDamageRecordAsync(receiveItem, receiveId);
                }

                return await GetReceiveByIdAsync(receiveId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assessing item condition");
                throw;
            }
        }

        public async Task<bool> AddDamagePhotoAsync(int receiveId, int itemId, string photoBase64)
        {
            try
            {
                var receiveItem = await _unitOfWork.ReceiveItems
                    .FirstOrDefaultAsync(ri => ri.ReceiveId == receiveId && ri.ItemId == itemId);

                if (receiveItem == null)
                    return false;

                // Save damage photo
                var fileName = $"damage_receive_{receiveId}_item_{itemId}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                var filePath = await SaveBase64ImageAsync(photoBase64, "damages/receives", fileName);

                receiveItem.DamagePhotoPath = filePath;
                _unitOfWork.ReceiveItems.Update(receiveItem);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving damage photo");
                return false;
            }
        }

        public async Task<bool> CompleteReceiveAsync(int receiveId, string completedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var receive = await _unitOfWork.Receives.GetByIdAsync(receiveId);
                if (receive == null) throw new InvalidOperationException("Receive not found");

                // ✅ FIX: Prevent double stock addition if already completed
                if (receive.Status == "Completed")
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning($"Receive {receiveId} is already completed. Skipping stock adjustment.");
                    return true; // Already completed, return success
                }

                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receiveId);

                // Update stock for each item
                foreach (var item in receiveItems)
                {
                    // Get store item
                    var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                        si => si.ItemId == item.ItemId && si.StoreId == item.StoreId);

                    if (storeItem == null)
                    {
                        // Create new store item if not exists
                        storeItem = new StoreItem
                        {
                            StoreId = item.StoreId,
                            ItemId = item.ItemId,
                            Quantity = 0,
                            MinimumStock = 0,
                            MaximumStock = 1000,
                            ReorderLevel = 10,
                            Status = ItemStatus.Available,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = completedBy,
                            IsActive = true
                        };
                        await _unitOfWork.StoreItems.AddAsync(storeItem);
                    }

                    // Update quantity based on condition
                    if (item.Condition != "Damaged")
                    {
                        storeItem.Quantity += item.ReceivedQuantity;
                    }

                    storeItem.UpdatedAt = DateTime.UtcNow;
                    storeItem.UpdatedBy = completedBy;
                    _unitOfWork.StoreItems.Update(storeItem);

                    // Create stock movement record
                    var movement = new StockMovement
                    {
                        ItemId = item.ItemId,
                        StoreId = item.StoreId,
                        MovementType = "Receive",
                        Quantity = item.ReceivedQuantity,
                        MovementDate = DateTime.Now,
                        ReferenceType = "Receive",
                        ReferenceId = receive.Id,
                        ReferenceNo = receive.ReceiveNo,
                        SourceStoreId = null,
                        DestinationStoreId = item.StoreId,
                        Remarks = $"Received from {receive.ReceivedFromType}",
                        MovedBy = completedBy,
                        OldBalance = (decimal)(storeItem.Quantity - item.ReceivedQuantity),
                        NewBalance = (decimal)storeItem.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = completedBy,
                        IsActive = true
                    };

                    await _unitOfWork.StockMovements.AddAsync(movement);
                }

                // Update receive status
                receive.Status = "Completed";
                receive.ReceivedDate = DateTime.Now;
                receive.UpdatedAt = DateTime.UtcNow;
                receive.UpdatedBy = completedBy;
                _unitOfWork.Receives.Update(receive);

                // After updating receive status, add life tracking
                if (receive.Status == "Completed")
                {
                    // Check if this is a Battalion/VDP receive
                    if ((receive.ReceiveType == "Battalion" || receive.ReceiveType == "VDP") &&
                        receive.ReceivedFromBattalionId.HasValue)
                    {
                        // Start life tracking for controlled items
                        await _personnelItemLifeService.StartLifeTrackingFromReceiveAsync(receive.Id);
                    }
                }


                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error completing receive");
                throw;
            }
        }

        public async Task<bool> UpdateReceivedQuantityAsync(int receiveId, int itemId, decimal quantity)
        {
            try
            {
                var receiveItem = await _unitOfWork.ReceiveItems.FirstOrDefaultAsync(
                    ri => ri.ReceiveId == receiveId && ri.ItemId == itemId);

                if (receiveItem == null) return false;

                receiveItem.ReceivedQuantity = quantity;
                receiveItem.UpdatedAt = DateTime.UtcNow;
                receiveItem.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.ReceiveItems.Update(receiveItem);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating received quantity");
                return false;
            }
        }

        public async Task<IEnumerable<ReceiveDto>> GetReceivesByIssueIdAsync(int issueId)
        {
            var receives = await _unitOfWork.Receives.FindAsync(r => r.OriginalIssueId == issueId);
            return await MapToReceiveDtos(receives);
        }

        public async Task<ReceiveDto> CreateQuickReceiveAsync(QuickReceiveDto dto)
        {
            try
            {
                var receive = new Receive
                {
                    ReceiveNo = dto.ReceiveNo,
                    ReceiveDate = dto.ReceiveDate,
                    ReceiveType = dto.ReceiveType,
                    ReceivedFromIndividualName = dto.ReceivedFromName,
                    ReceivedFromIndividualBadgeNo = dto.BadgeNo,
                    ReceivedBy = dto.ReceivedBy,
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.ReceivedBy,
                    IsActive = true
                };

                await _unitOfWork.Receives.AddAsync(receive);
                await _unitOfWork.CompleteAsync();

                // Add receive item
                var receiveItem = new ReceiveItem
                {
                    ReceiveId = receive.Id,
                    ItemId = dto.ItemId,
                    StoreId = dto.StoreId,
                    Quantity = dto.Quantity,
                    ReceivedQuantity = dto.Quantity,
                    Condition = "Good",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.ReceivedBy,
                    IsActive = true
                };

                await _unitOfWork.ReceiveItems.AddAsync(receiveItem);
                await _unitOfWork.CompleteAsync();

                // Update stock
                await CompleteReceiveAsync(receive.Id, dto.ReceivedBy);

                return await GetReceiveByIdAsync(receive.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quick receive");
                throw;
            }
        }

        private async Task CreateDamageRecordAsync(ReceiveItem item, int receiveId)
        {
            var damage = new Damage
            {
                DamageNo = await GenerateDamageNoAsync(),
                DamageDate = DateTime.Now,
                ItemId = item.ItemId,
                StoreId = item.StoreId,
                Quantity = item.ReceivedQuantity,
                DamageType = "Received Damaged",
                Remarks = $"Damaged on receipt - Receive No: {receiveId}",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _userContext.CurrentUserName
            };

            await _unitOfWork.Damages.AddAsync(damage);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> GenerateDamageNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"DMG{date:yyyyMMdd}";
            var count = await _unitOfWork.Damages.CountAsync(d => d.DamageNo.StartsWith(prefix));
            return $"{prefix}{(count + 1):D4}";
        }

        private async Task<string> SaveBase64ImageAsync(string base64String, string folder, string fileName)
        {
            // Remove data:image/jpeg;base64, prefix if exists
            var base64Data = base64String.Contains(",")
                ? base64String.Split(',')[1]
                : base64String;

            var bytes = Convert.FromBase64String(base64Data);
            var path = Path.Combine("wwwroot", "uploads", folder);
            Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, fileName);
            await File.WriteAllBytesAsync(filePath, bytes);

            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<ReceiveDto> CreateReceiveFromIssueAsync(int issueId, string receivedBy)
        {
            // Implementation for creating receive from issue
            var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
            if (issue == null) throw new Exception("Issue not found");

            var receiveDto = new ReceiveDto
            {
                IssueId = issueId,
                ReceiveDate = DateTime.Now,
                ReceivedBy = receivedBy,
                Status = "Pending"
            };

            return receiveDto;
        }

        public async Task<bool> VerifyVoucherAsync(string voucherNo)
        {
            // Implementation for voucher verification
            var voucher = await _unitOfWork.IssueVouchers
                .FirstOrDefaultAsync(v => v.VoucherNo == voucherNo);

            return voucher != null && voucher.IsActive;
        }

        // Add these methods to your existing ReceiveService.cs class

        public async Task<ReceiveDto> UpdateReceiveAsync(ReceiveDto receiveDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var receive = await _unitOfWork.Receives.GetByIdAsync(receiveDto.Id);
                if (receive == null || !receive.IsActive)
                    throw new InvalidOperationException("Receive not found");

                if (receive.Status == "Completed")
                    throw new InvalidOperationException("Cannot update completed receive");

                // Update receive properties
                receive.ReceiveDate = receiveDto.ReceiveDate;
                receive.ReceiveType = receiveDto.ReceiveType;
                receive.ReceivedFromBattalionId = receiveDto.ReceivedFromBattalionId;
                receive.ReceivedFromRangeId = receiveDto.ReceivedFromRangeId;
                receive.ReceivedFromZilaId = receiveDto.ReceivedFromZilaId;
                receive.ReceivedFromUpazilaId = receiveDto.ReceivedFromUpazilaId;
                receive.ReceivedFromIndividualName = receiveDto.ReceivedFromIndividualName;
                receive.ReceivedFromIndividualBadgeNo = receiveDto.ReceivedFromIndividualBadgeNo;
                receive.Remarks = receiveDto.Remarks;
                receive.UpdatedAt = DateTime.UtcNow;
                receive.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Receives.Update(receive);

                // Update receive items if provided
                if (receiveDto.Items != null && receiveDto.Items.Any())
                {
                    var existingItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receive.Id);

                    // Remove deleted items
                    foreach (var existingItem in existingItems)
                    {
                        if (!receiveDto.Items.Any(i => i.ItemId == existingItem.ItemId))
                        {
                            _unitOfWork.ReceiveItems.Remove(existingItem);
                        }
                    }

                    // Update or add items
                    foreach (var itemDto in receiveDto.Items)
                    {
                        var existingItem = existingItems.FirstOrDefault(i => i.ItemId == itemDto.ItemId);

                        if (existingItem != null)
                        {
                            existingItem.Quantity = itemDto.Quantity;
                            existingItem.StoreId = itemDto.StoreId ?? existingItem.StoreId;
                            existingItem.Condition = itemDto.Condition;
                            existingItem.Remarks = itemDto.Remarks;
                            existingItem.UpdatedAt = DateTime.UtcNow;
                            existingItem.UpdatedBy = _userContext.CurrentUserName;
                            _unitOfWork.ReceiveItems.Update(existingItem);
                        }
                        else
                        {
                            var newItem = new ReceiveItem
                            {
                                ReceiveId = receive.Id,
                                ItemId = itemDto.ItemId,
                                StoreId = itemDto.StoreId ?? 0,
                                Quantity = itemDto.Quantity,
                                ReceivedQuantity = itemDto.Quantity,
                                Condition = itemDto.Condition ?? "Good",
                                Remarks = itemDto.Remarks,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = _userContext.CurrentUserName,
                                IsActive = true
                            };
                            await _unitOfWork.ReceiveItems.AddAsync(newItem);
                        }
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Receive", receive.Id, "Update",
                    $"Updated receive {receive.ReceiveNo}",
                    _userContext.GetCurrentUserId()
                );

                return await GetReceiveByIdAsync(receive.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating receive");
                throw;
            }
        }

        public async Task<bool> DeleteReceiveAsync(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var receive = await _unitOfWork.Receives.GetByIdAsync(id);
                if (receive == null)
                    throw new InvalidOperationException("Receive not found");

                if (receive.Status == "Completed")
                    throw new InvalidOperationException("Cannot delete completed receive");

                // Soft delete
                receive.IsActive = false;
                receive.UpdatedAt = DateTime.UtcNow;
                receive.UpdatedBy = _userContext.CurrentUserName;

                // Soft delete receive items
                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == id);
                foreach (var item in receiveItems)
                {
                    item.IsActive = false;
                    item.UpdatedAt = DateTime.UtcNow;
                    item.UpdatedBy = _userContext.CurrentUserName;
                    _unitOfWork.ReceiveItems.Update(item);
                }

                _unitOfWork.Receives.Update(receive);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Receive", receive.Id, "Delete",
                    $"Deleted receive {receive.ReceiveNo}",
                    _userContext.GetCurrentUserId()
                );

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting receive");
                throw;
            }
        }

        public async Task<bool> CanEditReceiveAsync(int id)
        {
            var receive = await _unitOfWork.Receives.GetByIdAsync(id);
            if (receive == null || !receive.IsActive)
                return false;

            return receive.Status == "Draft" || receive.Status == "Pending";
        }

        public async Task<bool> CanDeleteReceiveAsync(int id)
        {
            var receive = await _unitOfWork.Receives.GetByIdAsync(id);
            if (receive == null || !receive.IsActive)
                return false;

            return receive.Status != "Completed";
        }

        public async Task<bool> IsReceiveCompletedAsync(int id)
        {
            var receive = await _unitOfWork.Receives.GetByIdAsync(id);
            return receive != null && receive.Status == "Completed";
        }

        public async Task<bool> ValidateReceiveAsync(int receiveId)
        {
            try
            {
                var receive = await _unitOfWork.Receives.GetByIdAsync(receiveId);
                if (receive == null)
                    return false;

                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receiveId);

                // Check if all items have conditions assessed
                if (receiveItems.Any(i => string.IsNullOrEmpty(i.Condition)))
                    return false;

                // Check if all items have received quantities
                if (receiveItems.Any(i => i.ReceivedQuantity == null || i.ReceivedQuantity == 0))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating receive");
                return false;
            }
        }

        public async Task<bool> UpdateStockFromReceiveAsync(int receiveId)
        {
            try
            {
                var receive = await _unitOfWork.Receives.GetByIdAsync(receiveId);
                if (receive == null || receive.Status != "Completed")
                    return false;

                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receiveId);

                foreach (var item in receiveItems)
                {
                    // Get or create store item
                    var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                        si => si.ItemId == item.ItemId && si.StoreId == item.StoreId);

                    if (storeItem == null)
                    {
                        storeItem = new StoreItem
                        {
                            StoreId = item.StoreId,
                            ItemId = item.ItemId,
                            Quantity = 0,
                            MinimumStock = 10, // Default values
                            MaximumStock = 1000,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = _userContext.CurrentUserName,
                            IsActive = true
                        };
                        await _unitOfWork.StoreItems.AddAsync(storeItem);
                    }

                    // Update quantity based on condition
                    if (item.Condition == "Good")
                    {
                        storeItem.Quantity = (storeItem.Quantity ?? 0) + (item.ReceivedQuantity ?? item.Quantity);
                    }
                    else if (item.Condition == "Damaged")
                    {
                        // Track damaged items separately - create damage record instead
                        var damageRecord = new Damage
                        {
                            ItemId = item.ItemId,
                            StoreId = item.StoreId,
                            Quantity = item.ReceivedQuantity ?? item.Quantity,
                            DamageDate = DateTime.UtcNow,
                            DamageType = "Received Damaged",
                            Description = "Item received in damaged condition",
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = _userContext.CurrentUserName,
                            IsActive = true
                        };
                        await _unitOfWork.Damages.AddAsync(damageRecord);
                    }

                    storeItem.UpdatedAt = DateTime.UtcNow;
                    storeItem.UpdatedBy = _userContext.CurrentUserName;

                    _unitOfWork.StoreItems.Update(storeItem);

                    // Create stock movement record
                    var stockMovement = new StockMovement
                    {
                        ItemId = item.ItemId,
                        StoreId = item.StoreId,
                        MovementType = "Receive",
                        ReferenceType = "Receive",
                        ReferenceId = receiveId,
                        ReferenceNo = receive.ReceiveNo,
                        Quantity = item.ReceivedQuantity ?? item.Quantity,
                        MovementDate = DateTime.UtcNow,
                        Remarks = $"Received from {receive.ReceiveNo}",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };

                    await _unitOfWork.StockMovements.AddAsync(stockMovement);
                }

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock from receive");
                return false;
            }
        }

        public async Task<IEnumerable<ReceiveDto>> GetPendingReceivesAsync()
        {
            var receives = await _unitOfWork.Receives.FindAsync(r => r.IsActive &&
                (r.Status == "Pending" || r.Status == "Processing"));
            return await MapToReceiveDtos(receives);
        }

        public async Task<IEnumerable<ReceiveDto>> GetReceivesByStatusAsync(string status)
        {
            var receives = await _unitOfWork.Receives.FindAsync(r => r.IsActive && r.Status == status);
            return await MapToReceiveDtos(receives);
        }

        private async Task<IEnumerable<ReceiveDto>> MapToReceiveDtos(IEnumerable<Receive> receives)
        {
            var receiveDtos = new List<ReceiveDto>();

            foreach (var receive in receives)
            {
                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receive.Id);

                // Get original issue items if linked
                List<IssueItemDto> issueItems = null;
                if (receive.OriginalIssueId.HasValue)
                {
                    var issueDto = await _issueService.GetIssueByIdAsync(receive.OriginalIssueId.Value);
                    issueItems = issueDto?.Items?.ToList();
                }

                var items = new List<ReceiveItemDto>();

                foreach (var ri in receiveItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(ri.ItemId);
                    var store = await _unitOfWork.Stores.GetByIdAsync(ri.StoreId);

                    // Get issued quantity from original issue if available
                    var issueItem = issueItems?.FirstOrDefault(ii => ii.ItemId == ri.ItemId);

                    items.Add(new ReceiveItemDto
                    {
                        Id = ri.Id,
                        ItemId = ri.ItemId,
                        ItemCode = item?.Code,
                        ItemName = item?.Name,
                        StoreId = ri.StoreId,
                        StoreName = store?.Name,
                        Quantity = ri.Quantity,
                        IssuedQuantity = issueItem?.Quantity ?? ri.Quantity,
                        ReceivedQuantity = ri.ReceivedQuantity ?? ri.Quantity,
                        Condition = ri.Condition,
                        Remarks = ri.Remarks,
                        Unit = item?.Unit
                    });
                }

                receiveDtos.Add(new ReceiveDto
                {
                    Id = receive.Id,
                    ReceiveNo = receive.ReceiveNo,
                    ReceiveDate = receive.ReceiveDate,
                    ReceiveType = receive.ReceiveType,
                    ReceivedFromBattalionId = receive.ReceivedFromBattalionId,
                    ReceivedFromRangeId = receive.ReceivedFromRangeId,
                    ReceivedFromZilaId = receive.ReceivedFromZilaId,
                    ReceivedFromUpazilaId = receive.ReceivedFromUpazilaId,
                    ReceivedFromIndividualName = receive.ReceivedFromIndividualName,
                    ReceivedFromIndividualBadgeNo = receive.ReceivedFromIndividualBadgeNo,
                    ReceivedFromName = GetReceivedFromName(receive),
                    ReceivedBy = receive.ReceivedBy,
                    OverallCondition = receive.OverallCondition,
                    AssessmentNotes = receive.AssessmentNotes,
                    OriginalIssueId = receive.OriginalIssueId,
                    Status = receive.Status,
                    Remarks = receive.Remarks,
                    Items = items,
                    CreatedAt = receive.CreatedAt,
                    CreatedBy = receive.CreatedBy
                });
            }

            return receiveDtos;
        }

        public async Task<IEnumerable<ReceiveDto>> GetActiveReceivesAsync()
        {
            var receives = await _unitOfWork.Receives.FindAsync(r => r.IsActive && r.Status != "Cancelled");
            return await MapToReceiveDtos(receives);
        }

        public async Task<bool> CancelReceiveAsync(int id, string reason)
        {
            try
            {
                var receive = await _unitOfWork.Receives.GetByIdAsync(id);
                if (receive == null) return false;

                receive.Status = "Cancelled";
                receive.Remarks = reason;
                receive.UpdatedAt = DateTime.UtcNow;
                receive.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Receives.Update(receive);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling receive");
                return false;
            }
        }

        public async Task<Dictionary<string, object>> GetReceiveSummaryAsync(int id)
        {
            var receive = await GetReceiveByIdAsync(id);
            if (receive == null) return null;

            return new Dictionary<string, object>
            {
                ["ReceiveNo"] = receive.ReceiveNo,
                ["TotalItems"] = receive.TotalItems,
                ["TotalQuantity"] = receive.TotalQuantity,
                ["Status"] = receive.Status,
                ["Date"] = receive.ReceiveDate
            };
        }

        public async Task<bool> BulkUpdateItemConditionsAsync(int receiveId, Dictionary<int, string> itemConditions)
        {
            try
            {
                foreach (var kvp in itemConditions)
                {
                    await AssessItemConditionAsync(receiveId, kvp.Key, kvp.Value, null);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk update item conditions");
                return false;
            }
        }

        public async Task<bool> BulkVerifyItemsAsync(int receiveId, List<ReceiveItemDto> items)
        {
            try
            {
                foreach (var item in items)
                {
                    await UpdateReceivedQuantityAsync(receiveId, item.ItemId, item.ReceivedQuantity ?? 0);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk verify items");
                return false;
            }
        }

        public async Task<byte[]> GenerateReceiveReportAsync(int id)
        {
            var receive = await GetReceiveByIdAsync(id);
            if (receive == null) return null;

            // Use existing ExportToPdf method
            return ExportToPdf(new List<ReceiveDto> { receive });
        }

        public async Task<IEnumerable<ReceiveDto>> GetReceivesForReportAsync(DateTime fromDate, DateTime toDate, string status = null)
        {
            var receives = await GetReceivesByDateRangeAsync(fromDate, toDate);
            if (!string.IsNullOrEmpty(status))
            {
                receives = receives.Where(r => r.Status == status);
            }
            return receives;
        }

        public async Task<bool> ReverseStockFromReceiveAsync(int receiveId)
        {
            try
            {
                var receive = await _unitOfWork.Receives.GetByIdAsync(receiveId);
                if (receive == null) return false;

                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receiveId);

                foreach (var item in receiveItems)
                {
                    var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                        si => si.ItemId == item.ItemId && si.StoreId == item.StoreId);

                    if (storeItem != null && item.Condition == "Good")
                    {
                        storeItem.Quantity = (storeItem.Quantity ?? 0) - (item.ReceivedQuantity ?? item.Quantity);
                        storeItem.UpdatedAt = DateTime.UtcNow;
                        storeItem.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.StoreItems.Update(storeItem);
                    }
                }

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reversing stock from receive");
                return false;
            }
        }
    }
}