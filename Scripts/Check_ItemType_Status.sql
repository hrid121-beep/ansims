-- =============================================
-- Quick Check: How many items have Type=0?
-- =============================================
-- Just run this query to see the current status

-- Summary view
SELECT
    Type,
    COUNT(*) as ItemCount,
    CASE
        WHEN Type = 0 THEN '⚠️ Invalid (Will default to Expendable in UI)'
        WHEN Type = 1 THEN '✅ Expendable (Valid)'
        WHEN Type = 2 THEN '✅ NonExpendable (Valid)'
        ELSE '❌ Unknown'
    END as Status
FROM Items
WHERE IsActive = 1
GROUP BY Type
ORDER BY Type;

-- If you want to see which items have Type=0:
SELECT TOP 10
    Id,
    Name,
    ItemCode,
    Type,
    CategoryId
FROM Items
WHERE Type = 0 AND IsActive = 1;
