-- =========================================================================
-- SAMPLE ALLOTMENT LETTERS - SIMPLE VERSION
-- =========================================================================
-- This script uses existing Items and Stores from your database
-- No new items will be created
-- =========================================================================

SET NOCOUNT ON;

PRINT '=============================================='
PRINT 'Finding Valid Store and Items...'
PRINT '=============================================='

-- Find a valid Store
DECLARE @CentralStoreId INT;
SELECT TOP 1 @CentralStoreId = Id FROM Stores WHERE IsActive = 1 ORDER BY Id;

IF @CentralStoreId IS NULL
BEGIN
    PRINT 'ERROR: No active stores found!'
    PRINT 'Please create at least one store first using:'
    PRINT 'INSERT INTO Stores (Name, Type, Location, CreatedAt, CreatedBy, IsActive)'
    PRINT 'VALUES (''Central Store'', ''CENTRAL'', ''Headquarters'', GETDATE(), ''admin'', 1);'
    RETURN;
END

PRINT 'Store Found - ID: ' + CAST(@CentralStoreId AS VARCHAR(10))

-- Find valid Items (get first 8)
DECLARE @ItemIds TABLE (RowNum INT, ItemId INT, ItemName NVARCHAR(200));
INSERT INTO @ItemIds (RowNum, ItemId, ItemName)
SELECT TOP 8
    ROW_NUMBER() OVER (ORDER BY Id),
    Id,
    ISNULL(NameBn, Name)
FROM Items
WHERE IsActive = 1
ORDER BY Id;

DECLARE @ItemCount INT = (SELECT COUNT(*) FROM @ItemIds);

IF @ItemCount < 3
BEGIN
    PRINT 'ERROR: Need at least 3 active items in the database!'
    PRINT 'Found only ' + CAST(@ItemCount AS VARCHAR(10)) + ' items.'
    PRINT 'Please add more items to Items table first.'
    RETURN;
END

PRINT 'Items Found: ' + CAST(@ItemCount AS VARCHAR(10))

-- Get individual item IDs
DECLARE @Item1 INT = (SELECT ItemId FROM @ItemIds WHERE RowNum = 1);
DECLARE @Item2 INT = (SELECT ItemId FROM @ItemIds WHERE RowNum = 2);
DECLARE @Item3 INT = (SELECT ItemId FROM @ItemIds WHERE RowNum = 3);
DECLARE @Item4 INT = (SELECT ItemId FROM @ItemIds WHERE RowNum = 4);
DECLARE @Item5 INT = (SELECT ItemId FROM @ItemIds WHERE RowNum = 5);
DECLARE @Item6 INT = (SELECT ItemId FROM @ItemIds WHERE RowNum = 6);
DECLARE @Item7 INT = (SELECT ItemId FROM @ItemIds WHERE RowNum = 7);
DECLARE @Item8 INT = (SELECT ItemId FROM @ItemIds WHERE RowNum = 8);

DECLARE @CurrentUser NVARCHAR(100) = 'admin';
DECLARE @Now DATETIME = GETDATE();

PRINT ''
PRINT '=============================================='
PRINT 'Creating Allotment Letter 1...'
PRINT '=============================================='

-- =========================================================================
-- ALLOTMENT LETTER 1: Multiple Recipients
-- =========================================================================

INSERT INTO AllotmentLetters (
    AllotmentNo, AllotmentDate, ValidFrom, ValidUntil,
    IssuedTo, IssuedToType, FromStoreId, Purpose, Status,
    ApprovedBy, ApprovedDate, ReferenceNo,
    Subject, SubjectBn, BodyText, BodyTextBn,
    SignatoryName, SignatoryDesignation, SignatoryDesignationBn,
    SignatoryPhone, SignatoryEmail,
    CreatedAt, CreatedBy, IsActive
)
VALUES (
    'AL-202511-001', '2025-11-10', '2025-11-10', '2026-05-10',
    'Multiple Recipients', 'Multiple', @CentralStoreId,
    N'বিভিন্ন ব্যাটালিয়ন ও রেঞ্জে পোশাক ও সরঞ্জাম বরাদ্দ প্রদান',
    'Active', 'DDGAdmin', '2025-11-10',
    N'৪৪.০৩.০০০০.০১৮.১৩.০০১.২৪-১৫০',
    'Allocation of uniforms and equipment to various battalions and ranges',
    N'বিভিন্ন ব্যাটালিয়ন ও রেঞ্জে পোশাক ও সরঞ্জাম বরাদ্দ প্রদান প্রসঙ্গে',
    'Reference to the above subject, allocation of uniforms and equipment is being made to various battalions and ranges based on their requirements and staff strength.',
    N'উপরোক্ত বিষয়ের প্রেক্ষিতে বিভিন্ন ব্যাটালিয়ন ও রেঞ্জের চাহিদা এবং কর্মরত জনবলের আলোকে ক্রোড়পত্র-"ক" অনুযায়ী পোশাক ও সরঞ্জাম বরাদ্দ প্রদান করা হলো।

উল্লেখ্য যে, বরাদ্দকৃত উপকরণাদি সংরক্ষণ রেজিস্টারে লিপিবদ্ধ করে যথাযথভাবে সংরক্ষণ এবং সঠিক ব্যবহার নিশ্চিত করতে হবে। বরাদ্দপত্রের মেয়াদ উত্তীর্ণ হওয়ার পূর্বে উপকরণাদি সংগ্রহ করতে হবে।

অনুগ্রহপূর্বক প্রাপ্তি স্বীকারপত্র প্রেরণ করবেন।',
    N'মোঃ আব্দুল হামিদ',
    'Deputy Director General (Admin)',
    N'উপ-মহাপরিচালক (প্রশাসন)',
    '02-9551484', 'ddg.admin@ansarvdp.gov.bd',
    @Now, @CurrentUser, 1
);

DECLARE @AllotmentLetter1Id INT = SCOPE_IDENTITY();
PRINT 'Created - ID: ' + CAST(@AllotmentLetter1Id AS VARCHAR(10))

-- Insert Recipients
INSERT INTO AllotmentLetterRecipients (
    AllotmentLetterId, RecipientType, RecipientName, RecipientNameBn,
    StaffStrength, SerialNo, Remarks, CreatedAt, CreatedBy, IsActive
)
VALUES
    (@AllotmentLetter1Id, 'Battalion', '1st Battalion, Dhaka', N'১ম ব্যাটালিয়ন, ঢাকা', N'১৫০ জন', 1, N'প্রথম কিস্তি', @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 'Battalion', '2nd Battalion, Chittagong', N'২য় ব্যাটালিয়ন, চট্টগ্রাম', N'১২০ জন', 2, '', @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 'Range', 'Dhaka Range', N'ঢাকা রেঞ্জ', N'৮০ জন', 3, '', @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 'Battalion', '5th Battalion, Sylhet', N'৫ম ব্যাটালিয়ন, সিলেট', N'১০০ জন', 4, '', @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 'Range', 'Chittagong Range', N'চট্টগ্রাম রেঞ্জ', N'৭৫ জন', 5, '', @Now, @CurrentUser, 1);

-- Get recipient IDs
DECLARE @Recipient1_1 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 1);
DECLARE @Recipient1_2 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 2);
DECLARE @Recipient1_3 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 3);
DECLARE @Recipient1_4 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 4);
DECLARE @Recipient1_5 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 5);

-- Insert Items for Recipients (uses first 4 items from database)
IF @ItemCount >= 3
BEGIN
    INSERT INTO AllotmentLetterRecipientItems (
        AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity,
        Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive
    )
    VALUES
        -- Recipient 1: 1st Battalion, Dhaka
        (@Recipient1_1, @Item1, 150, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 1), @Now, @CurrentUser, 1),
        (@Recipient1_1, @Item2, 150, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 2), @Now, @CurrentUser, 1),
        (@Recipient1_1, @Item3, 150, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 3), @Now, @CurrentUser, 1),

        -- Recipient 2: 2nd Battalion, Chittagong
        (@Recipient1_2, @Item1, 120, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 1), @Now, @CurrentUser, 1),
        (@Recipient1_2, @Item2, 120, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 2), @Now, @CurrentUser, 1),
        (@Recipient1_2, @Item3, 120, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 3), @Now, @CurrentUser, 1),

        -- Recipient 3: Dhaka Range
        (@Recipient1_3, @Item1, 80, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 1), @Now, @CurrentUser, 1),
        (@Recipient1_3, @Item2, 80, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 2), @Now, @CurrentUser, 1),

        -- Recipient 4: 5th Battalion, Sylhet
        (@Recipient1_4, @Item1, 100, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 1), @Now, @CurrentUser, 1),
        (@Recipient1_4, @Item2, 100, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 2), @Now, @CurrentUser, 1),
        (@Recipient1_4, @Item3, 100, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 3), @Now, @CurrentUser, 1);

    -- Add 4th item if available
    IF @Item4 IS NOT NULL
        INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
        VALUES (@Recipient1_4, @Item4, 50, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 4), @Now, @CurrentUser, 1);

    -- Recipient 5: Chittagong Range
    INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
    VALUES
        (@Recipient1_5, @Item1, 75, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 1), @Now, @CurrentUser, 1),
        (@Recipient1_5, @Item2, 75, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 2), @Now, @CurrentUser, 1);
END

-- Insert Distribution List
INSERT INTO AllotmentLetterDistributions (
    AllotmentLetterId, SerialNo, RecipientTitle, RecipientTitleBn,
    Address, AddressBn, Purpose, PurposeBn, DisplayOrder,
    CreatedAt, CreatedBy, IsActive
)
VALUES
    (@AllotmentLetter1Id, 1, 'Director General, Ansar & VDP', N'মহাপরিচালক, আনসার ও ভিডিপি',
        'Headquarters, Dhaka', N'সদর দপ্তর, ঢাকা', 'For kind information', N'সদয় অবগতির জন্য', 1, @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 2, 'Deputy Director General (Store)', N'উপ-মহাপরিচালক (স্টোর)',
        'Headquarters, Dhaka', N'সদর দপ্তর, ঢাকা', 'For necessary action', N'প্রয়োজনীয় ব্যবস্থা গ্রহণের জন্য', 2, @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 3, 'Director (Finance)', N'পরিচালক (অর্থ)',
        'Headquarters, Dhaka', N'সদর দপ্তর, ঢাকা', 'For information', N'অবগতির জন্য', 3, @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 4, 'Office Copy', N'অফিস কপি', '', '', 'For record', N'নথিভুক্তির জন্য', 4, @Now, @CurrentUser, 1);

PRINT 'Letter 1 completed with ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' distribution entries'

-- =========================================================================
-- ALLOTMENT LETTER 2 (Optional - only if we have enough items)
-- =========================================================================

IF @ItemCount >= 6
BEGIN
    PRINT ''
    PRINT '=============================================='
    PRINT 'Creating Allotment Letter 2...'
    PRINT '=============================================='

    INSERT INTO AllotmentLetters (
        AllotmentNo, AllotmentDate, ValidFrom, ValidUntil,
        IssuedTo, IssuedToType, FromStoreId, Purpose, Status,
        ApprovedBy, ApprovedDate, ReferenceNo,
        Subject, SubjectBn, BodyText, BodyTextBn,
        SignatoryName, SignatoryDesignation, SignatoryDesignationBn,
        SignatoryPhone, SignatoryEmail,
        CreatedAt, CreatedBy, IsActive
    )
    VALUES (
        'AL-202511-002', '2025-11-10', '2025-11-10', '2026-02-10',
        'Multiple Recipients', 'Multiple', @CentralStoreId,
        N'অফিস সরঞ্জাম ও স্টেশনারি বরাদ্দ প্রদান',
        'Active', 'DDGAdmin', '2025-11-10',
        N'৪৪.০৩.০০০০.০১৮.১৩.০০২.২৪-১৫১',
        'Allocation of office equipment and stationery',
        N'বিভিন্ন জেলা কার্যালয়ে অফিস সরঞ্জাম ও স্টেশনারি বরাদ্দ প্রসঙ্গে',
        'Allocation of office equipment.',
        N'উপরোক্ত বিষয়ের প্রেক্ষিতে বিভিন্ন জেলা কার্যালয়ের চাহিদার আলোকে ক্রোড়পত্র-"ক" অনুযায়ী অফিস সরঞ্জাম ও স্টেশনারি বরাদ্দ প্রদান করা হলো।

বরাদ্দকৃত উপকরণাদি সংরক্ষণ রেজিস্টারে লিপিবদ্ধ করে যথাযথভাবে সংরক্ষণ এবং সঠিক ব্যবহার নিশ্চিত করতে হবে।

প্রাপ্তি স্বীকারপত্র প্রেরণের জন্য অনুরোধ করা হলো।',
        N'মোঃ রফিকুল ইসলাম',
        'Deputy Director (Store)',
        N'উপ-পরিচালক (স্টোর)',
        '02-9564098', 'dd.store@ansarvdp.gov.bd',
        @Now, @CurrentUser, 1
    );

    DECLARE @AllotmentLetter2Id INT = SCOPE_IDENTITY();
    PRINT 'Created - ID: ' + CAST(@AllotmentLetter2Id AS VARCHAR(10))

    -- Insert Recipients
    INSERT INTO AllotmentLetterRecipients (
        AllotmentLetterId, RecipientType, RecipientName, RecipientNameBn,
        StaffStrength, SerialNo, Remarks, CreatedAt, CreatedBy, IsActive
    )
    VALUES
        (@AllotmentLetter2Id, 'Zila', 'Dhaka Zila Office', N'ঢাকা জেলা কার্যালয়', N'৫০ জন', 1, '', @Now, @CurrentUser, 1),
        (@AllotmentLetter2Id, 'Zila', 'Chittagong Zila Office', N'চট্টগ্রাম জেলা কার্যালয়', N'৪৫ জন', 2, '', @Now, @CurrentUser, 1),
        (@AllotmentLetter2Id, 'Zila', 'Sylhet Zila Office', N'সিলেট জেলা কার্যালয়', N'৩৫ জন', 3, '', @Now, @CurrentUser, 1);

    -- Get recipient IDs
    DECLARE @Recipient2_1 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter2Id AND SerialNo = 1);
    DECLARE @Recipient2_2 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter2Id AND SerialNo = 2);
    DECLARE @Recipient2_3 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter2Id AND SerialNo = 3);

    -- Insert Items (uses items 5-8)
    INSERT INTO AllotmentLetterRecipientItems (
        AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity,
        Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive
    )
    VALUES
        -- Dhaka
        (@Recipient2_1, @Item5, 20, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 5), @Now, @CurrentUser, 1),
        (@Recipient2_1, @Item6, 20, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 6), @Now, @CurrentUser, 1),

        -- Chittagong
        (@Recipient2_2, @Item5, 15, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 5), @Now, @CurrentUser, 1),
        (@Recipient2_2, @Item6, 15, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 6), @Now, @CurrentUser, 1),

        -- Sylhet
        (@Recipient2_3, @Item5, 10, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 5), @Now, @CurrentUser, 1),
        (@Recipient2_3, @Item6, 10, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 6), @Now, @CurrentUser, 1);

    -- Add items 7 and 8 if available
    IF @Item7 IS NOT NULL
    BEGIN
        INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
        VALUES
            (@Recipient2_1, @Item7, 100, 0, 'Rim', N'রিম', (SELECT ItemName FROM @ItemIds WHERE RowNum = 7), @Now, @CurrentUser, 1),
            (@Recipient2_2, @Item7, 80, 0, 'Rim', N'রিম', (SELECT ItemName FROM @ItemIds WHERE RowNum = 7), @Now, @CurrentUser, 1),
            (@Recipient2_3, @Item7, 60, 0, 'Rim', N'রিম', (SELECT ItemName FROM @ItemIds WHERE RowNum = 7), @Now, @CurrentUser, 1);
    END

    IF @Item8 IS NOT NULL
    BEGIN
        INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
        VALUES
            (@Recipient2_1, @Item8, 50, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 8), @Now, @CurrentUser, 1),
            (@Recipient2_2, @Item8, 40, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 8), @Now, @CurrentUser, 1),
            (@Recipient2_3, @Item8, 30, 0, 'Pcs', N'টি', (SELECT ItemName FROM @ItemIds WHERE RowNum = 8), @Now, @CurrentUser, 1);
    END

    -- Distribution List
    INSERT INTO AllotmentLetterDistributions (
        AllotmentLetterId, SerialNo, RecipientTitle, RecipientTitleBn,
        Address, AddressBn, Purpose, PurposeBn, DisplayOrder,
        CreatedAt, CreatedBy, IsActive
    )
    VALUES
        (@AllotmentLetter2Id, 1, 'Director General, Ansar & VDP', N'মহাপরিচালক, আনসার ও ভিডিপি',
            'Headquarters, Dhaka', N'সদর দপ্তর, ঢাকা', 'For kind information', N'সদয় অবগতির জন্য', 1, @Now, @CurrentUser, 1),
        (@AllotmentLetter2Id, 2, 'Deputy Director General (Admin)', N'উপ-মহাপরিচালক (প্রশাসন)',
            'Headquarters, Dhaka', N'সদর দপ্তর, ঢাকা', 'For information', N'অবগতির জন্য', 2, @Now, @CurrentUser, 1),
        (@AllotmentLetter2Id, 3, 'Office Copy', N'অফিস কপি', '', '', 'For record', N'নথিভুক্তির জন্য', 3, @Now, @CurrentUser, 1);

    PRINT 'Letter 2 completed!'
END
ELSE
BEGIN
    PRINT ''
    PRINT 'Skipping Letter 2 - not enough items (need 6+, found ' + CAST(@ItemCount AS VARCHAR(10)) + ')'
END

-- Summary
PRINT ''
PRINT '=============================================='
PRINT 'SUMMARY'
PRINT '=============================================='

SELECT 'Letters Created' AS Info, COUNT(*) AS Total
FROM AllotmentLetters
WHERE AllotmentNo IN ('AL-202511-001', 'AL-202511-002');

SELECT 'Recipients' AS Info, COUNT(*) AS Total
FROM AllotmentLetterRecipients ALR
JOIN AllotmentLetters AL ON AL.Id = ALR.AllotmentLetterId
WHERE AL.AllotmentNo IN ('AL-202511-001', 'AL-202511-002');

SELECT 'Items Allocated' AS Info, COUNT(*) AS Total
FROM AllotmentLetterRecipientItems ALRI
JOIN AllotmentLetterRecipients ALR ON ALR.Id = ALRI.AllotmentLetterRecipientId
JOIN AllotmentLetters AL ON AL.Id = ALR.AllotmentLetterId
WHERE AL.AllotmentNo IN ('AL-202511-001', 'AL-202511-002');

PRINT ''
PRINT '=============================================='
PRINT 'SUCCESS!'
PRINT '=============================================='
PRINT 'View at: https://localhost:7029/AllotmentLetter'
PRINT '=============================================='

SET NOCOUNT OFF;
