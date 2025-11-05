using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace IMS.Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IValidationService _validationService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILogger<TransferService> _logger;
        private readonly IBarcodeService _barcodeService;
        private readonly INotificationService _notificationService;
        private readonly IApprovalService _approvalService;
        private readonly IStockService _stockService;

        public TransferService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            IValidationService validationService,
            IBarcodeService barcodeService,
            IApprovalService approvalService,
            IStockService stockService,
            INotificationService notificationService,
            IActivityLogService activityLogService,
            ILogger<TransferService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _validationService = validationService;
            _barcodeService = barcodeService;
            _notificationService = notificationService;
            _approvalService = approvalService;
            _activityLogService = activityLogService;
            _logger = logger;
            _stockService = stockService;
        }

        // Create Transfer Request
        public async Task<TransferDto> CreateTransferAsync(TransferDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate stores
                var fromStore = await _unitOfWork.Stores.GetByIdAsync(dto.FromStoreId);
                var toStore = await _unitOfWork.Stores.GetByIdAsync(dto.ToStoreId);

                if (fromStore == null || toStore == null)
                    throw new InvalidOperationException("Invalid store(s) specified");

                if (dto.FromStoreId == dto.ToStoreId)
                    throw new InvalidOperationException("Source and destination stores cannot be same");

                // Validate stock availability
                foreach (var item in dto.Items)
                {
                    var availableStock = await _stockService.GetAvailableStockAsync(dto.FromStoreId, item.ItemId);
                    if (availableStock < item.TransferQuantity)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for item {item.ItemName}. Available: {availableStock}, Requested: {item.TransferQuantity}");
                    }
                }

                // Create transfer
                var transfer = new Transfer
                {
                    TransferNo = await GenerateTransferNoAsync(),
                    FromStoreId = dto.FromStoreId,
                    ToStoreId = dto.ToStoreId,
                    TransferDate = dto.TransferDate,
                    TransferType = dto.TransferType,
                    Status = TransferStatus.Pending.ToString(),
                    Purpose = dto.Purpose,
                    RequestedBy = dto.RequestedBy,
                    RequestedDate = DateTime.Now,

                    // Hierarchy information
                    FromBattalionId = fromStore.BattalionId,
                    FromRangeId = fromStore.RangeId,
                    FromZilaId = fromStore.ZilaId,
                    ToBattalionId = toStore.BattalionId,
                    ToRangeId = toStore.RangeId,
                    ToZilaId = toStore.ZilaId,

                    EstimatedDeliveryDate = dto.EstimatedDeliveryDate,
                    TransportMode = dto.TransportMode,
                    VehicleNo = dto.VehicleNo,
                    DriverName = dto.DriverName,
                    DriverContact = dto.DriverContact,

                    Items = new List<TransferItem>()
                };

                decimal? totalValue = 0;
                foreach (var itemDto in dto.Items)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                    var itemValue = (item.UnitPrice ?? 0) * itemDto.TransferQuantity;

                    transfer.Items.Add(new TransferItem
                    {
                        ItemId = itemDto.ItemId,
                        RequestedQuantity = itemDto.TransferQuantity,
                        ApprovedQuantity = 0,
                        ShippedQuantity = 0,
                        ReceivedQuantity = 0,
                        UnitPrice = item.UnitPrice ?? 0,
                        TotalValue = itemValue,
                        BatchNo = itemDto.BatchNo,
                        Remarks = itemDto.Remarks
                    });

                    totalValue += itemValue;
                }

                transfer.TotalValue = totalValue;

                await _unitOfWork.Transfers.AddAsync(transfer);
                await _unitOfWork.CompleteAsync();

                // Create approval request if needed
                if (totalValue > 25000 || dto.TransferType == "Emergency")
                {
                    await _approvalService.CreateApprovalRequestAsync(new ApprovalRequestDto
                    {
                        EntityType = "Transfer",
                        EntityId = transfer.Id,
                        RequestedBy = dto.RequestedBy,
                        RequestedDate = DateTime.Now,
                        Amount = totalValue,
                        Description = $"Transfer Request: {transfer.TransferNo}",
                        Priority = dto.TransferType == "Emergency" ? "High" : "Normal"
                    });
                }
                else
                {
                    // Auto-approve for low value transfers
                    transfer.Status = TransferStatus.Approved.ToString();
                    transfer.ApprovedBy = "System";
                    transfer.ApprovedDate = DateTime.Now;
                }

                // Send notifications
                await NotifyTransferCreationAsync(transfer);

                await _unitOfWork.CommitTransactionAsync();

                dto.Id = transfer.Id;
                dto.TransferNo = transfer.TransferNo;
                dto.Status = transfer.Status;
                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating transfer");
                throw;
            }
        }

        // Approve Transfer
        public async Task<bool> ApproveTransferAsync(int transferId, TransferApprovalDto approvalDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var transfer = await _unitOfWork.Transfers.Query()
                    .Include(t => t.Items)
                    .FirstOrDefaultAsync(t => t.Id == transferId);

                if (transfer == null)
                    throw new InvalidOperationException("Transfer not found");

                if (transfer.Status != TransferStatus.Pending.ToString())
                    throw new InvalidOperationException("Transfer is not pending approval");

                // Update approved quantities
                foreach (var item in transfer.Items)
                {
                    var approvedItem = approvalDto.ApprovedItems
                        .FirstOrDefault(ai => ai.ItemId == item.ItemId);

                    if (approvedItem != null)
                    {
                        item.ApprovedQuantity = approvedItem.ApprovedQuantity;

                        // Reserve stock from source store
                        await _stockService.ReserveStockAsync(
                            transfer.FromStoreId,
                            item.ItemId,
                            item.ApprovedQuantity);
                    }
                }

                transfer.Status = TransferStatus.Approved.ToString();
                transfer.ApprovedBy = approvalDto.ApprovedBy;
                transfer.ApprovedDate = DateTime.Now;
                transfer.Remarks = approvalDto.Remarks;

                await _unitOfWork.CompleteAsync();

                // Generate transfer document
                await GenerateTransferDocumentAsync(transfer);

                // Send notifications
                await NotifyTransferApprovalAsync(transfer);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving transfer");
                throw;
            }
        }

        // Ship Items (Source Store)
        public async Task<bool> ShipTransferAsync(int transferId, ShipmentDto shipmentDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var transfer = await _unitOfWork.Transfers.Query()
                    .Include(t => t.Items)
                    .ThenInclude(ti => ti.Item)
                    .FirstOrDefaultAsync(t => t.Id == transferId);

                if (transfer == null)
                    throw new InvalidOperationException("Transfer not found");

                if (transfer.Status != TransferStatus.Approved.ToString())
                    throw new InvalidOperationException("Transfer is not approved for shipping");

                // Create shipment record
                var shipment = new TransferShipment
                {
                    TransferId = transferId,
                    ShipmentNo = await GenerateShipmentNoAsync(),
                    ShippedDate = shipmentDto.ShippedDate,
                    ShippedBy = shipmentDto.ShippedBy,
                    PackingListNo = shipmentDto.PackingListNo,
                    TransportCompany = shipmentDto.TransportCompany,
                    VehicleNo = shipmentDto.VehicleNo,
                    DriverName = shipmentDto.DriverName,
                    DriverContact = shipmentDto.DriverContact,
                    SealNo = shipmentDto.SealNo,
                    EstimatedArrival = shipmentDto.EstimatedArrival,
                    Items = new List<TransferShipmentItem>()
                };

                // Process shipped items
                foreach (var shippedItem in shipmentDto.ShippedItems)
                {
                    var transferItem = transfer.Items
                        .FirstOrDefault(ti => ti.ItemId == shippedItem.ItemId);

                    if (transferItem == null) continue;

                    transferItem.ShippedQuantity = shippedItem.ShippedQuantity;
                    transferItem.ShippedDate = shipmentDto.ShippedDate;
                    transferItem.PackageCount = shippedItem.PackageCount;
                    transferItem.PackageDetails = shippedItem.PackageDetails;

                    // Add to shipment
                    shipment.Items.Add(new TransferShipmentItem
                    {
                        ItemId = shippedItem.ItemId,
                        ShippedQuantity = shippedItem.ShippedQuantity,
                        PackageCount = shippedItem.PackageCount,
                        PackageDetails = shippedItem.PackageDetails,
                        BatchNo = shippedItem.BatchNo,
                        Condition = shippedItem.Condition
                    });

                    // Note: Stock deduction and movement tracking is handled by ProcessTransferDispatchAsync
                    // This method only tracks shipment details
                }

                await _unitOfWork.TransferShipments.AddAsync(shipment);

                // Update transfer status
                transfer.Status = TransferStatus.InTransit.ToString();
                transfer.ShippedDate = shipmentDto.ShippedDate;
                transfer.ShippedBy = shipmentDto.ShippedBy;
                transfer.ShipmentNo = shipment.ShipmentNo;

                // Generate shipping documents and QR code
                transfer.ShippingQRCode = await GenerateShippingQRCodeAsync(transfer, shipment);

                await _unitOfWork.CompleteAsync();

                // Send notifications
                await NotifyShipmentAsync(transfer, shipment);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error shipping transfer");
                throw;
            }
        }

        // Receive Items (Destination Store)
        public async Task<bool> ReceiveTransferAsync(int transferId, ReceiveTransferDto receiveDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var transfer = await _unitOfWork.Transfers
                    .Query()
                    .Include(t => t.Items)
                    .Include(t => t.Shipment)
                    .FirstOrDefaultAsync(t => t.Id == transferId);

                if (transfer == null)
                    throw new InvalidOperationException("Transfer not found");

                if (transfer.Status != TransferStatus.InTransit.ToString())
                    throw new InvalidOperationException("Transfer is not in transit");

                // Verify QR code if provided
                if (!string.IsNullOrEmpty(receiveDto.ScannedQRCode))
                {
                    if (receiveDto.ScannedQRCode != transfer.ShippingQRCode)
                    {
                        throw new InvalidOperationException("Invalid shipping QR code");
                    }
                }

                // Process received items
                bool hasDiscrepancy = false;
                var discrepancies = new List<TransferDiscrepancy>();

                foreach (var receivedItem in receiveDto.ReceivedItems)
                {
                    var transferItem = transfer.Items
                        .FirstOrDefault(ti => ti.ItemId == receivedItem.ItemId);

                    if (transferItem == null) continue;

                    transferItem.ReceivedQuantity = receivedItem.ReceivedQuantity;
                    transferItem.ReceivedDate = receiveDto.ReceivedDate;
                    transferItem.ReceivedCondition = receivedItem.Condition;
                    transferItem.Remarks = receivedItem.Remarks;

                    // Check for discrepancy
                    if (transferItem.ReceivedQuantity != transferItem.ShippedQuantity)
                    {
                        hasDiscrepancy = true;
                        discrepancies.Add(new TransferDiscrepancy
                        {
                            TransferId = transferId,
                            ItemId = receivedItem.ItemId,
                            ShippedQuantity = transferItem.ShippedQuantity,
                            ReceivedQuantity = transferItem.ReceivedQuantity,
                            Variance = transferItem.ReceivedQuantity - transferItem.ShippedQuantity,
                            Reason = receivedItem.DiscrepancyReason,
                            ReportedBy = receiveDto.ReceivedBy,
                            ReportedDate = DateTime.Now
                        });
                    }

                    // Note: Stock addition and movement tracking is handled by ConfirmTransferReceiptAsync
                    // This method only verifies shipment and tracks discrepancies
                }

                // Save discrepancies if any
                if (hasDiscrepancy)
                {
                    foreach (var discrepancy in discrepancies)
                    {
                        await _unitOfWork.TransferDiscrepancies.AddAsync(discrepancy);
                    }
                }

                // Update transfer status
                transfer.Status = hasDiscrepancy ? TransferStatus.ReceivedWithDiscrepancy.ToString() : TransferStatus.Completed.ToString();
                transfer.ReceivedDate = receiveDto.ReceivedDate;
                transfer.ReceivedBy = receiveDto.ReceivedBy;
                transfer.ReceiverSignature = receiveDto.ReceiverSignature;
                transfer.ReceiptRemarks = receiveDto.Remarks;
                transfer.HasDiscrepancy = hasDiscrepancy;

                // Update shipment
                if (transfer.Shipment != null)
                {
                    transfer.Shipment.ActualArrival = receiveDto.ReceivedDate;
                    transfer.Shipment.ReceivedBy = receiveDto.ReceivedBy;
                    transfer.Shipment.ReceiptCondition = receiveDto.OverallCondition;
                }

                await _unitOfWork.CompleteAsync();

                // Generate receipt
                var receipt = await GenerateTransferReceiptAsync(transfer);

                // Send notifications
                await NotifyTransferCompletionAsync(transfer, receipt, hasDiscrepancy);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error receiving transfer");
                throw;
            }
        }

        // Track Transfer Status
        public async Task<TransferTrackingDto> TrackTransferAsync(string transferNo)
        {
            try
            {
                var transfer = await _unitOfWork.Transfers.Query()
                    .Include(t => t.FromStore)
                    .Include(t => t.ToStore)
                    .Include(t => t.Shipment)
                    .FirstOrDefaultAsync(t => t.TransferNo == transferNo);

                if (transfer == null)
                    throw new InvalidOperationException("Transfer not found");

                var tracking = new TransferTrackingDto
                {
                    TransferNo = transfer.TransferNo,
                    Status = transfer.Status,
                    FromStore = transfer.FromStore.Name,
                    ToStore = transfer.ToStore.Name,

                    Timeline = new List<TransferTimelineDto>
                    {
                        new TransferTimelineDto
                        {
                            Event = "Transfer Created",
                            Date = transfer.RequestedDate,
                            Description = $"Transfer request created by {transfer.RequestedBy}",
                            Status = "Completed"
                        }
                    }
                };

                if (transfer.ApprovedDate.HasValue)
                {
                    tracking.Timeline.Add(new TransferTimelineDto
                    {
                        Event = "Transfer Approved",
                        Date = transfer.ApprovedDate.Value,
                        Description = $"Approved by {transfer.ApprovedBy}",
                        Status = "Completed"
                    });
                }

                if (transfer.ShippedDate.HasValue)
                {
                    tracking.Timeline.Add(new TransferTimelineDto
                    {
                        Event = "Items Shipped",
                        Date = transfer.ShippedDate.Value,
                        Description = $"Shipped by {transfer.ShippedBy}",
                        Status = "Completed"
                    });
                }

                if (transfer.Status == TransferStatus.InTransit.ToString() && transfer.Shipment != null)
                {
                    tracking.Timeline.Add(new TransferTimelineDto
                    {
                        Event = "In Transit",
                        Date = DateTime.Now,
                        Description = $"Estimated arrival: {transfer.Shipment.EstimatedArrival:dd/MM/yyyy}",
                        Status = "InProgress"
                    });

                    tracking.CurrentLocation = "In Transit";
                    tracking.EstimatedDelivery = transfer.Shipment.EstimatedArrival;
                    tracking.VehicleNo = transfer.Shipment.VehicleNo;
                    tracking.DriverContact = transfer.Shipment.DriverContact;
                }

                if (transfer.ReceivedDate.HasValue)
                {
                    tracking.Timeline.Add(new TransferTimelineDto
                    {
                        Event = "Transfer Received",
                        Date = transfer.ReceivedDate.Value,
                        Description = $"Received by {transfer.ReceivedBy}",
                        Status = "Completed"
                    });
                }

                return tracking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking transfer");
                throw;
            }
        }

        // Helper Methods
        public async Task<string> GenerateTransferNoAsync() // Change from private to public
        {
            var prefix = "TRF";
            var date = DateTime.Now.ToString("yyyyMMdd");
            var lastTransfer = await _unitOfWork.Transfers
                .GetLastAsync(t => t.TransferNo.StartsWith($"{prefix}{date}"));

            int sequence = 1;
            if (lastTransfer != null)
            {
                var lastNo = lastTransfer.TransferNo.Substring(11);
                sequence = int.Parse(lastNo) + 1;
            }

            return $"{prefix}{date}{sequence:D4}";
        }
        private async Task<string> GenerateShipmentNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"SHP-{date:yyyyMM}";

            var lastShipment = await _unitOfWork.TransferShipments.Query()
                .Where(s => s.ShipmentNo.StartsWith(prefix))
                .OrderByDescending(s => s.ShipmentNo)
                .FirstOrDefaultAsync();

            if (lastShipment == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastShipment.ShipmentNo.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }
        private async Task<string> GenerateShippingQRCodeAsync(Transfer transfer, TransferShipment shipment)
        {
            var qrData = new
            {
                Type = "TRANSFER_SHIPMENT",
                TransferNo = transfer.TransferNo,
                ShipmentNo = shipment.ShipmentNo,
                FromStore = transfer.FromStoreId,
                ToStore = transfer.ToStoreId,
                ShippedDate = shipment.ShippedDate,
                ItemCount = shipment.Items.Count,
                TotalQuantity = shipment.Items.Sum(i => i.ShippedQuantity),
                SealNo = shipment.SealNo
            };

            var qrJson = System.Text.Json.JsonSerializer.Serialize(qrData);
            return await _barcodeService.GenerateQRCodeAsync(qrJson);
        }
        private async Task GenerateTransferDocumentAsync(Transfer transfer)
        {
            // Generate transfer document/challan
            await Task.CompletedTask;
        }
        private async Task<TransferReceiptDto> GenerateTransferReceiptAsync(Transfer transfer)
        {
            var receipt = new TransferReceiptDto
            {
                ReceiptNo = $"TR-{DateTime.Now:yyyyMMdd}-{transfer.Id:D4}",
                TransferNo = transfer.TransferNo,
                TransferDate = transfer.TransferDate,
                ReceivedDate = transfer.ReceivedDate.Value,
                FromStore = transfer.FromStore?.Name,
                ToStore = transfer.ToStore?.Name,
                Items = transfer.Items.Select(i => new TransferReceiptItemDto
                {
                    ItemName = i.Item?.Name,
                    ShippedQuantity = i.ShippedQuantity,
                    ReceivedQuantity = i.ReceivedQuantity,
                    Variance = i.ReceivedQuantity - i.ShippedQuantity
                }).ToList()
            };

            return receipt;
        }
        private async Task NotifyTransferCreationAsync(Transfer transfer)
        {
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "New Transfer Request",
                Message = $"Transfer request {transfer.TransferNo} has been created",
                Type = "Transfer",
                CreatedAt = DateTime.Now
            });
        }
        private async Task NotifyTransferApprovalAsync(Transfer transfer)
        {
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                UserId = transfer.RequestedBy,
                Title = "Transfer Approved",
                Message = $"Transfer {transfer.TransferNo} has been approved",
                Type = "Approval",
                CreatedAt = DateTime.Now
            });
        }
        private async Task NotifyShipmentAsync(Transfer transfer, TransferShipment shipment)
        {
            // Notify destination store
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Transfer Shipped",
                Message = $"Transfer {transfer.TransferNo} has been shipped. Expected arrival: {shipment.EstimatedArrival:dd/MM/yyyy}",
                Type = "Shipment",
                CreatedAt = DateTime.Now
            });
        }
        private async Task NotifyTransferCompletionAsync(Transfer transfer, TransferReceiptDto receipt, bool hasDiscrepancy)
        {
            var message = hasDiscrepancy
                ? $"Transfer {transfer.TransferNo} received with discrepancies. Please review."
                : $"Transfer {transfer.TransferNo} received successfully.";

            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Transfer Completed",
                Message = message,
                Type = hasDiscrepancy ? "Warning" : "Success",
                CreatedAt = DateTime.Now
            });
        }
        public async Task<PagedResult<TransferDto>> GetAllTransfersAsync(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var totalCount = await _unitOfWork.Transfers.CountAsync(t => t.IsActive);
                var transfers = await _unitOfWork.Transfers.GetPagedAsync(pageNumber, pageSize, t => t.IsActive);

                var transferDtos = new List<TransferDto>();
                foreach (var transfer in transfers)
                {
                    var fromStore = await _unitOfWork.Stores.GetByIdAsync(transfer.FromStoreId);
                    var toStore = await _unitOfWork.Stores.GetByIdAsync(transfer.ToStoreId);
                    var transferItems = await _unitOfWork.TransferItems.FindAsync(ti => ti.TransferId == transfer.Id);

                    var items = new List<TransferItemDto>();
                    foreach (var ti in transferItems)
                    {
                        var item = await _unitOfWork.Items.GetByIdAsync(ti.ItemId);
                        items.Add(new TransferItemDto
                        {
                            ItemId = ti.ItemId,
                            ItemName = item?.Name ?? "Unknown",
                            Quantity = ti.Quantity
                        });
                    }

                    transferDtos.Add(new TransferDto
                    {
                        Id = transfer.Id,
                        TransferNo = transfer.TransferNo,
                        TransferDate = transfer.TransferDate,
                        FromStoreId = transfer.FromStoreId,
                        FromStoreName = fromStore?.Name ?? "Unknown",
                        ToStoreId = transfer.ToStoreId,
                        ToStoreName = toStore?.Name ?? "Unknown",
                        Remarks = transfer.Remarks,
                        Items = items
                    });
                }

                return new PagedResult<TransferDto>
                {
                    Items = transferDtos.OrderByDescending(t => t.TransferDate),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all transfers");
                throw;
            }
        }
        public async Task<bool> ValidateTransferAsync(TransferDto transferDto)
        {
            var validation = await _validationService.ValidateTransfer(transferDto);
            return validation.IsValid;
        }
        public async Task<IEnumerable<TransferDto>> GetTransfersByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var transfers = await _unitOfWork.Transfers.FindAsync(t =>
                    t.IsActive &&
                    t.TransferDate >= fromDate &&
                    t.TransferDate <= toDate);

                var transferDtos = new List<TransferDto>();
                foreach (var transfer in transfers)
                {
                    transferDtos.Add(await MapToTransferDto(transfer));
                }

                return transferDtos.OrderByDescending(t => t.TransferDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transfers by date range");
                throw;
            }
        }
        public async Task<IEnumerable<TransferDto>> GetTransfersByStoreAsync(int? storeId)
        {
            try
            {
                var transfers = await _unitOfWork.Transfers.FindAsync(t =>
                    t.IsActive &&
                    (t.FromStoreId == storeId || t.ToStoreId == storeId));

                var transferDtos = new List<TransferDto>();
                foreach (var transfer in transfers)
                {
                    transferDtos.Add(await MapToTransferDto(transfer));
                }

                return transferDtos.OrderByDescending(t => t.TransferDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transfers by store");
                throw;
            }
        }
        public async Task<IEnumerable<TransferDto>> GetTransfersFromStoreAsync(int? fromStoreId)
        {
            try
            {
                var transfers = await _unitOfWork.Transfers.FindAsync(t =>
                    t.IsActive &&
                    t.FromStoreId == fromStoreId);

                var transferDtos = new List<TransferDto>();
                foreach (var transfer in transfers)
                {
                    transferDtos.Add(await MapToTransferDto(transfer));
                }

                return transferDtos.OrderByDescending(t => t.TransferDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transfers from store");
                throw;
            }
        }
        public async Task<IEnumerable<TransferDto>> GetTransfersToStoreAsync(int? toStoreId)
        {
            try
            {
                var transfers = await _unitOfWork.Transfers.FindAsync(t =>
                    t.IsActive &&
                    t.ToStoreId == toStoreId);

                var transferDtos = new List<TransferDto>();
                foreach (var transfer in transfers)
                {
                    transferDtos.Add(await MapToTransferDto(transfer));
                }

                return transferDtos.OrderByDescending(t => t.TransferDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transfers to store");
                throw;
            }
        }
        public async Task<int> GetTransferCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = await _unitOfWork.Transfers.FindAsync(t => t.IsActive);

                if (fromDate.HasValue)
                    query = query.Where(t => t.TransferDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(t => t.TransferDate <= toDate.Value);

                return query.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transfer count");
                throw;
            }
        }
        public async Task<bool> TransferNoExistsAsync(string transferNo)
        {
            try
            {
                var transfer = await _unitOfWork.Transfers.FirstOrDefaultAsync(t => t.TransferNo == transferNo);
                return transfer != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking transfer number existence");
                throw;
            }
        }
        public async Task<IEnumerable<TransferDto>> GetPagedTransfersAsync(int pageNumber, int pageSize)
        {
            try
            {
                var result = await GetAllTransfersAsync(pageNumber, pageSize);
                return result.Items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged transfers");
                throw;
            }
        }
        private async Task<TransferDto> MapToTransferDto(Transfer transfer)
        {
            var fromStore = await _unitOfWork.Stores.GetByIdAsync(transfer.FromStoreId);
            var toStore = await _unitOfWork.Stores.GetByIdAsync(transfer.ToStoreId);
            var transferItems = await _unitOfWork.TransferItems.FindAsync(ti => ti.TransferId == transfer.Id);

            var items = new List<TransferItemDto>();
            foreach (var ti in transferItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(ti.ItemId);
                items.Add(new TransferItemDto
                {
                    ItemId = ti.ItemId,
                    ItemName = item?.Name ?? "Unknown",
                    Quantity = ti.Quantity
                });
            }

            return new TransferDto
            {
                Id = transfer.Id,
                TransferNo = transfer.TransferNo,
                TransferDate = transfer.TransferDate,
                FromStoreId = transfer.FromStoreId,
                FromStoreName = fromStore?.Name ?? "Unknown",
                ToStoreId = transfer.ToStoreId,
                ToStoreName = toStore?.Name ?? "Unknown",
                Remarks = transfer.Remarks,
                Items = items,
                CreatedAt = transfer.CreatedAt,
                CreatedBy = transfer.CreatedBy,
                UpdatedAt = transfer.UpdatedAt,
                UpdatedBy = transfer.UpdatedBy
            };
        }
        public async Task<ServiceResult> CreateTransferRequestAsync(TransferDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate stores
                var fromStore = await _unitOfWork.Stores.GetByIdAsync(dto.FromStoreId);
                var toStore = await _unitOfWork.Stores.GetByIdAsync(dto.ToStoreId);

                if (fromStore == null || toStore == null)
                    return ServiceResult.Failure("Invalid store selection");

                // Create transfer
                var transfer = new Transfer
                {
                    TransferNo = await GenerateTransferNoAsync(),
                    FromStoreId = dto.FromStoreId,
                    ToStoreId = dto.ToStoreId,
                    TransferDate = DateTime.Now,
                    Status = "Draft",
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.Transfers.AddAsync(transfer);
                await _unitOfWork.CompleteAsync();

                // Calculate total value for transfer items
                decimal totalValue = 0;

                // Add transfer items
                foreach (var itemDto in dto.Items)
                {
                    // Validate stock availability
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == dto.FromStoreId && si.ItemId == itemDto.ItemId);

                    if (storeItem == null || storeItem.Quantity < itemDto.Quantity)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        var item = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                        return ServiceResult.Failure($"Insufficient stock for item: {item?.Name}");
                    }

                    // Get item price for value calculation
                    var item2 = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                    if (item2 != null && item2.UnitPrice.HasValue)
                    {
                        totalValue += item2.UnitPrice.Value * itemDto.Quantity;
                    }

                    var transferItem = new TransferItem
                    {
                        TransferId = transfer.Id,
                        ItemId = itemDto.ItemId,
                        Quantity = itemDto.Quantity,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };

                    await _unitOfWork.TransferItems.AddAsync(transferItem);
                }

                await _unitOfWork.CompleteAsync();

                // Create approval request for transfer
                var approvalRequest = new ApprovalRequest
                {
                    EntityType = "TRANSFER",
                    EntityId = transfer.Id,
                    RequestedBy = _userContext.UserId,
                    RequestedDate = DateTime.Now,
                    Amount = totalValue,
                    Description = $"Transfer Request: {transfer.TransferNo}",
                    Priority = totalValue > 50000 ? "High" : "Normal",
                    Status = "Pending",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName
                };

                await _unitOfWork.ApprovalRequests.AddAsync(approvalRequest);
                await _unitOfWork.CompleteAsync();

                // Create approval steps based on hierarchy
                await CreateTransferApprovalSteps(approvalRequest, fromStore, toStore, totalValue);

                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Transfer",
                    transfer.Id,
                    "Create",
                    $"Created transfer request {transfer.TransferNo} from {fromStore.Name} to {toStore.Name}",
                    _userContext.CurrentUserName
                );

                return ServiceResult.SuccessResult($"Transfer request {transfer.TransferNo} created successfully and sent for approval");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating transfer request");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        private async Task CreateTransferApprovalSteps(ApprovalRequest approvalRequest, Store fromStore, Store toStore, decimal totalValue)
        {
            // Level 1: Source Store In-charge
            var step1 = new ApprovalStep
            {
                ApprovalRequestId = approvalRequest.Id,
                StepLevel = 1,
                ApproverRole = "StoreIncharge",
                Status = ApprovalStatus.Pending,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            await _unitOfWork.ApprovalSteps.AddAsync(step1);

            // Level 2: Destination Store In-charge (for confirmation)
            var step2 = new ApprovalStep
            {
                ApprovalRequestId = approvalRequest.Id,
                StepLevel = 2,
                ApproverRole = "StoreIncharge",
                Status = ApprovalStatus.Awaiting,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            await _unitOfWork.ApprovalSteps.AddAsync(step2);

            // Level 3: Higher authority if value is high
            if (totalValue > 50000)
            {
                var step3 = new ApprovalStep
                {
                    ApprovalRequestId = approvalRequest.Id,
                    StepLevel = 3,
                    ApproverRole = "DeputyDirector",
                    Status = ApprovalStatus.Awaiting,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                await _unitOfWork.ApprovalSteps.AddAsync(step3);
            }

            await _unitOfWork.CompleteAsync();
        }
        public async Task<ServiceResult> ApproveTransferAsync(int transferId, string approvedBy)
        {
            try
            {
                var transfer = await _unitOfWork.Transfers.GetByIdAsync(transferId);
                if (transfer == null || transfer.Status != "Draft")
                    return ServiceResult.Failure("Invalid transfer status");

                transfer.Status = "Approved";
                transfer.UpdatedAt = DateTime.Now;
                transfer.UpdatedBy = approvedBy;

                _unitOfWork.Transfers.Update(transfer);
                await _unitOfWork.CompleteAsync();

                // Send notification
                var notification = new NotificationDto
                {
                    Title = "Transfer Approved",
                    Message = $"Transfer {transfer.TransferNo} has been approved",
                    Type = "success",
                    UserId = transfer.CreatedBy
                };
                await _notificationService.CreateNotificationAsync(notification);

                return ServiceResult.SuccessResult("Transfer approved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving transfer");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }
        public async Task<ServiceResult> ProcessTransferDispatchAsync(int transferId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var transfer = await _unitOfWork.Transfers
                    .GetAsync(t => t.Id == transferId,
                        includes: new[] { "Items.Item", "FromStore", "ToStore" });

                if (transfer == null || transfer.Status != "Approved")
                    return ServiceResult.Failure("Invalid transfer status");

                // TransferNo already serves as the tracking code (format: TRF-YYYYMMDD-XXXX)
                // No need to generate additional tracking data in Remarks

                // Process each item
                foreach (var transferItem in transfer.Items)
                {
                    // Deduct from source store
                    var fromStoreItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == transfer.FromStoreId && si.ItemId == transferItem.ItemId);

                    if (fromStoreItem != null)
                    {
                        fromStoreItem.Quantity -= transferItem.Quantity;
                        fromStoreItem.UpdatedAt = DateTime.Now;
                        fromStoreItem.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.StoreItems.Update(fromStoreItem);

                        // Create outgoing stock movement
                        var outMovement = new StockMovement
                        {
                            StoreId = transfer.FromStoreId,
                            ItemId = transferItem.ItemId,
                            MovementType = "OUT",
                            MovementDate = DateTime.Now,
                            Quantity = transferItem.Quantity,
                            ReferenceType = "TRANSFER_OUT",
                            ReferenceNo = transfer.TransferNo,
                            Remarks = $"Transfer to {transfer.ToStore.Name}",
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userContext.CurrentUserName,
                            IsActive = true
                        };
                        await _unitOfWork.StockMovements.AddAsync(outMovement);
                    }
                }

                transfer.Status = "In Transit";
                transfer.UpdatedAt = DateTime.Now;
                transfer.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Transfers.Update(transfer);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Send notification
                var notification = new NotificationDto
                {
                    Title = "Transfer Dispatched",
                    Message = $"Transfer {transfer.TransferNo} has been dispatched to {transfer.ToStore.Name}",
                    Type = "info",
                    UserId = transfer.CreatedBy
                };
                await _notificationService.CreateNotificationAsync(notification);

                return ServiceResult.SuccessResult("Transfer dispatched successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error processing transfer dispatch");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }
        public async Task<ServiceResult> ConfirmTransferReceiptAsync(int transferId, TransferReceiptDto receiptDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var transfer = await _unitOfWork.Transfers
                    .GetAsync(t => t.Id == transferId,
                        includes: new[] { "Items.Item", "ToStore" });

                if (transfer == null || transfer.Status != "In Transit")
                    return ServiceResult.Failure("Invalid transfer status");

                // Process each item
                foreach (var receiptItem in receiptDto.Items)
                {
                    var transferItem = transfer.Items.FirstOrDefault(ti => ti.ItemId == receiptItem.ItemId);

                    if (transferItem == null)
                        continue;

                    // Update transfer item
                    transferItem.UpdatedAt = DateTime.Now;
                    transferItem.UpdatedBy = _userContext.CurrentUserName;
                    _unitOfWork.TransferItems.Update(transferItem);

                    // Add to destination store
                    var toStoreItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == transfer.ToStoreId && si.ItemId == receiptItem.ItemId);

                    if (toStoreItem != null)
                    {
                        toStoreItem.Quantity += receiptItem.ReceivedQuantity;
                        toStoreItem.UpdatedAt = DateTime.Now;
                        toStoreItem.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.StoreItems.Update(toStoreItem);
                    }
                    else
                    {
                        toStoreItem = new StoreItem
                        {
                            StoreId = transfer.ToStoreId,
                            ItemId = receiptItem.ItemId,
                            Quantity = receiptItem.ReceivedQuantity,
                            Location = receiptItem.Location,
                            Status = ItemStatus.Available,
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userContext.CurrentUserName
                        };
                        await _unitOfWork.StoreItems.AddAsync(toStoreItem);
                    }

                    // Create incoming stock movement
                    var inMovement = new StockMovement
                    {
                        StoreId = transfer.ToStoreId,
                        ItemId = receiptItem.ItemId,
                        MovementType = "IN",
                        MovementDate = DateTime.Now,
                        Quantity = receiptItem.ReceivedQuantity,
                        ReferenceType = "TRANSFER_IN",
                        ReferenceNo = transfer.TransferNo,
                        Remarks = $"Transfer from {transfer.FromStore.Name}",
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };
                    await _unitOfWork.StockMovements.AddAsync(inMovement);
                }

                transfer.Status = "Completed";
                transfer.UpdatedAt = DateTime.Now;
                transfer.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Transfers.Update(transfer);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ServiceResult.SuccessResult("Transfer receipt confirmed successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error confirming transfer receipt");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }
        public async Task<TransferDto> GetTransferByTrackingCodeAsync(string trackingCode)
        {
            // Use TransferNo directly as tracking code (format: TRF-YYYYMMDD-XXXX)
            var transfer = await _unitOfWork.Transfers
                .GetAsync(t => t.TransferNo == trackingCode && t.IsActive,
                    includes: new[] { "Items.Item", "FromStore", "ToStore" });

            if (transfer == null)
                return null;

            return await GetTransferByIdAsync(transfer.Id);
        }
        public async Task<TransferDto> GetTransferByIdAsync(int id)
        {
            var transfer = await _unitOfWork.Transfers
                .GetAsync(t => t.Id == id,
                    includes: new[] { "Items.Item", "FromStore", "ToStore" });

            if (transfer == null)
                return null;

            return new TransferDto
            {
                Id = transfer.Id,
                TransferNo = transfer.TransferNo,
                FromStoreId = transfer.FromStoreId,
                FromStoreName = transfer.FromStore?.Name ?? "Unknown",
                ToStoreId = transfer.ToStoreId,
                ToStoreName = transfer.ToStore?.Name ?? "Unknown",
                TransferDate = transfer.TransferDate,
                Remarks = transfer.Remarks,
                Status = transfer.Status,
                CreatedAt = transfer.CreatedAt,
                CreatedBy = transfer.CreatedBy,
                Items = transfer.Items.Select(ti => new TransferItemDto
                {
                    ItemId = ti.ItemId,
                    ItemName = ti.Item?.Name,
                    Quantity = ti.Quantity
                }).ToList()
            };
        }
        public async Task<IEnumerable<TransferDto>> GetAllTransfersAsync()
        {
            var transfers = await _unitOfWork.Transfers
                .GetAllAsync(t => t.IsActive,
                    includes: new[] { "Items.Item", "FromStore", "ToStore" });

            return transfers.Select(t => new TransferDto
            {
                Id = t.Id,
                TransferNo = t.TransferNo,
                FromStoreId = t.FromStoreId,
                FromStoreName = t.FromStore?.Name ?? "Unknown",
                ToStoreId = t.ToStoreId,
                ToStoreName = t.ToStore?.Name ?? "Unknown",
                TransferDate = t.TransferDate,
                Remarks = t.Remarks,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy,
                Items = t.Items.Select(ti => new TransferItemDto
                {
                    ItemId = ti.ItemId,
                    ItemName = ti.Item?.Name,
                    Quantity = ti.Quantity
                }).ToList()
            }).OrderByDescending(t => t.TransferDate);
        }


    }

}