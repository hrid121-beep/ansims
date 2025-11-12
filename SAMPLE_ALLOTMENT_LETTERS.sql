-- =========================================================================
-- SAMPLE ALLOTMENT LETTERS FOR PDF TESTING
-- =========================================================================
-- This script creates 2 sample allotment letters with complete data
-- matching the official government format
-- =========================================================================

-- First, check if we have the necessary data (stores, items, etc.)
-- You may need to adjust IDs based on your existing data

DECLARE @CentralStoreId INT = 1; -- Adjust based on your Stores table
DECLARE @CurrentUser NVARCHAR(100) = 'admin'; -- Current logged-in user
DECLARE @Now DATETIME = GETDATE();

-- ===========================================
-- ALLOTMENT LETTER 1: Multiple Recipients
-- ===========================================

-- Insert Main Allotment Letter
INSERT INTO AllotmentLetters (
    AllotmentNo,
    AllotmentDate,
    ValidFrom,
    ValidUntil,
    IssuedTo,
    IssuedToType,
    FromStoreId,
    Purpose,
    Status,
    ApprovedBy,
    ApprovedDate,
    ReferenceNo,
    Subject,
    SubjectBn,
    BodyText,
    BodyTextBn,
    SignatoryName,
    SignatoryDesignation,
    SignatoryDesignationBn,
    SignatoryPhone,
    SignatoryEmail,
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES (
    'AL-202511-001',
    '2025-11-10',
    '2025-11-10',
    '2026-05-10', -- 6 months validity
    'Multiple Recipients',
    'Multiple',
    @CentralStoreId,
    'বিভিন্ন ব্যাটালিয়ন ও রেঞ্জে পোশাক ও সরঞ্জাম বরাদ্দ প্রদান',
    'Active',
    'DDGAdmin',
    '2025-11-10',
    '৪৪.০৩.০০০০.০১৮.১৩.০০১.২৪-১৫০',
    'Allocation of uniforms and equipment to various battalions and ranges',
    'বিভিন্ন ব্যাটালিয়ন ও রেঞ্জে পোশাক ও সরঞ্জাম বরাদ্দ প্রদান প্রসঙ্গে',
    'Reference to the above subject, allocation of uniforms and equipment is being made to various battalions and ranges based on their requirements and staff strength.',
    'উপরোক্ত বিষয়ের প্রেক্ষিতে বিভিন্ন ব্যাটালিয়ন ও রেঞ্জের চাহিদা এবং কর্মরত জনবলের আলোকে ক্রোড়পত্র-"ক" অনুযায়ী পোশাক ও সরঞ্জাম বরাদ্দ প্রদান করা হলো।

উল্লেখ্য যে, বরাদ্দকৃত উপকরণাদি সংরক্ষণ রেজিস্টারে লিপিবদ্ধ করে যথাযথভাবে সংরক্ষণ এবং সঠিক ব্যবহার নিশ্চিত করতে হবে। বরাদ্দপত্রের মেয়াদ উত্তীর্ণ হওয়ার পূর্বে উপকরণাদি সংগ্রহ করতে হবে।

অনুগ্রহপূর্বক প্রাপ্তি স্বীকারপত্র প্রেরণ করবেন।',
    'মোঃ আব্দুল হামিদ',
    'Deputy Director General (Admin)',
    'উপ-মহাপরিচালক (প্রশাসন)',
    '০২-৯৫৫১৪৮৪',
    'ddg.admin@ansarvdp.gov.bd',
    @Now,
    @CurrentUser,
    1
);

DECLARE @AllotmentLetter1Id INT = SCOPE_IDENTITY();

-- Insert Recipients for Allotment Letter 1
INSERT INTO AllotmentLetterRecipients (
    AllotmentLetterId,
    RecipientType,
    RecipientName,
    RecipientNameBn,
    StaffStrength,
    SerialNo,
    Remarks,
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES
    (@AllotmentLetter1Id, 'Battalion', '1st Battalion, Dhaka', '১ম ব্যাটালিয়ন, ঢাকা', '১৫০ জন', 1, 'প্রথম কিস্তি', @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 'Battalion', '2nd Battalion, Chittagong', '২য় ব্যাটালিয়ন, চট্টগ্রাম', '১২০ জন', 2, '', @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 'Range', 'Dhaka Range', 'ঢাকা রেঞ্জ', '৮০ জন', 3, '', @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 'Battalion', '5th Battalion, Sylhet', '৫ম ব্যাটালিয়ন, সিলেট', '১০০ জন', 4, '', @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 'Range', 'Chittagong Range', 'চট্টগ্রাম রেঞ্জ', '৭৫ জন', 5, '', @Now, @CurrentUser, 1);

-- Get recipient IDs for items
DECLARE @Recipient1_1 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 1);
DECLARE @Recipient1_2 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 2);
DECLARE @Recipient1_3 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 3);
DECLARE @Recipient1_4 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 4);
DECLARE @Recipient1_5 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter1Id AND SerialNo = 5);

-- Insert Items for Recipients (you'll need to adjust ItemId based on your Items table)
-- For demonstration, using placeholder ItemIds - you should use real ItemIds from your Items table

INSERT INTO AllotmentLetterRecipientItems (
    AllotmentLetterRecipientId,
    ItemId,
    AllottedQuantity,
    IssuedQuantity,
    Unit,
    UnitBn,
    ItemNameBn,
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES
    -- Recipient 1 items
    (@Recipient1_1, 1, 150, 0, 'Pcs', 'টি', 'ইউনিফর্ম (শার্ট)', @Now, @CurrentUser, 1),
    (@Recipient1_1, 2, 150, 0, 'Pcs', 'টি', 'ইউনিফর্ম (প্যান্ট)', @Now, @CurrentUser, 1),
    (@Recipient1_1, 3, 150, 0, 'Pcs', 'টি', 'বুট', @Now, @CurrentUser, 1),

    -- Recipient 2 items
    (@Recipient1_2, 1, 120, 0, 'Pcs', 'টি', 'ইউনিফর্ম (শার্ট)', @Now, @CurrentUser, 1),
    (@Recipient1_2, 2, 120, 0, 'Pcs', 'টি', 'ইউনিফর্ম (প্যান্ট)', @Now, @CurrentUser, 1),
    (@Recipient1_2, 3, 120, 0, 'Pcs', 'টি', 'বুট', @Now, @CurrentUser, 1),

    -- Recipient 3 items
    (@Recipient1_3, 1, 80, 0, 'Pcs', 'টি', 'ইউনিফর্ম (শার্ট)', @Now, @CurrentUser, 1),
    (@Recipient1_3, 2, 80, 0, 'Pcs', 'টি', 'ইউনিফর্ম (প্যান্ট)', @Now, @CurrentUser, 1),

    -- Recipient 4 items
    (@Recipient1_4, 1, 100, 0, 'Pcs', 'টি', 'ইউনিফর্ম (শার্ট)', @Now, @CurrentUser, 1),
    (@Recipient1_4, 2, 100, 0, 'Pcs', 'টি', 'ইউনিফর্ম (প্যান্ট)', @Now, @CurrentUser, 1),
    (@Recipient1_4, 3, 100, 0, 'Pcs', 'টি', 'বুট', @Now, @CurrentUser, 1),
    (@Recipient1_4, 4, 50, 0, 'Pcs', 'টি', 'টুপি', @Now, @CurrentUser, 1),

    -- Recipient 5 items
    (@Recipient1_5, 1, 75, 0, 'Pcs', 'টি', 'ইউনিফর্ম (শার্ট)', @Now, @CurrentUser, 1),
    (@Recipient1_5, 2, 75, 0, 'Pcs', 'টি', 'ইউনিফর্ম (প্যান্ট)', @Now, @CurrentUser, 1);

-- Insert Distribution List for Allotment Letter 1
INSERT INTO AllotmentLetterDistributions (
    AllotmentLetterId,
    SerialNo,
    RecipientTitle,
    RecipientTitleBn,
    Address,
    AddressBn,
    Purpose,
    PurposeBn,
    DisplayOrder,
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES
    (@AllotmentLetter1Id, 1, 'Director General, Ansar & VDP', 'মহাপরিচালক, আনসার ও ভিডিপি', 'Headquarters, Dhaka', 'সদর দপ্তর, ঢাকা', 'For kind information', 'সদয় অবগতির জন্য', 1, @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 2, 'Deputy Director General (Store)', 'উপ-মহাপরিচালক (স্টোর)', 'Headquarters, Dhaka', 'সদর দপ্তর, ঢাকা', 'For necessary action', 'প্রয়োজনীয় ব্যবস্থা গ্রহণের জন্য', 2, @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 3, 'Director (Finance)', 'পরিচালক (অর্থ)', 'Headquarters, Dhaka', 'সদর দপ্তর, ঢাকা', 'For information', 'অবগতির জন্য', 3, @Now, @CurrentUser, 1),
    (@AllotmentLetter1Id, 4, 'Office Copy', 'অফিস কপি', '', '', 'For record', 'নথিভুক্তির জন্য', 4, @Now, @CurrentUser, 1);

-- ===========================================
-- ALLOTMENT LETTER 2: Equipment & Stationery
-- ===========================================

INSERT INTO AllotmentLetters (
    AllotmentNo,
    AllotmentDate,
    ValidFrom,
    ValidUntil,
    IssuedTo,
    IssuedToType,
    FromStoreId,
    Purpose,
    Status,
    ApprovedBy,
    ApprovedDate,
    ReferenceNo,
    Subject,
    SubjectBn,
    BodyText,
    BodyTextBn,
    SignatoryName,
    SignatoryDesignation,
    SignatoryDesignationBn,
    SignatoryPhone,
    SignatoryEmail,
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES (
    'AL-202511-002',
    '2025-11-10',
    '2025-11-10',
    '2026-02-10', -- 3 months validity
    'Multiple Recipients',
    'Multiple',
    @CentralStoreId,
    'অফিস সরঞ্জাম ও স্টেশনারি বরাদ্দ প্রদান',
    'Active',
    'DDGAdmin',
    '2025-11-10',
    '৪৪.০৩.০০০০.০১৮.১৩.০০২.২৪-১৫১',
    'Allocation of office equipment and stationery',
    'বিভিন্ন জেলা কার্যালয়ে অফিস সরঞ্জাম ও স্টেশনারি বরাদ্দ প্রসঙ্গে',
    'Reference to the above subject, allocation of office equipment and stationery is being made to various district offices.',
    'উপরোক্ত বিষয়ের প্রেক্ষিতে বিভিন্ন জেলা কার্যালয়ের চাহিদার আলোকে ক্রোড়পত্র-"ক" অনুযায়ী অফিস সরঞ্জাম ও স্টেশনারি বরাদ্দ প্রদান করা হলো।

বরাদ্দকৃত উপকরণাদি সংরক্ষণ রেজিস্টারে লিপিবদ্ধ করে যথাযথভাবে সংরক্ষণ এবং সঠিক ব্যবহার নিশ্চিত করতে হবে।

প্রাপ্তি স্বীকারপত্র প্রেরণের জন্য অনুরোধ করা হলো।',
    'মোঃ রফিকুল ইসলাম',
    'Deputy Director (Store)',
    'উপ-পরিচালক (স্টোর)',
    '০২-৯৫৬৪০৯৮',
    'dd.store@ansarvdp.gov.bd',
    @Now,
    @CurrentUser,
    1
);

DECLARE @AllotmentLetter2Id INT = SCOPE_IDENTITY();

-- Insert Recipients for Allotment Letter 2
INSERT INTO AllotmentLetterRecipients (
    AllotmentLetterId,
    RecipientType,
    RecipientName,
    RecipientNameBn,
    StaffStrength,
    SerialNo,
    Remarks,
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES
    (@AllotmentLetter2Id, 'Zila', 'Dhaka Zila Office', 'ঢাকা জেলা কার্যালয়', '৫০ জন', 1, '', @Now, @CurrentUser, 1),
    (@AllotmentLetter2Id, 'Zila', 'Chittagong Zila Office', 'চট্টগ্রাম জেলা কার্যালয়', '৪৫ জন', 2, '', @Now, @CurrentUser, 1),
    (@AllotmentLetter2Id, 'Zila', 'Sylhet Zila Office', 'সিলেট জেলা কার্যালয়', '৩৫ জন', 3, '', @Now, @CurrentUser, 1);

-- Get recipient IDs for items
DECLARE @Recipient2_1 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter2Id AND SerialNo = 1);
DECLARE @Recipient2_2 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter2Id AND SerialNo = 2);
DECLARE @Recipient2_3 INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentLetter2Id AND SerialNo = 3);

-- Insert Items for Recipients of Allotment Letter 2
INSERT INTO AllotmentLetterRecipientItems (
    AllotmentLetterRecipientId,
    ItemId,
    AllottedQuantity,
    IssuedQuantity,
    Unit,
    UnitBn,
    ItemNameBn,
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES
    -- Recipient 1 items (Dhaka)
    (@Recipient2_1, 5, 20, 0, 'Pcs', 'টি', 'কম্পিউটার টেবিল', @Now, @CurrentUser, 1),
    (@Recipient2_1, 6, 20, 0, 'Pcs', 'টি', 'চেয়ার', @Now, @CurrentUser, 1),
    (@Recipient2_1, 7, 100, 0, 'Rim', 'রিম', 'এ৪ পেপার', @Now, @CurrentUser, 1),
    (@Recipient2_1, 8, 50, 0, 'Pcs', 'টি', 'ফাইল কভার', @Now, @CurrentUser, 1),

    -- Recipient 2 items (Chittagong)
    (@Recipient2_2, 5, 15, 0, 'Pcs', 'টি', 'কম্পিউটার টেবিল', @Now, @CurrentUser, 1),
    (@Recipient2_2, 6, 15, 0, 'Pcs', 'টি', 'চেয়ার', @Now, @CurrentUser, 1),
    (@Recipient2_2, 7, 80, 0, 'Rim', 'রিম', 'এ৪ পেপার', @Now, @CurrentUser, 1),
    (@Recipient2_2, 8, 40, 0, 'Pcs', 'টি', 'ফাইল কভার', @Now, @CurrentUser, 1),

    -- Recipient 3 items (Sylhet)
    (@Recipient2_3, 5, 10, 0, 'Pcs', 'টি', 'কম্পিউটার টেবিল', @Now, @CurrentUser, 1),
    (@Recipient2_3, 6, 10, 0, 'Pcs', 'টি', 'চেয়ার', @Now, @CurrentUser, 1),
    (@Recipient2_3, 7, 60, 0, 'Rim', 'রিম', 'এ৪ পেপার', @Now, @CurrentUser, 1),
    (@Recipient2_3, 8, 30, 0, 'Pcs', 'টি', 'ফাইল কভার', @Now, @CurrentUser, 1);

-- Insert Distribution List for Allotment Letter 2
INSERT INTO AllotmentLetterDistributions (
    AllotmentLetterId,
    SerialNo,
    RecipientTitle,
    RecipientTitleBn,
    Address,
    AddressBn,
    Purpose,
    PurposeBn,
    DisplayOrder,
    CreatedAt,
    CreatedBy,
    IsActive
)
VALUES
    (@AllotmentLetter2Id, 1, 'Director General, Ansar & VDP', 'মহাপরিচালক, আনসার ও ভিডিপি', 'Headquarters, Dhaka', 'সদর দপ্তর, ঢাকা', 'For kind information', 'সদয় অবগতির জন্য', 1, @Now, @CurrentUser, 1),
    (@AllotmentLetter2Id, 2, 'Deputy Director General (Admin)', 'উপ-মহাপরিচালক (প্রশাসন)', 'Headquarters, Dhaka', 'সদর দপ্তর, ঢাকা', 'For information', 'অবগতির জন্য', 2, @Now, @CurrentUser, 1),
    (@AllotmentLetter2Id, 3, 'Office Copy', 'অফিস কপি', '', '', 'For record', 'নথিভুক্তির জন্য', 3, @Now, @CurrentUser, 1);

-- =========================================================================
-- SUMMARY
-- =========================================================================

SELECT
    'Allotment Letters Created' AS Info,
    COUNT(*) AS Total
FROM AllotmentLetters
WHERE AllotmentNo IN ('AL-202511-001', 'AL-202511-002');

SELECT
    'Recipients Created' AS Info,
    COUNT(*) AS Total
FROM AllotmentLetterRecipients
WHERE AllotmentLetterId IN (@AllotmentLetter1Id, @AllotmentLetter2Id);

SELECT
    'Items Allocated' AS Info,
    COUNT(*) AS Total
FROM AllotmentLetterRecipientItems
WHERE AllotmentLetterRecipientId IN (
    SELECT Id FROM AllotmentLetterRecipients
    WHERE AllotmentLetterId IN (@AllotmentLetter1Id, @AllotmentLetter2Id)
);

SELECT
    'Distribution List Entries' AS Info,
    COUNT(*) AS Total
FROM AllotmentLetterDistributions
WHERE AllotmentLetterId IN (@AllotmentLetter1Id, @AllotmentLetter2Id);

PRINT '=============================================='
PRINT 'Sample Allotment Letters Created Successfully!'
PRINT '=============================================='
PRINT 'Letter 1: AL-202511-001 (Uniforms & Equipment - 5 Recipients)'
PRINT 'Letter 2: AL-202511-002 (Office Equipment - 3 Recipients)'
PRINT ''
PRINT 'IMPORTANT NOTES:'
PRINT '1. You may need to adjust @CentralStoreId based on your Stores table'
PRINT '2. ItemId values (1-8) are placeholders - update them with real ItemIds from your Items table'
PRINT '3. You can view these allotments at: https://localhost:7029/AllotmentLetter'
PRINT '4. PDF will be generated with full government format including:'
PRINT '   - Bengali header and letterhead'
PRINT '   - Recipient table (ক্রোড়পত্র)'
PRINT '   - Distribution list (অনুলিপি)'
PRINT '   - Signature section'
PRINT '=============================================='
