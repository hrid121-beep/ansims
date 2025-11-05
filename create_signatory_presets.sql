-- Create SignatoryPresets table for AllotmentLetter
USE ansvdp_ims;
GO

-- Create table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SignatoryPresets')
BEGIN
    CREATE TABLE SignatoryPresets (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PresetName NVARCHAR(200) NOT NULL,           -- Display name in dropdown
        PresetNameBn NVARCHAR(200) NOT NULL,         -- Bengali display name

        SignatoryName NVARCHAR(200) NOT NULL,        -- Full name
        SignatoryDesignation NVARCHAR(200),          -- English designation
        SignatoryDesignationBn NVARCHAR(200),        -- Bengali designation
        SignatoryId NVARCHAR(100),                   -- Badge/ID number
        SignatoryPhone NVARCHAR(100),                -- Contact phone
        SignatoryEmail NVARCHAR(200),                -- Email address

        Department NVARCHAR(100),                     -- Store, Admin, Provision, etc.
        IsDefault BIT NOT NULL DEFAULT 0,             -- Is this the default preset?
        DisplayOrder INT NOT NULL DEFAULT 0,          -- Sort order in dropdown

        -- Audit fields
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100),
        UpdatedAt DATETIME,
        UpdatedBy NVARCHAR(100)
    );

    PRINT 'Table SignatoryPresets created successfully!';
END
ELSE
BEGIN
    PRINT 'Table SignatoryPresets already exists.';
END
GO

-- Insert default presets
-- Preset 1: উপপরিচালক (ভাণ্ডার) - DEFAULT
INSERT INTO SignatoryPresets (
    PresetName, PresetNameBn,
    SignatoryName, SignatoryDesignation, SignatoryDesignationBn,
    SignatoryId, SignatoryPhone, SignatoryEmail,
    Department, IsDefault, DisplayOrder,
    CreatedBy, IsActive
) VALUES (
    'A.B.M. Forhad - Deputy Director (Store)',
    'এ.বি.এম. ফরহাদ - উপপরিচালক (ভাণ্ডার)',
    'এ.বি.এম. ফরহাদ, বিবিএম',
    'Deputy Director (Store)',
    'উপপরিচালক (ভাণ্ডার)',
    'বিএমভি-১২০২১৮',
    '০২-৭২১৩৪০০, ০১৭৩০০৩৮০১৩',
    'ddstore@ansarvdp.gov.bd',
    'Store',
    1, -- Default
    1,
    'system',
    1
);

-- Preset 2: উপ-মহাপরিচালক (প্রশাসন)
INSERT INTO SignatoryPresets (
    PresetName, PresetNameBn,
    SignatoryName, SignatoryDesignation, SignatoryDesignationBn,
    SignatoryId, SignatoryPhone, SignatoryEmail,
    Department, IsDefault, DisplayOrder,
    CreatedBy, IsActive
) VALUES (
    'Md. Abdul Karim - Deputy Director General (Admin)',
    'মোঃ আবদুল করিম - উপ-মহাপরিচালক (প্রশাসন)',
    'মোঃ আবদুল করিম',
    'Deputy Director General (Admin)',
    'উপ-মহাপরিচালক (প্রশাসন)',
    'DDG-ADM-001',
    '০২-৯৫৫৮০৯০',
    'ddg.admin@ansar-vdp.gov.bd',
    'Admin',
    0,
    2,
    'system',
    1
);

-- Preset 3: উপপরিচালক (প্রভিশন)
INSERT INTO SignatoryPresets (
    PresetName, PresetNameBn,
    SignatoryName, SignatoryDesignation, SignatoryDesignationBn,
    SignatoryId, SignatoryPhone, SignatoryEmail,
    Department, IsDefault, DisplayOrder,
    CreatedBy, IsActive
) VALUES (
    'Deputy Director (Provision)',
    'উপপরিচালক (প্রভিশন)',
    'জনাব মোঃ রহিম উদ্দিন',
    'Deputy Director (Provision)',
    'উপপরিচালক (প্রভিশন)',
    'DD-PROV-001',
    '০২-৯৫৫৮০৯১',
    'ddprovision@ansar-vdp.gov.bd',
    'Provision',
    0,
    3,
    'system',
    1
);

-- Preset 4: সহকারী পরিচালক (ভাণ্ডার)
INSERT INTO SignatoryPresets (
    PresetName, PresetNameBn,
    SignatoryName, SignatoryDesignation, SignatoryDesignationBn,
    SignatoryId, SignatoryPhone, SignatoryEmail,
    Department, IsDefault, DisplayOrder,
    CreatedBy, IsActive
) VALUES (
    'Assistant Director (Store)',
    'সহকারী পরিচালক (ভাণ্ডার)',
    'জনাব সাইফুল ইসলাম',
    'Assistant Director (Store)',
    'সহকারী পরিচালক (ভাণ্ডার)',
    'AD-STORE-001',
    '০২-৭২১৩৪০১',
    'adstore@ansar-vdp.gov.bd',
    'Store',
    0,
    4,
    'system',
    1
);

PRINT 'Signatory presets inserted successfully!';

-- Verify
SELECT
    Id,
    PresetNameBn AS 'Preset Name',
    SignatoryName AS 'Signatory',
    SignatoryDesignationBn AS 'Designation',
    Department,
    IsDefault AS 'Default?'
FROM SignatoryPresets
WHERE IsActive = 1
ORDER BY DisplayOrder;

GO
