# 📖 Ledger Books Setup Guide

এই guide আপনাকে বলবে কিভাবে Ledger Books module এ sample data populate করবেন।

---

## 🚀 Quick Start (3 Steps)

### Step 1: Migration Run করুন

**Package Manager Console:**
```powershell
Add-Migration AddLedgerBookIdToIssueAndReceiveItems
Update-Database
```

**অথবা Command Line:**
```cmd
dotnet ef migrations add AddLedgerBookIdToIssueAndReceiveItems --project IMS.Infrastructure --startup-project IMS.Web
dotnet ef database update --project IMS.Infrastructure --startup-project IMS.Web
```

### Step 2: SQL Script Run করুন

**SQL Server Management Studio (SSMS):**
1. Open `seed_ledger_books.sql` file
2. Connect to your database: `ansvdp_ims`
3. Execute (F5)

**অথবা sqlcmd:**
```cmd
sqlcmd -S ADMIN\SQLEXPRESS -d ansvdp_ims -i seed_ledger_books.sql
```

### Step 3: Verify

1. Run application: `dotnet run --project IMS.Web`
2. Navigate: Left Sidebar → Store Management → Ledger Books
3. You should see 8 sample ledger books!

---

## 📊 What the Script Creates

### Ledger Books Created:

| Ledger No | Book Name | Type | Pages | Status |
|-----------|-----------|------|-------|--------|
| ISS-2025-001 | Issue Register - Central | Issue | 500 | Active |
| ISS-2025-002 | Issue Register - Provision | Issue | 500 | Active |
| ISS-2025-003 | Issue Register - Store 1 | Issue | 500 | In Use (25 pages used) |
| RCV-2025-001 | Receive Register - Central | Receive | 500 | Active |
| RCV-2025-002 | Receive Register - Provision | Receive | 500 | Active |
| RCV-2025-003 | Receive Register - Store 1 | Receive | 500 | In Use (15 pages used) |
| TRF-2025-001 | Transfer Register - Central | Transfer | 500 | Active |
| GEN-2025-001 | General Register | General | 500 | In Use (50 pages used) |

---

## 🔗 What Gets Linked

### Automatic Linking:

1. **Existing IssueItems** → Linked to appropriate Issue ledger books
   - Based on StoreId matching
   - Auto-assigned page numbers
   - LedgerNo populated

2. **Existing ReceiveItems** → Linked to appropriate Receive ledger books
   - Based on StoreId matching
   - Auto-assigned page numbers
   - LedgerNo populated

3. **CurrentPageNo Updated** → All ledger books updated with correct page tracking

---

## 📝 Script Features

### Smart Store Detection:
- ✅ Automatically finds Central Store
- ✅ Automatically finds Provision Store
- ✅ Uses available stores if specific not found
- ✅ Shows error if no stores exist

### Safe Re-execution:
- ✅ Can run multiple times safely
- ✅ Cleans up existing sample data before creating new
- ✅ Doesn't duplicate data

### Comprehensive Linking:
- ✅ Links IssueItems by Store matching
- ✅ Links ReceiveItems by Store matching
- ✅ Assigns remaining items to default ledger
- ✅ Updates page counters automatically

---

## 🔍 Verification Queries

### Check Created Ledger Books:
```sql
SELECT LedgerNo, BookName, BookType, CurrentPageNo, TotalPages
FROM LedgerBooks
WHERE IsActive = 1
ORDER BY BookType, LedgerNo
```

### Check Linked IssueItems:
```sql
SELECT
    ii.Id,
    i.IssueNo,
    lb.LedgerNo,
    ii.PageNo,
    ii.Quantity
FROM IssueItems ii
INNER JOIN Issues i ON ii.IssueId = i.Id
LEFT JOIN LedgerBooks lb ON ii.LedgerBookId = lb.Id
WHERE ii.LedgerBookId IS NOT NULL
ORDER BY ii.CreatedAt DESC
```

### Check Linked ReceiveItems:
```sql
SELECT
    ri.Id,
    r.ReceiveNo,
    lb.LedgerNo,
    ri.PageNo,
    ri.Quantity
FROM ReceiveItems ri
INNER JOIN Receives r ON ri.ReceiveId = r.Id
LEFT JOIN LedgerBooks lb ON ri.LedgerBookId = lb.Id
WHERE ri.LedgerBookId IS NOT NULL
ORDER BY ri.CreatedAt DESC
```

### Statistics by Book Type:
```sql
SELECT
    BookType,
    COUNT(*) AS TotalBooks,
    SUM(TotalPages) AS TotalPages,
    SUM(CurrentPageNo - 1) AS PagesUsed,
    SUM(TotalPages - CurrentPageNo + 1) AS PagesRemaining
FROM LedgerBooks
WHERE IsActive = 1
GROUP BY BookType
```

---

## 🧪 Testing Workflow

### Test Issue Form:
1. Go to: **Issue → Create New Issue**
2. Select a Store from "From Store" dropdown
3. Add an item
4. Check "Ledger Book" dropdown - should load books for that store
5. Select a ledger book - Page No should auto-fill
6. Complete and create issue

### Test Receive Form:
1. Go to: **Receive → Create New Receive**
2. Select a Store
3. Add an item
4. Check "Ledger Book" dropdown - should load Receive type books
5. Select a ledger book - Page No should auto-fill
6. Complete and create receive

### Test Ledger Book Management:
1. Go to: **Left Sidebar → Store Management → Ledger Books**
2. View list - should show all 8 books
3. Filter by Store, Book Type, Status
4. Click "Details" on any book - should show statistics
5. Try creating a new ledger book
6. Try editing an existing book

---

## ⚠️ Important Notes

### Before Running Script:

1. **Backup your database first!**
   ```sql
   BACKUP DATABASE ansvdp_ims TO DISK = 'C:\Backup\ansvdp_ims_backup.bak'
   ```

2. **Ensure migrations are applied:**
   ```powershell
   Update-Database
   ```

3. **Ensure at least one Store exists:**
   ```sql
   SELECT * FROM Stores WHERE IsActive = 1
   ```

### After Running Script:

1. **Check the output messages** - script provides detailed logs
2. **Verify ledger books created** - check counts match expected
3. **Test the UI** - ensure dropdowns work correctly
4. **Check linking** - ensure existing items are linked properly

---

## 🐛 Troubleshooting

### Error: "No active stores found"
**Solution:** Create at least one store first
```sql
-- Check stores
SELECT * FROM Stores WHERE IsActive = 1
```

### Error: "Invalid column name 'LedgerBookId'"
**Solution:** Run migrations first
```powershell
Update-Database
```

### Issue: Ledger dropdown empty in Issue/Receive form
**Solution:**
1. Check JavaScript console for errors
2. Verify API endpoint: `/LedgerBook/GetActiveLedgerBooks`
3. Check store is selected first

### Issue: Page number not auto-filling
**Solution:**
1. Check ledger book has `CurrentPageNo` set
2. Verify JavaScript `updatePageNumber` function exists
3. Check browser console for JavaScript errors

---

## 📚 Related Files

- **SQL Script:** `seed_ledger_books.sql`
- **Entity:** `IMS.Domain/Entities.cs` (LedgerBook class)
- **Service:** `IMS.Application/Services/LedgerBookService.cs`
- **Controller:** `IMS.Web/Controllers/LedgerBookController.cs`
- **Views:** `IMS.Web/Views/LedgerBook/`
- **Migration:** `IMS.Infrastructure/Migrations/*_AddLedgerBookManagement.cs`

---

## 🎯 Summary

**What You Get:**
- ✅ 8 sample ledger books across different stores
- ✅ Issue, Receive, Transfer, and General book types
- ✅ Automatic linking of existing Issue/Receive items
- ✅ Page number tracking and auto-increment
- ✅ Ready-to-use ledger management system

**Next Steps:**
1. Run the migration
2. Execute the SQL script
3. Test the UI
4. Start using ledger books in Issue/Receive workflows!

---

**Created:** November 2025
**For:** Ansar VDP Inventory Management System
**Module:** Ledger Books Management
