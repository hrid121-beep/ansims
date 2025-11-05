-- =============================================
-- Simplified Physical Inventory Test Data Migration Script
-- Bangladesh Ansar & VDP IMS
-- Uses EXISTING stores, creates minimal test data
-- =============================================

USE [ansvdp_ims]
GO

PRINT 'Starting Physical Inventory Test Data Migration (Simplified)...'
GO

-- Get existing store IDs
DECLARE @StoreId1 INT = (SELECT TOP 1 Id FROM Stores WHERE IsActive = 1 ORDER BY Id)
DECLARE @StoreId2 INT = (SELECT TOP 1 Id FROM Stores WHERE IsActive = 1 AND Id > @StoreId1 ORDER BY Id)

IF @StoreId1 IS NULL OR @StoreId2 IS NULL
BEGIN
    PRINT 'ERROR: Need at least 2 active stores in the database!'
    RETURN
END

PRINT 'Using Store ID 1: ' + CAST(@StoreId1 AS NVARCHAR(10))
PRINT 'Using Store ID 2: ' + CAST(@StoreId2 AS NVARCHAR(10))

-- Get sample item IDs
DECLARE @Item1Id INT = (SELECT TOP 1 Id FROM Items WHERE IsActive = 1 ORDER BY Id)
DECLARE @Item2Id INT = (SELECT TOP 1 Id FROM Items WHERE IsActive = 1 AND Id > @Item1Id ORDER BY Id)
DECLARE @Item3Id INT = (SELECT TOP 1 Id FROM Items WHERE IsActive = 1 AND Id > @Item2Id ORDER BY Id)

IF @Item1Id IS NULL
BEGIN
    PRINT 'ERROR: Need at least 1 active item in the database!'
    RETURN
END

PRINT 'Using Item IDs: ' + CAST(@Item1Id AS NVARCHAR(10)) + ', ' + CAST(ISNULL(@Item2Id, 0) AS NVARCHAR(10)) + ', ' + CAST(ISNULL(@Item3Id, 0) AS NVARCHAR(10))

-- =============================================
-- Insert Physical Inventory Records
-- =============================================

DECLARE @Inv1Id INT, @Inv2Id INT, @Inv3Id INT, @Inv4Id INT, @Inv5Id INT

-- Inventory 1: Approved
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-0001')
BEGIN
    INSERT INTO PhysicalInventories (
        CountNo, ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, CompletedBy, CompletedDate,
        ApprovedBy, ApprovedDate,
        VarianceValue, TotalItemsCounted, ItemsWithVariance,
        IsReconciled, IsStockFrozen, IsAuditRequired,
        CountEndTime, AdjustmentCreatedDate, PostedDate,
        IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'CNT-001', 'PI-TEST-2024-25-0001', @StoreId1,
        DATEADD(DAY, -30, GETDATE()), 0, 5, '2024-25',
        'admin', DATEADD(DAY, -30, GETDATE()),
        'storemanager', DATEADD(DAY, -28, GETDATE()),
        'director', DATEADD(DAY, -27, GETDATE()),
        -5000, 2, 2, 1, 0, 0,
        DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -27, GETDATE()), DATEADD(DAY, -27, GETDATE()),
        1, DATEADD(DAY, -30, GETDATE()), 'admin'
    )
    SET @Inv1Id = SCOPE_IDENTITY()
    PRINT 'Created Approved Physical Inventory (ID: ' + CAST(@Inv1Id AS NVARCHAR(10)) + ')'
END

-- Inventory 2: In Progress
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-0002')
BEGIN
    INSERT INTO PhysicalInventories (
        CountNo, ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate,
        VarianceValue, TotalItemsCounted, ItemsWithVariance,
        IsReconciled, IsStockFrozen, IsAuditRequired,
        CountEndTime, AdjustmentCreatedDate, PostedDate,
        IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'CNT-002', 'PI-TEST-2024-25-0002', @StoreId2,
        DATEADD(DAY, -5, GETDATE()), 0, 2, '2024-25',
        'storemanager', DATEADD(DAY, -5, GETDATE()),
        0, 3, 0, 0, 0, 0,
        GETDATE(), GETDATE(), GETDATE(),
        1, DATEADD(DAY, -5, GETDATE()), 'storemanager'
    )
    SET @Inv2Id = SCOPE_IDENTITY()
    PRINT 'Created In Progress Physical Inventory (ID: ' + CAST(@Inv2Id AS NVARCHAR(10)) + ')'
END

-- Inventory 3: Under Review
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-0003')
BEGIN
    INSERT INTO PhysicalInventories (
        CountNo, ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, CompletedBy, CompletedDate,
        VarianceValue, TotalItemsCounted, ItemsWithVariance,
        IsReconciled, IsStockFrozen, IsAuditRequired,
        CountEndTime, AdjustmentCreatedDate, PostedDate,
        IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'CNT-003', 'PI-TEST-2024-25-0003', @StoreId1,
        DATEADD(DAY, -10, GETDATE()), 1, 4, '2024-25',
        'storekeeper', DATEADD(DAY, -10, GETDATE()),
        'storemanager', DATEADD(DAY, -8, GETDATE()),
        8000, 2, 1, 0, 0, 0,
        DATEADD(DAY, -8, GETDATE()), GETDATE(), GETDATE(),
        1, DATEADD(DAY, -10, GETDATE()), 'storekeeper'
    )
    SET @Inv3Id = SCOPE_IDENTITY()
    PRINT 'Created Under Review Physical Inventory (ID: ' + CAST(@Inv3Id AS NVARCHAR(10)) + ')'
END

-- Inventory 4: Scheduled
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-0004')
BEGIN
    INSERT INTO PhysicalInventories (
        CountNo, ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, Remarks,
        VarianceValue, TotalItemsCounted, ItemsWithVariance,
        IsReconciled, IsStockFrozen, IsAuditRequired,
        CountEndTime, AdjustmentCreatedDate, PostedDate,
        IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'CNT-004', 'PI-TEST-2024-25-0004', @StoreId2,
        DATEADD(DAY, 7, GETDATE()), 4, 7, '2024-25',
        'admin', GETDATE(), 'Annual physical count scheduled',
        0, 0, 0, 0, 0, 0,
        GETDATE(), GETDATE(), GETDATE(),
        1, GETDATE(), 'admin'
    )
    SET @Inv4Id = SCOPE_IDENTITY()
    PRINT 'Created Scheduled Physical Inventory (ID: ' + CAST(@Inv4Id AS NVARCHAR(10)) + ')'
END

-- Inventory 5: Initiated
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-0005')
BEGIN
    INSERT INTO PhysicalInventories (
        CountNo, ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, Remarks,
        VarianceValue, TotalItemsCounted, ItemsWithVariance,
        IsReconciled, IsStockFrozen, IsAuditRequired,
        CountEndTime, AdjustmentCreatedDate, PostedDate,
        IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'CNT-005', 'PI-TEST-2024-25-0005', @StoreId1,
        GETDATE(), 2, 1, '2024-25',
        'storemanager', GETDATE(), 'Quarterly cycle count',
        0, 0, 0, 0, 0, 0,
        GETDATE(), GETDATE(), GETDATE(),
        1, GETDATE(), 'storemanager'
    )
    SET @Inv5Id = SCOPE_IDENTITY()
    PRINT 'Created Initiated Physical Inventory (ID: ' + CAST(@Inv5Id AS NVARCHAR(10)) + ')'
END

-- Refresh IDs from database
SET @Inv1Id = (SELECT Id FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-0001')
SET @Inv2Id = (SELECT Id FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-0002')
SET @Inv3Id = (SELECT Id FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-0003')

-- =============================================
-- Insert Physical Inventory Details (Simplified)
-- =============================================

PRINT 'Inserting Physical Inventory Detail records...'

-- Details for Approved Inventory
IF @Inv1Id IS NOT NULL AND @Item1Id IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM PhysicalInventoryDetails WHERE PhysicalInventoryId = @Inv1Id)
    BEGIN
        INSERT INTO PhysicalInventoryDetails (
            PhysicalInventoryId, ItemId, SystemQuantity, PhysicalQuantity,
            FirstCountQuantity, FirstCountTime, Variance, VarianceValue, UnitPrice, Status,
            CountedBy, CountedDate, VerifiedBy, IsActive
        )
        VALUES
        (@Inv1Id, @Item1Id, 100, 98, 98, DATEADD(DAY, -28, GETDATE()), -2, -2000, 1000, 2, 'storemanager', DATEADD(DAY, -28, GETDATE()), 'director', 1)

        IF @Item2Id IS NOT NULL
            INSERT INTO PhysicalInventoryDetails (
                PhysicalInventoryId, ItemId, SystemQuantity, PhysicalQuantity,
                FirstCountQuantity, FirstCountTime, Variance, VarianceValue, UnitPrice, Status,
                CountedBy, CountedDate, VerifiedBy, IsActive
            )
            VALUES
            (@Inv1Id, @Item2Id, 200, 202, 202, DATEADD(DAY, -28, GETDATE()), 2, 1000, 500, 2, 'storemanager', DATEADD(DAY, -28, GETDATE()), 'director', 1)

        PRINT 'Added detail records for approved inventory'
    END
END

-- Details for In Progress Inventory
IF @Inv2Id IS NOT NULL AND @Item1Id IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM PhysicalInventoryDetails WHERE PhysicalInventoryId = @Inv2Id)
    BEGIN
        INSERT INTO PhysicalInventoryDetails (
            PhysicalInventoryId, ItemId, SystemQuantity, PhysicalQuantity,
            FirstCountQuantity, FirstCountTime, Variance, VarianceValue, UnitPrice, Status,
            CountedBy, CountedDate, IsActive
        )
        VALUES
        (@Inv2Id, @Item1Id, 150, 148, 148, DATEADD(DAY, -4, GETDATE()), -2, -2000, 1000, 1, 'storekeeper', DATEADD(DAY, -4, GETDATE()), 1)

        IF @Item2Id IS NOT NULL
            INSERT INTO PhysicalInventoryDetails (
                PhysicalInventoryId, ItemId, SystemQuantity, PhysicalQuantity,
                FirstCountQuantity, FirstCountTime, Variance, VarianceValue, UnitPrice, Status,
                CountedBy, CountedDate, IsActive
            )
            VALUES
            (@Inv2Id, @Item2Id, 250, 250, 250, DATEADD(DAY, -4, GETDATE()), 0, 0, 500, 1, 'storekeeper', DATEADD(DAY, -4, GETDATE()), 1)

        IF @Item3Id IS NOT NULL
            INSERT INTO PhysicalInventoryDetails (
                PhysicalInventoryId, ItemId, SystemQuantity, PhysicalQuantity,
                FirstCountQuantity, FirstCountTime, Variance, VarianceValue, UnitPrice, Status, IsActive
            )
            VALUES
            (@Inv2Id, @Item3Id, 300, 0, 0, GETDATE(), 0, 0, 800, 0, 1)

        PRINT 'Added detail records for in-progress inventory'
    END
END

-- =============================================
-- Insert Stock Movements (Simplified)
-- =============================================

PRINT 'Creating stock movement records...'

IF @Inv1Id IS NOT NULL AND @Item1Id IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM StockMovements WHERE ReferenceType = 'PhysicalCount' AND ReferenceNo = 'PI-TEST-2024-25-0001')
    BEGIN
        INSERT INTO StockMovements (
            ItemId, StoreId, MovementType, Quantity,
            OldBalance, NewBalance, ReferenceType, ReferenceNo,
            MovementDate, CreatedAt, CreatedBy, Remarks, IsActive
        )
        VALUES
        (@Item1Id, @StoreId1, 'PhysicalCount', -2, 100, 98, 'PhysicalCount', 'PI-TEST-2024-25-0001', DATEADD(DAY, -27, GETDATE()), DATEADD(DAY, -27, GETDATE()), 'director', 'Physical count adjustment', 1)

        IF @Item2Id IS NOT NULL
            INSERT INTO StockMovements (
                ItemId, StoreId, MovementType, Quantity,
                OldBalance, NewBalance, ReferenceType, ReferenceNo,
                MovementDate, CreatedAt, CreatedBy, Remarks, IsActive
            )
            VALUES
            (@Item2Id, @StoreId1, 'PhysicalCount', 2, 200, 202, 'PhysicalCount', 'PI-TEST-2024-25-0001', DATEADD(DAY, -27, GETDATE()), DATEADD(DAY, -27, GETDATE()), 'director', 'Physical count adjustment', 1)

        PRINT 'Created stock movement records'
    END
END

-- =============================================
-- Summary Report
-- =============================================

PRINT ''
PRINT '=========================================='
PRINT 'Physical Inventory Test Data Summary:'
PRINT '=========================================='

SELECT
    Status,
    COUNT(*) AS [Count]
FROM PhysicalInventories
WHERE ReferenceNumber LIKE 'PI-TEST-%'
GROUP BY Status
ORDER BY Status

PRINT ''
PRINT 'Physical Inventory Records:'
SELECT
    ReferenceNumber,
    CASE Status
        WHEN 1 THEN 'Initiated'
        WHEN 2 THEN 'InProgress'
        WHEN 3 THEN 'Completed'
        WHEN 4 THEN 'UnderReview'
        WHEN 5 THEN 'Approved'
        WHEN 6 THEN 'Rejected'
        WHEN 7 THEN 'Scheduled'
        WHEN 8 THEN 'Cancelled'
        ELSE 'Unknown'
    END AS StatusName,
    CountDate,
    InitiatedBy
FROM PhysicalInventories
WHERE ReferenceNumber LIKE 'PI-TEST-%'
ORDER BY CountDate DESC

PRINT ''
PRINT '=========================================='
PRINT 'Migration completed successfully!'
PRINT '=========================================='

GO
