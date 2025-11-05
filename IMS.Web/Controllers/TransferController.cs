using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class TransferController : Controller
    {
        private readonly ITransferService _transferService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly IStoreItemService _storeItemService;
        private readonly ILogger<TransferController> _logger;

        public TransferController(
            ITransferService transferService,
            IItemService itemService,
            IStoreService storeService,
            IStoreItemService storeItemService,
            ILogger<TransferController> logger)
        {
            _transferService = transferService;
            _itemService = itemService;
            _storeService = storeService;
            _storeItemService = storeItemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<JsonResult> GetStoreStock(int? storeId, int itemId)
        {
            try
            {
                var stock = await _storeItemService.GetStoreItemQuantityAsync(storeId, itemId);
                return Json(new { success = true, stock = stock });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store stock");
                return Json(new { success = false, stock = 0 });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetItemDetails(int itemId)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(itemId);
                if (item == null)
                    return Json(new { success = false });

                return Json(new
                {
                    success = true,
                    itemCode = item.ItemCode,
                    itemName = item.Name,
                    unitPrice = item.UnitPrice ?? 0,
                    unit = item.Unit ?? "pcs"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item details");
                return Json(new { success = false });
            }
        }

        [HasPermission(Permission.ViewTransfer)]
        public async Task<IActionResult> Print(int id)
        {
            try
            {
                var transfer = await _transferService.GetTransferByIdAsync(id);
                if (transfer == null)
                {
                    return NotFound();
                }
                return View("~/Views/Shared/PrintTemplates/_TransferPrintTemplate.cshtml", transfer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing transfer");
                TempData["Error"] = "An error occurred while generating print view.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HasPermission(Permission.ViewTransfer)]
        public async Task<IActionResult> ByStore(int? storeId, string type = "all")
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(storeId);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                IEnumerable<TransferDto> transfers;
                switch (type.ToLower())
                {
                    case "from":
                        transfers = await _transferService.GetTransfersFromStoreAsync(storeId);
                        ViewBag.TransferType = "Outgoing";
                        break;
                    case "to":
                        transfers = await _transferService.GetTransfersToStoreAsync(storeId);
                        ViewBag.TransferType = "Incoming";
                        break;
                    default:
                        transfers = await _transferService.GetTransfersByStoreAsync(storeId);
                        ViewBag.TransferType = "All";
                        break;
                }

                ViewBag.Store = store;
                return View(transfers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading transfers by store");
                TempData["Error"] = "An error occurred while loading transfers.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task LoadViewBagData()
        {
            var items = await _itemService.GetAllItemsAsync();
            var stores = await _storeService.GetAllStoresAsync();
            
            ViewBag.Items = new SelectList(items, "Id", "Name");
            ViewBag.Stores = new SelectList(stores, "Id", "Name");
        }

        [HttpGet]
        [HasPermission(Permission.ViewTransfer)]
        public async Task<IActionResult> Index()
        {
            var transfers = await _transferService.GetAllTransfersAsync();
            return View(transfers);
        }

        [HttpGet]
        [HasPermission(Permission.CreateTransfer)]
        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();

            // Generate transfer number
            var transfers = await _transferService.GetAllTransfersAsync();
            var nextNumber = transfers.Count() + 1;
            ViewBag.TransferNo = $"TRF-{DateTime.Now:yyyyMMdd}-{nextNumber:D4}";

            return View(new TransferViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateTransfer)]
        public async Task<IActionResult> Create(TransferViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBagData();
                    var transfers = await _transferService.GetAllTransfersAsync();
                    ViewBag.TransferNo = $"TRF-{DateTime.Now:yyyyMMdd}-{(transfers.Count() + 1):D4}";
                    return View(model);
                }

                // Additional validation
                if (model.FromStoreId == model.ToStoreId)
                {
                    ModelState.AddModelError("", "Source and destination stores cannot be the same.");
                    await LoadViewBagData();
                    var transfers = await _transferService.GetAllTransfersAsync();
                    ViewBag.TransferNo = $"TRF-{DateTime.Now:yyyyMMdd}-{(transfers.Count() + 1):D4}";
                    return View(model);
                }

                if (model.Items == null || !model.Items.Any())
                {
                    ModelState.AddModelError("", "Please add at least one item to transfer.");
                    await LoadViewBagData();
                    var transfers = await _transferService.GetAllTransfersAsync();
                    ViewBag.TransferNo = $"TRF-{DateTime.Now:yyyyMMdd}-{(transfers.Count() + 1):D4}";
                    return View(model);
                }

                var dto = new TransferDto
                {
                    FromStoreId = model.FromStoreId,
                    ToStoreId = model.ToStoreId,
                    Remarks = model.Remarks,
                    Items = model.Items.Select(i => new TransferItemDto
                    {
                        ItemId = i.ItemId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                var result = await _transferService.CreateTransferRequestAsync(dto);

                if (result.Success)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
                await LoadViewBagData();
                var transfersForError = await _transferService.GetAllTransfersAsync();
                ViewBag.TransferNo = $"TRF-{DateTime.Now:yyyyMMdd}-{(transfersForError.Count() + 1):D4}";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transfer");
                TempData["Error"] = "An error occurred while creating the transfer.";
                await LoadViewBagData();
                var transfers = await _transferService.GetAllTransfersAsync();
                ViewBag.TransferNo = $"TRF-{DateTime.Now:yyyyMMdd}-{(transfers.Count() + 1):D4}";
                return View(model);
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewTransfer)]
        public async Task<IActionResult> Details(int id)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);
            if (transfer == null)
            {
                TempData["Error"] = "Transfer not found";
                return RedirectToAction(nameof(Index));
            }
            return View(transfer);
        }

        [HttpPost]
        [HasPermission(Permission.ApproveTransfer)]
        public async Task<IActionResult> Approve(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _transferService.ApproveTransferAsync(id, userId);

            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        [HasPermission(Permission.ProcessTransfer)]
        public async Task<IActionResult> Dispatch(int id)
        {
            var result = await _transferService.ProcessTransferDispatchAsync(id);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [HasPermission(Permission.ReceiveTransfer)]
        public async Task<IActionResult> Receive(int id)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);
            if (transfer == null)
            {
                TempData["Error"] = "Transfer not found";
                return RedirectToAction(nameof(Index));
            }

            // Check if status is "In Transit"
            if (transfer.Status != "In Transit")
            {
                TempData["Error"] = "Only transfers with 'In Transit' status can be received";
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new TransferReceiptViewModel
            {
                TransferId = id,
                TransferNo = transfer.TransferNo,
                FromStoreName = transfer.FromStoreName,
                ToStoreName = transfer.ToStoreName,
                TransferDate = transfer.TransferDate,
                Status = transfer.Status,
                Items = transfer.Items.Select(i => new TransferReceiptItemViewModel
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    TransferredQuantity = i.Quantity,
                    ReceivedQuantity = i.Quantity // Default to full quantity
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [HasPermission(Permission.ReceiveTransfer)]
        public async Task<IActionResult> ConfirmReceipt(TransferReceiptViewModel model)
        {
            var dto = new TransferReceiptDto
            {
                TransferId = model.TransferId,
                Items = model.Items.Select(i => new TransferReceiptItemDto
                {
                    ItemId = i.ItemId,
                    ReceivedQuantity = i.ReceivedQuantity,
                    Location = i.Location,
                    Remarks = i.Remarks
                }).ToList()
            };

            var result = await _transferService.ConfirmTransferReceiptAsync(model.TransferId, dto);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Message;
            return View("Receive", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Track(string code)
        {
            if (string.IsNullOrEmpty(code))
                return View("TrackingForm");

            var transfer = await _transferService.GetTransferByTrackingCodeAsync(code);
            if (transfer == null)
            {
                TempData["Error"] = "Invalid tracking code";
                return View("TrackingForm");
            }

            return View("TrackingDetails", transfer);
        }
    }
}