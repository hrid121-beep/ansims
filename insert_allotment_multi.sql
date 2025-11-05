-- Insert AllotmentLetter with MULTIPLE RECIPIENTS and MULTIPLE ITEMS
-- Database: ansvdp_ims

USE ansvdp_ims;
GO

-- Step 1: Insert AllotmentLetter (Main Record)
DECLARE @AllotmentLetterId INT;
DECLARE @CurrentDate DATETIME = GETDATE();
DECLARE @ValidUntil DATETIME = DATEADD(MONTH, 6, @CurrentDate);

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
) VALUES (
    'ALLOT-2025-002',
    @CurrentDate,
    @CurrentDate,
    @ValidUntil,
    'Multiple Battalions',
    'Multiple',
    1, -- Dhaka Central Store
    'Quarterly uniform and equipment allotment for all battalions',
    'Approved',
    '৪৪.০৩.০০০০.০১৮.১৩.০০২.২৫',
    'Quarterly Allotment of Uniforms and Equipment',
    'ত্রৈমাসিক ইউনিফর্ম ও সরঞ্জাম বরাদ্দ প্রদান প্রসঙ্গে',
    '1. All battalions to collect items within 30 days
2. Items must be distributed to personnel immediately
3. Proper documentation required
4. Return unused items within 7 days',
    '১. সকল ব্যাটালিয়ন ৩০ দিনের মধ্যে পণ্য সংগ্রহ করবেন
২. অবিলম্বে কর্মীদের মধ্যে বিতরণ করতে হবে
৩. যথাযথ ডকুমেন্টেশন প্রয়োজন
৪. অব্যবহৃত পণ্য ৭ দিনের মধ্যে ফেরত দিন',
    'মোঃ আবদুল করিম',
    'Deputy Director General (Admin)',
    'উপ-মহাপরিচালক (প্রশাসন)',
    '+880-2-9558090',
    'ddg.admin@ansar-vdp.gov.bd',
    @CurrentDate,
    'admin',
    1
);

SET @AllotmentLetterId = SCOPE_IDENTITY();
PRINT 'AllotmentLetter created with ID: ' + CAST(@AllotmentLetterId AS VARCHAR(10));

-- ========================================
-- RECIPIENT 1: 1st Battalion Dhaka
-- ========================================
DECLARE @Recipient1Id INT;

INSERT INTO AllotmentLetterRecipients (
    AllotmentLetterId,
    RecipientType,
    BattalionId,
    RecipientName,
    RecipientNameBn,
    StaffStrength,
    SerialNo,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @AllotmentLetterId,
    'Battalion',
    1, -- 1st Battalion Dhaka
    '1st Battalion Dhaka',
    'ঢাকা আনসার ব্যাটালিয়ন (১ম)',
    '৫০০',
    1,
    @CurrentDate,
    'admin',
    1
);

SET @Recipient1Id = SCOPE_IDENTITY();
PRINT 'Recipient 1 created: 1st Battalion Dhaka (ID: ' + CAST(@Recipient1Id AS VARCHAR(10)) + ')';

-- Items for Recipient 1
INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, ItemNameBn, Remarks, CreatedAt, CreatedBy, IsActive)
VALUES
(@Recipient1Id, 1, 150, 0, 'PCS', 'টি', 'আনসার ইউনিফর্ম শার্ট (খাকি)', 'For active personnel', @CurrentDate, 'admin', 1),
(@Recipient1Id, 2, 150, 0, 'PCS', 'টি', 'আনসার ইউনিফর্ম প্যান্ট (খাকি)', 'For active personnel', @CurrentDate, 'admin', 1),
(@Recipient1Id, 3, 75, 0, 'PAIR', 'জোড়া', 'সামরিক বুট (কালো)', 'Standard issue', @CurrentDate, 'admin', 1),
(@Recipient1Id, 4, 50, 0, 'KIT', 'সেট', 'প্রাথমিক চিকিৎসা বাক্স', 'For medical use', @CurrentDate, 'admin', 1);

-- ========================================
-- RECIPIENT 2: 2nd Battalion Dhaka
-- ========================================
DECLARE @Recipient2Id INT;

INSERT INTO AllotmentLetterRecipients (
    AllotmentLetterId,
    RecipientType,
    BattalionId,
    RecipientName,
    RecipientNameBn,
    StaffStrength,
    SerialNo,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @AllotmentLetterId,
    'Battalion',
    2, -- 2nd Battalion Dhaka
    '2nd Battalion Dhaka',
    'ঢাকা আনসার ব্যাটালিয়ন (২য়)',
    '৪৫০',
    2,
    @CurrentDate,
    'admin',
    1
);

SET @Recipient2Id = SCOPE_IDENTITY();
PRINT 'Recipient 2 created: 2nd Battalion Dhaka (ID: ' + CAST(@Recipient2Id AS VARCHAR(10)) + ')';

-- Items for Recipient 2
INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, ItemNameBn, Remarks, CreatedAt, CreatedBy, IsActive)
VALUES
(@Recipient2Id, 1, 120, 0, 'PCS', 'টি', 'আনসার ইউনিফর্ম শার্ট (খাকি)', 'For new recruits', @CurrentDate, 'admin', 1),
(@Recipient2Id, 2, 120, 0, 'PCS', 'টি', 'আনসার ইউনিফর্ম প্যান্ট (খাকি)', 'For new recruits', @CurrentDate, 'admin', 1),
(@Recipient2Id, 3, 60, 0, 'PAIR', 'জোড়া', 'সামরিক বুট (কালো)', 'Standard issue', @CurrentDate, 'admin', 1),
(@Recipient2Id, 4, 30, 0, 'KIT', 'সেট', 'প্রাথমিক চিকিৎসা বাক্স', 'For medical use', @CurrentDate, 'admin', 1),
(@Recipient2Id, 5, 200, 0, 'TABLET', 'ট্যাবলেট', 'প্যারাসিটামল ৫০০ মিগ্রা', 'For medical stock', @CurrentDate, 'admin', 1);

-- ========================================
-- RECIPIENT 3: 3rd Battalion Chattogram
-- ========================================
DECLARE @Recipient3Id INT;

INSERT INTO AllotmentLetterRecipients (
    AllotmentLetterId,
    RecipientType,
    BattalionId,
    RecipientName,
    RecipientNameBn,
    StaffStrength,
    SerialNo,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @AllotmentLetterId,
    'Battalion',
    3, -- 3rd Battalion Chattogram
    '3rd Battalion Chattogram',
    'চট্টগ্রাম আনসার ব্যাটালিয়ন (৩য়)',
    '৬০০',
    3,
    @CurrentDate,
    'admin',
    1
);

SET @Recipient3Id = SCOPE_IDENTITY();
PRINT 'Recipient 3 created: 3rd Battalion Chattogram (ID: ' + CAST(@Recipient3Id AS VARCHAR(10)) + ')';

-- Items for Recipient 3
INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, ItemNameBn, Remarks, CreatedAt, CreatedBy, IsActive)
VALUES
(@Recipient3Id, 1, 200, 0, 'PCS', 'টি', 'আনসার ইউনিফর্ম শার্ট (খাকি)', 'For expansion', @CurrentDate, 'admin', 1),
(@Recipient3Id, 2, 200, 0, 'PCS', 'টি', 'আনসার ইউনিফর্ম প্যান্ট (খাকি)', 'For expansion', @CurrentDate, 'admin', 1),
(@Recipient3Id, 3, 100, 0, 'PAIR', 'জোড়া', 'সামরিক বুট (কালো)', 'Standard issue', @CurrentDate, 'admin', 1);

-- ========================================
-- RECIPIENT 4: 4th Battalion Rajshahi
-- ========================================
DECLARE @Recipient4Id INT;

INSERT INTO AllotmentLetterRecipients (
    AllotmentLetterId,
    RecipientType,
    BattalionId,
    RecipientName,
    RecipientNameBn,
    StaffStrength,
    SerialNo,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @AllotmentLetterId,
    'Battalion',
    4, -- 4th Battalion Rajshahi
    '4th Battalion Rajshahi',
    'রাজশাহী আনসার ব্যাটালিয়ন (৪র্থ)',
    '৩৫০',
    4,
    @CurrentDate,
    'admin',
    1
);

SET @Recipient4Id = SCOPE_IDENTITY();
PRINT 'Recipient 4 created: 4th Battalion Rajshahi (ID: ' + CAST(@Recipient4Id AS VARCHAR(10)) + ')';

-- Items for Recipient 4
INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, ItemNameBn, Remarks, CreatedAt, CreatedBy, IsActive)
VALUES
(@Recipient4Id, 1, 100, 0, 'PCS', 'টি', 'আনসার ইউনিফর্ম শার্ট (খাকি)', 'Replacement stock', @CurrentDate, 'admin', 1),
(@Recipient4Id, 2, 100, 0, 'PCS', 'টি', 'আনসার ইউনিফর্ম প্যান্ট (খাকি)', 'Replacement stock', @CurrentDate, 'admin', 1),
(@Recipient4Id, 3, 50, 0, 'PAIR', 'জোড়া', 'সামরিক বুট (কালো)', 'Replacement stock', @CurrentDate, 'admin', 1),
(@Recipient4Id, 4, 25, 0, 'KIT', 'সেট', 'প্রাথমিক চিকিৎসা বাক্স', 'For medical use', @CurrentDate, 'admin', 1),
(@Recipient4Id, 5, 150, 0, 'TABLET', 'ট্যাবলেট', 'প্যারাসিটামল ৫০০ মিগ্রা', 'For medical stock', @CurrentDate, 'admin', 1);

PRINT 'All recipients and items created successfully!';

-- ========================================
-- Summary Items for AllotmentLetter
-- (Aggregated totals from all recipients)
-- ========================================

-- Item 1: Uniform Shirt (Total: 150+120+200+100 = 570)
INSERT INTO AllotmentLetterItems (AllotmentLetterId, ItemId, AllottedQuantity, IssuedQuantity, RemainingQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
VALUES (@AllotmentLetterId, 1, 570, 0, 570, 'PCS', 'টি', 'আনসার ইউনিফর্ম শার্ট (খাকি)', @CurrentDate, 'admin', 1);

-- Item 2: Uniform Pant (Total: 150+120+200+100 = 570)
INSERT INTO AllotmentLetterItems (AllotmentLetterId, ItemId, AllottedQuantity, IssuedQuantity, RemainingQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
VALUES (@AllotmentLetterId, 2, 570, 0, 570, 'PCS', 'টি', 'আনসার ইউনিফর্ম প্যান্ট (খাকি)', @CurrentDate, 'admin', 1);

-- Item 3: Military Boot (Total: 75+60+100+50 = 285)
INSERT INTO AllotmentLetterItems (AllotmentLetterId, ItemId, AllottedQuantity, IssuedQuantity, RemainingQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
VALUES (@AllotmentLetterId, 3, 285, 0, 285, 'PAIR', 'জোড়া', 'সামরিক বুট (কালো)', @CurrentDate, 'admin', 1);

-- Item 4: First Aid Kit (Total: 50+30+25 = 105)
INSERT INTO AllotmentLetterItems (AllotmentLetterId, ItemId, AllottedQuantity, IssuedQuantity, RemainingQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
VALUES (@AllotmentLetterId, 4, 105, 0, 105, 'KIT', 'সেট', 'প্রাথমিক চিকিৎসা বাক্স', @CurrentDate, 'admin', 1);

-- Item 5: Paracetamol (Total: 200+150 = 350)
INSERT INTO AllotmentLetterItems (AllotmentLetterId, ItemId, AllottedQuantity, IssuedQuantity, RemainingQuantity, Unit, UnitBn, ItemNameBn, CreatedAt, CreatedBy, IsActive)
VALUES (@AllotmentLetterId, 5, 350, 0, 350, 'TABLET', 'ট্যাবলেট', 'প্যারাসিটামল ৫০০ মিগ্রা', @CurrentDate, 'admin', 1);

PRINT 'Summary items created successfully!';

-- Verification Query
SELECT
    AL.Id AS AllotmentId,
    AL.AllotmentNo,
    AL.Status,
    AL.ReferenceNo,
    AL.Subject,
    COUNT(DISTINCT ALR.Id) AS RecipientCount,
    COUNT(DISTINCT ALRI.Id) AS RecipientItemCount,
    COUNT(DISTINCT ALI.Id) AS SummaryItemCount
FROM AllotmentLetters AL
LEFT JOIN AllotmentLetterRecipients ALR ON AL.Id = ALR.AllotmentLetterId
LEFT JOIN AllotmentLetterRecipientItems ALRI ON ALR.Id = ALRI.AllotmentLetterRecipientId
LEFT JOIN AllotmentLetterItems ALI ON AL.Id = ALI.AllotmentLetterId
WHERE AL.Id = @AllotmentLetterId
GROUP BY AL.Id, AL.AllotmentNo, AL.Status, AL.ReferenceNo, AL.Subject;

-- Detailed Recipient-wise breakdown
SELECT
    ALR.SerialNo,
    ALR.RecipientName,
    ALR.RecipientNameBn,
    ALR.StaffStrength,
    COUNT(ALRI.Id) AS ItemCount
FROM AllotmentLetterRecipients ALR
LEFT JOIN AllotmentLetterRecipientItems ALRI ON ALR.Id = ALRI.AllotmentLetterRecipientId
WHERE ALR.AllotmentLetterId = @AllotmentLetterId
GROUP BY ALR.SerialNo, ALR.RecipientName, ALR.RecipientNameBn, ALR.StaffStrength
ORDER BY ALR.SerialNo;

PRINT '✅ Multi-Recipient AllotmentLetter created successfully!';
PRINT 'Total Recipients: 4';
PRINT 'Total Unique Items: 5';
PRINT 'Total Recipient Items: 17';

GO
