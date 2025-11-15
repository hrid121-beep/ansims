using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorService vendorService, ILogger<VendorController> logger)
        {
            _vendorService = vendorService;
            _logger = logger;
        }

        [HasPermission(Permission.ViewVendor)]
        public async Task<IActionResult> Index()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return View(vendors);
        }

        [HasPermission(Permission.CreateVendor)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateVendor)]
        public async Task<IActionResult> Create(VendorDto vendorDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _vendorService.CreateVendorAsync(vendorDto);
                    TempData["Success"] = "Vendor created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating vendor: " + ex.Message);
                }
            }
            return View(vendorDto);
        }

        [HasPermission(Permission.UpdateVendor)]
        public async Task<IActionResult> Edit(int id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateVendor)]
        public async Task<IActionResult> Edit(int id, VendorDto vendorDto)
        {
            if (id != vendorDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _vendorService.UpdateVendorAsync(vendorDto);
                    TempData["Success"] = "Vendor updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating vendor: " + ex.Message);
                }
            }
            return View(vendorDto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteVendor)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _vendorService.DeleteVendorAsync(id);
                TempData["Success"] = "Vendor deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting vendor: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // ==================== EXPORT OPERATIONS ====================

        [HttpGet]
        [HasPermission(Permission.ViewVendor)]
        public async Task<IActionResult> ExportToCsv(string status = null)
        {
            try
            {
                var vendors = await _vendorService.GetAllVendorsAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        vendors = vendors.Where(v => v.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        vendors = vendors.Where(v => !v.IsActive);
                }

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Name,Contact Person,Phone,Email,Address,Status");

                foreach (var vendor in vendors)
                {
                    csv.AppendLine($"\"{EscapeCsv(vendor.Name)}\"," +
                        $"\"{EscapeCsv(vendor.ContactPerson)}\"," +
                        $"\"{EscapeCsv(vendor.Phone)}\"," +
                        $"\"{EscapeCsv(vendor.Email)}\"," +
                        $"\"{EscapeCsv(vendor.Address)}\"," +
                        $"\"{(vendor.IsActive ? "Active" : "Inactive")}\"");
                }

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"Vendors_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting vendors to CSV");
                TempData["Error"] = "Error exporting data to CSV.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewVendor)]
        public async Task<IActionResult> ExportToPdf(string status = null)
        {
            try
            {
                var vendors = await _vendorService.GetAllVendorsAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        vendors = vendors.Where(v => v.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        vendors = vendors.Where(v => !v.IsActive);
                }

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18);
                    var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

                    var titleParagraph = new iTextSharp.text.Paragraph("ANSAR & VDP - Vendors Report", titleFont);
                    titleParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    titleParagraph.SpacingAfter = 10f;
                    document.Add(titleParagraph);

                    var infoParagraph = new iTextSharp.text.Paragraph($"Report Generated: {DateTime.Now:dd-MMM-yyyy HH:mm} | Total: {vendors.Count()}", normalFont);
                    infoParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    infoParagraph.SpacingAfter = 15f;
                    document.Add(infoParagraph);

                    var mainTable = new iTextSharp.text.pdf.PdfPTable(5);
                    mainTable.WidthPercentage = 100;
                    mainTable.SetWidths(new float[] { 25f, 20f, 18f, 22f, 15f });

                    var headerTexts = new[] { "Name", "Contact Person", "Phone", "Email", "Status" };
                    foreach (var headerText in headerTexts)
                    {
                        var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(headerText, headerFont));
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(220, 220, 220);
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.Padding = 5f;
                        mainTable.AddCell(cell);
                    }

                    foreach (var vendor in vendors)
                    {
                        mainTable.AddCell(new iTextSharp.text.Phrase(vendor.Name ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(vendor.ContactPerson ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(vendor.Phone ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(vendor.Email ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(vendor.IsActive ? "Active" : "Inactive", normalFont));
                    }

                    document.Add(mainTable);

                    var footerParagraph = new iTextSharp.text.Paragraph($"\nGenerated by: IMS System | Date: {DateTime.Now:dd-MMM-yyyy HH:mm}",
                        iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8));
                    footerParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    footerParagraph.SpacingBefore = 20f;
                    document.Add(footerParagraph);

                    document.Close();
                    return File(memoryStream.ToArray(), "application/pdf", $"Vendors_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting vendors to PDF");
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
    }
}