-- =============================================
-- Allotment Letter Bengali Fields Migration
-- Run this script manually if dotnet ef database update fails
-- =============================================

USE [ansvdp_ims];
GO

-- Check if migration has already been applied
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20251101000000_AddBengaliFieldsToAllotmentLetter')
BEGIN
    PRINT 'Applying migration: AddBengaliFieldsToAllotmentLetter';

    -- =============================================
    -- AllotmentLetters Table
    -- =============================================
    PRINT 'Adding Bengali fields to AllotmentLetters table...';

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'Subject')
        ALTER TABLE [AllotmentLetters] ADD [Subject] NVARCHAR(500) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'SubjectBn')
        ALTER TABLE [AllotmentLetters] ADD [SubjectBn] NVARCHAR(500) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'BodyText')
        ALTER TABLE [AllotmentLetters] ADD [BodyText] NVARCHAR(MAX) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'BodyTextBn')
        ALTER TABLE [AllotmentLetters] ADD [BodyTextBn] NVARCHAR(MAX) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'CollectionDeadline')
        ALTER TABLE [AllotmentLetters] ADD [CollectionDeadline] DATETIME2 NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'SignatoryName')
        ALTER TABLE [AllotmentLetters] ADD [SignatoryName] NVARCHAR(200) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'SignatoryDesignation')
        ALTER TABLE [AllotmentLetters] ADD [SignatoryDesignation] NVARCHAR(200) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'SignatoryDesignationBn')
        ALTER TABLE [AllotmentLetters] ADD [SignatoryDesignationBn] NVARCHAR(200) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'SignatoryId')
        ALTER TABLE [AllotmentLetters] ADD [SignatoryId] NVARCHAR(50) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'SignatoryPhone')
        ALTER TABLE [AllotmentLetters] ADD [SignatoryPhone] NVARCHAR(50) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'SignatoryEmail')
        ALTER TABLE [AllotmentLetters] ADD [SignatoryEmail] NVARCHAR(100) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetters') AND [name] = 'BengaliDate')
        ALTER TABLE [AllotmentLetters] ADD [BengaliDate] NVARCHAR(100) NULL;

    -- =============================================
    -- AllotmentLetterItems Table
    -- =============================================
    PRINT 'Adding Bengali fields to AllotmentLetterItems table...';

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetterItems') AND [name] = 'ItemNameBn')
        ALTER TABLE [AllotmentLetterItems] ADD [ItemNameBn] NVARCHAR(500) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetterItems') AND [name] = 'UnitBn')
        ALTER TABLE [AllotmentLetterItems] ADD [UnitBn] NVARCHAR(50) NULL;

    -- =============================================
    -- AllotmentLetterRecipients Table
    -- =============================================
    PRINT 'Adding Bengali fields to AllotmentLetterRecipients table...';

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetterRecipients') AND [name] = 'RecipientNameBn')
        ALTER TABLE [AllotmentLetterRecipients] ADD [RecipientNameBn] NVARCHAR(500) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetterRecipients') AND [name] = 'StaffStrength')
        ALTER TABLE [AllotmentLetterRecipients] ADD [StaffStrength] NVARCHAR(50) NULL;

    -- =============================================
    -- AllotmentLetterRecipientItems Table
    -- =============================================
    PRINT 'Adding Bengali fields to AllotmentLetterRecipientItems table...';

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetterRecipientItems') AND [name] = 'ItemNameBn')
        ALTER TABLE [AllotmentLetterRecipientItems] ADD [ItemNameBn] NVARCHAR(500) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID('AllotmentLetterRecipientItems') AND [name] = 'UnitBn')
        ALTER TABLE [AllotmentLetterRecipientItems] ADD [UnitBn] NVARCHAR(50) NULL;

    -- =============================================
    -- Insert Migration History Record
    -- =============================================
    PRINT 'Adding migration to history...';

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20251101000000_AddBengaliFieldsToAllotmentLetter', '9.0.7');

    PRINT 'Migration completed successfully!';
END
ELSE
BEGIN
    PRINT 'Migration already applied. Skipping...';
END
GO

-- =============================================
-- Verification: Check if all columns exist
-- =============================================
PRINT '';
PRINT '=== Verification ===';
PRINT 'Checking if all Bengali fields were added successfully...';

-- AllotmentLetters
SELECT COUNT(*) AS AllotmentLetters_BengaliFields_Count
FROM sys.columns
WHERE [object_id] = OBJECT_ID('AllotmentLetters')
    AND [name] IN ('Subject', 'SubjectBn', 'BodyText', 'BodyTextBn', 'CollectionDeadline',
                   'SignatoryName', 'SignatoryDesignation', 'SignatoryDesignationBn',
                   'SignatoryId', 'SignatoryPhone', 'SignatoryEmail', 'BengaliDate');

-- AllotmentLetterItems
SELECT COUNT(*) AS AllotmentLetterItems_BengaliFields_Count
FROM sys.columns
WHERE [object_id] = OBJECT_ID('AllotmentLetterItems')
    AND [name] IN ('ItemNameBn', 'UnitBn');

-- AllotmentLetterRecipients
SELECT COUNT(*) AS AllotmentLetterRecipients_BengaliFields_Count
FROM sys.columns
WHERE [object_id] = OBJECT_ID('AllotmentLetterRecipients')
    AND [name] IN ('RecipientNameBn', 'StaffStrength');

-- AllotmentLetterRecipientItems
SELECT COUNT(*) AS AllotmentLetterRecipientItems_BengaliFields_Count
FROM sys.columns
WHERE [object_id] = OBJECT_ID('AllotmentLetterRecipientItems')
    AND [name] IN ('ItemNameBn', 'UnitBn');

PRINT '';
PRINT 'Expected counts:';
PRINT '  AllotmentLetters: 12 fields';
PRINT '  AllotmentLetterItems: 2 fields';
PRINT '  AllotmentLetterRecipients: 2 fields';
PRINT '  AllotmentLetterRecipientItems: 2 fields';
GO
