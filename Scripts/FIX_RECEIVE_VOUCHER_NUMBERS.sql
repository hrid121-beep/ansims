-- =============================================
-- Fix Receive Voucher Number Mismatches
-- =============================================
-- Problem: Receive PDFs showing wrong Issue voucher numbers
-- Example: User received from ISS-202511-0008 but PDF shows ISS-202511-0005
-- =============================================

-- Step 1: Find all Receives with mismatched OriginalIssueNo
SELECT
    r.Id AS ReceiveId,
    r.ReceiveNo,
    r.OriginalIssueId,
    r.OriginalIssueNo AS [Stored_IssueNo],
    i.IssueNo AS [Actual_IssueNo],
    i.VoucherNumber AS [Actual_VoucherNo],
    r.OriginalVoucherNo AS [Stored_VoucherNo],
    CASE
        WHEN r.OriginalIssueNo = i.IssueNo THEN '✅ Match'
        WHEN r.OriginalIssueNo IS NULL THEN '⚠️ NULL'
        ELSE '❌ MISMATCH'
    END AS IssueNo_Status,
    CASE
        WHEN r.OriginalVoucherNo = i.VoucherNumber THEN '✅ Match'
        WHEN r.OriginalVoucherNo IS NULL THEN '⚠️ NULL'
        ELSE '❌ MISMATCH'
    END AS VoucherNo_Status,
    r.ReceiverName,
    r.ReceiverBadgeNo,
    r.ReceivedDate,
    r.CreatedAt
FROM Receives r
LEFT JOIN Issues i ON r.OriginalIssueId = i.Id
WHERE r.OriginalIssueId IS NOT NULL
ORDER BY r.Id DESC;

-- Step 2: Count mismatches
SELECT
    COUNT(*) AS Total_Receives,
    SUM(CASE WHEN r.OriginalIssueNo = i.IssueNo THEN 1 ELSE 0 END) AS Matching,
    SUM(CASE WHEN r.OriginalIssueNo != i.IssueNo OR r.OriginalIssueNo IS NULL THEN 1 ELSE 0 END) AS Mismatched
FROM Receives r
LEFT JOIN Issues i ON r.OriginalIssueId = i.Id
WHERE r.OriginalIssueId IS NOT NULL;

-- Step 3: Find specific receives user mentioned
SELECT
    r.Id,
    r.ReceiveNo,
    r.OriginalIssueNo AS [Stored_IssueNo],
    i.IssueNo AS [Actual_IssueNo],
    'Should be: ' + i.IssueNo AS Correction
FROM Receives r
LEFT JOIN Issues i ON r.OriginalIssueId = i.Id
WHERE r.OriginalIssueNo LIKE '%ISS-202511%'
   OR i.IssueNo LIKE '%ISS-202511-0008%'
ORDER BY r.Id DESC;

-- Step 4: Preview what will be fixed
SELECT
    r.Id AS ReceiveId,
    r.ReceiveNo,
    r.OriginalIssueNo AS [BEFORE_IssueNo],
    i.IssueNo AS [AFTER_IssueNo],
    r.OriginalVoucherNo AS [BEFORE_VoucherNo],
    i.VoucherNumber AS [AFTER_VoucherNo]
FROM Receives r
INNER JOIN Issues i ON r.OriginalIssueId = i.Id
WHERE r.OriginalIssueNo != i.IssueNo
   OR r.OriginalIssueNo IS NULL
   OR r.OriginalVoucherNo != i.VoucherNumber
   OR r.OriginalVoucherNo IS NULL
ORDER BY r.Id;

-- Step 5: FIX - Update OriginalIssueNo and OriginalVoucherNo
-- UNCOMMENT THE FOLLOWING LINES TO EXECUTE THE FIX
/*
BEGIN TRANSACTION;

UPDATE r
SET
    r.OriginalIssueNo = i.IssueNo,
    r.OriginalVoucherNo = i.VoucherNumber,
    r.UpdatedAt = GETDATE(),
    r.UpdatedBy = 'System - Voucher Number Fix'
FROM Receives r
INNER JOIN Issues i ON r.OriginalIssueId = i.Id
WHERE (r.OriginalIssueNo != i.IssueNo OR r.OriginalIssueNo IS NULL)
   OR (r.OriginalVoucherNo != i.VoucherNumber OR r.OriginalVoucherNo IS NULL);

-- Verify the fix
SELECT
    COUNT(*) AS Fixed_Count,
    'Updated OriginalIssueNo and OriginalVoucherNo to match Issues table' AS Description
FROM Receives r
INNER JOIN Issues i ON r.OriginalIssueId = i.Id
WHERE r.UpdatedBy = 'System - Voucher Number Fix';

-- Show updated records
SELECT
    r.Id AS ReceiveId,
    r.ReceiveNo,
    r.OriginalIssueNo,
    i.IssueNo AS [Should_Match],
    CASE WHEN r.OriginalIssueNo = i.IssueNo THEN '✅' ELSE '❌' END AS Status
FROM Receives r
INNER JOIN Issues i ON r.OriginalIssueId = i.Id
WHERE r.UpdatedBy = 'System - Voucher Number Fix';

-- If everything looks good, commit the transaction
COMMIT TRANSACTION;

-- If something went wrong, rollback
-- ROLLBACK TRANSACTION;
*/

-- Step 6: Verify all Receives now have correct numbers
SELECT
    COUNT(*) AS Total_Linked_Receives,
    SUM(CASE WHEN r.OriginalIssueNo = i.IssueNo THEN 1 ELSE 0 END) AS Correct_IssueNo,
    SUM(CASE WHEN r.OriginalVoucherNo = i.VoucherNumber THEN 1 ELSE 0 END) AS Correct_VoucherNo
FROM Receives r
INNER JOIN Issues i ON r.OriginalIssueId = i.Id;

-- Step 7: Find receives that still need manual review
SELECT
    r.Id,
    r.ReceiveNo,
    r.OriginalIssueId,
    r.OriginalIssueNo,
    'No matching Issue found!' AS Warning
FROM Receives r
WHERE r.OriginalIssueId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM Issues i WHERE i.Id = r.OriginalIssueId);

-- =============================================
-- Instructions:
-- =============================================
-- 1. Run Step 1 to see all mismatches
-- 2. Run Step 2 to see count summary
-- 3. Run Step 3 to find specific receives user mentioned
-- 4. Run Step 4 to preview what will be fixed
-- 5. Uncomment Step 5 and run to fix the data
-- 6. Run Step 6 to verify fix worked
-- 7. Run Step 7 to check for orphaned receives
--
-- After fixing, PDFs will show correct voucher numbers!
-- =============================================
