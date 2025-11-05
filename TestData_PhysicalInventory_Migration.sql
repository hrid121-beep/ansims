-- =============================================
-- Physical Inventory Test Data Migration Script
-- Bangladesh Ansar & VDP IMS
-- =============================================

USE [ansvdp_ims]
GO

PRINT 'Starting Physical Inventory Test Data Migration...'
GO

-- =============================================
-- Step 1: Check if test stores exist, if not create sample stores
-- =============================================

IF NOT EXISTS (SELECT 1 FROM Stores WHERE Code = 'CENTRAL-HQ')
BEGIN
    PRINT 'Creating Central Store...'

    INSERT INTO Stores (Code, Name, StoreTypeId, Level, Location, IsActive, StoreKeeperAssignedDate, RequiresTemperatureControl, IsStockFrozen, StockFrozenAt, CreatedAt, CreatedBy)
    SELECT
        'CENTRAL-HQ',
        'Central Store - Headquarters',
        Id,
        0, -- HQ Level
        'Dhaka Headquarters',
        1,
        GETDATE(),
        0, -- No temperature control required
        0, -- Not frozen
        GETDATE(),
        GETDATE(),
        'admin'
    FROM StoreTypes WHERE Code = 'CENTRAL'
END

IF NOT EXISTS (SELECT 1 FROM Stores WHERE Code = 'PROV-HQ')
BEGIN
    PRINT 'Creating Provision Store...'

    INSERT INTO Stores (Code, Name, StoreTypeId, Level, Location, IsActive, StoreKeeperAssignedDate, RequiresTemperatureControl, IsStockFrozen, StockFrozenAt, CreatedAt, CreatedBy)
    SELECT
        'PROV-HQ',
        'Provision Store - Headquarters',
        Id,
        0, -- HQ Level
        'Dhaka Headquarters',
        1,
        GETDATE(),
        0, -- No temperature control required
        0, -- Not frozen
        GETDATE(),
        GETDATE(),
        'admin'
    FROM StoreTypes WHERE Code = 'PROVISION'
END

-- =============================================
-- Step 2: Get Store IDs for test data
-- =============================================

DECLARE @CentralStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code LIKE 'CENTRAL%')
DECLARE @ProvisionStoreId INT = (SELECT TOP 1 Id FROM Stores WHERE Code LIKE 'PROV%' OR StoreTypeId IN (SELECT Id FROM StoreTypes WHERE Code = 'PROVISION'))

PRINT 'Central Store ID: ' + CAST(@CentralStoreId AS NVARCHAR(10))
PRINT 'Provision Store ID: ' + CAST(@ProvisionStoreId AS NVARCHAR(10))

-- =============================================
-- Step 3: Create Physical Inventory Records
-- =============================================

PRINT 'Inserting Physical Inventory test data...'

-- Inventory 1: Approved Full Count
DECLARE @Inv1Id INT

IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-CENTRAL-2024-25-0001')
BEGIN
    INSERT INTO PhysicalInventories (
        ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, CompletedBy, CompletedDate,
        ApprovedBy, ApprovedDate, TotalSystemQuantity, TotalPhysicalQuantity,
        TotalVariance, TotalVarianceValue, IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'PI-CENTRAL-2024-25-0001',
        @CentralStoreId,
        DATEADD(DAY, -30, GETDATE()), -- 30 days ago
        0, -- Full Count
        5, -- Approved
        '2024-25',
        'admin',
        DATEADD(DAY, -30, GETDATE()),
        'storemanager',
        DATEADD(DAY, -28, GETDATE()),
        'director',
        DATEADD(DAY, -27, GETDATE()),
        1000,
        995,
        -5,
        -5000,
        1,
        DATEADD(DAY, -30, GETDATE()),
        'admin'
    )

    SET @Inv1Id = SCOPE_IDENTITY()
    PRINT 'Created Approved Physical Inventory: PI-CENTRAL-2024-25-0001 (ID: ' + CAST(@Inv1Id AS NVARCHAR(10)) + ')'
END

-- Inventory 2: In Progress Count
DECLARE @Inv2Id INT

IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-PROV-2024-25-0001')
BEGIN
    INSERT INTO PhysicalInventories (
        ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, StartTime,
        TotalSystemQuantity, TotalPhysicalQuantity,
        TotalVariance, IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'PI-PROV-2024-25-0001',
        @ProvisionStoreId,
        DATEADD(DAY, -5, GETDATE()), -- 5 days ago
        0, -- Full Count
        2, -- InProgress
        '2024-25',
        'storemanager',
        DATEADD(DAY, -5, GETDATE()),
        DATEADD(DAY, -5, GETDATE()),
        800,
        750,
        -50,
        1,
        DATEADD(DAY, -5, GETDATE()),
        'storemanager'
    )

    SET @Inv2Id = SCOPE_IDENTITY()
    PRINT 'Created In Progress Physical Inventory: PI-PROV-2024-25-0001 (ID: ' + CAST(@Inv2Id AS NVARCHAR(10)) + ')'
END

-- Inventory 3: Completed - Under Review
DECLARE @Inv3Id INT

IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-CENTRAL-2024-25-0002')
BEGIN
    INSERT INTO PhysicalInventories (
        ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, CompletedBy, CompletedDate,
        StartTime, EndTime, TotalSystemQuantity, TotalPhysicalQuantity,
        TotalVariance, TotalVarianceValue, IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'PI-CENTRAL-2024-25-0002',
        @CentralStoreId,
        DATEADD(DAY, -10, GETDATE()), -- 10 days ago
        1, -- Partial Count
        4, -- UnderReview
        '2024-25',
        'storekeeper',
        DATEADD(DAY, -10, GETDATE()),
        'storemanager',
        DATEADD(DAY, -8, GETDATE()),
        DATEADD(DAY, -10, GETDATE()),
        DATEADD(DAY, -8, GETDATE()),
        500,
        508,
        8,
        8000,
        1,
        DATEADD(DAY, -10, GETDATE()),
        'storekeeper'
    )

    SET @Inv3Id = SCOPE_IDENTITY()
    PRINT 'Created Under Review Physical Inventory: PI-CENTRAL-2024-25-0002 (ID: ' + CAST(@Inv3Id AS NVARCHAR(10)) + ')'
END

-- Inventory 4: Scheduled for Future
DECLARE @Inv4Id INT

IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-PROV-2024-25-0002')
BEGIN
    INSERT INTO PhysicalInventories (
        ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, Remarks,
        IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'PI-PROV-2024-25-0002',
        @ProvisionStoreId,
        DATEADD(DAY, 7, GETDATE()), -- 7 days from now
        4, -- Annual Count
        7, -- Scheduled
        '2024-25',
        'admin',
        GETDATE(),
        'Annual physical count scheduled for fiscal year end',
        1,
        GETDATE(),
        'admin'
    )

    SET @Inv4Id = SCOPE_IDENTITY()
    PRINT 'Created Scheduled Physical Inventory: PI-PROV-2024-25-0002 (ID: ' + CAST(@Inv4Id AS NVARCHAR(10)) + ')'
END

-- Inventory 5: Initiated - Ready to Count
DECLARE @Inv5Id INT

IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-CENTRAL-2024-25-0003')
BEGIN
    INSERT INTO PhysicalInventories (
        ReferenceNumber, StoreId, CountDate, CountType, Status,
        FiscalYear, InitiatedBy, InitiatedDate, Remarks,
        IsActive, CreatedAt, CreatedBy
    )
    VALUES (
        'PI-CENTRAL-2024-25-0003',
        @CentralStoreId,
        GETDATE(), -- Today
        2, -- Cycle Count
        1, -- Initiated
        '2024-25',
        'storemanager',
        GETDATE(),
        'Quarterly cycle count for high-value items',
        1,
        GETDATE(),
        'storemanager'
    )

    SET @Inv5Id = SCOPE_IDENTITY()
    PRINT 'Created Initiated Physical Inventory: PI-CENTRAL-2024-25-0003 (ID: ' + CAST(@Inv5Id AS NVARCHAR(10)) + ')'
END

-- =============================================
-- Step 4: Insert Sample Physical Inventory Details
-- =============================================

PRINT 'Inserting Physical Inventory Detail records...'

-- Get sample item IDs
DECLARE @Item1Id INT = (SELECT TOP 1 Id FROM Items WHERE IsActive = 1 ORDER BY Id)
DECLARE @Item2Id INT = (SELECT TOP 1 Id FROM Items WHERE IsActive = 1 AND Id > @Item1Id ORDER BY Id)
DECLARE @Item3Id INT = (SELECT TOP 1 Id FROM Items WHERE IsActive = 1 AND Id > @Item2Id ORDER BY Id)

IF @Item1Id IS NOT NULL AND @Inv1Id IS NOT NULL
BEGIN
    -- Details for Approved Inventory
    IF NOT EXISTS (SELECT 1 FROM PhysicalInventoryDetails WHERE PhysicalInventoryId = @Inv1Id)
    BEGIN
        INSERT INTO PhysicalInventoryDetails (
            PhysicalInventoryId, ItemId, SystemQuantity, PhysicalQuantity,
            Variance, VarianceValue, UnitPrice, Status,
            CountedBy, CountedDate, VerifiedBy
        )
        VALUES
        (@Inv1Id, @Item1Id, 100, 98, -2, -2000, 1000, 3, 'storemanager', DATEADD(DAY, -28, GETDATE()), 'director'),
        (@Inv1Id, @Item2Id, 200, 202, 2, 1000, 500, 3, 'storemanager', DATEADD(DAY, -28, GETDATE()), 'director')

        PRINT 'Added detail records for approved inventory'
    END
END

IF @Item1Id IS NOT NULL AND @Inv2Id IS NOT NULL
BEGIN
    -- Details for In Progress Inventory
    IF NOT EXISTS (SELECT 1 FROM PhysicalInventoryDetails WHERE PhysicalInventoryId = @Inv2Id)
    BEGIN
        INSERT INTO PhysicalInventoryDetails (
            PhysicalInventoryId, ItemId, SystemQuantity, PhysicalQuantity,
            Variance, VarianceValue, UnitPrice, Status,
            CountedBy, CountedDate
        )
        VALUES
        (@Inv2Id, @Item1Id, 150, 148, -2, -2000, 1000, 1, 'storekeeper', DATEADD(DAY, -4, GETDATE())),
        (@Inv2Id, @Item2Id, 250, 250, 0, 0, 500, 1, 'storekeeper', DATEADD(DAY, -4, GETDATE())),
        (@Inv2Id, @Item3Id, 300, 0, 0, 0, 800, 0, NULL, NULL) -- Not counted yet

        PRINT 'Added detail records for in-progress inventory'
    END
END

-- =============================================
-- Step 5: Update Stock Movement records
-- =============================================

PRINT 'Creating stock movement records for approved physical count...'

IF @Inv1Id IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM StockMovements
    WHERE ReferenceType = 'PhysicalCount'
    AND ReferenceNo = 'PI-CENTRAL-2024-25-0001'
)
BEGIN
    INSERT INTO StockMovements (
        ItemId, StoreId, MovementType, Quantity,
        OldBalance, NewBalance, ReferenceType, ReferenceNo,
        MovementDate, CreatedBy, Remarks
    )
    VALUES
    (@Item1Id, @CentralStoreId, 'PhysicalCount', -2, 100, 98, 'PhysicalCount', 'PI-CENTRAL-2024-25-0001', DATEADD(DAY, -27, GETDATE()), 'director', 'Physical count adjustment'),
    (@Item2Id, @CentralStoreId, 'PhysicalCount', 2, 200, 202, 'PhysicalCount', 'PI-CENTRAL-2024-25-0001', DATEADD(DAY, -27, GETDATE()), 'director', 'Physical count adjustment')

    PRINT 'Created stock movement records'
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
WHERE ReferenceNumber LIKE 'PI-%2024-25%'
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
    InitiatedBy,
    TotalVariance
FROM PhysicalInventories
WHERE ReferenceNumber LIKE 'PI-%2024-25%'
ORDER BY CountDate DESC

PRINT ''
PRINT '=========================================='
PRINT 'Migration completed successfully!'
PRINT '=========================================='

GO
