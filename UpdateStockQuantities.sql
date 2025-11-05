-- ===============================================
-- Script to Update Stock Quantities and Add WriteOffs
-- ===============================================

USE ansvdp_ims;
GO

-- Get the Central Store ID
DECLARE @CentralStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Name LIKE '%Central%' OR Name LIKE '%Headquarters%');

PRINT 'Central Store ID: ' + CAST(@CentralStoreId AS NVARCHAR(10));

-- ===============================================
-- 1. UPDATE STOCK QUANTITIES FOR EXISTING ITEMS
-- ===============================================
PRINT '';
PRINT 'Updating stock quantities...';

-- Update all StoreItems to have proper quantities
UPDATE si
SET si.Quantity = CASE
    WHEN i.Name LIKE '%Uniform%Shirt%' THEN 300
    WHEN i.Name LIKE '%Uniform%Pant%' THEN 250
    WHEN i.Name LIKE '%Cap%' THEN 500
    WHEN i.Name LIKE '%Belt%' THEN 150
    WHEN i.Name LIKE '%Pen%' THEN 2000
    WHEN i.Name LIKE '%Pencil%' THEN 1500
    WHEN i.Name LIKE '%Notebook%' THEN 800
    WHEN i.Name LIKE '%Paper%' THEN 500
    WHEN i.Name LIKE '%Folder%' THEN 400
    WHEN i.Name LIKE '%Computer%' THEN 25
    WHEN i.Name LIKE '%Printer%' THEN 15
    WHEN i.Name LIKE '%Cable%' THEN 200
    WHEN i.Name LIKE '%USB%' OR i.Name LIKE '%Flash%' THEN 150
    WHEN i.Name LIKE '%Hard Drive%' THEN 50
    WHEN i.Name LIKE '%Webcam%' THEN 20
    ELSE 100
END,
si.LastRestocked = GETDATE(),
si.UpdatedAt = GETDATE()
FROM StoreItems si
INNER JOIN Items i ON si.ItemId = i.Id
WHERE si.Quantity IS NULL OR si.Quantity = 0;

PRINT 'Stock quantities updated successfully.';
PRINT '';

-- ===============================================
-- 2. INSERT OR UPDATE WRITE-OFFS
-- ===============================================
PRINT 'Inserting/updating write-offs...';

-- Delete existing write-offs for clean slate
DELETE FROM WriteOffItems WHERE WriteOffId IN (SELECT Id FROM WriteOffs WHERE WriteOffNo LIKE 'WO-2025-%');
DELETE FROM WriteOffs WHERE WriteOffNo LIKE 'WO-2025-%';

-- Get some Item IDs for write-offs
DECLARE @UniformShirtId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Uniform%Shirt%');
DECLARE @NotebookId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Notebook%');
DECLARE @PenId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Pen%Blue%' OR Name LIKE '%Ballpoint%Blue%');

-- Insert Write-Offs
DECLARE @WriteOff1Id INT, @WriteOff2Id INT, @WriteOff3Id INT, @WriteOff4Id INT;

-- Write-Off 1: Damaged uniforms (Approved, High Value)
INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, ApprovedBy, ApprovedDate, ApprovalComments, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-001', DATEADD(day, -15, GETDATE()), @CentralStoreId, 'Damaged', 18000.00, 'Approved', '1', DATEADD(day, -10, GETDATE()), 'Uniforms damaged during transportation from manufacturer', 1, DATEADD(day, -15, GETDATE()), 1);
SET @WriteOff1Id = SCOPE_IDENTITY();

-- Write-Off 2: Expired notebooks (Approved, Medium Value)
INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, ApprovedBy, ApprovedDate, ApprovalComments, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-002', DATEADD(day, -12, GETDATE()), @CentralStoreId, 'Expired', 8000.00, 'Approved', '1', DATEADD(day, -7, GETDATE()), 'Notebooks damaged due to moisture in storage area', 1, DATEADD(day, -12, GETDATE()), 1);
SET @WriteOff2Id = SCOPE_IDENTITY();

-- Write-Off 3: Lost pens (Pending, Low Value)
INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-003', DATEADD(day, -3, GETDATE()), @CentralStoreId, 'Lost', 5000.00, 'Pending', 1, DATEADD(day, -3, GETDATE()), 1);
SET @WriteOff3Id = SCOPE_IDENTITY();

-- Write-Off 4: Fire damage (Pending, Very High Value - requires approval)
INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, RequiredApproverRole, ApprovalThreshold, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-004', DATEADD(day, -1, GETDATE()), @CentralStoreId, 'Fire Damage', 45000.00, 'Pending', 'DDGAdmin', 10000, 1, DATEADD(day, -1, GETDATE()), 1);
SET @WriteOff4Id = SCOPE_IDENTITY();

PRINT '4 write-offs inserted successfully.';
GO

-- ===============================================
-- 3. INSERT WRITE-OFF ITEMS
-- ===============================================
PRINT 'Inserting write-off items...';

-- Re-declare variables for this batch
DECLARE @WriteOff1Id INT = (SELECT TOP 1 Id FROM WriteOffs WHERE WriteOffNo = 'WO-2025-001');
DECLARE @WriteOff2Id INT = (SELECT TOP 1 Id FROM WriteOffs WHERE WriteOffNo = 'WO-2025-002');
DECLARE @WriteOff3Id INT = (SELECT TOP 1 Id FROM WriteOffs WHERE WriteOffNo = 'WO-2025-003');
DECLARE @WriteOff4Id INT = (SELECT TOP 1 Id FROM WriteOffs WHERE WriteOffNo = 'WO-2025-004');

DECLARE @CentralStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Name LIKE '%Central%' OR Name LIKE '%Headquarters%');
DECLARE @UniformShirtId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Uniform%Shirt%');
DECLARE @UniformPantId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Uniform%Pant%');
DECLARE @NotebookId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Notebook%');
DECLARE @PenId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Pen%Blue%' OR Name LIKE '%Ballpoint%Blue%');
DECLARE @ComputerId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Computer%');

-- Insert Write-Off Items
-- WO-2025-001 items (Damaged uniforms)
INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy, WriteOffRequestId)
VALUES
(@WriteOff1Id, @UniformShirtId, @CentralStoreId, 15, 1200.00, 18000.00, 18000.00, 'Torn and stained during transport', 1, DATEADD(day, -15, GETDATE()), 1, 0);

-- WO-2025-002 items (Expired notebooks)
IF @NotebookId IS NOT NULL
BEGIN
    INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy, WriteOffRequestId)
    VALUES
    (@WriteOff2Id, @NotebookId, @CentralStoreId, 100, 80.00, 8000.00, 8000.00, 'Water damaged and unusable', 1, DATEADD(day, -12, GETDATE()), 1, 0);
END

-- WO-2025-003 items (Lost pens)
IF @PenId IS NOT NULL
BEGIN
    INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy, WriteOffRequestId)
    VALUES
    (@WriteOff3Id, @PenId, @CentralStoreId, 500, 10.00, 5000.00, 5000.00, 'Missing during physical inventory count', 1, DATEADD(day, -3, GETDATE()), 1, 0);
END

-- WO-2025-004 items (Fire damage - multiple items)
INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy, WriteOffRequestId)
VALUES
(@WriteOff4Id, @UniformShirtId, @CentralStoreId, 20, 1200.00, 24000.00, 24000.00, 'Burned in storage room fire', 1, DATEADD(day, -1, GETDATE()), 1, 0);

IF @UniformPantId IS NOT NULL
BEGIN
    INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy, WriteOffRequestId)
    VALUES
    (@WriteOff4Id, @UniformPantId, @CentralStoreId, 15, 1000.00, 15000.00, 15000.00, 'Burned in storage room fire', 1, DATEADD(day, -1, GETDATE()), 1, 0);
END

IF @NotebookId IS NOT NULL
BEGIN
    INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy, WriteOffRequestId)
    VALUES
    (@WriteOff4Id, @NotebookId, @CentralStoreId, 75, 80.00, 6000.00, 6000.00, 'Burned in storage room fire', 1, DATEADD(day, -1, GETDATE()), 1, 0);
END

PRINT 'Write-off items inserted successfully.';
GO

-- ===============================================
-- SUMMARY AND VERIFICATION
-- ===============================================
PRINT '';
PRINT '===============================================';
PRINT 'DATA UPDATE COMPLETED SUCCESSFULLY!';
PRINT '===============================================';
PRINT '';

-- Show current stock levels
PRINT 'Current Stock Levels (Sample):';
SELECT TOP 10
    s.Name AS StoreName,
    i.Name AS ItemName,
    i.Code AS ItemCode,
    si.Quantity AS CurrentStock,
    i.MinimumStock,
    CASE
        WHEN si.Quantity <= i.MinimumStock THEN 'LOW STOCK'
        WHEN si.Quantity > i.MaximumStock THEN 'OVERSTOCK'
        ELSE 'NORMAL'
    END AS StockStatus
FROM StoreItems si
INNER JOIN Stores s ON si.StoreId = s.Id
INNER JOIN Items i ON si.ItemId = i.Id
WHERE si.IsActive = 1 AND si.Quantity IS NOT NULL
ORDER BY si.Quantity ASC;

PRINT '';
PRINT 'Write-Offs Summary:';
SELECT
    wo.WriteOffNo,
    CONVERT(VARCHAR(10), wo.WriteOffDate, 120) AS WriteOffDate,
    s.Name AS StoreName,
    wo.Reason,
    wo.TotalValue,
    wo.Status,
    COUNT(woi.Id) AS TotalItems,
    SUM(woi.Quantity) AS TotalQuantity
FROM WriteOffs wo
INNER JOIN Stores s ON wo.StoreId = s.Id
LEFT JOIN WriteOffItems woi ON wo.Id = woi.WriteOffId
WHERE wo.IsActive = 1 AND wo.WriteOffNo LIKE 'WO-2025-%'
GROUP BY wo.WriteOffNo, wo.WriteOffDate, s.Name, wo.Reason, wo.TotalValue, wo.Status
ORDER BY wo.WriteOffDate DESC;

PRINT '';
PRINT '===============================================';
PRINT 'Total Records:';
PRINT '- Updated StoreItems with Quantity';
PRINT '- Write-Offs: 4';
PRINT '- Write-Off Items: 6';
PRINT '===============================================';
