-- Fix old receives that are missing StoreId and OriginalIssueId
-- Run this SQL script to update existing receives

-- First, check which receives need fixing
SELECT
    Id,
    ReceiveNo,
    ReceiveType,
    StoreId,
    OriginalIssueId,
    Status
FROM Receives
WHERE ReceiveType = 'Issue'
  AND (StoreId IS NULL OR OriginalIssueId IS NULL);

-- Update receives with missing data
-- You need to manually set the correct StoreId for each receive
-- Example: Update RCV-202511-0010 and RCV-202511-0011 with appropriate store

-- UPDATE Receives
-- SET StoreId = 1,  -- Change this to the correct Store ID
--     OriginalIssueId = NULL  -- Set to NULL if you don't know the original issue
-- WHERE Id = 13;  -- Change to the correct Receive ID

-- After updating, verify:
SELECT
    r.Id,
    r.ReceiveNo,
    r.StoreId,
    s.Name as StoreName,
    r.OriginalIssueId
FROM Receives r
LEFT JOIN Stores s ON r.StoreId = s.Id
WHERE r.ReceiveType = 'Issue';
