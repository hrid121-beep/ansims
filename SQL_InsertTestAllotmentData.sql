-- =============================================
-- Insert Test Allotment Letter Data
-- Date: 2025-11-01
-- Purpose: Test cascading dropdown and government format
-- =============================================

USE ansvdp_ims;
GO

DECLARE @AllotmentLetterId INT;
DECLARE @FromStoreId INT;
DECLARE @RecipientId INT;

-- Get a Provision Store ID (or any store)
SELECT TOP 1 @FromStoreId = Id FROM Stores WHERE IsActive = 1;

IF @FromStoreId IS NULL
BEGIN
    PRINT 'ERROR: No active store found!';
    RETURN;
END

PRINT 'Using Store ID: ' + CAST(@FromStoreId AS VARCHAR);
PRINT '';

-- =============================================
-- 1. INSERT ALLOTMENT LETTER
-- =============================================
PRINT 'Creating Allotment Letter...';

INSERT INTO AllotmentLetters (
    AllotmentNo,
    AllotmentDate,
    FromStoreId,
    ValidFrom,
    ValidUntil,
    Purpose,
    ReferenceNo,
    Status,

    -- Government Format Fields
    Subject,
    SubjectBn,
    BodyText,
    BodyTextBn,
    CollectionDeadline,
    SignatoryName,
    SignatoryDesignation,
    SignatoryDesignationBn,
    SignatoryId,
    SignatoryPhone,
    SignatoryEmail,
    BengaliDate,

    -- Audit fields
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES (
    'ALLOT-TEST-001',
    GETDATE(),
    @FromStoreId,
    GETDATE(),
    DATEADD(MONTH, 3, GETDATE()),
    'Test allotment for cascading dropdown verification and government format printing',
    '৪৪.০৩.০০০০.০১৮.১৩.০০১.২৫',
    'Approved',

    -- Government Format
    'Allotment of Kit Bags and Uniforms (New Design)',
    'কিট ব্যাগ ও ইউনিফর্ম (নতুন নকশা) বরাদ্দ প্রদান প্রসঙ্গে',
    'Distribution of newly designed kit bags and uniforms to battalions and upazilas as per approved allocation plan.',
    'নতুন নকশার কিট ব্যাগ ও ইউনিফর্ম বিতরণ করা হচ্ছে। অনুগ্রহপূর্বক নির্ধারিত সময়ের মধ্যে সংগ্রহ করার জন্য অনুরোধ করা হলো।',
    DATEADD(DAY, 15, GETDATE()),
    'এ.বি.এম. ফরহাদ, বিবিএম',
    'Deputy Director (Store)',
    'উপপরিচালক (ভান্ডার)',
    'বিএমভি-১২০২১৮',
    '০২-৭২১৩৪০০',
    'ddstore@ansarvdp.gov.bd',
    'কার্তিক ১৪৩২ বঙ্গাব্দ',

    GETDATE(),
    'admin',
    1
);

SET @AllotmentLetterId = SCOPE_IDENTITY();
PRINT 'Allotment Letter created with ID: ' + CAST(@AllotmentLetterId AS VARCHAR);
PRINT '';

-- =============================================
-- 2. INSERT RECIPIENTS (Different Types)
-- =============================================

-- Get some entity IDs for testing
DECLARE @RangeId INT, @BattalionId1 INT, @BattalionId2 INT;
DECLARE @ZilaId INT, @UpazilaId INT, @UnionId INT;

SELECT TOP 1 @RangeId = Id FROM Ranges WHERE IsActive = 1;
SELECT TOP 2 @BattalionId1 = Id FROM Battalions WHERE IsActive = 1 AND RangeId = @RangeId ORDER BY Id;
SELECT @BattalionId2 = Id FROM Battalions WHERE IsActive = 1 AND RangeId = @RangeId AND Id > @BattalionId1 ORDER BY Id;
SELECT TOP 1 @ZilaId = Id FROM Zilas WHERE IsActive = 1;
SELECT TOP 1 @UpazilaId = Id FROM Upazilas WHERE IsActive = 1 AND ZilaId = @ZilaId;
SELECT TOP 1 @UnionId = Id FROM Unions WHERE IsActive = 1 AND UpazilaId = @UpazilaId;

PRINT 'Creating Recipients...';
PRINT '  Range ID: ' + ISNULL(CAST(@RangeId AS VARCHAR), 'NULL');
PRINT '  Battalion IDs: ' + ISNULL(CAST(@BattalionId1 AS VARCHAR), 'NULL') + ', ' + ISNULL(CAST(@BattalionId2 AS VARCHAR), 'NULL');
PRINT '  Zila ID: ' + ISNULL(CAST(@ZilaId AS VARCHAR), 'NULL');
PRINT '  Upazila ID: ' + ISNULL(CAST(@UpazilaId AS VARCHAR), 'NULL');
PRINT '  Union ID: ' + ISNULL(CAST(@UnionId AS VARCHAR), 'NULL');
PRINT '';

-- Recipient 1: Direct Battalion (Type: Battalion)
IF @BattalionId1 IS NOT NULL
BEGIN
    INSERT INTO AllotmentLetterRecipients (
        AllotmentLetterId, RecipientType, SerialNo,
        BattalionId, RecipientName, RecipientNameBn, StaffStrength,
        CreatedAt, CreatedBy, IsActive
    )
    SELECT
        @AllotmentLetterId, 'Battalion', 1,
        Id, Name, NameBn, '৫০০',
        GETDATE(), 'admin', 1
    FROM Battalions
    WHERE Id = @BattalionId1;

    SET @RecipientId = SCOPE_IDENTITY();
    PRINT 'Recipient 1 created (Battalion Direct): ID ' + CAST(@RecipientId AS VARCHAR);
END

-- Recipient 2: Another Battalion from same Range (Type: Range)
IF @BattalionId2 IS NOT NULL
BEGIN
    INSERT INTO AllotmentLetterRecipients (
        AllotmentLetterId, RecipientType, SerialNo,
        BattalionId, RecipientName, RecipientNameBn, StaffStrength,
        CreatedAt, CreatedBy, IsActive
    )
    SELECT
        @AllotmentLetterId, 'Range', 2,
        Id, Name, NameBn, '৪৫০',
        GETDATE(), 'admin', 1
    FROM Battalions
    WHERE Id = @BattalionId2;

    SET @RecipientId = SCOPE_IDENTITY();
    PRINT 'Recipient 2 created (Range→Battalion): ID ' + CAST(@RecipientId AS VARCHAR);
END

-- Recipient 3: Upazila (Type: Zila)
IF @UpazilaId IS NOT NULL
BEGIN
    INSERT INTO AllotmentLetterRecipients (
        AllotmentLetterId, RecipientType, SerialNo,
        UpazilaId, RecipientName, RecipientNameBn, StaffStrength,
        CreatedAt, CreatedBy, IsActive
    )
    SELECT
        @AllotmentLetterId, 'Zila', 3,
        Id, Name, NameBn, '২০০',
        GETDATE(), 'admin', 1
    FROM Upazilas
    WHERE Id = @UpazilaId;

    SET @RecipientId = SCOPE_IDENTITY();
    PRINT 'Recipient 3 created (Zila→Upazila): ID ' + CAST(@RecipientId AS VARCHAR);
END

-- Recipient 4: Union (Type: Upazila)
IF @UnionId IS NOT NULL
BEGIN
    INSERT INTO AllotmentLetterRecipients (
        AllotmentLetterId, RecipientType, SerialNo,
        UnionId, RecipientName, RecipientNameBn, StaffStrength,
        CreatedAt, CreatedBy, IsActive
    )
    SELECT
        @AllotmentLetterId, 'Upazila', 4,
        Id, Name, NameBangla, '১০০',
        GETDATE(), 'admin', 1
    FROM Unions
    WHERE Id = @UnionId;

    SET @RecipientId = SCOPE_IDENTITY();
    PRINT 'Recipient 4 created (Upazila→Union): ID ' + CAST(@RecipientId AS VARCHAR);
END

PRINT '';

-- =============================================
-- 3. INSERT ITEMS FOR EACH RECIPIENT
-- =============================================

PRINT 'Adding items to recipients...';

-- Get some item IDs
DECLARE @Item1Id INT, @Item2Id INT, @Item3Id INT;
SELECT TOP 1 @Item1Id = Id FROM Items WHERE IsActive = 1 AND Name LIKE '%Kit%';
SELECT TOP 1 @Item2Id = Id FROM Items WHERE IsActive = 1 AND Name LIKE '%Uniform%';
SELECT TOP 1 @Item3Id = Id FROM Items WHERE IsActive = 1 AND (@Item1Id IS NULL OR Id <> @Item1Id) AND (@Item2Id IS NULL OR Id <> @Item2Id);

-- If no specific items found, get any 3 items
IF @Item1Id IS NULL SELECT TOP 1 @Item1Id = Id FROM Items WHERE IsActive = 1 ORDER BY Id;
IF @Item2Id IS NULL SELECT TOP 1 @Item2Id = Id FROM Items WHERE IsActive = 1 AND Id > ISNULL(@Item1Id, 0) ORDER BY Id;
IF @Item3Id IS NULL SELECT TOP 1 @Item3Id = Id FROM Items WHERE IsActive = 1 AND Id > ISNULL(@Item2Id, 0) ORDER BY Id;

PRINT '  Item IDs: ' + ISNULL(CAST(@Item1Id AS VARCHAR), 'NULL') + ', ' + ISNULL(CAST(@Item2Id AS VARCHAR), 'NULL') + ', ' + ISNULL(CAST(@Item3Id AS VARCHAR), 'NULL');

-- Add items to each recipient
DECLARE @RecipId INT;
DECLARE recipient_cursor CURSOR FOR
    SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetterId;

OPEN recipient_cursor;
FETCH NEXT FROM recipient_cursor INTO @RecipId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Add Item 1 (e.g., Kit Bag)
    IF @Item1Id IS NOT NULL
    BEGIN
        INSERT INTO AllotmentLetterRecipientItems (
            AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity,
            ItemNameBn, Unit, UnitBn, Remarks,
            CreatedAt, CreatedBy, IsActive
        )
        SELECT
            @RecipId, Id, 100, 0,
            NameBn, Unit, 'টি', 'New design',
            GETDATE(), 'admin', 1
        FROM Items
        WHERE Id = @Item1Id;
    END

    -- Add Item 2 (e.g., Uniform)
    IF @Item2Id IS NOT NULL
    BEGIN
        INSERT INTO AllotmentLetterRecipientItems (
            AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity,
            ItemNameBn, Unit, UnitBn, Remarks,
            CreatedAt, CreatedBy, IsActive
        )
        SELECT
            @RecipId, Id, 200, 0,
            NameBn, Unit, 'পিস', 'Standard issue',
            GETDATE(), 'admin', 1
        FROM Items
        WHERE Id = @Item2Id;
    END

    -- Add Item 3 (random item)
    IF @Item3Id IS NOT NULL
    BEGIN
        INSERT INTO AllotmentLetterRecipientItems (
            AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity,
            ItemNameBn, Unit, UnitBn, Remarks,
            CreatedAt, CreatedBy, IsActive
        )
        SELECT
            @RecipId, Id, 50, 0,
            NameBn, Unit, 'সেট', 'Additional',
            GETDATE(), 'admin', 1
        FROM Items
        WHERE Id = @Item3Id;
    END

    PRINT '  Items added to recipient ID: ' + CAST(@RecipId AS VARCHAR);

    FETCH NEXT FROM recipient_cursor INTO @RecipId;
END

CLOSE recipient_cursor;
DEALLOCATE recipient_cursor;

PRINT '';

-- =============================================
-- 4. INSERT SUMMARY ITEMS (AllotmentLetterItems)
-- =============================================

PRINT 'Creating summary items...';

-- Aggregate items from all recipients
INSERT INTO AllotmentLetterItems (
    AllotmentLetterId, ItemId, AllottedQuantity, IssuedQuantity, RemainingQuantity,
    Unit, UnitBn, ItemNameBn, Remarks,
    CreatedAt, CreatedBy, IsActive
)
SELECT
    @AllotmentLetterId,
    ri.ItemId,
    SUM(ri.AllottedQuantity) AS TotalQuantity,
    0 AS IssuedQuantity,
    SUM(ri.AllottedQuantity) AS RemainingQuantity,
    MAX(ri.Unit) AS Unit,
    MAX(ri.UnitBn) AS UnitBn,
    MAX(ri.ItemNameBn) AS ItemNameBn,
    'Aggregated from ' + CAST(COUNT(DISTINCT r.Id) AS VARCHAR) + ' recipients' AS Remarks,
    GETDATE(),
    'admin',
    1
FROM AllotmentLetterRecipients r
INNER JOIN AllotmentLetterRecipientItems ri ON r.Id = ri.AllotmentLetterRecipientId
WHERE r.AllotmentLetterId = @AllotmentLetterId
GROUP BY ri.ItemId;

PRINT 'Summary items created!';
PRINT '';

-- =============================================
-- 5. VERIFICATION
-- =============================================

PRINT '========================================';
PRINT 'TEST DATA CREATED SUCCESSFULLY! ✓';
PRINT '========================================';
PRINT '';
PRINT 'Allotment Letter Details:';
PRINT '  ID: ' + CAST(@AllotmentLetterId AS VARCHAR);
PRINT '  Allotment No: ALLOT-TEST-001';
PRINT '  Reference No: ৪৪.০৩.০০০০.০১৮.১৩.০০১.২৫';
PRINT '  Status: Approved';
PRINT '';

SELECT
    COUNT(*) AS TotalRecipients,
    SUM(CASE WHEN RecipientType = 'Battalion' THEN 1 ELSE 0 END) AS DirectBattalions,
    SUM(CASE WHEN RecipientType = 'Range' THEN 1 ELSE 0 END) AS RangeBattalions,
    SUM(CASE WHEN RecipientType = 'Zila' THEN 1 ELSE 0 END) AS ZilaUpazilas,
    SUM(CASE WHEN RecipientType = 'Upazila' THEN 1 ELSE 0 END) AS UpazilaUnions
FROM AllotmentLetterRecipients
WHERE AllotmentLetterId = @AllotmentLetterId;

PRINT '';
PRINT 'Total Items Per Recipient:';
SELECT
    r.SerialNo,
    r.RecipientType,
    r.RecipientName,
    COUNT(ri.Id) AS ItemCount,
    SUM(ri.AllottedQuantity) AS TotalQuantity
FROM AllotmentLetterRecipients r
LEFT JOIN AllotmentLetterRecipientItems ri ON r.Id = ri.AllotmentLetterRecipientId
WHERE r.AllotmentLetterId = @AllotmentLetterId
GROUP BY r.SerialNo, r.RecipientType, r.RecipientName
ORDER BY r.SerialNo;

PRINT '';
PRINT '========================================';
PRINT 'Next Steps:';
PRINT '1. Navigate to: /AllotmentLetter/Index';
PRINT '2. Find: ALLOT-TEST-001';
PRINT '3. Click Details to view';
PRINT '4. Click "Print (Government Format)"';
PRINT '5. Verify cascading dropdown data';
PRINT '========================================';
GO
