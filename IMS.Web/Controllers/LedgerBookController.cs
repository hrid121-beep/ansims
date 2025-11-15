using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class LedgerBookController : Controller
    {
        private readonly ILedgerBookService _ledgerBookService;
        private readonly IStoreService _storeService;
        private readonly ILogger<LedgerBookController> _logger;

        public LedgerBookController(
            ILedgerBookService ledgerBookService,
            IStoreService storeService,
            ILogger<LedgerBookController> logger)
        {
            _ledgerBookService = ledgerBookService;
            _storeService = storeService;
            _logger = logger;
        }

        // GET: LedgerBook/Index
        [HttpGet]
        public async Task<IActionResult> Index(int? storeId, string bookType)
        {
            try
            {
                var ledgerBooks = await _ledgerBookService.GetActiveLedgerBooksByStoreAndTypeAsync(storeId, bookType);

                ViewBag.Stores = await _storeService.GetAllStoresAsync();
                ViewBag.SelectedStoreId = storeId;
                ViewBag.SelectedBookType = bookType;

                return View(ledgerBooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ledger books");
                TempData["Error"] = "Error loading ledger books.";
                return View();
            }
        }

        // GET: LedgerBook/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Stores = await _storeService.GetAllStoresAsync();
            return View(new LedgerBookCreateDto { StartDate = DateTime.Now, TotalPages = 500 });
        }

        // POST: LedgerBook/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LedgerBookCreateDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ledgerBookId = await _ledgerBookService.CreateLedgerBookAsync(model, User.Identity.Name);
                    TempData["Success"] = $"Ledger book '{model.LedgerNo}' created successfully!";
                    return RedirectToAction(nameof(Details), new { id = ledgerBookId });
                }

                ViewBag.Stores = await _storeService.GetAllStoresAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ledger book");
                TempData["Error"] = ex.Message;
                ViewBag.Stores = await _storeService.GetAllStoresAsync();
                return View(model);
            }
        }

        // GET: LedgerBook/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var ledgerBook = await _ledgerBookService.GetLedgerBookByIdAsync(id);
                if (ledgerBook == null)
                {
                    TempData["Error"] = "Ledger book not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Stores = await _storeService.GetAllStoresAsync();
                return View(ledgerBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ledger book for edit");
                TempData["Error"] = "Error loading ledger book.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: LedgerBook/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LedgerBookDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _ledgerBookService.UpdateLedgerBookAsync(id, model, User.Identity.Name);
                    TempData["Success"] = "Ledger book updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }

                ViewBag.Stores = await _storeService.GetAllStoresAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ledger book");
                TempData["Error"] = ex.Message;
                ViewBag.Stores = await _storeService.GetAllStoresAsync();
                return View(model);
            }
        }

        // GET: LedgerBook/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var ledgerBook = await _ledgerBookService.GetLedgerBookByIdAsync(id);
                if (ledgerBook == null)
                {
                    TempData["Error"] = "Ledger book not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(ledgerBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ledger book details");
                TempData["Error"] = "Error loading ledger book details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: LedgerBook/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            try
            {
                await _ledgerBookService.CloseLedgerBookAsync(id, User.Identity.Name);
                TempData["Success"] = "Ledger book closed successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing ledger book");
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: LedgerBook/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _ledgerBookService.DeleteLedgerBookAsync(id);
                TempData["Success"] = "Ledger book deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ledger book");
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        #region Export Methods

        // GET: LedgerBook/ExportToCsv
        [HttpGet]
        public async Task<IActionResult> ExportToCsv(int? storeId = null, string bookType = null)
        {
            try
            {
                var ledgerBooks = await _ledgerBookService.GetActiveLedgerBooksByStoreAndTypeAsync(storeId, bookType);

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Ledger No,Book Name,Type,Store,Total Pages,Current Page,Pages Used,Pages Remaining,Location,Status,Start Date,End Date");

                foreach (var book in ledgerBooks)
                {
                    csv.AppendLine($"\"{EscapeCsv(book.LedgerNo)}\"," +
                        $"\"{EscapeCsv(book.BookName)}\"," +
                        $"\"{EscapeCsv(book.BookType)}\"," +
                        $"\"{EscapeCsv(book.StoreName)}\"," +
                        $"{book.TotalPages}," +
                        $"{book.CurrentPageNo}," +
                        $"{book.PagesUsed}," +
                        $"{book.PagesRemaining}," +
                        $"\"{EscapeCsv(book.Location)}\"," +
                        $"\"{EscapeCsv(book.Status)}\"," +
                        $"\"{book.StartDate:dd-MMM-yyyy}\"," +
                        $"\"{(book.EndDate.HasValue ? book.EndDate.Value.ToString("dd-MMM-yyyy") : "")}\"");
                }

                var fileName = $"LedgerBooks_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting ledger books to CSV");
                TempData["Error"] = "Error exporting data to CSV.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: LedgerBook/ExportToPdf
        [HttpGet]
        public async Task<IActionResult> ExportToPdf(int? storeId = null, string bookType = null)
        {
            try
            {
                var ledgerBooks = await _ledgerBookService.GetActiveLedgerBooksByStoreAndTypeAsync(storeId, bookType);

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);

                    document.Open();

                    // Add header
                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
                    var title = new iTextSharp.text.Paragraph("Bangladesh Ansar & VDP\nLedger Books Report\n\n", titleFont);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    document.Add(title);

                    // Add filter info
                    var filterFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);
                    var filterText = $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}";
                    if (storeId.HasValue)
                    {
                        var store = ledgerBooks.FirstOrDefault()?.StoreName;
                        if (!string.IsNullOrEmpty(store))
                            filterText += $" | Store: {store}";
                    }
                    if (!string.IsNullOrEmpty(bookType))
                        filterText += $" | Type: {bookType}";

                    var filterPara = new iTextSharp.text.Paragraph(filterText + "\n\n", filterFont);
                    filterPara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    document.Add(filterPara);

                    // Create table
                    var table = new iTextSharp.text.pdf.PdfPTable(11) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 10f, 15f, 10f, 12f, 8f, 8f, 8f, 8f, 12f, 10f, 10f });

                    // Table header
                    var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 8);
                    var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 7);

                    string[] headers = { "Ledger No", "Book Name", "Type", "Store", "Total Pages", "Current", "Used", "Remaining", "Location", "Status", "Start Date" };
                    foreach (var header in headers)
                    {
                        var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                        cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GREY;
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }

                    // Table rows
                    foreach (var book in ledgerBooks)
                    {
                        table.AddCell(new iTextSharp.text.Phrase(book.LedgerNo ?? "", cellFont));
                        table.AddCell(new iTextSharp.text.Phrase(book.BookName ?? "", cellFont));
                        table.AddCell(new iTextSharp.text.Phrase(book.BookType ?? "", cellFont));
                        table.AddCell(new iTextSharp.text.Phrase(book.StoreName ?? "", cellFont));

                        var centerCell1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(book.TotalPages.ToString(), cellFont));
                        centerCell1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        table.AddCell(centerCell1);

                        var centerCell2 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(book.CurrentPageNo.ToString(), cellFont));
                        centerCell2.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        table.AddCell(centerCell2);

                        var centerCell3 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(book.PagesUsed.ToString(), cellFont));
                        centerCell3.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        table.AddCell(centerCell3);

                        var centerCell4 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(book.PagesRemaining.ToString(), cellFont));
                        centerCell4.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        table.AddCell(centerCell4);

                        table.AddCell(new iTextSharp.text.Phrase(book.Location ?? "", cellFont));
                        table.AddCell(new iTextSharp.text.Phrase(book.Status ?? "", cellFont));
                        table.AddCell(new iTextSharp.text.Phrase(book.StartDate.ToString("dd-MMM-yy"), cellFont));
                    }

                    document.Add(table);

                    // Add footer
                    document.Add(new iTextSharp.text.Paragraph("\n"));
                    var footerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_OBLIQUE, 8);
                    var footer = new iTextSharp.text.Paragraph($"Total Ledger Books: {ledgerBooks.Count()}", footerFont);
                    footer.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    document.Add(footer);

                    document.Close();

                    var fileName = $"LedgerBooks_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    return File(memoryStream.ToArray(), "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting ledger books to PDF");
                TempData["Error"] = "Error exporting data to PDF.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains("\"")) value = value.Replace("\"", "\"\"");
            return value;
        }

        #endregion

        #region API Endpoints

        // API: GET /LedgerBook/GetActiveLedgerBooks?storeId=1&bookType=Issue
        [HttpGet]
        public async Task<IActionResult> GetActiveLedgerBooks(int? storeId, string bookType)
        {
            try
            {
                var ledgerBooks = await _ledgerBookService.GetActiveLedgerBooksByStoreAndTypeAsync(storeId, bookType);
                return Json(ledgerBooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active ledger books");
                return Json(new { error = "Error loading ledger books" });
            }
        }

        // API: GET /LedgerBook/GetNextPage/5
        [HttpGet]
        public async Task<IActionResult> GetNextPage(int id)
        {
            try
            {
                var nextPage = await _ledgerBookService.GetNextAvailablePageAsync(id);
                var ledgerBook = await _ledgerBookService.GetLedgerBookByIdAsync(id);

                return Json(new
                {
                    success = true,
                    nextPage = nextPage,
                    ledgerNo = ledgerBook?.LedgerNo,
                    pagesRemaining = ledgerBook?.PagesRemaining ?? 0,
                    isFull = (ledgerBook?.PagesRemaining ?? 0) <= 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next page");
                return Json(new { success = false, error = ex.Message });
            }
        }

        // API: GET /LedgerBook/GetNextPageByLedgerNo?ledgerNo=ISS-2025
        [HttpGet]
        public async Task<IActionResult> GetNextPageByLedgerNo(string ledgerNo)
        {
            try
            {
                var nextPage = await _ledgerBookService.GetNextAvailablePageByLedgerNoAsync(ledgerNo);
                var ledgerBook = await _ledgerBookService.GetLedgerBookByLedgerNoAsync(ledgerNo);

                return Json(new
                {
                    success = true,
                    nextPage = nextPage,
                    ledgerNo = ledgerBook?.LedgerNo,
                    pagesRemaining = ledgerBook?.PagesRemaining ?? 0,
                    isFull = (ledgerBook?.PagesRemaining ?? 0) <= 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next page by ledger no");
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion
    }
}
