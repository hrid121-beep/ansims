-- Check if Physical Inventory exists and is active
SELECT
    Id,
    ReferenceNumber,
    StoreId,
    Status,
    IsActive,
    InitiatedBy,
    InitiatedDate,
    TotalVarianceValue
FROM PhysicalInventories
WHERE ReferenceNumber = 'PI-HQ-2025-0001';

-- Check current user's roles (replace with your user ID)
SELECT
    u.UserName,
    u.Email,
    r.Name as RoleName
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'admin'; -- Change to your current username

-- Count all Physical Inventories with UnderReview status
SELECT
    COUNT(*) as TotalUnderReview,
    SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) as ActiveCount
FROM PhysicalInventories
WHERE Status = 3; -- 3 = UnderReview
