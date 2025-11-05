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
