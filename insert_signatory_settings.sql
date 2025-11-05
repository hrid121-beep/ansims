-- Insert Default Signatory Settings for AllotmentLetter
USE ansvdp_ims;
GO

DECLARE @CurrentDate DATETIME = GETDATE();

-- Delete existing signatory settings if any
DELETE FROM Settings WHERE [Key] LIKE 'AllotmentLetter.Signatory%';

-- Insert Signatory Name
INSERT INTO Settings ([Key], Value, Description, Category, DataType, IsReadOnly, CreatedAt, CreatedBy, IsActive)
VALUES (
    'AllotmentLetter.SignatoryName',
    'এ.বি.এম. ফরহাদ, বিবিএম',
    'Default signatory name for Allotment Letters (can be overridden in form)',
    'AllotmentLetter',
    'String',
    0,
    @CurrentDate,
    'system',
    1
);

-- Insert Signatory Designation (English)
INSERT INTO Settings ([Key], Value, Description, Category, DataType, IsReadOnly, CreatedAt, CreatedBy, IsActive)
VALUES (
    'AllotmentLetter.SignatoryDesignation',
    'Deputy Director (Store)',
    'Default signatory designation in English for Allotment Letters',
    'AllotmentLetter',
    'String',
    0,
    @CurrentDate,
    'system',
    1
);

-- Insert Signatory Designation (Bengali)
INSERT INTO Settings ([Key], Value, Description, Category, DataType, IsReadOnly, CreatedAt, CreatedBy, IsActive)
VALUES (
    'AllotmentLetter.SignatoryDesignationBn',
    'উপপরিচালক (ভাণ্ডার)',
    'Default signatory designation in Bengali for Allotment Letters',
    'AllotmentLetter',
    'String',
    0,
    @CurrentDate,
    'system',
    1
);

-- Insert Signatory ID
INSERT INTO Settings ([Key], Value, Description, Category, DataType, IsReadOnly, CreatedAt, CreatedBy, IsActive)
VALUES (
    'AllotmentLetter.SignatoryId',
    'বিএমভি-১২০২১৮',
    'Default signatory ID/Badge number for Allotment Letters',
    'AllotmentLetter',
    'String',
    0,
    @CurrentDate,
    'system',
    1
);

-- Insert Signatory Email
INSERT INTO Settings ([Key], Value, Description, Category, DataType, IsReadOnly, CreatedAt, CreatedBy, IsActive)
VALUES (
    'AllotmentLetter.SignatoryEmail',
    'ddstore@ansarvdp.gov.bd',
    'Default signatory email for Allotment Letters',
    'AllotmentLetter',
    'String',
    0,
    @CurrentDate,
    'system',
    1
);

-- Insert Signatory Phone
INSERT INTO Settings ([Key], Value, Description, Category, DataType, IsReadOnly, CreatedAt, CreatedBy, IsActive)
VALUES (
    'AllotmentLetter.SignatoryPhone',
    '০২-৭২১৩৪০০, ০১৭৩০০৩৮০১৩',
    'Default signatory phone number for Allotment Letters',
    'AllotmentLetter',
    'String',
    0,
    @CurrentDate,
    'system',
    1
);

PRINT 'Signatory settings inserted successfully!';

-- Verify
SELECT [Key], Value, Description
FROM Settings
WHERE [Key] LIKE 'AllotmentLetter.Signatory%'
ORDER BY [Key];

GO
