using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class WriteOffController : Controller
    {
        private readonly IWriteOffService _writeOffService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly IUserContext _userContext;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly INotificationService _notificationService;
        private readonly ILogger<WriteOffController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public WriteOffController(
            IWriteOffService writeOffService,
            IItemService itemService,
            IStoreService storeService,
            IUserContext userContext,
            IUserService userService,
            UserManager<User> userManager,
            IWebHostEnvironment environment,
            INotificationService notificationService,
            ILogger<WriteOffController> logger,
            IUnitOfWork unitOfWork)
        {
            _writeOffService = writeOffService ?? throw new ArgumentNullException(nameof(writeOffService));
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        #region List and Search Actions

        /// <summary>
        /// Display list of write-offs with optional filtering
        /// </summary>
        [HttpGet]
        [RequirePermission(Permission.ViewWriteOff)]
        public async Task<IActionResult> Index(string status = null, string searchTerm = null, int page = 1, int pageSize = 20)
        {
            try
            {
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.Status = status;

                IEnumerable<WriteOffDto> writeOffs;

                // Apply filters
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    writeOffs = await _writeOffService.SearchWriteOffsAsync(searchTerm);
                }
                else if (!string.IsNullOrWhiteSpace(status))
                {
                    writeOffs = await _writeOffService.GetWriteOffsByStatusAsync(status);
                }
                else
                {
                    writeOffs = await _writeOffService.GetAllWriteOffsAsync();
                }

                // Get status counts for badges
                var allWriteOffs = await _writeOffService.GetAllWriteOffsAsync();
                ViewBag.TotalCount = allWriteOffs.Count();
                ViewBag.PendingCount = allWriteOffs.Count(w => w.Status == "Pending");
                ViewBag.ApprovedCount = allWriteOffs.Count(w => w.Status == "Approved");
                ViewBag.RejectedCount = allWriteOffs.Count(w => w.Status == "Rejected");
                ViewBag.DraftCount = allWriteOffs.Count(w => w.Status == "Draft");

                // Get current user's roles for approval buttons
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(currentUser);
                    ViewBag.UserRoles = userRoles;
                    ViewBag.CurrentUserId = currentUser.Id;
                }

                // Apply pagination
                var totalItems = writeOffs.Count();
                var pagedWriteOffs = writeOffs
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                return View(pagedWriteOffs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading write-offs list");
                TempData["Error"] = "An error occurred while loading write-offs. Please try again.";
                return View(new List<WriteOffDto>());
            }
        }

        /// <summary>
        /// Display pending approvals for current user - REDIRECTS to Centralized Approval Center
        /// </summary>
        [HttpGet]
        [RequirePermission(Permission.ApproveWriteOff)]
        public IActionResult PendingApprovals()
        {
            // ✅ Redirect to Centralized Approval Center
            // All approvals (Issues, Purchases, WriteOffs, Transfers, etc.) are managed centrally
            TempData["Info"] = "All pending approvals are now managed in the Approval Center.";
            return RedirectToAction("Index", "Approval");
        }

        /// <summary>
        /// OLD PendingApprovals method - kept for reference if needed
        /// </summary>
        private async Task<IActionResult> OldPendingApprovals()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var userRoles = await _userManager.GetRolesAsync(currentUser);
                var pendingWriteOffs = new List<WriteOffDto>();

                // Get pending write-offs for each role
                foreach (var role in userRoles)
                {
                    var rolePending = await _writeOffService.GetPendingApprovalsAsync(role);
                    pendingWriteOffs.AddRange(rolePending);
                }

                // Remove duplicates
                pendingWriteOffs = pendingWriteOffs
                    .GroupBy(w => w.Id)
                    .Select(g => g.First())
                    .OrderBy(w => w.CreatedAt)
                    .ToList();

                ViewBag.Title = "Pending Approvals";
                ViewBag.IsPendingView = true;
                ViewBag.UserRoles = userRoles;
                ViewBag.CurrentUserId = currentUser.Id;

                return View("Index", pendingWriteOffs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading pending approvals");
                TempData["Error"] = "An error occurred while loading pending approvals.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Create Actions

        /// <summary>
        /// Display create write-off form
        /// </summary>
        [HttpGet]
        [RequirePermission(Permission.CreateWriteOff)]
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateViewData();

                var model = new WriteOffDto
                {
                    WriteOffDate = DateTime.Now,
                    Items = new List<WriteOffItemDto>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create write-off form");
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Process write-off creation
        /// </summary>
        // ================================================================================
        // FIX FOR WriteOffController.cs
        // Path: Web/Controllers/WriteOffController.cs
        // ================================================================================

        // REPLACE the existing Create method with this corrected version:

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.CreateWriteOff)]
        public async Task<IActionResult> Create(WriteOffDto model, List<IFormFile> attachments)
        {
            try
            {
                // Remove validation for fields that will be set server-side
                ModelState.Remove("WriteOffNo");
                ModelState.Remove("Status");
                ModelState.Remove("TotalValue");
                ModelState.Remove("CreatedBy");
                ModelState.Remove("CreatedAt");
                ModelState.Remove("ApprovalDate");  // ✅ FIX: Add this
                ModelState.Remove("ApprovedBy");
                ModelState.Remove("CanEdit");

                if (!ModelState.IsValid)
                {
                    await PopulateViewData();
                    return View(model);
                }

                // Validate items
                if (model.Items == null || !model.Items.Any())
                {
                    ModelState.AddModelError("", "Please add at least one item to write off.");
                    await PopulateViewData();
                    return View(model);
                }

                // Validate stock availability for each item
                var stockErrors = new List<string>();
                foreach (var item in model.Items)
                {
                    var currentStock = await _storeService.GetStockLevelAsync(item.ItemId, item.StoreId);
                    // FIX: Access the CurrentStock property of StockLevelDto
                    if (currentStock.CurrentStock < item.Quantity)
                    {
                        var itemDetails = await _itemService.GetItemByIdAsync(item.ItemId);
                        stockErrors.Add($"Insufficient stock for {itemDetails?.Name}. Available: {currentStock.CurrentStock}, Requested: {item.Quantity}");
                    }
                }

                if (stockErrors.Any())
                {
                    foreach (var error in stockErrors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    await PopulateViewData();
                    return View(model);
                }

                // Validate attachments
                if (attachments != null && attachments.Any())
                {
                    var maxFileSize = 10 * 1024 * 1024; // 10MB
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };

                    foreach (var file in attachments)
                    {
                        if (file.Length > maxFileSize)
                        {
                            ModelState.AddModelError("", $"File {file.FileName} exceeds 10MB limit.");
                            await PopulateViewData();
                            return View(model);
                        }

                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("", $"File type {extension} is not allowed. Allowed types: PDF, JPG, PNG, DOC, DOCX");
                            await PopulateViewData();
                            return View(model);
                        }
                    }
                }

                // Create the write-off
                var result = await _writeOffService.CreateWriteOffAsync(model, attachments);

                if (result != null)
                {
                    TempData["Success"] = result.Status == "Approved"
                        ? $"Write-off {result.WriteOffNo} created and auto-approved successfully."
                        : $"Write-off {result.WriteOffNo} created and sent for approval.";

                    return RedirectToAction(nameof(Details), new { id = result.Id });
                }

                TempData["Error"] = "Failed to create write-off. Please try again.";
                await PopulateViewData();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating write-off");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                await PopulateViewData();
                return View(model);
            }
        }


        #endregion

        #region Edit Actions

        /// <summary>
        /// Display edit write-off form
        /// </summary>
        [HttpGet]
        [RequirePermission(Permission.UpdateWriteOff)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var writeOff = await _writeOffService.GetWriteOffByIdAsync(id);
                if (writeOff == null)
                {
                    TempData["Error"] = "Write-off not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if write-off can be edited
                if (!writeOff.CanEdit)
                {
                    TempData["Error"] = "This write-off cannot be edited in its current status.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Check permission - only creator or admin can edit
                var currentUserId = _userContext.GetCurrentUserId();
                var currentUser = await _userManager.GetUserAsync(User);
                var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

                if (writeOff.CreatedBy != currentUser?.UserName && !isAdmin)
                {
                    TempData["Error"] = "You don't have permission to edit this write-off.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await PopulateViewData();
                return View(writeOff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading write-off for edit: {WriteOffId}", id);
                TempData["Error"] = "An error occurred while loading the write-off.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Process write-off update
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.UpdateWriteOff)]
        public async Task<IActionResult> Edit(int id, WriteOffDto model)
        {
            try
            {
                // Remove validation for server-set fields
                ModelState.Remove("WriteOffNo");
                ModelState.Remove("Status");
                ModelState.Remove("TotalValue");
                ModelState.Remove("CreatedBy");
                ModelState.Remove("CreatedAt");
                ModelState.Remove("UpdatedBy");
                ModelState.Remove("UpdatedAt");

                if (!ModelState.IsValid)
                {
                    await PopulateViewData();
                    return View(model);
                }

                // Validate items
                if (model.Items == null || !model.Items.Any())
                {
                    ModelState.AddModelError("", "Please add at least one item to write off.");
                    await PopulateViewData();
                    return View(model);
                }

                var result = await _writeOffService.UpdateWriteOffAsync(id, model);
                if (result != null)
                {
                    TempData["Success"] = "Write-off updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = result.Id });
                }

                TempData["Error"] = "Failed to update write-off.";
                await PopulateViewData();
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation updating write-off: {WriteOffId}", id);
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating write-off: {WriteOffId}", id);
                TempData["Error"] = "An error occurred while updating the write-off.";
                await PopulateViewData();
                return View(model);
            }
        }

        #endregion

        #region Details and Delete Actions

        /// <summary>
        /// Display write-off details
        /// </summary>
        [HttpGet]
        [RequirePermission(Permission.ViewWriteOff)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var writeOff = await _writeOffService.GetWriteOffByIdAsync(id);
                if (writeOff == null)
                {
                    TempData["Error"] = "Write-off not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if current user can approve
                var currentUserId = _userContext.GetCurrentUserId();
                var canApprove = await _writeOffService.CanUserApproveAsync(currentUserId, writeOff.TotalValue);
                ViewBag.CanApprove = canApprove && writeOff.Status == "Pending";

                // Get current user info
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    ViewBag.CurrentUserId = currentUser.Id;
                    ViewBag.CurrentUserName = currentUser.UserName;
                    ViewBag.IsCreator = writeOff.CreatedBy == currentUser.UserName;
                    ViewBag.IsAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                }

                return View(writeOff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading write-off details: {WriteOffId}", id);
                TempData["Error"] = "An error occurred while loading the write-off details.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Delete a write-off
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.DeleteWriteOff)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var writeOff = await _writeOffService.GetWriteOffByIdAsync(id);
                if (writeOff == null)
                {
                    return Json(new { success = false, message = "Write-off not found." });
                }

                // Check permission
                var currentUser = await _userManager.GetUserAsync(User);
                var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

                if (writeOff.CreatedBy != currentUser?.UserName && !isAdmin)
                {
                    return Json(new { success = false, message = "You don't have permission to delete this write-off." });
                }

                var result = await _writeOffService.DeleteWriteOffAsync(id);
                if (result)
                {
                    TempData["Success"] = "Write-off deleted successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
                }

                return Json(new { success = false, message = "Failed to delete write-off." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete write-off: {WriteOffId}", id);
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting write-off: {WriteOffId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the write-off." });
            }
        }

        #endregion

        #region Approval Actions

        /// <summary>
        /// Submit write-off for approval
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.CreateWriteOff)]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            try
            {
                var result = await _writeOffService.SubmitForApprovalAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Write-off submitted for approval successfully." });
                }

                return Json(new { success = false, message = "Failed to submit write-off for approval." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting write-off for approval: {WriteOffId}", id);
                return Json(new { success = false, message = "An error occurred while submitting for approval." });
            }
        }

        /// <summary>
        /// Approve a write-off
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.ApproveWriteOff)]
        public async Task<IActionResult> Approve(int id, string comments)
        {
            try
            {
                var currentUserId = _userContext.GetCurrentUserId();
                var writeOff = await _writeOffService.GetWriteOffByIdAsync(id);

                if (writeOff == null)
                {
                    return Json(new { success = false, message = "Write-off not found." });
                }

                // Verify user can approve
                if (!await _writeOffService.CanUserApproveAsync(currentUserId, writeOff.TotalValue))
                {
                    return Json(new { success = false, message = "You don't have permission to approve this write-off." });
                }

                var result = await _writeOffService.ApproveWriteOffAsync(id, currentUserId, comments);

                if (result)
                {
                    TempData["Success"] = "Write-off approved successfully.";
                    return Json(new { success = true, message = "Write-off approved successfully.", redirectUrl = Url.Action(nameof(Details), new { id }) });
                }

                return Json(new { success = false, message = "Failed to approve write-off." });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized approval attempt for write-off: {WriteOffId}", id);
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving write-off: {WriteOffId}", id);
                return Json(new { success = false, message = "An error occurred while approving the write-off." });
            }
        }

        /// <summary>
        /// Reject a write-off
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.ApproveWriteOff)]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    return Json(new { success = false, message = "Rejection reason is required." });
                }

                var currentUserId = _userContext.GetCurrentUserId();
                var result = await _writeOffService.RejectWriteOffAsync(id, currentUserId, reason);

                if (result)
                {
                    TempData["Success"] = "Write-off rejected successfully.";
                    return Json(new { success = true, message = "Write-off rejected successfully.", redirectUrl = Url.Action(nameof(Details), new { id }) });
                }

                return Json(new { success = false, message = "Failed to reject write-off." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting write-off: {WriteOffId}", id);
                return Json(new { success = false, message = "An error occurred while rejecting the write-off." });
            }
        }

        #endregion

        /// <summary>
        /// Get item details including unit price from StoreStock or Item
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetItemDetails(int itemId, int? storeId = null)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(itemId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Item not found." });
                }

                decimal unitPrice = item.UnitPrice ?? 0;

                // Try to get price from StoreStock (LastPurchasePrice or AveragePrice)
                if (storeId.HasValue && storeId.Value > 0)
                {
                    var storeStock = await _unitOfWork.StoreStocks
                        .SingleOrDefaultAsync(ss => ss.ItemId == itemId && ss.StoreId == storeId.Value);

                    if (storeStock != null)
                    {
                        // Prefer LastPurchasePrice, then AveragePrice, then item UnitPrice
                        if (storeStock.LastPurchasePrice.HasValue && storeStock.LastPurchasePrice.Value > 0)
                        {
                            unitPrice = storeStock.LastPurchasePrice.Value;
                            _logger.LogInformation("Using LastPurchasePrice {Price} for ItemId: {ItemId}, StoreId: {StoreId}",
                                unitPrice, itemId, storeId);
                        }
                        else if (storeStock.AveragePrice.HasValue && storeStock.AveragePrice.Value > 0)
                        {
                            unitPrice = storeStock.AveragePrice.Value;
                            _logger.LogInformation("Using AveragePrice {Price} for ItemId: {ItemId}, StoreId: {StoreId}",
                                unitPrice, itemId, storeId);
                        }
                    }
                }
                else
                {
                    // No storeId provided, try to get average price across all stores
                    var storeStocks = await _unitOfWork.StoreStocks
                        .FindAsync(ss => ss.ItemId == itemId && ss.IsActive);

                    if (storeStocks.Any())
                    {
                        // Get the most recent purchase price
                        var latestPurchasePrice = storeStocks
                            .Where(ss => ss.LastPurchasePrice.HasValue && ss.LastPurchasePrice.Value > 0)
                            .OrderByDescending(ss => ss.LastUpdated)
                            .FirstOrDefault()?.LastPurchasePrice;

                        if (latestPurchasePrice.HasValue)
                        {
                            unitPrice = latestPurchasePrice.Value;
                            _logger.LogInformation("Using latest LastPurchasePrice {Price} for ItemId: {ItemId}",
                                unitPrice, itemId);
                        }
                        else
                        {
                            // Try average price across all stores
                            var avgPrice = storeStocks
                                .Where(ss => ss.AveragePrice.HasValue && ss.AveragePrice.Value > 0)
                                .Average(ss => (decimal?)ss.AveragePrice);

                            if (avgPrice.HasValue)
                            {
                                unitPrice = avgPrice.Value;
                                _logger.LogInformation("Using average AveragePrice {Price} for ItemId: {ItemId}",
                                    unitPrice, itemId);
                            }
                        }
                    }
                }

                // If still 0, log a warning
                if (unitPrice == 0)
                {
                    _logger.LogWarning("Unit price is 0 for Item: {ItemId}. Please ensure items have unit prices set or have been purchased at least once.", itemId);
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        itemId = item.Id,
                        name = item.Name,
                        code = item.Code,
                        unit = item.Unit,
                        unitPrice = unitPrice,
                        minimumStock = item.MinimumStock
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching item details for Item: {ItemId}", itemId);
                return Json(new { success = false, message = "Error fetching item details." });
            }
        }

        /// <summary>
        /// Get current stock level
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStockLevel(int itemId, int? storeId)
        {
            try
            {
                _logger.LogInformation("GetStockLevel called - ItemId: {ItemId}, StoreId: {StoreId}", itemId, storeId);

                var stockLevel = await _storeService.GetStockLevelAsync(itemId, storeId);

                _logger.LogInformation("Stock Level Result - CurrentStock: {CurrentStock}, Available: {Available}, Reserved: {Reserved}",
                    stockLevel.CurrentStock, stockLevel.AvailableStock, stockLevel.ReservedStock);

                return Json(new {
                    success = true,
                    stockLevel = stockLevel.CurrentStock,
                    availableStock = stockLevel.AvailableStock,
                    reservedStock = stockLevel.ReservedStock,
                    debug = new {
                        itemId = itemId,
                        storeId = storeId,
                        fullObject = stockLevel
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stock level for Item: {ItemId}, Store: {StoreId}", itemId, storeId);
                return Json(new { success = false, message = ex.Message, stockLevel = 0 });
            }
        }

        /// <summary>
        /// Get approval requirement based on total value
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetApprovalRequirement(decimal totalValue)
        {
            try
            {
                var requirement = await _writeOffService.GetApprovalRequirementAsync(totalValue);
                return Json(new
                {
                    success = true,
                    requiresApproval = requirement.RequiresApproval,
                    approverRole = requirement.ApproverRole,
                    thresholdAmount = requirement.ThresholdAmount,
                    description = requirement.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching approval requirement for value: {TotalValue}", totalValue);
                return Json(new { success = false, message = "Error fetching approval requirement." });
            }
        }

        /// <summary>
        /// Get stores for specific user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserStores()
        {
            try
            {
                var userId = _userContext.GetCurrentUserId();
                var stores = await _storeService.GetUserStoresAsync(userId);

                return Json(new
                {
                    success = true,
                    stores = stores.Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        location = s.Location
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user stores");
                return Json(new { success = false, message = "Error fetching stores." });
            }
        }

        #region Report Actions

        /// <summary>
        /// Generate write-off report
        /// </summary>
        [HttpGet]
        [RequirePermission(Permission.ViewWriteOffReport)]
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate, string status = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                IEnumerable<WriteOffDto> writeOffs;

                if (!string.IsNullOrEmpty(status))
                {
                    var filtered = await _writeOffService.GetWriteOffsByStatusAsync(status);
                    writeOffs = filtered.Where(w => w.WriteOffDate >= start && w.WriteOffDate <= end);
                }
                else
                {
                    writeOffs = await _writeOffService.GetWriteOffsByDateRangeAsync(start, end);
                }

                var reportData = new WriteOffReportViewModel
                {
                    StartDate = start,
                    EndDate = end,
                    Status = status,
                    WriteOffs = writeOffs,
                    TotalCount = writeOffs.Count(),
                    TotalValue = writeOffs.Where(w => w.Status == "Approved").Sum(w => w.TotalValue),
                    ApprovedCount = writeOffs.Count(w => w.Status == "Approved"),
                    PendingCount = writeOffs.Count(w => w.Status == "Pending"),
                    RejectedCount = writeOffs.Count(w => w.Status == "Rejected"),
                    DraftCount = writeOffs.Count(w => w.Status == "Draft"),

                    // Group by reason
                    ReasonSummary = writeOffs
                        .GroupBy(w => w.Reason)
                        .Select(g => new ReasonSummaryDto
                        {
                            Reason = g.Key,
                            Count = g.Count(),
                            TotalValue = g.Sum(w => w.TotalValue)
                        })
                        .OrderByDescending(r => r.TotalValue)
                        .ToList(),

                    // Monthly summary
                    MonthlySummary = writeOffs
                        .GroupBy(w => new { w.WriteOffDate.Year, w.WriteOffDate.Month })
                        .Select(g => new MonthlySummaryDto
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Count = g.Count(),
                            TotalValue = g.Sum(w => w.TotalValue)
                        })
                        .OrderBy(m => m.Year).ThenBy(m => m.Month)
                        .ToList()
                };

                ViewBag.StatusList = new SelectList(new[]
                {
                    new { Value = "", Text = "All Status" },
                    new { Value = "Draft", Text = "Draft" },
                    new { Value = "Pending", Text = "Pending" },
                    new { Value = "Approved", Text = "Approved" },
                    new { Value = "Rejected", Text = "Rejected" }
                }, "Value", "Text", status);

                return View(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating write-off report");
                TempData["Error"] = "An error occurred while generating the report.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Print write-off document
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Print(int id)
        {
            try
            {
                var writeOff = await _writeOffService.GetWriteOffByIdAsync(id);
                if (writeOff == null)
                {
                    TempData["Error"] = "Write-off not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View("PrintTemplate", writeOff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing write-off: {WriteOffId}", id);
                TempData["Error"] = "An error occurred while preparing the print view.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// Export write-off report to PDF
        /// </summary>
        [HttpGet]
        [RequirePermission(Permission.ViewWriteOffReport)]
        public async Task<IActionResult> ExportToPdf(DateTime? startDate, DateTime? endDate, string status = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                IEnumerable<WriteOffDto> writeOffs;

                if (!string.IsNullOrEmpty(status))
                {
                    var filtered = await _writeOffService.GetWriteOffsByStatusAsync(status);
                    writeOffs = filtered.Where(w => w.WriteOffDate >= start && w.WriteOffDate <= end);
                }
                else
                {
                    writeOffs = await _writeOffService.GetWriteOffsByDateRangeAsync(start, end);
                }

                var reportData = new WriteOffReportViewModel
                {
                    StartDate = start,
                    EndDate = end,
                    Status = status,
                    WriteOffs = writeOffs,
                    TotalCount = writeOffs.Count(),
                    TotalValue = writeOffs.Where(w => w.Status == "Approved").Sum(w => w.TotalValue),
                    ApprovedCount = writeOffs.Count(w => w.Status == "Approved"),
                    PendingCount = writeOffs.Count(w => w.Status == "Pending"),
                    RejectedCount = writeOffs.Count(w => w.Status == "Rejected"),
                    DraftCount = writeOffs.Count(w => w.Status == "Draft"),
                    ReasonSummary = writeOffs
                        .GroupBy(w => w.Reason)
                        .Select(g => new ReasonSummaryDto
                        {
                            Reason = g.Key,
                            Count = g.Count(),
                            TotalValue = g.Sum(w => w.TotalValue)
                        })
                        .OrderByDescending(r => r.TotalValue)
                        .ToList(),
                    MonthlySummary = writeOffs
                        .GroupBy(w => new { w.WriteOffDate.Year, w.WriteOffDate.Month })
                        .Select(g => new MonthlySummaryDto
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Count = g.Count(),
                            TotalValue = g.Sum(w => w.TotalValue)
                        })
                        .OrderBy(m => m.Year).ThenBy(m => m.Month)
                        .ToList()
                };

                // TODO: Implement PDF generation using iTextSharp or similar library
                // The reportData object contains all necessary data for the PDF
                TempData["Info"] = "PDF export feature will be implemented soon";
                return RedirectToAction(nameof(Report), new { startDate, endDate, status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting write-off report to PDF");
                TempData["Error"] = "An error occurred while exporting to PDF.";
                return RedirectToAction(nameof(Report), new { startDate, endDate, status });
            }
        }

        /// <summary>
        /// Export write-off report to Excel
        /// </summary>
        [HttpGet]
        [RequirePermission(Permission.ViewWriteOffReport)]
        public async Task<IActionResult> ExportToExcel(DateTime? startDate, DateTime? endDate, string status = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                IEnumerable<WriteOffDto> writeOffs;

                if (!string.IsNullOrEmpty(status))
                {
                    var filtered = await _writeOffService.GetWriteOffsByStatusAsync(status);
                    writeOffs = filtered.Where(w => w.WriteOffDate >= start && w.WriteOffDate <= end);
                }
                else
                {
                    writeOffs = await _writeOffService.GetWriteOffsByDateRangeAsync(start, end);
                }

                var reportData = new WriteOffReportViewModel
                {
                    StartDate = start,
                    EndDate = end,
                    Status = status,
                    WriteOffs = writeOffs,
                    TotalCount = writeOffs.Count(),
                    TotalValue = writeOffs.Where(w => w.Status == "Approved").Sum(w => w.TotalValue),
                    ApprovedCount = writeOffs.Count(w => w.Status == "Approved"),
                    PendingCount = writeOffs.Count(w => w.Status == "Pending"),
                    RejectedCount = writeOffs.Count(w => w.Status == "Rejected"),
                    DraftCount = writeOffs.Count(w => w.Status == "Draft"),
                    ReasonSummary = writeOffs
                        .GroupBy(w => w.Reason)
                        .Select(g => new ReasonSummaryDto
                        {
                            Reason = g.Key,
                            Count = g.Count(),
                            TotalValue = g.Sum(w => w.TotalValue)
                        })
                        .OrderByDescending(r => r.TotalValue)
                        .ToList(),
                    MonthlySummary = writeOffs
                        .GroupBy(w => new { w.WriteOffDate.Year, w.WriteOffDate.Month })
                        .Select(g => new MonthlySummaryDto
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Count = g.Count(),
                            TotalValue = g.Sum(w => w.TotalValue)
                        })
                        .OrderBy(m => m.Year).ThenBy(m => m.Month)
                        .ToList()
                };

                // TODO: Implement Excel generation using EPPlus or ClosedXML
                // The reportData object contains all necessary data for the Excel file
                TempData["Info"] = "Excel export feature will be implemented soon";
                return RedirectToAction(nameof(Report), new { startDate, endDate, status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting write-off report to Excel");
                TempData["Error"] = "An error occurred while exporting to Excel.";
                return RedirectToAction(nameof(Report), new { startDate, endDate, status });
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Populate ViewData for dropdowns
        /// </summary>
        private async Task PopulateViewData()
        {
            try
            {
                // Load active items
                var items = await _itemService.GetAllItemsAsync();
                ViewBag.Items = new SelectList(
                    items.Where(i => i.IsActive).OrderBy(i => i.Name),
                    "Id",
                    "Name"
                );

                // Load user's assigned stores
                var userId = _userContext.GetCurrentUserId();
                var stores = await _storeService.GetUserStoresAsync(userId);
                ViewBag.Stores = new SelectList(
                    stores.OrderBy(s => s.Name),
                    "Id",
                    "Name"
                );

                // Load write-off reasons
                ViewBag.WriteOffReasons = new List<SelectListItem>
                {
                    new SelectListItem("-- Select Reason --", ""),
                    new SelectListItem("Damaged", "Damaged"),
                    new SelectListItem("Expired", "Expired"),
                    new SelectListItem("Lost", "Lost"),
                    new SelectListItem("Obsolete", "Obsolete"),
                    new SelectListItem("Quality Issues", "Quality Issues"),
                    new SelectListItem("Theft", "Theft"),
                    new SelectListItem("Natural Disaster", "Natural Disaster"),
                    new SelectListItem("Other", "Other")
                };

                // Load approval info
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    ViewBag.CurrentUserRoles = await _userManager.GetRolesAsync(currentUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating view data");
                // Continue with empty ViewBag data rather than throwing
            }
        }

        #endregion
    }

    #region View Models

    public class WriteOffReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public IEnumerable<WriteOffDto> WriteOffs { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalValue { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int RejectedCount { get; set; }
        public int DraftCount { get; set; }
        public List<ReasonSummaryDto> ReasonSummary { get; set; }
        public List<MonthlySummaryDto> MonthlySummary { get; set; }
    }

    public class ReasonSummaryDto
    {
        public string Reason { get; set; }
        public int Count { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class MonthlySummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
        public decimal TotalValue { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
    }

    #endregion
}