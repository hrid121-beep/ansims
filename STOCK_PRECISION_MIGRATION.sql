-- ============================================
-- Stock Precision Optimization Migration
-- Purpose: Reduce unnecessary decimal places
-- ============================================

-- BACKUP FIRST!
-- CREATE TABLE Items_Backup AS SELECT * FROM Items;

USE ansvdp_ims;
GO

-- Option A: Change to 2 decimal places (Recommended for general use)
ALTER TABLE Items
ALTER COLUMN MinimumStock decimal(18, 2);

ALTER TABLE Items
ALTER COLUMN MaximumStock decimal(18, 2);

ALTER TABLE Items
ALTER COLUMN ReorderLevel decimal(18, 2);

-- Keep currency fields at 2 decimals (already standard)
-- UnitPrice, UnitCost should already be decimal(18, 2)

-- Weight can stay at 3 decimals (precision needed)
-- Weight decimal(18, 3) -- Keep as is

GO

-- Option B: If you want integer-only for countable items
-- (Uncomment if needed)
/*
ALTER TABLE Items
ALTER COLUMN MinimumStock int;

ALTER TABLE Items
ALTER COLUMN MaximumStock int;

ALTER TABLE Items
ALTER COLUMN ReorderLevel int;
*/

-- Verify changes
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    NUMERIC_PRECISION,
    NUMERIC_SCALE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Items'
AND COLUMN_NAME IN ('MinimumStock', 'MaximumStock', 'ReorderLevel', 'UnitPrice', 'UnitCost', 'Weight');
