-- =============================================
-- Simple Fix: Update Type=0 to Type=1 (Expendable)
-- =============================================
-- Run this ONLY if you want to clean up the data
-- The application works fine WITHOUT running this!
-- =============================================

-- Step 1: See how many items will be affected
SELECT COUNT(*) as WillBeUpdated
FROM Items
WHERE Type = 0 AND IsActive = 1;

-- Step 2: See the actual items
SELECT Id, Name, ItemCode, Type
FROM Items
WHERE Type = 0 AND IsActive = 1;

-- Step 3: UPDATE (uncomment to execute)
/*
UPDATE Items
SET Type = 1,  -- Expendable
    UpdatedAt = GETDATE(),
    UpdatedBy = 'Admin - Type Fix'
WHERE Type = 0 AND IsActive = 1;

-- Check result
SELECT 'Updated successfully!' as Message,
       COUNT(*) as UpdatedCount
FROM Items
WHERE Type = 1 AND UpdatedBy = 'Admin - Type Fix';
*/
