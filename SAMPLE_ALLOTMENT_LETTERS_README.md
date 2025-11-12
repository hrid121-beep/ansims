# Sample Allotment Letters - Setup Guide

## 📋 Overview

আমি 2টা complete sample allotment letter এর SQL script তৈরি করেছি যেটা run করলে proper government format এ PDF generate হবে।

## 📁 File Location

**SQL File**: `E:\Github Projects\zzzSir\ANSAR VDP\IMS\SAMPLE_ALLOTMENT_LETTERS.sql`

## 📝 What's Included

### Allotment Letter 1: **AL-202511-001**
- **Type**: Uniforms & Equipment Distribution
- **Recipients**: 5টা (Battalions and Ranges)
  - 1st Battalion, Dhaka (150 জন)
  - 2nd Battalion, Chittagong (120 জন)
  - Dhaka Range (80 জন)
  - 5th Battalion, Sylhet (100 জন)
  - Chittagong Range (75 জন)
- **Items per Recipient**: Uniforms (Shirt, Pant, Boots, Cap)
- **Status**: Active (Approved)
- **Validity**: 6 months

### Allotment Letter 2: **AL-202511-002**
- **Type**: Office Equipment & Stationery
- **Recipients**: 3টা (Zila Offices)
  - Dhaka Zila Office (50 জন)
  - Chittagong Zila Office (45 জন)
  - Sylhet Zila Office (35 জন)
- **Items per Recipient**: Tables, Chairs, A4 Paper, File Covers
- **Status**: Active (Approved)
- **Validity**: 3 months

## ⚙️ Prerequisites - আগে করতে হবে

### 1. **Store তৈরি করুন** (যদি না থাকে)
```sql
-- Check existing stores
SELECT Id, Name, Type FROM Stores;

-- If needed, create a Central Store
INSERT INTO Stores (Name, Type, Location, CreatedAt, CreatedBy, IsActive)
VALUES ('Central Store', 'CENTRAL', 'Headquarters, Dhaka', GETDATE(), 'admin', 1);
```

### 2. **Items তৈরি করুন** (Sample data)
```sql
-- Check existing items
SELECT Id, Code, Name FROM Items;

-- Create sample items if needed
INSERT INTO Items (Code, Name, NameBn, Category, Unit, UnitBn, MinStockLevel, CreatedAt, CreatedBy, IsActive)
VALUES
    ('UNI-001', 'Uniform (Shirt)', 'ইউনিফর্ম (শার্ট)', 'Uniform', 'Pcs', 'টি', 100, GETDATE(), 'admin', 1),
    ('UNI-002', 'Uniform (Pant)', 'ইউনিফর্ম (প্যান্ট)', 'Uniform', 'Pcs', 'টি', 100, GETDATE(), 'admin', 1),
    ('UNI-003', 'Boots', 'বুট', 'Uniform', 'Pcs', 'টি', 50, GETDATE(), 'admin', 1),
    ('UNI-004', 'Cap', 'টুপি', 'Uniform', 'Pcs', 'টি', 50, GETDATE(), 'admin', 1),
    ('OFF-001', 'Computer Table', 'কম্পিউটার টেবিল', 'Furniture', 'Pcs', 'টি', 10, GETDATE(), 'admin', 1),
    ('OFF-002', 'Chair', 'চেয়ার', 'Furniture', 'Pcs', 'টি', 20, GETDATE(), 'admin', 1),
    ('STA-001', 'A4 Paper', 'এ৪ পেপার', 'Stationery', 'Rim', 'রিম', 50, GETDATE(), 'admin', 1),
    ('STA-002', 'File Cover', 'ফাইল কভার', 'Stationery', 'Pcs', 'টি', 100, GETDATE(), 'admin', 1);
```

## 🔧 How to Use

### Step 1: Update IDs in SQL Script

SQL script এ এই variables গুলো update করুন:

```sql
DECLARE @CentralStoreId INT = 1; -- Your actual Store Id
DECLARE @CurrentUser NVARCHAR(100) = 'admin'; -- Your username
```

### Step 2: Update Item IDs

Script এ ItemId values (1-8) আছে যেটা আপনার database এর actual ItemIds দিয়ে replace করতে হবে:

```sql
-- Example: Current in script
(@Recipient1_1, 1, 150, 0, 'Pcs', 'টি', 'ইউনিফর্ম (শার্ট)', @Now, @CurrentUser, 1),

-- Update to your actual ItemId:
(@Recipient1_1, 15, 150, 0, 'Pcs', 'টি', 'ইউনিফর্ম (শার্ট)', @Now, @CurrentUser, 1),
                 ^^
```

### Step 3: Run SQL Script

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your database: `ansvdp_ims`
3. Open the file: `SAMPLE_ALLOTMENT_LETTERS.sql`
4. Execute (F5)

### Step 4: View Results

```
Navigate to: https://localhost:7029/AllotmentLetter
```

আপনি 2টা new allotment letters দেখতে পাবেন:
- **AL-202511-001**
- **AL-202511-002**

## 📄 PDF Format যা দেখবেন

### Government Header:
```
গণপ্রজাতন্ত্রী বাংলাদেশ সরকার
বাংলাদেশ আনসার ও ভিডিপি
আনসার-ভিডিপি মহাপরিচালকের কার্যালয়
```

### Memo Section:
```
স্মারক নং: ৪৪.০৩.০০০০.০১৮.১৩.০০১.২৪-১৫০
তারিখ: ১০ নভেম্বর ২০২৫
```

### Subject:
```
বিষয়: বিভিন্ন ব্যাটালিয়ন ও রেঞ্জে পোশাক ও সরঞ্জাম বরাদ্দ প্রদান প্রসঙ্গে
```

### Body (Full Bengali text)

### Recipient Table (ক্রোড়পত্র-'ক'):
| ক্রমিক নং | প্রাপকের নাম | ইউনিট/দপ্তর | উপকরণের বিবরণ | বরাদ্দ পরিমাণ |
|----------|-------------|------------|--------------|-------------|
| ১ | ১ম ব্যাটালিয়ন, ঢাকা | Battalion | Items... | ১৫০ |
| ... | ... | ... | ... | ... |

### Distribution List (অনুলিপি):
```
১. মহাপরিচালক, আনসার ও ভিডিপি - সদয় অবগতির জন্য
২. উপ-মহাপরিচালক (স্টোর) - প্রয়োজনীয় ব্যবস্থা গ্রহণের জন্য
...
```

### Signature:
```
মোঃ আব্দুল হামিদ
উপ-মহাপরিচালক (প্রশাসন)
ফোন: ০২-৯৫৫১৪৮৪
```

## 🎯 Features Demonstrated

✅ **Government Letterhead** - Full Bengali header
✅ **Reference Number** (স্মারক নং) - Bengali formatted
✅ **Subject Line** (বিষয়) - Bengali & English
✅ **Body Text** - Complete Bengali paragraphs
✅ **Multiple Recipients** - 5 recipients in Letter 1, 3 in Letter 2
✅ **Items per Recipient** - Different items for each
✅ **Distribution List** (অনুলিপি) - 3-4 entries
✅ **Signature Block** - Complete with designation & contact
✅ **Bengali Dates** - Auto-converted
✅ **Bengali Numbers** - All quantities in Bengali digits
✅ **Staff Strength** (কর্মরত জনবল) - Shown for each recipient

## 🐛 Troubleshooting

### Error: "Invalid object name 'AllotmentLetters'"
**Solution**: Table name might be different. Check with:
```sql
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '%Allotment%';
```

### Error: "Foreign key constraint violation"
**Solution**: Update `@CentralStoreId` with a valid Store ID:
```sql
SELECT Id, Name FROM Stores;
```

### Items not showing in PDF
**Solution**: Update ItemId values in the script with actual Item IDs from your database:
```sql
SELECT Id, Code, Name FROM Items;
```

### Recipients showing as "-"
**Solution**: Make sure RecipientName and RecipientNameBn are populated in the INSERT statements.

## 📊 Verification Queries

### Check created letters:
```sql
SELECT AllotmentNo, Status, AllotmentDate,
       (SELECT COUNT(*) FROM AllotmentLetterRecipients WHERE AllotmentLetterId = AL.Id) AS Recipients
FROM AllotmentLetters AL
WHERE AllotmentNo IN ('AL-202511-001', 'AL-202511-002');
```

### Check recipients:
```sql
SELECT AL.AllotmentNo, ALR.SerialNo, ALR.RecipientNameBn, ALR.StaffStrength
FROM AllotmentLetterRecipients ALR
JOIN AllotmentLetters AL ON AL.Id = ALR.AllotmentLetterId
WHERE AL.AllotmentNo IN ('AL-202511-001', 'AL-202511-002')
ORDER BY AL.AllotmentNo, ALR.SerialNo;
```

### Check items per recipient:
```sql
SELECT AL.AllotmentNo, ALR.RecipientNameBn, ALRI.ItemNameBn, ALRI.AllottedQuantity, ALRI.UnitBn
FROM AllotmentLetterRecipientItems ALRI
JOIN AllotmentLetterRecipients ALR ON ALR.Id = ALRI.AllotmentLetterRecipientId
JOIN AllotmentLetters AL ON AL.Id = ALR.AllotmentLetterId
WHERE AL.AllotmentNo IN ('AL-202511-001', 'AL-202511-002')
ORDER BY AL.AllotmentNo, ALR.SerialNo;
```

## 🎉 Success!

Script run হওয়ার পর আপনি:
1. https://localhost:7029/AllotmentLetter এ যান
2. 2টা new entries দেখবেন
3. "Details" button এ click করুন
4. Full government format letter দেখবেন
5. "Print This Page" বা "Download PDF" button click করে PDF generate করুন

---

**Created by**: Claude Code
**Date**: November 10, 2025
**Purpose**: Sample data for testing Allotment Letter PDF generation with proper government format
