using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;

namespace IMS.Application.Services
{
    public class VendorService : IVendorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VendorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync()
        {
            var vendors = await _unitOfWork.Vendors.GetAllAsync();
            return vendors.Where(v => v.IsActive).Select(MapToDto);
        }

        public async Task<VendorDto> GetVendorByIdAsync(int id)
        {
            var vendor = await _unitOfWork.Vendors.GetByIdAsync(id);
            if (vendor == null || !vendor.IsActive) return null;

            return MapToDto(vendor);
        }

        public async Task<VendorDto> CreateVendorAsync(VendorDto vendorDto)
        {
            var vendor = new Vendor
            {
                Name = vendorDto.Name,
                VendorType = vendorDto.VendorType,
                ContactPerson = vendorDto.ContactPerson,
                Phone = vendorDto.Phone,
                Mobile = vendorDto.Mobile,
                Email = vendorDto.Email,
                Address = vendorDto.Address,
                TIN = vendorDto.TIN,
                BIN = vendorDto.BIN,
                CreatedAt = DateTime.Now,
                CreatedBy = "System" // TODO: Get from current user
            };

            await _unitOfWork.Vendors.AddAsync(vendor);
            await _unitOfWork.CompleteAsync();

            vendorDto.Id = vendor.Id;
            vendorDto.CreatedAt = vendor.CreatedAt;
            return vendorDto;
        }

        public async Task UpdateVendorAsync(VendorDto vendorDto)
        {
            var vendor = await _unitOfWork.Vendors.GetByIdAsync(vendorDto.Id);
            if (vendor != null)
            {
                vendor.Name = vendorDto.Name;
                vendor.VendorType = vendorDto.VendorType;
                vendor.ContactPerson = vendorDto.ContactPerson;
                vendor.Phone = vendorDto.Phone;
                vendor.Mobile = vendorDto.Mobile;
                vendor.Email = vendorDto.Email;
                vendor.Address = vendorDto.Address;
                vendor.TIN = vendorDto.TIN;
                vendor.BIN = vendorDto.BIN;
                vendor.IsActive = vendorDto.IsActive;
                vendor.UpdatedAt = DateTime.Now;
                vendor.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.Vendors.Update(vendor);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteVendorAsync(int id)
        {
            var vendor = await _unitOfWork.Vendors.GetByIdAsync(id);
            if (vendor != null)
            {
                vendor.IsActive = false;
                vendor.UpdatedAt = DateTime.Now;
                vendor.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.Vendors.Update(vendor);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<VendorDto>> GetActiveVendorsAsync()
        {
            return await GetAllVendorsAsync();
        }

        public async Task<bool> VendorExistsAsync(string name, string email, int? excludeId = null)
        {
            var vendors = await _unitOfWork.Vendors.FindAsync(
                v => (v.Name == name || v.Email == email) && v.IsActive);

            if (excludeId.HasValue)
            {
                vendors = vendors.Where(v => v.Id != excludeId.Value);
            }

            return vendors.Any();
        }

        public async Task<IEnumerable<PurchaseDto>> GetVendorPurchasesAsync(int vendorId)
        {
            var purchases = await _unitOfWork.Purchases.FindAsync(p => p.VendorId == vendorId && p.IsActive);
            var purchaseDtos = new List<PurchaseDto>();

            foreach (var purchase in purchases)
            {
                var vendor = await _unitOfWork.Vendors.GetByIdAsync(purchase.VendorId.Value);
                var purchaseItems = await _unitOfWork.PurchaseItems.FindAsync(pi => pi.PurchaseId == purchase.Id);

                var items = new List<PurchaseItemDto>();
                foreach (var pi in purchaseItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(pi.ItemId);
                    items.Add(new PurchaseItemDto
                    {
                        ItemId = pi.ItemId,
                        ItemName = item?.Name,
                        Quantity = pi.Quantity,
                        UnitPrice = pi.UnitPrice,
                        TotalPrice = pi.TotalPrice,
                        StoreId = pi.StoreId
                    });
                }

                purchaseDtos.Add(new PurchaseDto
                {
                    Id = purchase.Id,
                    PurchaseOrderNo = purchase.PurchaseOrderNo,
                    VendorId = purchase.VendorId,
                    VendorName = vendor?.Name,
                    PurchaseDate = purchase.PurchaseDate,
                    TotalAmount = purchase.TotalAmount,
                    Remarks = purchase.Remarks,
                    IsMarketplacePurchase = purchase.IsMarketplacePurchase,
                    Items = items
                });
            }

            return purchaseDtos;
        }

        public async Task<decimal> GetVendorTotalPurchaseValueAsync(int vendorId)
        {
            var purchases = await _unitOfWork.Purchases.FindAsync(p => p.VendorId == vendorId && p.IsActive);
            return purchases.Sum(p => p.TotalAmount);
        }

        public async Task<IEnumerable<VendorDto>> SearchVendorsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllVendorsAsync();

            var vendors = await GetAllVendorsAsync();
            searchTerm = searchTerm.ToLower();

            return vendors.Where(v =>
                v.Name.ToLower().Contains(searchTerm) ||
                (v.ContactPerson != null && v.ContactPerson.ToLower().Contains(searchTerm)) ||
                (v.Email != null && v.Email.ToLower().Contains(searchTerm)) ||
                (v.Phone != null && v.Phone.Contains(searchTerm)) ||
                (v.VendorType != null && v.VendorType.ToLower().Contains(searchTerm))
            );
        }

        public async Task<IEnumerable<VendorDto>> GetPagedVendorsAsync(int pageNumber, int pageSize)
        {
            var allVendors = await GetAllVendorsAsync();
            return allVendors
                .OrderBy(v => v.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        private VendorDto MapToDto(Vendor vendor)
        {
            return new VendorDto
            {
                Id = vendor.Id,
                Name = vendor.Name,
                VendorType = vendor.VendorType,
                ContactPerson = vendor.ContactPerson,
                Phone = vendor.Phone,
                Mobile = vendor.Mobile,
                Email = vendor.Email,
                Address = vendor.Address,
                TIN = vendor.TIN,
                BIN = vendor.BIN,
                IsActive = vendor.IsActive,
                CreatedAt = vendor.CreatedAt,
                CreatedBy = vendor.CreatedBy,
                UpdatedAt = vendor.UpdatedAt,
                UpdatedBy = vendor.UpdatedBy
            };
        }
    }
}