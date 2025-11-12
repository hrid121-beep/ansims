-- =====================================================
-- Quick Fix: Set UnitCost for Items with 0 or NULL prices
-- =====================================================

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT 'Setting UnitCost for items...'

-- Set prices for the 3 items in your requisition
UPDATE Items SET UnitCost = 45000.00 WHERE ItemCode = 'IT-DESK-001'; -- Desktop Computer
UPDATE Items SET UnitCost = 10.00 WHERE ItemCode = 'ST-PEN-001';     -- Ballpoint Pen
UPDATE Items SET UnitCost = 50.00 WHERE ItemCode = 'IT-CABL-002';    -- Network Cable

-- Set default prices for other IT Equipment (if UnitCost is NULL or 0)
UPDATE Items SET UnitCost = 35000.00 WHERE ItemCode = 'IT-WCAM-001' AND (UnitCost IS NULL OR UnitCost = 0); -- Webcam
UPDATE Items SET UnitCost = 150000.00 WHERE ItemCode = 'IT-HDD-001' AND (UnitCost IS NULL OR UnitCost = 0); -- External HDD

-- Set default prices for Uniforms (if UnitCost is NULL or 0)
UPDATE Items SET UnitCost = 500.00 WHERE CategoryId IN (SELECT Id FROM Categories WHERE Name LIKE '%Uniform%' OR Name LIKE '%Clothing%') AND (UnitCost IS NULL OR UnitCost = 0);

-- Check results
PRINT ''
PRINT '=== Updated Items ==='
SELECT ItemCode, Name, UnitCost
FROM Items
WHERE ItemCode IN ('IT-DESK-001', 'ST-PEN-001', 'IT-CABL-002')
ORDER BY ItemCode;

PRINT ''
PRINT '=== Price Status Summary ==='
SELECT 
    COUNT(*) AS TotalActiveItems,
    SUM(CASE WHEN UnitCost IS NULL OR UnitCost = 0 THEN 1 ELSE 0 END) AS ItemsWithoutPrice,
    SUM(CASE WHEN UnitCost > 0 THEN 1 ELSE 0 END) AS ItemsWithPrice,
    AVG(UnitCost) AS AveragePrice,
    MIN(UnitCost) AS MinPrice,
    MAX(UnitCost) AS MaxPrice
FROM Items
WHERE IsActive = 1;
