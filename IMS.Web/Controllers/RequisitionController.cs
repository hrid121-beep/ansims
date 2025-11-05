using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class RequisitionController : Controller
    {
        private readonly IRequisitionService _requisitionService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;
        private readonly IStockService _stockService;
        private readonly IUserContext _userContext;

        public RequisitionController(
            IRequisitionService requisitionService,
            IStoreService storeService,
            IItemService itemService,
            IStockService stockService,
            IUserContext userContext)
        {
            _requisitionService = requisitionService;
            _storeService = storeService;
            _itemService = itemService;
            _stockService = stockService;
            _userContext = userContext;
        }

        // GET: Requisition
        public async Task<IActionResult> Index()
        {
            var requisitions = await _requisitionService.GetAllRequisitionsAsync();
            return View(requisitions);
        }

        // GET: Requisition/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var requisition = await _requisitionService.GetRequisitionByIdAsync(id);
            if (requisition == null)
            {
                TempData["Error"] = "Requisition not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(requisition);
        }

        // GET: Requisition/GetDetails/5 (API endpoint for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            var requisition = await _requisitionService.GetRequisitionByIdAsync(id);
            if (requisition == null)
            {
                return NotFound();
            }
            return Json(requisition);
        }

        // GET: Requisition/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.FromStores = new SelectList(await _storeService.GetAllStoresAsync(), "Id", "Name");
            ViewBag.ToStores = new SelectList(await _storeService.GetAllStoresAsync(), "Id", "Name");
            ViewBag.Items = await _itemService.GetAllItemsAsync();
            ViewBag.Priorities = new SelectList(new[] { "Normal", "High", "Urgent", "Critical" });

            var model = new RequisitionDto
            {
                RequestDate = DateTime.Now,
                RequiredByDate = DateTime.Now.AddDays(7),
                RequestedBy = _userContext.CurrentUserName
            };

            return View(model);
        }

        // POST: Requisition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequisitionDto dto, string[] itemIds, string[] quantities, string[] specifications)
        {
            try
            {
                // Add items to DTO
                dto.Items = new List<RequisitionItemDto>();
                for (int i = 0; i < itemIds.Length; i++)
                {
                    if (!string.IsNullOrEmpty(itemIds[i]) && decimal.TryParse(quantities[i], out decimal qty) && qty > 0)
                    {
                        dto.Items.Add(new RequisitionItemDto
                        {
                            ItemId = int.Parse(itemIds[i]),
                            RequestedQuantity = qty,
                            Specification = specifications[i]
                        });
                    }
                }

                if (!dto.Items.Any())
                {
                    ModelState.AddModelError("", "Please add at least one item.");
                    await PrepareCreateViewBags();
                    return View(dto);
                }

                var result = await _requisitionService.CreateRequisitionAsync(dto);
                TempData["Success"] = $"Requisition {result.RequisitionNumber} created successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await PrepareCreateViewBags();
                return View(dto);
            }
        }

        // GET: Requisition/FromLowStock
        public async Task<IActionResult> FromLowStock()
        {
            ViewBag.Stores = new SelectList(await _storeService.GetAllStoresAsync(), "Id", "Name");

            // Get all low stock items from all stores
            var lowStockItems = new List<LowStockItemViewModel>();
            var stores = await _storeService.GetAllStoresAsync();

            foreach (var store in stores)
            {
                var storeStock = await _stockService.GetLowStockItemsAsync(store.Id);
                foreach (var item in storeStock)
                {
                    // Calculate suggested quantity properly
                    decimal currentStock = item.CurrentStock ?? 0;
                    decimal minimumStock = item.MinimumStock ?? 0;
                    decimal maximumStock = item.MaximumStock ?? (minimumStock * 2);
                    decimal suggested = maximumStock - currentStock;

                    lowStockItems.Add(new LowStockItemViewModel
                    {
                        ItemId = item.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = item.ItemName,
                        StoreId = store.Id,
                        StoreName = store.Name,
                        CurrentStock = currentStock,
                        MinimumStock = minimumStock,
                        MaximumStock = maximumStock,
                        SuggestedQuantity = Math.Max(1m, suggested) // Use 1m to specify decimal literal
                    });
                }
            }

            return View(lowStockItems);
        }

        // POST: Requisition/CreateFromLowStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromLowStock(int toStoreId, int[] selectedItems, decimal[] requestQuantities)
        {
            try
            {
                if (!selectedItems.Any())
                {
                    TempData["Error"] = "Please select at least one item.";
                    return RedirectToAction(nameof(FromLowStock));
                }

                var dto = new RequisitionDto
                {
                    RequestedBy = _userContext.CurrentUserName,
                    RequestDate = DateTime.Now,
                    RequiredByDate = DateTime.Now.AddDays(7),
                    Priority = "High",
                    Department = "Store Management",
                    Purpose = "Replenishment for low stock items",
                    ToStoreId = toStoreId,
                    AutoConvertToPO = true,
                    Notes = "Auto-generated from low stock alert",
                    Items = new List<RequisitionItemDto>()
                };

                for (int i = 0; i < selectedItems.Length; i++)
                {
                    dto.Items.Add(new RequisitionItemDto
                    {
                        ItemId = selectedItems[i],
                        RequestedQuantity = requestQuantities[i],
                        Specification = "Low stock replenishment"
                    });
                }

                var result = await _requisitionService.CreateRequisitionAsync(dto);
                TempData["Success"] = $"Requisition {result.RequisitionNumber} created from low stock items.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(FromLowStock));
            }
        }

        // POST: Requisition/Submit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id)
        {
            try
            {
                await _requisitionService.SubmitForApprovalAsync(id);
                TempData["Success"] = "Requisition submitted for approval.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Requisition/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var requisition = await _requisitionService.GetRequisitionByIdAsync(id);
            if (requisition == null)
            {
                return NotFound();
            }

            if (requisition.Status != "Draft")
            {
                TempData["Error"] = "Only draft requisitions can be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await PrepareCreateViewBags();
            return View(requisition);
        }

        // POST: Requisition/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RequisitionDto dto, string[] itemIds, string[] quantities, string[] specifications)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            try
            {
                // Check if still draft
                var existing = await _requisitionService.GetRequisitionByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                if (existing.Status != "Draft")
                {
                    TempData["Error"] = "Only draft requisitions can be edited.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Build items list from arrays
                dto.Items = new List<RequisitionItemDto>();
                for (int i = 0; i < itemIds.Length; i++)
                {
                    if (!string.IsNullOrEmpty(itemIds[i]) && decimal.TryParse(quantities[i], out decimal qty) && qty > 0)
                    {
                        dto.Items.Add(new RequisitionItemDto
                        {
                            ItemId = int.Parse(itemIds[i]),
                            RequestedQuantity = qty,
                            Specification = specifications[i]
                        });
                    }
                }

                if (!dto.Items.Any())
                {
                    ModelState.AddModelError("", "Please add at least one item.");
                    await PrepareCreateViewBags();
                    return View(dto);
                }

                // TODO: Create UpdateRequisitionAsync method in RequisitionService
                // For now, using CreateRequisitionAsync with existing ID will update
                var result = await _requisitionService.CreateRequisitionAsync(dto);

                TempData["Success"] = "Requisition updated successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await PrepareCreateViewBags();
                return View(dto);
            }
        }

        // POST: Requisition/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var requisition = await _requisitionService.GetRequisitionByIdAsync(id);
                if (requisition == null)
                {
                    TempData["Error"] = "Requisition not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (requisition.Status != "Draft")
                {
                    TempData["Error"] = "Only draft requisitions can be deleted.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Soft delete - mark as inactive
                await _requisitionService.DeleteRequisitionAsync(id);

                TempData["Success"] = $"Requisition {requisition.RequisitionNumber} deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PrepareCreateViewBags()
        {
            ViewBag.FromStores = new SelectList(await _storeService.GetAllStoresAsync(), "Id", "Name");
            ViewBag.ToStores = new SelectList(await _storeService.GetAllStoresAsync(), "Id", "Name");
            ViewBag.Items = await _itemService.GetAllItemsAsync();
            ViewBag.Priorities = new SelectList(new[] { "Normal", "High", "Urgent", "Critical" });
        }
    }

}