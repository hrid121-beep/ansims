-- Fix SignatoryPresets with correct Bengali encoding
USE ansvdp_ims;
GO

-- Insert correct data with N prefix for Unicode
INSERT INTO SignatoryPresets (
    PresetName,
    PresetNameBn,
    SignatoryName,
    SignatoryDesignation,
    SignatoryDesignationBn,
    SignatoryId,
    SignatoryPhone,
    SignatoryEmail,
    Department,
    IsDefault,
    DisplayOrder,
    IsActive,
    CreatedBy,
    CreatedAt
) VALUES
-- Preset 1: Deputy Director (Store)
(
    'A.B.M. Forhad - Deputy Director (Store)',
    N'এ.বি.এম. ফরহাদ - উপ-পরিচালক (ভাণ্ডার)',
    'A.B.M. Forhad',
    'Deputy Director (Store)',
    N'উপ-পরিচালক (ভাণ্ডার)',
    'ANS-DD-001',
    '+880-2-9898989',
    'dd.store@ansar-vdp.gov.bd',
    'Store',
    1, -- Default
    1,
    1,
    'system',
    GETDATE()
),
-- Preset 2: Deputy Director General (Admin)
(
    'Md. Abdul Karim - Deputy Director General (Admin)',
    N'মোঃ আবদুল করিম - উপ-মহাপরিচালক (প্রশাসন)',
    'Md. Abdul Karim',
    'Deputy Director General (Admin)',
    N'উপ-মহাপরিচালক (প্রশাসন)',
    'ANS-DDG-002',
    '+880-2-9898990',
    'ddg.admin@ansar-vdp.gov.bd',
    'Admin',
    0,
    2,
    1,
    'system',
    GETDATE()
),
-- Preset 3: Deputy Director (Provision)
(
    'Deputy Director (Provision)',
    N'উপ-পরিচালক (প্রভিশন)',
    'Mohammad Hasan',
    'Deputy Director (Provision)',
    N'উপ-পরিচালক (প্রভিশন)',
    'ANS-DD-003',
    '+880-2-9898991',
    'dd.provision@ansar-vdp.gov.bd',
    'Provision',
    0,
    3,
    1,
    'system',
    GETDATE()
),
-- Preset 4: Assistant Director (Store)
(
    'Assistant Director (Store)',
    N'সহকারী পরিচালক (ভাণ্ডার)',
    'Md. Jahangir Alam',
    'Assistant Director (Store)',
    N'সহকারী পরিচালক (ভাণ্ডার)',
    'ANS-AD-004',
    '+880-2-9898992',
    'ad.store@ansar-vdp.gov.bd',
    'Store',
    0,
    4,
    1,
    'system',
    GETDATE()
);

-- Verify the data
SELECT
    Id,
    PresetName,
    PresetNameBn,
    SignatoryName,
    SignatoryDesignation,
    SignatoryDesignationBn,
    Department,
    IsDefault,
    DisplayOrder
FROM SignatoryPresets
ORDER BY DisplayOrder;
