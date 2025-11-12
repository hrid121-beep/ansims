using ClosedXML.Excel;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace IMS.Application.Services
{
    /// <summary>
    /// Central Store Register Report Implementation (কেন্দ্রীয় ভান্ডার মজুদ তালিকা)
    /// Partial class extension for ReportService
    /// </summary>
    public partial class ReportService
    {
        public async Task<CentralStoreRegisterDto> GetCentralStoreRegisterAsync(
            int? storeId = null,
            int? categoryId = null,
            string sortBy = "Ledger",
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var report = new CentralStoreRegisterDto
            {
                ReportDate = DateTime.Now
            };

            // Get Central Store if specified, otherwise get all items
            if (storeId.HasValue)
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId.Value);
                if (store != null)
                {
                    report.StoreId = store.Id;
                    report.StoreName = store.Name;
                    report.StoreNameBn = store.NameBn ?? store.Name;
                }
            }

            // Get Category if specified
            if (categoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId.Value);
                if (category != null)
                {
                    report.CategoryName = category.Name;
                    report.CategoryNameBn = category.NameBn ?? category.Name;
                    report.ReportTitle = $"কেন্দ্রীয় আনসার ভান্ডারের মজুদ উপকরণের তালিকা: {category.NameBn ?? category.Name}";
                }
            }

            // Query to get items with stock information
            var itemsQuery = _unitOfWork.Items.Query()
                .Where(i => i.IsActive)
                .Include(i => i.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(i => i.StoreItems)
                    .ThenInclude(si => si.Store)
                .Include(i => i.PurchaseItems)
                    .ThenInclude(pi => pi.Purchase)
                        .ThenInclude(p => p.Vendor)
                .AsQueryable();

            // Filter by category if specified
            if (categoryId.HasValue)
            {
                itemsQuery = itemsQuery.Where(i => i.CategoryId == categoryId.Value);
            }

            var items = await itemsQuery.ToListAsync();

            int serialNo = 1;
            foreach (var item in items)
            {
                // Get total stock in central store
                var storeItem = item.StoreItems.FirstOrDefault(si =>
                    si.StoreId == (storeId ?? si.StoreId) && si.IsActive);

                if (storeItem == null) continue;

                // Get first received date
                var firstPurchase = item.PurchaseItems
                    .OrderBy(pi => pi.CreatedAt)
                    .FirstOrDefault();

                var receivedDate = item.FirstReceivedDate ?? item.CatalogueEntryDate ?? firstPurchase?.CreatedAt;

                // Filter by date range if specified
                if (startDate.HasValue && receivedDate.HasValue && receivedDate.Value.Date < startDate.Value.Date)
                    continue;
                if (endDate.HasValue && receivedDate.HasValue && receivedDate.Value.Date > endDate.Value.Date)
                    continue;

                var totalQuantity = storeItem.CurrentStock;

                // Calculate allocated quantity (issued to other units)
                var allocatedQuantity = await _unitOfWork.IssueItems.Query()
                    .Where(ii => ii.ItemId == item.Id && ii.IsActive)
                    .SumAsync(ii => (decimal?)ii.IssuedQuantity) ?? 0;

                var remainingQuantity = totalQuantity - allocatedQuantity;

                // Get supplier name
                var supplierName = firstPurchase?.Purchase?.Vendor?.Name ?? "";

                var reportItem = new CentralStoreRegisterItemDto
                {
                    SerialNo = serialNo++,
                    LedgerNo = item.CatalogueLedgerNo ?? item.SubCategory?.Category?.Code ?? "",
                    PageNo = item.CataloguePageNo ?? "",
                    ItemId = item.Id,
                    ItemCode = item.ItemCode ?? item.Code,
                    ItemName = item.Name,
                    ItemNameBn = item.NameBn ?? item.Name,
                    Unit = item.Unit,
                    CategoryName = item.SubCategory?.Category?.Name ?? "",
                    TotalQuantity = totalQuantity,
                    AllocatedQuantity = allocatedQuantity,
                    RemainingQuantity = remainingQuantity,
                    ReceivedDate = receivedDate,
                    SupplierName = supplierName,
                    UnitPrice = item.UnitPrice ?? 0,
                    TotalValue = totalQuantity * (item.UnitPrice ?? 0)
                };

                report.Items.Add(reportItem);
            }

            // Apply sorting
            report.Items = sortBy switch
            {
                "Item" => report.Items.OrderBy(i => i.ItemNameBn).ToList(),
                "Category" => report.Items.OrderBy(i => i.CategoryName).ThenBy(i => i.PageNo).ToList(),
                "Quantity" => report.Items.OrderByDescending(i => i.TotalQuantity).ToList(),
                _ => report.Items.OrderBy(i => i.LedgerNo).ThenBy(i => i.PageNo).ToList() // Default: Ledger
            };

            // Calculate summary
            report.TotalItems = report.Items.Count;
            report.TotalQuantity = report.Items.Sum(i => i.TotalQuantity);
            report.TotalValue = report.Items.Sum(i => i.TotalValue);

            return report;
        }

        public async Task<byte[]> GenerateCentralStoreRegisterPdfAsync(
            int? storeId = null,
            int? categoryId = null,
            string sortBy = "Ledger",
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var report = await GetCentralStoreRegisterAsync(storeId, categoryId, sortBy, startDate, endDate);

            // Use QuestPDF for perfect Bengali rendering (same as VoucherService)
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Legal); // Legal Portrait (8.5" x 14")
                    page.Margin(20);
                    page.PageColor(Colors.White);

                    // Set Kalpurush as default font for the entire document
                    page.DefaultTextStyle(x => x.FontFamily("Kalpurush").FontSize(8));

                    page.Content().Column(column =>
                    {
                        // Title
                        column.Item().AlignCenter().Text(report.ReportTitle)
                            .FontSize(14).Bold().FontFamily("Kalpurush");

                        // Subtitle (Category name if specified)
                        if (!string.IsNullOrEmpty(report.CategoryNameBn))
                        {
                            column.Item().AlignCenter().Text(report.CategoryNameBn)
                                .FontSize(12).Bold().FontFamily("Kalpurush");
                        }

                        // Date
                        column.Item().PaddingTop(5).AlignCenter()
                            .Text($"তারিখ: {report.ReportDate:dd/MM/yyyy} খ্রি:")
                            .FontSize(10).FontFamily("Kalpurush");

                        column.Item().PaddingVertical(10);

                        // Main table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);   // ক্রম
                                columns.ConstantColumn(40);   // লেজার নং
                                columns.ConstantColumn(40);   // পাতা নং
                                columns.RelativeColumn(3);    // উপকরণের নাম
                                columns.ConstantColumn(50);   // হিসাবের একক
                                columns.ConstantColumn(60);   // মোট পরিমাণ
                                columns.ConstantColumn(60);   // বরাদ্দকৃত
                                columns.ConstantColumn(60);   // অবশিষ্ট
                                columns.ConstantColumn(70);   // গ্রহণের তারিখ
                                columns.RelativeColumn(2);    // সরবরাহকারী
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("ক্রম").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("লেজার নং").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("পাতা নং").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("উপকরণের নাম").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("হিসাবের একক").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("কেন্দ্রীয় ভান্ডারে মোট পরিমাণ").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("বিভিন্ন ইউনিটের জন্য বরাদ্দকৃত উপকরণ").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("অবশিষ্ট উপকরণ").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("কেন্দ্রীয় ভান্ডারে গ্রহণের তারিখ").FontSize(7).Bold().FontFamily("Kalpurush");

                                header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Black)
                                    .Padding(5).AlignCenter().AlignMiddle()
                                    .Text("সরবরাহকারী প্রতিষ্ঠান").FontSize(7).Bold().FontFamily("Kalpurush");
                            });

                            // Data rows
                            foreach (var item in report.Items)
                            {
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignCenter().AlignMiddle()
                                    .Text(item.SerialNo.ToString()).FontSize(7);

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignCenter().AlignMiddle()
                                    .Text(item.LedgerNo ?? "").FontSize(7);

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignCenter().AlignMiddle()
                                    .Text(item.PageNo ?? "").FontSize(7);

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignLeft().AlignMiddle()
                                    .Text(item.ItemNameBn ?? "").FontSize(7).FontFamily("Kalpurush");

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignCenter().AlignMiddle()
                                    .Text(item.Unit ?? "").FontSize(7).FontFamily("Kalpurush");

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignRight().AlignMiddle()
                                    .Text(item.TotalQuantity.ToString("N2")).FontSize(7);

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignRight().AlignMiddle()
                                    .Text(item.AllocatedQuantity.ToString("N2")).FontSize(7);

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignRight().AlignMiddle()
                                    .Text(item.RemainingQuantity.ToString("N2")).FontSize(7);

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignCenter().AlignMiddle()
                                    .Text(item.ReceivedDate?.ToString("dd/MM/yyyy") ?? "").FontSize(7);

                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Padding(4)
                                    .AlignLeft().AlignMiddle()
                                    .Text(item.SupplierName ?? "").FontSize(7).FontFamily("Kalpurush");
                            }
                        });

                        column.Item().PaddingTop(15);

                        // Summary footer
                        column.Item().AlignCenter()
                            .Text($"মোট উপকরণ: {report.TotalItems} | মোট পরিমাণ: {report.TotalQuantity:N2} | মোট মূল্য: ৳{report.TotalValue:N2}")
                            .FontSize(10).Bold().FontFamily("Kalpurush");
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateCentralStoreRegisterExcelAsync(
            int? storeId = null,
            int? categoryId = null,
            string sortBy = "Ledger",
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var report = await GetCentralStoreRegisterAsync(storeId, categoryId, sortBy, startDate, endDate);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Central Store Register");

            // Title
            worksheet.Cell(1, 1).Value = report.ReportTitle;
            worksheet.Range(1, 1, 1, 10).Merge().Style
                .Font.SetBold(true)
                .Font.SetFontSize(14)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Date
            worksheet.Cell(2, 1).Value = $"তারিখ: {report.ReportDate:dd/MM/yyyy}";
            worksheet.Range(2, 1, 2, 10).Merge().Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Headers
            int headerRow = 4;
            string[] headers = {
                "ক্রম", "লেজার নং", "পাতা নং", "উপকরণের নাম", "একক",
                "মোট পরিমাণ", "বরাদ্দকৃত", "অবশিষ্ট", "গ্রহণের তারিখ", "সরবরাহকারী"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(headerRow, i + 1).Value = headers[i];
            }

            worksheet.Range(headerRow, 1, headerRow, headers.Length).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightGray)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Data
            int currentRow = headerRow + 1;
            foreach (var item in report.Items)
            {
                worksheet.Cell(currentRow, 1).Value = item.SerialNo;
                worksheet.Cell(currentRow, 2).Value = item.LedgerNo;
                worksheet.Cell(currentRow, 3).Value = item.PageNo;
                worksheet.Cell(currentRow, 4).Value = item.ItemNameBn;
                worksheet.Cell(currentRow, 5).Value = item.Unit;
                worksheet.Cell(currentRow, 6).Value = item.TotalQuantity;
                worksheet.Cell(currentRow, 7).Value = item.AllocatedQuantity;
                worksheet.Cell(currentRow, 8).Value = item.RemainingQuantity;
                worksheet.Cell(currentRow, 9).Value = item.ReceivedDate?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cell(currentRow, 10).Value = item.SupplierName;
                currentRow++;
            }

            // Summary
            currentRow += 2;
            worksheet.Cell(currentRow, 1).Value = $"মোট উপকরণ: {report.TotalItems}";
            worksheet.Cell(currentRow, 5).Value = $"মোট পরিমাণ: {report.TotalQuantity:N2}";
            worksheet.Cell(currentRow, 8).Value = $"মোট মূল্য: ৳{report.TotalValue:N2}";

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

    }
}
