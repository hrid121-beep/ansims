USE ansvdp_ims;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Insert Items
INSERT INTO Items (Name, ItemCode, Unit, Type, Status, SubCategoryId, CategoryId, MinimumStock, MaximumStock, ReorderLevel, UnitPrice, HasExpiry, RequiresSpecialHandling, IsHazardous, RequiresPersonalIssue, IsAnsarAuthorized, IsVDPAuthorized, CreatedAt, IsActive)
SELECT TOP 1
'Desktop Computer Core i5', 'IT-DESK-001', 'Piece', 1, 0, Id, Id, 5, 50, 10, 45000.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1
FROM Categories WHERE Code = 'CAT-IT';
GO

INSERT INTO Items (Name, ItemCode, Unit, Type, Status, SubCategoryId, CategoryId, MinimumStock, MaximumStock, ReorderLevel, UnitPrice, HasExpiry, RequiresSpecialHandling, IsHazardous, RequiresPersonalIssue, IsAnsarAuthorized, IsVDPAuthorized, CreatedAt, IsActive)
SELECT TOP 1
'Laser Printer A4', 'IT-PRNT-001', 'Piece', 1, 0, Id, Id, 3, 30, 5, 18000.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1
FROM Categories WHERE Code = 'CAT-IT';
GO

INSERT INTO Items (Name, ItemCode, Unit, Type, Status, SubCategoryId, CategoryId, MinimumStock, MaximumStock, ReorderLevel, UnitPrice, HasExpiry, RequiresSpecialHandling, IsHazardous, RequiresPersonalIssue, IsAnsarAuthorized, IsVDPAuthorized, CreatedAt, IsActive)
SELECT TOP 1
'A4 Paper 80GSM (Ream)', 'ST-PAPR-001', 'Ream', 0, 0, Id, Id, 200, 2000, 300, 450.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1
FROM Categories WHERE Code = 'CAT-OE';
GO

-- Insert Store
INSERT INTO Stores (Name, Code, Status, Location, Address, Description, InCharge, ContactNumber, Email, StoreKeeperName, StoreKeeperId, StoreKeeperAssignedDate, Level, StoreTypeId, RequiresTemperatureControl, IsStockFrozen, StockFrozenAt, CreatedAt, IsActive)
VALUES
('Central Store - Headquarters', 'CS-HQ-01', 'Active', 'Headquarters', 'Ansar & VDP HQ, Dhaka-1000', 'Main Central Store', 'Md. Altaf Hossain', '01700-501001', 'central.hq@ansar.gov.bd', 'Abdul Karim', 'SK001', GETDATE(), 1, 1, 0, 0, GETDATE(), GETDATE(), 1);
GO

-- Insert a Purchase
DECLARE @VendorId INT, @StoreId INT;
SELECT TOP 1 @VendorId = Id FROM Vendors;
SELECT TOP 1 @StoreId = Id FROM Stores;

INSERT INTO Purchases (PurchaseOrderNo, PurchaseDate, ExpectedDeliveryDate, VendorId, StoreId, TotalAmount, Discount, PurchaseType, Status, Remarks, ProcurementType, CreatedAt, IsActive)
VALUES
('PO-2024-001', DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -15, GETDATE()), @VendorId, @StoreId, 1170000.00, 0.00, 'Direct', 'Received', 'IT Equipment for HQ', 1, DATEADD(DAY, -30, GETDATE()), 1);
GO

-- Insert Purchase Items
DECLARE @PurchaseId INT;
DECLARE @Item1Id INT, @Item2Id INT, @Item3Id INT;

SELECT TOP 1 @PurchaseId = Id FROM Purchases ORDER BY Id DESC;
SELECT @Item1Id = Id FROM Items WHERE ItemCode = 'IT-DESK-001';
SELECT @Item2Id = Id FROM Items WHERE ItemCode = 'IT-PRNT-001';
SELECT @Item3Id = Id FROM Items WHERE ItemCode = 'ST-PAPR-001';

INSERT INTO PurchaseItems (PurchaseId, ItemId, Quantity, UnitPrice, TotalPrice, ReceivedQuantity, AcceptedQuantity, CreatedAt, IsActive)
VALUES
(@PurchaseId, @Item1Id, 10, 45000.00, 450000.00, 10, 10, DATEADD(DAY, -30, GETDATE()), 1),
(@PurchaseId, @Item2Id, 20, 18000.00, 360000.00, 20, 20, DATEADD(DAY, -30, GETDATE()), 1),
(@PurchaseId, @Item3Id, 800, 450.00, 360000.00, 800, 800, DATEADD(DAY, -30, GETDATE()), 1);
GO

PRINT 'Items, Store, Purchase data inserted successfully!';
