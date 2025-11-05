using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class LedgerBookService : ILedgerBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LedgerBookService> _logger;

        public LedgerBookService(IUnitOfWork unitOfWork, ILogger<LedgerBookService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region CRUD Operations

        public async Task<LedgerBookDto> GetLedgerBookByIdAsync(int id)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(id);
                if (ledgerBook == null) return null;

                return await MapToDto(ledgerBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ledger book by ID: {Id}", id);
                throw;
            }
        }

        public async Task<LedgerBookDto> GetLedgerBookByLedgerNoAsync(string ledgerNo)
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb => lb.LedgerNo == ledgerNo && lb.IsActive);
                var ledgerBook = ledgerBooks.FirstOrDefault();

                if (ledgerBook == null) return null;

                return await MapToDto(ledgerBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ledger book by LedgerNo: {LedgerNo}", ledgerNo);
                throw;
            }
        }

        public async Task<IEnumerable<LedgerBookDto>> GetAllLedgerBooksAsync()
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.GetAllAsync();
                var dtos = new List<LedgerBookDto>();

                foreach (var book in ledgerBooks.OrderByDescending(lb => lb.CreatedAt))
                {
                    dtos.Add(await MapToDto(book));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all ledger books");
                throw;
            }
        }

        public async Task<IEnumerable<LedgerBookDto>> GetActiveLedgerBooksAsync()
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb => lb.IsActive && !lb.IsClosed);
                var dtos = new List<LedgerBookDto>();

                foreach (var book in ledgerBooks.OrderBy(lb => lb.BookType).ThenBy(lb => lb.LedgerNo))
                {
                    dtos.Add(await MapToDto(book));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active ledger books");
                throw;
            }
        }

        public async Task<IEnumerable<LedgerBookDto>> GetLedgerBooksByStoreAsync(int storeId)
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb => lb.StoreId == storeId && lb.IsActive);
                var dtos = new List<LedgerBookDto>();

                foreach (var book in ledgerBooks.OrderBy(lb => lb.BookType).ThenByDescending(lb => lb.StartDate))
                {
                    dtos.Add(await MapToDto(book));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ledger books by store: {StoreId}", storeId);
                throw;
            }
        }

        public async Task<IEnumerable<LedgerBookDto>> GetLedgerBooksByTypeAsync(string bookType)
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb => lb.BookType == bookType && lb.IsActive);
                var dtos = new List<LedgerBookDto>();

                foreach (var book in ledgerBooks.OrderByDescending(lb => lb.StartDate))
                {
                    dtos.Add(await MapToDto(book));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ledger books by type: {BookType}", bookType);
                throw;
            }
        }

        public async Task<IEnumerable<LedgerBookDto>> GetActiveLedgerBooksByStoreAndTypeAsync(int? storeId, string bookType)
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb =>
                    lb.IsActive &&
                    !lb.IsClosed &&
                    (storeId == null || lb.StoreId == storeId) &&
                    (string.IsNullOrEmpty(bookType) || lb.BookType == bookType));

                var dtos = new List<LedgerBookDto>();

                foreach (var book in ledgerBooks.OrderBy(lb => lb.LedgerNo))
                {
                    dtos.Add(await MapToDto(book));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active ledger books by store {StoreId} and type {BookType}", storeId, bookType);
                throw;
            }
        }

        public async Task<int> CreateLedgerBookAsync(LedgerBookCreateDto dto, string createdBy)
        {
            try
            {
                // Validate unique ledger number
                var isUnique = await IsLedgerNoUniqueAsync(dto.LedgerNo);
                if (!isUnique)
                {
                    throw new InvalidOperationException($"Ledger number '{dto.LedgerNo}' already exists.");
                }

                var ledgerBook = new LedgerBook
                {
                    LedgerNo = dto.LedgerNo,
                    BookName = dto.BookName,
                    BookType = dto.BookType,
                    Description = dto.Description,
                    StoreId = dto.StoreId,
                    TotalPages = dto.TotalPages,
                    CurrentPageNo = 1,
                    StartDate = dto.StartDate,
                    IsClosed = false,
                    Location = dto.Location,
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsActive = true
                };

                await _unitOfWork.LedgerBooks.AddAsync(ledgerBook);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Ledger book created: {LedgerNo} by {CreatedBy}", dto.LedgerNo, createdBy);

                return ledgerBook.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ledger book: {LedgerNo}", dto.LedgerNo);
                throw;
            }
        }

        public async Task<bool> UpdateLedgerBookAsync(int id, LedgerBookDto dto, string updatedBy)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(id);
                if (ledgerBook == null)
                {
                    throw new InvalidOperationException($"Ledger book with ID {id} not found.");
                }

                // Validate unique ledger number (excluding current book)
                if (ledgerBook.LedgerNo != dto.LedgerNo)
                {
                    var isUnique = await IsLedgerNoUniqueAsync(dto.LedgerNo, id);
                    if (!isUnique)
                    {
                        throw new InvalidOperationException($"Ledger number '{dto.LedgerNo}' already exists.");
                    }
                }

                ledgerBook.LedgerNo = dto.LedgerNo;
                ledgerBook.BookName = dto.BookName;
                ledgerBook.BookType = dto.BookType;
                ledgerBook.Description = dto.Description;
                ledgerBook.StoreId = dto.StoreId;
                ledgerBook.TotalPages = dto.TotalPages;
                ledgerBook.Location = dto.Location;
                ledgerBook.Remarks = dto.Remarks;
                ledgerBook.UpdatedAt = DateTime.UtcNow;
                ledgerBook.UpdatedBy = updatedBy;

                _unitOfWork.LedgerBooks.Update(ledgerBook);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Ledger book updated: {LedgerNo} by {UpdatedBy}", dto.LedgerNo, updatedBy);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ledger book: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteLedgerBookAsync(int id)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(id);
                if (ledgerBook == null)
                {
                    return false;
                }

                ledgerBook.IsActive = false;
                ledgerBook.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.LedgerBooks.Update(ledgerBook);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Ledger book deleted (soft): {LedgerNo}", ledgerBook.LedgerNo);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ledger book: {Id}", id);
                throw;
            }
        }

        public async Task<bool> CloseLedgerBookAsync(int id, string closedBy)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(id);
                if (ledgerBook == null)
                {
                    throw new InvalidOperationException($"Ledger book with ID {id} not found.");
                }

                ledgerBook.IsClosed = true;
                ledgerBook.EndDate = DateTime.UtcNow;
                ledgerBook.UpdatedAt = DateTime.UtcNow;
                ledgerBook.UpdatedBy = closedBy;

                _unitOfWork.LedgerBooks.Update(ledgerBook);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Ledger book closed: {LedgerNo} by {ClosedBy}", ledgerBook.LedgerNo, closedBy);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing ledger book: {Id}", id);
                throw;
            }
        }

        #endregion

        #region Page Management

        public async Task<int> GetNextAvailablePageAsync(int ledgerBookId)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(ledgerBookId);
                if (ledgerBook == null)
                {
                    throw new InvalidOperationException($"Ledger book with ID {ledgerBookId} not found.");
                }

                return ledgerBook.CurrentPageNo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next available page for ledger book: {Id}", ledgerBookId);
                throw;
            }
        }

        public async Task<int> GetNextAvailablePageByLedgerNoAsync(string ledgerNo)
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb => lb.LedgerNo == ledgerNo && lb.IsActive);
                var ledgerBook = ledgerBooks.FirstOrDefault();

                if (ledgerBook == null)
                {
                    throw new InvalidOperationException($"Ledger book '{ledgerNo}' not found.");
                }

                return ledgerBook.CurrentPageNo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next available page for ledger: {LedgerNo}", ledgerNo);
                throw;
            }
        }

        public async Task<bool> IncrementPageNumberAsync(int ledgerBookId)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(ledgerBookId);
                if (ledgerBook == null)
                {
                    throw new InvalidOperationException($"Ledger book with ID {ledgerBookId} not found.");
                }

                if (ledgerBook.IsClosed)
                {
                    throw new InvalidOperationException($"Ledger book '{ledgerBook.LedgerNo}' is closed.");
                }

                if (ledgerBook.CurrentPageNo >= ledgerBook.TotalPages)
                {
                    throw new InvalidOperationException($"Ledger book '{ledgerBook.LedgerNo}' is full.");
                }

                ledgerBook.CurrentPageNo++;
                ledgerBook.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.LedgerBooks.Update(ledgerBook);
                await _unitOfWork.CompleteAsync();

                _logger.LogDebug("Page incremented for ledger book {LedgerNo}: {CurrentPage}", ledgerBook.LedgerNo, ledgerBook.CurrentPageNo);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing page number for ledger book: {Id}", ledgerBookId);
                throw;
            }
        }

        public async Task<bool> IncrementPageNumberAsync(string ledgerNo)
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb => lb.LedgerNo == ledgerNo && lb.IsActive);
                var ledgerBook = ledgerBooks.FirstOrDefault();

                if (ledgerBook == null)
                {
                    throw new InvalidOperationException($"Ledger book '{ledgerNo}' not found.");
                }

                return await IncrementPageNumberAsync(ledgerBook.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing page number for ledger: {LedgerNo}", ledgerNo);
                throw;
            }
        }

        #endregion

        #region Validation

        public async Task<bool> IsLedgerBookFullAsync(int ledgerBookId)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(ledgerBookId);
                if (ledgerBook == null)
                {
                    return false;
                }

                return ledgerBook.CurrentPageNo >= ledgerBook.TotalPages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if ledger book is full: {Id}", ledgerBookId);
                throw;
            }
        }

        public async Task<bool> IsLedgerNoUniqueAsync(string ledgerNo, int? excludeId = null)
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb =>
                    lb.LedgerNo == ledgerNo &&
                    lb.IsActive &&
                    (!excludeId.HasValue || lb.Id != excludeId.Value));

                return !ledgerBooks.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking ledger number uniqueness: {LedgerNo}", ledgerNo);
                throw;
            }
        }

        public async Task<bool> CanUseLedgerBookAsync(int ledgerBookId)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(ledgerBookId);
                if (ledgerBook == null || !ledgerBook.IsActive || ledgerBook.IsClosed)
                {
                    return false;
                }

                return ledgerBook.CurrentPageNo < ledgerBook.TotalPages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if ledger book can be used: {Id}", ledgerBookId);
                throw;
            }
        }

        #endregion

        #region Statistics

        public async Task<int> GetPagesUsedAsync(int ledgerBookId)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(ledgerBookId);
                if (ledgerBook == null)
                {
                    return 0;
                }

                return ledgerBook.CurrentPageNo - 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pages used for ledger book: {Id}", ledgerBookId);
                throw;
            }
        }

        public async Task<int> GetPagesRemainingAsync(int ledgerBookId)
        {
            try
            {
                var ledgerBook = await _unitOfWork.LedgerBooks.GetByIdAsync(ledgerBookId);
                if (ledgerBook == null)
                {
                    return 0;
                }

                return ledgerBook.TotalPages - ledgerBook.CurrentPageNo + 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pages remaining for ledger book: {Id}", ledgerBookId);
                throw;
            }
        }

        public async Task<IEnumerable<LedgerBookDto>> GetAlmostFullLedgerBooksAsync(int threshold = 10)
        {
            try
            {
                var ledgerBooks = await _unitOfWork.LedgerBooks.FindAsync(lb => lb.IsActive && !lb.IsClosed);
                var almostFull = ledgerBooks.Where(lb => (lb.TotalPages - lb.CurrentPageNo + 1) <= threshold);

                var dtos = new List<LedgerBookDto>();

                foreach (var book in almostFull.OrderBy(lb => lb.TotalPages - lb.CurrentPageNo))
                {
                    dtos.Add(await MapToDto(book));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting almost full ledger books");
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private async Task<LedgerBookDto> MapToDto(LedgerBook ledgerBook)
        {
            string storeName = null;
            if (ledgerBook.StoreId.HasValue)
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(ledgerBook.StoreId.Value);
                storeName = store?.Name;
            }

            return new LedgerBookDto
            {
                Id = ledgerBook.Id,
                LedgerNo = ledgerBook.LedgerNo,
                BookName = ledgerBook.BookName,
                BookType = ledgerBook.BookType,
                Description = ledgerBook.Description,
                StoreId = ledgerBook.StoreId,
                StoreName = storeName,
                TotalPages = ledgerBook.TotalPages,
                CurrentPageNo = ledgerBook.CurrentPageNo,
                StartDate = ledgerBook.StartDate,
                EndDate = ledgerBook.EndDate,
                IsClosed = ledgerBook.IsClosed,
                Location = ledgerBook.Location,
                Remarks = ledgerBook.Remarks,
                CreatedAt = ledgerBook.CreatedAt,
                CreatedBy = ledgerBook.CreatedBy,
                UpdatedAt = ledgerBook.UpdatedAt,
                UpdatedBy = ledgerBook.UpdatedBy,
                IsActive = ledgerBook.IsActive
            };
        }

        #endregion
    }
}
