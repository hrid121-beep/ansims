-- ANSAR VDP IMS - Comprehensive Test Data Seed Script
-- This script populates all essential tables with test data for reports

USE ansvdp_ims;
GO

-- ============================================
-- 1. ORGANIZATIONAL STRUCTURE
-- ============================================

-- Insert Ranges
SET IDENTITY_INSERT Ranges ON;
INSERT INTO Ranges (Id, Name, Code, HeadquarterLocation, CommanderName, CommanderRank, ContactNumber, Email, CoverageArea, NameBn, CreatedAt, IsActive)
VALUES
(1, 'Dhaka Range', 'DR-01', 'Dhaka Cantonment', 'Brigadier General Md. Kamrul Hassan', 'Brigadier General', '01700-111111', 'dhaka.range@ansar.gov.bd', 'Dhaka Division', 'ঢাকা রেঞ্জ', GETDATE(), 1),
(2, 'Rajshahi Range', 'RR-01', 'Rajshahi Cantonment', 'Colonel Md. Habibur Rahman', 'Colonel', '01700-222222', 'rajshahi.range@ansar.gov.bd', 'Rajshahi Division', 'রাজশাহী রেঞ্জ', GETDATE(), 1),
(3, 'Chittagong Range', 'CR-01', 'Chittagong Cantonment', 'Colonel Md. Azizul Haque', 'Colonel', '01700-333333', 'chittagong.range@ansar.gov.bd', 'Chittagong Division', 'চট্টগ্রাম রেঞ্জ', GETDATE(), 1),
(4, 'Khulna Range', 'KR-01', 'Khulna Cantonment', 'Colonel Md. Abdul Mannan', 'Colonel', '01700-444444', 'khulna.range@ansar.gov.bd', 'Khulna Division', 'খুলনা রেঞ্জ', GETDATE(), 1),
(5, 'Sylhet Range', 'SR-01', 'Sylhet Cantonment', 'Colonel Md. Shamsul Alam', 'Colonel', '01700-555555', 'sylhet.range@ansar.gov.bd', 'Sylhet Division', 'সিলেট রেঞ্জ', GETDATE(), 1);
SET IDENTITY_INSERT Ranges OFF;

-- Insert Battalions (Type: 0=Regular, 1=Reserve, 2=Training)
SET IDENTITY_INSERT Battalions ON;
INSERT INTO Battalions (Id, Name, Code, Type, Location, CommanderName, CommanderRank, ContactNumber, Email, RangeId, TotalPersonnel, OfficerCount, EnlistedCount, NameBn, CreatedAt, IsActive)
VALUES
(1, '1st Ansar Battalion', 'AB-01', 0, 'Mirpur, Dhaka', 'Lt. Colonel Md. Jahangir Alam', 'Lt. Colonel', '01700-101001', 'ab01@ansar.gov.bd', 1, 500, 25, 475, '১ম আনসার ব্যাটালিয়ন', GETDATE(), 1),
(2, '2nd Ansar Battalion', 'AB-02', 0, 'Uttara, Dhaka', 'Lt. Colonel Md. Rafiqul Islam', 'Lt. Colonel', '01700-101002', 'ab02@ansar.gov.bd', 1, 500, 25, 475, '২য় আনসার ব্যাটালিয়ন', GETDATE(), 1),
(3, '3rd Ansar Battalion', 'AB-03', 0, 'Rajshahi', 'Major Md. Kamal Uddin', 'Major', '01700-102001', 'ab03@ansar.gov.bd', 2, 450, 20, 430, '৩য় আনসার ব্যাটালিয়ন', GETDATE(), 1),
(4, '4th Ansar Battalion', 'AB-04', 0, 'Chittagong', 'Major Md. Selim Reza', 'Major', '01700-103001', 'ab04@ansar.gov.bd', 3, 450, 20, 430, '৪র্থ আনসার ব্যাটালিয়ন', GETDATE(), 1),
(5, '5th Ansar Battalion', 'AB-05', 1, 'Khulna', 'Major Md. Nurul Islam', 'Major', '01700-104001', 'ab05@ansar.gov.bd', 4, 400, 18, 382, '৫ম আনসার ব্যাটালিয়ন', GETDATE(), 1),
(6, '6th Ansar Battalion', 'AB-06', 0, 'Sylhet', 'Major Md. Monir Hossain', 'Major', '01700-105001', 'ab06@ansar.gov.bd', 5, 400, 18, 382, '৬ষ্ঠ আনসার ব্যাটালিয়ন', GETDATE(), 1);
SET IDENTITY_INSERT Battalions OFF;

-- Insert Zilas
SET IDENTITY_INSERT Zilas ON;
INSERT INTO Zilas (Id, Name, Code, HeadquarterLocation, CommanderName, CommanderRank, ContactNumber, Email, NameBn, CreatedAt, IsActive)
VALUES
(1, 'Dhaka Zila', 'DZ-01', 'Dhaka', 'Captain Md. Aminul Islam', 'Captain', '01700-201001', 'dhaka.zila@ansar.gov.bd', 'ঢাকা জেলা', GETDATE(), 1),
(2, 'Gazipur Zila', 'GZ-01', 'Gazipur', 'Captain Md. Shafiqul Islam', 'Captain', '01700-201002', 'gazipur.zila@ansar.gov.bd', 'গাজীপুর জেলা', GETDATE(), 1),
(3, 'Narayanganj Zila', 'NZ-01', 'Narayanganj', 'Lieutenant Md. Kamrul Hasan', 'Lieutenant', '01700-201003', 'narayanganj.zila@ansar.gov.bd', 'নারায়ণগঞ্জ জেলা', GETDATE(), 1),
(4, 'Rajshahi Zila', 'RZ-01', 'Rajshahi', 'Captain Md. Abdul Quddus', 'Captain', '01700-202001', 'rajshahi.zila@ansar.gov.bd', 'রাজশাহী জেলা', GETDATE(), 1),
(5, 'Chittagong Zila', 'CZ-01', 'Chittagong', 'Captain Md. Shahidul Islam', 'Captain', '01700-203001', 'chittagong.zila@ansar.gov.bd', 'চট্টগ্রাম জেলা', GETDATE(), 1),
(6, 'Khulna Zila', 'KZ-01', 'Khulna', 'Lieutenant Md. Iqbal Hossain', 'Lieutenant', '01700-204001', 'khulna.zila@ansar.gov.bd', 'খুলনা জেলা', GETDATE(), 1),
(7, 'Sylhet Zila', 'SZ-01', 'Sylhet', 'Lieutenant Md. Monirul Islam', 'Lieutenant', '01700-205001', 'sylhet.zila@ansar.gov.bd', 'সিলেট জেলা', GETDATE(), 1);
SET IDENTITY_INSERT Zilas OFF;

-- Insert Upazilas
SET IDENTITY_INSERT Upazilas ON;
INSERT INTO Upazilas (Id, Name, Code, ZilaId, HeadquarterLocation, ContactNumber, Email, NameBn, CreatedAt, IsActive)
VALUES
(1, 'Mirpur Upazila', 'MU-01', 1, 'Mirpur-10', '01700-301001', 'mirpur.upazila@ansar.gov.bd', 'মিরপুর উপজেলা', GETDATE(), 1),
(2, 'Uttara Upazila', 'UU-01', 1, 'Uttara Sector-7', '01700-301002', 'uttara.upazila@ansar.gov.bd', 'উত্তরা উপজেলা', GETDATE(), 1),
(3, 'Savar Upazila', 'SU-01', 1, 'Savar Bazar', '01700-301003', 'savar.upazila@ansar.gov.bd', 'সাভার উপজেলা', GETDATE(), 1),
(4, 'Rajshahi Sadar', 'RS-01', 4, 'Rajshahi Sadar', '01700-302001', 'rajshahi.sadar@ansar.gov.bd', 'রাজশাহী সদর', GETDATE(), 1);
SET IDENTITY_INSERT Upazilas OFF;

-- ============================================
-- 2. STORES
-- ============================================

SET IDENTITY_INSERT Stores ON;
INSERT INTO Stores (Id, Name, NameBn, Code, Type, Status, Location, Address, Description, InCharge, ContactNumber, Email, StoreKeeperName, StoreKeeperId, StoreKeeperAssignedDate, Level, StoreTypeId, BattalionId, RangeId, ZilaId, RequiresTemperatureControl, IsStockFrozen, StockFrozenAt, CreatedAt, IsActive)
VALUES
-- Central Stores
(1, 'Central Store - Headquarters', 'কেন্দ্রীয় ভাণ্ডার - সদর দপ্তর', 'CS-HQ-01', 'CENTRAL', 'Active', 'Headquarters', 'Ansar & VDP HQ, Dhaka-1000', 'Main Central Store for all incoming purchases', 'Md. Altaf Hossain', '01700-501001', 'central.hq@ansar.gov.bd', 'Abdul Karim', 'SK001', GETDATE(), 1, 1, NULL, NULL, NULL, 0, 0, GETDATE(), GETDATE(), 1),

-- Provision Stores
(2, 'Provision Store - Dhaka', 'প্রভিশন ভাণ্ডার - ঢাকা', 'PS-DH-01', 'PROVISION', 'Active', 'Dhaka Range', 'Dhaka Range Office, Mirpur-12', 'Provision Store for Dhaka Range', 'Md. Shahjahan Ali', '01700-502001', 'provision.dhaka@ansar.gov.bd', 'Fazlul Haque', 'SK002', GETDATE(), 2, 2, NULL, 1, NULL, 0, 0, GETDATE(), GETDATE(), 1),
(3, 'Provision Store - Rajshahi', 'প্রভিশন ভাণ্ডার - রাজশাহী', 'PS-RJ-01', 'PROVISION', 'Active', 'Rajshahi Range', 'Rajshahi Range Office', 'Provision Store for Rajshahi Range', 'Md. Abdur Rahman', '01700-502002', 'provision.rajshahi@ansar.gov.bd', 'Hafizur Rahman', 'SK003', GETDATE(), 2, 2, NULL, 2, NULL, 0, 0, GETDATE(), GETDATE(), 1),

-- Battalion Stores
(4, 'Battalion Store - 1st Battalion', 'ব্যাটালিয়ন ভাণ্ডার - ১ম ব্যাটালিয়ন', 'BS-AB01', 'OTHER', 'Active', 'Mirpur', '1st Ansar Battalion, Mirpur', 'Store for 1st Battalion', 'Lt. Farooq Ahmed', '01700-503001', 'store.ab01@ansar.gov.bd', 'Azizul Islam', 'SK004', GETDATE(), 3, 3, 1, NULL, NULL, 0, 0, GETDATE(), GETDATE(), 1),
(5, 'Battalion Store - 2nd Battalion', 'ব্যাটালিয়ন ভাণ্ডার - ২য় ব্যাটালিয়ন', 'BS-AB02', 'OTHER', 'Active', 'Uttara', '2nd Ansar Battalion, Uttara', 'Store for 2nd Battalion', 'Lt. Mizanur Rahman', '01700-503002', 'store.ab02@ansar.gov.bd', 'Delwar Hossain', 'SK005', GETDATE(), 3, 3, 2, NULL, NULL, 0, 0, GETDATE(), GETDATE(), 1),

-- Zila Stores
(6, 'Zila Store - Dhaka', 'জেলা ভাণ্ডার - ঢাকা', 'ZS-DH-01', 'OTHER', 'Active', 'Dhaka', 'Dhaka Zila Office', 'Store for Dhaka Zila', 'Warrant Officer Sirajul Islam', '01700-504001', 'store.dhaka.zila@ansar.gov.bd', 'Mostafa Kamal', 'SK006', GETDATE(), 4, 3, NULL, NULL, 1, 0, 0, GETDATE(), GETDATE(), 1);
SET IDENTITY_INSERT Stores OFF;

-- ============================================
-- 3. CATEGORIES AND ITEMS
-- ============================================

-- Insert Categories
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (Id, Name, NameBn, Code, Description, CreatedAt, IsActive)
VALUES
(1, 'IT Equipment', 'আইটি যন্ত্রপাতি', 'CAT-IT', 'Information Technology Equipment', GETDATE(), 1),
(2, 'Office Equipment', 'অফিস যন্ত্রপাতি', 'CAT-OE', 'General Office Equipment', GETDATE(), 1),
(3, 'Uniforms & Clothing', 'ইউনিফর্ম ও পোশাক', 'CAT-UC', 'Uniforms, Clothing and Accessories', GETDATE(), 1),
(4, 'Stationary', 'স্টেশনারি', 'CAT-ST', 'Office Stationary Items', GETDATE(), 1),
(5, 'Safety Equipment', 'নিরাপত্তা সরঞ্জাম', 'CAT-SE', 'Safety and Security Equipment', GETDATE(), 1);
SET IDENTITY_INSERT Categories OFF;

-- Insert Items (Type: 0=Consumable, 1=NonConsumable, 2=Service; Status: 0=Active, 1=Inactive)
SET IDENTITY_INSERT Items ON;
INSERT INTO Items (Id, Name, NameBn, ItemCode, Unit, Type, Status, SubCategoryId, CategoryId, MinimumStock, MaximumStock, ReorderLevel, UnitPrice, HasExpiry, RequiresSpecialHandling, IsHazardous, RequiresPersonalIssue, IsAnsarAuthorized, IsVDPAuthorized, CreatedAt, IsActive)
VALUES
(1, 'Desktop Computer Core i5', 'ডেস্কটপ কম্পিউটার কোর আই৫', 'IT-DESK-001', 'Piece', 1, 0, 1, 1, 5, 50, 10, 45000.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1),
(2, 'Laser Printer A4', 'লেজার প্রিন্টার এ৪', 'IT-PRNT-001', 'Piece', 1, 0, 1, 1, 3, 30, 5, 18000.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1),
(3, 'Scanner Flatbed', 'স্ক্যানার ফ্ল্যাটবেড', 'IT-SCAN-001', 'Piece', 1, 0, 1, 1, 2, 20, 5, 12000.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1),
(4, 'UPS 1000VA', 'ইউপিএস ১০০০ ভিএ', 'IT-UPS-001', 'Piece', 1, 0, 1, 1, 5, 40, 10, 8500.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1),
(5, 'Wireless Mouse', 'ওয়্যারলেস মাউস', 'IT-MOUS-001', 'Piece', 1, 0, 1, 1, 20, 200, 30, 450.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1),
(6, 'USB Keyboard', 'ইউএসবি কীবোর্ড', 'IT-KEYB-001', 'Piece', 1, 0, 1, 1, 20, 200, 30, 850.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1),
(7, 'Ansar Kit Bag', 'আনসার কিট ব্যাগ', 'UC-BAG-001', 'Piece', 1, 0, 3, 3, 50, 500, 100, 1200.00, 0, 0, 0, 1, 1, 1, GETDATE(), 1),
(8, 'Office File Folder', 'অফিস ফাইল ফোল্ডার', 'ST-FILE-001', 'Piece', 0, 0, 4, 4, 100, 1000, 150, 25.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1),
(9, 'A4 Paper 80GSM (Ream)', 'এ৪ কাগজ ৮০জিএসএম (রিম)', 'ST-PAPR-001', 'Ream', 0, 0, 4, 4, 200, 2000, 300, 450.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1),
(10, 'Ballpoint Pen Blue', 'বলপয়েন্ট কলম নীল', 'ST-PEN-001', 'Piece', 0, 0, 4, 4, 500, 5000, 750, 5.00, 0, 0, 0, 0, 1, 1, GETDATE(), 1);
SET IDENTITY_INSERT Items OFF;

-- ============================================
-- 4. VENDORS
-- ============================================

SET IDENTITY_INSERT Vendors ON;
INSERT INTO Vendors (Id, Name, VendorType, ContactPerson, Phone, Email, Mobile, Address, TIN, BIN, CreatedAt, IsActive)
VALUES
(1, 'Computer Point Ltd.', 'IT Equipment Supplier', 'Md. Shahabuddin Ahmed', '02-9876543', 'info@computerpoint.com.bd', '01711-123456', '45/2 Eskaton Garden, Dhaka-1000', '123456789012', '000123456789', GETDATE(), 1),
(2, 'Office Solutions BD', 'Office Equipment Supplier', 'Mrs. Fatema Begum', '02-8765432', 'sales@officesolutions.com.bd', '01712-234567', '78 Motijheel C/A, Dhaka-1000', '234567890123', '000234567890', GETDATE(), 1),
(3, 'Uniform Industries Ltd.', 'Uniform Manufacturer', 'Md. Kamal Uddin', '02-7654321', 'contact@uniformindustries.com', '01713-345678', '123 Tejgaon I/A, Dhaka-1208', '345678901234', '000345678901', GETDATE(), 1);
SET IDENTITY_INSERT Vendors OFF;

-- ============================================
-- 5. PURCHASES
-- ============================================

SET IDENTITY_INSERT Purchases ON;
INSERT INTO Purchases (Id, PurchaseOrderNo, PurchaseDate, ExpectedDeliveryDate, VendorId, StoreId, TotalAmount, Discount, PurchaseType, Status, Remarks, ProcurementType, CreatedAt, IsActive)
VALUES
(1, 'PO-2024-001', DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -15, GETDATE()), 1, 1, 2280000.00, 0.00, 'Direct', 'Received', 'IT Equipment for HQ', 1, DATEADD(DAY, -30, GETDATE()), 1),
(2, 'PO-2024-002', DATEADD(DAY, -25, GETDATE()), DATEADD(DAY, -10, GETDATE()), 2, 1, 895000.00, 10000.00, 'Direct', 'Received', 'Office Equipment', 1, DATEADD(DAY, -25, GETDATE()), 1),
(3, 'PO-2024-003', DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -5, GETDATE()), 3, 1, 600000.00, 0.00, 'Direct', 'Received', 'Uniforms and Kit Bags', 1, DATEADD(DAY, -20, GETDATE()), 1);
SET IDENTITY_INSERT Purchases OFF;

-- Insert Purchase Items
SET IDENTITY_INSERT PurchaseItems ON;
INSERT INTO PurchaseItems (Id, PurchaseId, ItemId, Quantity, UnitPrice, TotalPrice, ReceivedQuantity, AcceptedQuantity, CreatedAt, IsActive)
VALUES
-- Purchase 1 (IT Equipment)
(1, 1, 1, 20, 45000.00, 900000.00, 20, 20, DATEADD(DAY, -30, GETDATE()), 1),
(2, 1, 2, 15, 18000.00, 270000.00, 15, 15, DATEADD(DAY, -30, GETDATE()), 1),
(3, 1, 3, 10, 12000.00, 120000.00, 10, 10, DATEADD(DAY, -30, GETDATE()), 1),
(4, 1, 4, 25, 8500.00, 212500.00, 25, 25, DATEADD(DAY, -30, GETDATE()), 1),
(5, 1, 5, 100, 450.00, 45000.00, 100, 100, DATEADD(DAY, -30, GETDATE()), 1),
(6, 1, 6, 100, 850.00, 85000.00, 100, 100, DATEADD(DAY, -30, GETDATE()), 1),

-- Purchase 2 (Office Equipment & Stationary)
(7, 2, 8, 500, 25.00, 12500.00, 500, 500, DATEADD(DAY, -25, GETDATE()), 1),
(8, 2, 9, 1000, 450.00, 450000.00, 1000, 1000, DATEADD(DAY, -25, GETDATE()), 1),
(9, 2, 10, 5000, 5.00, 25000.00, 5000, 5000, DATEADD(DAY, -25, GETDATE()), 1),

-- Purchase 3 (Uniforms)
(10, 3, 7, 500, 1200.00, 600000.00, 500, 500, DATEADD(DAY, -20, GETDATE()), 1);
SET IDENTITY_INSERT PurchaseItems OFF;

-- ============================================
-- 6. STOCK ENTRIES
-- ============================================

SET IDENTITY_INSERT StockEntries ON;
INSERT INTO StockEntries (Id, EntryNo, EntryDate, EntryType, Status, Remarks, StoreId, CreatedAt, IsActive)
VALUES
(1, 'SE-2024-001', DATEADD(DAY, -28, GETDATE()), 'Purchase', 'Completed', 'Stock entry from Purchase PO-2024-001', 1, DATEADD(DAY, -28, GETDATE()), 1),
(2, 'SE-2024-002', DATEADD(DAY, -23, GETDATE()), 'Purchase', 'Completed', 'Stock entry from Purchase PO-2024-002', 1, DATEADD(DAY, -23, GETDATE()), 1),
(3, 'SE-2024-003', DATEADD(DAY, -18, GETDATE()), 'Purchase', 'Completed', 'Stock entry from Purchase PO-2024-003', 1, DATEADD(DAY, -18, GETDATE()), 1);
SET IDENTITY_INSERT StockEntries OFF;

-- Insert Stock Entry Items
SET IDENTITY_INSERT StockEntryItems ON;
INSERT INTO StockEntryItems (Id, StockEntryId, ItemId, Quantity, UnitCost, BatchNumber, BarcodesGenerated, CreatedAt, IsActive)
VALUES
-- Stock Entry 1 (IT Equipment)
(1, 1, 1, 20, 45000.00, 'BATCH-IT-001-2024', 0, DATEADD(DAY, -28, GETDATE()), 1),
(2, 1, 2, 15, 18000.00, 'BATCH-IT-002-2024', 0, DATEADD(DAY, -28, GETDATE()), 1),
(3, 1, 3, 10, 12000.00, 'BATCH-IT-003-2024', 0, DATEADD(DAY, -28, GETDATE()), 1),
(4, 1, 4, 25, 8500.00, 'BATCH-IT-004-2024', 0, DATEADD(DAY, -28, GETDATE()), 1),
(5, 1, 5, 100, 450.00, 'BATCH-IT-005-2024', 0, DATEADD(DAY, -28, GETDATE()), 1),
(6, 1, 6, 100, 850.00, 'BATCH-IT-006-2024', 0, DATEADD(DAY, -28, GETDATE()), 1),

-- Stock Entry 2 (Office & Stationary)
(7, 2, 8, 500, 25.00, 'BATCH-ST-001-2024', 0, DATEADD(DAY, -23, GETDATE()), 1),
(8, 2, 9, 1000, 450.00, 'BATCH-ST-002-2024', 0, DATEADD(DAY, -23, GETDATE()), 1),
(9, 2, 10, 5000, 5.00, 'BATCH-ST-003-2024', 0, DATEADD(DAY, -23, GETDATE()), 1),

-- Stock Entry 3 (Uniforms)
(10, 3, 7, 500, 1200.00, 'BATCH-UC-001-2024', 0, DATEADD(DAY, -18, GETDATE()), 1);
SET IDENTITY_INSERT StockEntryItems OFF;

-- ============================================
-- 7. STORE ITEMS (Current Stock)
-- ============================================

SET IDENTITY_INSERT StoreItems ON;
INSERT INTO StoreItems (Id, ItemId, StoreId, CurrentStock, MinimumStock, MaximumStock, ReorderLevel, ReservedQuantity, Status, LastUpdated, LastStockUpdate, LastCountDate, LastIssueDate, LastReceiveDate, LastTransferDate, LastAdjustmentDate, LastCountQuantity, CreatedAt, IsActive)
VALUES
-- Central Store inventory
(1, 1, 1, 20, 5, 50, 10, 0, 0, DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), GETDATE(), DATEADD(DAY, -28, GETDATE()), GETDATE(), GETDATE(), 20, DATEADD(DAY, -28, GETDATE()), 1),
(2, 2, 1, 15, 3, 30, 5, 0, 0, DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), GETDATE(), DATEADD(DAY, -28, GETDATE()), GETDATE(), GETDATE(), 15, DATEADD(DAY, -28, GETDATE()), 1),
(3, 3, 1, 10, 2, 20, 5, 0, 0, DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), GETDATE(), DATEADD(DAY, -28, GETDATE()), GETDATE(), GETDATE(), 10, DATEADD(DAY, -28, GETDATE()), 1),
(4, 4, 1, 25, 5, 40, 10, 0, 0, DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), GETDATE(), DATEADD(DAY, -28, GETDATE()), GETDATE(), GETDATE(), 25, DATEADD(DAY, -28, GETDATE()), 1),
(5, 5, 1, 100, 20, 200, 30, 0, 0, DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), GETDATE(), DATEADD(DAY, -28, GETDATE()), GETDATE(), GETDATE(), 100, DATEADD(DAY, -28, GETDATE()), 1),
(6, 6, 1, 100, 20, 200, 30, 0, 0, DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -28, GETDATE()), GETDATE(), DATEADD(DAY, -28, GETDATE()), GETDATE(), GETDATE(), 100, DATEADD(DAY, -28, GETDATE()), 1),
(7, 7, 1, 500, 50, 500, 100, 0, 0, DATEADD(DAY, -18, GETDATE()), DATEADD(DAY, -18, GETDATE()), DATEADD(DAY, -18, GETDATE()), GETDATE(), DATEADD(DAY, -18, GETDATE()), GETDATE(), GETDATE(), 500, DATEADD(DAY, -18, GETDATE()), 1),
(8, 8, 1, 500, 100, 1000, 150, 0, 0, DATEADD(DAY, -23, GETDATE()), DATEADD(DAY, -23, GETDATE()), DATEADD(DAY, -23, GETDATE()), GETDATE(), DATEADD(DAY, -23, GETDATE()), GETDATE(), GETDATE(), 500, DATEADD(DAY, -23, GETDATE()), 1),
(9, 9, 1, 1000, 200, 2000, 300, 0, 0, DATEADD(DAY, -23, GETDATE()), DATEADD(DAY, -23, GETDATE()), DATEADD(DAY, -23, GETDATE()), GETDATE(), DATEADD(DAY, -23, GETDATE()), GETDATE(), GETDATE(), 1000, DATEADD(DAY, -23, GETDATE()), 1),
(10, 10, 1, 5000, 500, 5000, 750, 0, 0, DATEADD(DAY, -23, GETDATE()), DATEADD(DAY, -23, GETDATE()), DATEADD(DAY, -23, GETDATE()), GETDATE(), DATEADD(DAY, -23, GETDATE()), GETDATE(), GETDATE(), 5000, DATEADD(DAY, -23, GETDATE()), 1);
SET IDENTITY_INSERT StoreItems OFF;

-- ============================================
-- 8. ALLOTMENT LETTERS
-- ============================================

SET IDENTITY_INSERT AllotmentLetters ON;
INSERT INTO AllotmentLetters (Id, AllotmentNo, AllotmentDate, ValidFrom, ValidUntil, IssuedTo, IssuedToType, IssuedToBattalionId, IssuedToRangeId, FromStoreId, Purpose, Status, ReferenceNo, Subject, SubjectBn, BodyText, BodyTextBn, SignatoryName, SignatoryDesignation, SignatoryDesignationBn, CreatedAt, IsActive)
VALUES
(1, 'AL-2024-001', DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, 20, GETDATE()), '1st Ansar Battalion', 'Battalion', 1, NULL, 2, 'IT Equipment allotment for battalion office', 'Approved', 'HQ/Store/2024/001', 'Allotment of IT Equipment', 'আইটি যন্ত্রপাতি বরাদ্দ', 'This allotment is for IT equipment for 1st Ansar Battalion office modernization', 'এই বরাদ্দটি ১ম আনসার ব্যাটালিয়নের অফিস আধুনিকায়নের জন্য আইটি যন্ত্রপাতির জন্য', 'Brigadier General Md. Kamrul Hassan', 'Director General', 'মহাপরিচালক', DATEADD(DAY, -10, GETDATE()), 1),
(2, 'AL-2024-002', DATEADD(DAY, -8, GETDATE()), DATEADD(DAY, -8, GETDATE()), DATEADD(DAY, 22, GETDATE()), '2nd Ansar Battalion', 'Battalion', 2, NULL, 2, 'Office stationary and equipment', 'Approved', 'HQ/Store/2024/002', 'Allotment of Office Supplies', 'অফিস সরবরাহ বরাদ্দ', 'Office stationary and equipment for 2nd Battalion', '২য় ব্যাটালিয়নের জন্য অফিস স্টেশনারি এবং যন্ত্রপাতি', 'Brigadier General Md. Kamrul Hassan', 'Director General', 'মহাপরিচালক', DATEADD(DAY, -8, GETDATE()), 1);
SET IDENTITY_INSERT AllotmentLetters OFF;

-- Insert Allotment Letter Items
SET IDENTITY_INSERT AllotmentLetterItems ON;
INSERT INTO AllotmentLetterItems (Id, AllotmentLetterId, ItemId, AllottedQuantity, IssuedQuantity, Status, CreatedAt, IsActive)
VALUES
-- Allotment Letter 1 (IT Equipment for Battalion 1)
(1, 1, 1, 5, 0, 'Pending', DATEADD(DAY, -10, GETDATE()), 1),
(2, 1, 2, 3, 0, 'Pending', DATEADD(DAY, -10, GETDATE()), 1),
(3, 1, 5, 10, 0, 'Pending', DATEADD(DAY, -10, GETDATE()), 1),
(4, 1, 6, 10, 0, 'Pending', DATEADD(DAY, -10, GETDATE()), 1),

-- Allotment Letter 2 (Office Supplies for Battalion 2)
(5, 2, 8, 100, 0, 'Pending', DATEADD(DAY, -8, GETDATE()), 1),
(6, 2, 9, 200, 0, 'Pending', DATEADD(DAY, -8, GETDATE()), 1),
(7, 2, 10, 500, 0, 'Pending', DATEADD(DAY, -8, GETDATE()), 1);
SET IDENTITY_INSERT AllotmentLetterItems OFF;

PRINT 'Seed data inserted successfully!';
PRINT '';
PRINT 'Summary:';
PRINT '- Ranges: 5';
PRINT '- Battalions: 6';
PRINT '- Zilas: 7';
PRINT '- Upazilas: 4';
PRINT '- Stores: 6 (1 Central, 2 Provision, 2 Battalion, 1 Zila)';
PRINT '- Categories: 5';
PRINT '- Items: 10';
PRINT '- Vendors: 3';
PRINT '- Purchases: 3 with 10 items';
PRINT '- Stock Entries: 3 with 10 items';
PRINT '- Store Items: 10 (current stock)';
PRINT '- Allotment Letters: 2 with 7 items';
GO
