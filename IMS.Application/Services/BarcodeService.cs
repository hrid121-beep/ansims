using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QRCoder;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Text.Json;
using ZXing;
using ZXing.Common;
using Barcode = IMS.Domain.Entities.Barcode;
using BarcodeLabel = IMS.Application.DTOs.BarcodeLabel;
using Bitmap = System.Drawing.Bitmap;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using Document = iTextSharp.text.Document;
using Element = iTextSharp.text.Element;
using FontFactory = iTextSharp.text.FontFactory;
using Graphics = System.Drawing.Graphics;
using PageSize = iTextSharp.text.PageSize;
using Paragraph = iTextSharp.text.Paragraph;
using PdfPCell = iTextSharp.text.pdf.PdfPCell;
using PdfPTable = iTextSharp.text.pdf.PdfPTable;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;
using Pens = System.Drawing.Pens;
using Phrase = iTextSharp.text.Phrase;
using Rectangle = iTextSharp.text.Rectangle;

namespace IMS.Application.Services
{
    public class BarcodeService : IBarcodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IActivityLogService _activityLogService;
        private readonly ILogger<BarcodeService> _logger;
        private readonly BarcodeLib.Barcode _code128Generator;
        private readonly string _barcodePrefix;

        public BarcodeService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            IActivityLogService activityLogService,
            ILogger<BarcodeService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _code128Generator = new BarcodeLib.Barcode();
            _barcodePrefix = "AVDP"; // From config
        }
        #region Core CRUD Operations

        public async Task<PagedResult<BarcodeDto>> GetAllBarcodesAsync(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var totalCount = await _unitOfWork.Barcodes.CountAsync(b => b.IsActive);
                var barcodes = await _unitOfWork.Barcodes.GetPagedAsync(pageNumber, pageSize, b => b.IsActive);

                var barcodeDtos = new List<BarcodeDto>();

                // Batch load related data to avoid N+1 queries
                var itemIds = barcodes.Select(b => b.ItemId).Distinct().ToList();
                var items = await _unitOfWork.Items.FindAsync(i => itemIds.Contains(i.Id));
                var itemDict = items.ToDictionary(i => i.Id);

                var subCategoryIds = items.Select(i => i.SubCategoryId).Distinct().ToList();
                var subCategories = await _unitOfWork.SubCategories.FindAsync(sc => subCategoryIds.Contains(sc.Id));
                var subCategoryDict = subCategories.ToDictionary(sc => sc.Id);

                var categoryIds = subCategories.Select(sc => sc.CategoryId).Distinct().ToList();
                var categories = await _unitOfWork.Categories.FindAsync(c => categoryIds.Contains(c.Id));
                var categoryDict = categories.ToDictionary(c => c.Id);

                // Load stores
                var storeIds = barcodes.Where(b => b.StoreId.HasValue).Select(b => b.StoreId.Value).Distinct().ToList();
                var stores = await _unitOfWork.Stores.FindAsync(s => storeIds.Contains(s.Id));
                var storeDict = stores.ToDictionary(s => s.Id);

                foreach (var barcode in barcodes)
                {
                    var dto = MapToDto(barcode, itemDict, subCategoryDict, categoryDict);

                    // Add store name if available
                    if (barcode.StoreId.HasValue && storeDict.ContainsKey(barcode.StoreId.Value))
                    {
                        dto.StoreName = storeDict[barcode.StoreId.Value].Name;
                    }

                    barcodeDtos.Add(dto);
                }

                return new PagedResult<BarcodeDto>
                {
                    Items = barcodeDtos.OrderByDescending(b => b.GeneratedDate),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged barcodes");
                throw;
            }
        }
        public async Task<IEnumerable<BarcodeDto>> GetAllBarcodesAsync()
        {
            try
            {
                var barcodes = await _unitOfWork.Barcodes.FindAsync(b => b.IsActive);
                var barcodeDtos = new List<BarcodeDto>();

                foreach (var barcode in barcodes)
                {
                    var dto = await GetBarcodeByIdAsync(barcode.Id);
                    if (dto != null)
                        barcodeDtos.Add(dto);
                }

                return barcodeDtos.OrderByDescending(b => b.GeneratedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all barcodes");
                throw;
            }
        }
        public async Task<BarcodeDto> GetBarcodeByIdAsync(int id)
        {
            try
            {
                var barcode = await _unitOfWork.Barcodes.GetByIdAsync(id);
                if (barcode == null || !barcode.IsActive)
                    return null;

                var item = await _unitOfWork.Items.GetByIdAsync(barcode.ItemId ?? 0);
                if (item == null)
                    return null;

                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                var category = subCategory != null ?
                    await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;

                // Get store information if available
                Store store = null;
                if (barcode.StoreId.HasValue)
                {
                    store = await _unitOfWork.Stores.GetByIdAsync(barcode.StoreId.Value);
                }

                return new BarcodeDto
                {
                    Id = barcode.Id,
                    BarcodeNumber = barcode.BarcodeNumber,
                    ItemId = barcode.ItemId ?? 0,
                    ItemName = item.Name,
                    ItemCode = item.ItemCode,
                    CategoryName = category?.Name ?? "Unknown",
                    SubCategoryName = subCategory?.Name ?? "Unknown",
                    SerialNumber = barcode.SerialNumber,
                    BatchNumber = barcode.BatchNumber,
                    GeneratedDate = barcode.GeneratedDate,
                    GeneratedBy = barcode.GeneratedBy,
                    IsActive = barcode.IsActive,
                    CreatedAt = barcode.CreatedAt,
                    CreatedBy = barcode.CreatedBy,
                    UpdatedAt = barcode.UpdatedAt,
                    UpdatedBy = barcode.UpdatedBy,
                    // Store and Location
                    StoreId = barcode.StoreId,
                    StoreName = store?.Name,
                    Location = barcode.Location,
                    Notes = barcode.Notes,
                    // Printing Information
                    PrintedBy = barcode.PrintedBy,
                    PrintedDate = barcode.PrintedDate,
                    PrintCount = barcode.PrintCount,
                    // Scanning Information
                    LastScannedLocation = barcode.LastScannedLocation,
                    LastScannedDate = barcode.LastScannedDate,
                    LastScannedBy = barcode.LastScannedBy,
                    ScanCount = barcode.ScanCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcode by id: {Id}", id);
                throw;
            }
        }
        public async Task<BarcodeDto> GetBarcodeByNumberAsync(string barcodeNumber)
        {
            if (string.IsNullOrWhiteSpace(barcodeNumber))
                return null;

            try
            {
                var barcode = await _unitOfWork.Barcodes.FirstOrDefaultAsync(
                    b => b.BarcodeNumber == barcodeNumber && b.IsActive);

                if (barcode == null)
                    return null;

                return await GetBarcodeByIdAsync(barcode.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcode by number: {BarcodeNumber}", barcodeNumber);
                throw;
            }
        }
        public async Task<BarcodeDto> GetBarcodeBySerialNumberAsync(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                return null;

            try
            {
                var barcode = await _unitOfWork.Barcodes.FirstOrDefaultAsync(
                    b => b.SerialNumber == serialNumber && b.IsActive);

                if (barcode == null)
                    return null;

                return await GetBarcodeByIdAsync(barcode.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcode by serial number: {SerialNumber}", serialNumber);
                throw;
            }
        }
        public async Task<BarcodeDto> CreateBarcodeAsync(BarcodeDto barcodeDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate item exists
                var item = await _unitOfWork.Items.GetByIdAsync(barcodeDto.ItemId);
                if (item == null || !item.IsActive)
                {
                    throw new InvalidOperationException("Invalid item ID or item is inactive");
                }

                // Validate store if provided
                if (barcodeDto.StoreId.HasValue)
                {
                    var store = await _unitOfWork.Stores.GetByIdAsync(barcodeDto.StoreId.Value);
                    if (store == null || !store.IsActive)
                    {
                        throw new InvalidOperationException("Invalid store ID or store is inactive");
                    }
                }

                // Check for duplicate barcode
                if (await BarcodeExistsAsync(barcodeDto.BarcodeNumber))
                {
                    throw new InvalidOperationException($"Barcode number {barcodeDto.BarcodeNumber} already exists");
                }

                // Generate barcode number if not provided
                if (string.IsNullOrEmpty(barcodeDto.BarcodeNumber))
                {
                    barcodeDto.BarcodeNumber = await GenerateUniqueBarcodeNumberAsync(barcodeDto.ItemId);
                }

                // Generate serial number if not provided
                if (string.IsNullOrEmpty(barcodeDto.SerialNumber))
                {
                    barcodeDto.SerialNumber = await GenerateSerialNumberAsync();
                }

                var barcode = new Barcode
                {
                    BarcodeNumber = barcodeDto.BarcodeNumber,
                    ItemId = barcodeDto.ItemId,
                    SerialNumber = barcodeDto.SerialNumber,
                    BatchNumber = barcodeDto.BatchNumber,
                    GeneratedDate = DateTime.Now,
                    GeneratedBy = barcodeDto.GeneratedBy ?? _userContext.GetCurrentUserName(),
                    // Store and Location
                    StoreId = barcodeDto.StoreId,
                    Location = barcodeDto.Location,
                    Notes = barcodeDto.Notes,
                    // Initialize tracking fields
                    PrintCount = 0,
                    ScanCount = 0,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.GetCurrentUserName()
                };

                await _unitOfWork.Barcodes.AddAsync(barcode);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    _userContext.GetCurrentUserId(),
                    "Barcode Created",
                    "Barcode",
                    barcode.Id,
                    null,
                    $"Barcode {barcode.BarcodeNumber} created for item {item.Name}" +
                    (barcode.StoreId.HasValue ? $" in store {barcode.StoreId}" : ""),
                    "System"
                );

                await _unitOfWork.CommitTransactionAsync();

                // Return complete DTO
                return await GetBarcodeByIdAsync(barcode.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating barcode");
                throw;
            }
        }
        public async Task<BarcodeDto> UpdateBarcodeAsync(BarcodeDto barcodeDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var barcode = await _unitOfWork.Barcodes.GetByIdAsync(barcodeDto.Id);
                if (barcode == null)
                {
                    throw new InvalidOperationException("Barcode not found");
                }

                // Log activity with old values
                var oldValues = $"SerialNumber: {barcode.SerialNumber}, BatchNumber: {barcode.BatchNumber}, " +
                               $"StoreId: {barcode.StoreId}, Location: {barcode.Location}";

                // Update barcode
                barcode.SerialNumber = barcodeDto.SerialNumber;
                barcode.BatchNumber = barcodeDto.BatchNumber;
                barcode.StoreId = barcodeDto.StoreId;
                barcode.Location = barcodeDto.Location;
                barcode.Notes = barcodeDto.Notes;
                barcode.UpdatedAt = DateTime.Now;
                barcode.UpdatedBy = _userContext.GetCurrentUserName();

                _unitOfWork.Barcodes.Update(barcode);
                await _unitOfWork.CompleteAsync();

                // Log activity
                var newValues = $"SerialNumber: {barcode.SerialNumber}, BatchNumber: {barcode.BatchNumber}, " +
                               $"StoreId: {barcode.StoreId}, Location: {barcode.Location}";

                await _activityLogService.LogActivityAsync(
                    _userContext.GetCurrentUserId(),
                    "Barcode Updated",
                    "Barcode",
                    barcode.Id,
                    oldValues,
                    newValues,
                    "System"
                );

                await _unitOfWork.CommitTransactionAsync();

                return await GetBarcodeByIdAsync(barcode.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating barcode");
                throw;
            }
        }
        public async Task<bool> DeleteBarcodeAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var barcode = await _unitOfWork.Barcodes.GetByIdAsync(id);
                if (barcode == null)
                {
                    return false;
                }

                // Soft delete
                barcode.IsActive = false;
                barcode.UpdatedAt = DateTime.Now;
                barcode.UpdatedBy = _userContext.GetCurrentUserName();

                _unitOfWork.Barcodes.Update(barcode);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    _userContext.GetCurrentUserId(),
                    "Barcode Deleted",
                    "Barcode",
                    barcode.Id,
                    barcode.BarcodeNumber,
                    "Barcode deactivated",
                    "System"
                );

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting barcode");
                throw;
            }
        }

        #endregion

        #region Search and Query Operations

        public async Task<IEnumerable<BarcodeDto>> GetBarcodesByItemAsync(int? itemId)
        {
            try
            {
                var barcodes = await _unitOfWork.Barcodes.FindAsync(
                    b => b.ItemId == itemId && b.IsActive);

                var barcodeDtos = new List<BarcodeDto>();
                foreach (var barcode in barcodes)
                {
                    var dto = await GetBarcodeByIdAsync(barcode.Id);
                    if (dto != null)
                        barcodeDtos.Add(dto);
                }

                return barcodeDtos.OrderByDescending(b => b.GeneratedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcodes by item: {ItemId}", itemId);
                throw;
            }
        }
        public async Task<IEnumerable<BarcodeDto>> GetBarcodesByStoreAsync(int? storeId)
        {
            try
            {
                // Get barcodes directly assigned to store
                var barcodes = await _unitOfWork.Barcodes.FindAsync(
                    b => b.IsActive && b.StoreId == storeId);

                var barcodeDtos = new List<BarcodeDto>();
                foreach (var barcode in barcodes)
                {
                    var dto = await GetBarcodeByIdAsync(barcode.Id);
                    if (dto != null)
                        barcodeDtos.Add(dto);
                }

                return barcodeDtos.OrderByDescending(b => b.GeneratedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcodes by store: {StoreId}", storeId);
                throw;
            }
        }
        public async Task<IEnumerable<BarcodeDto>> GetBarcodesByLocationAsync(int? storeId, string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                    return await GetBarcodesByStoreAsync(storeId);

                var barcodes = await _unitOfWork.Barcodes.FindAsync(
                    b => b.IsActive &&
                         b.StoreId == storeId &&
                         b.Location != null &&
                         b.Location.ToLower().Contains(location.ToLower()));

                var barcodeDtos = new List<BarcodeDto>();
                foreach (var barcode in barcodes)
                {
                    var dto = await GetBarcodeByIdAsync(barcode.Id);
                    if (dto != null)
                        barcodeDtos.Add(dto);
                }

                return barcodeDtos.OrderByDescending(b => b.GeneratedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcodes by location: {StoreId}/{Location}", storeId, location);
                throw;
            }
        }
        public async Task<IEnumerable<BarcodeDto>> GetPagedBarcodesAsync(int pageNumber, int pageSize)
        {
            try
            {
                var result = await GetAllBarcodesAsync(pageNumber, pageSize);
                return result.Items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged barcodes");
                throw;
            }
        }
        public async Task<IEnumerable<BarcodeDto>> SearchBarcodesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<BarcodeDto>();

            try
            {
                searchTerm = searchTerm.Trim().ToLower();

                var barcodes = await _unitOfWork.Barcodes.FindAsync(b =>
                    b.IsActive && (
                        b.BarcodeNumber.ToLower().Contains(searchTerm) ||
                        b.SerialNumber.ToLower().Contains(searchTerm) ||
                        b.BatchNumber.ToLower().Contains(searchTerm)
                    ));

                var barcodeDtos = new List<BarcodeDto>();
                foreach (var barcode in barcodes)
                {
                    var dto = await GetBarcodeByIdAsync(barcode.Id);
                    if (dto != null)
                        barcodeDtos.Add(dto);
                }

                return barcodeDtos.OrderByDescending(b => b.GeneratedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching barcodes with term: {SearchTerm}", searchTerm);
                throw;
            }
        }
        public async Task<bool> BarcodeExistsAsync(string barcodeNumber)
        {
            if (string.IsNullOrWhiteSpace(barcodeNumber))
                return false;

            try
            {
                return await _unitOfWork.Barcodes.ExistsAsync(
                    b => b.BarcodeNumber == barcodeNumber && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if barcode exists: {BarcodeNumber}", barcodeNumber);
                throw;
            }
        }
        public async Task<bool> SerialNumberExistsAsync(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                return false;

            try
            {
                return await _unitOfWork.Barcodes.ExistsAsync(
                    b => b.SerialNumber == serialNumber && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if serial number exists: {SerialNumber}", serialNumber);
                throw;
            }
        }

        #endregion

        #region Barcode Generation Operations

        public async Task<string> GenerateUniqueBarcodeNumberAsync(int? itemId)
        {
            try
            {
                var item = await _unitOfWork.Items.GetByIdAsync(itemId);
                if (item == null)
                    throw new InvalidOperationException("Item not found");

                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                var category = await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId);

                // CRS Format: {CategoryCode}{YYMMDD}{Seq}
                var categoryCode = string.IsNullOrEmpty(category.Code)
                    ? category.Name.Length >= 2
                        ? category.Name.Substring(0, 2).ToUpper()
                        : category.Name.ToUpper().PadRight(2, 'X')
                    : category.Code.Length >= 2
                        ? category.Code.Substring(0, 2).ToUpper()
                        : category.Code.ToUpper().PadRight(2, 'X');

                var dateCode = DateTime.Now.ToString("yyyyMMdd"); // Correct: 8 digits

                // Get sequence for today
                var todayStart = DateTime.Today;
                var todayEnd = todayStart.AddDays(1);

                var todayBarcodes = await _unitOfWork.Barcodes.FindAsync(b =>
                    b.BarcodeNumber.StartsWith($"{categoryCode}{dateCode}") &&
                    b.CreatedAt >= todayStart &&
                    b.CreatedAt < todayEnd);

                var sequence = todayBarcodes.Count() + 1;

                return $"{categoryCode}{dateCode}{sequence:D4}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating unique barcode number for item: {ItemId}", itemId);
                throw;
            }
        }
        public async Task<string> GenerateSerialNumberAsync(string prefix = "SN")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return await Task.FromResult($"{prefix}-{timestamp}-{random}");
        }
        public async Task<string> GenerateBatchNumberAsync(string prefix = "BN")
        {
            var dateCode = DateTime.Now.ToString("yyyyMMdd");
            var random = new Random().Next(1000, 9999);
            return await Task.FromResult($"{prefix}-{dateCode}-{random}");
        }
        public byte[] GenerateQRCode(string text)
        {
            try
            {
                // QRCoder is the primary method for QR code generation
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

                // Using PngByteQRCode which directly returns byte array
                var pngByteQRCode = new PngByteQRCode(qrCodeData);
                return pngByteQRCode.GetGraphic(10);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code for text: {Text}", text);
                _logger.LogWarning("QRCoder failed, generating placeholder image with text");

                // Generate a simple placeholder image with the text
                return GeneratePlaceholderImage(text);
            }
        }
        private byte[] GeneratePlaceholderImage(string text)
        {
            try
            {
                // Create a simple image with text as placeholder
                using var bitmap = new Bitmap(200, 200);
                using var graphics = Graphics.FromImage(bitmap);

                graphics.Clear(Color.White);
                graphics.DrawRectangle(Pens.Black, 0, 0, 199, 199);

                using var font = new System.Drawing.Font("Arial", 10);
                var textSize = graphics.MeasureString(text, font);

                // Center the text
                var x = (200 - textSize.Width) / 2;
                var y = (200 - textSize.Height) / 2;

                graphics.DrawString(text, font, Brushes.Black, x, y);
                graphics.DrawString("QR Not Available", font, Brushes.Red, 50, 10);

                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            catch
            {
                // If even this fails, return a 1x1 transparent PNG
                return Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");
            }
        }
        public string GenerateQRCodeBase64(string text)
        {
            var qrCodeBytes = GenerateQRCode(text);
            return Convert.ToBase64String(qrCodeBytes);
        }
        public byte[] GenerateCode128Barcode(string text)
        {
            try
            {
                _code128Generator.IncludeLabel = true;
                _code128Generator.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                _code128Generator.Height = 100;
                _code128Generator.Width = 300;
                _code128Generator.BackColor = Color.White;
                _code128Generator.ForeColor = Color.Black;

                var barcodeImage = _code128Generator.Encode(BarcodeLib.TYPE.CODE128, text);

                using var ms = new MemoryStream();
                barcodeImage.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Code128 barcode for text: {Text}", text);
                throw;
            }
        }
        public string GenerateCode128BarcodeBase64(string text)
        {
            var barcodeBytes = GenerateCode128Barcode(text);
            return Convert.ToBase64String(barcodeBytes);
        }

        #endregion

        #region Batch Operations

        public async Task<List<BarcodeDto>> GenerateBatchBarcodesAsync(int itemId, int quantity, string createdBy = null)
        {
            if (quantity <= 0 || quantity > 100)
            {
                throw new ArgumentException("Quantity must be between 1 and 100");
            }

            var barcodes = new List<BarcodeDto>();

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var batchNumber = await GenerateBatchNumberAsync();

                for (int i = 0; i < quantity; i++)
                {
                    var barcodeDto = new BarcodeDto
                    {
                        ItemId = itemId,
                        BarcodeNumber = await GenerateUniqueBarcodeNumberAsync(itemId),
                        SerialNumber = await GenerateSerialNumberAsync(),
                        BatchNumber = batchNumber,
                        GeneratedBy = createdBy ?? _userContext.GetCurrentUserName()
                    };

                    var created = await CreateBarcodeAsync(barcodeDto);
                    barcodes.Add(created);

                    // Small delay to ensure unique timestamps
                    if (i < quantity - 1)
                        await Task.Delay(10);
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Successfully generated {Count} barcodes for item {ItemId}",
                    barcodes.Count, itemId);

                return barcodes;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error generating batch barcodes");
                throw;
            }
        }
        public async Task<List<BarcodeDto>> GenerateBatchBarcodesForStoreAsync(int itemId, int? storeId, int quantity, string location = null)
        {
            if (quantity <= 0 || quantity > 100)
            {
                throw new ArgumentException("Quantity must be between 1 and 100");
            }

            // Validate store
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            if (store == null || !store.IsActive)
            {
                throw new InvalidOperationException("Invalid store ID or store is inactive");
            }

            var barcodes = new List<BarcodeDto>();

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var batchNumber = await GenerateBatchNumberAsync();

                for (int i = 0; i < quantity; i++)
                {
                    var barcodeDto = new BarcodeDto
                    {
                        ItemId = itemId,
                        BarcodeNumber = await GenerateUniqueBarcodeNumberAsync(itemId),
                        SerialNumber = await GenerateSerialNumberAsync(),
                        BatchNumber = batchNumber,
                        StoreId = storeId,
                        StoreName = store.Name,
                        Location = location,
                        GeneratedBy = _userContext.GetCurrentUserName()
                    };

                    var created = await CreateBarcodeAsync(barcodeDto);
                    barcodes.Add(created);

                    // Small delay to ensure unique timestamps
                    if (i < quantity - 1)
                        await Task.Delay(10);
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Successfully generated {Count} barcodes for item {ItemId} in store {StoreId}",
                    barcodes.Count, itemId, storeId);

                return barcodes;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error generating batch barcodes for store");
                throw;
            }
        }
        public async Task<byte[]> GenerateBatchBarcodePDF(List<BarcodeDto> barcodes)
        {
            if (barcodes == null || !barcodes.Any())
            {
                throw new ArgumentException("Barcodes list cannot be null or empty");
            }

            try
            {
                using var memoryStream = new MemoryStream();

                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Barcode Labels", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var currentUser = _userContext?.GetCurrentUserName() ?? "System";
                var printInfo = new Paragraph($"Printed by: {currentUser} on {DateTime.Now:yyyy-MM-dd HH:mm}", infoFont);
                printInfo.Alignment = Element.ALIGN_CENTER;
                document.Add(printInfo);
                document.Add(new Paragraph("\n"));

                var table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new[] { 33f, 33f, 33f });

                foreach (var barcode in barcodes)
                {
                    if (barcode != null)
                    {
                        var cell = CreateBarcodeCell(barcode);
                        table.AddCell(cell);
                    }
                }

                var emptyCells = 3 - (barcodes.Count % 3);
                if (emptyCells < 3)
                {
                    for (int i = 0; i < emptyCells; i++)
                    {
                        var emptyCell = new PdfPCell();
                        emptyCell.Border = Rectangle.NO_BORDER;
                        table.AddCell(emptyCell);
                    }
                }

                document.Add(table);
                document.Close();

                var printedBy = _userContext?.GetCurrentUserName() ?? "System";
                foreach (var barcode in barcodes)
                {
                    if (barcode != null && barcode.Id > 0)
                    {
                        try
                        {
                            await UpdatePrintInformationAsync(barcode.Id, printedBy);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error updating print information for barcode {Id}", barcode.Id);
                        }
                    }
                }

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating batch barcode PDF");
                throw;
            }
        }

        private PdfPCell CreateBarcodeCell(BarcodeDto barcode)
        {
            try
            {
                var cellTable = new PdfPTable(1);
                cellTable.WidthPercentage = 100;

                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

                if (!string.IsNullOrEmpty(barcode.BarcodeNumber))
                {
                    try
                    {
                        var qrCodeBytes = GenerateQRCode(barcode.BarcodeNumber);
                        if (qrCodeBytes != null && qrCodeBytes.Length > 0)
                        {
                            var qrImage = iTextSharp.text.Image.GetInstance(qrCodeBytes);
                            qrImage.ScaleToFit(80f, 80f);
                            qrImage.Alignment = Element.ALIGN_CENTER;

                            var imageCell = new PdfPCell(qrImage);
                            imageCell.Border = Rectangle.NO_BORDER;
                            imageCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            imageCell.PaddingBottom = 5;
                            cellTable.AddCell(imageCell);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating QR code for barcode {BarcodeNumber}", barcode.BarcodeNumber);
                    }

                    var barcodeNumberPara = new Paragraph(barcode.BarcodeNumber, boldFont);
                    barcodeNumberPara.Alignment = Element.ALIGN_CENTER;
                    var barcodeCell = new PdfPCell(barcodeNumberPara);
                    barcodeCell.Border = Rectangle.NO_BORDER;
                    barcodeCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    barcodeCell.PaddingBottom = 3;
                    cellTable.AddCell(barcodeCell);
                }

                if (!string.IsNullOrEmpty(barcode.ItemName))
                {
                    var itemNamePara = new Paragraph(barcode.ItemName, normalFont);
                    itemNamePara.Alignment = Element.ALIGN_CENTER;
                    var itemCell = new PdfPCell(itemNamePara);
                    itemCell.Border = Rectangle.NO_BORDER;
                    itemCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    itemCell.PaddingBottom = 2;
                    cellTable.AddCell(itemCell);
                }

                if (!string.IsNullOrEmpty(barcode.SerialNumber))
                {
                    var serialPara = new Paragraph($"SN: {barcode.SerialNumber}", smallFont);
                    serialPara.Alignment = Element.ALIGN_CENTER;
                    var serialCell = new PdfPCell(serialPara);
                    serialCell.Border = Rectangle.NO_BORDER;
                    serialCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    serialCell.PaddingBottom = 2;
                    cellTable.AddCell(serialCell);
                }

                if (!string.IsNullOrEmpty(barcode.StoreName))
                {
                    var storePara = new Paragraph($"Store: {barcode.StoreName}", smallFont);
                    storePara.Alignment = Element.ALIGN_CENTER;
                    var storeCell = new PdfPCell(storePara);
                    storeCell.Border = Rectangle.NO_BORDER;
                    storeCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    storeCell.PaddingBottom = 2;
                    cellTable.AddCell(storeCell);
                }

                var datePara = new Paragraph($"Generated: {barcode.GeneratedDate:MM/dd/yyyy}", smallFont);
                datePara.Alignment = Element.ALIGN_CENTER;
                var dateCell = new PdfPCell(datePara);
                dateCell.Border = Rectangle.NO_BORDER;
                dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
                cellTable.AddCell(dateCell);

                var mainCell = new PdfPCell(cellTable);
                mainCell.Padding = 10;
                mainCell.BorderWidth = 1;
                mainCell.BorderColor = new iTextSharp.text.BaseColor(211, 211, 211); // Light Gray

                return mainCell;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating barcode cell for barcode {Id}", barcode?.Id);

                var errorCell = new PdfPCell(new Phrase("Error generating barcode", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
                errorCell.Padding = 10;
                errorCell.BorderWidth = 1;
                errorCell.BorderColor = new iTextSharp.text.BaseColor(255, 0, 0); // Red
                return errorCell;
            }
        }

        public async Task UpdatePrintInformationAsync(int barcodeId, string printedBy)        {
            try
            {
                var barcode = await _unitOfWork.Barcodes.GetByIdAsync(barcodeId);
                if (barcode != null)
                {
                    barcode.PrintCount++;
                    barcode.PrintedDate = DateTime.Now;
                    barcode.PrintedBy = printedBy;

                    _unitOfWork.Barcodes.Update(barcode);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating print information for barcode {BarcodeId}", barcodeId);
                throw;
            }
        }
        public async Task<byte[]> GenerateBarcodeLabelsAsync(List<int> barcodeIds, BarcodeLabel labelFormat)
        {
            try
            {
                var barcodes = new List<BarcodeDto>();
                foreach (var id in barcodeIds)
                {
                    var barcode = await GetBarcodeByIdAsync(id);
                    if (barcode != null)
                        barcodes.Add(barcode);
                }

                if (!barcodes.Any())
                    throw new InvalidOperationException("No valid barcodes found");

                // Generate PDF based on label format
                return await GenerateBatchBarcodePDF(barcodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating barcode labels");
                throw;
            }
        }

        #endregion

        #region Validation Operations

        public async Task<(bool isValid, string message, BarcodeDto barcode)> ValidateBarcodeAsync(string barcodeNumber)
        {
            if (string.IsNullOrWhiteSpace(barcodeNumber))
            {
                return (false, "Barcode number is required", null);
            }

            try
            {
                // Check barcode format
                var formatValidation = await ValidateBarcodeFormatAsync(barcodeNumber);
                if (!formatValidation)
                {
                    return (false, "Invalid barcode format", null);
                }

                var barcode = await GetBarcodeByNumberAsync(barcodeNumber);
                if (barcode == null)
                {
                    return (false, "Barcode not found", null);
                }

                if (!barcode.IsActive)
                {
                    return (false, "Barcode has been deactivated", barcode);
                }

                var item = await _unitOfWork.Items.GetByIdAsync(barcode.ItemId);
                if (item == null)
                {
                    return (false, "Item associated with barcode not found", barcode);
                }

                if (!item.IsActive)
                {
                    return (false, "Item associated with barcode is inactive", barcode);
                }

                // Check stock availability
                var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id);
                var totalStock = storeItems.Sum(si => si.Quantity);

                if (totalStock <= 0)
                {
                    return (true, "Valid barcode - Warning: No stock available", barcode);
                }

                return (true, "Valid barcode", barcode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating barcode: {BarcodeNumber}", barcodeNumber);
                return (false, "Error validating barcode", null);
            }
        }
        public async Task<bool> ValidateBarcodeFormatAsync(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return false;

            try
            {
                // CRS Format: {CategoryCode}{YYMMDD}{Seq}
                // Expected length: 2 + 6 + 4 = 12 characters
                if (barcode.Length != 12)
                    return false;

                // Extract parts
                var categoryCode = barcode.Substring(0, 2);
                var dateCode = barcode.Substring(2, 6);
                var sequence = barcode.Substring(8, 4);

                // Validate category code (should be letters)
                if (!categoryCode.All(char.IsLetter))
                    return false;

                // Validate date code (should be valid date in yyMMdd format)
                if (!DateTime.TryParseExact(dateCode, "yyMMdd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    return false;

                // Validate sequence (should be numeric)
                if (!int.TryParse(sequence, out _))
                    return false;

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating barcode format: {Barcode}", barcode);
                return false;
            }
        }
        public async Task<IMS.Application.Services.ValidationResult> ValidateBarcodeChecksum(string barcode)
        {
            var result = new IMS.Application.Services.ValidationResult { IsValid = true };

            try
            {
                if (string.IsNullOrEmpty(barcode))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Barcode is required";
                    return result;
                }

                // Validate format
                if (!await ValidateBarcodeFormatAsync(barcode))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Invalid barcode format";
                    return result;
                }

                // Check if exists
                if (!await BarcodeExistsAsync(barcode))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Barcode does not exist";
                    return result;
                }

                result.ErrorMessage = result.IsValid ? "Barcode is valid" : "Barcode validation failed";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating barcode checksum");
                result.IsValid = false;
                result.ErrorMessage = "Error during validation";
                return result;
            }
        }

        #endregion

        #region Tracking and History
        public async Task UpdateBarcodeTrackingAsync(int barcodeId, string action, string userId)
        {
            try
            {
                var barcode = await _unitOfWork.Barcodes.GetByIdAsync(barcodeId);
                if (barcode == null)
                    return;

                // Update barcode tracking info
                barcode.LastScannedDate = DateTime.Now;
                barcode.LastScannedBy = userId;
                barcode.LastScannedLocation = action;
                barcode.ScanCount = barcode.ScanCount + 1;
                barcode.UpdatedAt = DateTime.Now;
                barcode.UpdatedBy = userId;

                _unitOfWork.Barcodes.Update(barcode);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    userId,
                    $"Barcode {action}",
                    "Barcode",
                    barcodeId,
                    null,
                    $"Barcode {barcode.BarcodeNumber} - Action: {action}",
                    "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating barcode tracking");
                throw;
            }
        }
        public async Task UpdateBarcodeLocationAsync(int barcodeId, int? storeId, string location, string notes = null)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var barcode = await _unitOfWork.Barcodes.GetByIdAsync(barcodeId);
                if (barcode == null)
                    throw new InvalidOperationException("Barcode not found");

                var oldLocation = $"Store: {barcode.StoreId}, Location: {barcode.Location}";

                barcode.StoreId = storeId;
                barcode.Location = location;
                if (!string.IsNullOrEmpty(notes))
                {
                    barcode.Notes = notes;
                }
                barcode.UpdatedAt = DateTime.Now;
                barcode.UpdatedBy = _userContext.GetCurrentUserName();

                _unitOfWork.Barcodes.Update(barcode);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    _userContext.GetCurrentUserId(),
                    "Barcode Location Updated",
                    "Barcode",
                    barcodeId,
                    oldLocation,
                    $"Store: {storeId}, Location: {location}",
                    "System"
                );

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating barcode location");
                throw;
            }
        }
        public async Task<IEnumerable<BarcodeTrackingDto>> GetBarcodeHistoryAsync(int barcodeId)
        {
            try
            {
                // Get activity logs for this barcode
                var logs = await _unitOfWork.ActivityLogs.FindAsync(
                    a => a.EntityName == "Barcode" && a.EntityId == barcodeId);

                return logs.Select(log => new BarcodeTrackingDto
                {
                    Id = log.Id,
                    BarcodeId = barcodeId,
                    Action = log.Action,
                    Location = log.Module,
                    PerformedBy = log.UserId,
                    Timestamp = log.Timestamp,
                    Details = log.Details
                }).OrderByDescending(t => t.Timestamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcode history");
                throw;
            }
        }
        public async Task<BarcodeDto> ProcessOfflineScanAsync(OfflineScanDto scanData, string userId)
        {
            if (scanData == null)
                throw new ArgumentNullException(nameof(scanData));

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var barcode = await GetBarcodeByNumberAsync(scanData.Barcode);
                if (barcode == null)
                    throw new InvalidOperationException("Barcode not found");

                // Update tracking
                await UpdateBarcodeTrackingAsync(barcode.Id, scanData.Action, userId);

                // Process based on action
                switch (scanData.Action.ToLower())
                {
                    case "issue":
                        await ProcessOfflineIssue(barcode, scanData, userId);
                        break;
                    case "receive":
                        await ProcessOfflineReceive(barcode, scanData, userId);
                        break;
                    case "transfer":
                        await ProcessOfflineTransfer(barcode, scanData, userId);
                        break;
                    case "count":
                        await ProcessOfflineCount(barcode, scanData, userId);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown action: {scanData.Action}");
                }

                await _unitOfWork.CommitTransactionAsync();

                return barcode;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error processing offline scan");
                throw;
            }
        }

        #endregion

        #region Analytics and Reports
        public async Task<BarcodeStatisticsDto> GetBarcodeStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                fromDate ??= DateTime.Now.AddMonths(-1);
                toDate ??= DateTime.Now;

                var allBarcodes = await _unitOfWork.Barcodes.GetAllAsync();
                var dateFilteredBarcodes = allBarcodes.Where(b =>
                    b.CreatedAt >= fromDate && b.CreatedAt <= toDate);

                // Get category statistics
                var barcodesByCategory = new Dictionary<string, int>();
                foreach (var barcode in allBarcodes.Where(b => b.IsActive))
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(barcode.ItemId);
                    if (item != null)
                    {
                        var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                        var category = await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId);

                        var categoryName = category?.Name ?? "Unknown";
                        if (!barcodesByCategory.ContainsKey(categoryName))
                            barcodesByCategory[categoryName] = 0;
                        barcodesByCategory[categoryName]++;
                    }
                }

                // Get store statistics
                var barcodesByStore = new Dictionary<string, int>();
                var barcodesByLocation = new Dictionary<string, int>();

                foreach (var barcode in allBarcodes.Where(b => b.IsActive))
                {
                    if (barcode.StoreId.HasValue)
                    {
                        var store = await _unitOfWork.Stores.GetByIdAsync(barcode.StoreId.Value);
                        var storeName = store?.Name ?? "Unknown";

                        if (!barcodesByStore.ContainsKey(storeName))
                            barcodesByStore[storeName] = 0;
                        barcodesByStore[storeName]++;
                    }

                    if (!string.IsNullOrEmpty(barcode.Location))
                    {
                        if (!barcodesByLocation.ContainsKey(barcode.Location))
                            barcodesByLocation[barcode.Location] = 0;
                        barcodesByLocation[barcode.Location]++;
                    }
                }

                // Calculate daily activity
                var dailyActivity = new List<DailyBarcodeActivity>();
                var currentDate = fromDate.Value.Date;

                while (currentDate <= toDate.Value.Date)
                {
                    var dayActivity = new DailyBarcodeActivity
                    {
                        Date = currentDate,
                        Generated = dateFilteredBarcodes.Count(b => b.CreatedAt.Date == currentDate),
                        Scanned = allBarcodes.Count(b => b.LastScannedDate?.Date == currentDate),
                        Printed = allBarcodes.Count(b => b.PrintedDate?.Date == currentDate),
                        Deactivated = dateFilteredBarcodes.Count(b => !b.IsActive && b.UpdatedAt?.Date == currentDate)
                    };

                    dailyActivity.Add(dayActivity);
                    currentDate = currentDate.AddDays(1);
                }

                return new BarcodeStatisticsDto
                {
                    TotalBarcodes = allBarcodes.Count(),
                    ActiveBarcodes = allBarcodes.Count(b => b.IsActive),
                    InactiveBarcodes = allBarcodes.Count(b => !b.IsActive),
                    ScannedToday = allBarcodes.Count(b => b.LastScannedDate?.Date == DateTime.Today),
                    GeneratedToday = allBarcodes.Count(b => b.CreatedAt.Date == DateTime.Today),
                    PrintedToday = allBarcodes.Count(b => b.PrintedDate?.Date == DateTime.Today),
                    TotalPrintCount = allBarcodes.Sum(b => b.PrintCount),
                    BarcodesWithLocation = allBarcodes.Count(b => !string.IsNullOrEmpty(b.Location)),
                    BarcodesByCategory = barcodesByCategory,
                    BarcodesByStore = barcodesByStore,
                    BarcodesByLocation = barcodesByLocation,
                    DailyActivity = dailyActivity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcode statistics");
                throw;
            }
        }
        public async Task<IEnumerable<BarcodeUsageDto>> GetBarcodeUsageReportAsync(
            int? storeId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                fromDate ??= DateTime.Now.AddMonths(-1);
                toDate ??= DateTime.Now;

                var query = await _unitOfWork.Barcodes.FindAsync(b =>
                    b.IsActive &&
                    b.LastScannedDate >= fromDate &&
                    b.LastScannedDate <= toDate);

                if (storeId.HasValue)
                {
                    var storeItemIds = (await _unitOfWork.StoreItems.FindAsync(
                        si => si.StoreId == storeId.Value))
                        .Select(si => si?.ItemId).ToList();

                    query = query.Where(b => storeItemIds.Contains(b.ItemId));
                }

                var usageReport = new List<BarcodeUsageDto>();

                foreach (var barcode in query)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(barcode.ItemId);

                    usageReport.Add(new BarcodeUsageDto
                    {
                        BarcodeNumber = barcode.BarcodeNumber,
                        ItemName = item?.Name ?? "Unknown",
                        TotalScans = barcode.ScanCount,
                        LastUsed = barcode.LastScannedDate ?? barcode.CreatedAt,
                        MostFrequentLocation = barcode.LastScannedLocation ?? "N/A",
                        UsageTypes = new List<string> { barcode.LastScannedLocation ?? "N/A" }
                    });
                }

                return usageReport.OrderByDescending(u => u.TotalScans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcode usage report");
                throw;
            }
        }
        public async Task<int> GetActiveBarcodeCountAsync()
        {
            try
            {
                return await _unitOfWork.Barcodes.CountAsync(b => b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active barcode count");
                throw;
            }
        }
        public async Task<int> GetBarcodeCountByItemAsync(int? itemId)
        {
            try
            {
                return await _unitOfWork.Barcodes.CountAsync(
                    b => b.ItemId == itemId && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcode count by item");
                throw;
            }
        }
        #endregion

        #region Private Helper Methods
        private BarcodeDto MapToDto(
            Barcode barcode,
            Dictionary<int, Item> itemDict,
            Dictionary<int, SubCategory> subCategoryDict,
            Dictionary<int, Category> categoryDict)
        {
            // Safely fetch item if ItemId has value
            Item item = null;
            if (barcode.ItemId.HasValue)
                itemDict.TryGetValue(barcode.ItemId.Value, out item);

            // Fetch subcategory
            SubCategory subCategory = null;
            if (item != null && subCategoryDict.TryGetValue(item.SubCategoryId, out var sc))
                subCategory = sc;

            // Fetch category
            Category category = null;
            if (subCategory != null && categoryDict.TryGetValue(subCategory.CategoryId, out var cat))
                category = cat;

            return new BarcodeDto
            {
                Id = barcode.Id,
                BarcodeNumber = barcode.BarcodeNumber,
                BarcodeType = barcode.BarcodeType,
                ItemId = barcode.ItemId,
                ItemName = item?.Name ?? "Unknown",
                ItemCode = item?.ItemCode ?? "Unknown",
                CategoryName = category?.Name ?? "Unknown",
                SubCategoryName = subCategory?.Name ?? "Unknown",
                SerialNumber = barcode.SerialNumber,
                BatchNumber = barcode.BatchNumber,
                ReferenceType = barcode.ReferenceType,
                ReferenceId = barcode.ReferenceId,
                GeneratedDate = barcode.GeneratedDate,
                GeneratedBy = barcode.GeneratedBy,
                BarcodeData = barcode.BarcodeData,
                IsActive = barcode.IsActive,
                CreatedAt = barcode.CreatedAt,
                CreatedBy = barcode.CreatedBy,
                UpdatedAt = barcode.UpdatedAt,
                UpdatedBy = barcode.UpdatedBy,

                StoreId = barcode.StoreId,
                StoreName = "N/A", // To be populated separately if needed
                Location = barcode.Location,
                Notes = barcode.Notes,

                PrintedBy = barcode.PrintedBy,
                PrintedDate = barcode.PrintedDate,
                PrintCount = barcode.PrintCount,

                LastScannedLocation = barcode.LastScannedLocation,
                LastScannedDate = barcode.LastScannedDate,
                LastScannedBy = barcode.LastScannedBy,
                ScanCount = barcode.ScanCount
            };
        }
        private async Task ProcessOfflineIssue(BarcodeDto barcode, OfflineScanDto scanData, string userId)
        {
            // Create issue record
            if (scanData.StoreId > 0)
                throw new InvalidOperationException("Store ID is required for issue");

            // Verify stock
            var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                si => si.ItemId == barcode.ItemId && si.StoreId == scanData.StoreId);

            if (storeItem == null || storeItem.Quantity < scanData.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            // Log the offline issue - you might want to create an actual issue record
            await _activityLogService.LogActivityAsync(
                userId,
                "Offline Issue",
                "Barcode",
                barcode.Id,
                null,
                $"Offline issue of {scanData.Quantity} units from store {scanData.StoreId}",
                scanData.DeviceId ?? "Unknown Device"
            );
        }
        private async Task ProcessOfflineReceive(BarcodeDto barcode, OfflineScanDto scanData, string userId)
        {
            // Process offline receive
            if (scanData.StoreId == 0)
                throw new InvalidOperationException("Store ID is required for receive");

            await _activityLogService.LogActivityAsync(
                userId,
                "Offline Receive",
                "Barcode",
                barcode.Id,
                null,
                $"Offline receive of {scanData.Quantity} units to store {scanData.StoreId}",
                scanData.DeviceId ?? "Unknown Device"
            );
        }
        private async Task ProcessOfflineTransfer(BarcodeDto barcode, OfflineScanDto scanData, string userId)
        {
            // Process offline transfer
            if (scanData.FromStoreId == 0 || scanData.ToStoreId == 0)
                throw new InvalidOperationException("Both FromStoreId and ToStoreId are required for transfer");

            await _activityLogService.LogActivityAsync(
                userId,
                "Offline Transfer",
                "Barcode",
                barcode.Id,
                null,
                $"Offline transfer of {scanData.Quantity} units from store {scanData.FromStoreId} to store {scanData.ToStoreId}",
                scanData.DeviceId ?? "Unknown Device"
            );
        }
        private async Task ProcessOfflineCount(BarcodeDto barcode, OfflineScanDto scanData, string userId)
        {
            // Process offline count
            if (scanData.StoreId == 0)
                throw new InvalidOperationException("Store ID is required for count");

            await _activityLogService.LogActivityAsync(
                userId,
                "Offline Count",
                "Barcode",
                barcode.Id,
                null,
                $"Offline count of {scanData.Quantity} units in store {scanData.StoreId}",
                scanData.DeviceId ?? "Unknown Device"
            );
        }
        #endregion
        public async Task<TrackingInfoDto> DecodeTrackingQRAsync(string qrCode)
        {
            try
            {
                var decodedBytes = Convert.FromBase64String(qrCode);
                var json = Encoding.UTF8.GetString(decodedBytes);
                var trackingData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                var trackingInfo = new TrackingInfoDto
                {
                    Type = trackingData["Type"]?.ToString(),
                    ReferenceId = Convert.ToInt32(trackingData["ReferenceId"]),
                    ReferenceNo = trackingData["ReferenceNo"]?.ToString(),
                    Timestamp = new DateTime(Convert.ToInt64(trackingData["Timestamp"]))
                };

                // Get current status based on type
                switch (trackingInfo.Type)
                {
                    case "ISSUE_VOUCHER":
                        var issue = await _unitOfWork.Issues.GetByIdAsync(trackingInfo.ReferenceId);
                        trackingInfo.CurrentStatus = issue?.Status;
                        trackingInfo.LastLocation = "Issue Store";
                        break;

                    case "TRANSFER":
                        var transfer = await _unitOfWork.Transfers.GetByIdAsync(trackingInfo.ReferenceId);
                        trackingInfo.CurrentStatus = transfer?.Status;
                        trackingInfo.LastLocation = transfer?.Status == "In Transit" ? "In Transit" : "Destination Store";
                        break;
                }

                return trackingInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decoding tracking QR");
                return null;
            }
        }
        public async Task<string> GenerateShipmentQRAsync(string referenceType, int referenceId, Dictionary<string, object> additionalData)
        {
            var trackingData = new Dictionary<string, object>
            {
                ["Type"] = referenceType,
                ["ReferenceId"] = referenceId,
                ["Timestamp"] = DateTime.Now.Ticks,
                ["GeneratedBy"] = _userContext.CurrentUserName
            };

            // Add additional data
            foreach (var kvp in additionalData)
            {
                trackingData[kvp.Key] = kvp.Value;
            }

            var json = JsonSerializer.Serialize(trackingData);
            var qrCodeBase64 = GenerateQRCodeBase64(json);

            // Store tracking info
            var tracking = new ShipmentTracking
            {
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                TrackingCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(json)),
                QRCode = qrCodeBase64,
                Status = "Generated",
                CreatedAt = DateTime.Now,
                CreatedBy = _userContext.CurrentUserName
            };

            await _unitOfWork.ShipmentTrackings.AddAsync(tracking);
            await _unitOfWork.CompleteAsync();

            return qrCodeBase64;
        }
        public async Task<ServiceResult> UpdateTrackingStatusAsync(string qrCode, string status, string location = null)
        {
            try
            {
                var tracking = await _unitOfWork.ShipmentTrackings
                    .FirstOrDefaultAsync(t => t.TrackingCode == qrCode);

                if (tracking == null)
                    return ServiceResult.Failure("Invalid tracking code");

                tracking.Status = status;
                tracking.LastLocation = location;
                tracking.LastUpdated = DateTime.Now;
                tracking.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.ShipmentTrackings.Update(tracking);

                // Add tracking history
                var history = new TrackingHistory
                {
                    ShipmentTrackingId = tracking.Id,
                    Status = status,
                    Location = location,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = _userContext.CurrentUserName
                };

                await _unitOfWork.TrackingHistories.AddAsync(history);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.SuccessResult("Tracking status updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tracking status");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }
        public async Task<string> GenerateBarcodeAsync(int itemId, string batchNumber = null)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null)
                throw new InvalidOperationException("Item not found");

            var barcodeNumber = GenerateBarcodeNumber(itemId, batchNumber);

            var barcode = new Barcode
            {
                BarcodeNumber = barcodeNumber,
                ItemId = itemId,
                BatchNumber = batchNumber,
                GeneratedDate = DateTime.Now,
                GeneratedBy = _userContext.CurrentUserName,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = _userContext.CurrentUserName
            };

            await _unitOfWork.Barcodes.AddAsync(barcode);
            await _unitOfWork.CompleteAsync();

            return barcodeNumber;
        }
        public async Task<QRCodeDto> GenerateQRCodeForVoucherAsync(string voucherType, int voucherId)
        {
            try
            {
                var qrData = new Dictionary<string, object>();

                switch (voucherType.ToUpper())
                {
                    case "ISSUE":
                        var issue = await _unitOfWork.Issues
                            .GetAsync(i => i.Id == voucherId, includes: new[] { "IssueItems" });

                        qrData = new Dictionary<string, object>
                        {
                            ["Type"] = "ISSUE_VOUCHER",
                            ["IssueId"] = issue.Id,
                            ["IssueNo"] = issue.IssueNo,
                            ["Date"] = issue.IssueDate.ToString("yyyy-MM-dd"),
                            ["StoreId"] = issue.FromStoreId,
                            ["Items"] = issue.Items.Select(i => new
                            {
                                ItemId = i.ItemId,
                                Qty = i.Quantity
                            }),
                            ["GeneratedAt"] = DateTime.Now
                        };
                        break;

                    case "RECEIVE":
                        var receive = await _unitOfWork.Receives
                            .GetAsync(r => r.Id == voucherId, includes: new[] { "ReceiveItems" });

                        qrData = new Dictionary<string, object>
                        {
                            ["Type"] = "RECEIVE_VOUCHER",
                            ["ReceiveId"] = receive.Id,
                            ["ReceiveNo"] = receive.ReceiveNo,
                            ["Date"] = receive.ReceiveDate.ToString("yyyy-MM-dd"),
                            ["StoreId"] = receive.StoreId,
                            ["Items"] = receive.ReceiveItems.Select(i => new
                            {
                                ItemId = i.ItemId,
                                Qty = i.Quantity
                            }),
                            ["GeneratedAt"] = DateTime.Now
                        };
                        break;

                    case "TRANSFER":
                        var transfer = await _unitOfWork.Transfers
                            .GetAsync(t => t.Id == voucherId, includes: new[] { "TransferItems" });

                        qrData = new Dictionary<string, object>
                        {
                            ["Type"] = "TRANSFER_VOUCHER",
                            ["TransferId"] = transfer.Id,
                            ["TransferNo"] = transfer.TransferNo,
                            ["Date"] = transfer.TransferDate.ToString("yyyy-MM-dd"),
                            ["FromStore"] = transfer.FromStoreId,
                            ["ToStore"] = transfer.ToStoreId,
                            ["Items"] = transfer.Items.Select(i => new
                            {
                                ItemId = i.ItemId,
                                Qty = i.Quantity
                            }),
                            ["GeneratedAt"] = DateTime.Now
                        };
                        break;

                    case "REQUISITION":
                        var requisition = await _unitOfWork.Requisitions
                            .GetAsync(r => r.Id == voucherId, includes: new[] { "RequisitionItems" });

                        qrData = new Dictionary<string, object>
                        {
                            ["Type"] = "REQUISITION_VOUCHER",
                            ["RequisitionId"] = requisition.Id,
                            ["RequisitionNo"] = requisition.RequisitionNumber,
                            ["Date"] = requisition.RequestDate.ToString("yyyy-MM-dd"),
                            ["Priority"] = requisition.Priority,
                            ["Department"] = requisition.Department,
                            ["Items"] = requisition.RequisitionItems.Select(i => new
                            {
                                ItemId = i.ItemId,
                                Qty = i.RequestedQuantity
                            }),
                            ["GeneratedAt"] = DateTime.Now
                        };
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid voucher type: {voucherType}");
                }

                var jsonData = JsonSerializer.Serialize(qrData);
                var qrCodeImage = GenerateQRCodeImage(jsonData);
                var base64Image = ConvertToBase64(qrCodeImage);

                // Save QR code reference
                var barcode = new Barcode
                {
                    BarcodeNumber = $"QR-{voucherType}-{voucherId}",
                    BarcodeType = "QRCode",
                    ReferenceType = voucherType,
                    ReferenceId = voucherId,
                    BarcodeData = jsonData,
                    GeneratedDate = DateTime.Now,
                    GeneratedBy = _userContext.CurrentUserName,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName
                };

                await _unitOfWork.Barcodes.AddAsync(barcode);
                await _unitOfWork.CompleteAsync();

                return new QRCodeDto
                {
                    QRCodeId = barcode.Id,
                    VoucherType = voucherType,
                    VoucherId = voucherId,
                    QRCodeData = jsonData,
                    QRCodeImage = base64Image,
                    GeneratedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating QR code for {voucherType} voucher {voucherId}");
                throw;
            }
        }
        public async Task<BatchPrintDto> GenerateBatchLabelsAsync(BatchPrintRequestDto request)
        {
            var labels = new List<BarcodeLabel>();

            foreach (var itemRequest in request.Items)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(itemRequest.ItemId);
                if (item == null) continue;

                for (int i = 0; i < itemRequest.Quantity; i++)
                {
                    var barcodeNumber = await GenerateBarcodeAsync(itemRequest.ItemId, itemRequest.BatchNumber);

                    var label = new BarcodeLabel
                    {
                        BarcodeNumber = barcodeNumber,
                        ItemName = item.Name,
                        ItemCode = item.Code,
                        BatchNumber = itemRequest.BatchNumber,
                        ExpiryDate = itemRequest.ExpiryDate,
                        Size = request.LabelSize,
                        BarcodeImage = GenerateBarcodeImage(barcodeNumber, request.LabelSize)
                    };

                    labels.Add(label);
                }
            }

            return new BatchPrintDto
            {
                Labels = labels,
                TotalLabels = labels.Count,
                PrintDate = DateTime.Now,
                PrintedBy = _userContext.CurrentUserName
            };
        }
        public async Task<ScanResultDto> ScanBarcodeAsync(string barcodeData)
        {
            try
            {
                // Check if it's JSON (QR code data)
                if (barcodeData.StartsWith("{"))
                {
                    var qrData = JsonSerializer.Deserialize<Dictionary<string, object>>(barcodeData);
                    return new ScanResultDto
                    {
                        Type = "QRCode",
                        Data = qrData,
                        Success = true
                    };
                }

                // Regular barcode
                var barcode = await _unitOfWork.Barcodes
                    .FirstOrDefaultAsync(b => b.BarcodeNumber == barcodeData);

                if (barcode != null)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(barcode.ItemId ?? 0);

                    return new ScanResultDto
                    {
                        Type = "Barcode",
                        BarcodeNumber = barcode.BarcodeNumber,
                        ItemId = barcode.ItemId,
                        ItemName = item?.Name,
                        BatchNumber = barcode.BatchNumber,
                        Success = true
                    };
                }

                // Try to find item by code
                var itemByCode = await _unitOfWork.Items
                    .FirstOrDefaultAsync(i => i.Code == barcodeData || i.Barcode == barcodeData);

                if (itemByCode != null)
                {
                    return new ScanResultDto
                    {
                        Type = "ItemCode",
                        ItemId = itemByCode.Id,
                        ItemName = itemByCode.Name,
                        ItemCode = itemByCode.Code,
                        Success = true
                    };
                }

                return new ScanResultDto
                {
                    Success = false,
                    ErrorMessage = "Barcode not found"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error scanning barcode: {barcodeData}");
                return new ScanResultDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        public async Task<bool> SyncOfflineScansAsync(List<OfflineScanDto> offlineScans)
        {
            try
            {
                foreach (var scan in offlineScans)
                {
                    // Process based on scan type
                    switch (scan.ScanType)
                    {
                        case "InventoryCount":
                            await ProcessInventoryCountScanAsync(scan);
                            break;
                        case "Issue":
                            await ProcessIssueScanAsync(scan);
                            break;
                        case "Receive":
                            await ProcessReceiveScanAsync(scan);
                            break;
                    }
                }

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing offline scans");
                return false;
            }
        }
        public async Task<StockRotationDto> GetStockRotationSuggestionAsync(int itemId, int storeId, string method = "FIFO")
        {
            var storeItems = await _unitOfWork.StockEntryItems
                .FindAsync(sei => sei.Item.Id == itemId && sei.StockEntry.StoreId == storeId);

            IEnumerable<StockEntryItem> sortedItems = method switch
            {
                "FIFO" => storeItems.OrderBy(s => s.CreatedAt),           // First In First Out
                "LIFO" => storeItems.OrderByDescending(s => s.CreatedAt), // Last In First Out
                "FEFO" => storeItems.OrderBy(s => s.ExpiryDate),          // First Expired First Out
                _ => storeItems.OrderBy(s => s.CreatedAt)
            };

            var suggestions = sortedItems.Select(item => new StockRotationItemDto
            {
                BatchNumber = item.BatchNumber,
                Quantity = item.Quantity,
                EntryDate = item.CreatedAt,
                ExpiryDate = item.ExpiryDate,
                Location = item.Location,
                Priority = method == "FEFO" && item.ExpiryDate.HasValue
                    ? (item.ExpiryDate.Value - DateTime.Now).Days < 30 ? "High" : "Normal"
                    : "Normal"
            }).ToList();

            return new StockRotationDto
            {
                ItemId = itemId,
                StoreId = storeId,
                Method = method,
                SuggestedOrder = suggestions,
                GeneratedAt = DateTime.Now
            };
        }
        private string GenerateBarcodeNumber(int itemId, string batchNumber)
        {
            var timestamp = DateTime.Now.ToString("yyMMddHHmmss");
            var random = new Random().Next(100, 999);

            if (!string.IsNullOrEmpty(batchNumber))
                return $"{_barcodePrefix}{itemId:D6}{batchNumber}{random}";

            return $"{_barcodePrefix}{itemId:D6}{timestamp}{random}";
        }
        private byte[] GenerateQRCodeImage(string data)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

                // Use BitmapByteQRCode instead of QRCode for byte array generation
                using (var qrCode = new BitmapByteQRCode(qrCodeData))
                {
                    var qrCodeImage = qrCode.GetGraphic(20);
                    return qrCodeImage;
                }
            }
        }

        #pragma warning disable CA1416
        private string GenerateBarcodeImage(string barcodeNumber, string size)
        {
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = size == "Small" ? 50 : size == "Large" ? 150 : 100,
                    Width = size == "Small" ? 150 : size == "Large" ? 450 : 300,
                    Margin = 10,
                    PureBarcode = false
                }
            };
            var pixelData = barcodeWriter.Write(barcodeNumber);
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(
                        new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(
                            pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        #pragma warning restore CA1416

        private string ConvertToBase64(byte[] imageBytes)
        {
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }
        private async Task ProcessIssueScanAsync(OfflineScanDto scan)
        {
            // Process issue scan
            _logger.LogInformation($"Processing offline issue scan: {scan.BarcodeData}");
        }
        private async Task ProcessReceiveScanAsync(OfflineScanDto scan)
        {
            // Process receive scan
            _logger.LogInformation($"Processing offline receive scan: {scan.BarcodeData}");
        }
        private async Task<List<BarcodeLabel>> GenerateLabelsAsync(
            BatchPrintRequestDto request)
        {
            var labels = new List<DTOs.BarcodeLabel>();

            foreach (var item in request.Items)
            {
                var label = new DTOs.BarcodeLabel
                {
                    BarcodeNumber = await GenerateBarcodeAsync(item.ItemId, item.BatchNumber),
                    ItemName = item.ItemName,
                    ItemCode = await GetItemCodeAsync(item.ItemId),
                    BatchNumber = item.BatchNumber,
                    ExpiryDate = item.ExpiryDate,
                    Size = request.LabelSize,
                    BarcodeImage = GenerateBarcodeImage(
                        await GenerateBarcodeAsync(item.ItemId, item.BatchNumber),
                        request.LabelSize)
                };

                if (request.IncludeQRCode)
                {
                    var qrData = new
                    {
                        ItemId = item.ItemId,
                        Batch = item.BatchNumber,
                        Expiry = item.ExpiryDate
                    };
                    label.QRCodeImage = Convert.ToBase64String(
                        GenerateQRCodeImage(JsonSerializer.Serialize(qrData)));
                }

                labels.Add(label);
            }

            return labels;
        }
        private async Task<string> GetItemCodeAsync(int itemId)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(itemId);
            return item?.Code ?? string.Empty;
        }
        private async Task<bool> ValidateItemList(List<int> itemIds)
        {
            var validIds = new HashSet<int>(itemIds);
            // Use HashSet.Contains instead of IEnumerable.Contains
            return validIds.All(id => id > 0);
        }
        public async Task<bool> ProcessInventoryCountScanAsync(OfflineScanDto scan)
        {
            try
            {
                // Implementation
                await _unitOfWork.CompleteAsync();
                return true; // Add return statement
            }
            catch
            {
                return false; // Add return statement
            }
        }

        public async Task<string> CreateBatchBarcodeAsync(int batchId)
        {
            var batch = await _unitOfWork.BatchTrackings.GetByIdAsync(batchId);
            if (batch == null) throw new Exception("Batch not found");

            var barcode = $"BTH{batchId:D8}";
            // Generate barcode image and save
            return barcode;
        }
        public async Task<string> CreateItemBarcodeAsync(int itemId)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null) throw new Exception("Item not found");

            var barcode = $"ITM{itemId:D8}";
            // Generate barcode image and save
            return barcode;
        }

        public async Task<byte[]> GenerateBarcodeAsync(string code)
        {
            // Generate barcode image
            return new byte[0]; // Placeholder
        }

        public async Task<byte[]> GenerateBarcodeLabelsAsync(List<int> itemIds)
        {
            // Generate multiple barcode labels
            return new byte[0]; // Placeholder
        }

        public async Task<byte[]> GenerateQRCodeAsync(string data)
        {
            // Generate QR code
            return new byte[0]; // Placeholder
        }

        Task<string> IBarcodeService.GenerateBarcodeAsync(string data)
        {
            throw new NotImplementedException();
        }

        Task<string> IBarcodeService.GenerateQRCodeAsync(string data)
        {
            throw new NotImplementedException();
        }

        Task<BarcodeDto> IBarcodeService.CreateItemBarcodeAsync(int itemId)
        {
            throw new NotImplementedException();
        }

        Task<BarcodeDto> IBarcodeService.CreateBatchBarcodeAsync(int batchId)
        {
            throw new NotImplementedException();
        }





        #region Advanced Search and Filtering Operations

        public async Task<IEnumerable<BarcodeDto>> SearchBySerialNumberAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Enumerable.Empty<BarcodeDto>();

            try
            {
                term = term.Trim().ToLower();

                var barcodes = await _unitOfWork.Barcodes.FindAsync(b =>
                    b.IsActive && b.SerialNumber.ToLower().Contains(term));

                var barcodeDtos = new List<BarcodeDto>();
                foreach (var barcode in barcodes)
                {
                    var dto = await GetBarcodeByIdAsync(barcode.Id);
                    if (dto != null)
                        barcodeDtos.Add(dto);
                }

                return barcodeDtos.OrderByDescending(b => b.GeneratedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching barcodes by serial number: {Term}", term);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetDistinctLocationsAsync()
        {
            try
            {
                var barcodes = await _unitOfWork.Barcodes.GetAllAsync();
                var locations = barcodes
                    .Where(b => !string.IsNullOrWhiteSpace(b.Location))
                    .Select(b => b.Location)
                    .Distinct()
                    .OrderBy(l => l)
                    .ToList();

                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct locations");
                throw;
            }
        }

        public async Task<IEnumerable<BarcodeDto>> GetFilteredBarcodesAsync(BarcodeFilterDto filters)
        {
            try
            {
                // Start with all active barcodes
                var allBarcodes = await GetAllBarcodesAsync();
                var filteredBarcodes = allBarcodes.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(filters.Search))
                {
                    var searchTerm = filters.Search.ToLower();
                    filteredBarcodes = filteredBarcodes.Where(b =>
                        b.BarcodeNumber.ToLower().Contains(searchTerm) ||
                        (b.SerialNumber != null && b.SerialNumber.ToLower().Contains(searchTerm)) ||
                        (b.ItemName != null && b.ItemName.ToLower().Contains(searchTerm)) ||
                        (b.ItemCode != null && b.ItemCode.ToLower().Contains(searchTerm)));
                }

                if (filters.StoreId.HasValue)
                {
                    filteredBarcodes = filteredBarcodes.Where(b => b.StoreId == filters.StoreId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filters.BarcodeType))
                {
                    filteredBarcodes = filteredBarcodes.Where(b => b.BarcodeType == filters.BarcodeType);
                }

                if (!string.IsNullOrWhiteSpace(filters.ReferenceType))
                {
                    filteredBarcodes = filteredBarcodes.Where(b => b.ReferenceType == filters.ReferenceType);
                }

                if (!string.IsNullOrWhiteSpace(filters.Location))
                {
                    filteredBarcodes = filteredBarcodes.Where(b =>
                        b.Location != null && b.Location.Contains(filters.Location));
                }

                if (filters.CategoryId.HasValue)
                {
                    filteredBarcodes = filteredBarcodes.Where(b => b.CategoryId == filters.CategoryId.Value);
                }

                if (filters.DateFrom.HasValue)
                {
                    filteredBarcodes = filteredBarcodes.Where(b => b.GeneratedDate >= filters.DateFrom.Value);
                }

                if (filters.DateTo.HasValue)
                {
                    filteredBarcodes = filteredBarcodes.Where(b => b.GeneratedDate <= filters.DateTo.Value);
                }

                // Apply scan status filter
                if (!string.IsNullOrWhiteSpace(filters.ScanStatus))
                {
                    switch (filters.ScanStatus.ToLower())
                    {
                        case "scanned":
                            filteredBarcodes = filteredBarcodes.Where(b => b.ScanCount > 0);
                            break;
                        case "unscanned":
                            filteredBarcodes = filteredBarcodes.Where(b => b.ScanCount == 0 || b.ScanCount == null);
                            break;
                        case "recent":
                            var recentDate = DateTime.Now.AddDays(-7);
                            filteredBarcodes = filteredBarcodes.Where(b => b.LastScannedDate >= recentDate);
                            break;
                    }
                }

                // Apply sorting
                switch (filters.SortBy?.ToLower())
                {
                    case "barcodenumber":
                        filteredBarcodes = filters.Order == "asc" ?
                            filteredBarcodes.OrderBy(b => b.BarcodeNumber) :
                            filteredBarcodes.OrderByDescending(b => b.BarcodeNumber);
                        break;
                    case "itemname":
                        filteredBarcodes = filters.Order == "asc" ?
                            filteredBarcodes.OrderBy(b => b.ItemName) :
                            filteredBarcodes.OrderByDescending(b => b.ItemName);
                        break;
                    case "printed":
                        filteredBarcodes = filters.Order == "asc" ?
                            filteredBarcodes.OrderBy(b => b.PrintCount) :
                            filteredBarcodes.OrderByDescending(b => b.PrintCount);
                        break;
                    case "scanned":
                        filteredBarcodes = filters.Order == "asc" ?
                            filteredBarcodes.OrderBy(b => b.ScanCount) :
                            filteredBarcodes.OrderByDescending(b => b.ScanCount);
                        break;
                    default: // GeneratedDate
                        filteredBarcodes = filters.Order == "asc" ?
                            filteredBarcodes.OrderBy(b => b.GeneratedDate) :
                            filteredBarcodes.OrderByDescending(b => b.GeneratedDate);
                        break;
                }

                return filteredBarcodes.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered barcodes");
                throw;
            }
        }

        #endregion

        #region Export Operations

        public async Task<byte[]> ExportToExcelAsync(IEnumerable<BarcodeDto> barcodes)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Barcodes");

                    // Headers
                    worksheet.Cells[1, 1].Value = "Barcode Number";
                    worksheet.Cells[1, 2].Value = "Serial Number";
                    worksheet.Cells[1, 3].Value = "Item Name";
                    worksheet.Cells[1, 4].Value = "Item Code";
                    worksheet.Cells[1, 5].Value = "Category";
                    worksheet.Cells[1, 6].Value = "Store Name";
                    worksheet.Cells[1, 7].Value = "Location";
                    worksheet.Cells[1, 8].Value = "Barcode Type";
                    worksheet.Cells[1, 9].Value = "Reference Type";
                    worksheet.Cells[1, 10].Value = "Print Count";
                    worksheet.Cells[1, 11].Value = "Scan Count";
                    worksheet.Cells[1, 12].Value = "Generated Date";
                    worksheet.Cells[1, 13].Value = "Generated By";
                    worksheet.Cells[1, 14].Value = "Last Scanned";

                    // Style headers
                    using (var range = worksheet.Cells[1, 1, 1, 14])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Data
                    int row = 2;
                    foreach (var barcode in barcodes)
                    {
                        worksheet.Cells[row, 1].Value = barcode.BarcodeNumber;
                        worksheet.Cells[row, 2].Value = barcode.SerialNumber ?? "";
                        worksheet.Cells[row, 3].Value = barcode.ItemName ?? "";
                        worksheet.Cells[row, 4].Value = barcode.ItemCode ?? "";
                        worksheet.Cells[row, 5].Value = barcode.CategoryName ?? "";
                        worksheet.Cells[row, 6].Value = barcode.StoreName ?? "";
                        worksheet.Cells[row, 7].Value = barcode.Location ?? "";
                        worksheet.Cells[row, 8].Value = barcode.BarcodeType ?? "CODE128";
                        worksheet.Cells[row, 9].Value = barcode.ReferenceType ?? "";
                        worksheet.Cells[row, 10].Value = barcode.PrintCount;
                        worksheet.Cells[row, 11].Value = barcode.ScanCount ?? 0;
                        worksheet.Cells[row, 12].Value = barcode.GeneratedDate.ToString("yyyy-MM-dd HH:mm");
                        worksheet.Cells[row, 13].Value = barcode.GeneratedBy ?? "";
                        worksheet.Cells[row, 14].Value = barcode.LastScannedDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never";
                        row++;
                    }

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting barcodes to Excel");
                throw;
            }
        }

        public async Task<byte[]> ExportToPDFAsync(IEnumerable<BarcodeDto> barcodes)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var document = new Document(PageSize.A4.Rotate());
                    var writer = PdfWriter.GetInstance(document, memoryStream);

                    document.Open();

                    // Title
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                    var title = new Paragraph("Barcode Report", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    document.Add(new Paragraph(" ")); // Empty line

                    // Info
                    var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    document.Add(new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}", infoFont));
                    document.Add(new Paragraph($"Total Records: {barcodes.Count()}", infoFont));
                    document.Add(new Paragraph(" ")); // Empty line

                    // Table
                    var table = new PdfPTable(8);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 15, 15, 20, 10, 10, 10, 10, 10 });

                    // Headers
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
                    table.AddCell(new PdfPCell(new Phrase("Barcode Number", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Item Name", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Store/Location", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Type", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Printed", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Scanned", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Generated", headerFont)));
                    table.AddCell(new PdfPCell(new Phrase("Status", headerFont)));

                    // Data
                    var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
                    foreach (var barcode in barcodes)
                    {
                        table.AddCell(new PdfPCell(new Phrase(barcode.BarcodeNumber ?? "", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(barcode.ItemName ?? "", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase($"{barcode.StoreName ?? ""} / {barcode.Location ?? ""}", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(barcode.BarcodeType ?? "CODE128", dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(barcode.PrintCount.ToString(), dataFont)));
                        table.AddCell(new PdfPCell(new Phrase((barcode.ScanCount ?? 0).ToString(), dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(barcode.GeneratedDate.ToString("MM/dd/yyyy"), dataFont)));
                        table.AddCell(new PdfPCell(new Phrase(barcode.ScanCount > 0 ? "Active" : "Unused", dataFont)));
                    }

                    document.Add(table);
                    document.Close();

                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting barcodes to PDF");
                throw;
            }
        }

        public async Task<byte[]> ExportToCSVAsync(IEnumerable<BarcodeDto> barcodes)
        {
            try
            {
                var csv = new StringBuilder();

                // Headers
                csv.AppendLine("Barcode Number,Serial Number,Item Name,Item Code,Category,Store Name,Location,Barcode Type,Reference Type,Print Count,Scan Count,Generated Date,Generated By,Last Scanned");

                // Data
                foreach (var barcode in barcodes)
                {
                    csv.AppendLine($"\"{barcode.BarcodeNumber ?? ""}\"," +
                                  $"\"{barcode.SerialNumber ?? ""}\"," +
                                  $"\"{barcode.ItemName ?? ""}\"," +
                                  $"\"{barcode.ItemCode ?? ""}\"," +
                                  $"\"{barcode.CategoryName ?? ""}\"," +
                                  $"\"{barcode.StoreName ?? ""}\"," +
                                  $"\"{barcode.Location ?? ""}\"," +
                                  $"\"{barcode.BarcodeType ?? "CODE128"}\"," +
                                  $"\"{barcode.ReferenceType ?? ""}\"," +
                                  $"{barcode.PrintCount}," +
                                  $"{barcode.ScanCount ?? 0}," +
                                  $"\"{barcode.GeneratedDate:yyyy-MM-dd HH:mm}\"," +
                                  $"\"{barcode.GeneratedBy ?? ""}\"," +
                                  $"\"{barcode.LastScannedDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never"}\"");
                }

                return Encoding.UTF8.GetBytes(csv.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting barcodes to CSV");
                throw;
            }
        }

        #endregion
    }
}
