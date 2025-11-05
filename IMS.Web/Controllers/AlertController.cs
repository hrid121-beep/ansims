using IMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AlertController : Controller
{
    private readonly IStockAlertService _stockAlertService;

    public AlertController(IStockAlertService stockAlertService)
    {
        _stockAlertService = stockAlertService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,LogisticsOfficer")]
    public async Task<IActionResult> SendStockAlerts()
    {
        try
        {
            await _stockAlertService.SendLowStockEmailsAsync();
            TempData["Success"] = "Stock alert emails sent successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to send stock alerts: " + ex.Message;
        }

        return RedirectToAction("Index", "Home");
    }
}