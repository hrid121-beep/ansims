using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using System.Text.Json;

namespace IMS.Application.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfigurationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetSettingAsync(string key)
        {
            var setting = await _unitOfWork.Settings.FirstOrDefaultAsync(s => s.Key == key && s.IsActive);
            return setting?.Value;
        }

        public async Task SetSettingAsync(string key, string value)
        {
            var setting = await _unitOfWork.Settings.FirstOrDefaultAsync(s => s.Key == key);

            if (setting != null)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.Now;
                setting.UpdatedBy = "System"; // TODO: Get from current user
                _unitOfWork.Settings.Update(setting);
            }
            else
            {
                // Use fully qualified name to avoid ambiguity
                setting = new IMS.Domain.Entities.Setting
                {
                    Key = key,
                    Value = value,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "System" // TODO: Get from current user
                };
                await _unitOfWork.Settings.AddAsync(setting);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<T> GetSettingAsync<T>(string key)
        {
            var value = await GetSettingAsync(key);
            if (string.IsNullOrEmpty(value))
                return default(T);

            try
            {
                // Handle basic types
                if (typeof(T) == typeof(string))
                    return (T)(object)value;
                if (typeof(T) == typeof(int))
                    return (T)(object)int.Parse(value);
                if (typeof(T) == typeof(bool))
                    return (T)(object)bool.Parse(value);
                if (typeof(T) == typeof(decimal))
                    return (T)(object)decimal.Parse(value);
                if (typeof(T) == typeof(DateTime))
                    return (T)(object)DateTime.Parse(value);

                // For complex types, deserialize from JSON
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return default(T);
            }
        }

        public async Task SetSettingAsync<T>(string key, T value)
        {
            string stringValue;

            // Handle basic types
            if (value is string || value is int || value is bool || value is decimal || value is DateTime)
            {
                stringValue = value.ToString();
            }
            else
            {
                // For complex types, serialize to JSON
                stringValue = JsonSerializer.Serialize(value);
            }

            await SetSettingAsync(key, stringValue);
        }

        public async Task<IEnumerable<SettingDto>> GetAllSettingsAsync()
        {
            var settings = await _unitOfWork.Settings.FindAsync(s => s.IsActive);
            return settings.Select(s => new SettingDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                Description = s.Description,
                Category = s.Category
            });
        }

        public async Task UpdateSettingsAsync(Dictionary<string, string> settings)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var kvp in settings)
                {
                    await SetSettingAsync(kvp.Key, kvp.Value);
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> SettingExistsAsync(string key)
        {
            var setting = await _unitOfWork.Settings.FirstOrDefaultAsync(s => s.Key == key && s.IsActive);
            return setting != null;
        }
    }
}