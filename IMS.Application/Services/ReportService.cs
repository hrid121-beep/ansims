using ClosedXML.Excel;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.IO;
using System.Text;

namespace IMS.Application.Services
{
    public partial class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportService> _logger;
        private const string BANGLA_FONT_PATH = "wwwroot/fonts/kalpurush.ttf";

        public ReportService(IUnitOfWork unitOfWork, ILogger<ReportService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            // Configure QuestPDF license (Community/Free for non-commercial)
            QuestPDF.Settings.License = LicenseType.Community;

            // Load and register Bengali font
            try
            {
                string fontPath = Path.Combine(Directory.GetCurrentDirectory(), BANGLA_FONT_PATH);
                _logger.LogInformation("Attempting to load Bengali font from: {FontPath}", fontPath);

                if (File.Exists(fontPath))
                {
                    var fontBytes = File.ReadAllBytes(fontPath);
                    QuestPDF.Drawing.FontManager.RegisterFontWithCustomName("Kalpurush", new MemoryStream(fontBytes));
                    _logger.LogInformation("✓ Bengali font 'Kalpurush' registered successfully with {Size} bytes", fontBytes.Length);
                }
                else
                {
                    _logger.LogWarning("❌ Bengali font not found at {FontPath}. PDF reports may not render Bengali text correctly.", fontPath);
                    _logger.LogWarning("Please add kalpurush.ttf to wwwroot/fonts/ directory");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to load/register Bengali font");
            }
        }

        // Update the GetStockReportAsync method in ReportService.cs
        public async Task<StockReportDto> GetStockReportAsync(int? storeId = null, int? categoryId = null)
        {
            var stockReportDto = new StockReportDto
            {
                StockItems = new List<StockReportItemDto>()
            };

            // Get all store items based on filter
            var storeItems = await _unitOfWork.StoreItems.GetAllAsync();
            if (storeId.HasValue)
            {
                storeItems = storeItems.Where(si => si.StoreId == storeId.Value);
            }

            // Group by item to get consolidated view
            var itemGroups = storeItems
                .Where(si => si.IsActive)
                .GroupBy(si => si.ItemId);

            decimal? totalValue = 0;
            int totalItems = 0;
            int lowStockItems = 0;
            int outOfStockItems = 0;

            foreach (var itemGroup in itemGroups)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(itemGroup.Key);
                if (item == null || !item.IsActive) continue;

                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                var category = subCategory != null ?
                    await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;

                // Apply category filter
                if (categoryId.HasValue && category?.Id != categoryId.Value)
                    continue;

                // Calculate total stock across all stores
                var totalStock = itemGroup.Sum(si => si.Quantity);

                // Get unit price from last purchase
                var lastPurchase = await _unitOfWork.PurchaseItems
                    .FirstOrDefaultAsync(pi => pi.ItemId == item.Id);
                var unitPrice = lastPurchase?.UnitPrice ?? 0;

                var itemTotalValue = totalStock * unitPrice;
                totalValue += itemTotalValue;

                // Check stock status
                if (totalStock == 0)
                {
                    outOfStockItems++;
                }
                else if (totalStock < item.MinimumStock)
                {
                    lowStockItems++;
                }

                totalItems++;

                // Add details for each store if no specific store filter
                if (!storeId.HasValue)
                {
                    foreach (var storeItem in itemGroup)
                    {
                        var store = await _unitOfWork.Stores.GetByIdAsync(storeItem.StoreId);

                        stockReportDto.StockItems.Add(new StockReportItemDto
                        {
                            ItemCode = item.ItemCode,
                            ItemName = item.Name,
                            CategoryName = category?.Name ?? "N/A",
                            StoreName = store?.Name ?? "N/A",
                            CurrentStock = storeItem.Quantity,
                            MinimumStock = item.MinimumStock,
                            Unit = item.Unit,
                            UnitPrice = unitPrice,
                            TotalValue = storeItem.Quantity * unitPrice
                        });
                    }
                }
                else
                {
                    // Single store view
                    var store = await _unitOfWork.Stores.GetByIdAsync(storeId.Value);

                    stockReportDto.StockItems.Add(new StockReportItemDto
                    {
                        ItemCode = item.ItemCode,
                        ItemName = item.Name,
                        CategoryName = category?.Name ?? "N/A",
                        StoreName = store?.Name ?? "N/A",
                        CurrentStock = totalStock,
                        MinimumStock = item.MinimumStock,
                        Unit = item.Unit,
                        UnitPrice = unitPrice,
                        TotalValue = itemTotalValue
                    });
                }
            }

            // Set summary statistics
            stockReportDto.TotalItems = totalItems;
            stockReportDto.TotalValue = totalValue;
            stockReportDto.LowStockItems = lowStockItems;
            stockReportDto.OutOfStockItems = outOfStockItems;

            return stockReportDto;
        }

        public async Task<IEnumerable<IssueDto>> GetIssueReportAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null)
        {
            var issues = await _unitOfWork.Issues.GetAllAsync();

            if (fromDate.HasValue)
                issues = issues.Where(i => i.IssueDate >= fromDate.Value);

            if (toDate.HasValue)
                issues = issues.Where(i => i.IssueDate <= toDate.Value);

            var issueDtos = new List<IssueDto>();
            foreach (var issue in issues.Where(i => i.IsActive))
            {
                var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);
                var items = new List<IssueItemDto>();

                foreach (var ii in issueItems)
                {
                    // Apply store filter
                    if (storeId.HasValue && ii.StoreId != storeId.Value)
                        continue;

                    var item = await _unitOfWork.Items.GetByIdAsync(ii.ItemId);
                    var store = await _unitOfWork.Stores.GetByIdAsync(ii.StoreId);

                    items.Add(new IssueItemDto
                    {
                        ItemId = ii.ItemId,
                        ItemName = item?.Name,
                        StoreId = ii.StoreId,
                        StoreName = store?.Name,
                        Quantity = ii.Quantity
                    });
                }

                // Only add issue if it has items after filtering
                if (items.Any())
                {
                    issueDtos.Add(new IssueDto
                    {
                        Id = issue.Id,
                        IssueNo = issue.IssueNo,
                        IssueDate = issue.IssueDate,
                        IssuedTo = issue.IssuedTo,
                        IssuedToType = issue.IssuedToType,
                        Purpose = issue.Purpose,
                        Items = items
                    });
                }
            }

            return issueDtos;
        }

        public async Task<IEnumerable<PurchaseDto>> GetPurchaseReportAsync(DateTime? fromDate, DateTime? toDate, int? vendorId = null)
        {
            var purchases = await _unitOfWork.Purchases.GetAllAsync();

            // Apply filters
            if (fromDate.HasValue)
                purchases = purchases.Where(p => p.PurchaseDate >= fromDate.Value);
            if (toDate.HasValue)
                purchases = purchases.Where(p => p.PurchaseDate <= toDate.Value);
            if (vendorId.HasValue)
                purchases = purchases.Where(p => p.VendorId == vendorId.Value);

            var purchaseDtos = new List<PurchaseDto>();

            foreach (var purchase in purchases.Where(p => p.IsActive))
            {
                // VendorId is non-nullable int, so directly use it
                var vendor = await _unitOfWork.Vendors.GetByIdAsync(purchase.VendorId);
                var purchaseItems = await _unitOfWork.PurchaseItems.FindAsync(pi => pi.PurchaseId == purchase.Id);

                var items = new List<PurchaseItemDto>();
                foreach (var pi in purchaseItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(pi.ItemId);
                    items.Add(new PurchaseItemDto
                    {
                        ItemId = pi.ItemId,
                        ItemName = item?.Name,
                        Quantity = pi.Quantity,
                        UnitPrice = pi.UnitPrice,
                        TotalPrice = pi.TotalPrice,
                        StoreId = pi.StoreId
                    });
                }

                purchaseDtos.Add(new PurchaseDto
                {
                    Id = purchase.Id,
                    PurchaseOrderNo = purchase.PurchaseOrderNo,
                    VendorId = purchase.VendorId,
                    VendorName = vendor?.Name,
                    PurchaseDate = purchase.PurchaseDate,
                    TotalAmount = purchase.TotalAmount,
                    Remarks = purchase.Remarks,
                    IsMarketplacePurchase = purchase.IsMarketplacePurchase,
                    Discount = purchase.Discount,
                    PurchaseType = purchase.IsMarketplacePurchase ? "Marketplace" : "Vendor",
                    Items = items
                });
            }

            return purchaseDtos;
        }
        public async Task<IEnumerable<DamageDto>> GetDamageReportAsync(DateTime? fromDate, DateTime? toDate)
        {
            var damages = await _unitOfWork.Damages.GetAllAsync();

            if (fromDate.HasValue)
                damages = damages.Where(d => d.DamageDate >= fromDate.Value);

            if (toDate.HasValue)
                damages = damages.Where(d => d.DamageDate <= toDate.Value);

            var damageDtos = new List<DamageDto>();
            foreach (var damage in damages.Where(d => d.IsActive))
            {
                var item = await _unitOfWork.Items.GetByIdAsync(damage.ItemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(damage.StoreId);

                damageDtos.Add(new DamageDto
                {
                    Id = damage.Id,
                    DamageNo = damage.DamageNo,
                    DamageDate = damage.DamageDate,
                    ItemId = damage.ItemId,
                    ItemName = item?.Name,
                    StoreId = damage.StoreId,
                    StoreName = store?.Name,
                    Quantity = damage.Quantity,
                    DamageType = damage.DamageType,
                    Cause = damage.Cause,
                    ActionTaken = damage.ActionTaken,
                    PhotoPath = damage.PhotoPath
                });
            }

            return damageDtos;
        }

        public async Task<IEnumerable<ReturnDto>> GetReturnReportAsync(DateTime? fromDate, DateTime? toDate)
        {
            var returns = await _unitOfWork.Returns.GetAllAsync();

            if (fromDate.HasValue)
                returns = returns.Where(r => r.ReturnDate >= fromDate.Value);

            if (toDate.HasValue)
                returns = returns.Where(r => r.ReturnDate <= toDate.Value);

            var returnDtos = new List<ReturnDto>();
            foreach (var returnItem in returns.Where(r => r.IsActive))
            {
                var item = await _unitOfWork.Items.GetByIdAsync(returnItem.ItemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(returnItem.StoreId);

                returnDtos.Add(new ReturnDto
                {
                    Id = returnItem.Id,
                    ReturnNo = returnItem.ReturnNo,
                    ReturnDate = returnItem.ReturnDate,
                    ReturnedBy = returnItem.ReturnedBy,
                    ReturnedByType = returnItem.ReturnedByType,
                    Reason = returnItem.Reason,
                    ItemId = returnItem.ItemId,
                    ItemName = item?.Name,
                    StoreId = returnItem.StoreId,
                    StoreName = store?.Name,
                    Quantity = returnItem.Quantity,
                    IsRestocked = returnItem.IsRestocked
                });
            }

            return returnDtos;
        }

        public async Task<IEnumerable<WriteOffDto>> GetWriteOffReportAsync(DateTime? fromDate, DateTime? toDate)
        {
            var writeOffs = await _unitOfWork.WriteOffs.GetAllAsync();

            if (fromDate.HasValue)
                writeOffs = writeOffs.Where(w => w.WriteOffDate >= fromDate.Value);

            if (toDate.HasValue)
                writeOffs = writeOffs.Where(w => w.WriteOffDate <= toDate.Value);

            var writeOffDtos = new List<WriteOffDto>();
            foreach (var writeOff in writeOffs.Where(w => w.IsActive))
            {
                var writeOffItems = await _unitOfWork.WriteOffItems.FindAsync(wi => wi.WriteOffId == writeOff.Id);

                var items = new List<WriteOffItemDto>();
                foreach (var wi in writeOffItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(wi.ItemId);
                    var store = await _unitOfWork.Stores.GetByIdAsync(wi.StoreId);

                    items.Add(new WriteOffItemDto
                    {
                        ItemId = wi.ItemId,
                        ItemName = item?.Name,
                        StoreId = wi.StoreId,
                        StoreName = store?.Name,
                        Quantity = wi.Quantity,
                        Value = wi.Value
                    });
                }

                writeOffDtos.Add(new WriteOffDto
                {
                    Id = writeOff.Id,
                    WriteOffNo = writeOff.WriteOffNo,
                    WriteOffDate = writeOff.WriteOffDate,
                    Reason = writeOff.Reason,
                    ApprovedBy = writeOff.ApprovedBy,
                    ApprovedDate = writeOff.ApprovedDate,
                    Items = items
                });
            }

            return writeOffDtos;
        }

        public async Task<IEnumerable<TransferDto>> GetTransferReportAsync(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null)
        {
            var transfers = await _unitOfWork.Transfers.GetAllAsync();

            if (fromDate.HasValue)
                transfers = transfers.Where(t => t.TransferDate >= fromDate.Value);

            if (toDate.HasValue)
                transfers = transfers.Where(t => t.TransferDate <= toDate.Value);

            if (fromStoreId.HasValue)
                transfers = transfers.Where(t => t.FromStoreId == fromStoreId.Value);

            if (toStoreId.HasValue)
                transfers = transfers.Where(t => t.ToStoreId == toStoreId.Value);

            var transferDtos = new List<TransferDto>();
            foreach (var transfer in transfers.Where(t => t.IsActive))
            {
                var fromStore = await _unitOfWork.Stores.GetByIdAsync(transfer.FromStoreId);
                var toStore = await _unitOfWork.Stores.GetByIdAsync(transfer.ToStoreId);
                var transferItems = await _unitOfWork.TransferItems.FindAsync(ti => ti.TransferId == transfer.Id);

                var items = new List<TransferItemDto>();
                foreach (var ti in transferItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(ti.ItemId);
                    items.Add(new TransferItemDto
                    {
                        ItemId = ti.ItemId,
                        ItemName = item?.Name,
                        Quantity = ti.Quantity
                    });
                }

                transferDtos.Add(new TransferDto
                {
                    Id = transfer.Id,
                    TransferNo = transfer.TransferNo,
                    TransferDate = transfer.TransferDate,
                    FromStoreId = transfer.FromStoreId,
                    FromStoreName = fromStore?.Name,
                    ToStoreId = transfer.ToStoreId,
                    ToStoreName = toStore?.Name,
                    Remarks = transfer.Remarks,
                    Items = items
                });
            }

            return transferDtos;
        }

        public async Task<IEnumerable<InventoryMovementDto>> GetInventoryMovementReportAsync(DateTime? fromDate, DateTime? toDate, int? itemId = null)
        {
            var movements = new List<InventoryMovementDto>();

            // Get purchases
            var purchases = await GetPurchaseReportAsync(fromDate, toDate);
            foreach (var purchase in purchases)
            {
                foreach (var item in purchase.Items)
                {
                    if (!itemId.HasValue || item.ItemId == itemId.Value)
                    {
                        movements.Add(new InventoryMovementDto
                        {
                            MovementDate = purchase.PurchaseDate,
                            MovementType = "Purchase",
                            ReferenceNo = purchase.PurchaseOrderNo,
                            ItemId = item.ItemId,
                            ItemName = item.ItemName,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            MovedBy = "System", // TODO: Get from purchase
                            Remarks = purchase.Remarks
                        });
                    }
                }
            }

            // Get issues
            var issues = await GetIssueReportAsync(fromDate, toDate);
            foreach (var issue in issues)
            {
                foreach (var item in issue.Items)
                {
                    if (!itemId.HasValue || item.ItemId == itemId.Value)
                    {
                        movements.Add(new InventoryMovementDto
                        {
                            MovementDate = issue.IssueDate,
                            MovementType = "Issue",
                            ReferenceNo = issue.IssueNo,
                            ItemId = item.ItemId,
                            ItemName = item.ItemName,
                            Quantity = -item.Quantity, // Negative for outgoing
                            MovedBy = "System", // TODO: Get from issue
                            Remarks = $"Issued to: {issue.IssuedTo}"
                        });
                    }
                }
            }

            return movements.OrderBy(m => m.MovementDate);
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var items = await _unitOfWork.Items.GetAllAsync();
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var issues = await _unitOfWork.Issues.GetAllAsync();
            var purchases = await _unitOfWork.Purchases.GetAllAsync();
            var categories = await _unitOfWork.Categories.GetAllAsync();

            var activeItems = items.Where(i => i.IsActive);
            var activeStores = stores.Where(s => s.IsActive);
            var recentIssues = issues.Where(i => i.IsActive && i.IssueDate >= DateTime.Now.AddDays(-30));
            var recentPurchases = purchases.Where(p => p.IsActive && p.PurchaseDate >= DateTime.Now.AddMonths(-1));

            // Get low stock items
            var lowStockCount = 0;
            var categoryStock = new List<CategoryStockDto>();

            foreach (var category in categories.Where(c => c.IsActive))
            {
                var categoryItemCount = 0;
                var categoryValue = 0m;

                var subCategories = await _unitOfWork.SubCategories.FindAsync(sc => sc.CategoryId == category.Id && sc.IsActive);
                foreach (var subCategory in subCategories)
                {
                    var categoryItems = await _unitOfWork.Items.FindAsync(i => i.SubCategoryId == subCategory.Id && i.IsActive);
                    categoryItemCount += categoryItems.Count();

                    foreach (var item in categoryItems)
                    {
                        var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);
                        var totalStock = storeItems.Sum(si => si.Quantity);

                        if (totalStock < item.MinimumStock)
                            lowStockCount++;

                        // Calculate value based on last purchase price
                        var lastPurchase = await _unitOfWork.PurchaseItems
                            .FirstOrDefaultAsync(pi => pi.ItemId == item.Id);
                        if (lastPurchase != null)
                        {
                            categoryValue += (decimal)(totalStock * lastPurchase.UnitPrice);
                        }
                    }
                }

                categoryStock.Add(new CategoryStockDto
                {
                    CategoryName = category.Name,
                    ItemCount = categoryItemCount,
                    TotalValue = categoryValue
                });
            }

            return new DashboardStatsDto
            {
                TotalItems = activeItems.Count(),
                TotalStores = activeStores.Count(),
                PendingIssues = recentIssues.Count(),
                LowStockItems = lowStockCount,
                MonthlyPurchases = recentPurchases.Count(),
                MonthlyIssues = recentIssues.Count(),
                MonthlyPurchaseValue = recentPurchases.Sum(p => p.TotalAmount),
                CategoryStock = categoryStock
            };
        }


        public async Task<IEnumerable<StockLevelDto>> GetStockLevelsAsync()
        {
            var stockLevels = new List<StockLevelDto>();
            var items = await _unitOfWork.Items.GetAllAsync();

            foreach (var item in items.Where(i => i.IsActive))
            {
                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                var category = await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId);
                var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);

                var totalStock = storeItems.Sum(si => si.Quantity);
                var status = "Good";

                if (totalStock == 0)
                    status = "Out of Stock";
                else if (totalStock < item.MinimumStock * 0.5m)
                    status = "Critical";
                else if (totalStock < item.MinimumStock)
                    status = "Low";

                var storeStocks = new List<StoreStockDto>();
                foreach (var storeItem in storeItems)
                {
                    var store = await _unitOfWork.Stores.GetByIdAsync(storeItem.StoreId);
                    storeStocks.Add(new StoreStockDto
                    {
                        StoreId = storeItem.StoreId,
                        StoreName = store?.Name,
                        Quantity = storeItem.Quantity,
                        Status = storeItem.Status.ToString()
                    });
                }

                stockLevels.Add(new StockLevelDto
                {
                    ItemId = item.Id,
                    ItemCode = item.ItemCode,
                    ItemName = item.Name,
                    CategoryName = category?.Name,
                    CurrentStock = totalStock,
                    MinimumStock = item.MinimumStock,
                    ReorderLevel = item.MinimumStock * 1.5m,
                    StockStatus = status,
                    StoreStocks = new List<StoreStockInfo>()
                });
            }

            return stockLevels;
        }

        public async Task<IEnumerable<CategoryStockDto>> GetCategoryWiseStockAsync()
        {
            var categoryStocks = new List<CategoryStockDto>();
            var categories = await _unitOfWork.Categories.GetAllAsync();

            foreach (var category in categories.Where(c => c.IsActive))
            {
                var itemCount = 0;
                var totalValue = 0m;

                var subCategories = await _unitOfWork.SubCategories.FindAsync(sc => sc.CategoryId == category.Id && sc.IsActive);
                foreach (var subCategory in subCategories)
                {
                    var items = await _unitOfWork.Items.FindAsync(i => i.SubCategoryId == subCategory.Id && i.IsActive);
                    itemCount += items.Count();

                    foreach (var item in items)
                    {
                        var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);
                        var totalStock = storeItems.Sum(si => si.Quantity);

                        // Get last purchase price
                        var lastPurchase = await _unitOfWork.PurchaseItems
                            .FirstOrDefaultAsync(pi => pi.ItemId == item.Id);
                        if (lastPurchase != null)
                        {
                            totalValue += (decimal)(totalStock * lastPurchase.UnitPrice);
                        }
                    }
                }

                categoryStocks.Add(new CategoryStockDto
                {
                    CategoryName = category.Name,
                    ItemCount = itemCount,
                    TotalValue = totalValue
                });
            }

            return categoryStocks;
        }

        public async Task<byte[]> GeneratePurchaseReportExcelAsync(DateTime? fromDate, DateTime? toDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Purchase Report");

                // Headers
                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "PO Number";
                worksheet.Cells[1, 3].Value = "Purchase Date";
                worksheet.Cells[1, 4].Value = "Vendor";
                worksheet.Cells[1, 5].Value = "Total Amount";
                worksheet.Cells[1, 6].Value = "Discount";
                worksheet.Cells[1, 7].Value = "Net Amount";
                worksheet.Cells[1, 8].Value = "Purchase Type";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                var purchases = await GetPurchaseReportAsync(fromDate, toDate);
                int row = 2;
                int serialNo = 1;

                foreach (var purchase in purchases)
                {
                    worksheet.Cells[row, 1].Value = serialNo;
                    worksheet.Cells[row, 2].Value = purchase.PurchaseOrderNo;
                    worksheet.Cells[row, 3].Value = purchase.PurchaseDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = purchase.VendorName ?? "Marketplace";
                    worksheet.Cells[row, 5].Value = purchase.TotalAmount;
                    worksheet.Cells[row, 6].Value = purchase.Discount;
                    worksheet.Cells[row, 7].Value = purchase.TotalAmount - purchase.Discount;
                    worksheet.Cells[row, 8].Value = purchase.PurchaseType;

                    row++;
                    serialNo++;
                }

                // Add totals
                row++;
                worksheet.Cells[row, 4].Value = "TOTAL:";
                worksheet.Cells[row, 4].Style.Font.Bold = true;
                worksheet.Cells[row, 5].Formula = $"SUM(E2:E{row - 1})";
                worksheet.Cells[row, 6].Formula = $"SUM(F2:F{row - 1})";
                worksheet.Cells[row, 7].Formula = $"SUM(G2:G{row - 1})";

                // Format currency columns
                worksheet.Column(5).Style.Numberformat.Format = "#,##0.00";
                worksheet.Column(6).Style.Numberformat.Format = "#,##0.00";
                worksheet.Column(7).Style.Numberformat.Format = "#,##0.00";

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> GenerateIssueReportExcelAsync(DateTime? fromDate, DateTime? toDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Issue Report");

                // Headers
                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "Issue No";
                worksheet.Cells[1, 3].Value = "Issue Date";
                worksheet.Cells[1, 4].Value = "Issued To";
                worksheet.Cells[1, 5].Value = "Type";
                worksheet.Cells[1, 6].Value = "Purpose";
                worksheet.Cells[1, 7].Value = "Total Items";
                worksheet.Cells[1, 8].Value = "Total Quantity";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                var issues = await GetIssueReportAsync(fromDate, toDate);
                int row = 2;
                int serialNo = 1;

                foreach (var issue in issues)
                {
                    worksheet.Cells[row, 1].Value = serialNo;
                    worksheet.Cells[row, 2].Value = issue.IssueNo;
                    worksheet.Cells[row, 3].Value = issue.IssueDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = issue.IssuedTo;
                    worksheet.Cells[row, 5].Value = issue.IssuedToType;
                    worksheet.Cells[row, 6].Value = issue.Purpose;
                    worksheet.Cells[row, 7].Value = issue.Items.Count();
                    worksheet.Cells[row, 8].Value = issue.Items.Sum(i => i.Quantity);

                    row++;
                    serialNo++;
                }

                // Add summary
                row++;
                worksheet.Cells[row, 1].Value = "Summary:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                row++;
                worksheet.Cells[row, 1].Value = "Total Issues:";
                worksheet.Cells[row, 2].Value = issues.Count();
                row++;
                worksheet.Cells[row, 1].Value = "Period:";
                worksheet.Cells[row, 2].Value = $"{fromDate?.ToString("yyyy-MM-dd") ?? "All"} to {toDate?.ToString("yyyy-MM-dd") ?? "All"}";

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        public async Task<object> GetInventorySummaryAsync()
        {
            var stats = await GetDashboardStatsAsync();
            var stockLevels = await GetStockLevelsAsync();

            return new
            {
                Stats = stats,
                StockLevels = stockLevels.Take(10),
                LastUpdated = DateTime.Now
            };
        }

        public async Task<IEnumerable<VendorDto>> GetVendorsAsync()
        {
            var vendors = await _unitOfWork.Vendors.GetAllAsync();
            return vendors.Where(v => v.IsActive).Select(v => new VendorDto
            {
                Id = v.Id,
                Name = v.Name,
                ContactPerson = v.ContactPerson,
                Phone = v.Phone,
                Email = v.Email,
                Address = v.Address,
                IsActive = v.IsActive,
                CreatedAt = v.CreatedAt
            });
        }


        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = await _unitOfWork.Items.GetAllAsync();
            var itemDtos = new List<ItemDto>();

            foreach (var item in items.Where(i => i.IsActive))
            {
                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;

                itemDtos.Add(new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    ItemCode = item.ItemCode,
                    Description = item.Description,
                    SubCategoryId = item.SubCategoryId,
                    SubCategoryName = subCategory?.Name,
                    CategoryName = category?.Name,
                    Unit = item.Unit,
                    MinimumStock = item.MinimumStock,
                    IsActive = item.IsActive,
                    CreatedAt = item.CreatedAt
                });
            }

            return itemDtos;
        }


        public async Task<object> GetReportStatisticsAsync()
        {
            var today = DateTime.Now.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            var purchases = await _unitOfWork.Purchases.GetAllAsync();
            var issues = await _unitOfWork.Issues.GetAllAsync();

            var thisMonthPurchases = purchases.Count(p => p.PurchaseDate >= thisMonth && p.IsActive);
            var lastMonthPurchases = purchases.Count(p => p.PurchaseDate >= lastMonth && p.PurchaseDate < thisMonth && p.IsActive);

            var thisMonthIssues = issues.Count(i => i.IssueDate >= thisMonth && i.IsActive);
            var lastMonthIssues = issues.Count(i => i.IssueDate >= lastMonth && i.IssueDate < thisMonth && i.IsActive);

            return new
            {
                PurchasesThisMonth = thisMonthPurchases,
                PurchasesLastMonth = lastMonthPurchases,
                PurchaseTrend = thisMonthPurchases - lastMonthPurchases,
                IssuesThisMonth = thisMonthIssues,
                IssuesLastMonth = lastMonthIssues,
                IssueTrend = thisMonthIssues - lastMonthIssues,
                GeneratedAt = DateTime.Now
            };
        }

        public async Task<IEnumerable<object>> GetLossReportAsync(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null)
        {
            var losses = new List<object>();

            // Get damages
            var damages = await GetDamageReportAsync(fromDate, toDate);
            if (string.IsNullOrEmpty(lossType) || lossType.Equals("Damage", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var damage in damages)
                {
                    if (!storeId.HasValue || damage.StoreId == storeId.Value)
                    {
                        losses.Add(new
                        {
                            Id = damage.Id,
                            LossNo = damage.DamageNo,
                            LossDate = damage.DamageDate,
                            LossType = "Damage",
                            ItemId = damage.ItemId,
                            ItemName = damage.ItemName,
                            StoreId = damage.StoreId,
                            StoreName = damage.StoreName,
                            Quantity = damage.Quantity,
                            Reason = damage.Cause,
                            Value = 0m
                        });
                    }
                }
            }

            // Get write-offs
            var writeOffs = await GetWriteOffReportAsync(fromDate, toDate);
            if (string.IsNullOrEmpty(lossType) || lossType.Equals("WriteOff", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var writeOff in writeOffs)
                {
                    foreach (var item in writeOff.Items)
                    {
                        if (!storeId.HasValue || item.StoreId == storeId.Value)
                        {
                            losses.Add(new
                            {
                                Id = writeOff.Id,
                                LossNo = writeOff.WriteOffNo,
                                LossDate = writeOff.WriteOffDate,
                                LossType = "WriteOff",
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                StoreId = item.StoreId,
                                StoreName = item.StoreName,
                                Quantity = item.Quantity,
                                Reason = writeOff.Reason,
                                Value = item.Value
                            });
                        }
                    }
                }
            }

            return losses.OrderByDescending(l => ((dynamic)l).LossDate);
        }

        public async Task<IEnumerable<object>> GetMovementHistoryAsync(DateTime? fromDate, DateTime? toDate, int? itemId = null, string movementType = null, int? storeId = null)
        {
            var movements = new List<object>();

            // Get purchases
            if (string.IsNullOrEmpty(movementType) || movementType.Equals("Purchase", StringComparison.OrdinalIgnoreCase))
            {
                var purchases = await GetPurchaseReportAsync(fromDate, toDate);
                foreach (var purchase in purchases)
                {
                    foreach (var item in purchase.Items)
                    {
                        if ((!itemId.HasValue || item.ItemId == itemId.Value) &&
                            (!storeId.HasValue || item.StoreId == storeId.Value))
                        {
                            movements.Add(new
                            {
                                MovementDate = purchase.PurchaseDate,
                                MovementType = "Purchase",
                                ReferenceNo = purchase.PurchaseOrderNo,
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                StoreId = item.StoreId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                TotalValue = item.TotalPrice,
                                MovedBy = "System",
                                Remarks = $"Purchase from {purchase.VendorName ?? "Marketplace"}"
                            });
                        }
                    }
                }
            }

            // Get issues
            if (string.IsNullOrEmpty(movementType) || movementType.Equals("Issue", StringComparison.OrdinalIgnoreCase))
            {
                var issues = await GetIssueReportAsync(fromDate, toDate, storeId);
                foreach (var issue in issues)
                {
                    foreach (var item in issue.Items)
                    {
                        if (!itemId.HasValue || item.ItemId == itemId.Value)
                        {
                            movements.Add(new
                            {
                                MovementDate = issue.IssueDate,
                                MovementType = "Issue",
                                ReferenceNo = issue.IssueNo,
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                StoreId = item.StoreId,
                                Quantity = -item.Quantity,
                                UnitPrice = (decimal?)null,
                                TotalValue = (decimal?)null,
                                MovedBy = "System",
                                Remarks = $"Issued to {issue.IssuedTo} ({issue.IssuedToType})"
                            });
                        }
                    }
                }
            }

            // Get transfers
            if (string.IsNullOrEmpty(movementType) || movementType.Equals("Transfer", StringComparison.OrdinalIgnoreCase))
            {
                var transfers = await GetTransferReportAsync(fromDate, toDate, storeId, storeId);
                foreach (var transfer in transfers)
                {
                    foreach (var item in transfer.Items)
                    {
                        if (!itemId.HasValue || item.ItemId == itemId.Value)
                        {
                            movements.Add(new
                            {
                                MovementDate = transfer.TransferDate,
                                MovementType = "Transfer Out",
                                ReferenceNo = transfer.TransferNo,
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                StoreId = transfer.FromStoreId,
                                Quantity = -item.Quantity,
                                UnitPrice = (decimal?)null,
                                TotalValue = (decimal?)null,
                                MovedBy = "System",
                                Remarks = $"Transferred to {transfer.ToStoreName}"
                            });

                            movements.Add(new
                            {
                                MovementDate = transfer.TransferDate,
                                MovementType = "Transfer In",
                                ReferenceNo = transfer.TransferNo,
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                StoreId = transfer.ToStoreId,
                                Quantity = item.Quantity,
                                UnitPrice = (decimal?)null,
                                TotalValue = (decimal?)null,
                                MovedBy = "System",
                                Remarks = $"Transferred from {transfer.FromStoreName}"
                            });
                        }
                    }
                }
            }

            return movements.OrderByDescending(m => ((dynamic)m).MovementDate);
        }

        public async Task<byte[]> GenerateTransferReportExcelAsync(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Transfer Report");

                // Headers
                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "Transfer No";
                worksheet.Cells[1, 3].Value = "Transfer Date";
                worksheet.Cells[1, 4].Value = "From Store";
                worksheet.Cells[1, 5].Value = "To Store";
                worksheet.Cells[1, 6].Value = "Total Items";
                worksheet.Cells[1, 7].Value = "Total Quantity";
                worksheet.Cells[1, 8].Value = "Remarks";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                var transfers = await GetTransferReportAsync(fromDate, toDate, fromStoreId, toStoreId);
                int row = 2;
                int serialNo = 1;

                foreach (var transfer in transfers)
                {
                    worksheet.Cells[row, 1].Value = serialNo;
                    worksheet.Cells[row, 2].Value = transfer.TransferNo;
                    worksheet.Cells[row, 3].Value = transfer.TransferDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = transfer.FromStoreName;
                    worksheet.Cells[row, 5].Value = transfer.ToStoreName;
                    worksheet.Cells[row, 6].Value = transfer.Items.Count();
                    worksheet.Cells[row, 7].Value = transfer.Items.Sum(i => i.Quantity);
                    worksheet.Cells[row, 8].Value = transfer.Remarks;

                    row++;
                    serialNo++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> GenerateTransferReportCsvAsync(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null)
        {
            var transfers = await GetTransferReportAsync(fromDate, toDate, fromStoreId, toStoreId);

            var csv = new System.Text.StringBuilder();

            // Add UTF-8 BOM for Excel compatibility
            csv.Append("\uFEFF");

            // Headers with serial column
            csv.AppendLine("#Ser,Transfer No,Transfer Date,From Store,To Store,Items,Total Quantity,Status,Remarks");

            // Data rows
            int serial = 1;
            foreach (var transfer in transfers)
            {
                var totalQuantity = transfer.Items.Sum(i => i.Quantity);
                csv.AppendLine($"{serial}," +
                              $"\"{EscapeCsv(transfer.TransferNo)}\"," +
                              $"\"{transfer.TransferDate:yyyy-MM-dd}\"," +
                              $"\"{EscapeCsv(transfer.FromStoreName)}\"," +
                              $"\"{EscapeCsv(transfer.ToStoreName)}\"," +
                              $"{transfer.Items.Count()}," +
                              $"{totalQuantity}," +
                              $"\"{EscapeCsv(transfer.Status)}\"," +
                              $"\"{EscapeCsv(transfer.Remarks ?? "")}\"");
                serial++;
            }

            return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> GenerateTransferReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null)
        {
            var transfers = await GetTransferReportAsync(fromDate, toDate, fromStoreId, toStoreId);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Organization Header
            var orgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18, new iTextSharp.text.BaseColor(64, 64, 64));
            var orgName = new iTextSharp.text.Paragraph("ANSAR & VDP", orgFont);
            orgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(orgName);

            var subOrgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 12, new iTextSharp.text.BaseColor(64, 64, 64));
            var subOrgName = new iTextSharp.text.Paragraph("Inventory Management System", subOrgFont);
            subOrgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            subOrgName.SpacingAfter = 5f;
            document.Add(subOrgName);

            // Report Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16, new iTextSharp.text.BaseColor(0, 0, 0));
            var title = new iTextSharp.text.Paragraph("TRANSFER REPORT", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            title.SpacingBefore = 10f;
            document.Add(title);

            // Filter Info
            string filterInfo = "All Transfers";
            if (fromDate.HasValue && toDate.HasValue)
            {
                filterInfo = $"Period: {fromDate.Value:dd MMM yyyy} to {toDate.Value:dd MMM yyyy}";
            }
            else if (fromDate.HasValue)
            {
                filterInfo = $"From: {fromDate.Value:dd MMM yyyy}";
            }
            else if (toDate.HasValue)
            {
                filterInfo = $"Until: {toDate.Value:dd MMM yyyy}";
            }

            if (fromStoreId.HasValue)
            {
                var fromStore = transfers.FirstOrDefault()?.FromStoreName;
                if (!string.IsNullOrEmpty(fromStore))
                    filterInfo += $" | From Store: {fromStore}";
            }

            if (toStoreId.HasValue)
            {
                var toStore = transfers.FirstOrDefault()?.ToStoreName;
                if (!string.IsNullOrEmpty(toStore))
                    filterInfo += $" | To Store: {toStore}";
            }

            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);
            var filterPara = new iTextSharp.text.Paragraph(filterInfo, infoFont);
            filterPara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(filterPara);

            var reportDate = new iTextSharp.text.Paragraph($"Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}", infoFont);
            reportDate.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            reportDate.SpacingAfter = 15f;
            document.Add(reportDate);

            // Summary Section
            var totalTransfers = transfers.Count();
            var totalItems = transfers.Sum(t => t.Items.Count());
            var totalQuantity = transfers.Sum(t => t.Items.Sum(i => i.Quantity));

            var summaryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var summaryText = $"Total Transfers: {totalTransfers} | Total Items: {totalItems} | Total Quantity: {totalQuantity}";
            var summaryPara = new iTextSharp.text.Paragraph(summaryText, summaryFont);
            summaryPara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            summaryPara.SpacingAfter = 10f;
            document.Add(summaryPara);

            // Table with 8 columns
            var table = new iTextSharp.text.pdf.PdfPTable(8);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 12f, 10f, 15f, 15f, 8f, 10f, 10f });
            table.SpacingBefore = 10f;

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 8, new iTextSharp.text.BaseColor(255, 255, 255));
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 7);

            string[] headers = { "#Ser", "Transfer No", "Date", "From Store", "To Store", "Items", "Quantity", "Status" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(41, 128, 185); // Professional blue
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                cell.Padding = 6;
                cell.BorderWidth = 1f;
                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
                table.AddCell(cell);
            }

            // Data rows
            int serialNo = 1;
            bool isAlternateRow = false;
            var alternateBgColor = new iTextSharp.text.BaseColor(245, 245, 245);
            var whiteBgColor = new iTextSharp.text.BaseColor(255, 255, 255);

            foreach (var transfer in transfers.OrderByDescending(t => t.TransferDate))
            {
                var rowBgColor = isAlternateRow ? alternateBgColor : whiteBgColor;

                // Serial Number
                var serCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                serCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                serCell.Padding = 4;
                serCell.BackgroundColor = rowBgColor;
                serCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(serCell);

                // Transfer Number
                var transferNoCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(transfer.TransferNo ?? "", cellFont));
                transferNoCell.Padding = 4;
                transferNoCell.BackgroundColor = rowBgColor;
                transferNoCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(transferNoCell);

                // Date
                var dateCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(transfer.TransferDate.ToString("dd-MMM-yyyy"), cellFont));
                dateCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                dateCell.Padding = 4;
                dateCell.BackgroundColor = rowBgColor;
                dateCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(dateCell);

                // From Store
                var fromStoreCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(transfer.FromStoreName ?? "", cellFont));
                fromStoreCell.Padding = 4;
                fromStoreCell.BackgroundColor = rowBgColor;
                fromStoreCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(fromStoreCell);

                // To Store
                var toStoreCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(transfer.ToStoreName ?? "", cellFont));
                toStoreCell.Padding = 4;
                toStoreCell.BackgroundColor = rowBgColor;
                toStoreCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(toStoreCell);

                // Items Count
                var itemsCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(transfer.Items.Count().ToString(), cellFont));
                itemsCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                itemsCell.Padding = 4;
                itemsCell.BackgroundColor = rowBgColor;
                itemsCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(itemsCell);

                // Total Quantity
                var quantityCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(transfer.Items.Sum(i => i.Quantity).ToString(), cellFont));
                quantityCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                quantityCell.Padding = 4;
                quantityCell.BackgroundColor = rowBgColor;
                quantityCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(quantityCell);

                // Status
                var statusCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(transfer.Status ?? "", cellFont));
                statusCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                statusCell.Padding = 4;
                statusCell.BackgroundColor = rowBgColor;
                statusCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(statusCell);

                serialNo++;
                isAlternateRow = !isAlternateRow;
            }

            document.Add(table);

            // Footer
            var footerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8, new iTextSharp.text.BaseColor(128, 128, 128));
            var footer = new iTextSharp.text.Paragraph("\n\nThis is a system-generated report from ANSAR & VDP Inventory Management System.", footerFont);
            footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(footer);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateLossReportExcelAsync(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Loss Report");

                // Headers
                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "Loss No";
                worksheet.Cells[1, 3].Value = "Loss Date";
                worksheet.Cells[1, 4].Value = "Loss Type";
                worksheet.Cells[1, 5].Value = "Item Name";
                worksheet.Cells[1, 6].Value = "Store";
                worksheet.Cells[1, 7].Value = "Quantity";
                worksheet.Cells[1, 8].Value = "Value";
                worksheet.Cells[1, 9].Value = "Reason";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                var losses = await GetLossReportAsync(fromDate, toDate, lossType, storeId);
                int row = 2;
                int serialNo = 1;

                foreach (var loss in losses)
                {
                    var lossItem = (dynamic)loss;
                    worksheet.Cells[row, 1].Value = serialNo;
                    worksheet.Cells[row, 2].Value = lossItem.LossNo;
                    worksheet.Cells[row, 3].Value = ((DateTime)lossItem.LossDate).ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = lossItem.LossType;
                    worksheet.Cells[row, 5].Value = lossItem.ItemName;
                    worksheet.Cells[row, 6].Value = lossItem.StoreName;
                    worksheet.Cells[row, 7].Value = lossItem.Quantity;
                    worksheet.Cells[row, 8].Value = lossItem.Value;
                    worksheet.Cells[row, 9].Value = lossItem.Reason;

                    row++;
                    serialNo++;
                }

                worksheet.Column(8).Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> GenerateLossReportCsvAsync(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null)
        {
            var losses = await GetLossReportAsync(fromDate, toDate, lossType, storeId);

            var csv = new System.Text.StringBuilder();

            // Add UTF-8 BOM for Excel compatibility
            csv.Append("\uFEFF");

            // Headers with serial column
            csv.AppendLine("#Ser,Loss No,Loss Date,Type,Item,Store,Quantity,Value,Reason");

            // Data rows
            int serial = 1;
            foreach (var loss in losses)
            {
                var lossItem = (dynamic)loss;
                csv.AppendLine($"{serial}," +
                              $"\"{EscapeCsv(lossItem.LossNo)}\"," +
                              $"\"{((DateTime)lossItem.LossDate):yyyy-MM-dd}\"," +
                              $"\"{EscapeCsv(lossItem.LossType)}\"," +
                              $"\"{EscapeCsv(lossItem.ItemName)}\"," +
                              $"\"{EscapeCsv(lossItem.StoreName)}\"," +
                              $"{lossItem.Quantity}," +
                              $"{((decimal)lossItem.Value):F2}," +
                              $"\"{EscapeCsv(lossItem.Reason ?? "")}\"");
                serial++;
            }

            return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> GenerateLossReportPdfAsync(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null)
        {
            var losses = await GetLossReportAsync(fromDate, toDate, lossType, storeId);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Organization Header
            var orgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18, new iTextSharp.text.BaseColor(64, 64, 64));
            var orgName = new iTextSharp.text.Paragraph("ANSAR & VDP", orgFont);
            orgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(orgName);

            var subOrgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 12, new iTextSharp.text.BaseColor(64, 64, 64));
            var subOrgName = new iTextSharp.text.Paragraph("Inventory Management System", subOrgFont);
            subOrgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            subOrgName.SpacingAfter = 5f;
            document.Add(subOrgName);

            // Report Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16, new iTextSharp.text.BaseColor(220, 53, 69));
            var title = new iTextSharp.text.Paragraph("LOSS REPORT", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            title.SpacingBefore = 10f;
            document.Add(title);

            // Filter Info
            string filterInfo = "All Losses";
            if (fromDate.HasValue && toDate.HasValue)
            {
                filterInfo = $"Period: {fromDate.Value:dd MMM yyyy} to {toDate.Value:dd MMM yyyy}";
            }
            else if (fromDate.HasValue)
            {
                filterInfo = $"From: {fromDate.Value:dd MMM yyyy}";
            }
            else if (toDate.HasValue)
            {
                filterInfo = $"Until: {toDate.Value:dd MMM yyyy}";
            }

            if (!string.IsNullOrEmpty(lossType))
            {
                filterInfo += $" | Loss Type: {lossType}";
            }

            if (storeId.HasValue)
            {
                var firstLoss = losses.FirstOrDefault();
                if (firstLoss != null)
                {
                    var lossItem = (dynamic)firstLoss;
                    filterInfo += $" | Store: {lossItem.StoreName}";
                }
            }

            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);
            var filterPara = new iTextSharp.text.Paragraph(filterInfo, infoFont);
            filterPara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(filterPara);

            var reportDate = new iTextSharp.text.Paragraph($"Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}", infoFont);
            reportDate.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            reportDate.SpacingAfter = 15f;
            document.Add(reportDate);

            // Summary Section
            var totalCount = losses.Count();
            var totalValue = losses.Sum(l => (decimal)((dynamic)l).Value);

            var summaryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var summaryText = $"Total Loss Records: {totalCount} | Total Loss Value: ৳{totalValue:N2}";
            var summaryPara = new iTextSharp.text.Paragraph(summaryText, summaryFont);
            summaryPara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            summaryPara.SpacingAfter = 10f;
            document.Add(summaryPara);

            // Table with 9 columns
            var table = new iTextSharp.text.pdf.PdfPTable(9);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 4f, 10f, 9f, 8f, 15f, 12f, 7f, 9f, 15f });
            table.SpacingBefore = 10f;

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 7, new iTextSharp.text.BaseColor(255, 255, 255));
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 6);

            string[] headers = { "#Ser", "Loss No", "Date", "Type", "Item", "Store", "Qty", "Value", "Reason" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(220, 53, 69); // Danger red
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                cell.BorderWidth = 1f;
                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
                table.AddCell(cell);
            }

            // Data rows
            int serialNo = 1;
            bool isAlternateRow = false;
            var alternateBgColor = new iTextSharp.text.BaseColor(255, 245, 245);
            var whiteBgColor = new iTextSharp.text.BaseColor(255, 255, 255);

            foreach (var loss in losses)
            {
                var lossItem = (dynamic)loss;
                var rowBgColor = isAlternateRow ? alternateBgColor : whiteBgColor;

                // Serial Number
                var serCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                serCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                serCell.Padding = 3;
                serCell.BackgroundColor = rowBgColor;
                serCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(serCell);

                // Loss Number
                var lossNoCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(lossItem.LossNo ?? "", cellFont));
                lossNoCell.Padding = 3;
                lossNoCell.BackgroundColor = rowBgColor;
                lossNoCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(lossNoCell);

                // Date
                var dateCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(((DateTime)lossItem.LossDate).ToString("dd-MMM-yyyy"), cellFont));
                dateCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                dateCell.Padding = 3;
                dateCell.BackgroundColor = rowBgColor;
                dateCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(dateCell);

                // Loss Type
                var typeCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(lossItem.LossType ?? "", cellFont));
                typeCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                typeCell.Padding = 3;
                typeCell.BackgroundColor = rowBgColor;
                typeCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(typeCell);

                // Item Name
                var itemCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(lossItem.ItemName ?? "", cellFont));
                itemCell.Padding = 3;
                itemCell.BackgroundColor = rowBgColor;
                itemCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(itemCell);

                // Store
                var storeCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(lossItem.StoreName ?? "", cellFont));
                storeCell.Padding = 3;
                storeCell.BackgroundColor = rowBgColor;
                storeCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(storeCell);

                // Quantity
                var qtyCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(lossItem.Quantity.ToString(), cellFont));
                qtyCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                qtyCell.Padding = 3;
                qtyCell.BackgroundColor = rowBgColor;
                qtyCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(qtyCell);

                // Value
                var valueCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"৳{((decimal)lossItem.Value):N2}", cellFont));
                valueCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                valueCell.Padding = 3;
                valueCell.BackgroundColor = rowBgColor;
                valueCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(valueCell);

                // Reason
                var reasonCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(lossItem.Reason ?? "", cellFont));
                reasonCell.Padding = 3;
                reasonCell.BackgroundColor = rowBgColor;
                reasonCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(reasonCell);

                serialNo++;
                isAlternateRow = !isAlternateRow;
            }

            document.Add(table);

            // Footer
            var footerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8, new iTextSharp.text.BaseColor(128, 128, 128));
            var footer = new iTextSharp.text.Paragraph("\n\nThis is a system-generated report from ANSAR & VDP Inventory Management System.", footerFont);
            footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(footer);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateMovementHistoryExcelAsync(DateTime? fromDate, DateTime? toDate, int? itemId = null, string movementType = null, int? storeId = null)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Movement History");

                // Headers
                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "Movement Date";
                worksheet.Cells[1, 3].Value = "Movement Type";
                worksheet.Cells[1, 4].Value = "Reference No";
                worksheet.Cells[1, 5].Value = "Item Name";
                worksheet.Cells[1, 6].Value = "Store ID";
                worksheet.Cells[1, 7].Value = "Quantity";
                worksheet.Cells[1, 8].Value = "Unit Price";
                worksheet.Cells[1, 9].Value = "Total Value";
                worksheet.Cells[1, 10].Value = "Remarks";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 10])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                var movements = await GetMovementHistoryAsync(fromDate, toDate, itemId, movementType, storeId);
                int row = 2;
                int serialNo = 1;

                foreach (var movement in movements)
                {
                    var movementItem = (dynamic)movement;
                    worksheet.Cells[row, 1].Value = serialNo;
                    worksheet.Cells[row, 2].Value = ((DateTime)movementItem.MovementDate).ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 3].Value = movementItem.MovementType;
                    worksheet.Cells[row, 4].Value = movementItem.ReferenceNo;
                    worksheet.Cells[row, 5].Value = movementItem.ItemName;
                    worksheet.Cells[row, 6].Value = movementItem.StoreId;
                    worksheet.Cells[row, 7].Value = movementItem.Quantity;
                    worksheet.Cells[row, 8].Value = movementItem.UnitPrice;
                    worksheet.Cells[row, 9].Value = movementItem.TotalValue;
                    worksheet.Cells[row, 10].Value = movementItem.Remarks;

                    row++;
                    serialNo++;
                }

                worksheet.Column(6).Style.Numberformat.Format = "#,##0.00";
                worksheet.Column(7).Style.Numberformat.Format = "#,##0.00";
                worksheet.Column(8).Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        // Excel generation methods with correct signatures
        public async Task<byte[]> GenerateIssueReportExcelAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Issue Report");

                // Title
                worksheet.Cells[1, 1].Value = "ANSAR & VDP - Issue Report";
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Date Range
                worksheet.Cells[2, 1].Value = $"Period: {fromDate?.ToString("dd MMM yyyy") ?? "All"} to {toDate?.ToString("dd MMM yyyy") ?? "Present"}";
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Style.Font.Italic = true;

                // Generated Date
                worksheet.Cells[3, 1].Value = $"Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}";
                worksheet.Cells[3, 1, 3, 8].Merge = true;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, 1].Style.Font.Size = 9;

                // Headers (Row 5)
                int headerRow = 5;
                worksheet.Cells[headerRow, 1].Value = "#Ser";
                worksheet.Cells[headerRow, 2].Value = "Issue No";
                worksheet.Cells[headerRow, 3].Value = "Issue Date";
                worksheet.Cells[headerRow, 4].Value = "Issued To";
                worksheet.Cells[headerRow, 5].Value = "Type";
                worksheet.Cells[headerRow, 6].Value = "Purpose";
                worksheet.Cells[headerRow, 7].Value = "Total Items";
                worksheet.Cells[headerRow, 8].Value = "Total Quantity";

                // Style headers
                using (var range = worksheet.Cells[headerRow, 1, headerRow, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(41, 128, 185)); // Blue
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }

                var issues = await GetIssueReportAsync(fromDate, toDate, storeId);
                int row = headerRow + 1;
                int serialNo = 1;

                foreach (var issue in issues)
                {
                    // Issued To with fallback logic
                    string issuedToText = issue.IssuedTo ?? issue.IssuedToName ?? issue.IssuedToIndividualName ??
                                         issue.IssuedToBattalionName ?? issue.IssuedToRangeName ??
                                         issue.IssuedToZilaName ?? issue.IssuedToUpazilaName ??
                                         issue.IssuedToUnionName ?? "N/A";

                    worksheet.Cells[row, 1].Value = serialNo;
                    worksheet.Cells[row, 2].Value = issue.IssueNo;
                    worksheet.Cells[row, 3].Value = issue.IssueDate.ToString("dd MMM yyyy");
                    worksheet.Cells[row, 4].Value = issuedToText;
                    worksheet.Cells[row, 5].Value = issue.IssuedToType;
                    worksheet.Cells[row, 6].Value = issue.Purpose;
                    worksheet.Cells[row, 7].Value = issue.Items?.Count() ?? 0;
                    worksheet.Cells[row, 8].Value = issue.Items?.Sum(i => i.Quantity) ?? 0;

                    // Alternate row coloring
                    if (serialNo % 2 == 0)
                    {
                        using (var range = worksheet.Cells[row, 1, row, 8])
                        {
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));
                        }
                    }

                    // Add borders
                    using (var range = worksheet.Cells[row, 1, row, 8])
                    {
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    // Center align specific columns
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0";

                    serialNo++;
                    row++;
                }

                // Summary Row
                int summaryRow = row + 1;
                worksheet.Cells[summaryRow, 1].Value = "SUMMARY";
                worksheet.Cells[summaryRow, 1, summaryRow, 5].Merge = true;
                worksheet.Cells[summaryRow, 6].Value = $"Total Issues: {issues.Count()}";
                worksheet.Cells[summaryRow, 7].Value = $"Unique Items: {issues.Sum(i => i.Items?.Count() ?? 0)}";
                worksheet.Cells[summaryRow, 8].Value = issues.Sum(i => i.Items?.Sum(item => item.Quantity) ?? 0);
                worksheet.Cells[summaryRow, 8].Style.Numberformat.Format = "#,##0";

                using (var range = worksheet.Cells[summaryRow, 1, summaryRow, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(52, 152, 219)); // Blue
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Column widths
                worksheet.Column(1).Width = 8;  // #Ser
                worksheet.Column(2).Width = 15; // Issue No
                worksheet.Column(3).Width = 14; // Issue Date
                worksheet.Column(4).Width = 25; // Issued To
                worksheet.Column(5).Width = 15; // Type
                worksheet.Column(6).Width = 30; // Purpose
                worksheet.Column(7).Width = 12; // Total Items
                worksheet.Column(8).Width = 15; // Total Quantity

                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> GeneratePurchaseReportExcelAsync(DateTime? fromDate, DateTime? toDate, int? vendorId = null)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Purchase Report");

                // Headers
                worksheet.Cells[1, 1].Value = "#";
                worksheet.Cells[1, 2].Value = "PO Number";
                worksheet.Cells[1, 3].Value = "Purchase Date";
                worksheet.Cells[1, 4].Value = "Vendor";
                worksheet.Cells[1, 5].Value = "Total Amount";
                worksheet.Cells[1, 6].Value = "Discount";
                worksheet.Cells[1, 7].Value = "Net Amount";
                worksheet.Cells[1, 8].Value = "Purchase Type";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                var purchases = await GetPurchaseReportAsync(fromDate, toDate, vendorId);
                int row = 2;
                int serialNo = 1;

                foreach (var purchase in purchases)
                {
                    worksheet.Cells[row, 1].Value = serialNo;
                    worksheet.Cells[row, 2].Value = purchase.PurchaseOrderNo;
                    worksheet.Cells[row, 3].Value = purchase.PurchaseDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = purchase.VendorName ?? "Marketplace";
                    worksheet.Cells[row, 5].Value = purchase.TotalAmount;
                    worksheet.Cells[row, 6].Value = purchase.Discount;
                    worksheet.Cells[row, 7].Value = purchase.TotalAmount - purchase.Discount;
                    worksheet.Cells[row, 8].Value = purchase.PurchaseType;

                    row++;
                    serialNo++;
                }

                // Format currency columns
                worksheet.Column(5).Style.Numberformat.Format = "#,##0.00";
                worksheet.Column(6).Style.Numberformat.Format = "#,##0.00";
                worksheet.Column(7).Style.Numberformat.Format = "#,##0.00";

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> GeneratePurchaseReportCsvAsync(DateTime? fromDate, DateTime? toDate, int? vendorId = null)
        {
            var purchases = await GetPurchaseReportAsync(fromDate, toDate, vendorId);

            var csv = new System.Text.StringBuilder();

            // Add UTF-8 BOM for Excel compatibility
            csv.Append("\uFEFF");

            // Headers
            csv.AppendLine("#,PO Number,Purchase Date,Vendor,Total Amount,Discount,Net Amount,Purchase Type,Status");

            // Data rows
            int serialNo = 1;
            foreach (var purchase in purchases)
            {
                var netAmount = purchase.TotalAmount - purchase.Discount;
                csv.AppendLine($"{serialNo}," +
                              $"\"{EscapeCsv(purchase.PurchaseOrderNo)}\"," +
                              $"\"{purchase.PurchaseDate:yyyy-MM-dd}\"," +
                              $"\"{EscapeCsv(purchase.VendorName ?? "Marketplace")}\"," +
                              $"{purchase.TotalAmount:F2}," +
                              $"{purchase.Discount:F2}," +
                              $"{netAmount:F2}," +
                              $"\"{EscapeCsv(purchase.PurchaseType)}\"," +
                              $"\"{EscapeCsv(purchase.Status)}\"");
                serialNo++;
            }

            return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Escape double quotes by doubling them
            return value.Replace("\"", "\"\"");
        }

        public async Task<byte[]> GenerateStockReportExcelAsync(int? storeId = null, int? categoryId = null)
        {
            var stockReport = await GetStockReportAsync(storeId, categoryId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Stock Report");

            // Title
            worksheet.Cell(1, 1).Value = "ANSAR & VDP - Stock Report";
            worksheet.Range(1, 1, 1, 11).Merge();
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Filter Info
            string filterInfo = "All Items";
            if (storeId.HasValue)
            {
                var store = stockReport.StockItems.FirstOrDefault()?.StoreName;
                if (!string.IsNullOrEmpty(store))
                    filterInfo = $"Store: {store}";
            }
            if (categoryId.HasValue)
            {
                var category = stockReport.StockItems.FirstOrDefault()?.CategoryName;
                if (!string.IsNullOrEmpty(category))
                    filterInfo += categoryId.HasValue && storeId.HasValue ? $" | Category: {category}" : $"Category: {category}";
            }

            worksheet.Cell(2, 1).Value = filterInfo;
            worksheet.Range(2, 1, 2, 11).Merge();
            worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(2, 1).Style.Font.Italic = true;

            // Generated Date
            worksheet.Cell(3, 1).Value = $"Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}";
            worksheet.Range(3, 1, 3, 11).Merge();
            worksheet.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3, 1).Style.Font.FontSize = 9;

            // Headers (Row 5)
            int headerRow = 5;
            worksheet.Cell(headerRow, 1).Value = "#Ser";
            worksheet.Cell(headerRow, 2).Value = "Item Code";
            worksheet.Cell(headerRow, 3).Value = "Item Name";
            worksheet.Cell(headerRow, 4).Value = "Category";
            worksheet.Cell(headerRow, 5).Value = "Store";
            worksheet.Cell(headerRow, 6).Value = "Current Stock";
            worksheet.Cell(headerRow, 7).Value = "Min Stock";
            worksheet.Cell(headerRow, 8).Value = "Max Stock";
            worksheet.Cell(headerRow, 9).Value = "Unit Price";
            worksheet.Cell(headerRow, 10).Value = "Total Value";
            worksheet.Cell(headerRow, 11).Value = "Status";

            // Style headers
            var headerRange = worksheet.Range(headerRow, 1, headerRow, 11);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(41, 128, 185); // Blue
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Data
            var row = headerRow + 1;
            int serialNo = 1;
            foreach (var item in stockReport.StockItems)
            {
                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = item.ItemCode;
                worksheet.Cell(row, 3).Value = item.ItemName;
                worksheet.Cell(row, 4).Value = item.CategoryName;
                worksheet.Cell(row, 5).Value = item.StoreName;
                worksheet.Cell(row, 6).Value = item.CurrentStock;
                worksheet.Cell(row, 7).Value = item.MinimumStock;
                worksheet.Cell(row, 8).Value = item.MaximumStock;
                worksheet.Cell(row, 9).Value = item.UnitPrice;
                worksheet.Cell(row, 10).Value = item.TotalValue;
                worksheet.Cell(row, 11).Value = item.StockStatus;

                // Alternating row colors (except for status-based coloring)
                var rowRange = worksheet.Range(row, 1, row, 11);
                if (item.StockStatus == "Out of Stock")
                {
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 220, 220); // Light red
                    rowRange.Style.Font.FontColor = XLColor.FromArgb(139, 0, 0); // Dark red
                }
                else if (item.StockStatus == "Low Stock")
                {
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 224); // Light yellow
                    rowRange.Style.Font.FontColor = XLColor.FromArgb(184, 134, 11); // Dark golden
                }
                else if (serialNo % 2 == 0)
                {
                    rowRange.Style.Fill.BackgroundColor = XLColor.FromArgb(245, 245, 245); // Light gray
                }

                // Add borders
                rowRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rowRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                rowRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                // Center align specific columns
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Cell(row, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                serialNo++;
                row++;
            }

            // Number formatting
            worksheet.Range(headerRow + 1, 9, row - 1, 10).Style.NumberFormat.Format = "#,##0.00";

            // Summary Row
            int summaryRow = row + 1;
            worksheet.Cell(summaryRow, 1).Value = "SUMMARY";
            worksheet.Range(summaryRow, 1, summaryRow, 5).Merge();
            worksheet.Cell(summaryRow, 6).Value = $"Total Items: {stockReport.TotalItems}";
            worksheet.Range(summaryRow, 6, summaryRow, 7).Merge();
            worksheet.Cell(summaryRow, 8).Value = $"Low Stock: {stockReport.LowStockItems}";
            worksheet.Range(summaryRow, 8, summaryRow, 9).Merge();
            worksheet.Cell(summaryRow, 10).Value = stockReport.TotalValue;
            worksheet.Cell(summaryRow, 10).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(summaryRow, 11).Value = $"Out: {stockReport.OutOfStockItems}";

            var summaryRange = worksheet.Range(summaryRow, 1, summaryRow, 11);
            summaryRange.Style.Font.Bold = true;
            summaryRange.Style.Fill.BackgroundColor = XLColor.FromArgb(52, 152, 219); // Blue
            summaryRange.Style.Font.FontColor = XLColor.White;
            summaryRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Column widths
            worksheet.Column(1).Width = 8;   // #Ser
            worksheet.Column(2).Width = 12;  // Item Code
            worksheet.Column(3).Width = 25;  // Item Name
            worksheet.Column(4).Width = 15;  // Category
            worksheet.Column(5).Width = 18;  // Store
            worksheet.Column(6).Width = 12;  // Current Stock
            worksheet.Column(7).Width = 10;  // Min Stock
            worksheet.Column(8).Width = 10;  // Max Stock
            worksheet.Column(9).Width = 12;  // Unit Price
            worksheet.Column(10).Width = 15; // Total Value
            worksheet.Column(11).Width = 12; // Status

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<BatchReportDto> GenerateBatchReportAsync(string reportType, Dictionary<string, object> parameters)
        {
            var batchReport = new BatchReportDto
            {
                ReportType = reportType,
                GeneratedDate = DateTime.Now,
                GeneratedBy = parameters.ContainsKey("UserId") ? parameters["UserId"].ToString() : "System"
            };

            try
            {
                byte[] reportData = reportType switch
                {
                    "Stock" => await GenerateStockReportExcelAsync(
                        parameters.ContainsKey("StoreId") ? (int?)parameters["StoreId"] : null,
                        parameters.ContainsKey("CategoryId") ? (int?)parameters["CategoryId"] : null),

                    "Purchase" => await GeneratePurchaseReportExcelAsync(
                        parameters.ContainsKey("FromDate") ? (DateTime?)parameters["FromDate"] : null,
                        parameters.ContainsKey("ToDate") ? (DateTime?)parameters["ToDate"] : null,
                        parameters.ContainsKey("VendorId") ? (int?)parameters["VendorId"] : null),

                    "Issue" => await GenerateIssueReportExcelAsync(
                        parameters.ContainsKey("FromDate") ? (DateTime?)parameters["FromDate"] : null,
                        parameters.ContainsKey("ToDate") ? (DateTime?)parameters["ToDate"] : null,
                        parameters.ContainsKey("StoreId") ? (int?)parameters["StoreId"] : null),

                    _ => throw new NotSupportedException($"Report type {reportType} not supported")
                };

                batchReport.ReportData = reportData;
                batchReport.Success = true;
                batchReport.Message = "Report generated successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating batch report: {reportType}");
                batchReport.Success = false;
                batchReport.Message = ex.Message;
            }

            return batchReport;
        }

        public async Task<object> GetMonthlyTrendAsync(string reportType, int months)
        {
            var startDate = DateTime.Now.AddMonths(-months);

            return reportType switch
            {
                "Purchase" => await _unitOfWork.Purchases.Query()
                    .Where(p => p.IsActive && p.CreatedAt >= startDate)
                    .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                    .Select(g => new
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Count = g.Count(),
                        Value = g.Sum(p => p.TotalAmount)
                    })
                    .OrderBy(x => x.Period)
                    .ToListAsync(),

                "Issue" => await _unitOfWork.Issues.Query()
                    .Where(i => i.IsActive && i.CreatedAt >= startDate)
                    .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                    .Select(g => new
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Period)
                    .ToListAsync(),

                _ => new List<object>()
            };
        }


        public async Task<byte[]> GenerateAuditReportAsync(AuditReportRequestDto request)
        {
            // Implementation for audit report generation
            return new byte[0]; // Placeholder
        }

        public async Task<byte[]> GenerateExpiryReportAsync(int storeId, DateTime? asOfDate)
        {
            // Implementation for expiry report
            return new byte[0]; // Placeholder
        }

        public async Task<byte[]> GenerateIssueReportAsync(DateTime fromDate, DateTime toDate)
        {
            // Implementation for issue report
            return new byte[0]; // Placeholder
        }

        public async Task<byte[]> GenerateMovementReportAsync(MovementReportRequestDto request)
        {
            // Implementation for movement report
            return new byte[0]; // Placeholder
        }

        public async Task<byte[]> GeneratePurchaseReportAsync(DateTime fromDate, DateTime toDate)
        {
            // Implementation for purchase report
            return new byte[0]; // Placeholder
        }

        public async Task<byte[]> GenerateStockReportAsync(StockReportRequestDto request)
        {
            // Implementation for stock report
            return new byte[0]; // Placeholder
        }

        public async Task<byte[]> GenerateVarianceReportAsync(int inventoryId)
        {
            // Implementation for variance report
            return new byte[0]; // Placeholder
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int? storeId)
        {
            // Implementation for dashboard stats
            return new DashboardStatsDto();
        }

        public async Task<ConsumptionAnalysisReportDto> GetConsumptionAnalysisReportAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null)
        {
            try
            {
                _logger.LogInformation("Generating consumption analysis report from {FromDate} to {ToDate}", fromDate, toDate);

                var report = new ConsumptionAnalysisReportDto
                {
                    FromDate = fromDate ?? DateTime.Now.AddMonths(-12),
                    ToDate = toDate ?? DateTime.Now
                };

                // Get all issues within date range
                var issues = await _unitOfWork.Issues.GetAllAsync();
                issues = issues.Where(i => i.IsActive &&
                                          i.IssueDate >= report.FromDate &&
                                          i.IssueDate <= report.ToDate);

                var consumptionItems = new Dictionary<int, ConsumptionItemDto>();
                var categoryConsumption = new Dictionary<string, CategoryConsumptionDto>();
                var storeConsumption = new Dictionary<int, StoreConsumptionDto>();
                var monthlyConsumption = new Dictionary<string, MonthlyConsumptionDto>();

                foreach (var issue in issues)
                {
                    var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);

                    foreach (var issueItem in issueItems)
                    {
                        // Apply filters
                        if (storeId.HasValue && issueItem.StoreId != storeId.Value)
                            continue;

                        var item = await _unitOfWork.Items.GetByIdAsync(issueItem.ItemId);
                        if (item == null) continue;

                        var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                        var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;

                        if (categoryId.HasValue && category?.Id != categoryId.Value)
                            continue;

                        // Get unit price from last purchase
                        var lastPurchase = await _unitOfWork.PurchaseItems.FirstOrDefaultAsync(pi => pi.ItemId == item.Id);
                        var unitPrice = lastPurchase?.UnitPrice ?? 0;
                        var totalValue = issueItem.Quantity * unitPrice;

                        // Aggregate by item
                        if (!consumptionItems.ContainsKey(item.Id))
                        {
                            consumptionItems[item.Id] = new ConsumptionItemDto
                            {
                                ItemId = item.Id,
                                ItemCode = item.ItemCode,
                                ItemName = item.Name,
                                CategoryName = category?.Name ?? "N/A",
                                TotalQuantity = 0,
                                Unit = item.Unit,
                                AverageConsumption = 0
                            };
                        }
                        consumptionItems[item.Id].TotalQuantity += issueItem.Quantity;

                        // Aggregate by category
                        var categoryName = category?.Name ?? "N/A";
                        if (!categoryConsumption.ContainsKey(categoryName))
                        {
                            categoryConsumption[categoryName] = new CategoryConsumptionDto
                            {
                                CategoryName = categoryName,
                                TotalQuantity = 0,
                                TotalValue = 0,
                                ItemCount = 0
                            };
                        }
                        categoryConsumption[categoryName].TotalQuantity += issueItem.Quantity;
                        categoryConsumption[categoryName].TotalValue += totalValue;

                        // Aggregate by store
                        if (issueItem.StoreId.HasValue)
                        {
                            var storeKey = issueItem.StoreId.Value;
                            if (!storeConsumption.ContainsKey(storeKey))
                            {
                                var store = await _unitOfWork.Stores.GetByIdAsync(storeKey);
                                storeConsumption[storeKey] = new StoreConsumptionDto
                                {
                                    StoreId = storeKey,
                                    StoreName = store?.Name ?? "N/A",
                                    TotalQuantity = 0,
                                    TotalValue = 0,
                                    IssueCount = 0
                                };
                            }
                            storeConsumption[storeKey].TotalQuantity += issueItem.Quantity;
                            storeConsumption[storeKey].TotalValue += totalValue;
                        }

                        // Aggregate by month
                        var monthKey = $"{issue.IssueDate.Year}-{issue.IssueDate.Month:D2}";
                        if (!monthlyConsumption.ContainsKey(monthKey))
                        {
                            monthlyConsumption[monthKey] = new MonthlyConsumptionDto
                            {
                                Month = issue.IssueDate.ToString("MMMM"),
                                Year = issue.IssueDate.Year,
                                TotalQuantity = 0,
                                TotalValue = 0,
                                IssueCount = 0
                            };
                        }
                        monthlyConsumption[monthKey].TotalQuantity += issueItem.Quantity;
                        monthlyConsumption[monthKey].TotalValue += totalValue;

                        report.TotalConsumptionValue += totalValue;
                    }

                    // Count issues per store
                    var firstIssueItem = issueItems.FirstOrDefault();
                    if (firstIssueItem != null && firstIssueItem.StoreId.HasValue && storeConsumption.ContainsKey(firstIssueItem.StoreId.Value))
                    {
                        storeConsumption[firstIssueItem.StoreId.Value].IssueCount++;
                    }

                    // Count issues per month
                    var monthKeyForCount = $"{issue.IssueDate.Year}-{issue.IssueDate.Month:D2}";
                    if (monthlyConsumption.ContainsKey(monthKeyForCount))
                    {
                        monthlyConsumption[monthKeyForCount].IssueCount++;
                    }
                }

                // Calculate average consumption per item
                var totalMonths = (report.ToDate - report.FromDate).Days / 30.0;
                if (totalMonths < 1) totalMonths = 1;

                foreach (var item in consumptionItems.Values)
                {
                    item.AverageConsumption = item.TotalQuantity / (decimal)totalMonths;
                }

                // Update category item counts
                foreach (var categoryName in categoryConsumption.Keys)
                {
                    categoryConsumption[categoryName].ItemCount = consumptionItems.Values
                        .Count(i => i.CategoryName == categoryName);
                }

                report.Items = consumptionItems.Values.OrderByDescending(i => i.TotalQuantity).ToList();
                report.CategoryBreakdown = categoryConsumption.Values.OrderByDescending(c => c.TotalValue).ToList();
                report.StoreBreakdown = storeConsumption.Values.OrderByDescending(s => s.TotalValue).ToList();
                report.MonthlyTrend = monthlyConsumption.Values.OrderBy(m => $"{m.Year}-{m.Month}").ToList();
                report.TotalItemsConsumed = consumptionItems.Count;

                _logger.LogInformation("Consumption analysis report generated successfully with {ItemCount} items", report.TotalItemsConsumed);
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating consumption analysis report");
                throw;
            }
        }

        public async Task<ExpiryReportDto> GetExpiryReportAsync(int? storeId = null, int? daysAhead = 90)
        {
            try
            {
                _logger.LogInformation("Generating expiry report for store {StoreId}, {DaysAhead} days ahead", storeId, daysAhead);

                var report = new ExpiryReportDto
                {
                    AsOfDate = DateTime.Now,
                    StoreId = storeId
                };

                if (storeId.HasValue)
                {
                    var store = await _unitOfWork.Stores.GetByIdAsync(storeId.Value);
                    report.StoreName = store?.Name;
                }

                var today = DateTime.Now.Date;
                var futureDate = today.AddDays(daysAhead ?? 90);

                // Get all batch trackings with expiry dates
                var batchTrackings = await _unitOfWork.BatchTrackings.GetAllAsync();
                batchTrackings = batchTrackings.Where(bt =>
                    bt.IsActive &&
                    bt.ExpiryDate.HasValue &&
                    bt.Quantity > 0);

                if (storeId.HasValue)
                {
                    batchTrackings = batchTrackings.Where(bt => bt.StoreId == storeId.Value);
                }

                foreach (var batch in batchTrackings)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(batch.ItemId);
                    if (item == null || !item.IsActive) continue;

                    var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                    var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;
                    var store = await _unitOfWork.Stores.GetByIdAsync(batch.StoreId);

                    var daysToExpiry = (batch.ExpiryDate.Value.Date - today).Days;
                    var itemValue = batch.Quantity * batch.CostPrice;

                    var expiryItem = new ExpiryItemDto
                    {
                        ItemId = item.Id,
                        ItemCode = item.ItemCode,
                        ItemName = item.Name,
                        BatchNumber = batch.BatchNumber,
                        ExpiryDate = batch.ExpiryDate.Value,
                        DaysToExpiry = daysToExpiry,
                        Quantity = batch.Quantity,
                        Unit = item.Unit,
                        StoreName = store?.Name ?? "N/A",
                        CategoryName = category?.Name ?? "N/A",
                        Value = itemValue
                    };

                    if (daysToExpiry < 0)
                    {
                        // Expired
                        report.ExpiredItems.Add(expiryItem);
                        report.TotalExpiredItems++;
                        report.TotalExpiredValue += itemValue;
                    }
                    else if (daysToExpiry <= daysAhead)
                    {
                        // Expiring soon
                        report.ExpiringItems.Add(expiryItem);
                        report.TotalExpiringItems++;
                        report.TotalExpiringValue += itemValue;
                    }
                }

                // Also check ExpiryTracking table
                var expiryTrackings = await _unitOfWork.ExpiryTrackings.GetAllAsync();
                expiryTrackings = expiryTrackings.Where(et =>
                    et.IsActive &&
                    et.Quantity > 0);

                if (storeId.HasValue)
                {
                    expiryTrackings = expiryTrackings.Where(et => et.StoreId == storeId.Value);
                }

                foreach (var expiry in expiryTrackings)
                {
                    // Skip if already added from batch tracking
                    if (report.ExpiredItems.Any(ei => ei.BatchNumber == expiry.BatchNumber && ei.ItemId == expiry.ItemId) ||
                        report.ExpiringItems.Any(ei => ei.BatchNumber == expiry.BatchNumber && ei.ItemId == expiry.ItemId))
                        continue;

                    var item = await _unitOfWork.Items.GetByIdAsync(expiry.ItemId);
                    if (item == null || !item.IsActive) continue;

                    var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                    var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;
                    var store = expiry.StoreId.HasValue ? await _unitOfWork.Stores.GetByIdAsync(expiry.StoreId.Value) : null;

                    var daysToExpiry = (expiry.ExpiryDate.Date - today).Days;

                    // Get cost from last purchase or batch
                    var lastPurchase = await _unitOfWork.PurchaseItems.FirstOrDefaultAsync(pi => pi.ItemId == item.Id);
                    var unitCost = lastPurchase?.UnitPrice ?? 0;
                    var itemValue = expiry.Quantity * unitCost;

                    var expiryItem = new ExpiryItemDto
                    {
                        ItemId = item.Id,
                        ItemCode = item.ItemCode,
                        ItemName = item.Name,
                        BatchNumber = expiry.BatchNumber,
                        ExpiryDate = expiry.ExpiryDate,
                        DaysToExpiry = daysToExpiry,
                        Quantity = expiry.Quantity,
                        Unit = item.Unit,
                        StoreName = store?.Name ?? "N/A",
                        CategoryName = category?.Name ?? "N/A",
                        Value = itemValue
                    };

                    if (daysToExpiry < 0)
                    {
                        report.ExpiredItems.Add(expiryItem);
                        report.TotalExpiredItems++;
                        report.TotalExpiredValue += itemValue;
                    }
                    else if (daysToExpiry <= daysAhead)
                    {
                        report.ExpiringItems.Add(expiryItem);
                        report.TotalExpiringItems++;
                        report.TotalExpiringValue += itemValue;
                    }
                }

                // Sort by days to expiry
                report.ExpiredItems = report.ExpiredItems.OrderBy(ei => ei.DaysToExpiry).ToList();
                report.ExpiringItems = report.ExpiringItems.OrderBy(ei => ei.DaysToExpiry).ToList();

                _logger.LogInformation("Expiry report generated: {Expired} expired, {Expiring} expiring soon",
                    report.TotalExpiredItems, report.TotalExpiringItems);
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating expiry report");
                throw;
            }
        }

        public async Task<AuditTrailReportDto> GetAuditTrailReportAsync(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null)
        {
            try
            {
                _logger.LogInformation("Generating audit trail report from {FromDate} to {ToDate}", fromDate, toDate);

                var report = new AuditTrailReportDto
                {
                    FromDate = fromDate ?? DateTime.Now.AddMonths(-1),
                    ToDate = toDate ?? DateTime.Now
                };

                var auditItems = new List<AuditTrailItemDto>();
                var transactionTypeCount = new Dictionary<string, int>();

                // Get stock movements
                if (string.IsNullOrEmpty(transactionType) || transactionType.Equals("StockMovement", StringComparison.OrdinalIgnoreCase))
                {
                    var stockMovements = await _unitOfWork.StockMovements.GetAllAsync();
                    stockMovements = stockMovements.Where(sm =>
                        sm.IsActive &&
                        sm.MovementDate >= report.FromDate &&
                        sm.MovementDate <= report.ToDate);

                    if (storeId.HasValue)
                    {
                        stockMovements = stockMovements.Where(sm => sm.StoreId == storeId.Value || sm.SourceStoreId == storeId.Value);
                    }

                    foreach (var movement in stockMovements)
                    {
                        var item = await _unitOfWork.Items.GetByIdAsync(movement.ItemId);
                        var store = movement.StoreId.HasValue ? await _unitOfWork.Stores.GetByIdAsync(movement.StoreId.Value) : null;

                        auditItems.Add(new AuditTrailItemDto
                        {
                            TransactionDate = movement.MovementDate,
                            TransactionType = movement.MovementType ?? "StockMovement",
                            ReferenceNo = movement.ReferenceNo ?? movement.Id.ToString(),
                            ItemName = item?.Name ?? "N/A",
                            StoreName = store?.Name ?? "N/A",
                            Quantity = movement.Quantity ?? 0,
                            PerformedBy = movement.CreatedBy ?? "System",
                            Action = movement.MovementType ?? "Movement",
                            OldValue = movement.OldBalance.ToString(),
                            NewValue = movement.NewBalance.ToString()
                        });

                        var txnType = movement.MovementType ?? "StockMovement";
                        transactionTypeCount[txnType] = transactionTypeCount.GetValueOrDefault(txnType, 0) + 1;
                    }
                }

                // Get activity logs
                if (string.IsNullOrEmpty(transactionType) || transactionType.Equals("Activity", StringComparison.OrdinalIgnoreCase))
                {
                    var activityLogs = await _unitOfWork.ActivityLogs.GetAllAsync();
                    activityLogs = activityLogs.Where(al =>
                        al.IsActive &&
                        al.ActionDate >= report.FromDate &&
                        al.ActionDate <= report.ToDate);

                    foreach (var activity in activityLogs)
                    {
                        // Try to extract store info from details if available
                        var storeName = "N/A";
                        if (activity.EntityId.HasValue)
                        {
                            // Attempt to get store based on entity type
                            if (activity.EntityType == "Store")
                            {
                                var store = await _unitOfWork.Stores.GetByIdAsync(activity.EntityId.Value);
                                storeName = store?.Name ?? "N/A";
                            }
                        }

                        // Apply store filter if specified
                        if (storeId.HasValue && storeName == "N/A")
                            continue;

                        auditItems.Add(new AuditTrailItemDto
                        {
                            TransactionDate = activity.ActionDate,
                            TransactionType = "Activity",
                            ReferenceNo = activity.EntityType ?? "N/A",
                            ItemName = activity.Description ?? "N/A",
                            StoreName = storeName,
                            Quantity = 0,
                            PerformedBy = activity.UserName ?? activity.UserId ?? "System",
                            Action = activity.Action ?? "N/A",
                            OldValue = activity.OldValue ?? "N/A",
                            NewValue = activity.NewValue ?? "N/A"
                        });

                        transactionTypeCount["Activity"] = transactionTypeCount.GetValueOrDefault("Activity", 0) + 1;
                    }
                }

                // Get purchases
                if (string.IsNullOrEmpty(transactionType) || transactionType.Equals("Purchase", StringComparison.OrdinalIgnoreCase))
                {
                    var purchases = await _unitOfWork.Purchases.GetAllAsync();
                    purchases = purchases.Where(p =>
                        p.IsActive &&
                        p.PurchaseDate >= report.FromDate &&
                        p.PurchaseDate <= report.ToDate);

                    foreach (var purchase in purchases)
                    {
                        var purchaseItems = await _unitOfWork.PurchaseItems.FindAsync(pi => pi.PurchaseId == purchase.Id);

                        foreach (var purchaseItem in purchaseItems)
                        {
                            if (storeId.HasValue && purchaseItem.StoreId != storeId.Value)
                                continue;

                            var item = await _unitOfWork.Items.GetByIdAsync(purchaseItem.ItemId);
                            var store = await _unitOfWork.Stores.GetByIdAsync(purchaseItem.StoreId);

                            auditItems.Add(new AuditTrailItemDto
                            {
                                TransactionDate = purchase.PurchaseDate,
                                TransactionType = "Purchase",
                                ReferenceNo = purchase.PurchaseOrderNo,
                                ItemName = item?.Name ?? "N/A",
                                StoreName = store?.Name ?? "N/A",
                                Quantity = purchaseItem.Quantity,
                                PerformedBy = purchase.CreatedBy ?? "System",
                                Action = "Purchase Received",
                                OldValue = "0",
                                NewValue = purchaseItem.Quantity.ToString()
                            });
                        }

                        transactionTypeCount["Purchase"] = transactionTypeCount.GetValueOrDefault("Purchase", 0) + 1;
                    }
                }

                // Get issues
                if (string.IsNullOrEmpty(transactionType) || transactionType.Equals("Issue", StringComparison.OrdinalIgnoreCase))
                {
                    var issues = await _unitOfWork.Issues.GetAllAsync();
                    issues = issues.Where(i =>
                        i.IsActive &&
                        i.IssueDate >= report.FromDate &&
                        i.IssueDate <= report.ToDate);

                    foreach (var issue in issues)
                    {
                        var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);

                        foreach (var issueItem in issueItems)
                        {
                            if (storeId.HasValue && issueItem.StoreId != storeId.Value)
                                continue;

                            var item = await _unitOfWork.Items.GetByIdAsync(issueItem.ItemId);
                            var store = await _unitOfWork.Stores.GetByIdAsync(issueItem.StoreId);

                            auditItems.Add(new AuditTrailItemDto
                            {
                                TransactionDate = issue.IssueDate,
                                TransactionType = "Issue",
                                ReferenceNo = issue.IssueNo,
                                ItemName = item?.Name ?? "N/A",
                                StoreName = store?.Name ?? "N/A",
                                Quantity = issueItem.Quantity,
                                PerformedBy = issue.CreatedBy ?? "System",
                                Action = "Item Issued",
                                OldValue = "N/A",
                                NewValue = $"Issued to {issue.IssuedTo}"
                            });
                        }

                        transactionTypeCount["Issue"] = transactionTypeCount.GetValueOrDefault("Issue", 0) + 1;
                    }
                }

                // Get transfers
                if (string.IsNullOrEmpty(transactionType) || transactionType.Equals("Transfer", StringComparison.OrdinalIgnoreCase))
                {
                    var transfers = await _unitOfWork.Transfers.GetAllAsync();
                    transfers = transfers.Where(t =>
                        t.IsActive &&
                        t.TransferDate >= report.FromDate &&
                        t.TransferDate <= report.ToDate);

                    if (storeId.HasValue)
                    {
                        transfers = transfers.Where(t => t.FromStoreId == storeId.Value || t.ToStoreId == storeId.Value);
                    }

                    foreach (var transfer in transfers)
                    {
                        var transferItems = await _unitOfWork.TransferItems.FindAsync(ti => ti.TransferId == transfer.Id);
                        var fromStore = await _unitOfWork.Stores.GetByIdAsync(transfer.FromStoreId);
                        var toStore = await _unitOfWork.Stores.GetByIdAsync(transfer.ToStoreId);

                        foreach (var transferItem in transferItems)
                        {
                            var item = await _unitOfWork.Items.GetByIdAsync(transferItem.ItemId);

                            auditItems.Add(new AuditTrailItemDto
                            {
                                TransactionDate = transfer.TransferDate,
                                TransactionType = "Transfer",
                                ReferenceNo = transfer.TransferNo,
                                ItemName = item?.Name ?? "N/A",
                                StoreName = fromStore?.Name ?? "N/A",
                                Quantity = transferItem.Quantity,
                                PerformedBy = transfer.CreatedBy ?? "System",
                                Action = "Transfer",
                                OldValue = fromStore?.Name ?? "N/A",
                                NewValue = toStore?.Name ?? "N/A"
                            });
                        }

                        transactionTypeCount["Transfer"] = transactionTypeCount.GetValueOrDefault("Transfer", 0) + 1;
                    }
                }

                report.AuditItems = auditItems.OrderByDescending(ai => ai.TransactionDate).ToList();
                report.TotalTransactions = auditItems.Count;
                report.TransactionTypeBreakdown = transactionTypeCount;

                _logger.LogInformation("Audit trail report generated with {TotalTransactions} transactions", report.TotalTransactions);
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating audit trail report");
                throw;
            }
        }

        public async Task<VarianceAnalysisDto> GetVarianceReportAsync(int physicalInventoryId)
        {
            try
            {
                _logger.LogInformation("Generating variance report for physical inventory {InventoryId}", physicalInventoryId);

                var physicalInventory = await _unitOfWork.PhysicalInventories.GetByIdAsync(physicalInventoryId);
                if (physicalInventory == null)
                {
                    throw new InvalidOperationException($"Physical inventory with ID {physicalInventoryId} not found");
                }

                var report = new VarianceAnalysisDto
                {
                    PhysicalInventoryId = physicalInventoryId,
                    ReferenceNumber = physicalInventory.CountNo,
                    CountDate = physicalInventory.CountDate,
                    InventoryId = physicalInventoryId,
                    TotalSystemValue = physicalInventory.TotalSystemValue ?? 0,
                    TotalPhysicalValue = physicalInventory.TotalPhysicalValue ?? 0,
                    TotalVarianceValue = physicalInventory.VarianceValue,
                    TotalItems = physicalInventory.TotalItemsCounted,
                    VarianceByCategory = new List<CategoryVarianceDto>(),
                    TopVarianceItems = new List<ItemVarianceDto>(),
                    RecommendedActions = new List<string>(),
                    ItemsWithPositiveVariance = 0,
                    ItemsWithNegativeVariance = 0,
                    TotalPositiveVariance = 0,
                    TotalNegativeVariance = 0,
                    TotalSystemQuantity = 0,
                    TotalPhysicalQuantity = 0,
                    TotalVariance = 0,
                    VarianceDetails = new List<VarianceDetailDto>(),
                    ItemsWithVariance = new List<VarianceItemDto>()
                };

                // Get physical inventory items
                var inventoryItems = await _unitOfWork.PhysicalInventoryItems.FindAsync(pii =>
                    pii.PhysicalInventoryId == physicalInventoryId && pii.IsActive);

                var categoryVariance = new Dictionary<string, CategoryVarianceDto>();

                foreach (var inventoryItem in inventoryItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(inventoryItem.ItemId);
                    if (item == null) continue;

                    var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                    var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;
                    var categoryName = category?.Name ?? "N/A";

                    var variance = inventoryItem.Variance;
                    var variancePercentage = inventoryItem.SystemQuantity > 0
                        ? (variance / inventoryItem.SystemQuantity) * 100
                        : 0;

                    // Add to variance details
                    var varianceDetail = new VarianceDetailDto
                    {
                        ItemId = item.Id,
                        ItemCode = item.ItemCode,
                        ItemName = item.Name,
                        CategoryName = categoryName,
                        SystemQuantity = inventoryItem.SystemQuantity,
                        PhysicalQuantity = inventoryItem.PhysicalQuantity,
                        Variance = variance,
                        VariancePercentage = variancePercentage,
                        VarianceValue = inventoryItem.VarianceValue,
                        Remarks = $"Location: {inventoryItem.Location ?? "N/A"}, Batch: {inventoryItem.BatchNumber ?? "N/A"}"
                    };

                    report.VarianceDetails.Add(varianceDetail);

                    // Track variance direction
                    if (variance > 0)
                    {
                        report.ItemsWithPositiveVariance++;
                        report.TotalPositiveVariance += variance;
                        report.OverageValue += inventoryItem.VarianceValue;
                    }
                    else if (variance < 0)
                    {
                        report.ItemsWithNegativeVariance++;
                        report.TotalNegativeVariance += Math.Abs(variance);
                        report.ShortageValue += Math.Abs(inventoryItem.VarianceValue);
                    }

                    // Add to items with variance list
                    if (variance != 0)
                    {
                        report.ItemsWithVariance.Add(new VarianceItemDto
                        {
                            ItemId = item.Id,
                            ItemName = item.Name,
                            SystemQuantity = inventoryItem.SystemQuantity,
                            PhysicalQuantity = inventoryItem.PhysicalQuantity,
                            Variance = variance,
                            VarianceValue = inventoryItem.VarianceValue
                        });
                    }

                    // Aggregate by category
                    if (!categoryVariance.ContainsKey(categoryName))
                    {
                        categoryVariance[categoryName] = new CategoryVarianceDto
                        {
                            CategoryName = categoryName,
                            SystemQuantity = 0,
                            PhysicalQuantity = 0,
                            Variance = 0,
                            VarianceValue = 0
                        };
                    }

                    categoryVariance[categoryName].SystemQuantity += inventoryItem.SystemQuantity;
                    categoryVariance[categoryName].PhysicalQuantity += inventoryItem.PhysicalQuantity;
                    categoryVariance[categoryName].Variance += variance;
                    categoryVariance[categoryName].VarianceValue += inventoryItem.VarianceValue;

                    // Update totals
                    report.TotalSystemQuantity += inventoryItem.SystemQuantity;
                    report.TotalPhysicalQuantity += inventoryItem.PhysicalQuantity;
                    report.TotalVariance += variance;
                }

                report.VarianceByCategory = categoryVariance.Values.OrderByDescending(cv => Math.Abs(cv.VarianceValue)).ToList();
                report.TopVarianceItems = report.VarianceDetails
                    .OrderByDescending(vd => Math.Abs(vd.VarianceValue))
                    .Take(10)
                    .Select(vd => new ItemVarianceDto
                    {
                        ItemId = vd.ItemId,
                        ItemName = vd.ItemName,
                        SystemQuantity = vd.SystemQuantity,
                        PhysicalQuantity = vd.PhysicalQuantity,
                        Variance = vd.Variance,
                        VarianceValue = vd.VarianceValue
                    })
                    .ToList();

                // Calculate variance percentage
                report.VariancePercentage = report.TotalSystemValue > 0
                    ? (report.TotalVarianceValue / report.TotalSystemValue) * 100
                    : 0;

                // Determine if approval is required (variance > 5%)
                report.RequiresApproval = Math.Abs(report.VariancePercentage) > 5;

                // Generate recommended actions
                if (report.ItemsWithNegativeVariance > 0)
                {
                    report.RecommendedActions.Add($"Investigate {report.ItemsWithNegativeVariance} items with shortages");
                }
                if (report.ItemsWithPositiveVariance > 0)
                {
                    report.RecommendedActions.Add($"Review {report.ItemsWithPositiveVariance} items with overages");
                }
                if (report.RequiresApproval)
                {
                    report.RecommendedActions.Add("Variance exceeds threshold - management approval required");
                }
                if (report.TopVarianceItems.Any())
                {
                    report.RecommendedActions.Add("Prioritize investigation of top variance items by value");
                }

                _logger.LogInformation("Variance report generated: {ItemsWithVariance} items with variance, Total Variance Value: {VarianceValue}",
                    report.ItemsWithVariance.Count, report.TotalVarianceValue);
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating variance report for physical inventory {InventoryId}", physicalInventoryId);
                throw;
            }
        }

        public async Task<ABCAnalysisReportDto> GetABCAnalysisReportAsync(string analysisMethod = "Value", int months = 12)
        {
            try
            {
                _logger.LogInformation("Generating ABC analysis report using {Method} method for {Months} months", analysisMethod, months);

                var report = new ABCAnalysisReportDto
                {
                    AnalysisDate = DateTime.Now,
                    AnalysisMethod = analysisMethod,
                    ClassASummary = new ABCClassSummaryDto { ClassName = "A" },
                    ClassBSummary = new ABCClassSummaryDto { ClassName = "B" },
                    ClassCSummary = new ABCClassSummaryDto { ClassName = "C" }
                };

                var startDate = DateTime.Now.AddMonths(-months);
                var itemAnalysis = new Dictionary<int, ABCItemDto>();

                // Get all items
                var items = await _unitOfWork.Items.GetAllAsync();

                foreach (var item in items.Where(i => i.IsActive))
                {
                    var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                    var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;

                    var abcItem = new ABCItemDto
                    {
                        ItemId = item.Id,
                        ItemCode = item.ItemCode,
                        ItemName = item.Name,
                        CategoryName = category?.Name ?? "N/A",
                        AnnualConsumptionValue = 0,
                        AnnualConsumptionQuantity = 0,
                        PercentageOfTotalValue = 0,
                        CumulativePercentage = 0,
                        ABCClass = "C"
                    };

                    // Calculate consumption from issues
                    var issueItems = await _unitOfWork.IssueItems.FindAsync(ii =>
                        ii.ItemId == item.Id &&
                        ii.Issue.IssueDate >= startDate);

                    var totalQuantity = issueItems.Sum(ii => ii.Quantity);
                    abcItem.AnnualConsumptionQuantity = totalQuantity;

                    // Get unit price from last purchase
                    var lastPurchase = await _unitOfWork.PurchaseItems.FirstOrDefaultAsync(pi => pi.ItemId == item.Id);
                    var unitPrice = lastPurchase?.UnitPrice ?? 0;

                    abcItem.AnnualConsumptionValue = totalQuantity * unitPrice;

                    // Calculate movement frequency from purchases and issues
                    var purchaseItems = await _unitOfWork.PurchaseItems.FindAsync(pi =>
                        pi.ItemId == item.Id &&
                        pi.Purchase.PurchaseDate >= startDate);

                    abcItem.MovementFrequency = issueItems.Count() + purchaseItems.Count();

                    // Get current stock
                    var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);
                    abcItem.CurrentStock = storeItems.Sum(si => si.Quantity ?? 0);
                    abcItem.Unit = item.Unit;

                    itemAnalysis[item.Id] = abcItem;
                }

                // Determine classification based on analysis method
                List<ABCItemDto> sortedItems;
                decimal totalValue = 0;

                switch (analysisMethod.ToUpper())
                {
                    case "VALUE":
                        sortedItems = itemAnalysis.Values.OrderByDescending(i => i.AnnualConsumptionValue).ToList();
                        totalValue = sortedItems.Sum(i => i.AnnualConsumptionValue);
                        break;

                    case "QUANTITY":
                        sortedItems = itemAnalysis.Values.OrderByDescending(i => i.AnnualConsumptionQuantity).ToList();
                        totalValue = sortedItems.Sum(i => i.AnnualConsumptionQuantity);
                        break;

                    case "MOVEMENT":
                        sortedItems = itemAnalysis.Values.OrderByDescending(i => i.MovementFrequency).ToList();
                        totalValue = sortedItems.Sum(i => i.MovementFrequency);
                        break;

                    default:
                        sortedItems = itemAnalysis.Values.OrderByDescending(i => i.AnnualConsumptionValue).ToList();
                        totalValue = sortedItems.Sum(i => i.AnnualConsumptionValue);
                        break;
                }

                // Calculate percentages and cumulative percentages
                decimal cumulativePercentage = 0;
                foreach (var item in sortedItems)
                {
                    decimal itemValue = analysisMethod.ToUpper() switch
                    {
                        "VALUE" => item.AnnualConsumptionValue,
                        "QUANTITY" => item.AnnualConsumptionQuantity,
                        "MOVEMENT" => item.MovementFrequency,
                        _ => item.AnnualConsumptionValue
                    };

                    item.PercentageOfTotalValue = totalValue > 0 ? (itemValue / totalValue) * 100 : 0;
                    cumulativePercentage += item.PercentageOfTotalValue;
                    item.CumulativePercentage = cumulativePercentage;

                    // Classify: A = 70%, B = 20%, C = 10% (80-20 rule variant)
                    if (cumulativePercentage <= 70)
                    {
                        item.ABCClass = "A";
                        report.ClassAItems.Add(item);
                        report.ClassASummary.ItemCount++;
                        report.ClassASummary.TotalValue += item.AnnualConsumptionValue;
                    }
                    else if (cumulativePercentage <= 90)
                    {
                        item.ABCClass = "B";
                        report.ClassBItems.Add(item);
                        report.ClassBSummary.ItemCount++;
                        report.ClassBSummary.TotalValue += item.AnnualConsumptionValue;
                    }
                    else
                    {
                        item.ABCClass = "C";
                        report.ClassCItems.Add(item);
                        report.ClassCSummary.ItemCount++;
                        report.ClassCSummary.TotalValue += item.AnnualConsumptionValue;
                    }
                }

                // Calculate summary percentages
                var totalItems = sortedItems.Count;
                var totalConsumptionValue = sortedItems.Sum(i => i.AnnualConsumptionValue);

                if (totalItems > 0)
                {
                    report.ClassASummary.PercentageOfTotalItems = (report.ClassASummary.ItemCount / (decimal)totalItems) * 100;
                    report.ClassBSummary.PercentageOfTotalItems = (report.ClassBSummary.ItemCount / (decimal)totalItems) * 100;
                    report.ClassCSummary.PercentageOfTotalItems = (report.ClassCSummary.ItemCount / (decimal)totalItems) * 100;
                }

                if (totalConsumptionValue > 0)
                {
                    report.ClassASummary.PercentageOfTotalValue = (report.ClassASummary.TotalValue / totalConsumptionValue) * 100;
                    report.ClassBSummary.PercentageOfTotalValue = (report.ClassBSummary.TotalValue / totalConsumptionValue) * 100;
                    report.ClassCSummary.PercentageOfTotalValue = (report.ClassCSummary.TotalValue / totalConsumptionValue) * 100;
                }

                _logger.LogInformation("ABC analysis report generated: Class A: {ClassA} items, Class B: {ClassB} items, Class C: {ClassC} items",
                    report.ClassASummary.ItemCount, report.ClassBSummary.ItemCount, report.ClassCSummary.ItemCount);
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating ABC analysis report");
                throw;
            }
        }

        // ========== NEW REPORT EXPORT METHODS ==========

        public async Task<byte[]> GenerateConsumptionReportExcelAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null)
        {
            var report = await GetConsumptionAnalysisReportAsync(fromDate, toDate, storeId, categoryId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Consumption Report");

            // Title and Date Range
            worksheet.Cell(1, 1).Value = "Consumption Analysis Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, 10).Merge();

            worksheet.Cell(2, 1).Value = $"Period: {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}";
            worksheet.Range(2, 1, 2, 10).Merge();

            // Headers
            var headerRow = 4;
            worksheet.Cell(headerRow, 1).Value = "#";
            worksheet.Cell(headerRow, 2).Value = "Item Code";
            worksheet.Cell(headerRow, 3).Value = "Item Name";
            worksheet.Cell(headerRow, 4).Value = "Category";
            worksheet.Cell(headerRow, 5).Value = "Total Quantity";
            worksheet.Cell(headerRow, 6).Value = "Unit";
            worksheet.Cell(headerRow, 7).Value = "Avg Consumption";
            worksheet.Cell(headerRow, 8).Value = "Total Value";
            worksheet.Cell(headerRow, 9).Value = "Avg Unit Price";
            worksheet.Cell(headerRow, 10).Value = "Consumption Count";

            worksheet.Range(headerRow, 1, headerRow, 10).Style.Font.Bold = true;
            worksheet.Range(headerRow, 1, headerRow, 10).Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data
            var row = headerRow + 1;
            int serialNo = 1;
            foreach (var item in report.Items)
            {
                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = item.ItemCode;
                worksheet.Cell(row, 3).Value = item.ItemName;
                worksheet.Cell(row, 4).Value = item.CategoryName;
                worksheet.Cell(row, 5).Value = item.TotalQuantity;
                worksheet.Cell(row, 6).Value = item.Unit;
                worksheet.Cell(row, 7).Value = item.AverageConsumption;
                worksheet.Cell(row, 8).Value = item.TotalValue;
                worksheet.Cell(row, 9).Value = item.AverageUnitPrice;
                worksheet.Cell(row, 10).Value = item.ConsumptionCount;
                row++;
                serialNo++;
            }

            // Totals
            worksheet.Cell(row, 4).Value = "TOTAL:";
            worksheet.Cell(row, 4).Style.Font.Bold = true;
            worksheet.Cell(row, 8).Value = report.TotalConsumptionValue;
            worksheet.Cell(row, 8).Style.Font.Bold = true;
            worksheet.Range(row, 1, row, 10).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Formatting
            worksheet.Columns(5, 5).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Columns(7, 9).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateExpiryReportExcelAsync(int? storeId = null, int? daysAhead = 90)
        {
            var report = await GetExpiryReportAsync(storeId, daysAhead);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Expiry Report");

            // Title
            worksheet.Cell(1, 1).Value = "Item Expiry Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, 11).Merge();

            worksheet.Cell(2, 1).Value = $"Report Date: {report.AsOfDate:dd MMM yyyy}";
            worksheet.Cell(2, 7).Value = $"Monitoring Period: Next {daysAhead} Days";

            // Headers
            var headerRow = 4;
            worksheet.Cell(headerRow, 1).Value = "#";
            worksheet.Cell(headerRow, 2).Value = "Item Code";
            worksheet.Cell(headerRow, 3).Value = "Item Name";
            worksheet.Cell(headerRow, 4).Value = "Batch Number";
            worksheet.Cell(headerRow, 5).Value = "Expiry Date";
            worksheet.Cell(headerRow, 6).Value = "Days to Expiry";
            worksheet.Cell(headerRow, 7).Value = "Quantity";
            worksheet.Cell(headerRow, 8).Value = "Unit";
            worksheet.Cell(headerRow, 9).Value = "Value";
            worksheet.Cell(headerRow, 10).Value = "Store";
            worksheet.Cell(headerRow, 11).Value = "Status";

            worksheet.Range(headerRow, 1, headerRow, 11).Style.Font.Bold = true;
            worksheet.Range(headerRow, 1, headerRow, 11).Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data
            var row = headerRow + 1;
            int serialNo = 1;
            foreach (var item in report.ExpiringItems)
            {
                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = item.ItemCode;
                worksheet.Cell(row, 3).Value = item.ItemName;
                worksheet.Cell(row, 4).Value = item.BatchNumber;
                worksheet.Cell(row, 5).Value = item.ExpiryDate.ToString("dd MMM yyyy");
                worksheet.Cell(row, 6).Value = item.DaysToExpiry;
                worksheet.Cell(row, 7).Value = item.Quantity;
                worksheet.Cell(row, 8).Value = item.Unit;
                worksheet.Cell(row, 9).Value = item.Value;
                worksheet.Cell(row, 10).Value = item.StoreName;
                worksheet.Cell(row, 11).Value = item.Status;

                // Color coding
                if (item.Status == "Expired")
                    worksheet.Range(row, 1, row, 11).Style.Fill.BackgroundColor = XLColor.Red;
                else if (item.DaysToExpiry <= 7)
                    worksheet.Range(row, 1, row, 11).Style.Fill.BackgroundColor = XLColor.Orange;
                else if (item.DaysToExpiry <= 30)
                    worksheet.Range(row, 1, row, 11).Style.Fill.BackgroundColor = XLColor.LightYellow;

                row++;
                serialNo++;
            }

            // Summary
            row++;
            worksheet.Cell(row, 1).Value = "Summary Statistics:";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            row++;
            worksheet.Cell(row, 1).Value = "Total Expiring Items:";
            worksheet.Cell(row, 2).Value = report.TotalExpiringItems;
            row++;
            worksheet.Cell(row, 1).Value = "Total Expired Items:";
            worksheet.Cell(row, 2).Value = report.TotalExpiredItems;
            row++;
            worksheet.Cell(row, 1).Value = "Total Expired Value:";
            worksheet.Cell(row, 2).Value = report.TotalExpiredValue;
            worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
            row++;
            worksheet.Cell(row, 1).Value = "Total Expiring Value:";
            worksheet.Cell(row, 2).Value = report.TotalExpiringValue;
            worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";

            // Formatting
            worksheet.Columns(6, 6).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Columns(8, 8).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateAuditTrailReportExcelAsync(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null)
        {
            var report = await GetAuditTrailReportAsync(fromDate, toDate, transactionType, storeId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Audit Trail");

            // Title
            worksheet.Cell(1, 1).Value = "Audit Trail Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, 10).Merge();

            worksheet.Cell(2, 1).Value = $"Period: {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}";
            worksheet.Range(2, 1, 2, 10).Merge();

            // Headers
            var headerRow = 4;
            worksheet.Cell(headerRow, 1).Value = "#";
            worksheet.Cell(headerRow, 2).Value = "Date";
            worksheet.Cell(headerRow, 3).Value = "Transaction Type";
            worksheet.Cell(headerRow, 4).Value = "Reference No";
            worksheet.Cell(headerRow, 5).Value = "Item Name";
            worksheet.Cell(headerRow, 6).Value = "Store";
            worksheet.Cell(headerRow, 7).Value = "Quantity";
            worksheet.Cell(headerRow, 8).Value = "Action";
            worksheet.Cell(headerRow, 9).Value = "Performed By";
            worksheet.Cell(headerRow, 10).Value = "Remarks";

            worksheet.Range(headerRow, 1, headerRow, 10).Style.Font.Bold = true;
            worksheet.Range(headerRow, 1, headerRow, 10).Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data
            var row = headerRow + 1;
            int serialNo = 1;
            foreach (var item in report.Transactions)
            {
                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = item.TransactionDate.ToString("dd MMM yyyy HH:mm");
                worksheet.Cell(row, 3).Value = item.TransactionType;
                worksheet.Cell(row, 4).Value = item.ReferenceNo;
                worksheet.Cell(row, 5).Value = item.ItemName;
                worksheet.Cell(row, 6).Value = item.StoreName;
                worksheet.Cell(row, 7).Value = item.Quantity;
                worksheet.Cell(row, 8).Value = item.Action;
                worksheet.Cell(row, 9).Value = item.PerformedBy;
                worksheet.Cell(row, 10).Value = item.Remarks;
                row++;
                serialNo++;
            }

            // Summary
            row++;
            worksheet.Cell(row, 1).Value = "Total Transactions:";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 2).Value = report.TotalTransactions;
            worksheet.Range(row, 1, row, 10).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Formatting
            worksheet.Columns(6, 6).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateVarianceReportExcelAsync(int physicalInventoryId)
        {
            var report = await GetVarianceReportAsync(physicalInventoryId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Variance Analysis");

            // Title
            worksheet.Cell(1, 1).Value = "Variance Analysis Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, 7).Merge();

            worksheet.Cell(2, 1).Value = $"Reference: {report.ReferenceNumber}";
            worksheet.Cell(2, 5).Value = $"Count Date: {report.CountDate:dd MMM yyyy}";

            // Headers
            var headerRow = 4;
            worksheet.Cell(headerRow, 1).Value = "#";
            worksheet.Cell(headerRow, 2).Value = "Item Name";
            worksheet.Cell(headerRow, 3).Value = "System Qty";
            worksheet.Cell(headerRow, 4).Value = "Physical Qty";
            worksheet.Cell(headerRow, 5).Value = "Variance Qty";
            worksheet.Cell(headerRow, 6).Value = "Variance Value";
            worksheet.Cell(headerRow, 7).Value = "Status";

            worksheet.Range(headerRow, 1, headerRow, 7).Style.Font.Bold = true;
            worksheet.Range(headerRow, 1, headerRow, 7).Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data
            var row = headerRow + 1;
            int serialNo = 1;
            foreach (var item in report.ItemsWithVariance)
            {
                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = item.ItemName;
                worksheet.Cell(row, 3).Value = item.SystemQuantity;
                worksheet.Cell(row, 4).Value = item.PhysicalQuantity;
                worksheet.Cell(row, 5).Value = item.Variance;
                worksheet.Cell(row, 6).Value = item.VarianceValue;

                string status = item.Variance == 0 ? "Match" : item.Variance < 0 ? "Shortage" : "Overage";
                worksheet.Cell(row, 7).Value = status;

                // Color coding
                if (item.Variance < 0)
                    worksheet.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.LightYellow;
                else if (item.Variance > 0)
                    worksheet.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.Pink;

                row++;
                serialNo++;
            }

            // Summary
            row++;
            worksheet.Cell(row, 1).Value = "Summary:";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            row++;
            worksheet.Cell(row, 1).Value = "Total Items Counted:";
            worksheet.Cell(row, 2).Value = report.TotalItems;
            row++;
            worksheet.Cell(row, 1).Value = "Items with Shortage:";
            worksheet.Cell(row, 2).Value = report.ItemsWithShortage;
            row++;
            worksheet.Cell(row, 1).Value = "Items with Overage:";
            worksheet.Cell(row, 2).Value = report.ItemsWithOverage;
            row++;
            worksheet.Cell(row, 1).Value = "Total Variance Value:";
            worksheet.Cell(row, 2).Value = report.TotalVarianceValue;
            worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 2).Style.Font.Bold = true;

            // Formatting
            worksheet.Columns(2, 5).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateABCAnalysisReportExcelAsync(string analysisMethod = "Value", int months = 12)
        {
            var report = await GetABCAnalysisReportAsync(analysisMethod, months);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ABC Analysis");

            // Title
            worksheet.Cell(1, 1).Value = "ABC Analysis Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, 9).Merge();

            worksheet.Cell(2, 1).Value = $"Analysis Method: {report.AnalysisMethod}";
            worksheet.Cell(2, 5).Value = $"Period: Last {report.Months} Months";
            worksheet.Cell(2, 7).Value = $"Analysis Date: {report.AnalysisDate:dd MMM yyyy}";

            // Summary
            var summaryRow = 4;
            worksheet.Cell(summaryRow, 1).Value = "Class";
            worksheet.Cell(summaryRow, 2).Value = "Item Count";
            worksheet.Cell(summaryRow, 3).Value = "% of Items";
            worksheet.Cell(summaryRow, 4).Value = "Total Value";
            worksheet.Cell(summaryRow, 5).Value = "% of Value";
            worksheet.Range(summaryRow, 1, summaryRow, 5).Style.Font.Bold = true;
            worksheet.Range(summaryRow, 1, summaryRow, 5).Style.Fill.BackgroundColor = XLColor.LightGray;

            summaryRow++;
            worksheet.Cell(summaryRow, 1).Value = "A";
            worksheet.Cell(summaryRow, 2).Value = report.ClassASummary.ItemCount;
            worksheet.Cell(summaryRow, 3).Value = report.ClassASummary.PercentageOfTotalItems;
            worksheet.Cell(summaryRow, 4).Value = report.ClassASummary.TotalValue;
            worksheet.Cell(summaryRow, 5).Value = report.ClassASummary.PercentageOfTotalValue;
            worksheet.Range(summaryRow, 1, summaryRow, 5).Style.Fill.BackgroundColor = XLColor.LightGreen;

            summaryRow++;
            worksheet.Cell(summaryRow, 1).Value = "B";
            worksheet.Cell(summaryRow, 2).Value = report.ClassBSummary.ItemCount;
            worksheet.Cell(summaryRow, 3).Value = report.ClassBSummary.PercentageOfTotalItems;
            worksheet.Cell(summaryRow, 4).Value = report.ClassBSummary.TotalValue;
            worksheet.Cell(summaryRow, 5).Value = report.ClassBSummary.PercentageOfTotalValue;
            worksheet.Range(summaryRow, 1, summaryRow, 5).Style.Fill.BackgroundColor = XLColor.LightYellow;

            summaryRow++;
            worksheet.Cell(summaryRow, 1).Value = "C";
            worksheet.Cell(summaryRow, 2).Value = report.ClassCSummary.ItemCount;
            worksheet.Cell(summaryRow, 3).Value = report.ClassCSummary.PercentageOfTotalItems;
            worksheet.Cell(summaryRow, 4).Value = report.ClassCSummary.TotalValue;
            worksheet.Cell(summaryRow, 5).Value = report.ClassCSummary.PercentageOfTotalValue;
            worksheet.Range(summaryRow, 1, summaryRow, 5).Style.Fill.BackgroundColor = XLColor.Pink;

            // Detailed Items Headers
            var headerRow = summaryRow + 2;
            worksheet.Cell(headerRow, 1).Value = "#";
            worksheet.Cell(headerRow, 2).Value = "Class";
            worksheet.Cell(headerRow, 3).Value = "Item Code";
            worksheet.Cell(headerRow, 4).Value = "Item Name";
            worksheet.Cell(headerRow, 5).Value = "Category";
            worksheet.Cell(headerRow, 6).Value = "Annual Value";
            worksheet.Cell(headerRow, 7).Value = "Annual Quantity";
            worksheet.Cell(headerRow, 8).Value = "% of Total";
            worksheet.Cell(headerRow, 9).Value = "Cumulative %";
            worksheet.Cell(headerRow, 10).Value = "Current Stock";

            worksheet.Range(headerRow, 1, headerRow, 10).Style.Font.Bold = true;
            worksheet.Range(headerRow, 1, headerRow, 10).Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data - Class A Items
            var row = headerRow + 1;
            int serialNo = 1;
            foreach (var item in report.ClassAItems)
            {
                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = "A";
                worksheet.Cell(row, 3).Value = item.ItemCode;
                worksheet.Cell(row, 4).Value = item.ItemName;
                worksheet.Cell(row, 5).Value = item.CategoryName;
                worksheet.Cell(row, 6).Value = item.AnnualConsumptionValue;
                worksheet.Cell(row, 7).Value = item.AnnualConsumptionQuantity;
                worksheet.Cell(row, 8).Value = item.PercentageOfTotalValue;
                worksheet.Cell(row, 9).Value = item.CumulativePercentage;
                worksheet.Cell(row, 10).Value = item.CurrentStock;
                row++;
                serialNo++;
            }

            // Class B Items
            foreach (var item in report.ClassBItems)
            {
                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = "B";
                worksheet.Cell(row, 3).Value = item.ItemCode;
                worksheet.Cell(row, 4).Value = item.ItemName;
                worksheet.Cell(row, 5).Value = item.CategoryName;
                worksheet.Cell(row, 6).Value = item.AnnualConsumptionValue;
                worksheet.Cell(row, 7).Value = item.AnnualConsumptionQuantity;
                worksheet.Cell(row, 8).Value = item.PercentageOfTotalValue;
                worksheet.Cell(row, 9).Value = item.CumulativePercentage;
                worksheet.Cell(row, 10).Value = item.CurrentStock;
                row++;
                serialNo++;
            }

            // Class C Items
            foreach (var item in report.ClassCItems)
            {
                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = "C";
                worksheet.Cell(row, 3).Value = item.ItemCode;
                worksheet.Cell(row, 4).Value = item.ItemName;
                worksheet.Cell(row, 5).Value = item.CategoryName;
                worksheet.Cell(row, 6).Value = item.AnnualConsumptionValue;
                worksheet.Cell(row, 7).Value = item.AnnualConsumptionQuantity;
                worksheet.Cell(row, 8).Value = item.PercentageOfTotalValue;
                worksheet.Cell(row, 9).Value = item.CumulativePercentage;
                worksheet.Cell(row, 10).Value = item.CurrentStock;
                row++;
                serialNo++;
            }

            // Formatting
            worksheet.Columns(3, 3).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Columns(5, 9).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // ========== NEW REPORT PDF EXPORT METHODS ==========

        public async Task<byte[]> GenerateStockReportPdfAsync(int? storeId = null, int? categoryId = null)
        {
            var stockReport = await GetStockReportAsync(storeId, categoryId);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Organization Header
            var orgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18, new iTextSharp.text.BaseColor(64, 64, 64));
            var orgName = new iTextSharp.text.Paragraph("ANSAR & VDP", orgFont);
            orgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(orgName);

            var subOrgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 12, new iTextSharp.text.BaseColor(64, 64, 64));
            var subOrgName = new iTextSharp.text.Paragraph("Inventory Management System", subOrgFont);
            subOrgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            subOrgName.SpacingAfter = 5f;
            document.Add(subOrgName);

            // Report Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16, new iTextSharp.text.BaseColor(0, 0, 0));
            var title = new iTextSharp.text.Paragraph("STOCK REPORT", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            title.SpacingBefore = 10f;
            document.Add(title);

            // Filter Info
            string filterInfo = "All Items";
            if (storeId.HasValue)
            {
                var store = stockReport.StockItems.FirstOrDefault()?.StoreName;
                if (!string.IsNullOrEmpty(store))
                    filterInfo = $"Store: {store}";
            }
            if (categoryId.HasValue)
            {
                var category = stockReport.StockItems.FirstOrDefault()?.CategoryName;
                if (!string.IsNullOrEmpty(category))
                    filterInfo += categoryId.HasValue && storeId.HasValue ? $" | Category: {category}" : $"Category: {category}";
            }

            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);
            var filterPara = new iTextSharp.text.Paragraph(filterInfo, infoFont);
            filterPara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(filterPara);

            var reportDate = new iTextSharp.text.Paragraph($"Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}", infoFont);
            reportDate.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            reportDate.SpacingAfter = 15f;
            document.Add(reportDate);

            // Table with 11 columns (added #Ser)
            var table = new iTextSharp.text.pdf.PdfPTable(11);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 8f, 15f, 10f, 12f, 8f, 7f, 7f, 8f, 10f, 8f });
            table.SpacingBefore = 10f;

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 8, new iTextSharp.text.BaseColor(255, 255, 255));
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 7);

            string[] headers = { "#Ser", "Code", "Item Name", "Category", "Store", "Stock", "Min", "Max", "Price", "Value", "Status" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(41, 128, 185); // Professional blue
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                cell.Padding = 6;
                cell.BorderWidth = 1f;
                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
                table.AddCell(cell);
            }

            // Data rows
            int serialNo = 1;
            bool isAlternateRow = false;
            var alternateBgColor = new iTextSharp.text.BaseColor(245, 245, 245);
            var whiteBgColor = new iTextSharp.text.BaseColor(255, 255, 255);
            var outOfStockBg = new iTextSharp.text.BaseColor(255, 220, 220);
            var lowStockBg = new iTextSharp.text.BaseColor(255, 255, 224);

            foreach (var item in stockReport.StockItems)
            {
                var rowBgColor = whiteBgColor;
                if (item.StockStatus == "Out of Stock")
                    rowBgColor = outOfStockBg;
                else if (item.StockStatus == "Low Stock")
                    rowBgColor = lowStockBg;
                else if (isAlternateRow)
                    rowBgColor = alternateBgColor;

                // Serial Number
                var serCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                serCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                serCell.Padding = 4;
                serCell.BackgroundColor = rowBgColor;
                serCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(serCell);

                // Item Code
                var codeCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.ItemCode ?? "", cellFont));
                codeCell.Padding = 4;
                codeCell.BackgroundColor = rowBgColor;
                codeCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(codeCell);

                // Item Name
                var nameCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.ItemName ?? "", cellFont));
                nameCell.Padding = 4;
                nameCell.BackgroundColor = rowBgColor;
                nameCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(nameCell);

                // Category
                var catCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.CategoryName ?? "", cellFont));
                catCell.Padding = 4;
                catCell.BackgroundColor = rowBgColor;
                catCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(catCell);

                // Store
                var storeCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.StoreName ?? "", cellFont));
                storeCell.Padding = 4;
                storeCell.BackgroundColor = rowBgColor;
                storeCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(storeCell);

                // Current Stock
                var stockCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(string.Format("{0:N0}", item.CurrentStock), cellFont));
                stockCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                stockCell.Padding = 4;
                stockCell.BackgroundColor = rowBgColor;
                stockCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(stockCell);

                // Min Stock
                var minCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(string.Format("{0:N0}", item.MinimumStock), cellFont));
                minCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                minCell.Padding = 4;
                minCell.BackgroundColor = rowBgColor;
                minCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(minCell);

                // Max Stock
                var maxCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(string.Format("{0:N0}", item.MaximumStock), cellFont));
                maxCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                maxCell.Padding = 4;
                maxCell.BackgroundColor = rowBgColor;
                maxCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(maxCell);

                // Unit Price
                var priceCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(string.Format("{0:N2}", item.UnitPrice), cellFont));
                priceCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                priceCell.Padding = 4;
                priceCell.BackgroundColor = rowBgColor;
                priceCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(priceCell);

                // Total Value
                var valueCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(string.Format("{0:N2}", item.TotalValue), cellFont));
                valueCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                valueCell.Padding = 4;
                valueCell.BackgroundColor = rowBgColor;
                valueCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(valueCell);

                // Status
                var statusCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.StockStatus ?? "", cellFont));
                statusCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                statusCell.Padding = 4;
                statusCell.BackgroundColor = rowBgColor;
                statusCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(statusCell);

                serialNo++;
                isAlternateRow = !isAlternateRow;
            }

            document.Add(table);

            // Summary Table
            var summaryTable = new iTextSharp.text.pdf.PdfPTable(4);
            summaryTable.WidthPercentage = 100;
            summaryTable.SpacingBefore = 15f;
            summaryTable.SetWidths(new float[] { 1f, 1f, 1f, 1f });

            var summaryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var summaryBgColor = new iTextSharp.text.BaseColor(52, 152, 219);

            // Total Items
            var totalItemsCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"Total Items: {stockReport.TotalItems}", summaryFont));
            totalItemsCell.BackgroundColor = summaryBgColor;
            totalItemsCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            totalItemsCell.Padding = 10;
            totalItemsCell.BorderWidth = 0;
            summaryTable.AddCell(totalItemsCell);

            // Total Value
            var totalValueCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"Total Value: {stockReport.TotalValue:N2}", summaryFont));
            totalValueCell.BackgroundColor = summaryBgColor;
            totalValueCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            totalValueCell.Padding = 10;
            totalValueCell.BorderWidth = 0;
            summaryTable.AddCell(totalValueCell);

            // Low Stock Items
            var lowStockCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"Low Stock: {stockReport.LowStockItems}", summaryFont));
            lowStockCell.BackgroundColor = summaryBgColor;
            lowStockCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            lowStockCell.Padding = 10;
            lowStockCell.BorderWidth = 0;
            summaryTable.AddCell(lowStockCell);

            // Out of Stock Items
            var outStockCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"Out of Stock: {stockReport.OutOfStockItems}", summaryFont));
            outStockCell.BackgroundColor = summaryBgColor;
            outStockCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            outStockCell.Padding = 10;
            outStockCell.BorderWidth = 0;
            summaryTable.AddCell(outStockCell);

            document.Add(summaryTable);

            // Footer
            var footerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8, new iTextSharp.text.BaseColor(128, 128, 128));
            var footer = new iTextSharp.text.Paragraph("\n\nThis is a system-generated report from ANSAR & VDP Inventory Management System.", footerFont);
            footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(footer);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GeneratePurchaseReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? vendorId = null)
        {
            var purchases = await GetPurchaseReportAsync(fromDate, toDate, vendorId);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Organization Header
            var orgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18, new iTextSharp.text.BaseColor(64, 64, 64));
            var orgName = new iTextSharp.text.Paragraph("ANSAR & VDP", orgFont);
            orgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(orgName);

            var subOrgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 12, new iTextSharp.text.BaseColor(64, 64, 64));
            var subOrgName = new iTextSharp.text.Paragraph("Inventory Management System", subOrgFont);
            subOrgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            subOrgName.SpacingAfter = 5f;
            document.Add(subOrgName);

            // Report Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16, new iTextSharp.text.BaseColor(0, 0, 0));
            var title = new iTextSharp.text.Paragraph("PURCHASE REPORT", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            title.SpacingBefore = 10f;
            document.Add(title);

            // Filter Info
            string filterInfo = "All Purchases";
            if (fromDate.HasValue && toDate.HasValue)
            {
                filterInfo = $"Period: {fromDate.Value:dd MMM yyyy} to {toDate.Value:dd MMM yyyy}";
            }
            else if (fromDate.HasValue)
            {
                filterInfo = $"From: {fromDate.Value:dd MMM yyyy}";
            }
            else if (toDate.HasValue)
            {
                filterInfo = $"Until: {toDate.Value:dd MMM yyyy}";
            }

            if (vendorId.HasValue)
            {
                var vendor = purchases.FirstOrDefault()?.VendorName;
                if (!string.IsNullOrEmpty(vendor))
                    filterInfo += $" | Vendor: {vendor}";
            }

            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);
            var filterPara = new iTextSharp.text.Paragraph(filterInfo, infoFont);
            filterPara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(filterPara);

            var reportDate = new iTextSharp.text.Paragraph($"Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}", infoFont);
            reportDate.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            reportDate.SpacingAfter = 15f;
            document.Add(reportDate);

            // Summary Section
            var totalAmount = purchases.Sum(p => p.TotalAmount);
            var totalDiscount = purchases.Sum(p => p.Discount);
            var netAmount = totalAmount - totalDiscount;

            var summaryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var summaryText = $"Total Orders: {purchases.Count()} | Total Amount: ৳{totalAmount:N2} | Discount: ৳{totalDiscount:N2} | Net Amount: ৳{netAmount:N2}";
            var summaryPara = new iTextSharp.text.Paragraph(summaryText, summaryFont);
            summaryPara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            summaryPara.SpacingAfter = 10f;
            document.Add(summaryPara);

            // Table with 8 columns
            var table = new iTextSharp.text.pdf.PdfPTable(8);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 12f, 10f, 15f, 8f, 10f, 10f, 10f });
            table.SpacingBefore = 10f;

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 8, new iTextSharp.text.BaseColor(255, 255, 255));
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 7);

            string[] headers = { "#Ser", "PO Number", "Date", "Vendor", "Items", "Total Amount", "Discount", "Net Amount" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(41, 128, 185); // Professional blue
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                cell.Padding = 6;
                cell.BorderWidth = 1f;
                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
                table.AddCell(cell);
            }

            // Data rows
            int serialNo = 1;
            bool isAlternateRow = false;
            var alternateBgColor = new iTextSharp.text.BaseColor(245, 245, 245);
            var whiteBgColor = new iTextSharp.text.BaseColor(255, 255, 255);

            foreach (var purchase in purchases.OrderByDescending(p => p.PurchaseDate))
            {
                var rowBgColor = isAlternateRow ? alternateBgColor : whiteBgColor;

                // Serial Number
                var serCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                serCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                serCell.Padding = 4;
                serCell.BackgroundColor = rowBgColor;
                serCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(serCell);

                // PO Number
                var poCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(purchase.PurchaseOrderNo ?? "", cellFont));
                poCell.Padding = 4;
                poCell.BackgroundColor = rowBgColor;
                poCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(poCell);

                // Date
                var dateCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(purchase.PurchaseDate.ToString("dd-MMM-yyyy"), cellFont));
                dateCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                dateCell.Padding = 4;
                dateCell.BackgroundColor = rowBgColor;
                dateCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(dateCell);

                // Vendor
                var vendorCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(purchase.VendorName ?? "Marketplace", cellFont));
                vendorCell.Padding = 4;
                vendorCell.BackgroundColor = rowBgColor;
                vendorCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(vendorCell);

                // Items Count
                var itemsCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(purchase.Items.Count.ToString(), cellFont));
                itemsCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                itemsCell.Padding = 4;
                itemsCell.BackgroundColor = rowBgColor;
                itemsCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(itemsCell);

                // Total Amount
                var totalCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"৳{purchase.TotalAmount:N2}", cellFont));
                totalCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                totalCell.Padding = 4;
                totalCell.BackgroundColor = rowBgColor;
                totalCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(totalCell);

                // Discount
                var discountCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"৳{purchase.Discount:N2}", cellFont));
                discountCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                discountCell.Padding = 4;
                discountCell.BackgroundColor = rowBgColor;
                discountCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(discountCell);

                // Net Amount
                var netCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"৳{(purchase.TotalAmount - purchase.Discount):N2}", cellFont));
                netCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                netCell.Padding = 4;
                netCell.BackgroundColor = rowBgColor;
                netCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(netCell);

                serialNo++;
                isAlternateRow = !isAlternateRow;
            }

            document.Add(table);

            // Footer
            var footerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8, new iTextSharp.text.BaseColor(128, 128, 128));
            var footer = new iTextSharp.text.Paragraph("\n\nThis is a system-generated report from ANSAR & VDP Inventory Management System.", footerFont);
            footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(footer);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateIssueReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null)
        {
            var issues = await GetIssueReportAsync(fromDate, toDate, storeId);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Organization Header
            var orgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18, new iTextSharp.text.BaseColor(64, 64, 64));
            var orgName = new iTextSharp.text.Paragraph("ANSAR & VDP", orgFont);
            orgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(orgName);

            var subOrgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 12, new iTextSharp.text.BaseColor(64, 64, 64));
            var subOrgName = new iTextSharp.text.Paragraph("Inventory Management System", subOrgFont);
            subOrgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            subOrgName.SpacingAfter = 5f;
            document.Add(subOrgName);

            // Report Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16, new iTextSharp.text.BaseColor(0, 0, 0));
            var title = new iTextSharp.text.Paragraph("ISSUE REPORT", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            title.SpacingBefore = 10f;
            document.Add(title);

            // Date Range and Report Info
            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);
            var dateRange = new iTextSharp.text.Paragraph($"Period: {fromDate?.ToString("dd MMM yyyy") ?? "All"} to {toDate?.ToString("dd MMM yyyy") ?? "Present"}", infoFont);
            dateRange.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(dateRange);

            var reportDate = new iTextSharp.text.Paragraph($"Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}", infoFont);
            reportDate.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            reportDate.SpacingAfter = 15f;
            document.Add(reportDate);

            // Table with 8 columns (added #Ser)
            var table = new iTextSharp.text.pdf.PdfPTable(8);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 6f, 12f, 11f, 20f, 14f, 18f, 9f, 10f });
            table.SpacingBefore = 10f;

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 9, new iTextSharp.text.BaseColor(255, 255, 255));
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

            string[] headers = { "#Ser", "Issue No", "Issue Date", "Issued To", "Type", "Purpose", "Items", "Total Qty" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(41, 128, 185); // Professional blue
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                cell.Padding = 8;
                cell.BorderWidth = 1f;
                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
                table.AddCell(cell);
            }

            // Data rows
            int serialNo = 1;
            bool isAlternateRow = false;
            var alternateBgColor = new iTextSharp.text.BaseColor(245, 245, 245); // Light gray for alternate rows

            foreach (var issue in issues)
            {
                var rowBgColor = isAlternateRow ? alternateBgColor : new iTextSharp.text.BaseColor(255, 255, 255);

                // Serial Number
                var serCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                serCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                serCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                serCell.Padding = 6;
                serCell.BackgroundColor = rowBgColor;
                serCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(serCell);

                // Issue No
                var issueNoCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(issue.IssueNo ?? "", cellFont));
                issueNoCell.Padding = 6;
                issueNoCell.BackgroundColor = rowBgColor;
                issueNoCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(issueNoCell);

                // Issue Date
                var dateCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(issue.IssueDate.ToString("dd MMM yyyy"), cellFont));
                dateCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                dateCell.Padding = 6;
                dateCell.BackgroundColor = rowBgColor;
                dateCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(dateCell);

                // Issued To (with fallback)
                string issuedToText = issue.IssuedTo ?? issue.IssuedToName ?? issue.IssuedToIndividualName ??
                                     issue.IssuedToBattalionName ?? issue.IssuedToRangeName ??
                                     issue.IssuedToZilaName ?? issue.IssuedToUpazilaName ??
                                     issue.IssuedToUnionName ?? "N/A";
                var issuedToCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(issuedToText, cellFont));
                issuedToCell.Padding = 6;
                issuedToCell.BackgroundColor = rowBgColor;
                issuedToCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(issuedToCell);

                // Type
                var typeCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(issue.IssuedToType ?? "", cellFont));
                typeCell.Padding = 6;
                typeCell.BackgroundColor = rowBgColor;
                typeCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(typeCell);

                // Purpose
                var purposeCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(issue.Purpose ?? "", cellFont));
                purposeCell.Padding = 6;
                purposeCell.BackgroundColor = rowBgColor;
                purposeCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(purposeCell);

                // Total Items
                var itemsCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(issue.Items?.Count().ToString() ?? "0", cellFont));
                itemsCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                itemsCell.Padding = 6;
                itemsCell.BackgroundColor = rowBgColor;
                itemsCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(itemsCell);

                // Total Quantity
                var qtyCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(issue.Items?.Sum(i => i.Quantity).ToString("N0") ?? "0", cellFont));
                qtyCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                qtyCell.Padding = 6;
                qtyCell.BackgroundColor = rowBgColor;
                qtyCell.BorderColor = new iTextSharp.text.BaseColor(220, 220, 220);
                table.AddCell(qtyCell);

                serialNo++;
                isAlternateRow = !isAlternateRow;
            }

            document.Add(table);

            // Summary Box
            var totalIssues = issues.Count();
            var totalItems = issues.Sum(i => i.Items?.Count() ?? 0);
            var totalQuantity = issues.Sum(i => i.Items?.Sum(item => item.Quantity) ?? 0);

            var summaryTable = new iTextSharp.text.pdf.PdfPTable(3);
            summaryTable.WidthPercentage = 100;
            summaryTable.SpacingBefore = 15f;
            summaryTable.SetWidths(new float[] { 1f, 1f, 1f });

            var summaryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var summaryBgColor = new iTextSharp.text.BaseColor(52, 152, 219); // Blue

            // Total Issues
            var totalIssuesCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"Total Issues: {totalIssues}", summaryFont));
            totalIssuesCell.BackgroundColor = summaryBgColor;
            totalIssuesCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            totalIssuesCell.Padding = 10;
            totalIssuesCell.BorderWidth = 0;
            summaryTable.AddCell(totalIssuesCell);

            // Total Items
            var totalItemsCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"Total Unique Items: {totalItems}", summaryFont));
            totalItemsCell.BackgroundColor = summaryBgColor;
            totalItemsCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            totalItemsCell.Padding = 10;
            totalItemsCell.BorderWidth = 0;
            summaryTable.AddCell(totalItemsCell);

            // Total Quantity
            var totalQtyCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"Total Quantity: {totalQuantity:N0}", summaryFont));
            totalQtyCell.BackgroundColor = summaryBgColor;
            totalQtyCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            totalQtyCell.Padding = 10;
            totalQtyCell.BorderWidth = 0;
            summaryTable.AddCell(totalQtyCell);

            document.Add(summaryTable);

            // Footer
            var footerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8, new iTextSharp.text.BaseColor(128, 128, 128));
            var footer = new iTextSharp.text.Paragraph("\n\nThis is a system-generated report from ANSAR & VDP Inventory Management System.", footerFont);
            footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(footer);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateConsumptionReportCsvAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null)
        {
            var report = await GetConsumptionAnalysisReportAsync(fromDate, toDate, storeId, categoryId);

            var csv = new StringBuilder();
            csv.AppendLine("Consumption Analysis Report");
            csv.AppendLine($"Period: {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}");
            csv.AppendLine();
            csv.AppendLine("#,Item Code,Item Name,Category,Total Quantity,Unit,Avg Unit Price,Total Value,% of Total");

            int serialNo = 1;
            foreach (var item in report.Items.OrderByDescending(x => x.TotalValue))
            {
                var percentage = report.TotalConsumptionValue > 0 ? (item.TotalValue / report.TotalConsumptionValue * 100) : 0;
                csv.AppendLine($"{serialNo}," +
                              $"\"{EscapeCsv(item.ItemCode)}\"," +
                              $"\"{EscapeCsv(item.ItemName)}\"," +
                              $"\"{EscapeCsv(item.CategoryName)}\"," +
                              $"{item.TotalQuantity:N2}," +
                              $"\"{EscapeCsv(item.Unit)}\"," +
                              $"{item.AverageUnitPrice:N2}," +
                              $"{item.TotalValue:N2}," +
                              $"{percentage:N2}%");
                serialNo++;
            }

            csv.AppendLine();
            csv.AppendLine($",,,,,,Total:,{report.TotalConsumptionValue:N2},100.00%");

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> GenerateConsumptionReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null)
        {
            var report = await GetConsumptionAnalysisReportAsync(fromDate, toDate, storeId, categoryId);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
            var title = new iTextSharp.text.Paragraph("Consumption Analysis Report", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(title);

            // Date Range
            var dateFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
            var dateRange = new iTextSharp.text.Paragraph($"Period: {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}", dateFont);
            dateRange.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            dateRange.SpacingAfter = 10f;
            document.Add(dateRange);

            // Table
            var table = new iTextSharp.text.pdf.PdfPTable(10);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 8f, 15f, 12f, 10f, 8f, 10f, 12f, 10f, 10f });

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

            string[] headers = { "#", "Item Code", "Item Name", "Category", "Total Qty", "Unit", "Avg Consumption", "Total Value", "Avg Unit Price", "Count" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(211, 211, 211);
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            // Data
            int serialNo = 1;
            foreach (var item in report.Items)
            {
                table.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ItemCode ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ItemName ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.CategoryName ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.TotalQuantity.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Unit ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.AverageConsumption.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.TotalValue.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.AverageUnitPrice.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ConsumptionCount.ToString(), cellFont));
                serialNo++;
            }

            document.Add(table);

            // Total
            var totalPara = new iTextSharp.text.Paragraph($"\nTotal Consumption Value: ?{report.TotalConsumptionValue:N2}", headerFont);
            totalPara.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
            document.Add(totalPara);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateExpiryReportPdfAsync(int? storeId = null, int? daysAhead = 90)
        {
            var report = await GetExpiryReportAsync(storeId, daysAhead);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
            var title = new iTextSharp.text.Paragraph("Item Expiry Report", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(title);

            // Info
            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
            var info = new iTextSharp.text.Paragraph($"Report Date: {report.AsOfDate:dd MMM yyyy} | Monitoring Period: Next {daysAhead} Days", infoFont);
            info.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            info.SpacingAfter = 10f;
            document.Add(info);

            // Table
            var table = new iTextSharp.text.pdf.PdfPTable(11);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 8f, 15f, 12f, 10f, 8f, 10f, 8f, 10f, 12f, 7f });

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 9);
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

            string[] headers = { "#", "Item Code", "Item Name", "Batch No", "Expiry Date", "Days", "Quantity", "Unit", "Value", "Store", "Status" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(211, 211, 211);
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            // Data
            int serialNo = 1;
            foreach (var item in report.ExpiringItems)
            {
                table.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ItemCode ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ItemName ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.BatchNumber ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ExpiryDate.ToString("dd MMM yyyy"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.DaysToExpiry.ToString(), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Quantity.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Unit ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Value.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.StoreName ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Status ?? "", cellFont));
                serialNo++;
            }

            document.Add(table);

            // Summary
            var summaryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var summary = new iTextSharp.text.Paragraph($"\nSummary: Total Expiring Items: {report.TotalExpiringItems} | Expired Items: {report.TotalExpiredItems}", summaryFont);
            document.Add(summary);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateAuditTrailReportPdfAsync(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null)
        {
            var report = await GetAuditTrailReportAsync(fromDate, toDate, transactionType, storeId);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
            var title = new iTextSharp.text.Paragraph("Audit Trail Report", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(title);

            // Date Range
            var dateFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
            var dateRange = new iTextSharp.text.Paragraph($"Period: {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}", dateFont);
            dateRange.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            dateRange.SpacingAfter = 10f;
            document.Add(dateRange);

            // Table
            var table = new iTextSharp.text.pdf.PdfPTable(10);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 12f, 12f, 10f, 15f, 12f, 10f, 10f, 12f, 7f });

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 9);
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

            string[] headers = { "#", "Date", "Type", "Ref No", "Item", "Store", "Quantity", "Action", "Performed By", "Remarks" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(211, 211, 211);
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            // Data
            int serialNo = 1;
            foreach (var item in report.Transactions.Take(100)) // Limit for PDF
            {
                table.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.TransactionDate.ToString("dd MMM yyyy"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.TransactionType ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ReferenceNo ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ItemName ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.StoreName ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Quantity.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Action ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.PerformedBy ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Remarks ?? "", cellFont));
                serialNo++;
            }

            document.Add(table);

            // Total
            var totalPara = new iTextSharp.text.Paragraph($"\nTotal Transactions: {report.TotalTransactions}", headerFont);
            document.Add(totalPara);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateVarianceReportPdfAsync(int physicalInventoryId)
        {
            var report = await GetVarianceReportAsync(physicalInventoryId);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
            var title = new iTextSharp.text.Paragraph("Variance Analysis Report", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(title);

            // Info
            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
            var info = new iTextSharp.text.Paragraph($"Reference: {report.ReferenceNumber} | Count Date: {report.CountDate:dd MMM yyyy}", infoFont);
            info.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            info.SpacingAfter = 10f;
            document.Add(info);

            // Table
            var table = new iTextSharp.text.pdf.PdfPTable(7);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 25f, 15f, 15f, 15f, 15f, 15f });

            // Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

            string[] headers = { "#", "Item Name", "System Qty", "Physical Qty", "Variance Qty", "Variance Value", "Status" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(211, 211, 211);
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            // Data
            int serialNo = 1;
            foreach (var item in report.ItemsWithVariance)
            {
                string status = item.Variance == 0 ? "Match" : item.Variance < 0 ? "Shortage" : "Overage";

                table.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.ItemName ?? "", cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.SystemQuantity.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.PhysicalQuantity.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.Variance.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(item.VarianceValue.ToString("N2"), cellFont));
                table.AddCell(new iTextSharp.text.Phrase(status, cellFont));
                serialNo++;
            }

            document.Add(table);

            // Summary
            var summaryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            var summary = new iTextSharp.text.Paragraph($"\nSummary: Total Items: {report.TotalItems} | Shortages: {report.ItemsWithShortage} | Overages: {report.ItemsWithOverage} | Total Variance Value: ?{report.TotalVarianceValue:N2}", summaryFont);
            document.Add(summary);

            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateABCAnalysisReportPdfAsync(string analysisMethod = "Value", int months = 12)
        {
            var report = await GetABCAnalysisReportAsync(analysisMethod, months);

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
            var title = new iTextSharp.text.Paragraph("ABC Analysis Report", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(title);

            // Info
            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
            var info = new iTextSharp.text.Paragraph($"Method: {report.AnalysisMethod} | Period: Last {report.Months} Months | Date: {report.AnalysisDate:dd MMM yyyy}", infoFont);
            info.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            info.SpacingAfter = 10f;
            document.Add(info);

            // Summary Table
            var summaryTable = new iTextSharp.text.pdf.PdfPTable(5);
            summaryTable.WidthPercentage = 70;
            summaryTable.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            summaryTable.SetWidths(new float[] { 10f, 15f, 20f, 20f, 20f });

            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
            string[] summaryHeaders = { "Class", "Items", "% of Items", "Total Value", "% of Value" };
            foreach (var header in summaryHeaders)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(211, 211, 211);
                cell.Padding = 5;
                summaryTable.AddCell(cell);
            }

            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

            // Class A
            summaryTable.AddCell(new iTextSharp.text.Phrase("A", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase(report.ClassASummary.ItemCount.ToString(), cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"{report.ClassASummary.PercentageOfTotalItems:N2}%", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"?{report.ClassASummary.TotalValue:N2}", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"{report.ClassASummary.PercentageOfTotalValue:N2}%", cellFont));

            // Class B
            summaryTable.AddCell(new iTextSharp.text.Phrase("B", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase(report.ClassBSummary.ItemCount.ToString(), cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"{report.ClassBSummary.PercentageOfTotalItems:N2}%", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"?{report.ClassBSummary.TotalValue:N2}", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"{report.ClassBSummary.PercentageOfTotalValue:N2}%", cellFont));

            // Class C
            summaryTable.AddCell(new iTextSharp.text.Phrase("C", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase(report.ClassCSummary.ItemCount.ToString(), cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"{report.ClassCSummary.PercentageOfTotalItems:N2}%", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"?{report.ClassCSummary.TotalValue:N2}", cellFont));
            summaryTable.AddCell(new iTextSharp.text.Phrase($"{report.ClassCSummary.PercentageOfTotalValue:N2}%", cellFont));

            document.Add(summaryTable);

            // Detailed Items Table (Top 50 only for PDF)
            document.Add(new iTextSharp.text.Paragraph("\nTop Items Detail:", headerFont));

            var detailTable = new iTextSharp.text.pdf.PdfPTable(8);
            detailTable.WidthPercentage = 100;
            detailTable.SpacingBefore = 10f;
            detailTable.SetWidths(new float[] { 5f, 8f, 10f, 18f, 15f, 15f, 15f, 10f });

            string[] detailHeaders = { "#", "Class", "Code", "Item Name", "Annual Value", "Annual Qty", "% of Total", "Stock" };
            foreach (var header in detailHeaders)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(211, 211, 211);
                cell.Padding = 5;
                detailTable.AddCell(cell);
            }

            var smallFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

            // Add top items from each class
            int serialNo = 1;
            foreach (var item in report.ClassAItems.Take(20))
            {
                detailTable.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), smallFont));
                detailTable.AddCell(new iTextSharp.text.Phrase("A", smallFont));
                detailTable.AddCell(new iTextSharp.text.Phrase(item.ItemCode ?? "", smallFont));
                detailTable.AddCell(new iTextSharp.text.Phrase(item.ItemName ?? "", smallFont));
                detailTable.AddCell(new iTextSharp.text.Phrase(item.AnnualConsumptionValue.ToString("N2"), smallFont));
                detailTable.AddCell(new iTextSharp.text.Phrase(item.AnnualConsumptionQuantity.ToString("N2"), smallFont));
                detailTable.AddCell(new iTextSharp.text.Phrase($"{item.PercentageOfTotalValue:N2}%", smallFont));
                detailTable.AddCell(new iTextSharp.text.Phrase(item.CurrentStock.ToString("N2"), smallFont));
                serialNo++;
            }

            document.Add(detailTable);

            document.Close();
            return ms.ToArray();
        }

    }
}
