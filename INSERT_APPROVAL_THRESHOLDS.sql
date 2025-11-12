-- ====================================================================
-- INSERT DEFAULT APPROVAL THRESHOLDS
-- This script inserts default approval thresholds for critical transactions
-- ====================================================================

USE [ansvdp_ims];
GO

-- Check if thresholds already exist before inserting
IF NOT EXISTS (SELECT 1 FROM ApprovalThresholds WHERE EntityType = 'PURCHASE')
BEGIN
    PRINT 'Inserting Purchase approval thresholds...';

    -- Purchase Threshold 1: Under 10,000 - No approval required (but we'll require DDGAdmin anyway)
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('PURCHASE', 0, 9999.99, 1, 'DDGAdmin', 'Purchase orders under 10,000 require DDGAdmin approval', GETDATE(), 'System', 1);

    -- Purchase Threshold 2: 10,000 - 49,999 - DDGAdmin required
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('PURCHASE', 10000, 49999.99, 1, 'DDGAdmin', 'Purchase orders 10K-50K require DDGAdmin approval', GETDATE(), 'System', 1);

    -- Purchase Threshold 3: 50,000 - 199,999 - DDGAdmin required
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('PURCHASE', 50000, 199999.99, 1, 'DDGAdmin', 'Purchase orders 50K-200K require DDGAdmin approval', GETDATE(), 'System', 1);

    -- Purchase Threshold 4: 200,000+ - DDGAdmin and Director approval
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('PURCHASE', 200000, NULL, 2, 'Director', 'Purchase orders above 200K require Director approval', GETDATE(), 'System', 1);

    PRINT 'Purchase thresholds inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Purchase thresholds already exist. Skipping.';
END

-- Issue Thresholds
IF NOT EXISTS (SELECT 1 FROM ApprovalThresholds WHERE EntityType = 'ISSUE')
BEGIN
    PRINT 'Inserting Issue approval thresholds...';

    -- Issue Threshold 1: Under 50,000 - DDProvision approval
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('ISSUE', 0, 49999.99, 1, 'DDProvision', 'Issues under 50K require DD Provision approval', GETDATE(), 'System', 1);

    -- Issue Threshold 2: 50,000+ - Director approval
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('ISSUE', 50000, NULL, 2, 'Director', 'Issues above 50K require Director approval', GETDATE(), 'System', 1);

    PRINT 'Issue thresholds inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Issue thresholds already exist. Skipping.';
END

-- Stock Entry Thresholds
IF NOT EXISTS (SELECT 1 FROM ApprovalThresholds WHERE EntityType = 'STOCK_ENTRY')
BEGIN
    PRINT 'Inserting StockEntry approval thresholds...';

    -- Stock Entry Threshold 1: Under 25,000 - DDStore approval
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('STOCK_ENTRY', 0, 24999.99, 1, 'DDStore', 'Stock entries under 25K require DD Store approval', GETDATE(), 'System', 1);

    -- Stock Entry Threshold 2: 25,000+ - DDGAdmin approval
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('STOCK_ENTRY', 25000, NULL, 1, 'DDGAdmin', 'Stock entries above 25K require DDGAdmin approval', GETDATE(), 'System', 1);

    PRINT 'Stock Entry thresholds inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Stock Entry thresholds already exist. Skipping.';
END

-- Transfer Thresholds
IF NOT EXISTS (SELECT 1 FROM ApprovalThresholds WHERE EntityType = 'TRANSFER')
BEGIN
    PRINT 'Inserting Transfer approval thresholds...';

    -- Transfer Threshold 1: All transfers require DDStore approval
    INSERT INTO ApprovalThresholds
    (EntityType, MinAmount, MaxAmount, ApprovalLevel, RequiredRole, Description, CreatedAt, CreatedBy, IsActive)
    VALUES
    ('TRANSFER', 0, NULL, 1, 'DDStore', 'All transfers require DD Store approval', GETDATE(), 'System', 1);

    PRINT 'Transfer thresholds inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Transfer thresholds already exist. Skipping.';
END

PRINT '';
PRINT '====================================================================';
PRINT 'Approval threshold seeding completed successfully!';
PRINT '====================================================================';
PRINT '';
PRINT 'Summary:';
PRINT '- PURCHASE: 4 thresholds (0-10K, 10K-50K, 50K-200K, 200K+)';
PRINT '- ISSUE: 2 thresholds (0-50K, 50K+)';
PRINT '- STOCK_ENTRY: 2 thresholds (0-25K, 25K+)';
PRINT '- TRANSFER: 1 threshold (All)';
PRINT '';
GO
