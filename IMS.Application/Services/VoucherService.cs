using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PdfDocument = iTextSharp.text.Document;

namespace IMS.Application.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VoucherService> _logger;
        private readonly string _voucherDirectory;

        // Bengali Font configuration
        private BaseFont _bengaliFont;
        private const string BANGLA_FONT_PATH = "wwwroot/fonts/kalpurush.ttf"; // You'll need to add this font

        public VoucherService(
            IUnitOfWork unitOfWork,
            ILogger<VoucherService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            // Set voucher directory
            _voucherDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "vouchers");
            if (!Directory.Exists(_voucherDirectory))
            {
                Directory.CreateDirectory(_voucherDirectory);
            }

            // Try to load Bengali font
            try
            {
                string fontPath = Path.Combine(Directory.GetCurrentDirectory(), BANGLA_FONT_PATH);
                if (File.Exists(fontPath))
                {
                    _bengaliFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                }
                else
                {
                    _logger.LogWarning("Bengali font not found at {FontPath}. Using default font.", fontPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load Bengali font");
            }
        }

        #region Issue Voucher Methods

        public async Task<string> GenerateIssueVoucherAsync(int issueId)
        {
            try
            {
                var issue = await _unitOfWork.Issues.FindAsync(i => i.Id == issueId);
                var issueEntity = issue.FirstOrDefault();

                if (issueEntity == null)
                {
                    throw new InvalidOperationException($"Issue with ID {issueId} not found");
                }

                // Generate voucher number if not exists
                if (string.IsNullOrEmpty(issueEntity.VoucherNo))
                {
                    issueEntity.VoucherNo = await GenerateVoucherNumberAsync("ISS");
                    issueEntity.VoucherDate = DateTime.Now;
                    issueEntity.VoucherGeneratedDate = DateTime.Now;

                    _unitOfWork.Issues.Update(issueEntity);
                    await _unitOfWork.CompleteAsync();
                }

                return issueEntity.VoucherNo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating issue voucher for Issue ID: {IssueId}", issueId);
                throw;
            }
        }

        public async Task<byte[]> GenerateIssueVoucherPdfAsync(int issueId)
        {
            try
            {
                // Get issue with related data
                var issueEntity = await _unitOfWork.Issues.GetAsync(
                    i => i.Id == issueId,
                    includes: new[] { "Items.Item", "FromStore", "IssuedToBattalion", "IssuedToRange", "IssuedToZila" }
                );

                if (issueEntity == null)
                {
                    throw new InvalidOperationException($"Issue with ID {issueId} not found");
                }

                // Generate voucher number if doesn't exist
                if (string.IsNullOrEmpty(issueEntity.VoucherNo))
                {
                    await GenerateIssueVoucherAsync(issueId);
                    // Reload entity
                    issueEntity = await _unitOfWork.Issues.GetAsync(
                        i => i.Id == issueId,
                        includes: new[] { "Items.Item", "FromStore", "IssuedToBattalion", "IssuedToRange", "IssuedToZila" }
                    );
                }

                // Generate PDF
                byte[] pdfBytes = CreateIssueVoucherPdf(issueEntity);

                // Save PDF to file
                string fileName = $"Issue_Voucher_{issueEntity.VoucherNo}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = Path.Combine(_voucherDirectory, fileName);
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Update voucher document path
                issueEntity.VoucherDocumentPath = Path.Combine("vouchers", fileName);
                _unitOfWork.Issues.Update(issueEntity);
                await _unitOfWork.CompleteAsync();

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating issue voucher PDF for Issue ID: {IssueId}", issueId);
                throw;
            }
        }

        public async Task<bool> RegenerateIssueVoucherAsync(int issueId)
        {
            try
            {
                var pdfBytes = await GenerateIssueVoucherPdfAsync(issueId);
                return pdfBytes != null && pdfBytes.Length > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating issue voucher for Issue ID: {IssueId}", issueId);
                return false;
            }
        }

        #endregion

        #region Receive Voucher Methods

        public async Task<string> GenerateReceiveVoucherAsync(int receiveId)
        {
            try
            {
                var receive = await _unitOfWork.Receives.FindAsync(r => r.Id == receiveId);
                var receiveEntity = receive.FirstOrDefault();

                if (receiveEntity == null)
                {
                    throw new InvalidOperationException($"Receive with ID {receiveId} not found");
                }

                // Generate voucher number if not exists
                if (string.IsNullOrEmpty(receiveEntity.VoucherNo))
                {
                    receiveEntity.VoucherNo = await GenerateVoucherNumberAsync("RCV");
                    receiveEntity.VoucherDate = DateTime.Now;
                    receiveEntity.VoucherGeneratedDate = DateTime.Now;

                    _unitOfWork.Receives.Update(receiveEntity);
                    await _unitOfWork.CompleteAsync();
                }

                return receiveEntity.VoucherNo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating receive voucher for Receive ID: {ReceiveId}", receiveId);
                throw;
            }
        }

        public async Task<byte[]> GenerateReceiveVoucherPdfAsync(int receiveId)
        {
            try
            {
                // Get receive with related data
                var receiveEntity = await _unitOfWork.Receives.GetAsync(
                    r => r.Id == receiveId,
                    includes: new[] { "ReceiveItems.Item", "Store", "ReceivedFromBattalion", "ReceivedFromRange", "ReceivedFromZila" }
                );

                if (receiveEntity == null)
                {
                    throw new InvalidOperationException($"Receive with ID {receiveId} not found");
                }

                // Generate voucher number if doesn't exist
                if (string.IsNullOrEmpty(receiveEntity.VoucherNo))
                {
                    await GenerateReceiveVoucherAsync(receiveId);
                    // Reload entity
                    receiveEntity = await _unitOfWork.Receives.GetAsync(
                        r => r.Id == receiveId,
                        includes: new[] { "ReceiveItems.Item", "Store", "ReceivedFromBattalion", "ReceivedFromRange", "ReceivedFromZila" }
                    );
                }

                // Generate PDF
                byte[] pdfBytes = CreateReceiveVoucherPdf(receiveEntity);

                // Save PDF to file
                string fileName = $"Receive_Voucher_{receiveEntity.VoucherNo}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = Path.Combine(_voucherDirectory, fileName);
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Update voucher document path
                receiveEntity.VoucherDocumentPath = Path.Combine("vouchers", fileName);
                _unitOfWork.Receives.Update(receiveEntity);
                await _unitOfWork.CompleteAsync();

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating receive voucher PDF for Receive ID: {ReceiveId}", receiveId);
                throw;
            }
        }

        public async Task<bool> RegenerateReceiveVoucherAsync(int receiveId)
        {
            try
            {
                var pdfBytes = await GenerateReceiveVoucherPdfAsync(receiveId);
                return pdfBytes != null && pdfBytes.Length > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating receive voucher for Receive ID: {ReceiveId}", receiveId);
                return false;
            }
        }

        #endregion

        #region Voucher Retrieval Methods

        public async Task<byte[]> GetIssueVoucherPdfAsync(int issueId)
        {
            try
            {
                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);

                if (issue == null || string.IsNullOrEmpty(issue.VoucherDocumentPath))
                {
                    // Generate if not exists
                    return await GenerateIssueVoucherPdfAsync(issueId);
                }

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", issue.VoucherDocumentPath);

                if (!File.Exists(filePath))
                {
                    // Regenerate if file doesn't exist
                    return await GenerateIssueVoucherPdfAsync(issueId);
                }

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue voucher PDF for Issue ID: {IssueId}", issueId);
                throw;
            }
        }

        public async Task<byte[]> GetReceiveVoucherPdfAsync(int receiveId)
        {
            try
            {
                var receive = await _unitOfWork.Receives.GetByIdAsync(receiveId);

                if (receive == null || string.IsNullOrEmpty(receive.VoucherDocumentPath))
                {
                    // Generate if not exists
                    return await GenerateReceiveVoucherPdfAsync(receiveId);
                }

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", receive.VoucherDocumentPath);

                if (!File.Exists(filePath))
                {
                    // Regenerate if file doesn't exist
                    return await GenerateReceiveVoucherPdfAsync(receiveId);
                }

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting receive voucher PDF for Receive ID: {ReceiveId}", receiveId);
                throw;
            }
        }

        public async Task<string> GetVoucherPathAsync(string voucherNo, string voucherType)
        {
            try
            {
                if (voucherType.ToUpper() == "ISSUE" || voucherType.ToUpper() == "ISS")
                {
                    var issue = await _unitOfWork.Issues.FirstOrDefaultAsync(i => i.VoucherNo == voucherNo);
                    return issue?.VoucherDocumentPath;
                }
                else if (voucherType.ToUpper() == "RECEIVE" || voucherType.ToUpper() == "RCV")
                {
                    var receive = await _unitOfWork.Receives.FirstOrDefaultAsync(r => r.VoucherNo == voucherNo);
                    return receive?.VoucherDocumentPath;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting voucher path for Voucher No: {VoucherNo}", voucherNo);
                throw;
            }
        }

        #endregion

        #region Voucher Validation Methods

        public async Task<bool> ValidateVoucherAsync(string voucherNo)
        {
            try
            {
                var issue = await _unitOfWork.Issues.FirstOrDefaultAsync(i => i.VoucherNo == voucherNo);
                if (issue != null) return true;

                var receive = await _unitOfWork.Receives.FirstOrDefaultAsync(r => r.VoucherNo == voucherNo);
                return receive != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating voucher: {VoucherNo}", voucherNo);
                return false;
            }
        }

        public async Task<bool> VoucherExistsAsync(string voucherNo, string voucherType)
        {
            try
            {
                if (voucherType.ToUpper() == "ISSUE" || voucherType.ToUpper() == "ISS")
                {
                    var issue = await _unitOfWork.Issues.FirstOrDefaultAsync(i => i.VoucherNo == voucherNo);
                    return issue != null;
                }
                else if (voucherType.ToUpper() == "RECEIVE" || voucherType.ToUpper() == "RCV")
                {
                    var receive = await _unitOfWork.Receives.FirstOrDefaultAsync(r => r.VoucherNo == voucherNo);
                    return receive != null;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking voucher existence: {VoucherNo}", voucherNo);
                return false;
            }
        }

        #endregion

        #region PDF Generation Helper Methods

        private byte[] CreateIssueVoucherPdf(Issue issue)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Create document in A4 landscape
                PdfDocument document = new PdfDocument(PageSize.A4, 20, 20, 20, 20);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Create fonts
                Font bengaliFont = GetBengaliFont(10);
                Font bengaliFontBold = GetBengaliFont(12, Font.BOLD);
                Font bengaliFontTitle = GetBengaliFont(16, Font.BOLD);
                Font bengaliFontSmall = GetBengaliFont(8);

                // Add title
                Paragraph title = new Paragraph("প্রাপ্তি বিলি ও ব্যয়ের রশিদ", bengaliFontTitle);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Create header table (2 columns)
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 50f, 50f });

                // Left side - Provider (প্রদানকারী)
                PdfPCell leftCell = CreateHeaderCell(issue, "প্রদানকারী", bengaliFont, bengaliFontBold, true);
                headerTable.AddCell(leftCell);

                // Right side - Receiver (গ্রহনকারী)
                PdfPCell rightCell = CreateHeaderCell(issue, "গ্রহনকারী", bengaliFont, bengaliFontBold, false);
                headerTable.AddCell(rightCell);

                document.Add(headerTable);
                document.Add(new Paragraph("\n", bengaliFont));

                // Create items table
                PdfPTable itemsTable = CreateItemsTable(issue.Items, bengaliFont, bengaliFontBold);
                document.Add(itemsTable);

                document.Add(new Paragraph("\n\n", bengaliFont));

                // Create signature table
                PdfPTable signatureTable = CreateSignatureTable(issue, bengaliFont);
                document.Add(signatureTable);

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        private byte[] CreateReceiveVoucherPdf(Receive receive)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Create document in A4 landscape
                PdfDocument document = new PdfDocument(PageSize.A4, 20, 20, 20, 20);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Create fonts
                Font bengaliFont = GetBengaliFont(10);
                Font bengaliFontBold = GetBengaliFont(12, Font.BOLD);
                Font bengaliFontTitle = GetBengaliFont(16, Font.BOLD);

                // Add title
                Paragraph title = new Paragraph("প্রাপ্তি বিলি ও ব্যয়ের রশিদ", bengaliFontTitle);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Create header table (2 columns)
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 50f, 50f });

                // Left side - Provider (প্রদানকারী)
                PdfPCell leftCell = CreateReceiveHeaderCell(receive, "প্রদানকারী", bengaliFont, bengaliFontBold, true);
                headerTable.AddCell(leftCell);

                // Right side - Receiver (গ্রহনকারী)
                PdfPCell rightCell = CreateReceiveHeaderCell(receive, "গ্রহনকারী", bengaliFont, bengaliFontBold, false);
                headerTable.AddCell(rightCell);

                document.Add(headerTable);
                document.Add(new Paragraph("\n", bengaliFont));

                // Create items table for receive
                PdfPTable itemsTable = CreateReceiveItemsTable(receive.ReceiveItems, bengaliFont, bengaliFontBold);
                document.Add(itemsTable);

                document.Add(new Paragraph("\n\n", bengaliFont));

                // Create signature table
                PdfPTable signatureTable = CreateReceiveSignatureTable(receive, bengaliFont);
                document.Add(signatureTable);

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        private PdfPCell CreateHeaderCell(Issue issue, string sectionTitle, Font normalFont, Font boldFont, bool isProvider)
        {
            PdfPCell cell = new PdfPCell();
            cell.Border = Rectangle.BOX;
            cell.Padding = 10f;

            // Section title
            Paragraph title = new Paragraph($"{sectionTitle} অফিসার পূরণ করবেন:", boldFont);
            cell.AddElement(title);

            if (isProvider)
            {
                // Provider details (Issuer) - with dots like nomuna
                cell.AddElement(new Paragraph($"প্রদান- ভাউচার নং………………………………...…", normalFont));
                cell.AddElement(new Paragraph($"ইউনিট ……………………………………...........", normalFont));
                cell.AddElement(new Paragraph($"স্টেশন ……………………………………...........", normalFont));
                cell.AddElement(new Paragraph("\n", normalFont));
                cell.AddElement(new Paragraph($"…………….আদেশানুসারে ………………….. কর্তৃক", normalFont));
                cell.AddElement(new Paragraph($"…………..…….কর্তৃক পরীক্ষানুযায়ী দ্রব্যাদির নিম্নোক্ত হিসাব :", normalFont));
            }
            else
            {
                // Receiver details - with dots like nomuna
                cell.AddElement(new Paragraph($"প্রাপ্ত- ভাউচার নং………………………………...…", normalFont));
                cell.AddElement(new Paragraph($"ইউনিট ……………………………………...........", normalFont));
                cell.AddElement(new Paragraph($"স্টেশন  ……………………………………...........", normalFont));
                cell.AddElement(new Paragraph("\n", normalFont));
                cell.AddElement(new Paragraph("প্রাপ্ত\tদ্রব্যাদির হিসাব নিম্নে দেয়া হল:", normalFont));
                cell.AddElement(new Paragraph("উৎপাদিত", normalFont));
            }

            return cell;
        }

        private PdfPCell CreateReceiveHeaderCell(Receive receive, string sectionTitle, Font normalFont, Font boldFont, bool isProvider)
        {
            PdfPCell cell = new PdfPCell();
            cell.Border = Rectangle.BOX;
            cell.Padding = 10f;

            // Section title
            Paragraph title = new Paragraph($"{sectionTitle} অফিসার পূরণ করবেন:", boldFont);
            cell.AddElement(title);

            if (isProvider)
            {
                // Provider details (Sender) - with dots like nomuna
                cell.AddElement(new Paragraph($"প্রদান- ভাউচার নং………………………………...…", normalFont));
                cell.AddElement(new Paragraph($"ইউনিট ……………………………………...........", normalFont));
                cell.AddElement(new Paragraph($"স্টেশন ……………………………………...........", normalFont));
                cell.AddElement(new Paragraph("\n", normalFont));
                cell.AddElement(new Paragraph($"…………….আদেশানুসারে ………………….. কর্তৃক", normalFont));
                cell.AddElement(new Paragraph($"…………..…….কর্তৃক পরীক্ষানুযায়ী দ্রব্যাদির নিম্নোক্ত হিসাব :", normalFont));
            }
            else
            {
                // Receiver details - with dots like nomuna
                cell.AddElement(new Paragraph($"প্রাপ্ত- ভাউচার নং………………………………...…", normalFont));
                cell.AddElement(new Paragraph($"ইউনিট ……………………………………...........", normalFont));
                cell.AddElement(new Paragraph($"স্টেশন  ……………………………………...........", normalFont));
                cell.AddElement(new Paragraph("\n", normalFont));
                cell.AddElement(new Paragraph("প্রাপ্ত\tদ্রব্যাদির হিসাব নিম্নে দেয়া হল:", normalFont));
                cell.AddElement(new Paragraph("উৎপাদিত", normalFont));
            }

            return cell;
        }

        private PdfPTable CreateItemsTable(ICollection<IssueItem> items, Font normalFont, Font boldFont)
        {
            // Create table with 10 columns matching the sample
            PdfPTable table = new PdfPTable(10);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 8f, 8f, 25f, 10f, 10f, 10f, 10f, 10f, 10f });

            // Add headers
            table.AddCell(CreateTableHeaderCell("ক্রম", boldFont));
            table.AddCell(CreateTableHeaderCell("লেজার\nনং", boldFont));
            table.AddCell(CreateTableHeaderCell("পৃষ্ঠা\nনং", boldFont));
            table.AddCell(CreateTableHeaderCell("দ্রব্যাদির বিবরণ", boldFont));
            table.AddCell(CreateTableHeaderCell("মোট\nসংখ্যা", boldFont));
            table.AddCell(CreateTableHeaderCell("ব্যবহার\nযোগ্য", boldFont));
            table.AddCell(CreateTableHeaderCell("আংশিক\nব্যবহারযোগ্য", boldFont));
            table.AddCell(CreateTableHeaderCell("অকেজো", boldFont));
            table.AddCell(CreateTableHeaderCell("একক\nদর", boldFont));
            table.AddCell(CreateTableHeaderCell("মোট\nমূল্য", boldFont));

            // Add items
            int serialNo = 1;
            foreach (var item in items)
            {
                table.AddCell(CreateTableCell(serialNo.ToString(), normalFont));
                table.AddCell(CreateTableCell(item.LedgerNo ?? "", normalFont));
                table.AddCell(CreateTableCell(item.PageNo ?? "", normalFont));
                table.AddCell(CreateTableCell(item.Item?.Name ?? "", normalFont));
                table.AddCell(CreateTableCell(item.Quantity.ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((item.UsableQuantity ?? item.Quantity).ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((item.PartiallyUsableQuantity ?? 0).ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((item.UnusableQuantity ?? 0).ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((item.Item?.UnitPrice ?? 0).ToString("N2"), normalFont));
                table.AddCell(CreateTableCell(((item.Quantity) * (item.Item?.UnitPrice ?? 0)).ToString("N2"), normalFont));

                serialNo++;
            }

            // No empty rows - table adjusts to item count automatically

            return table;
        }

        private PdfPTable CreateReceiveItemsTable(ICollection<ReceiveItem> items, Font normalFont, Font boldFont)
        {
            // Create table with 10 columns matching the sample
            PdfPTable table = new PdfPTable(10);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 8f, 8f, 25f, 10f, 10f, 10f, 10f, 10f, 10f });

            // Add headers
            table.AddCell(CreateTableHeaderCell("ক্রম", boldFont));
            table.AddCell(CreateTableHeaderCell("লেজার\nনং", boldFont));
            table.AddCell(CreateTableHeaderCell("পৃষ্ঠা\nনং", boldFont));
            table.AddCell(CreateTableHeaderCell("দ্রব্যাদির বিবরণ", boldFont));
            table.AddCell(CreateTableHeaderCell("মোট\nসংখ্যা", boldFont));
            table.AddCell(CreateTableHeaderCell("ব্যবহার\nযোগ্য", boldFont));
            table.AddCell(CreateTableHeaderCell("আংশিক\nব্যবহারযোগ্য", boldFont));
            table.AddCell(CreateTableHeaderCell("অকেজো", boldFont));
            table.AddCell(CreateTableHeaderCell("একক\nদর", boldFont));
            table.AddCell(CreateTableHeaderCell("মোট\nমূল্য", boldFont));

            // Add items
            int serialNo = 1;
            foreach (var item in items)
            {
                decimal quantity = item.ReceivedQuantity ?? item.Quantity ?? 0;

                table.AddCell(CreateTableCell(serialNo.ToString(), normalFont));
                table.AddCell(CreateTableCell(item.LedgerNo ?? "", normalFont));
                table.AddCell(CreateTableCell(item.PageNo ?? "", normalFont));
                table.AddCell(CreateTableCell(item.Item?.Name ?? "", normalFont));
                table.AddCell(CreateTableCell(quantity.ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((item.UsableQuantity ?? quantity).ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((item.PartiallyUsableQuantity ?? 0).ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((item.UnusableQuantity ?? 0).ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((item.Item?.UnitPrice ?? 0).ToString("N2"), normalFont));
                table.AddCell(CreateTableCell((quantity * (item.Item?.UnitPrice ?? 0)).ToString("N2"), normalFont));

                serialNo++;
            }

            // No empty rows - table adjusts to item count automatically

            return table;
        }

        private PdfPTable CreateSignatureTable(Issue issue, Font normalFont)
        {
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 50f, 50f });

            // Left side - Provider signature
            PdfPCell leftCell = new PdfPCell();
            leftCell.Border = Rectangle.NO_BORDER;
            leftCell.AddElement(new Paragraph("বিতরণকারীর স্বাক্ষর", normalFont));
            leftCell.AddElement(new Paragraph("\n\n", normalFont));
            leftCell.AddElement(new Paragraph("পদবী …………………………………………", normalFont));
            leftCell.AddElement(new Paragraph("তারিখ…………………………………………..", normalFont));
            table.AddCell(leftCell);

            // Right side - Receiver signature
            PdfPCell rightCell = new PdfPCell();
            rightCell.Border = Rectangle.NO_BORDER;
            rightCell.AddElement(new Paragraph("গ্রহণকারীর স্বাক্ষর", normalFont));
            rightCell.AddElement(new Paragraph("\n\n", normalFont));
            rightCell.AddElement(new Paragraph("পদবী …………………………………………", normalFont));
            rightCell.AddElement(new Paragraph("তারিখ…………………………………………..", normalFont));
            table.AddCell(rightCell);

            return table;
        }

        private PdfPTable CreateReceiveSignatureTable(Receive receive, Font normalFont)
        {
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 50f, 50f });

            // Left side - Provider signature
            PdfPCell leftCell = new PdfPCell();
            leftCell.Border = Rectangle.NO_BORDER;
            leftCell.AddElement(new Paragraph("বিতরণকারীর স্বাক্ষর", normalFont));
            leftCell.AddElement(new Paragraph("\n\n", normalFont));
            leftCell.AddElement(new Paragraph("পদবী …………………………………………", normalFont));
            leftCell.AddElement(new Paragraph("তারিখ…………………………………………..", normalFont));
            table.AddCell(leftCell);

            // Right side - Receiver signature
            PdfPCell rightCell = new PdfPCell();
            rightCell.Border = Rectangle.NO_BORDER;
            rightCell.AddElement(new Paragraph("গ্রহণকারীর স্বাক্ষর", normalFont));
            rightCell.AddElement(new Paragraph("\n\n", normalFont));
            rightCell.AddElement(new Paragraph("পদবী …………………………………………", normalFont));
            rightCell.AddElement(new Paragraph("তারিখ…………………………………………..", normalFont));
            table.AddCell(rightCell);

            return table;
        }

        private PdfPCell CreateTableHeaderCell(string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5f;
            cell.Border = Rectangle.BOX;
            return cell;
        }

        private PdfPCell CreateTableCell(string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5f;
            cell.Border = Rectangle.BOX;
            return cell;
        }

        private Font GetBengaliFont(int size, int style = Font.NORMAL)
        {
            if (_bengaliFont != null)
            {
                return new Font(_bengaliFont, size, style, BaseColor.BLACK);
            }
            else
            {
                // Try to load font on-demand if not loaded during construction
                try
                {
                    string fontPath = Path.Combine(Directory.GetCurrentDirectory(), BANGLA_FONT_PATH);
                    if (File.Exists(fontPath))
                    {
                        _bengaliFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        return new Font(_bengaliFont, size, style, BaseColor.BLACK);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load Bengali font on-demand");
                }

                // Final fallback - use Arial Unicode or Helvetica
                _logger.LogWarning("Using fallback font for Bengali text");
                return FontFactory.GetFont(FontFactory.HELVETICA, size, style, BaseColor.BLACK);
            }
        }

        #endregion

        #region Helper Methods

        private async Task<string> GenerateVoucherNumberAsync(string prefix)
        {
            try
            {
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string pattern = $"{prefix}-{yearMonth}-";

                // Get last voucher number
                int maxNumber = 0;

                if (prefix == "ISS")
                {
                    var issues = await _unitOfWork.Issues.FindAsync(i => i.VoucherNo.StartsWith(pattern));
                    if (issues.Any())
                    {
                        maxNumber = issues
                            .Select(i => i.VoucherNo)
                            .Where(vn => !string.IsNullOrEmpty(vn))
                            .Select(vn => int.TryParse(vn.Substring(pattern.Length), out int num) ? num : 0)
                            .DefaultIfEmpty(0)
                            .Max();
                    }
                }
                else if (prefix == "RCV")
                {
                    var receives = await _unitOfWork.Receives.FindAsync(r => r.VoucherNo.StartsWith(pattern));
                    if (receives.Any())
                    {
                        maxNumber = receives
                            .Select(r => r.VoucherNo)
                            .Where(vn => !string.IsNullOrEmpty(vn))
                            .Select(vn => int.TryParse(vn.Substring(pattern.Length), out int num) ? num : 0)
                            .DefaultIfEmpty(0)
                            .Max();
                    }
                }

                int nextNumber = maxNumber + 1;
                return $"{pattern}{nextNumber:D4}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating voucher number with prefix: {Prefix}", prefix);
                // Fallback to timestamp-based number
                return $"{prefix}-{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        #endregion
    }
}
