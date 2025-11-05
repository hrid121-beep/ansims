-- ===============================================
-- Script to Seed Sample Data for IMS Database
-- Includes: Stores, Items, Stock Entries, WriteOffs
-- ===============================================

USE ansvdp_ims;
GO

-- Disable foreign key constraints temporarily
ALTER TABLE StockEntries NOCHECK CONSTRAINT ALL;
ALTER TABLE StoreItems NOCHECK CONSTRAINT ALL;
ALTER TABLE WriteOffs NOCHECK CONSTRAINT ALL;
ALTER TABLE WriteOffItems NOCHECK CONSTRAINT ALL;
GO

-- ===============================================
-- 1. INSERT SAMPLE STORES (if not exists)
-- ===============================================
PRINT 'Inserting sample stores...';

-- Check if stores exist, if not insert
IF NOT EXISTS (SELECT 1 FROM Stores WHERE Name = 'Central Store - Dhaka')
BEGIN
    INSERT INTO Stores (Name, NameBn, Code, StoreTypeId, Address, PhoneNumber, Email, IsActive, CreatedAt, CreatedBy)
    VALUES
    ('Central Store - Dhaka', 'কেন্দ্রীয় স্টোর - ঢাকা', 'CS-DHK-001', 1, 'Dhaka Cantonment', '01700000001', 'central.dhaka@ansar.gov.bd', 1, GETDATE(), 1),
    ('Provision Store - Dhaka', 'প্রভিশন স্টোর - ঢাকা', 'PS-DHK-001', 2, 'Dhaka Cantonment', '01700000002', 'provision.dhaka@ansar.gov.bd', 1, GETDATE(), 1),
    ('Battalion Store - Dhaka', 'ব্যাটালিয়ন স্টোর - ঢাকা', 'BS-DHK-001', 3, 'Dhaka City', '01700000003', 'battalion.dhaka@ansar.gov.bd', 1, GETDATE(), 1),
    ('Range Store - Chittagong', 'রেঞ্জ স্টোর - চট্টগ্রাম', 'RS-CTG-001', 3, 'Chittagong City', '01700000004', 'range.ctg@ansar.gov.bd', 1, GETDATE(), 1);

    PRINT '4 stores inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Stores already exist. Skipping...';
END
GO

-- ===============================================
-- 2. INSERT SAMPLE CATEGORIES (if not exists)
-- ===============================================
PRINT 'Inserting sample categories...';

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Uniform')
BEGIN
    INSERT INTO Categories (Name, NameBn, Description, IsActive, CreatedAt, CreatedBy)
    VALUES
    ('Uniform', 'ইউনিফর্ম', 'Ansar & VDP Uniforms', 1, GETDATE(), 1),
    ('Stationery', 'স্টেশনারি', 'Office Stationery Items', 1, GETDATE(), 1),
    ('Electronics', 'ইলেকট্রনিক্স', 'Electronic Equipment', 1, GETDATE(), 1),
    ('Furniture', 'আসবাবপত্র', 'Office Furniture', 1, GETDATE(), 1),
    ('Medical', 'চিকিৎসা', 'Medical Supplies', 1, GETDATE(), 1);

    PRINT '5 categories inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Categories already exist. Skipping...';
END
GO

-- ===============================================
-- 3. INSERT/UPDATE SAMPLE ITEMS
-- ===============================================
PRINT 'Inserting/updating sample items...';

-- Get CategoryId for each category
DECLARE @UniformCatId INT = (SELECT TOP 1 Id FROM Categories WHERE Name = 'Uniform');
DECLARE @StationeryCatId INT = (SELECT TOP 1 Id FROM Categories WHERE Name = 'Stationery');
DECLARE @ElectronicsCatId INT = (SELECT TOP 1 Id FROM Categories WHERE Name = 'Electronics');
DECLARE @FurnitureCatId INT = (SELECT TOP 1 Id FROM Categories WHERE Name = 'Furniture');
DECLARE @MedicalCatId INT = (SELECT TOP 1 Id FROM Categories WHERE Name = 'Medical');

-- Update or Insert Items
MERGE INTO Items AS target
USING (VALUES
    ('Uniform Shirt', 'ইউনিফর্ম শার্ট', 'IT-UNI-001', @UniformCatId, 'Piece', 'পিস', 'Ansar VDP Uniform Shirt', 50, 500, 100, 1200.00, 1500.00),
    ('Uniform Pant', 'ইউনিফর্ম প্যান্ট', 'IT-UNI-002', @UniformCatId, 'Piece', 'পিস', 'Ansar VDP Uniform Pant', 50, 500, 100, 1000.00, 1300.00),
    ('Uniform Cap', 'ইউনিফর্ম টুপি', 'IT-UNI-003', @UniformCatId, 'Piece', 'পিস', 'Ansar VDP Uniform Cap', 100, 1000, 200, 200.00, 300.00),
    ('Rifle 303', 'রাইফেল ৩০৩', 'IT-WPN-001', @UniformCatId, 'Piece', 'পিস', '303 Rifle', 10, 100, 20, 50000.00, 60000.00),
    ('Pen (Blue)', 'কলম (নীল)', 'IT-STA-001', @StationeryCatId, 'Piece', 'পিস', 'Blue Ball Pen', 500, 5000, 1000, 10.00, 15.00),
    ('Notebook A4', 'নোটবুক A4', 'IT-STA-002', @StationeryCatId, 'Piece', 'পিস', 'A4 Size Notebook', 200, 2000, 400, 80.00, 120.00),
    ('Printer Paper', 'প্রিন্টার কাগজ', 'IT-STA-003', @StationeryCatId, 'Ream', 'রিম', 'A4 Printer Paper', 100, 1000, 200, 350.00, 450.00),
    ('Computer Desktop', 'কম্পিউটার ডেস্কটপ', 'IT-ELC-001', @ElectronicsCatId, 'Piece', 'পিস', 'Desktop Computer', 5, 50, 10, 45000.00, 55000.00),
    ('Printer', 'প্রিন্টার', 'IT-ELC-002', @ElectronicsCatId, 'Piece', 'পিস', 'Laser Printer', 10, 100, 20, 25000.00, 32000.00),
    ('Office Table', 'অফিস টেবিল', 'IT-FUR-001', @FurnitureCatId, 'Piece', 'পিস', 'Office Desk Table', 20, 200, 40, 8000.00, 12000.00),
    ('Office Chair', 'অফিস চেয়ার', 'IT-FUR-002', @FurnitureCatId, 'Piece', 'পিস', 'Office Chair', 30, 300, 60, 3500.00, 5000.00),
    ('First Aid Box', 'প্রাথমিক চিকিৎসা বাক্স', 'IT-MED-001', @MedicalCatId, 'Box', 'বক্স', 'First Aid Kit', 50, 500, 100, 800.00, 1200.00)
) AS source (Name, NameBn, Code, CategoryId, Unit, UnitBn, Description, MinimumStock, MaximumStock, ReorderLevel, UnitPrice, SellingPrice)
ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        Name = source.Name,
        NameBn = source.NameBn,
        CategoryId = source.CategoryId,
        Unit = source.Unit,
        UnitBn = source.UnitBn,
        Description = source.Description,
        MinimumStock = source.MinimumStock,
        MaximumStock = source.MaximumStock,
        ReorderLevel = source.ReorderLevel,
        UnitPrice = source.UnitPrice,
        SellingPrice = source.SellingPrice,
        UpdatedAt = GETDATE()
WHEN NOT MATCHED THEN
    INSERT (Name, NameBn, Code, CategoryId, Unit, UnitBn, Description, MinimumStock, MaximumStock, ReorderLevel, UnitPrice, SellingPrice, IsActive, CreatedAt, CreatedBy)
    VALUES (source.Name, source.NameBn, source.Code, source.CategoryId, source.Unit, source.UnitBn, source.Description, source.MinimumStock, source.MaximumStock, source.ReorderLevel, source.UnitPrice, source.SellingPrice, 1, GETDATE(), 1);

PRINT '12 items inserted/updated successfully.';
GO

-- ===============================================
-- 4. INSERT STOCK ENTRIES
-- ===============================================
PRINT 'Inserting stock entries...';

-- Get Store IDs
DECLARE @CentralStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code = 'CS-DHK-001');
DECLARE @ProvisionStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code = 'PS-DHK-001');
DECLARE @BattalionStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code = 'BS-DHK-001');

-- Get Item IDs
DECLARE @UniformShirtId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-001');
DECLARE @UniformPantId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-002');
DECLARE @UniformCapId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-003');
DECLARE @RifleId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-WPN-001');
DECLARE @PenId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-001');
DECLARE @NotebookId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-002');
DECLARE @PrinterPaperId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-003');
DECLARE @ComputerId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-ELC-001');
DECLARE @PrinterId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-ELC-002');
DECLARE @TableId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-FUR-001');
DECLARE @ChairId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-FUR-002');
DECLARE @FirstAidId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-MED-001');

-- Delete existing stock entries for these stores (to avoid duplicates)
DELETE FROM StockEntries WHERE StoreId IN (@CentralStoreId, @ProvisionStoreId, @BattalionStoreId);

-- Insert Stock Entries
INSERT INTO StockEntries (StoreId, ItemId, Quantity, UnitPrice, TotalValue, TransactionType, ReferenceNo, Remarks, IsActive, CreatedAt, CreatedBy)
VALUES
-- Central Store Stock
(@CentralStoreId, @UniformShirtId, 300, 1200.00, 360000.00, 'Opening', 'OPEN-2025-001', 'Opening Stock', 1, GETDATE(), 1),
(@CentralStoreId, @UniformPantId, 250, 1000.00, 250000.00, 'Opening', 'OPEN-2025-001', 'Opening Stock', 1, GETDATE(), 1),
(@CentralStoreId, @UniformCapId, 500, 200.00, 100000.00, 'Opening', 'OPEN-2025-001', 'Opening Stock', 1, GETDATE(), 1),
(@CentralStoreId, @RifleId, 50, 50000.00, 2500000.00, 'Opening', 'OPEN-2025-001', 'Opening Stock', 1, GETDATE(), 1),
(@CentralStoreId, @PenId, 3000, 10.00, 30000.00, 'Opening', 'OPEN-2025-001', 'Opening Stock', 1, GETDATE(), 1),
(@CentralStoreId, @NotebookId, 1000, 80.00, 80000.00, 'Opening', 'OPEN-2025-001', 'Opening Stock', 1, GETDATE(), 1),
(@CentralStoreId, @ComputerId, 25, 45000.00, 1125000.00, 'Opening', 'OPEN-2025-001', 'Opening Stock', 1, GETDATE(), 1),

-- Provision Store Stock
(@ProvisionStoreId, @UniformShirtId, 150, 1200.00, 180000.00, 'Opening', 'OPEN-2025-002', 'Opening Stock', 1, GETDATE(), 1),
(@ProvisionStoreId, @UniformPantId, 120, 1000.00, 120000.00, 'Opening', 'OPEN-2025-002', 'Opening Stock', 1, GETDATE(), 1),
(@ProvisionStoreId, @PenId, 2000, 10.00, 20000.00, 'Opening', 'OPEN-2025-002', 'Opening Stock', 1, GETDATE(), 1),
(@ProvisionStoreId, @NotebookId, 500, 80.00, 40000.00, 'Opening', 'OPEN-2025-002', 'Opening Stock', 1, GETDATE(), 1),
(@ProvisionStoreId, @PrinterPaperId, 300, 350.00, 105000.00, 'Opening', 'OPEN-2025-002', 'Opening Stock', 1, GETDATE(), 1),

-- Battalion Store Stock
(@BattalionStoreId, @UniformShirtId, 80, 1200.00, 96000.00, 'Opening', 'OPEN-2025-003', 'Opening Stock', 1, GETDATE(), 1),
(@BattalionStoreId, @UniformCapId, 200, 200.00, 40000.00, 'Opening', 'OPEN-2025-003', 'Opening Stock', 1, GETDATE(), 1),
(@BattalionStoreId, @PenId, 1000, 10.00, 10000.00, 'Opening', 'OPEN-2025-003', 'Opening Stock', 1, GETDATE(), 1),
(@BattalionStoreId, @TableId, 50, 8000.00, 400000.00, 'Opening', 'OPEN-2025-003', 'Opening Stock', 1, GETDATE(), 1),
(@BattalionStoreId, @ChairId, 100, 3500.00, 350000.00, 'Opening', 'OPEN-2025-003', 'Opening Stock', 1, GETDATE(), 1),
(@BattalionStoreId, @FirstAidId, 150, 800.00, 120000.00, 'Opening', 'OPEN-2025-003', 'Opening Stock', 1, GETDATE(), 1);

PRINT '18 stock entries inserted successfully.';
GO

-- ===============================================
-- 5. INSERT STORE ITEMS (Current Stock Status)
-- ===============================================
PRINT 'Inserting/updating store items...';

-- Get Store and Item IDs (re-declare for this batch)
DECLARE @CentralStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code = 'CS-DHK-001');
DECLARE @ProvisionStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code = 'PS-DHK-001');
DECLARE @BattalionStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code = 'BS-DHK-001');

DECLARE @UniformShirtId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-001');
DECLARE @UniformPantId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-002');
DECLARE @UniformCapId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-003');
DECLARE @RifleId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-WPN-001');
DECLARE @PenId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-001');
DECLARE @NotebookId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-002');
DECLARE @PrinterPaperId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-003');
DECLARE @ComputerId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-ELC-001');
DECLARE @TableId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-FUR-001');
DECLARE @ChairId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-FUR-002');
DECLARE @FirstAidId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-MED-001');

-- Delete existing to avoid duplicates
DELETE FROM StoreItems WHERE StoreId IN (@CentralStoreId, @ProvisionStoreId, @BattalionStoreId);

-- Insert Current Stock Status
INSERT INTO StoreItems (StoreId, ItemId, Quantity, LastRestocked, IsActive, CreatedAt, CreatedBy)
VALUES
-- Central Store
(@CentralStoreId, @UniformShirtId, 300, GETDATE(), 1, GETDATE(), 1),
(@CentralStoreId, @UniformPantId, 250, GETDATE(), 1, GETDATE(), 1),
(@CentralStoreId, @UniformCapId, 500, GETDATE(), 1, GETDATE(), 1),
(@CentralStoreId, @RifleId, 50, GETDATE(), 1, GETDATE(), 1),
(@CentralStoreId, @PenId, 3000, GETDATE(), 1, GETDATE(), 1),
(@CentralStoreId, @NotebookId, 1000, GETDATE(), 1, GETDATE(), 1),
(@CentralStoreId, @ComputerId, 25, GETDATE(), 1, GETDATE(), 1),

-- Provision Store
(@ProvisionStoreId, @UniformShirtId, 150, GETDATE(), 1, GETDATE(), 1),
(@ProvisionStoreId, @UniformPantId, 120, GETDATE(), 1, GETDATE(), 1),
(@ProvisionStoreId, @PenId, 2000, GETDATE(), 1, GETDATE(), 1),
(@ProvisionStoreId, @NotebookId, 500, GETDATE(), 1, GETDATE(), 1),
(@ProvisionStoreId, @PrinterPaperId, 300, GETDATE(), 1, GETDATE(), 1),

-- Battalion Store
(@BattalionStoreId, @UniformShirtId, 80, GETDATE(), 1, GETDATE(), 1),
(@BattalionStoreId, @UniformCapId, 200, GETDATE(), 1, GETDATE(), 1),
(@BattalionStoreId, @PenId, 1000, GETDATE(), 1, GETDATE(), 1),
(@BattalionStoreId, @TableId, 50, GETDATE(), 1, GETDATE(), 1),
(@BattalionStoreId, @ChairId, 100, GETDATE(), 1, GETDATE(), 1),
(@BattalionStoreId, @FirstAidId, 150, GETDATE(), 1, GETDATE(), 1);

PRINT '18 store items inserted successfully.';
GO

-- ===============================================
-- 6. INSERT WRITE-OFFS
-- ===============================================
PRINT 'Inserting write-offs...';

-- Get Store and Item IDs (re-declare for this batch)
DECLARE @CentralStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code = 'CS-DHK-001');
DECLARE @ProvisionStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code = 'PS-DHK-001');

DECLARE @UniformShirtId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-001');
DECLARE @UniformCapId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-003');
DECLARE @PenId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-001');
DECLARE @NotebookId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-002');

-- Delete existing write-offs (if any)
DELETE FROM WriteOffItems WHERE WriteOffId IN (SELECT Id FROM WriteOffs WHERE WriteOffNo LIKE 'WO-2025-%');
DELETE FROM WriteOffs WHERE WriteOffNo LIKE 'WO-2025-%';

-- Insert Write-Offs
DECLARE @WriteOff1Id INT, @WriteOff2Id INT, @WriteOff3Id INT;

INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, ApprovedBy, ApprovedDate, Remarks, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-001', DATEADD(day, -10, GETDATE()), @CentralStoreId, 'Damaged', 15000.00, 'Approved', 1, DATEADD(day, -5, GETDATE()), 'Uniforms damaged during transportation', 1, DATEADD(day, -10, GETDATE()), 1);
SET @WriteOff1Id = SCOPE_IDENTITY();

INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, ApprovedBy, ApprovedDate, Remarks, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-002', DATEADD(day, -7, GETDATE()), @ProvisionStoreId, 'Expired', 4000.00, 'Approved', 1, DATEADD(day, -3, GETDATE()), 'Notebooks expired (moisture damage)', 1, DATEADD(day, -7, GETDATE()), 1);
SET @WriteOff2Id = SCOPE_IDENTITY();

INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, Remarks, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-003', DATEADD(day, -2, GETDATE()), @CentralStoreId, 'Lost', 5000.00, 'Pending', 'Items missing during stock count', 1, DATEADD(day, -2, GETDATE()), 1);
SET @WriteOff3Id = SCOPE_IDENTITY();

PRINT '3 write-offs inserted successfully.';
GO

-- ===============================================
-- 7. INSERT WRITE-OFF ITEMS
-- ===============================================
PRINT 'Inserting write-off items...';

-- Get IDs
DECLARE @WriteOff1Id INT = (SELECT TOP 1 Id FROM WriteOffs WHERE WriteOffNo = 'WO-2025-001');
DECLARE @WriteOff2Id INT = (SELECT TOP 1 Id FROM WriteOffs WHERE WriteOffNo = 'WO-2025-002');
DECLARE @WriteOff3Id INT = (SELECT TOP 1 Id FROM WriteOffs WHERE WriteOffNo = 'WO-2025-003');

DECLARE @UniformShirtId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-001');
DECLARE @UniformCapId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-UNI-003');
DECLARE @PenId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-001');
DECLARE @NotebookId INT = (SELECT TOP 1 Id FROM Items WHERE Code = 'IT-STA-002');

-- Insert Write-Off Items
INSERT INTO WriteOffItems (WriteOffId, ItemId, Quantity, UnitPrice, TotalValue, Remarks, IsActive, CreatedAt, CreatedBy)
VALUES
-- WO-2025-001 items (Damaged uniforms)
(@WriteOff1Id, @UniformShirtId, 10, 1200.00, 12000.00, 'Torn and damaged during transport', 1, DATEADD(day, -10, GETDATE()), 1),
(@WriteOff1Id, @UniformCapId, 15, 200.00, 3000.00, 'Soiled and unusable', 1, DATEADD(day, -10, GETDATE()), 1),

-- WO-2025-002 items (Expired notebooks)
(@WriteOff2Id, @NotebookId, 50, 80.00, 4000.00, 'Water damaged and unusable', 1, DATEADD(day, -7, GETDATE()), 1),

-- WO-2025-003 items (Lost items)
(@WriteOff3Id, @PenId, 500, 10.00, 5000.00, 'Missing during physical inventory', 1, DATEADD(day, -2, GETDATE()), 1);

PRINT '4 write-off items inserted successfully.';
GO

-- Re-enable foreign key constraints
ALTER TABLE StockEntries CHECK CONSTRAINT ALL;
ALTER TABLE StoreItems CHECK CONSTRAINT ALL;
ALTER TABLE WriteOffs CHECK CONSTRAINT ALL;
ALTER TABLE WriteOffItems CHECK CONSTRAINT ALL;
GO

-- ===============================================
-- SUMMARY
-- ===============================================
PRINT '';
PRINT '===============================================';
PRINT 'DATA SEEDING COMPLETED SUCCESSFULLY!';
PRINT '===============================================';
PRINT 'Summary:';
PRINT '- Stores: 4';
PRINT '- Categories: 5';
PRINT '- Items: 12';
PRINT '- Stock Entries: 18';
PRINT '- Store Items: 18';
PRINT '- Write-Offs: 3';
PRINT '- Write-Off Items: 4';
PRINT '===============================================';
PRINT '';

-- Display current stock levels
SELECT
    s.Name AS StoreName,
    i.Name AS ItemName,
    si.Quantity AS CurrentStock,
    i.MinimumStock,
    CASE
        WHEN si.Quantity <= i.MinimumStock THEN 'LOW STOCK'
        WHEN si.Quantity > i.MaximumStock THEN 'OVERSTOCK'
        ELSE 'OK'
    END AS StockStatus
FROM StoreItems si
INNER JOIN Stores s ON si.StoreId = s.Id
INNER JOIN Items i ON si.ItemId = i.Id
WHERE si.IsActive = 1
ORDER BY s.Name, i.Name;

-- Display write-offs summary
SELECT
    wo.WriteOffNo,
    wo.WriteOffDate,
    s.Name AS StoreName,
    wo.Reason,
    wo.TotalValue,
    wo.Status,
    COUNT(woi.Id) AS TotalItems
FROM WriteOffs wo
INNER JOIN Stores s ON wo.StoreId = s.Id
LEFT JOIN WriteOffItems woi ON wo.Id = woi.WriteOffId
WHERE wo.IsActive = 1
GROUP BY wo.WriteOffNo, wo.WriteOffDate, s.Name, wo.Reason, wo.TotalValue, wo.Status
ORDER BY wo.WriteOffDate DESC;
