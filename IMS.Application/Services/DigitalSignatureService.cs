using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class DigitalSignatureService : IDigitalSignatureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DigitalSignatureService> _logger;

        public DigitalSignatureService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DigitalSignatureService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
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
                    // Update existing
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
                    CreatedBy = _userContext.CurrentUserName,
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

        public async Task<Signature> GetSignatureByIdAsync(int id)
        {
            return await _unitOfWork.Signatures.GetByIdAsync(id);
        }

        // Return DigitalSignatureDto version
        public async Task<DigitalSignatureDto> GetSignatureDtoByIdAsync(int id)
        {
            var signature = await _unitOfWork.DigitalSignatures.GetByIdAsync(id);
            return signature != null ? MapToDto(signature) : null;
        }

        public async Task<bool> ValidateSignatureAsync(string signatureData)
        {
            try
            {
                if (string.IsNullOrEmpty(signatureData))
                    return false;

                if (!signatureData.StartsWith("data:image"))
                    return false;

                var base64Data = signatureData.Substring(signatureData.IndexOf(',') + 1);
                var bytes = Convert.FromBase64String(base64Data);

                return await Task.FromResult(bytes.Length > 100);
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> ConvertSignatureToBase64Async(byte[] signatureBytes)
        {
            await Task.CompletedTask;
            return $"data:image/png;base64,{Convert.ToBase64String(signatureBytes)}";
        }

        private DigitalSignatureDto MapToDto(DigitalSignature signature)
        {
            return new DigitalSignatureDto
            {
                Id = signature.Id,
                ReferenceType = signature.ReferenceType,
                ReferenceId = signature.ReferenceId,
                SignatureType = signature.SignatureType,
                SignedBy = signature.SignedBy,
                SignedAt = signature.SignedAt,
                SignatureData = signature.SignatureData,
                DeviceInfo = signature.DeviceInfo,
                IPAddress = signature.IPAddress,
                LocationInfo = signature.LocationInfo,
                IsVerified = signature.IsVerified,
                VerifiedBy = signature.VerifiedBy,
                VerifiedDate = signature.VerificationDate,
                CreatedAt = signature.CreatedAt,
                CreatedBy = signature.CreatedBy
            };
        }

        public async Task<DigitalSignature> CreateSignatureAsync(DigitalSignatureDto dto)
        {
            try
            {
                // Validate signature data
                if (string.IsNullOrEmpty(dto.SignatureData))
                    throw new InvalidOperationException("Signature data is required");

                // Create signature record
                var signature = new DigitalSignature
                {
                    EntityType = dto.EntityType,
                    EntityId = dto.EntityId,
                    SignedBy = dto.SignedBy,
                    SignedDate = dto.SignedDate,
                    SignatureData = dto.SignatureData,
                    SignatureType = dto.SignatureType,

                    // Generate hash for integrity
                    SignatureHash = GenerateSignatureHash(dto.SignatureData),

                    // Capture metadata
                    IPAddress = dto.IPAddress,
                    DeviceInfo = dto.DeviceInfo,
                    DeviceId = dto.DeviceId,
                    Location = dto.Location,

                    // Verification
                    IsVerified = false,
                    VerificationCode = GenerateVerificationCode(),

                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.DigitalSignatures.AddAsync(signature);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Digital signature created for {dto.EntityType} {dto.EntityId}");

                return signature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating digital signature");
                throw;
            }
        }

        // Verify Signature
        public async Task<bool> VerifySignatureAsync(int signatureId, string verificationCode = null)
        {
            try
            {
                var signature = await _unitOfWork.DigitalSignatures
                    .FirstOrDefaultAsync(ds => ds.Id == signatureId);

                if (signature == null)
                    throw new InvalidOperationException("Signature not found");

                // If verification code provided, verify it
                if (!string.IsNullOrEmpty(verificationCode))
                {
                    if (signature.VerificationCode != verificationCode)
                        return false;
                }

                // Verify signature integrity
                var currentHash = GenerateSignatureHash(signature.SignatureData);
                if (signature.SignatureHash != currentHash)
                {
                    signature.IsVerified = false;
                    signature.VerificationFailReason = "Signature integrity check failed";
                    await _unitOfWork.CompleteAsync();
                    return false;
                }

                // Mark as verified
                signature.IsVerified = true;
                signature.VerifiedDate = DateTime.Now;
                signature.VerificationMethod = verificationCode != null ? "Code" : "Hash";

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Signature {signatureId} verified successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying signature {signatureId}");
                throw;
            }
        }

        // Get Signature by Entity
        public async Task<IEnumerable<DigitalSignatureDto>> GetSignaturesByEntityAsync(
            string entityType, int entityId)
        {
            try
            {
                var signatures = await _unitOfWork.DigitalSignatures.Query()
                    .Where(ds => ds.EntityType == entityType && ds.EntityId == entityId)
                    .OrderByDescending(ds => ds.SignedDate)
                    .Select(ds => new DigitalSignatureDto
                    {
                        Id = ds.Id,
                        EntityType = ds.EntityType,
                        EntityId = ds.EntityId,
                        SignedBy = ds.SignedBy,
                        SignedDate = ds.SignedDate,
                        SignatureData = ds.SignatureData,
                        SignatureType = ds.SignatureType,
                        IsVerified = ds.IsVerified,
                        VerifiedDate = ds.VerifiedDate
                    })
                    .ToListAsync();

                return signatures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting signatures for {entityType} {entityId}");
                throw;
            }
        }

        // Create Batch Signature (for multiple items)
        public async Task<BatchSignatureDto> CreateBatchSignatureAsync(BatchSignatureDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var batchSignature = new BatchSignature
                {
                    BatchNo = GenerateBatchNo(),
                    SignedBy = dto.SignedBy,
                    SignedDate = DateTime.Now,
                    TotalItems = dto.Items.Count,
                    Purpose = dto.Purpose,
                    SignatureData = dto.SignatureData,
                    SignatureHash = GenerateSignatureHash(dto.SignatureData),
                    Items = new List<BatchSignatureItem>()
                };

                foreach (var item in dto.Items)
                {
                    batchSignature.Items.Add(new BatchSignatureItem
                    {
                        EntityType = item.EntityType,
                        EntityId = item.EntityId,
                        ItemDescription = item.Description,
                        Quantity = item.Quantity,
                        Remarks = item.Remarks
                    });

                    // Create individual signature record
                    await CreateSignatureAsync(new DigitalSignatureDto
                    {
                        EntityType = item.EntityType,
                        EntityId = item.EntityId,
                        SignedBy = dto.SignedBy,
                        SignedDate = DateTime.Now,
                        SignatureData = dto.SignatureData,
                        SignatureType = "Batch",
                        IPAddress = dto.IPAddress,
                        DeviceInfo = dto.DeviceInfo
                    });
                }

                await _unitOfWork.BatchSignatures.AddAsync(batchSignature);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                dto.Id = batchSignature.Id;
                dto.BatchNo = batchSignature.BatchNo;

                _logger.LogInformation($"Batch signature created: {batchSignature.BatchNo}");

                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating batch signature");
                throw;
            }
        }

        // Generate Signature Image from Data
        public async Task<byte[]> GenerateSignatureImageAsync(string signatureData)
        {
            try
            {
                // Convert base64 signature data to image bytes
                if (signatureData.StartsWith("data:image"))
                {
                    // Extract base64 data from data URL
                    var base64Data = signatureData.Substring(signatureData.IndexOf(',') + 1);
                    return Convert.FromBase64String(base64Data);
                }
                else
                {
                    // Already base64 string
                    return Convert.FromBase64String(signatureData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating signature image");
                throw;
            }
        }

        // Create OTP for Signature Verification
        public async Task<string> GenerateSignatureOTPAsync(string userId, string purpose)
        {
            try
            {
                var otp = GenerateOTP();

                var otpRecord = new SignatureOTP
                {
                    UserId = userId,
                    OTPCode = otp,
                    Purpose = purpose,
                    GeneratedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(5),
                    IsUsed = false
                };

                await _unitOfWork.SignatureOTPs.AddAsync(otpRecord);
                await _unitOfWork.CompleteAsync();

                // Send OTP via SMS/Email (implementation depends on notification service)
                await SendOTPAsync(userId, otp);

                return otp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating signature OTP");
                throw;
            }
        }

        // Verify OTP
        public async Task<bool> VerifyOTPAsync(string userId, string otp, string purpose)
        {
            try
            {
                var otpRecord = await _unitOfWork.SignatureOTPs
                    .FirstOrDefaultAsync(o =>
                        o.UserId == userId &&
                        o.OTPCode == otp &&
                        o.Purpose == purpose &&
                        !o.IsUsed &&
                        o.ExpiresAt > DateTime.Now);

                if (otpRecord == null)
                    return false;

                otpRecord.IsUsed = true;
                otpRecord.UsedAt = DateTime.Now;

                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP");
                throw;
            }
        }

        // Get Signature Audit Trail
        public async Task<SignatureAuditTrailDto> GetSignatureAuditTrailAsync(
            string entityType, int entityId)
        {
            try
            {
                var signatures = await _unitOfWork.DigitalSignatures.Query()
                    .Where(ds => ds.EntityType == entityType && ds.EntityId == entityId)
                    .OrderBy(ds => ds.SignedDate)
                    .ToListAsync();

                var auditTrail = new SignatureAuditTrailDto
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    TotalSignatures = signatures.Count,
                    VerifiedSignatures = signatures.Count(s => s.IsVerified),

                    Timeline = signatures.Select(s => new SignatureTimelineDto
                    {
                        SignatureId = s.Id,
                        SignedBy = s.SignedBy,
                        SignedDate = s.SignedDate,
                        SignatureType = s.SignatureType,
                        IsVerified = s.IsVerified,
                        VerifiedDate = s.VerifiedDate,
                        IPAddress = s.IPAddress,
                        DeviceInfo = s.DeviceInfo
                    }).ToList(),

                    FirstSignedDate = signatures.FirstOrDefault()?.SignedDate,
                    LastSignedDate = signatures.LastOrDefault()?.SignedDate
                };

                return auditTrail;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting signature audit trail for {entityType} {entityId}");
                throw;
            }
        }

        // Fixed CaptureSignatureAsync method
        public async Task<DigitalSignatureDto> CaptureSignatureAsync(DigitalSignatureDto dto)
        {
            try
            {
                // Validate signature data
                if (string.IsNullOrEmpty(dto.SignatureData))
                    throw new InvalidOperationException("Signature data is required");

                // Get client information
                var httpContext = _httpContextAccessor.HttpContext;
                var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
                var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString();

                var signature = new DigitalSignature
                {
                    ReferenceType = dto.ReferenceType,
                    ReferenceId = dto.ReferenceId,
                    SignatureType = dto.SignatureType,
                    SignedBy = dto.SignedBy ?? _userContext.CurrentUserName,
                    SignedAt = DateTime.Now,
                    SignatureData = dto.SignatureData,
                    DeviceInfo = userAgent,
                    IPAddress = ipAddress,
                    LocationInfo = dto.LocationInfo,
                    IsVerified = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.DigitalSignatures.AddAsync(signature);
                await _unitOfWork.CompleteAsync();

                // Update the reference document based on type
                await UpdateReferenceDocumentAsync(dto.ReferenceType, dto.ReferenceId, dto.SignatureType);

                _logger.LogInformation($"Digital signature captured for {dto.ReferenceType} ID: {dto.ReferenceId}");

                // Return DTO instead of calling GetSignatureByIdAsync
                return MapToDto(signature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing digital signature");
                throw;
            }
        }

        // Add the missing UpdateReferenceDocumentAsync method
        private async Task UpdateReferenceDocumentAsync(string referenceType, int referenceId, string signatureType)
        {
            switch (referenceType.ToUpper())
            {
                case "ISSUE":
                    var issue = await _unitOfWork.Issues.GetByIdAsync(referenceId);
                    if (issue != null)
                    {
                        // Update signature foreign keys based on signature type
                        if (signatureType == "Issuer")
                        {
                            var sig = await GetSignatureAsync("Issue", referenceId, "Issuer");
                            if (sig != null)
                                issue.IssuerSignatureId = sig.Id;
                        }
                        else if (signatureType == "Approver")
                        {
                            var sig = await GetSignatureAsync("Issue", referenceId, "Approver");
                            if (sig != null)
                                issue.ApproverSignatureId = sig.Id;
                        }
                        else if (signatureType == "Receiver")
                        {
                            var sig = await GetSignatureAsync("Issue", referenceId, "Receiver");
                            if (sig != null)
                                issue.ReceiverSignatureId = sig.Id;
                        }

                        issue.UpdatedAt = DateTime.Now;
                        issue.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.Issues.Update(issue);
                    }
                    break;

                case "RECEIVE":
                    var receive = await _unitOfWork.Receives.GetByIdAsync(referenceId);
                    if (receive != null)
                    {
                        receive.UpdatedAt = DateTime.Now;
                        receive.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.Receives.Update(receive);
                    }
                    break;

                case "TRANSFER":
                    var transfer = await _unitOfWork.Transfers.GetByIdAsync(referenceId);
                    if (transfer != null)
                    {
                        transfer.UpdatedAt = DateTime.Now;
                        transfer.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.Transfers.Update(transfer);
                    }
                    break;

                case "RETURN":
                    var returnDoc = await _unitOfWork.Returns.GetByIdAsync(referenceId);
                    if (returnDoc != null)
                    {
                        returnDoc.UpdatedAt = DateTime.Now;
                        returnDoc.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.Returns.Update(returnDoc);
                    }
                    break;
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<DigitalSignatureDto>> GetSignaturesByReferenceAsync(string referenceType, int referenceId)
        {
            var signatures = await _unitOfWork.DigitalSignatures
                .FindAsync(ds => ds.ReferenceType == referenceType &&
                          ds.ReferenceId == referenceId &&
                          ds.IsActive);

            return signatures.Select(s => MapToDto(s)).OrderBy(s => s.SignedAt);
        }

        public async Task<bool> HasRequiredSignaturesAsync(string referenceType, int referenceId)
        {
            var requiredSignatures = GetRequiredSignatures(referenceType);
            var existingSignatures = await _unitOfWork.DigitalSignatures
                .FindAsync(ds => ds.ReferenceType == referenceType &&
                          ds.ReferenceId == referenceId &&
                          ds.IsActive);

            foreach (var required in requiredSignatures)
            {
                if (!existingSignatures.Any(s => s.SignatureType == required))
                    return false;
            }

            return true;
        }

        private List<string> GetRequiredSignatures(string referenceType)
        {
            return referenceType.ToUpper() switch
            {
                "ISSUE" => new List<string> { "Issuer", "Receiver" },
                "RECEIVE" => new List<string> { "Receiver" },
                "TRANSFER" => new List<string> { "Sender", "Receiver" },
                "RETURN" => new List<string> { "Returner", "Receiver" },
                _ => new List<string>()
            };
        }

        // Helper Methods
        private string GenerateSignatureHash(string signatureData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(signatureData);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GenerateBatchNo()
        {
            return $"BS-{DateTime.Now:yyyyMMddHHmmss}";
        }

        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task SendOTPAsync(string userId, string otp)
        {
            // Implementation for sending OTP via SMS/Email
            // This would integrate with notification service
            await Task.CompletedTask;
            _logger.LogInformation($"OTP {otp} sent to user {userId}");
        }
    }
}