using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Services
{
    public class SignatoryPresetService : ISignatoryPresetService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SignatoryPresetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SignatoryPreset>> GetAllPresetsAsync()
        {
            var presets = await _unitOfWork.SignatoryPresets.GetAllAsync();
            return presets.OrderBy(p => p.DisplayOrder).ThenBy(p => p.PresetName);
        }

        public async Task<IEnumerable<SignatoryPreset>> GetActivePresetsAsync()
        {
            var presets = await _unitOfWork.SignatoryPresets.FindAsync(p => p.IsActive);
            return presets.OrderBy(p => p.DisplayOrder).ThenBy(p => p.PresetName);
        }

        public async Task<SignatoryPreset> GetPresetByIdAsync(int id)
        {
            return await _unitOfWork.SignatoryPresets.GetByIdAsync(id);
        }

        public async Task<SignatoryPreset> GetDefaultPresetAsync()
        {
            var presets = await _unitOfWork.SignatoryPresets.FindAsync(p => p.IsDefault && p.IsActive);
            return presets.FirstOrDefault();
        }

        public async Task<SignatoryPreset> CreatePresetAsync(SignatoryPreset preset)
        {
            preset.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.SignatoryPresets.AddAsync(preset);
            await _unitOfWork.CompleteAsync();
            return preset;
        }

        public async Task<SignatoryPreset> UpdatePresetAsync(SignatoryPreset preset)
        {
            preset.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.SignatoryPresets.Update(preset);
            await _unitOfWork.CompleteAsync();
            return preset;
        }

        public async Task<bool> DeletePresetAsync(int id)
        {
            var preset = await _unitOfWork.SignatoryPresets.GetByIdAsync(id);
            if (preset == null) return false;

            preset.IsActive = false;
            preset.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.SignatoryPresets.Update(preset);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> SetDefaultPresetAsync(int id)
        {
            // First, remove default flag from all presets
            var allPresets = await _unitOfWork.SignatoryPresets.GetAllAsync();
            foreach (var preset in allPresets.Where(p => p.IsDefault))
            {
                preset.IsDefault = false;
                preset.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.SignatoryPresets.Update(preset);
            }

            // Set the new default
            var newDefault = await _unitOfWork.SignatoryPresets.GetByIdAsync(id);
            if (newDefault == null) return false;

            newDefault.IsDefault = true;
            newDefault.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.SignatoryPresets.Update(newDefault);

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
