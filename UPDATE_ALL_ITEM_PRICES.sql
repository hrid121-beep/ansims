-- =====================================================
-- Set UnitCost for ALL Items with NULL prices
-- =====================================================

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT 'Setting UnitCost for all items...'

-- IT Equipment
UPDATE Items SET UnitCost = 150.00 WHERE ItemCode = 'IT-CABL-001' AND (UnitCost IS NULL OR UnitCost = 0); -- HDMI Cable
UPDATE Items SET UnitCost = 25000.00 WHERE ItemCode = 'IT-PRNT-001' AND (UnitCost IS NULL OR UnitCost = 0); -- Laser Printer
UPDATE Items SET UnitCost = 800.00 WHERE ItemCode = 'IT-USB-001' AND (UnitCost IS NULL OR UnitCost = 0); -- USB Flash Drive 32GB

-- Stationery
UPDATE Items SET UnitCost = 15.00 WHERE ItemCode = 'ST-FILE-001' AND (UnitCost IS NULL OR UnitCost = 0); -- File Folder
UPDATE Items SET UnitCost = 45.00 WHERE ItemCode = 'ST-NOTE-001' AND (UnitCost IS NULL OR UnitCost = 0); -- Notebook 100 Pages
UPDATE Items SET UnitCost = 350.00 WHERE ItemCode = 'ST-PAPR-001' AND (UnitCost IS NULL OR UnitCost = 0); -- A4 Paper (Ream)
UPDATE Items SET UnitCost = 10.00 WHERE ItemCode = 'ST-PEN-002' AND (UnitCost IS NULL OR UnitCost = 0); -- Ballpoint Pen Black
UPDATE Items SET UnitCost = 5.00 WHERE ItemCode = 'ST-PENC-001' AND (UnitCost IS NULL OR UnitCost = 0); -- Pencil HB

PRINT ''
PRINT '=== All Items with Prices ==='
SELECT ItemCode, Name, UnitCost
FROM Items
WHERE IsActive = 1
ORDER BY ItemCode;

PRINT ''
PRINT '=== Summary ==='
SELECT
    COUNT(*) AS TotalActiveItems,
    SUM(CASE WHEN UnitCost IS NULL OR UnitCost = 0 THEN 1 ELSE 0 END) AS ItemsWithoutPrice,
    SUM(CASE WHEN UnitCost > 0 THEN 1 ELSE 0 END) AS ItemsWithPrice,
    AVG(UnitCost) AS AveragePrice,
    MIN(UnitCost) AS MinPrice,
    MAX(UnitCost) AS MaxPrice
FROM Items
WHERE IsActive = 1;
