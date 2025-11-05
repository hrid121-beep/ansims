USE ansvdp_ims;
GO

-- Insert Ranges
INSERT INTO Ranges (Name, Code, HeadquarterLocation, CommanderName, CommanderRank, ContactNumber, Email, CoverageArea, NameBn, CreatedAt, IsActive)
VALUES
('Dhaka Range', 'DR-01', 'Dhaka Cantonment', 'Brigadier General Md. Kamrul Hassan', 'Brigadier General', '01700-111111', 'dhaka.range@ansar.gov.bd', 'Dhaka Division', NULL, GETDATE(), 1),
('Rajshahi Range', 'RR-01', 'Rajshahi Cantonment', 'Colonel Md. Habibur Rahman', 'Colonel', '01700-222222', 'rajshahi.range@ansar.gov.bd', 'Rajshahi Division', NULL, GETDATE(), 1);
GO

-- Insert Battalions
INSERT INTO Battalions (Name, Code, Type, Location, CommanderName, CommanderRank, ContactNumber, Email, RangeId, TotalPersonnel, OfficerCount, EnlistedCount, NameBn, CreatedAt, IsActive)
SELECT TOP 1
'1st Ansar Battalion', 'AB-01', 0, 'Mirpur, Dhaka', 'Lt. Colonel Md. Jahangir Alam', 'Lt. Colonel', '01700-101001', 'ab01@ansar.gov.bd', Id, 500, 25, 475, NULL, GETDATE(), 1
FROM Ranges WHERE Code = 'DR-01';
GO

-- Insert Categories
INSERT INTO Categories (Name, NameBn, Code, Description, CreatedAt, IsActive)
VALUES
('IT Equipment', NULL, 'CAT-IT', 'Information Technology Equipment', GETDATE(), 1),
('Office Equipment', NULL, 'CAT-OE', 'General Office Equipment', GETDATE(), 1),
('Uniforms & Clothing', NULL, 'CAT-UC', 'Uniforms, Clothing and Accessories', GETDATE(), 1);
GO

-- Insert Items
INSERT INTO Items (Name, NameBn, ItemCode, Unit, Type, Status, SubCategoryId, CategoryId, MinimumStock, MaximumStock, ReorderLevel, UnitPrice, HasExpiry, RequiresSpecialHandling, IsHazardous, RequiresPersonalIssue, IsAnsarAuthorized, IsVDPAuthorized, CreatedAt, IsActive)
SELECT TOP 1
'Desktop Computer Core i5', NULL, 'IT-DESK-001', 'Piece', 1, 0, Id, Id, 5, 50, 10, 45000.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1
FROM Categories WHERE Code = 'CAT-IT';
GO

INSERT INTO Items (Name, NameBn, ItemCode, Unit, Type, Status, SubCategoryId, CategoryId, MinimumStock, MaximumStock, ReorderLevel, UnitPrice, HasExpiry, RequiresSpecialHandling, IsHazardous, RequiresPersonalIssue, IsAnsarAuthorized, IsVDPAuthorized, CreatedAt, IsActive)
SELECT TOP 1
'Laser Printer A4', NULL, 'IT-PRNT-001', 'Piece', 1, 0, Id, Id, 3, 30, 5, 18000.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1
FROM Categories WHERE Code = 'CAT-IT';
GO

-- Insert Vendors
INSERT INTO Vendors (Name, VendorType, ContactPerson, Phone, Email, Mobile, Address, TIN, BIN, CreatedAt, IsActive)
VALUES
('Computer Point Ltd.', 'IT Equipment Supplier', 'Md. Shahabuddin Ahmed', '02-9876543', 'info@computerpoint.com.bd', '01711-123456', '45/2 Eskaton Garden, Dhaka-1000', '123456789012', '000123456789', GETDATE(), 1);
GO

-- Insert Stores
DECLARE @StoreTypeId INT;
SELECT TOP 1 @StoreTypeId = Id FROM StoreTypes WHERE Type = 'CENTRAL';

INSERT INTO Stores (Name, NameBn, Code, Type, Status, Location, Address, Description, InCharge, ContactNumber, Email, StoreKeeperName, StoreKeeperId, StoreKeeperAssignedDate, Level, StoreTypeId, RequiresTemperatureControl, IsStockFrozen, StockFrozenAt, CreatedAt, IsActive)
VALUES
('Central Store - Headquarters', NULL, 'CS-HQ-01', 'CENTRAL', 'Active', 'Headquarters', 'Ansar & VDP HQ, Dhaka-1000', 'Main Central Store', 'Md. Altaf Hossain', '01700-501001', 'central.hq@ansar.gov.bd', 'Abdul Karim', 'SK001', GETDATE(), 1, @StoreTypeId, 0, 0, GETDATE(), GETDATE(), 1);
GO

PRINT 'Basic seed data inserted successfully!';
