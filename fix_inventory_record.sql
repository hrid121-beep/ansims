-- Fix existing Physical Inventory record (ID: 1)
-- This script updates the NULL ReferenceNumber and FiscalYear

-- First, check current state
SELECT Id, ReferenceNumber, FiscalYear, StoreId, Status, CountDate, InitiatedDate
FROM PhysicalInventories
WHERE Id = 1;

-- Update with proper values
UPDATE PhysicalInventories
SET
    ReferenceNumber = 'PI-HQ-2025-0001',
    FiscalYear = '2025-2026'
WHERE Id = 1;

-- Verify the update
SELECT Id, ReferenceNumber, FiscalYear, StoreId, Status, CountDate, InitiatedDate
FROM PhysicalInventories
WHERE Id = 1;

-- If you have multiple records with NULL values, use this query to find them:
SELECT Id, ReferenceNumber, FiscalYear, StoreId, StoreName = S.Name, Status, CountDate
FROM PhysicalInventories PI
LEFT JOIN Stores S ON PI.StoreId = S.Id
WHERE ReferenceNumber IS NULL OR FiscalYear IS NULL;
