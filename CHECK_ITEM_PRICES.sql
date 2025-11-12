-- Check prices of items in requisition
SELECT 
    Id,
    ItemCode,
    Name,
    UnitCost,
    CASE 
        WHEN UnitCost IS NULL THEN 'NULL - No Price Set'
        WHEN UnitCost = 0 THEN 'ZERO - Price is 0'
        ELSE 'OK - Price Set'
    END AS PriceStatus
FROM Items
WHERE ItemCode IN ('IT-DESK-001', 'ST-PEN-001', 'IT-CABL-002')
ORDER BY ItemCode;

-- Check all items with NULL or ZERO prices
SELECT 
    COUNT(*) AS TotalItems,
    SUM(CASE WHEN UnitCost IS NULL THEN 1 ELSE 0 END) AS NullPriceCount,
    SUM(CASE WHEN UnitCost = 0 THEN 1 ELSE 0 END) AS ZeroPriceCount,
    SUM(CASE WHEN UnitCost > 0 THEN 1 ELSE 0 END) AS WithPriceCount
FROM Items
WHERE IsActive = 1;
