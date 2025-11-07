-- =============================================
-- Fix Invalid ItemType Values in Items Table
-- =============================================
-- Problem: Some items have Type=0 which is not a valid ItemType enum value
-- Valid values: Expendable=1, NonExpendable=2
-- This script identifies and fixes items with invalid Type values
-- =============================================

-- Step 1: Check how many items have invalid Type values
SELECT
    COUNT(*) as InvalidItemCount,
    'Items with Type=0 (Invalid)' as Description
FROM Items
WHERE Type = 0;

-- Step 2: Show all items with invalid Type values
SELECT
    Id,
    Name,
    ItemCode,
    Code,
    Type,
    CategoryId,
    CreatedAt,
    CreatedBy
FROM Items
WHERE Type = 0
ORDER BY Id;

-- Step 3: Fix invalid Type values by setting them to Expendable (1)
-- UNCOMMENT THE FOLLOWING LINES TO EXECUTE THE FIX
/*
BEGIN TRANSACTION;

UPDATE Items
SET
    Type = 1,  -- ItemType.Expendable
    UpdatedAt = GETDATE(),
    UpdatedBy = 'System - Data Fix'
WHERE Type = 0;

-- Verify the fix
SELECT
    COUNT(*) as FixedItemCount,
    'Items updated to Expendable' as Description
FROM Items
WHERE Type = 1 AND UpdatedBy = 'System - Data Fix';

-- If everything looks good, commit the transaction
COMMIT TRANSACTION;

-- If something went wrong, rollback
-- ROLLBACK TRANSACTION;
*/

-- Step 4: Verify no items have invalid Type values
SELECT
    Type,
    COUNT(*) as ItemCount,
    CASE
        WHEN Type = 1 THEN 'Expendable (Valid)'
        WHEN Type = 2 THEN 'NonExpendable (Valid)'
        ELSE 'Invalid'
    END as TypeDescription
FROM Items
GROUP BY Type
ORDER BY Type;
