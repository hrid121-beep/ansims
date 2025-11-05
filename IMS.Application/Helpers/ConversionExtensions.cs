using IMS.Application.DTOs;
using IMS.Application.Interfaces;

namespace IMS.Application.Extensions
{
    public static class ConversionExtensions
    {
        public static int SafeValue(this int? value, int defaultValue = 0)
        {
            return value ?? defaultValue;
        }

        public static decimal SafeValue(this decimal? value, decimal defaultValue = 0)
        {
            return value ?? defaultValue;
        }

        public static DateTime SafeValue(this DateTime? value)
        {
            return value ?? DateTime.MinValue;
        }

        public static string SafeString(this object value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
    public static class ServiceExtensions
    {
        // Extension method to get active stores
        public static async Task<IEnumerable<StoreDto>> GetActiveStoresAsync(this IStoreService storeService)
        {
            var stores = await storeService.GetAllStoresAsync();
            return stores.Where(s => s.IsActive);
        }

        // Extension method to get active items
        public static async Task<IEnumerable<ItemDto>> GetActiveItemsAsync(this IItemService itemService)
        {
            var items = await itemService.GetAllItemsAsync();
            return items.Where(i => i.IsActive);
        }
    }

}