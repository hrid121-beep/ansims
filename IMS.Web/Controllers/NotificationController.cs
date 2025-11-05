using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: Notification
        [HasPermission(Permission.ViewNotifications)]
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name;
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return View(notifications);
        }

        // GET: Notification/GetRecent
        [HttpGet]
        public async Task<IActionResult> GetRecent()
        {
            var userId = User.Identity.Name;
            var notifications = await _notificationService.GetRecentNotificationsAsync(userId, 5);

            return PartialView("_NotificationDropdown", notifications);
        }

        // GET: Notification/GetUnread
        [HttpGet]
        public async Task<IActionResult> GetUnread(int count = 5)
        {
            var userId = User.Identity.Name;
            var notifications = await _notificationService.GetRecentNotificationsAsync(userId, count);

            // Filter only unread notifications
            var unreadNotifications = notifications.Where(n => !n.IsRead).ToList();

            return Json(unreadNotifications);
        }

        // GET: Notification/GetUnreadCount
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.Identity.Name;
            var count = await _notificationService.GetUnreadCountAsync(userId);

            return Json(new { count });
        }

        // POST: Notification/MarkAsRead/5
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Notification/MarkAllAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = User.Identity.Name;
                await _notificationService.MarkAllAsReadAsync(userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Notification/Delete/5
        [HttpPost]
        [HasPermission(Permission.DeleteNotifications)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Notification/DeleteAllRead
        [HttpPost]
        [HasPermission(Permission.DeleteNotifications)]
        public async Task<IActionResult> DeleteAllRead()
        {
            try
            {
                var userId = User.Identity.Name;
                await _notificationService.DeleteAllReadNotificationsAsync(userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
