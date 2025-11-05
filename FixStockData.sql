-- ===============================================
-- Simple Script to Fix Stock Data
-- ===============================================

USE ansvdp_ims;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

PRINT 'Starting data fix...';
PRINT '';

-- ===============================================
-- 1. UPDATE STOCK QUANTITIES
-- ===============================================
PRINT '1. Updating stock quantities in StoreItems...';

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
si.CurrentStock = CASE
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
si.UpdatedAt = GETDATE(),
si.LastStockUpdate = GETDATE()
FROM StoreItems si
INNER JOIN Items i ON si.ItemId = i.Id
WHERE si.Quantity IS NULL OR si.Quantity = 0;

DECLARE @UpdatedRows INT = @@ROWCOUNT;
PRINT CAST(@UpdatedRows AS NVARCHAR(10)) + ' stock items updated.';
PRINT '';

-- ===============================================
-- 2. DELETE OLD WRITE-OFFS (for clean slate)
-- ===============================================
PRINT '2. Cleaning up old test write-offs...';

DELETE FROM WriteOffItems WHERE WriteOffId IN (SELECT Id FROM WriteOffs WHERE WriteOffNo LIKE 'WO-2025-%' OR WriteOffNo LIKE 'WO-2024-%');
DELETE FROM WriteOffs WHERE WriteOffNo LIKE 'WO-2025-%' OR WriteOffNo LIKE 'WO-2024-%';

PRINT 'Old write-offs cleaned up.';
PRINT '';

-- ===============================================
-- 3. INSERT WRITE-OFFS (All in one transaction)
-- ===============================================
PRINT '3. Inserting new write-offs...';

BEGIN TRANSACTION;

DECLARE @CentralStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Name LIKE '%Central%' OR Name LIKE '%Headquarters%');
DECLARE @UniformShirtId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Uniform%Shirt%');
DECLARE @UniformPantId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Uniform%Pant%');
DECLARE @NotebookId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Notebook%');
DECLARE @PenId INT = (SELECT TOP 1 Id FROM Items WHERE Name LIKE '%Pen%Blue%' OR Name LIKE '%Ballpoint%Blue%');

-- Write-Off 1: Damaged uniforms (Approved)
INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, ApprovedBy, ApprovedDate, ApprovalComments, ApprovalThreshold, NotificationSent, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-001', DATEADD(day, -15, GETDATE()), @CentralStoreId, 'Damaged', 18000.00, 'Approved', '1', DATEADD(day, -10, GETDATE()), 'Uniforms damaged during transportation', 10000, 1, 1, DATEADD(day, -15, GETDATE()), 1);

DECLARE @WO1 INT = SCOPE_IDENTITY();

INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy)
VALUES
(@WO1, @UniformShirtId, @CentralStoreId, 15, 1200.00, 18000.00, 18000.00, 'Torn and stained during transport', 1, DATEADD(day, -15, GETDATE()), 1);

-- Write-Off 2: Expired notebooks (Approved)
INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, ApprovedBy, ApprovedDate, ApprovalComments, ApprovalThreshold, NotificationSent, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-002', DATEADD(day, -12, GETDATE()), @CentralStoreId, 'Expired', 8000.00, 'Approved', '1', DATEADD(day, -7, GETDATE()), 'Notebooks damaged due to moisture', 0, 1, 1, DATEADD(day, -12, GETDATE()), 1);

DECLARE @WO2 INT = SCOPE_IDENTITY();

IF @NotebookId IS NOT NULL
BEGIN
    INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy)
    VALUES
    (@WO2, @NotebookId, @CentralStoreId, 100, 80.00, 8000.00, 8000.00, 'Water damaged and unusable', 1, DATEADD(day, -12, GETDATE()), 1);
END

-- Write-Off 3: Lost pens (Pending)
INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, ApprovedDate, ApprovalThreshold, NotificationSent, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-003', DATEADD(day, -3, GETDATE()), @CentralStoreId, 'Lost', 5000.00, 'Pending', '1900-01-01', 0, 0, 1, DATEADD(day, -3, GETDATE()), 1);

DECLARE @WO3 INT = SCOPE_IDENTITY();

IF @PenId IS NOT NULL
BEGIN
    INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy)
    VALUES
    (@WO3, @PenId, @CentralStoreId, 500, 10.00, 5000.00, 5000.00, 'Missing during inventory count', 1, DATEADD(day, -3, GETDATE()), 1);
END

-- Write-Off 4: Fire damage (Pending, High Value)
INSERT INTO WriteOffs (WriteOffNo, WriteOffDate, StoreId, Reason, TotalValue, Status, RequiredApproverRole, ApprovalThreshold, ApprovedDate, NotificationSent, IsActive, CreatedAt, CreatedBy)
VALUES
('WO-2025-004', DATEADD(day, -1, GETDATE()), @CentralStoreId, 'Fire Damage', 45000.00, 'Pending', 'DDGAdmin', 10000, '1900-01-01', 0, 1, DATEADD(day, -1, GETDATE()), 1);

DECLARE @WO4 INT = SCOPE_IDENTITY();

-- Multiple items for fire damage
INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy)
VALUES
(@WO4, @UniformShirtId, @CentralStoreId, 20, 1200.00, 24000.00, 24000.00, 'Burned in storage fire', 1, DATEADD(day, -1, GETDATE()), 1);

IF @UniformPantId IS NOT NULL
BEGIN
    INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy)
    VALUES
    (@WO4, @UniformPantId, @CentralStoreId, 15, 1000.00, 15000.00, 15000.00, 'Burned in storage fire', 1, DATEADD(day, -1, GETDATE()), 1);
END

IF @NotebookId IS NOT NULL
BEGIN
    INSERT INTO WriteOffItems (WriteOffId, ItemId, StoreId, Quantity, UnitCost, Value, TotalCost, Reason, IsActive, CreatedAt, CreatedBy)
    VALUES
    (@WO4, @NotebookId, @CentralStoreId, 75, 80.00, 6000.00, 6000.00, 'Burned in storage fire', 1, DATEADD(day, -1, GETDATE()), 1);
END

COMMIT TRANSACTION;

PRINT '4 write-offs with 6 items inserted successfully.';
PRINT '';

-- ===============================================
-- 4. VERIFICATION
-- ===============================================
PRINT '===============================================';
PRINT 'DATA FIX COMPLETED!';
PRINT '===============================================';
PRINT '';

PRINT 'Sample Stock Levels (First 15):';
SELECT TOP 15
    s.Name AS StoreName,
    i.Name AS ItemName,
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
WHERE si.IsActive = 1 AND si.Quantity IS NOT NULL AND si.Quantity > 0
ORDER BY s.Name, i.Name;

PRINT '';
PRINT 'Write-Offs Created:';
SELECT
    wo.WriteOffNo,
    FORMAT(wo.WriteOffDate, 'yyyy-MM-dd') AS Date,
    s.Name AS StoreName,
    wo.Reason,
    wo.TotalValue,
    wo.Status,
    COUNT(woi.Id) AS Items
FROM WriteOffs wo
INNER JOIN Stores s ON wo.StoreId = s.Id
LEFT JOIN WriteOffItems woi ON wo.Id = woi.WriteOffId
WHERE wo.IsActive = 1 AND wo.WriteOffNo LIKE 'WO-2025-%'
GROUP BY wo.WriteOffNo, wo.WriteOffDate, s.Name, wo.Reason, wo.TotalValue, wo.Status
ORDER BY wo.WriteOffDate DESC;

PRINT '';
PRINT '===============================================';
