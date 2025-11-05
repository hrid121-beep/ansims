// Application/Services/StoreConfigurationService.cs
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class StoreConfigurationService : IStoreConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StoreConfigurationService> _logger;

        public StoreConfigurationService(
            IUnitOfWork unitOfWork,
            ILogger<StoreConfigurationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<StoreConfigurationDto>> GetAllConfigurationsAsync()
        {
            try
            {
                var configs = await _unitOfWork.StoreConfigurations.GetAllAsync();
                var configDtos = new List<StoreConfigurationDto>();

                foreach (var config in configs)
                {
                    configDtos.Add(await MapToDtoAsync(config));
                }

                return configDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all configurations");
                throw;
            }
        }

        public async Task<IEnumerable<StoreConfigurationDto>> GetStoreConfigurationsAsync(int storeId)
        {
            try
            {
                var configs = await _unitOfWork.StoreConfigurations
                    .FindAsync(c => c.StoreId == storeId);

                var configDtos = new List<StoreConfigurationDto>();
                foreach (var config in configs)
                {
                    configDtos.Add(await MapToDtoAsync(config));
                }

                return configDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving store configurations");
                throw;
            }
        }

        public async Task<StoreConfigurationDto> GetConfigurationByIdAsync(int id)
        {
            try
            {
                var config = await _unitOfWork.StoreConfigurations.GetByIdAsync(id);
                if (config == null) return null;

                return await MapToDtoAsync(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving configuration by id");
                throw;
            }
        }

        public async Task<StoreConfigurationDto> CreateConfigurationAsync(StoreConfigurationDto dto)
        {
            try
            {
                // Check if configuration already exists
                var existing = await _unitOfWork.StoreConfigurations
                    .FirstOrDefaultAsync(c => c.StoreId == dto.StoreId && c.ConfigKey == dto.ConfigKey);

                if (existing != null)
                    throw new InvalidOperationException($"Configuration with key '{dto.ConfigKey}' already exists for this store");

                var config = new StoreConfiguration
                {
                    StoreId = dto.StoreId,
                    ConfigKey = dto.ConfigKey,
                    ConfigValue = dto.ConfigValue,
                    Description = dto.Description,
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.CreatedBy ?? "System"
                };

                await _unitOfWork.StoreConfigurations.AddAsync(config);
                await _unitOfWork.CompleteAsync();

                return await GetConfigurationByIdAsync(config.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating configuration");
                throw;
            }
        }

        public async Task UpdateConfigurationAsync(StoreConfigurationDto dto)
        {
            try
            {
                var config = await _unitOfWork.StoreConfigurations.GetByIdAsync(dto.Id);
                if (config == null)
                    throw new InvalidOperationException("Configuration not found");

                config.ConfigValue = dto.ConfigValue;
                config.Description = dto.Description;
                config.UpdatedAt = DateTime.Now;
                config.UpdatedBy = dto.CreatedBy ?? "System";

                _unitOfWork.StoreConfigurations.Update(config);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating configuration");
                throw;
            }
        }

        public async Task DeleteConfigurationAsync(int id)
        {
            try
            {
                var config = await _unitOfWork.StoreConfigurations.GetByIdAsync(id);
                if (config == null)
                    throw new InvalidOperationException("Configuration not found");

                _unitOfWork.StoreConfigurations.Remove(config);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting configuration");
                throw;
            }
        }

        public async Task<string> GetConfigValueAsync(int? storeId, string configKey)
        {
            try
            {
                var config = await _unitOfWork.StoreConfigurations
                    .FirstOrDefaultAsync(c => c.StoreId == storeId && c.ConfigKey == configKey);

                return config?.ConfigValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting config value");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAvailableConfigKeysAsync()
        {
            // Return predefined configuration keys that can be used
            return await Task.FromResult(new List<string>
            {
                "LOW_STOCK_THRESHOLD",
                "AUTO_REORDER_ENABLED",
                "REORDER_LEAD_TIME_DAYS",
                "MAX_ISSUE_QUANTITY",
                "ALLOW_NEGATIVE_STOCK",
                "STOCK_VALUATION_METHOD",
                "REQUIRE_APPROVAL_FOR_ISSUE",
                "REQUIRE_APPROVAL_FOR_RECEIVE",
                "ENABLE_BATCH_TRACKING",
                "ENABLE_SERIAL_TRACKING",
                "ENABLE_EXPIRY_TRACKING",
                "EXPIRY_ALERT_DAYS",
                "OPERATING_HOURS",
                "INVENTORY_COUNT_FREQUENCY",
                "ENABLE_BARCODE_SCANNING",
                "DEFAULT_UNIT_OF_MEASURE",
                "TEMPERATURE_CONTROLLED",
                "TEMPERATURE_MIN",
                "TEMPERATURE_MAX",
                "SECURITY_LEVEL"
            });
        }

        public async Task<Dictionary<string, string>> GetAllStoreConfigsAsync(int? storeId)
        {
            try
            {
                var configs = await _unitOfWork.StoreConfigurations
                    .FindAsync(c => c.StoreId == storeId);

                return configs.ToDictionary(c => c.ConfigKey, c => c.ConfigValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all store configs");
                throw;
            }
        }

        public async Task BulkUpdateConfigurationsAsync(int? storeId, Dictionary<string, string> configs)
        {
            try
            {
                foreach (var kvp in configs)
                {
                    var existing = await _unitOfWork.StoreConfigurations
                        .FirstOrDefaultAsync(c => c.StoreId == storeId && c.ConfigKey == kvp.Key);

                    if (existing != null)
                    {
                        existing.ConfigValue = kvp.Value;
                        existing.UpdatedAt = DateTime.Now;
                        _unitOfWork.StoreConfigurations.Update(existing);
                    }
                    else
                    {
                        var newConfig = new StoreConfiguration
                        {
                            StoreId = storeId,
                            ConfigKey = kvp.Key,
                            ConfigValue = kvp.Value,
                            CreatedAt = DateTime.Now,
                            CreatedBy = "System"
                        };
                        await _unitOfWork.StoreConfigurations.AddAsync(newConfig);
                    }
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating configurations");
                throw;
            }
        }

        public async Task CopyConfigurationsAsync(int sourceStoreId, int targetStoreId)
        {
            try
            {
                var sourceConfigs = await _unitOfWork.StoreConfigurations
                    .FindAsync(c => c.StoreId == sourceStoreId);

                foreach (var config in sourceConfigs)
                {
                    var existing = await _unitOfWork.StoreConfigurations
                        .FirstOrDefaultAsync(c => c.StoreId == targetStoreId && c.ConfigKey == config.ConfigKey);

                    if (existing == null)
                    {
                        var newConfig = new StoreConfiguration
                        {
                            StoreId = targetStoreId,
                            ConfigKey = config.ConfigKey,
                            ConfigValue = config.ConfigValue,
                            Description = config.Description,
                            CreatedAt = DateTime.Now,
                            CreatedBy = "System"
                        };
                        await _unitOfWork.StoreConfigurations.AddAsync(newConfig);
                    }
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying configurations");
                throw;
            }
        }

        // Private helper method - FIXED
        private async Task<StoreConfigurationDto> MapToDtoAsync(StoreConfiguration config)
        {
            if (config == null) return null;

            var store = await _unitOfWork.Stores.GetByIdAsync(config.StoreId);

            return new StoreConfigurationDto
            {
                Id = config.Id,
                StoreId = config.StoreId ?? 0,
                StoreName = store?.Name ?? "Unknown",
                ConfigKey = config.ConfigKey,
                ConfigValue = config.ConfigValue,
                Description = config.Description,
                CreatedDate = config.CreatedAt,
                CreatedBy = config.CreatedBy
            };
        }

        // Alternative simple MapToDto without async
        private StoreConfigurationDto MapToDto(StoreConfiguration config)
        {
            if (config == null) return null;

            return new StoreConfigurationDto
            {
                Id = config.Id,
                StoreId = config.StoreId ?? 0,
                StoreName = config.Store?.Name ?? "Store " + config.StoreId,
                ConfigKey = config.ConfigKey,
                ConfigValue = config.ConfigValue,
                Description = config.Description,
                CreatedDate = config.CreatedAt,
                CreatedBy = config.CreatedBy
            };
        }
    }
}