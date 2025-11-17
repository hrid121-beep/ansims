-- Simple update for Physical Inventory record (ID: 1)
-- Just updates ReferenceNumber and FiscalYear

-- Update the record
UPDATE PhysicalInventories
SET
    ReferenceNumber = 'PI-HQ-2025-0001',
    FiscalYear = '2025-2026'
WHERE Id = 1;

-- Verify the update worked
SELECT
    Id,
    ReferenceNumber,
    FiscalYear,
    StoreId,
    Status,
    CountDate
FROM PhysicalInventories
WHERE Id = 1;

PRINT 'Update completed successfully!';
