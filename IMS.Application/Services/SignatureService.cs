using System;
using System.Threading.Tasks;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services
{
    public interface ISignatureService
    {
        Task<Signature> SaveSignatureAsync(string referenceType, int referenceId, string signatureType,
            string signatureData, string signerName, string signerBadgeId, string signerDesignation);
        Task<Signature> GetSignatureAsync(string referenceType, int referenceId, string signatureType);
    }

    public class SignatureService : ISignatureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SignatureService> _logger;

        public SignatureService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SignatureService> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Signature> SaveSignatureAsync(
            string referenceType,
            int referenceId,
            string signatureType,
            string signatureData,
            string signerName,
            string signerBadgeId,
            string signerDesignation)
        {
            try
            {
                // Check if signature already exists
                var existingSignature = await _unitOfWork.Signatures
                    .FirstOrDefaultAsync(s => s.ReferenceType == referenceType &&
                                             s.ReferenceId == referenceId &&
                                             s.SignatureType == signatureType);

                if (existingSignature != null)
                {
                    // Update existing signature
                    existingSignature.SignatureData = signatureData;
                    existingSignature.SignerName = signerName;
                    existingSignature.SignerBadgeId = signerBadgeId;
                    existingSignature.SignerDesignation = signerDesignation;
                    existingSignature.SignedDate = DateTime.Now;
                    existingSignature.UpdatedAt = DateTime.Now;
                    existingSignature.IPAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

                    _unitOfWork.Signatures.Update(existingSignature);
                    await _unitOfWork.CompleteAsync();

                    return existingSignature;
                }

                // Create new signature
                var signature = new Signature
                {
                    ReferenceType = referenceType,
                    ReferenceId = referenceId,
                    SignatureType = signatureType,
                    SignatureData = signatureData,
                    SignerName = signerName,
                    SignerBadgeId = signerBadgeId,
                    SignerDesignation = signerDesignation,
                    SignedDate = DateTime.Now,
                    IPAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    DeviceInfo = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.Signatures.AddAsync(signature);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Signature saved for {referenceType} #{referenceId}");

                return signature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving signature for {referenceType} #{referenceId}");
                throw;
            }
        }

        public async Task<Signature> GetSignatureAsync(string referenceType, int referenceId, string signatureType)
        {
            return await _unitOfWork.Signatures
                .FirstOrDefaultAsync(s => s.ReferenceType == referenceType &&
                                         s.ReferenceId == referenceId &&
                                         s.SignatureType == signatureType &&
                                         s.IsActive);
        }
    }
}