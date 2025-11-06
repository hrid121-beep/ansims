-- =======================================================
-- Seed Ledger Books Data
-- This script creates sample ledger books for stores
-- =======================================================

USE [ansvdp_ims]
GO

-- =======================================================
-- STEP 1: Create Sample Ledger Books for Different Stores
-- =======================================================

PRINT 'Creating sample ledger books...'

-- Check if any stores exist
DECLARE @StoreCount INT
SELECT @StoreCount = COUNT(*) FROM Stores WHERE IsActive = 1

IF @StoreCount = 0
BEGIN
    PRINT 'ERROR: No active stores found. Please create stores first.'
    RETURN
END

-- Variables for store IDs
DECLARE @CentralStoreId INT
DECLARE @ProvisionStoreId INT
DECLARE @Store1Id INT
DECLARE @Store2Id INT

-- Get store IDs (adjust these based on your actual stores)
SELECT TOP 1 @CentralStoreId = Id FROM Stores WHERE Name LIKE '%Central%' AND IsActive = 1
SELECT TOP 1 @ProvisionStoreId = Id FROM Stores WHERE Name LIKE '%Provision%' AND IsActive = 1
SELECT TOP 1 @Store1Id = Id FROM Stores WHERE IsActive = 1 ORDER BY Id
SELECT TOP 1 @Store2Id = Id FROM Stores WHERE IsActive = 1 AND Id <> @Store1Id ORDER BY Id

-- If specific stores not found, use first available store
IF @CentralStoreId IS NULL SET @CentralStoreId = @Store1Id
IF @ProvisionStoreId IS NULL SET @ProvisionStoreId = @Store2Id

PRINT 'Using Store IDs: Central=' + CAST(ISNULL(@CentralStoreId, 0) AS VARCHAR) +
      ', Provision=' + CAST(ISNULL(@ProvisionStoreId, 0) AS VARCHAR)

-- Delete existing sample ledger books (if re-running script)
DELETE FROM LedgerBooks WHERE LedgerNo LIKE 'ISS-2025-%' OR LedgerNo LIKE 'RCV-2025-%'
PRINT 'Cleaned up existing sample ledger books'

-- =======================================================
-- Create Issue Ledger Books
-- =======================================================

-- Issue Ledger Book 1 (Central Store)
IF @CentralStoreId IS NOT NULL
BEGIN
    INSERT INTO LedgerBooks (
        LedgerNo, BookName, BookType, Description, StoreId,
        TotalPages, CurrentPageNo, StartDate, IsClosed,
        Location, CreatedAt, IsActive
    ) VALUES (
        'ISS-2025-001',
        'Issue Register 2025 - Central Store',
        'Issue',
        'Main issue register for central store',
        @CentralStoreId,
        500,
        1,
        '2025-01-01',
        0,
        'Central Store - Shelf A1',
        GETDATE(),
        1
    )
    PRINT 'Created: Issue Ledger ISS-2025-001 for Central Store'
END

-- Issue Ledger Book 2 (Provision Store)
IF @ProvisionStoreId IS NOT NULL
BEGIN
    INSERT INTO LedgerBooks (
        LedgerNo, BookName, BookType, Description, StoreId,
        TotalPages, CurrentPageNo, StartDate, IsClosed,
        Location, CreatedAt, IsActive
    ) VALUES (
        'ISS-2025-002',
        'Issue Register 2025 - Provision Store',
        'Issue',
        'Main issue register for provision store',
        @ProvisionStoreId,
        500,
        1,
        '2025-01-01',
        0,
        'Provision Store - Shelf B2',
        GETDATE(),
        1
    )
    PRINT 'Created: Issue Ledger ISS-2025-002 for Provision Store'
END

-- Issue Ledger Book 3 (General - for Store 1)
IF @Store1Id IS NOT NULL
BEGIN
    INSERT INTO LedgerBooks (
        LedgerNo, BookName, BookType, Description, StoreId,
        TotalPages, CurrentPageNo, StartDate, IsClosed,
        Location, CreatedAt, IsActive
    ) VALUES (
        'ISS-2025-003',
        'Issue Register 2025 - Store 1',
        'Issue',
        'Issue register for general store operations',
        @Store1Id,
        500,
        25, -- Already used some pages
        '2025-01-01',
        0,
        'Store Room 1 - Cabinet C',
        GETDATE(),
        1
    )
    PRINT 'Created: Issue Ledger ISS-2025-003 for Store 1'
END

-- =======================================================
-- Create Receive Ledger Books
-- =======================================================

-- Receive Ledger Book 1 (Central Store)
IF @CentralStoreId IS NOT NULL
BEGIN
    INSERT INTO LedgerBooks (
        LedgerNo, BookName, BookType, Description, StoreId,
        TotalPages, CurrentPageNo, StartDate, IsClosed,
        Location, CreatedAt, IsActive
    ) VALUES (
        'RCV-2025-001',
        'Receive Register 2025 - Central Store',
        'Receive',
        'Main receive register for central store',
        @CentralStoreId,
        500,
        1,
        '2025-01-01',
        0,
        'Central Store - Shelf A2',
        GETDATE(),
        1
    )
    PRINT 'Created: Receive Ledger RCV-2025-001 for Central Store'
END

-- Receive Ledger Book 2 (Provision Store)
IF @ProvisionStoreId IS NOT NULL
BEGIN
    INSERT INTO LedgerBooks (
        LedgerNo, BookName, BookType, Description, StoreId,
        TotalPages, CurrentPageNo, StartDate, IsClosed,
        Location, CreatedAt, IsActive
    ) VALUES (
        'RCV-2025-002',
        'Receive Register 2025 - Provision Store',
        'Receive',
        'Main receive register for provision store',
        @ProvisionStoreId,
        500,
        1,
        '2025-01-01',
        0,
        'Provision Store - Shelf B3',
        GETDATE(),
        1
    )
    PRINT 'Created: Receive Ledger RCV-2025-002 for Provision Store'
END

-- Receive Ledger Book 3 (General - for Store 1)
IF @Store1Id IS NOT NULL
BEGIN
    INSERT INTO LedgerBooks (
        LedgerNo, BookName, BookType, Description, StoreId,
        TotalPages, CurrentPageNo, StartDate, IsClosed,
        Location, CreatedAt, IsActive
    ) VALUES (
        'RCV-2025-003',
        'Receive Register 2025 - Store 1',
        'Receive',
        'Receive register for general store operations',
        @Store1Id,
        500,
        15, -- Already used some pages
        '2025-01-01',
        0,
        'Store Room 1 - Cabinet D',
        GETDATE(),
        1
    )
    PRINT 'Created: Receive Ledger RCV-2025-003 for Store 1'
END

-- =======================================================
-- Create Transfer Ledger Books
-- =======================================================

-- Transfer Ledger Book (Central Store)
IF @CentralStoreId IS NOT NULL
BEGIN
    INSERT INTO LedgerBooks (
        LedgerNo, BookName, BookType, Description, StoreId,
        TotalPages, CurrentPageNo, StartDate, IsClosed,
        Location, CreatedAt, IsActive
    ) VALUES (
        'TRF-2025-001',
        'Transfer Register 2025 - Central Store',
        'Transfer',
        'Transfer register for central store',
        @CentralStoreId,
        500,
        1,
        '2025-01-01',
        0,
        'Central Store - Shelf A3',
        GETDATE(),
        1
    )
    PRINT 'Created: Transfer Ledger TRF-2025-001 for Central Store'
END

-- =======================================================
-- Create General Purpose Ledger Books
-- =======================================================

-- General Ledger Book
IF @Store1Id IS NOT NULL
BEGIN
    INSERT INTO LedgerBooks (
        LedgerNo, BookName, BookType, Description, StoreId,
        TotalPages, CurrentPageNo, StartDate, IsClosed,
        Location, CreatedAt, IsActive
    ) VALUES (
        'GEN-2025-001',
        'General Register 2025',
        'General',
        'General purpose ledger for miscellaneous transactions',
        @Store1Id,
        500,
        50, -- Half used
        '2025-01-01',
        0,
        'Store Room 1 - Cabinet E',
        GETDATE(),
        1
    )
    PRINT 'Created: General Ledger GEN-2025-001'
END

-- =======================================================
-- STEP 2: Link Existing IssueItems to Ledger Books
-- =======================================================

PRINT ''
PRINT 'Linking existing IssueItems to ledger books...'

-- Count existing IssueItems
DECLARE @IssueItemCount INT
SELECT @IssueItemCount = COUNT(*) FROM IssueItems

IF @IssueItemCount > 0
BEGIN
    -- Update IssueItems with LedgerBookId based on their StoreId
    -- Assign to corresponding Issue type ledger books

    -- First, update LedgerBookId and LedgerNo
    UPDATE ii
    SET ii.LedgerBookId = lb.Id,
        ii.LedgerNo = lb.LedgerNo
    FROM IssueItems ii
    INNER JOIN Issues i ON ii.IssueId = i.Id
    INNER JOIN LedgerBooks lb ON (
        ii.StoreId = lb.StoreId
        AND lb.BookType = 'Issue'
        AND lb.IsActive = 1
    )
    WHERE ii.LedgerBookId IS NULL

    DECLARE @UpdatedIssueItems INT = @@ROWCOUNT
    PRINT 'Updated ' + CAST(@UpdatedIssueItems AS VARCHAR) + ' IssueItems with LedgerBookId'

    -- Then, update PageNo using CTE
    ;WITH PageNumbers AS (
        SELECT
            ii.Id,
            ROW_NUMBER() OVER (PARTITION BY ii.LedgerBookId ORDER BY ii.CreatedAt) AS PageNum
        FROM IssueItems ii
        WHERE ii.LedgerBookId IS NOT NULL
    )
    UPDATE ii
    SET ii.PageNo = CAST(pn.PageNum AS VARCHAR(10))
    FROM IssueItems ii
    INNER JOIN PageNumbers pn ON ii.Id = pn.Id

    PRINT 'Assigned page numbers to IssueItems'

    -- For IssueItems without matching store, assign to first available Issue ledger
    UPDATE ii
    SET ii.LedgerBookId = (SELECT TOP 1 Id FROM LedgerBooks WHERE BookType = 'Issue' AND IsActive = 1),
        ii.LedgerNo = (SELECT TOP 1 LedgerNo FROM LedgerBooks WHERE BookType = 'Issue' AND IsActive = 1),
        ii.PageNo = CAST((SELECT COUNT(*) FROM IssueItems WHERE LedgerBookId IS NOT NULL) + 1 AS VARCHAR(10))
    FROM IssueItems ii
    WHERE ii.LedgerBookId IS NULL

    SET @UpdatedIssueItems = @@ROWCOUNT
    PRINT 'Assigned ' + CAST(@UpdatedIssueItems AS VARCHAR) + ' remaining IssueItems to default ledger'

    -- Update CurrentPageNo in LedgerBooks for Issue type
    UPDATE lb
    SET lb.CurrentPageNo = (
        SELECT ISNULL(MAX(CAST(ii.PageNo AS INT)), 0) + 1
        FROM IssueItems ii
        WHERE ii.LedgerBookId = lb.Id AND ISNUMERIC(ii.PageNo) = 1
    )
    FROM LedgerBooks lb
    WHERE lb.BookType = 'Issue' AND lb.IsActive = 1

    PRINT 'Updated CurrentPageNo for Issue ledger books'
END
ELSE
BEGIN
    PRINT 'No IssueItems found to link'
END

-- =======================================================
-- STEP 3: Link Existing ReceiveItems to Ledger Books
-- =======================================================

PRINT ''
PRINT 'Linking existing ReceiveItems to ledger books...'

-- Count existing ReceiveItems
DECLARE @ReceiveItemCount INT
SELECT @ReceiveItemCount = COUNT(*) FROM ReceiveItems

IF @ReceiveItemCount > 0
BEGIN
    -- Update ReceiveItems with LedgerBookId based on their StoreId

    -- First, update LedgerBookId and LedgerNo
    UPDATE ri
    SET ri.LedgerBookId = lb.Id,
        ri.LedgerNo = lb.LedgerNo
    FROM ReceiveItems ri
    INNER JOIN Receives r ON ri.ReceiveId = r.Id
    INNER JOIN LedgerBooks lb ON (
        ri.StoreId = lb.StoreId
        AND lb.BookType = 'Receive'
        AND lb.IsActive = 1
    )
    WHERE ri.LedgerBookId IS NULL

    DECLARE @UpdatedReceiveItems INT = @@ROWCOUNT
    PRINT 'Updated ' + CAST(@UpdatedReceiveItems AS VARCHAR) + ' ReceiveItems with LedgerBookId'

    -- Then, update PageNo using CTE
    ;WITH PageNumbers AS (
        SELECT
            ri.Id,
            ROW_NUMBER() OVER (PARTITION BY ri.LedgerBookId ORDER BY ri.CreatedAt) AS PageNum
        FROM ReceiveItems ri
        WHERE ri.LedgerBookId IS NOT NULL
    )
    UPDATE ri
    SET ri.PageNo = CAST(pn.PageNum AS VARCHAR(10))
    FROM ReceiveItems ri
    INNER JOIN PageNumbers pn ON ri.Id = pn.Id

    PRINT 'Assigned page numbers to ReceiveItems'

    -- For ReceiveItems without matching store, assign to first available Receive ledger
    UPDATE ri
    SET ri.LedgerBookId = (SELECT TOP 1 Id FROM LedgerBooks WHERE BookType = 'Receive' AND IsActive = 1),
        ri.LedgerNo = (SELECT TOP 1 LedgerNo FROM LedgerBooks WHERE BookType = 'Receive' AND IsActive = 1),
        ri.PageNo = CAST((SELECT COUNT(*) FROM ReceiveItems WHERE LedgerBookId IS NOT NULL) + 1 AS VARCHAR(10))
    FROM ReceiveItems ri
    WHERE ri.LedgerBookId IS NULL

    SET @UpdatedReceiveItems = @@ROWCOUNT
    PRINT 'Assigned ' + CAST(@UpdatedReceiveItems AS VARCHAR) + ' remaining ReceiveItems to default ledger'

    -- Update CurrentPageNo in LedgerBooks for Receive type
    UPDATE lb
    SET lb.CurrentPageNo = (
        SELECT ISNULL(MAX(CAST(ri.PageNo AS INT)), 0) + 1
        FROM ReceiveItems ri
        WHERE ri.LedgerBookId = lb.Id AND ISNUMERIC(ri.PageNo) = 1
    )
    FROM LedgerBooks lb
    WHERE lb.BookType = 'Receive' AND lb.IsActive = 1

    PRINT 'Updated CurrentPageNo for Receive ledger books'
END
ELSE
BEGIN
    PRINT 'No ReceiveItems found to link'
END

-- =======================================================
-- STEP 4: Summary Report
-- =======================================================

PRINT ''
PRINT '============================================'
PRINT '         LEDGER BOOKS SUMMARY'
PRINT '============================================'
PRINT ''

-- Show created ledger books
SELECT
    LedgerNo,
    BookName,
    BookType,
    s.Name AS StoreName,
    TotalPages,
    CurrentPageNo,
    (TotalPages - CurrentPageNo + 1) AS PagesRemaining,
    CASE
        WHEN IsClosed = 1 THEN 'Closed'
        WHEN (TotalPages - CurrentPageNo + 1) <= 10 THEN 'Almost Full'
        WHEN (TotalPages - CurrentPageNo + 1) <= 50 THEN 'Low'
        ELSE 'Active'
    END AS Status,
    Location
FROM LedgerBooks lb
LEFT JOIN Stores s ON lb.StoreId = s.Id
WHERE lb.IsActive = 1
ORDER BY BookType, LedgerNo

PRINT ''
PRINT 'Statistics:'
PRINT '----------'

-- Count by type
SELECT
    BookType,
    COUNT(*) AS TotalBooks,
    SUM(TotalPages) AS TotalPages,
    SUM(CurrentPageNo - 1) AS PagesUsed,
    SUM(TotalPages - CurrentPageNo + 1) AS PagesRemaining
FROM LedgerBooks
WHERE IsActive = 1
GROUP BY BookType
ORDER BY BookType

-- IssueItems with ledger books
DECLARE @IssueItemsWithLedger INT
SELECT @IssueItemsWithLedger = COUNT(*) FROM IssueItems WHERE LedgerBookId IS NOT NULL
PRINT ''
PRINT 'IssueItems with LedgerBookId: ' + CAST(@IssueItemsWithLedger AS VARCHAR)

-- ReceiveItems with ledger books
DECLARE @ReceiveItemsWithLedger INT
SELECT @ReceiveItemsWithLedger = COUNT(*) FROM ReceiveItems WHERE LedgerBookId IS NOT NULL
PRINT 'ReceiveItems with LedgerBookId: ' + CAST(@ReceiveItemsWithLedger AS VARCHAR)

PRINT ''
PRINT '============================================'
PRINT '         SCRIPT COMPLETED SUCCESSFULLY'
PRINT '============================================'
PRINT ''
PRINT 'Next Steps:'
PRINT '1. Test the application'
PRINT '2. Go to: Left Sidebar > Store Management > Ledger Books'
PRINT '3. Verify all ledger books are listed'
PRINT '4. Test Issue/Receive forms with ledger book dropdown'
PRINT ''

GO
