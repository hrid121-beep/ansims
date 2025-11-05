-- ================================================
-- COMPREHENSIVE DATA SEEDING SCRIPT
-- Ansar VDP Inventory Management System
-- ================================================

USE ansvdp_ims;
GO

-- ================================================
-- 1. ORGANIZATIONAL STRUCTURE
-- ================================================

-- Ranges
INSERT INTO Ranges (Name, NameBn, Code, Address, Phone, Email, CreatedAt, CreatedBy, IsActive)
VALUES
('Dhaka Range', N'ঢাকা রেঞ্জ', 'DHK-RNG', 'Dhaka', '02-9876543', 'dhaka.range@ansarvdp.gov.bd', GETDATE(), 'system', 1),
('Rajshahi Range', N'রাজশাহী রেঞ্জ', 'RAJ-RNG', 'Rajshahi', '0721-123456', 'rajshahi.range@ansarvdp.gov.bd', GETDATE(), 'system', 1),
('Chittagong Range', N'চট্টগ্রাম রেঞ্জ', 'CTG-RNG', 'Chittagong', '031-654321', 'ctg.range@ansarvdp.gov.bd', GETDATE(), 'system', 1),
('Khulna Range', N'খুলনা রেঞ্জ', 'KHL-RNG', 'Khulna', '041-123456', 'khulna.range@ansarvdp.gov.bd', GETDATE(), 'system', 1),
('Sylhet Range', N'সিলেট রেঞ্জ', 'SYL-RNG', 'Sylhet', '0821-123456', 'sylhet.range@ansarvdp.gov.bd', GETDATE(), 'system', 1);

-- Battalions
INSERT INTO Battalions (Name, NameBn, Code, RangeId, Address, Phone, CreatedAt, CreatedBy, IsActive)
SELECT '1st Battalion Dhaka', N'১ম ব্যাটালিয়ন ঢাকা', '1BN', Id, 'Mirpur, Dhaka', '02-9012345', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT '2nd Battalion Dhaka', N'২য় ব্যাটালিয়ন ঢাকা', '2BN', Id, 'Uttara, Dhaka', '02-9012346', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT '3rd Battalion Chittagong', N'৩য় ব্যাটালিয়ন চট্টগ্রাম', '3BN', Id, 'Agrabad, CTG', '031-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'CTG-RNG'
UNION ALL
SELECT '4th Battalion Rajshahi', N'৪র্থ ব্যাটালিয়ন রাজশাহী', '4BN', Id, 'Rajshahi', '0721-234567', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'RAJ-RNG'
UNION ALL
SELECT '14th Battalion Chotomerun', N'১৪তম ব্যাটালিয়ন ছোটেমেরুন', '14BN', Id, 'Patuakhali', '0441-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT '22nd Battalion Patuakhali', N'২২তম ব্যাটালিয়ন পটুয়াখালী', '22BN', Id, 'Patuakhali', '0441-234567', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT '26th Battalion Netrokona', N'২৬তম ব্যাটালিয়ন নেত্রকোনা', '26BN', Id, 'Netrokona', '0951-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT '33rd Battalion Dinajpur', N'৩৩তম ব্যাটালিয়ন দিনাজপুর', '33BN', Id, 'Dinajpur', '0531-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'RAJ-RNG'
UNION ALL
SELECT '36th Battalion Manikganj', N'৩৬তম ব্যাটালিয়ন মানিকগঞ্জ', '36BN', Id, 'Manikganj', '0651-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT '37th Battalion Patiya', N'৩৭তম ব্যাটালিয়ন পাটিয়া', '37BN', Id, 'Patiya, CTG', '031-234567', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'CTG-RNG';

-- Zilas
INSERT INTO Zilas (Name, NameBn, Code, RangeId, Address, Phone, CreatedAt, CreatedBy, IsActive)
SELECT 'Dhaka Zila', N'ঢাকা জেলা', 'DHK', Id, 'Dhaka', '02-9876543', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT 'Gazipur Zila', N'গাজীপুর জেলা', 'GAZ', Id, 'Gazipur', '02-9234567', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT 'Chittagong Zila', N'চট্টগ্রাম জেলা', 'CTG', Id, 'Chittagong', '031-654321', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'CTG-RNG'
UNION ALL
SELECT 'Rajshahi Zila', N'রাজশাহী জেলা', 'RAJ', Id, 'Rajshahi', '0721-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'RAJ-RNG'
UNION ALL
SELECT 'Lalmonirhat Zila', N'লালমনিরহাট জেলা', 'LAL', Id, 'Lalmonirhat', '0591-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'RAJ-RNG'
UNION ALL
SELECT 'Narail Zila', N'নড়াইল জেলা', 'NAR', Id, 'Narail', '0481-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'KHL-RNG'
UNION ALL
SELECT 'Jhalokathi Zila', N'ঝালকাঠি জেলা', 'JHA', Id, 'Jhalokathi', '0498-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'DHK-RNG'
UNION ALL
SELECT 'Jhinaidah Zila', N'ঝিনাইদহ জেলা', 'JHI', Id, 'Jhinaidah', '0451-123456', GETDATE(), 'system', 1 FROM Ranges WHERE Code = 'KHL-RNG';

-- Upazilas
INSERT INTO Upazilas (Name, NameBn, Code, ZilaId, Address, Phone, CreatedAt, CreatedBy, IsActive)
SELECT 'Mirpur', N'মিরপুর', 'MIR', Id, 'Mirpur', '02-9012345', GETDATE(), 'system', 1 FROM Zilas WHERE Code = 'DHK'
UNION ALL
SELECT 'Uttara', N'উত্তরা', 'UTT', Id, 'Uttara', '02-9012346', GETDATE(), 'system', 1 FROM Zilas WHERE Code = 'DHK'
UNION ALL
SELECT 'Savar', N'সাভার', 'SAV', Id, 'Savar', '02-7791234', GETDATE(), 'system', 1 FROM Zilas WHERE Code = 'DHK'
UNION ALL
SELECT 'Agrabad', N'আগ্রাবাদ', 'AGR', Id, 'Agrabad', '031-234567', GETDATE(), 'system', 1 FROM Zilas WHERE Code = 'CTG';

GO

-- ================================================
-- 2. STORES
-- ================================================

INSERT INTO Stores (Name, NameBn, Code, StoreTypeId, Location, Address, Phone, Email, CreatedAt, CreatedBy, IsActive)
SELECT 'Central Ansar/VDP Store', N'কেন্দ্রীয় আনসার/ভিডিপি ভান্ডার', 'CENTRAL-HQ', Id, 'Ansar-VDP Academy, Shafipur, Gazipur', 'Shafipur, Gazipur', '02-9876543', 'central.store@ansarvdp.gov.bd', GETDATE(), 'system', 1 FROM StoreTypes WHERE Code = 'CENTRAL'
UNION ALL
SELECT 'Provision Store Headquarters', N'প্রভিশন ভান্ডার সদরদপ্তর', 'PROV-HQ', Id, 'Headquarters, Khilgaon, Dhaka', 'Khilgaon, Dhaka', '02-8765432', 'provision.store@ansarvdp.gov.bd', GETDATE(), 'system', 1 FROM StoreTypes WHERE Code = 'PROVISION';

-- Battalion Stores
INSERT INTO Stores (Name, NameBn, Code, StoreTypeId, BattalionId, Location, Address, Phone, CreatedAt, CreatedBy, IsActive)
SELECT '1st Battalion Store', N'১ম ব্যাটালিয়ন ভান্ডার', 'STORE-1BN', st.Id, b.Id, 'Mirpur, Dhaka', 'Mirpur', '02-9012345', GETDATE(), 'system', 1
FROM StoreTypes st, Battalions b WHERE st.Code = 'OTHER' AND b.Code = '1BN'
UNION ALL
SELECT '14th Battalion Store', N'১৪তম ব্যাটালিয়ন ভান্ডার', 'STORE-14BN', st.Id, b.Id, 'Patuakhali', 'Patuakhali', '0441-123456', GETDATE(), 'system', 1
FROM StoreTypes st, Battalions b WHERE st.Code = 'OTHER' AND b.Code = '14BN'
UNION ALL
SELECT '22nd Battalion Store', N'২২তম ব্যাটালিয়ন ভান্ডার', 'STORE-22BN', st.Id, b.Id, 'Patuakhali', 'Patuakhali', '0441-234567', GETDATE(), 'system', 1
FROM StoreTypes st, Battalions b WHERE st.Code = 'OTHER' AND b.Code = '22BN'
UNION ALL
SELECT '4th Battalion Store', N'৪র্থ ব্যাটালিয়ন ভান্ডার', 'STORE-4BN', st.Id, b.Id, 'Rajshahi', 'Rajshahi', '0721-234567', GETDATE(), 'system', 1
FROM StoreTypes st, Battalions b WHERE st.Code = 'OTHER' AND b.Code = '4BN';

-- Range Stores
INSERT INTO Stores (Name, NameBn, Code, StoreTypeId, RangeId, Location, Address, Phone, CreatedAt, CreatedBy, IsActive)
SELECT 'Rajshahi Range Store', N'রাজশাহী রেঞ্জ ভান্ডার', 'STORE-RAJ-RNG', st.Id, r.Id, 'Rajshahi', 'Rajshahi', '0721-123456', GETDATE(), 'system', 1
FROM StoreTypes st, Ranges r WHERE st.Code = 'OTHER' AND r.Code = 'RAJ-RNG';

-- Zila Stores
INSERT INTO Stores (Name, NameBn, Code, StoreTypeId, ZilaId, Location, Address, Phone, CreatedAt, CreatedBy, IsActive)
SELECT 'Jhalokathi Zila Store', N'ঝালকাঠি জেলা ভান্ডার', 'STORE-JHA', st.Id, z.Id, 'Jhalokathi', 'Jhalokathi', '0498-123456', GETDATE(), 'system', 1
FROM StoreTypes st, Zilas z WHERE st.Code = 'OTHER' AND z.Code = 'JHA'
UNION ALL
SELECT 'Jhinaidah Zila Store', N'ঝিনাইদহ জেলা ভান্ডার', 'STORE-JHI', st.Id, z.Id, 'Jhinaidah', 'Jhinaidah', '0451-123456', GETDATE(), 'system', 1
FROM StoreTypes st, Zilas z WHERE st.Code = 'OTHER' AND z.Code = 'JHI'
UNION ALL
SELECT 'Narail Zila Store', N'নড়াইল জেলা ভান্ডার', 'STORE-NAR', st.Id, z.Id, 'Narail', 'Narail', '0481-123456', GETDATE(), 'system', 1
FROM StoreTypes st, Zilas z WHERE st.Code = 'OTHER' AND z.Code = 'NAR'
UNION ALL
SELECT 'Lalmonirhat Zila Store', N'লালমনিরহাট জেলা ভান্ডার', 'STORE-LAL', st.Id, z.Id, 'Lalmonirhat', 'Lalmonirhat', '0591-123456', GETDATE(), 'system', 1
FROM StoreTypes st, Zilas z WHERE st.Code = 'OTHER' AND z.Code = 'LAL';

GO

-- ================================================
-- 3. CATEGORIES & ITEMS
-- ================================================

-- Categories
INSERT INTO Categories (Name, NameBn, Code, Description, CreatedAt, CreatedBy, IsActive)
VALUES
('Electronics', N'ইলেকট্রনিক্স', 'ELEC', 'Electronic items and accessories', GETDATE(), 'system', 1),
('Office Equipment', N'অফিস সরঞ্জাম', 'OFF', 'Office equipment and supplies', GETDATE(), 'system', 1),
('Furniture', N'আসবাবপত্র', 'FURN', 'Furniture items', GETDATE(), 'system', 1),
('Clothing', N'পোশাক', 'CLOTH', 'Uniforms and clothing', GETDATE(), 'system', 1),
('Accessories', N'আনুষাঙ্গিক', 'ACC', 'Accessories and small items', GETDATE(), 'system', 1);

-- Subcategories
INSERT INTO Subcategories (Name, NameBn, Code, CategoryId, CreatedAt, CreatedBy, IsActive)
SELECT 'Computers', N'কম্পিউটার', 'COMP', Id, GETDATE(), 'system', 1 FROM Categories WHERE Code = 'ELEC'
UNION ALL
SELECT 'Computer Accessories', N'কম্পিউটার আনুষাঙ্গিক', 'COMP-ACC', Id, GETDATE(), 'system', 1 FROM Categories WHERE Code = 'ELEC'
UNION ALL
SELECT 'Printers', N'প্রিন্টার', 'PRINT', Id, GETDATE(), 'system', 1 FROM Categories WHERE Code = 'ELEC'
UNION ALL
SELECT 'Scanners', N'স্ক্যানার', 'SCAN', Id, GETDATE(), 'system', 1 FROM Categories WHERE Code = 'ELEC'
UNION ALL
SELECT 'UPS', N'ইউপিএস', 'UPS', Id, GETDATE(), 'system', 1 FROM Categories WHERE Code = 'ELEC'
UNION ALL
SELECT 'Bags', N'ব্যাগ', 'BAG', Id, GETDATE(), 'system', 1 FROM Categories WHERE Code = 'ACC';

-- Items
INSERT INTO Items (ItemCode, Name, NameBn, CategoryId, SubcategoryId, Unit, UnitBn, UnitPrice, ReorderLevel, MinimumStockLevel, MaximumStockLevel,
                   IsPerishable, AnsarLifespanMonths, VDPLifespanMonths, AnsarAlertBeforeDays, VDPAlertBeforeDays, AnsarEntitlementQuantity, VDPEntitlementQuantity, CreatedAt, CreatedBy, IsActive)
SELECT 'DESK-PC-001', 'Desktop Computer Set', N'ডেস্কটপ কম্পিউটার সেট', c.Id, s.Id, 'Set', N'সেট', 45000, 5, 3, 50, 0, 60, 60, 180, 180, 1, 1, GETDATE(), 'system', 1
FROM Categories c, Subcategories s WHERE c.Code = 'ELEC' AND s.Code = 'COMP'
UNION ALL
SELECT 'PRINT-001', 'Laser Printer', N'লেজার প্রিন্টার', c.Id, s.Id, 'Unit', N'টি', 15000, 10, 5, 100, 0, 36, 36, 90, 90, NULL, NULL, GETDATE(), 'system', 1
FROM Categories c, Subcategories s WHERE c.Code = 'ELEC' AND s.Code = 'PRINT'
UNION ALL
SELECT 'SCAN-001', 'Flatbed Scanner', N'স্ক্যানার', c.Id, s.Id, 'Unit', N'টি', 8000, 5, 3, 50, 0, 36, 36, 90, 90, NULL, NULL, GETDATE(), 'system', 1
FROM Categories c, Subcategories s WHERE c.Code = 'ELEC' AND s.Code = 'SCAN'
UNION ALL
SELECT 'UPS-001', 'UPS 650VA', N'ইউপিএস ৬৫০VA', c.Id, s.Id, 'Unit', N'টি', 5000, 10, 5, 100, 0, 24, 24, 60, 60, NULL, NULL, GETDATE(), 'system', 1
FROM Categories c, Subcategories s WHERE c.Code = 'ELEC' AND s.Code = 'UPS'
UNION ALL
SELECT 'MOUSE-001', 'Optical Mouse', N'অপটিক্যাল মাউস', c.Id, s.Id, 'Piece', N'টি', 200, 50, 20, 500, 0, 12, 12, 30, 30, NULL, NULL, GETDATE(), 'system', 1
FROM Categories c, Subcategories s WHERE c.Code = 'ELEC' AND s.Code = 'COMP-ACC'
UNION ALL
SELECT 'KB-001', 'USB Keyboard', N'কীবোর্ড', c.Id, s.Id, 'Piece', N'টি', 500, 30, 10, 300, 0, 24, 24, 60, 60, NULL, NULL, GETDATE(), 'system', 1
FROM Categories c, Subcategories s WHERE c.Code = 'ELEC' AND s.Code = 'COMP-ACC'
UNION ALL
SELECT 'BAG-KIT-001', 'Kit Bag (New Design)', N'কিট ব্যাগ (নতুন নকশা)', c.Id, s.Id, 'Piece', N'টি', 800, 100, 50, 2000, 0, 36, 36, 90, 90, 1, 1, GETDATE(), 'system', 1
FROM Categories c, Subcategories s WHERE c.Code = 'ACC' AND s.Code = 'BAG';

GO

-- ================================================
-- 4. SUPPLIERS
-- ================================================

INSERT INTO Suppliers (Name, NameBn, Code, ContactPerson, Phone, Email, Address, TIN, Rating, CreatedAt, CreatedBy, IsActive)
VALUES
('Tech Solutions Ltd', N'টেক সল্যুশনস লি.', 'TECH-001', 'Mr. Karim', '01711-123456', 'info@techsolutions.com', 'Dhaka', '123456789', 4.5, GETDATE(), 'system', 1),
('Computer World', N'কম্পিউটার ওয়ার্ল্ড', 'CW-001', 'Mr. Rahman', '01711-234567', 'sales@computerworld.com', 'Chittagong', '987654321', 4.0, GETDATE(), 'system', 1),
('Office Supplies BD', N'অফিস সাপ্লাইজ বিডি', 'OS-001', 'Mrs. Sultana', '01711-345678', 'info@officesupplies.com', 'Dhaka', '456789123', 4.2, GETDATE(), 'system', 1);

GO

-- ================================================
-- 5. PURCHASES & STOCK
-- ================================================

DECLARE @CentralStoreId INT = (SELECT Id FROM Stores WHERE Code = 'CENTRAL-HQ');
DECLARE @Supplier1Id INT = (SELECT Id FROM Suppliers WHERE Code = 'TECH-001');
DECLARE @Supplier2Id INT = (SELECT Id FROM Suppliers WHERE Code = 'OS-001');
DECLARE @DesktopItemId INT = (SELECT Id FROM Items WHERE ItemCode = 'DESK-PC-001');
DECLARE @KitBagItemId INT = (SELECT Id FROM Items WHERE ItemCode = 'BAG-KIT-001');
DECLARE @MouseItemId INT = (SELECT Id FROM Items WHERE ItemCode = 'MOUSE-001');
DECLARE @KeyboardItemId INT = (SELECT Id FROM Items WHERE ItemCode = 'KB-001');

-- Purchase 1: Desktops
INSERT INTO Purchases (PurchaseOrderNo, OrderDate, StoreId, SupplierId, TotalAmount, Status, DeliveryDate, ReceivedDate, CreatedAt, CreatedBy, IsActive)
VALUES ('PO-2024-001', DATEADD(MONTH, -2, GETDATE()), @CentralStoreId, @Supplier1Id, 450000, 'Received', DATEADD(MONTH, -2, DATEADD(DAY, 7, GETDATE())), DATEADD(MONTH, -2, DATEADD(DAY, 7, GETDATE())), DATEADD(MONTH, -2, GETDATE()), 'admin', 1);

DECLARE @Purchase1Id INT = SCOPE_IDENTITY();

INSERT INTO PurchaseItems (PurchaseId, ItemId, Quantity, UnitPrice, TotalPrice, ReceivedQuantity, CreatedAt, CreatedBy, IsActive)
VALUES (@Purchase1Id, @DesktopItemId, 10, 45000, 450000, 10, DATEADD(MONTH, -2, GETDATE()), 'admin', 1);

-- Stock Movement for Purchase 1
INSERT INTO StockMovements (ItemId, StoreId, MovementType, Quantity, ReferenceNo, MovementDate, Notes, CreatedAt, CreatedBy, IsActive)
VALUES (@DesktopItemId, @CentralStoreId, 'Purchase', 10, 'PO-2024-001', DATEADD(MONTH, -2, GETDATE()), 'Purchase received', DATEADD(MONTH, -2, GETDATE()), 'admin', 1);

-- Stock Entry for Desktops
INSERT INTO StockEntries (ItemId, StoreId, Quantity, AvailableQuantity, ReservedQuantity, MinimumLevel, MaximumLevel, ReorderLevel, LastRestockDate, CreatedAt, CreatedBy, IsActive)
VALUES (@DesktopItemId, @CentralStoreId, 10, 10, 0, 3, 50, 5, DATEADD(MONTH, -2, GETDATE()), DATEADD(MONTH, -2, GETDATE()), 'admin', 1);

-- Purchase 2: Kit Bags
INSERT INTO Purchases (PurchaseOrderNo, OrderDate, StoreId, SupplierId, TotalAmount, Status, DeliveryDate, ReceivedDate, CreatedAt, CreatedBy, IsActive)
VALUES ('PO-2024-002', DATEADD(MONTH, -1, GETDATE()), @CentralStoreId, @Supplier2Id, 320000, 'Received', DATEADD(MONTH, -1, DATEADD(DAY, 5, GETDATE())), DATEADD(MONTH, -1, DATEADD(DAY, 5, GETDATE())), DATEADD(MONTH, -1, GETDATE()), 'admin', 1);

DECLARE @Purchase2Id INT = SCOPE_IDENTITY();

INSERT INTO PurchaseItems (PurchaseId, ItemId, Quantity, UnitPrice, TotalPrice, ReceivedQuantity, CreatedAt, CreatedBy, IsActive)
VALUES (@Purchase2Id, @KitBagItemId, 400, 800, 320000, 400, DATEADD(MONTH, -1, GETDATE()), 'admin', 1);

-- Stock Movement for Purchase 2
INSERT INTO StockMovements (ItemId, StoreId, MovementType, Quantity, ReferenceNo, MovementDate, Notes, CreatedAt, CreatedBy, IsActive)
VALUES (@KitBagItemId, @CentralStoreId, 'Purchase', 400, 'PO-2024-002', DATEADD(MONTH, -1, GETDATE()), 'Purchase received', DATEADD(MONTH, -1, GETDATE()), 'admin', 1);

-- Stock Entry for Kit Bags
INSERT INTO StockEntries (ItemId, StoreId, Quantity, AvailableQuantity, ReservedQuantity, MinimumLevel, MaximumLevel, ReorderLevel, LastRestockDate, CreatedAt, CreatedBy, IsActive)
VALUES (@KitBagItemId, @CentralStoreId, 400, 400, 0, 50, 2000, 100, DATEADD(MONTH, -1, GETDATE()), DATEADD(MONTH, -1, GETDATE()), 'admin', 1);

-- Purchase 3: Accessories
INSERT INTO Purchases (PurchaseOrderNo, OrderDate, StoreId, SupplierId, TotalAmount, Status, DeliveryDate, ReceivedDate, CreatedAt, CreatedBy, IsActive)
VALUES ('PO-2024-003', DATEADD(DAY, -20, GETDATE()), @CentralStoreId, @Supplier1Id, 75000, 'Received', DATEADD(DAY, -15, GETDATE()), DATEADD(DAY, -15, GETDATE()), DATEADD(DAY, -20, GETDATE()), 'admin', 1);

DECLARE @Purchase3Id INT = SCOPE_IDENTITY();

INSERT INTO PurchaseItems (PurchaseId, ItemId, Quantity, UnitPrice, TotalPrice, ReceivedQuantity, CreatedAt, CreatedBy, IsActive)
VALUES
(@Purchase3Id, @MouseItemId, 100, 200, 20000, 100, DATEADD(DAY, -20, GETDATE()), 'admin', 1),
(@Purchase3Id, @KeyboardItemId, 110, 500, 55000, 110, DATEADD(DAY, -20, GETDATE()), 'admin', 1);

-- Stock Movements for Purchase 3
INSERT INTO StockMovements (ItemId, StoreId, MovementType, Quantity, ReferenceNo, MovementDate, Notes, CreatedAt, CreatedBy, IsActive)
VALUES
(@MouseItemId, @CentralStoreId, 'Purchase', 100, 'PO-2024-003', DATEADD(DAY, -20, GETDATE()), 'Purchase received', DATEADD(DAY, -20, GETDATE()), 'admin', 1),
(@KeyboardItemId, @CentralStoreId, 'Purchase', 110, 'PO-2024-003', DATEADD(DAY, -20, GETDATE()), 'Purchase received', DATEADD(DAY, -20, GETDATE()), 'admin', 1);

-- Stock Entries for Accessories
INSERT INTO StockEntries (ItemId, StoreId, Quantity, AvailableQuantity, ReservedQuantity, MinimumLevel, MaximumLevel, ReorderLevel, LastRestockDate, CreatedAt, CreatedBy, IsActive)
VALUES
(@MouseItemId, @CentralStoreId, 100, 100, 0, 20, 500, 50, DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -20, GETDATE()), 'admin', 1),
(@KeyboardItemId, @CentralStoreId, 110, 110, 0, 10, 300, 30, DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -20, GETDATE()), 'admin', 1);

GO

-- ================================================
-- 6. ALLOTMENT LETTERS
-- ================================================

DECLARE @ProvisionStoreId INT = (SELECT Id FROM Stores WHERE Code = 'PROV-HQ');
DECLARE @KitBagId INT = (SELECT Id FROM Items WHERE ItemCode = 'BAG-KIT-001');
DECLARE @Battalion1Id INT = (SELECT Id FROM Battalions WHERE Code = '1BN');
DECLARE @Battalion2Id INT = (SELECT Id FROM Battalions WHERE Code = '2BN');
DECLARE @Battalion3Id INT = (SELECT Id FROM Battalions WHERE Code = '3BN');
DECLARE @Battalion4Id INT = (SELECT Id FROM Battalions WHERE Code = '4BN');

-- Allotment Letter
INSERT INTO AllotmentLetters (AllotmentNo, ReferenceNo, AllotmentDate, ValidFrom, ValidUntil, FromStoreId, IssuedTo, IssuedToType, Subject, SubjectBn, Purpose, Status, CollectionDeadline, BengaliDate, SignatoryName, SignatoryId, SignatoryDesignation, SignatoryDesignationBn, SignatoryEmail, SignatoryPhone, CreatedAt, CreatedBy, IsActive)
VALUES ('AL2511001', '44.03.0000.018.13.001.25.428', DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -10, GETDATE()), DATEADD(MONTH, 3, GETDATE()), @ProvisionStoreId, 'Multiple Battalions', 'Multiple', 'Kit Bag (New Design) Allotment', N'কিট ব্যাগ (নতুন নকশা) বরাদ্দ প্রদান প্রসঙ্গে', 'Annual allotment for battalions', 'Active', DATEADD(DAY, 20, GETDATE()), N'১০ ভাদ্র ১৪৩২ বঙ্গাব্দ', N'এ.বি.এম. ফরহাদ, বিবিএম', N'বিএভি-১২০২১৮', 'Deputy Director (Store)', N'উপপরিচালক (ভাণ্ডার)', 'ddstore@ansarvdp.gov.bd', '02-7213400,01730038013', DATEADD(DAY, -10, GETDATE()), 'admin', 1);

DECLARE @AllotmentId INT = SCOPE_IDENTITY();

-- Allotment Letter Items (main list)
INSERT INTO AllotmentLetterItems (AllotmentLetterId, ItemId, AllottedQuantity, IssuedQuantity, RemainingQuantity, Unit, CreatedAt, CreatedBy, IsActive)
VALUES (@AllotmentId, @KitBagId, 42, 0, 42, 'Piece', DATEADD(DAY, -10, GETDATE()), 'admin', 1);

-- Allotment Letter Recipients
INSERT INTO AllotmentLetterRecipients (AllotmentLetterId, RecipientType, BattalionId, RecipientName, RecipientNameBn, SerialNo, CreatedAt, CreatedBy, IsActive)
SELECT @AllotmentId, 'Battalion', Id, Name, NameBn, 1, DATEADD(DAY, -10, GETDATE()), 'admin', 1 FROM Battalions WHERE Code = '1BN'
UNION ALL
SELECT @AllotmentId, 'Battalion', Id, Name, NameBn, 2, DATEADD(DAY, -10, GETDATE()), 'admin', 1 FROM Battalions WHERE Code = '2BN'
UNION ALL
SELECT @AllotmentId, 'Battalion', Id, Name, NameBn, 3, DATEADD(DAY, -10, GETDATE()), 'admin', 1 FROM Battalions WHERE Code = '3BN'
UNION ALL
SELECT @AllotmentId, 'Battalion', Id, Name, NameBn, 4, DATEADD(DAY, -10, GETDATE()), 'admin', 1 FROM Battalions WHERE Code = '4BN';

-- Recipient Items
DECLARE @Recipient1Id INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentId AND SerialNo = 1);
DECLARE @Recipient2Id INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentId AND SerialNo = 2);
DECLARE @Recipient3Id INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentId AND SerialNo = 3);
DECLARE @Recipient4Id INT = (SELECT Id FROM AllotmentLetterRecipients WHERE AllotmentLetterId = @AllotmentId AND SerialNo = 4);

INSERT INTO AllotmentLetterRecipientItems (AllotmentLetterRecipientId, ItemId, AllottedQuantity, IssuedQuantity, Unit, UnitBn, CreatedAt, CreatedBy, IsActive)
VALUES
(@Recipient1Id, @KitBagId, 12, 0, 'Piece', N'টি', DATEADD(DAY, -10, GETDATE()), 'admin', 1),
(@Recipient2Id, @KitBagId, 10, 0, 'Piece', N'টি', DATEADD(DAY, -10, GETDATE()), 'admin', 1),
(@Recipient3Id, @KitBagId, 10, 0, 'Piece', N'টি', DATEADD(DAY, -10, GETDATE()), 'admin', 1),
(@Recipient4Id, @KitBagId, 10, 0, 'Piece', N'টি', DATEADD(DAY, -10, GETDATE()), 'admin', 1);

GO

PRINT 'Data seeding completed successfully!';
PRINT 'Total Ranges: ' + CAST((SELECT COUNT(*) FROM Ranges) AS VARCHAR);
PRINT 'Total Battalions: ' + CAST((SELECT COUNT(*) FROM Battalions) AS VARCHAR);
PRINT 'Total Stores: ' + CAST((SELECT COUNT(*) FROM Stores) AS VARCHAR);
PRINT 'Total Items: ' + CAST((SELECT COUNT(*) FROM Items) AS VARCHAR);
PRINT 'Total Purchases: ' + CAST((SELECT COUNT(*) FROM Purchases) AS VARCHAR);
PRINT 'Total Stock Entries: ' + CAST((SELECT COUNT(*) FROM StockEntries) AS VARCHAR);
PRINT 'Total Allotment Letters: ' + CAST((SELECT COUNT(*) FROM AllotmentLetters) AS VARCHAR);
