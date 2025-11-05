using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ValidationResult> ValidateStockAvailability(int itemId, int? storeId, decimal requestedQuantity)
        {
            var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(si =>
                si.ItemId == itemId && si.StoreId == storeId && si.IsActive);

            if (storeItem == null)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = "Item not found in store" };
            }

            if (storeItem.Quantity < requestedQuantity)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Insufficient stock. Available: {storeItem.Quantity}, Requested: {requestedQuantity}"
                };
            }

            return new ValidationResult { IsValid = true };
        }

        public async Task<ValidationResult> ValidatePurchaseOrder(PurchaseDto purchaseDto)
        {
            if (purchaseDto == null)
                return new ValidationResult { IsValid = false, ErrorMessage = "Purchase order data is required" };

            if (purchaseDto.Items == null || !purchaseDto.Items.Any())
                return new ValidationResult { IsValid = false, ErrorMessage = "Purchase order must contain at least one item" };

            if (purchaseDto.TotalAmount <= 0)
                return new ValidationResult { IsValid = false, ErrorMessage = "Total amount must be greater than zero" };

            // Validate vendor exists
            if (purchaseDto.VendorId > 0)
            {
                var vendor = await _unitOfWork.Vendors.GetByIdAsync(purchaseDto.VendorId);
                if (vendor == null || !vendor.IsActive)
                    return new ValidationResult { IsValid = false, ErrorMessage = "Invalid vendor" };
            }

            return new ValidationResult { IsValid = true };
        }

        public async Task<ValidationResult> ValidateIssue(IssueDto issueDto)
        {
            if (issueDto == null)
                return new ValidationResult { IsValid = false, ErrorMessage = "Issue data is required" };

            if (issueDto.Items == null || !issueDto.Items.Any())
                return new ValidationResult { IsValid = false, ErrorMessage = "Issue must contain at least one item" };

            // Validate stock availability for each item
            foreach (var item in issueDto.Items)
            {
                var stockValidation = await ValidateStockAvailability(item.ItemId, item.StoreId, item.Quantity);
                if (!stockValidation.IsValid)
                    return stockValidation;
            }

            return new ValidationResult { IsValid = true };
        }

        public async Task<ValidationResult> ValidateTransfer(TransferDto transferDto)
        {
            if (transferDto == null)
                return new ValidationResult { IsValid = false, ErrorMessage = "Transfer data is required" };

            if (transferDto.FromStoreId == transferDto.ToStoreId)
                return new ValidationResult { IsValid = false, ErrorMessage = "Cannot transfer to the same store" };

            // Validate stores exist
            var fromStore = await _unitOfWork.Stores.GetByIdAsync(transferDto.FromStoreId);
            var toStore = await _unitOfWork.Stores.GetByIdAsync(transferDto.ToStoreId);

            if (fromStore == null || !fromStore.IsActive)
                return new ValidationResult { IsValid = false, ErrorMessage = "Invalid source store" };

            if (toStore == null || !toStore.IsActive)
                return new ValidationResult { IsValid = false, ErrorMessage = "Invalid destination store" };

            // Validate stock availability
            foreach (var item in transferDto.Items)
            {
                var stockValidation = await ValidateStockAvailability(item.ItemId, transferDto.FromStoreId, item.Quantity);
                if (!stockValidation.IsValid)
                    return stockValidation;
            }

            return new ValidationResult { IsValid = true };
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

}
