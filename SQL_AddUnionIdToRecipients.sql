-- =============================================
-- Add UnionId to AllotmentLetterRecipients Table
-- Date: 2025-11-01
-- Purpose: Support Union as a recipient type in cascading hierarchy
-- =============================================

USE ansvdp_ims;
GO

-- Check if column already exists
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'AllotmentLetterRecipients'
    AND COLUMN_NAME = 'UnionId'
)
BEGIN
    PRINT 'Adding UnionId column to AllotmentLetterRecipients table...';

    ALTER TABLE AllotmentLetterRecipients
    ADD UnionId INT NULL;

    PRINT 'UnionId column added successfully! ✓';

    -- Add foreign key constraint
    IF NOT EXISTS (
        SELECT 1
        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
        WHERE CONSTRAINT_NAME = 'FK_AllotmentLetterRecipients_Unions_UnionId'
    )
    BEGIN
        PRINT 'Adding foreign key constraint...';

        ALTER TABLE AllotmentLetterRecipients
        ADD CONSTRAINT FK_AllotmentLetterRecipients_Unions_UnionId
        FOREIGN KEY (UnionId) REFERENCES Unions(Id);

        PRINT 'Foreign key constraint added successfully! ✓';
    END
END
ELSE
BEGIN
    PRINT 'UnionId column already exists. Skipping...';
END
GO

-- Verify the change
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AllotmentLetterRecipients'
  AND COLUMN_NAME IN ('RangeId', 'BattalionId', 'ZilaId', 'UpazilaId', 'UnionId')
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT '========================================';
PRINT 'Migration completed successfully! ✓';
PRINT '========================================';
PRINT 'AllotmentLetterRecipients table now supports:';
PRINT '  - Range → Battalions (cascading)';
PRINT '  - Battalion (direct)';
PRINT '  - Zila → Upazilas (cascading)';
PRINT '  - Upazila → Unions (cascading)';
PRINT '  - Union (direct)';
PRINT '========================================';
GO
