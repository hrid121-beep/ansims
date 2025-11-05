-- =============================================
-- Physical Inventory Test Data - Header Records Only
-- Bangladesh Ansar & VDP IMS
-- Creates test physical inventory header records for testing Count History
-- Details can be added through the application
-- =============================================

USE [ansvdp_ims]
GO

PRINT '==================================================='
PRINT 'Physical Inventory Test Data Migration (Header Only)'
PRINT '==================================================='
GO

-- Get existing store IDs
DECLARE @StoreId1 INT = (SELECT TOP 1 Id FROM Stores WHERE IsActive = 1 ORDER BY Id)
DECLARE @StoreId2 INT = (SELECT TOP 1 Id FROM Stores WHERE IsActive = 1 AND Id > @StoreId1 ORDER BY Id)

IF @StoreId1 IS NULL OR @StoreId2 IS NULL
BEGIN
    PRINT 'ERROR: Need at least 2 active stores in the database!'
    RETURN
END

PRINT 'Using Store 1 ID: ' + CAST(@StoreId1 AS NVARCHAR(10))
PRINT 'Using Store 2 ID: ' + CAST(@StoreId2 AS NVARCHAR(10))
PRINT ''

-- =============================================
-- Insert Physical Inventory Header Records
-- =============================================

-- Inventory 1: Approved
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-001')
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
        'CNT-TEST-001', 'PI-TEST-2024-25-001', @StoreId1,
        DATEADD(DAY, -30, GETDATE()), 0, 5, '2024-25',
        'admin', DATEADD(DAY, -30, GETDATE()),
        'storemanager', DATEADD(DAY, -28, GETDATE()),
        'director', DATEADD(DAY, -27, GETDATE()),
        -5000, 2, 2, 1, 0, 0,
        DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -27, GETDATE()), DATEADD(DAY, -27, GETDATE()),
        1, DATEADD(DAY, -30, GETDATE()), 'admin'
    )
    PRINT '✓ Created: PI-TEST-2024-25-001 (Approved)'
END
ELSE
    PRINT '- Skipped: PI-TEST-2024-25-001 (Already exists)'

-- Inventory 2: In Progress
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-002')
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
        'CNT-TEST-002', 'PI-TEST-2024-25-002', @StoreId2,
        DATEADD(DAY, -5, GETDATE()), 0, 2, '2024-25',
        'storemanager', DATEADD(DAY, -5, GETDATE()),
        0, 0, 0, 0, 0, 0,
        GETDATE(), GETDATE(), GETDATE(),
        1, DATEADD(DAY, -5, GETDATE()), 'storemanager'
    )
    PRINT '✓ Created: PI-TEST-2024-25-002 (In Progress)'
END
ELSE
    PRINT '- Skipped: PI-TEST-2024-25-002 (Already exists)'

-- Inventory 3: Under Review
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-003')
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
        'CNT-TEST-003', 'PI-TEST-2024-25-003', @StoreId1,
        DATEADD(DAY, -10, GETDATE()), 1, 4, '2024-25',
        'storekeeper', DATEADD(DAY, -10, GETDATE()),
        'storemanager', DATEADD(DAY, -8, GETDATE()),
        8000, 2, 1, 0, 0, 0,
        DATEADD(DAY, -8, GETDATE()), GETDATE(), GETDATE(),
        1, DATEADD(DAY, -10, GETDATE()), 'storekeeper'
    )
    PRINT '✓ Created: PI-TEST-2024-25-003 (Under Review)'
END
ELSE
    PRINT '- Skipped: PI-TEST-2024-25-003 (Already exists)'

-- Inventory 4: Scheduled
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-004')
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
        'CNT-TEST-004', 'PI-TEST-2024-25-004', @StoreId2,
        DATEADD(DAY, 7, GETDATE()), 4, 7, '2024-25',
        'admin', GETDATE(), 'Annual physical count scheduled for fiscal year end',
        0, 0, 0, 0, 0, 0,
        GETDATE(), GETDATE(), GETDATE(),
        1, GETDATE(), 'admin'
    )
    PRINT '✓ Created: PI-TEST-2024-25-004 (Scheduled)'
END
ELSE
    PRINT '- Skipped: PI-TEST-2024-25-004 (Already exists)'

-- Inventory 5: Initiated
IF NOT EXISTS (SELECT 1 FROM PhysicalInventories WHERE ReferenceNumber = 'PI-TEST-2024-25-005')
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
        'CNT-TEST-005', 'PI-TEST-2024-25-005', @StoreId1,
        GETDATE(), 2, 1, '2024-25',
        'storemanager', GETDATE(), 'Quarterly cycle count for high-value items',
        0, 0, 0, 0, 0, 0,
        GETDATE(), GETDATE(), GETDATE(),
        1, GETDATE(), 'storemanager'
    )
    PRINT '✓ Created: PI-TEST-2024-25-005 (Initiated)'
END
ELSE
    PRINT '- Skipped: PI-TEST-2024-25-005 (Already exists)'

PRINT ''
PRINT '==================================================='
PRINT 'Migration Summary'
PRINT '==================================================='

-- Summary Report
SELECT
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
    END AS Status,
    COUNT(*) AS [Count]
FROM PhysicalInventories
WHERE ReferenceNumber LIKE 'PI-TEST-%'
GROUP BY Status
ORDER BY Status

PRINT ''
PRINT 'Physical Inventory Records Created:'

SELECT
    ReferenceNumber AS [Reference],
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
    END AS Status,
    FORMAT(CountDate, 'dd-MMM-yyyy') AS [Count Date],
    InitiatedBy AS [Initiated By],
    CASE CountType
        WHEN 0 THEN 'Full'
        WHEN 1 THEN 'Partial'
        WHEN 2 THEN 'Cycle'
        WHEN 3 THEN 'Spot'
        WHEN 4 THEN 'Annual'
        ELSE 'Unknown'
    END AS [Type]
FROM PhysicalInventories
WHERE ReferenceNumber LIKE 'PI-TEST-%'
ORDER BY CountDate DESC

PRINT ''
PRINT '==================================================='
PRINT 'SUCCESS! Test data created successfully!'
PRINT '==================================================='
PRINT ''
PRINT 'Next Steps:'
PRINT '1. Navigate to Physical Count → Count History'
PRINT '2. You should see 5 test physical inventory records'
PRINT '3. Test the filtering, export, and detail views'
PRINT '4. Use "Start Count" to add item details to the Initiated count'
PRINT ''

GO
