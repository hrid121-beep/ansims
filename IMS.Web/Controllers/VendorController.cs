using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
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
    }
}