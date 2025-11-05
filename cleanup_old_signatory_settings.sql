-- Cleanup old signatory Settings (no longer needed with SignatoryPresets system)
USE ansvdp_ims;
GO

-- Delete individual signatory settings that are now replaced by SignatoryPresets
DELETE FROM Settings
WHERE [Key] IN (
    'AllotmentLetter.SignatoryName',
    'AllotmentLetter.SignatoryDesignation',
    'AllotmentLetter.SignatoryDesignationBn',
    'AllotmentLetter.SignatoryId',
    'AllotmentLetter.SignatoryEmail',
    'AllotmentLetter.SignatoryPhone'
);

PRINT 'Old signatory settings removed. SignatoryPresets system is now in use.';

-- Verify cleanup
SELECT COUNT(*) AS RemainingSignatorySettings
FROM Settings
WHERE [Key] LIKE 'AllotmentLetter.Signatory%';

GO
