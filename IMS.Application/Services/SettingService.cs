using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;

namespace IMS.Application.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SettingDto>> GetAllSettingsAsync()
        {
            var settings = await _unitOfWork.Settings.GetAllAsync();
            return settings.Select(s => new SettingDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                Description = s.Description,
                Category = s.Category
            });
        }

        public async Task<SettingDto> GetSettingByKeyAsync(string key)
        {
            var setting = await _unitOfWork.Settings.SingleOrDefaultAsync(s => s.Key == key);
            if (setting == null) return null;

            return new SettingDto
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                Category = setting.Category
            };
        }

        public async Task UpdateSettingAsync(string key, string value)
        {
            var setting = await _unitOfWork.Settings.SingleOrDefaultAsync(s => s.Key == key);
            if (setting != null)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.Now;
                setting.UpdatedBy = "System"; // Should be current user

                // Use Update method without Async
                _unitOfWork.Settings.Update(setting);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<string> GetSettingValueAsync(string key)
        {
            var setting = await _unitOfWork.Settings.SingleOrDefaultAsync(s => s.Key == key);
            return setting?.Value;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsDictionaryAsync()
        {
            var settings = await _unitOfWork.Settings.GetAllAsync();
            return settings.ToDictionary(s => s.Key, s => s.Value);
        }
    }
}