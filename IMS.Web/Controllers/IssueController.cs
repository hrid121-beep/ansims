using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using IMS.Web.Extensions;  // ADD THIS for ToPagedResult
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class IssueController : Controller
    {
        private readonly IIssueService _issueService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly IBarcodeService _barcodeService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IBattalionService _battalionService;
        private readonly IRangeService _rangeService;
        private readonly IZilaService _zilaService;
        private readonly IUpazilaService _upazilaService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<IssueController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IAllotmentLetterService _allotmentLetterService;
        private readonly IVoucherService _voucherService;

        public IssueController(
            IIssueService issueService,
            IItemService itemService,
            IStoreService storeService,
            IBarcodeService barcodeService,
            IUserService userService,
            INotificationService notificationService,
            IBattalionService battalionService,
            IRangeService rangeService,
            IZilaService zilaService,
            IUpazilaService upazilaService,
            UserManager<User> userManager,
            ILogger<IssueController> logger,
            IWebHostEnvironment environment,
            IAllotmentLetterService allotmentLetterService,
            IVoucherService voucherService)
        {
            _issueService = issueService;
            _storeService = storeService;
            _barcodeService = barcodeService;
            _userService = userService;
            _notificationService = notificationService;
            _battalionService = battalionService;
            _rangeService = rangeService;
            _zilaService = zilaService;
            _upazilaService = upazilaService;
            _userManager = userManager;
            _logger = logger;
            _itemService = itemService;
            _environment = environment;
            _allotmentLetterService = allotmentLetterService;
            _voucherService = voucherService;
        }

        #region Index & List Views

        [HasPermission(Permission.ViewIssue)]
        public async Task<IActionResult> Index(string searchTerm, string status, string issueType,
            DateTime? fromDate, DateTime? toDate, int pageNumber = 1)
        {
            var result = await _issueService.GetAllIssuesAsync(pageNumber, 50, searchTerm, status, issueType, fromDate, toDate);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;
            ViewBag.IssueType = issueType;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View(result);
        }

        [HasPermission(Permission.ViewIssue)]
        public async Task<IActionResult> Details(int id)
        {
            var issue = await _issueService.GetIssueByIdAsync(id);
            if (issue == null)
            {
                TempData["Error"] = "Issue not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(issue);
        }

        [HttpGet]
        [Authorize(Roles = "Officer,Admin")]
        public IActionResult PendingApprovals(int page = 1)
        {
            // ✅ Redirect to Centralized Approval Center
            // All approvals (Issues, Purchases, Transfers, etc.) are managed centrally
            TempData["Info"] = "All pending approvals are now managed in the Approval Center.";
            return RedirectToAction("Index", "Approval");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateIssue)]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("User not found when submitting issue {IssueId} for approval", id);
                    TempData["Error"] = "User session expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                var result = await _issueService.SubmitForApprovalAsync(id, currentUser.Id);

                if (result)
                {
                    var issue = await _issueService.GetIssueByIdAsync(id);
                    await SendApprovalNotificationAsync(issue);

                    // Check if AJAX request
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new
                        {
                            success = true,
                            message = "Issue submitted for approval successfully!",
                            redirectUrl = Url.Action(nameof(Details), new { id })
                        });
                    }

                    TempData["Success"] = "Issue submitted for approval!";
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Unable to submit issue for approval." });
                    }

                    TempData["Error"] = "Unable to submit issue for approval.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting issue");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Create Issue

        [HasPermission(Permission.CreateIssue)]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user is Admin, Director, or StoreManager - they can access all stores
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Director") || User.IsInRole("StoreManager");

            IEnumerable<StoreDto> userStores;
            if (isAdmin)
            {
                // Admins can issue from any active store
                userStores = await _storeService.GetAllStoresAsync();
            }
            else
            {
                // Regular users can only issue from their assigned stores
                userStores = await _storeService.GetUserStoresAsync(currentUser.Id);
            }

            // Check if stores are available
            if (!userStores.Any())
            {
                if (isAdmin)
                {
                    TempData["Warning"] = "No stores found in the system. Please create stores first.";
                }
                else
                {
                    TempData["Warning"] = "You are not assigned to any stores. Please contact an administrator to assign you to a store.";
                }
            }

            ViewBag.IssueNo = await _issueService.GenerateIssueNoAsync();
            ViewBag.Stores = userStores;
            ViewBag.Battalions = await _battalionService.GetActiveBattalionsAsync();
            ViewBag.Ranges = await _rangeService.GetActiveRangesAsync();
            ViewBag.Zilas = await _zilaService.GetActiveZilasAsync();
            ViewBag.Upazilas = await _upazilaService.GetActiveUpazilasAsync();

            var model = new IssueDto
            {
                IssueNo = ViewBag.IssueNo,
                IssueDate = DateTime.Now,
                Items = new List<IssueItemDto>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateIssue)]
        public async Task<IActionResult> Create(IssueDto model, string action, IFormFile voucherDocument)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // STEP 1: Validate Allotment Letter BEFORE creating the issue
                    if (model.AllotmentLetterId.HasValue)
                    {
                        var isValid = await _allotmentLetterService.ValidateAllotmentLetterForIssueAsync(
                            model.AllotmentLetterId.Value,
                            model.Items.ToList()
                        );

                        if (!isValid)
                        {
                            var errorMsg = "Allotment Letter validation failed! Check quantities or expiry.";

                            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                            {
                                return Json(new { success = false, message = errorMsg });
                            }

                            TempData["Error"] = errorMsg;
                            await LoadCreateViewBag();
                            return View(model);
                        }
                    }
                    else
                    {
                        // ✅ Check if FromStoreId is provided before accessing .Value
                        if (model.FromStoreId.HasValue)
                        {
                            // Check if Allotment Letter is required for Provision Store
                            var fromStore = await _storeService.GetStoreByIdAsync(model.FromStoreId.Value);

                            if (fromStore?.StoreTypeName != null && fromStore.StoreTypeName.Contains("Provision"))
                            {
                                var warningMsg = "Allotment Letter required for issues from Provision Store!";

                                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                                {
                                    return Json(new { success = false, message = warningMsg });
                                }

                                TempData["Warning"] = warningMsg;
                                await LoadCreateViewBag();
                                return View(model);
                            }
                        }
                    }

                    // STEP 2: Set user information
                    var currentUser = await _userManager.GetUserAsync(User);
                    model.CreatedBy = currentUser.Id;
                    model.IssuedBy = currentUser.FullName ?? currentUser.UserName;

                    // STEP 3: Handle voucher document upload
                    if (voucherDocument != null && voucherDocument.Length > 0)
                    {
                        var uploadResult = await UploadVoucherDocumentAsync(voucherDocument);
                        if (uploadResult.Success)
                        {
                            model.VoucherDocumentPath = uploadResult.FilePath;
                        }
                    }

                    // STEP 4: Create the issue
                    model.Status = "Draft";
                    var result = await _issueService.CreateIssueAsync(model);

                    // STEP 5: Update Allotment Letter issued quantities
                    if (model.AllotmentLetterId.HasValue)
                    {
                        await _allotmentLetterService.UpdateIssuedQuantitiesAsync(
                            model.AllotmentLetterId.Value,
                            model.Items.ToList()
                        );
                    }

                    // STEP 6: Handle submission and response
                    // Check if this is an AJAX request
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        if (action == "submit")
                        {
                            var submitResult = await _issueService.SubmitForApprovalAsync(result.Id, currentUser.Id);

                            if (submitResult)
                            {
                                await SendApprovalNotificationAsync(result);
                                return Json(new
                                {
                                    success = true,
                                    id = result.Id,
                                    issueNo = result.IssueNo,
                                    message = "Issue created and submitted for approval successfully!",
                                    redirectUrl = Url.Action(nameof(Details), new { id = result.Id })
                                });
                            }
                            else
                            {
                                return Json(new
                                {
                                    success = true,
                                    id = result.Id,
                                    issueNo = result.IssueNo,
                                    message = "Issue created but could not be submitted for approval.",
                                    redirectUrl = Url.Action(nameof(Details), new { id = result.Id })
                                });
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                success = true,
                                id = result.Id,
                                issueNo = result.IssueNo,
                                message = "Issue saved as draft successfully!",
                                redirectUrl = Url.Action(nameof(Details), new { id = result.Id })
                            });
                        }
                    }
                    else
                    {
                        // Traditional form submission
                        if (action == "submit")
                        {
                            var submitResult = await _issueService.SubmitForApprovalAsync(result.Id, currentUser.Id);

                            if (submitResult)
                            {
                                await SendApprovalNotificationAsync(result);
                                TempData["Success"] = "Issue submitted for approval successfully!";
                            }
                            else
                            {
                                TempData["Warning"] = "Issue created but could not be submitted for approval.";
                            }
                        }
                        else
                        {
                            TempData["Success"] = "Issue saved as draft successfully!";
                        }

                        return RedirectToAction(nameof(Details), new { id = result.Id });
                    }
                }

                // If AJAX request with validation errors
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please correct the errors and try again.",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                TempData["Error"] = "Please correct the errors and try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating issue: {Message}", ex.Message);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                TempData["Error"] = $"Error: {ex.Message}";
                ModelState.AddModelError("", ex.Message);
            }

            await LoadCreateViewBag();
            return View(model);
        }

        #endregion

        #region Edit Issue

        [HttpGet]
        [HasPermission(Permission.UpdateIssue)]
        public async Task<IActionResult> Edit(int id)
        {
            var issue = await _issueService.GetIssueByIdAsync(id);
            if (issue == null || issue.Status != "Draft")
            {
                TempData["Error"] = "Issue not found or cannot be edited.";
                return RedirectToAction(nameof(Index));
            }

            await LoadEditViewBag();
            return View(issue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateIssue)]
        public async Task<IActionResult> Edit(int id, IssueDto model, string action, IFormFile voucherDocument)
        {
            if (id != model.Id) return NotFound();

            try
            {
                // Handle voucher document upload
                if (voucherDocument != null && voucherDocument.Length > 0)
                {
                    // Delete old document if exists
                    if (!string.IsNullOrEmpty(model.VoucherDocumentPath))
                    {
                        DeleteVoucherDocument(model.VoucherDocumentPath);
                    }

                    var uploadResult = await UploadVoucherDocumentAsync(voucherDocument);
                    if (uploadResult.Success)
                    {
                        model.VoucherDocumentPath = uploadResult.FilePath;
                    }
                }

                model.Status = action == "submit" ? "Pending" : "Draft";
                var result = await _issueService.UpdateIssueAsync(model);

                if (action == "submit")
                {
                    await SendApprovalNotificationAsync(result);
                    TempData["Success"] = "Issue submitted for approval!";
                }
                else
                {
                    TempData["Success"] = "Issue updated!";
                }

                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue");
                TempData["Error"] = ex.Message;
                await LoadEditViewBag();
                return View(model);
            }
        }

        #endregion

        #region Approval Workflow

        [HasPermission(Permission.ApproveIssue)]
        public async Task<IActionResult> Approve(int id)
        {
            var issue = await _issueService.GetIssueByIdAsync(id);
            if (issue == null || issue.Status != "Pending")
            {
                TempData["Error"] = "Issue not found or not pending approval.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var item in issue.Items)
            {
                var stockLevel = await _storeService.GetStockLevelAsync(item.ItemId, item.StoreId);
                item.CurrentStock = (decimal)stockLevel.CurrentStock;
            }

            return View(issue);
        }

        [HttpPost]
        [HasPermission(Permission.ApproveIssue)]
        public async Task<IActionResult> Approve(int id, string comments, DigitalSignatureDto signature)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var approvedIssue = await _issueService.ApproveIssueAsync(id, currentUser.Id, comments);

                if (signature != null && !string.IsNullOrEmpty(signature.SignatureData))
                {
                    signature.SignedDate = DateTime.Now;
                    await _issueService.AddDigitalSignatureAsync(id, signature);
                }

                var voucher = await GenerateIssueVoucherAsync(id);

                TempData["Success"] = "Issue approved successfully!";
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action(nameof(GenerateVoucher), new { id })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving issue");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [HasPermission(Permission.ApproveIssue)]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                await _issueService.RejectIssueAsync(id, currentUser.Id, reason);

                TempData["Success"] = "Issue rejected.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting issue");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateIssue)]
        public async Task<IActionResult> Submit(int id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var result = await _issueService.SubmitForApprovalAsync(id, currentUser.Id);

                if (result)
                {
                    await SendApprovalNotificationAsync(await _issueService.GetIssueByIdAsync(id));
                    TempData["Success"] = "Issue submitted for approval!";
                }
                else
                {
                    TempData["Error"] = "Unable to submit issue for approval.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting issue");
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Voucher & Handover

        [HttpGet]
        public async Task<IActionResult> GenerateVoucher(int id)
        {
            try
            {
                var voucher = await _issueService.GenerateIssueVoucherAsync(id);

                // Generate QR code
                var qrData = new
                {
                    Type = "ISSUE_VOUCHER",
                    VoucherNo = voucher.VoucherNumber,
                    IssueId = id,
                    Date = DateTime.Now.ToString("yyyyMMdd")
                };
                voucher.QRCodeImage = _barcodeService.GenerateQRCodeBase64(JsonSerializer.Serialize(qrData));

                TempData["Success"] = "Voucher generated successfully!";
                return View(voucher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating voucher for issue {IssueId}", id);
                TempData["Error"] = $"Failed to generate voucher: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrintVoucher(int id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                var voucher = await _issueService.CreateIssueWithVoucherAsync(id);

                var qrData = new
                {
                    Type = "ISSUE_VOUCHER",
                    VoucherNo = voucher.VoucherNumber,
                    IssueId = issue.Id,
                    IssueNo = issue.IssueNo,
                    Date = issue.IssueDate.ToString("yyyyMMdd"),
                    Items = issue.Items.Count
                };

                voucher.QRCodeImage = _barcodeService.GenerateQRCodeBase64(JsonSerializer.Serialize(qrData));

                var signature = await _issueService.GetDigitalSignatureAsync(id);
                if (signature != null)
                {
                    voucher.SignatureImage = signature.SignatureData;
                    voucher.ApprovedBy = signature.SignerName;
                }

                return View(voucher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing voucher");
                TempData["Error"] = "Error generating voucher.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewIssue)]
        public async Task<IActionResult> Vouchers()
        {
            try
            {
                // GetAllIssuesAsync returns PagedResult<IssueDto>
                var pagedResult = await _issueService.GetAllIssuesAsync(1, 1000);

                // Access the Items property to get the list
                var voucherIssues = pagedResult.Items
                    .Where(i =>
                        (i.Status == "Approved" || i.Status == "Issued" || i.Status == "Completed")
                        && !string.IsNullOrEmpty(i.VoucherNumber)
                    )
                    .OrderByDescending(i => i.IssueDate)
                    .ToList();

                _logger.LogInformation($"Total issues: {pagedResult.TotalCount}, With vouchers: {voucherIssues.Count}");

                return View(voucherIssues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vouchers: {Message}", ex.Message);
                TempData["Error"] = "Failed to load vouchers";
                return View(new List<IssueDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProcessHandover(int id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                if (issue == null)
                {
                    TempData["Error"] = "Issue not found!";
                    return RedirectToAction("Index");
                }

                if (issue.Status != "Approved")
                {
                    TempData["Error"] = "Issue must be approved before handover!";
                    return RedirectToAction("Details", new { id });
                }

                // ✅ FIX: Prevent duplicate handover
                if (!string.IsNullOrEmpty(issue.SignaturePath) || issue.Status == "Issued" || issue.Status == "Completed")
                {
                    TempData["Warning"] = "This issue has already been handed over! You can view the receipt.";
                    return RedirectToAction("ViewReceipt", new { id });
                }

                return View(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading handover process for issue {IssueId}", id);
                TempData["Error"] = "Failed to load handover form";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessHandover(int id, string receiverName,
            string receiverBadgeId, string receiverPhone, string receiverEmail,
            DateTime handoverDateTime, string handoverNotes, string signature)
        {
            try
            {
                // ✅ FIX: Check for duplicate handover before processing
                var existingIssue = await _issueService.GetIssueByIdAsync(id);
                if (existingIssue == null)
                {
                    TempData["Error"] = "Issue not found!";
                    return RedirectToAction("Index");
                }

                if (!string.IsNullOrEmpty(existingIssue.SignaturePath) ||
                    existingIssue.Status == "Issued" ||
                    existingIssue.Status == "Completed")
                {
                    TempData["Warning"] = "This issue has already been handed over!";
                    return RedirectToAction("ViewReceipt", new { id });
                }

                var handoverDto = new HandoverDto
                {
                    IssueId = id,
                    ReceiverName = receiverName,
                    ReceiverBadgeId = receiverBadgeId,
                    ReceiverPhone = receiverPhone,
                    ReceiverEmail = receiverEmail,
                    HandoverDate = handoverDateTime,
                    HandoverNotes = handoverNotes,
                    ReceiverSignature = signature,
                    HandedOverBy = User.Identity.Name
                };

                var result = await _issueService.CompletePhysicalHandoverAsync(id, handoverDto);

                if (result)
                {
                    TempData["Success"] = "Physical handover completed successfully!";
                    return RedirectToAction("ViewReceipt", new { id });
                }

                TempData["Error"] = "Failed to complete handover!";
                return RedirectToAction("ProcessHandover", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing handover for issue {IssueId}", id);
                TempData["Error"] = $"Handover failed: {ex.Message}";
                return RedirectToAction("ProcessHandover", new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewReceipt(int id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                if (issue == null)
                {
                    TempData["Error"] = "Issue not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing receipt for issue {IssueId}", id);
                TempData["Error"] = "Failed to load receipt";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> PhysicalHandover()
        {
            try
            {
                var issues = await _issueService.GetAllIssuesAsync();
                var readyForHandover = issues.Where(i =>
                    i.Status == "Approved" &&
                    !string.IsNullOrEmpty(i.VoucherNumber) &&
                    string.IsNullOrEmpty(i.SignaturePath)
                ).OrderBy(i => i.ApprovedDate);

                return View(readyForHandover);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading physical handover page");
                TempData["Error"] = "Failed to load handover list";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Barcode Scanning

        [HttpPost]
        public async Task<IActionResult> ScanBarcode(string barcodeNumber, int? storeId)
        {
            try
            {
                var scanResult = await _issueService.ScanItemForIssueAsync(barcodeNumber, storeId);

                if (!scanResult.IsValid)
                {
                    return Json(new
                    {
                        isValid = false,
                        validationMessage = scanResult.ValidationMessage
                    });
                }

                return Json(new
                {
                    isValid = true,
                    itemId = scanResult.ItemId,
                    itemName = scanResult.ItemName,
                    itemCode = scanResult.ItemCode,
                    currentStock = scanResult.CurrentStock,
                    unit = scanResult.Unit,
                    storeId = scanResult.StoreId,
                    storeName = scanResult.StoreName,
                    barcodeNumber = scanResult.BarcodeNumber,
                    isLowStock = scanResult.IsLowStock,
                    minimumStock = scanResult.MinimumStock
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning barcode");
                return Json(new { isValid = false, validationMessage = "Error scanning barcode" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkScan(List<string> barcodeNumbers, int? storeId)
        {
            try
            {
                var results = await _issueService.BulkScanItemsAsync(barcodeNumbers, storeId);
                return Json(new { success = true, items = results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk scan");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemDetails(int itemId, int storeId)
        {
            try
            {
                var stockLevel = await _storeService.GetStockLevelAsync(itemId, storeId);
                var item = await _storeService.GetItemDetailsAsync(itemId);

                return Json(new
                {
                    success = true,
                    currentStock = stockLevel.CurrentStock,
                    item = new
                    {
                        id = item.Id,
                        name = item.Name,
                        code = item.Code,
                        unit = item.Unit,
                        unitPrice = item.UnitPrice,
                        minimumStock = item.MinimumStock
                    },
                    isLowStock = stockLevel.IsLowStock
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item details");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Quick Issue

        [HttpGet]
        public async Task<IActionResult> QuickIssue()
        {
            try
            {
                var stores = await _storeService.GetActiveStoresAsync();
                ViewBag.Stores = new SelectList(stores, "Id", "Name");

                var items = await _itemService.GetAllItemsAsync();
                var commonItems = items.Where(i =>
                    i.CategoryName == "Uniform" ||
                    i.CategoryName == "Equipment" ||
                    i.CategoryName == "Safety Gear" ||
                    i.SubCategoryName == "Emergency"
                ).ToList();

                ViewBag.Items = new SelectList(commonItems, "Id", "Name");

                var battalions = await _battalionService.GetAllBattalionsAsync();
                ViewBag.Battalions = new SelectList(battalions, "Id", "Name");

                return View(new IssueDto { IssueDate = DateTime.Now });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading quick issue page");
                TempData["Error"] = "Failed to load quick issue form";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickIssue(IssueDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadQuickIssueDropdowns();
                    return View(model);
                }

                model.Purpose = "Quick Issue - Emergency/Urgent";
                model.Status = "Pending";
                model.CreatedBy = User.Identity.Name;
                model.CreatedAt = DateTime.Now;

                var result = await _issueService.CreateIssueAsync(model);

                if (result != null)
                {
                    await _issueService.SubmitForApprovalAsync(result.Id, User.Identity.Name);

                    TempData["Success"] = $"Quick issue {result.IssueNo} created and submitted for approval!";
                    return RedirectToAction("Details", new { id = result.Id });
                }

                TempData["Error"] = "Failed to create quick issue";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quick issue");
                TempData["Error"] = ex.Message;
                await LoadQuickIssueDropdowns();
                return View(model);
            }
        }

        #endregion

        #region Delete & Cancel

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteIssue)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var canDelete = await _issueService.CanDeleteIssueAsync(id);
                if (!canDelete)
                {
                    TempData["Error"] = "This issue cannot be deleted. Only draft issues can be deleted.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _issueService.DeleteIssueAsync(id);
                if (result)
                {
                    TempData["Success"] = "Issue deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to delete issue.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting issue");
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [HasPermission(Permission.CancelIssue)]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            try
            {
                var result = await _issueService.CancelIssueAsync(id, reason);
                if (result.Success)
                {
                    TempData["Success"] = result.Message;
                }
                else
                {
                    TempData["Error"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling issue");
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Export & Tracking

        [HttpGet]
        [HasPermission(Permission.ExportIssues)]
        public async Task<IActionResult> Export(string searchTerm, string status, string issueType, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var fileContent = await _issueService.ExportIssuesToExcelAsync(searchTerm, status, issueType, fromDate, toDate);
                var fileName = $"Issues_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting issues");
                TempData["Error"] = "Error exporting data.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Track(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return View("TrackingForm");
            }

            var issue = await _issueService.GetIssueByTrackingCode(code);
            if (issue == null)
            {
                TempData["Error"] = "Invalid tracking code";
                return View("TrackingForm");
            }

            return View("TrackingDetails", issue);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmReceipt(string trackingCode)
        {
            var result = await _issueService.ConfirmReceiptAsync(trackingCode);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpGet]
        [HasPermission(Permission.ViewIssue)]
        public IActionResult ScanQR()
        {
            return View();
        }

        // GET: Issue/SearchApproved - Search approved issues for receive linking
        [HttpGet]
        public async Task<IActionResult> SearchApproved(string term)
        {
            try
            {
                var issues = await _issueService.GetAllIssuesAsync();
                var approvedIssues = issues
                    .Where(i => i.Status == "Approved" || i.Status == "Issued")
                    .Where(i => string.IsNullOrEmpty(term) ||
                               i.IssueNo.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                               i.IssuedToName.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(i => i.IssueDate)
                    .Take(10)
                    .Select(i => new
                    {
                        id = i.Id,
                        issueNo = i.IssueNo,
                        issuedTo = i.IssuedToName,
                        issueDate = i.IssueDate.ToString("dd MMM yyyy"),
                        itemCount = i.Items?.Count ?? 0
                    });

                return Json(approvedIssues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching approved issues");
                return Json(new List<object>());
            }
        }

        [HttpPost]
        [HasPermission(Permission.ProcessIssue)]
        public async Task<IActionResult> Process(int id)
        {
            var result = await _issueService.ProcessIssueAsync(id);

            if (result.Success)
            {
                TempData["Success"] = "Issue processed and dispatched successfully";
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Helper Methods

        private async Task LoadCreateViewBag()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user is Admin, Director, or StoreManager - they can access all stores
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Director") || User.IsInRole("StoreManager");

            IEnumerable<StoreDto> userStores;
            if (isAdmin)
            {
                // Admins can issue from any active store
                userStores = await _storeService.GetAllStoresAsync();
            }
            else
            {
                // Regular users can only issue from their assigned stores
                userStores = await _storeService.GetUserStoresAsync(currentUser.Id);
            }

            ViewBag.Stores = userStores;
            ViewBag.Battalions = await _battalionService.GetActiveBattalionsAsync();
            ViewBag.Ranges = await _rangeService.GetActiveRangesAsync();
            ViewBag.Zilas = await _zilaService.GetActiveZilasAsync();
            ViewBag.Upazilas = await _upazilaService.GetActiveUpazilasAsync();
        }

        private async Task LoadEditViewBag()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var items = await _itemService.GetActiveItemsAsync();
            ViewBag.Items = items.Select(i => new
            {
                Id = i.Id,
                Name = i.Name,
                Code = i.Code ?? i.ItemCode,
                Unit = i.Unit ?? "Piece"
            }).ToList();

            // Check if user is Admin, Director, or StoreManager - they can access all stores
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Director") || User.IsInRole("StoreManager");

            IEnumerable<StoreDto> userStores;
            if (isAdmin)
            {
                userStores = await _storeService.GetAllStoresAsync();
            }
            else
            {
                userStores = await _storeService.GetUserStoresAsync(currentUser.Id);
            }

            ViewBag.Stores = userStores.Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            ViewBag.Battalions = await _battalionService.GetActiveBattalionsAsync();
            ViewBag.Ranges = await _rangeService.GetActiveRangesAsync();
            ViewBag.Zilas = await _zilaService.GetActiveZilasAsync();
            ViewBag.Upazilas = await _upazilaService.GetActiveUpazilasAsync();
        }

        private async Task LoadQuickIssueDropdowns()
        {
            var stores = await _storeService.GetActiveStoresAsync();
            ViewBag.Stores = new SelectList(stores, "Id", "Name");

            var items = await _itemService.GetAllItemsAsync();
            var commonItems = items.Where(i =>
                i.CategoryName == "Uniform" ||
                i.CategoryName == "Equipment" ||
                i.CategoryName == "Safety Gear"
            ).ToList();
            ViewBag.Items = new SelectList(commonItems, "Id", "Name");

            var battalions = await _battalionService.GetAllBattalionsAsync();
            ViewBag.Battalions = new SelectList(battalions, "Id", "Name");
        }

        private async Task SendApprovalNotificationAsync(IssueDto issue)
        {
            var notification = new NotificationDto
            {
                Title = "Issue Approval Required",
                Message = $"Issue {issue.IssueNo} requires your approval.",
                Type = "Approval",
                Priority = "High",
                RelatedEntity = "Issue",
                RelatedEntityId = issue.Id,
                TargetRole = "Officer"
            };

            await _notificationService.CreateNotificationAsync(notification);
        }

        private async Task<IssueVoucherDto> GenerateIssueVoucherAsync(int issueId)
        {
            var issue = await _issueService.GetIssueByIdAsync(issueId);
            var voucher = await _issueService.CreateIssueWithVoucherAsync(issueId);

            if (string.IsNullOrEmpty(voucher.VoucherNumber))
            {
                voucher.VoucherNumber = $"IV-{DateTime.Now:yyyyMMdd}-{issue.Id:D4}";
            }

            return voucher;
        }

        private async Task<(bool Success, string FilePath, string Message)> UploadVoucherDocumentAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, null, "No file uploaded");

                // Validate file size (5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return (false, null, "File size exceeds 5MB limit");

                // Validate file extension
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    return (false, null, "Invalid file format");

                // Create uploads directory if not exists
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "vouchers");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return (true, fileName, "File uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading voucher document");
                return (false, null, ex.Message);
            }
        }

        private void DeleteVoucherDocument(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName)) return;

                var filePath = Path.Combine(_environment.WebRootPath, "uploads", "vouchers", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting voucher document");
            }
        }

        #endregion

        #region Voucher Actions

        [HttpGet]
        [HasPermission(Permission.ViewIssue)]
        public async Task<IActionResult> DownloadVoucher(int id)
        {
            try
            {
                var pdfBytes = await _voucherService.GetIssueVoucherPdfAsync(id);
                var issue = await _issueService.GetIssueByIdAsync(id);

                string fileName = $"Issue_Voucher_{issue.VoucherNo}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading voucher for Issue {IssueId}", id);
                TempData["Error"] = "ভাউচার ডাউনলোডে সমস্যা হয়েছে।";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion
    }
}