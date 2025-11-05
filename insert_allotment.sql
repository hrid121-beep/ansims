-- Insert AllotmentLetter with Recipients and Items
-- Database: ansvdp_ims

USE ansvdp_ims;
GO

-- Step 1: Insert AllotmentLetter (Main Record)
DECLARE @AllotmentLetterId INT;
DECLARE @CurrentDate DATETIME = GETDATE();
DECLARE @ValidUntil DATETIME = DATEADD(MONTH, 3, @CurrentDate);

INSERT INTO AllotmentLetters (
    AllotmentNo,
    AllotmentDate,
    ValidFrom,
    ValidUntil,
    IssuedTo,
    IssuedToType,
    IssuedToBattalionId,
    FromStoreId,
    Purpose,
    Status,
    ReferenceNo,
    Subject,
    SubjectBn,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    'ALLOT-2025-001',
    @CurrentDate,
    @CurrentDate,
    @ValidUntil,
    '1st Battalion Dhaka',
    'Battalion',
    1, -- Battalion ID
    1, -- Dhaka Central Store
    'Monthly uniform allotment for battalion personnel',
    'Draft',
    '৪৪.০৩.০০০০.০১৮.১৩.০০১.২৫',
    'Allotment of Uniforms (Khaki Design)',
    'ইউনিফর্ম (খাকি নকশা) বরাদ্দ প্রদান প্রসঙ্গে',
    @CurrentDate,
    'admin',
    1
);

SET @AllotmentLetterId = SCOPE_IDENTITY();
PRINT 'AllotmentLetter created with ID: ' + CAST(@AllotmentLetterId AS VARCHAR(10));

-- Step 2: Insert AllotmentLetterRecipient (Recipient details)
DECLARE @RecipientId INT;

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
    'ঢাকা আনসার ব্যাটালিয়ন (১ম বিএন)',
    '৫০০',
    1,
    @CurrentDate,
    'admin',
    1
);

SET @RecipientId = SCOPE_IDENTITY();
PRINT 'Recipient created with ID: ' + CAST(@RecipientId AS VARCHAR(10));

-- Step 3: Insert AllotmentLetterRecipientItem (Items for this recipient)
-- Item 1: Uniform Shirt
INSERT INTO AllotmentLetterRecipientItems (
    AllotmentLetterRecipientId,
    ItemId,
    AllottedQuantity,
    IssuedQuantity,
    Unit,
    UnitBn,
    ItemNameBn,
    Remarks,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @RecipientId,
    1, -- Ansar Uniform Shirt (Khaki)
    100,
    0,
    'PCS',
    'টি',
    'আনসার ইউনিফর্ম শার্ট (খাকি)',
    'For new recruits',
    @CurrentDate,
    'admin',
    1
);

-- Item 2: Uniform Pant
INSERT INTO AllotmentLetterRecipientItems (
    AllotmentLetterRecipientId,
    ItemId,
    AllottedQuantity,
    IssuedQuantity,
    Unit,
    UnitBn,
    ItemNameBn,
    Remarks,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @RecipientId,
    2, -- Ansar Uniform Pant (Khaki)
    100,
    0,
    'PCS',
    'টি',
    'আনসার ইউনিফর্ম প্যান্ট (খাকি)',
    'For new recruits',
    @CurrentDate,
    'admin',
    1
);

-- Item 3: Military Boot
INSERT INTO AllotmentLetterRecipientItems (
    AllotmentLetterRecipientId,
    ItemId,
    AllottedQuantity,
    IssuedQuantity,
    Unit,
    UnitBn,
    ItemNameBn,
    Remarks,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @RecipientId,
    3, -- Military Boot (Black)
    50,
    0,
    'PAIR',
    'জোড়া',
    'সামরিক বুট (কালো)',
    'Standard issue',
    @CurrentDate,
    'admin',
    1
);

PRINT 'Recipient items created successfully';

-- Step 4: Insert AllotmentLetterItem (Summary items for the letter)
-- Item 1: Uniform Shirt
INSERT INTO AllotmentLetterItems (
    AllotmentLetterId,
    ItemId,
    AllottedQuantity,
    IssuedQuantity,
    RemainingQuantity,
    Unit,
    UnitBn,
    ItemNameBn,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @AllotmentLetterId,
    1,
    100,
    0,
    100,
    'PCS',
    'টি',
    'আনসার ইউনিফর্ম শার্ট (খাকি)',
    @CurrentDate,
    'admin',
    1
);

-- Item 2: Uniform Pant
INSERT INTO AllotmentLetterItems (
    AllotmentLetterId,
    ItemId,
    AllottedQuantity,
    IssuedQuantity,
    RemainingQuantity,
    Unit,
    UnitBn,
    ItemNameBn,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @AllotmentLetterId,
    2,
    100,
    0,
    100,
    'PCS',
    'টি',
    'আনসার ইউনিফর্ম প্যান্ট (খাকি)',
    @CurrentDate,
    'admin',
    1
);

-- Item 3: Military Boot
INSERT INTO AllotmentLetterItems (
    AllotmentLetterId,
    ItemId,
    AllottedQuantity,
    IssuedQuantity,
    RemainingQuantity,
    Unit,
    UnitBn,
    ItemNameBn,
    CreatedAt,
    CreatedBy,
    IsActive
) VALUES (
    @AllotmentLetterId,
    3,
    50,
    0,
    50,
    'PAIR',
    'জোড়া',
    'সামরিক বুট (কালো)',
    @CurrentDate,
    'admin',
    1
);

PRINT 'Summary items created successfully';

-- Verification Query
SELECT
    AL.Id,
    AL.AllotmentNo,
    AL.Status,
    AL.Purpose,
    AL.ReferenceNo,
    COUNT(DISTINCT ALR.Id) AS RecipientCount,
    COUNT(DISTINCT ALI.Id) AS ItemCount
FROM AllotmentLetters AL
LEFT JOIN AllotmentLetterRecipients ALR ON AL.Id = ALR.AllotmentLetterId
LEFT JOIN AllotmentLetterItems ALI ON AL.Id = ALI.AllotmentLetterId
WHERE AL.Id = @AllotmentLetterId
GROUP BY AL.Id, AL.AllotmentNo, AL.Status, AL.Purpose, AL.ReferenceNo;

PRINT 'AllotmentLetter entry completed successfully!';
GO
