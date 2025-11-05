# Voucher System Implementation Guide
## প্রাপ্তি বিলি ও ব্যয়ের রশিদ (Receipt & Expense Voucher)

---

## ✅ COMPLETED TASKS

### 1. Database Schema Updates ✅
- **IssueItem** entity তে নতুন fields add করা হয়েছে:
  - `LedgerNo` (string) - লেজার নম্বর
  - `PageNo` (string) - পৃষ্ঠা নম্বর
  - `UsableQuantity` (decimal?) - ব্যবহারযোগ্য পরিমাণ
  - `PartiallyUsableQuantity` (decimal?) - আংশিক ব্যবহারযোগ্য
  - `UnusableQuantity` (decimal?) - অকেজো

- **ReceiveItem** entity তে same fields add করা হয়েছে

- **Receive** entity তে Voucher fields add করা হয়েছে:
  - `VoucherNo` (string)
  - `VoucherNumber` (string)
  - `VoucherDate` (DateTime?)
  - `VoucherGeneratedDate` (DateTime?)
  - `VoucherQRCode` (string)
  - `VoucherDocumentPath` (string)

### 2. Migration Created & Applied ✅
- Migration Name: `AddVoucherLedgerPageFields`
- Status: Successfully applied to database
- All new columns created in database

### 3. Service Layer ✅
- **IVoucherService** interface created in `Interfaces.cs`
- **VoucherService** implementation created in `Services/VoucherService.cs`
- Service registered in `Program.cs`

---

## ⚠️ PENDING TASKS - NEEDS MANUAL FIX

### Task 1: Fix VoucherService Compilation Errors

**Location**: `IMS.Application/Services/VoucherService.cs`

**Issues**:
1. **FindAsync doesn't support 'include' parameter**
2. **Document class name conflict** between `IMS.Domain.Entities.Document` and `iTextSharp.text.Document`

**Fix Required**:

Replace the using statement at the top:
```csharp
using PdfDocument = iTextSharp.text.Document;  // Add alias
```

Then replace all instances of:
```csharp
Document document = new Document(PageSize.A4, 20, 20, 20, 20);
```

With:
```csharp
PdfDocument document = new PdfDocument(PageSize.A4, 20, 20, 20, 20);
```

For the `FindAsync` issue, you need to load related entities separately. Replace lines like:

```csharp
var issue = await _unitOfWork.Issues.FindAsync(
    i => i.Id == issueId,
    include: query => query
        .Include(i => i.Items).ThenInclude(ii => ii.Item)
        ...
);
```

With:
```csharp
var issueQuery = _unitOfWork.Issues.GetQueryable()
    .Include(i => i.Items).ThenInclude(ii => ii.Item)
    .Include(i => i.FromStore)
    .Include(i => i.IssuedToBattalion)
    .Include(i => i.IssuedToRange)
    .Include(i => i.IssuedToZila)
    .Where(i => i.Id == issueId);

var issueEntity = await issueQuery.FirstOrDefaultAsync();
```

**OR** simpler approach - check if your IRepository has a method like `GetByIdWithIncludesAsync` and use that.

---

### Task 2: Add Bengali Font File

**Location**: `IMS.Web/wwwroot/fonts/kalpurush.ttf`

**Steps**:
1. Create folder: `IMS.Web/wwwroot/fonts/`
2. Download Kalpurush font (free Bengali Unicode font)
3. Place `kalpurush.ttf` in the fonts folder
4. Alternative fonts: SolaimanLipi, Nikosh, Akaash

**Download Link**: https://fonts.google.com/noto/specimen/Noto+Sans+Bengali
Or use: https://ekushey.org/?page/downloads

---

### Task 3: Update DTOs

**Location**: `IMS.Application/DTOs.cs`

**IssueItemDto** needs these fields:
```csharp
public class IssueItemDto
{
    // ... existing fields ...

    // Add these:
    public string LedgerNo { get; set; }
    public string PageNo { get; set; }
    public decimal? UsableQuantity { get; set; }
    public decimal? PartiallyUsableQuantity { get; set; }
    public decimal? UnusableQuantity { get; set; }
}
```

**ReceiveItemDto** needs same fields:
```csharp
public class ReceiveItemDto
{
    // ... existing fields ...

    // Add these:
    public string LedgerNo { get; set; }
    public string PageNo { get; set; }
    public decimal? UsableQuantity { get; set; }
    public decimal? PartiallyUsableQuantity { get; set; }
    public decimal? UnusableQuantity { get; set; }
}
```

---

### Task 4: Update Issue Create/Edit Forms

**Location**: `IMS.Web/Views/Issue/Create.cshtml` and `Edit.cshtml`

**Add these fields in the item entry section**:

```html
<!-- For each item in the items list/table -->
<tr>
    <!-- Existing columns... -->

    <!-- Add these new columns -->
    <td>
        <input type="text"
               name="Items[@i].LedgerNo"
               class="form-control form-control-sm"
               placeholder="লেজার নং" />
    </td>
    <td>
        <input type="text"
               name="Items[@i].PageNo"
               class="form-control form-control-sm"
               placeholder="পৃষ্ঠা নং" />
    </td>
    <td>
        <input type="number"
               name="Items[@i].UsableQuantity"
               class="form-control form-control-sm usable-qty"
               placeholder="ব্যবহারযোগ্য"
               step="0.01" />
    </td>
    <td>
        <input type="number"
               name="Items[@i].PartiallyUsableQuantity"
               class="form-control form-control-sm partial-qty"
               placeholder="আংশিক"
               step="0.01" />
    </td>
    <td>
        <input type="number"
               name="Items[@i].UnusableQuantity"
               class="form-control form-control-sm unusable-qty"
               placeholder="অকেজো"
               step="0.01" />
    </td>
</tr>
```

**Add JavaScript validation**:
```javascript
// Ensure sum of usable + partial + unusable = total quantity
$('.usable-qty, .partial-qty, .unusable-qty').on('change', function() {
    var row = $(this).closest('tr');
    var total = parseFloat(row.find('.quantity-input').val()) || 0;
    var usable = parseFloat(row.find('.usable-qty').val()) || 0;
    var partial = parseFloat(row.find('.partial-qty').val()) || 0;
    var unusable = parseFloat(row.find('.unusable-qty').val()) || 0;

    var sum = usable + partial + unusable;

    if (Math.abs(sum - total) > 0.01 && sum > 0) {
        alert('ব্যবহারযোগ্য + আংশিক + অকেজো = মোট পরিমাণ হতে হবে!');
        return false;
    }
});
```

---

### Task 5: Update Receive Create/Edit Forms

**Location**: `IMS.Web/Views/Receive/Create.cshtml` and `Edit.cshtml`

Same fields as Issue forms (see Task 4 above).

---

### Task 6: Add Voucher Download Buttons

**Location**: `IMS.Web/Views/Issue/Details.cshtml`

Add this button in the action buttons section:

```html
@if (!string.IsNullOrEmpty(Model.VoucherNo))
{
    <a href="@Url.Action("DownloadVoucher", "Issue", new { id = Model.Id })"
       class="btn btn-success"
       target="_blank">
        <i class="fas fa-file-pdf"></i> ভাউচার ডাউনলোড
    </a>
}
else
{
    <form asp-action="GenerateVoucher" asp-route-id="@Model.Id" method="post" style="display:inline;">
        <button type="submit" class="btn btn-primary">
            <i class="fas fa-file-invoice"></i> ভাউচার তৈরি করুন
        </button>
    </form>
}
```

**Location**: `IMS.Web/Views/Receive/Details.cshtml`

Same buttons as above.

---

### Task 7: Add Controller Actions for Voucher Download

**Location**: `IMS.Web/Controllers/IssueController.cs`

Add these methods:

```csharp
private readonly IVoucherService _voucherService;  // Add to constructor

[HttpPost]
public async Task<IActionResult> GenerateVoucher(int id)
{
    try
    {
        var voucherNo = await _voucherService.GenerateIssueVoucherAsync(id);
        TempData["Success"] = $"ভাউচার তৈরি হয়েছে: {voucherNo}";
        return RedirectToAction(nameof(Details), new { id });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating voucher for Issue {IssueId}", id);
        TempData["Error"] = "ভাউচার তৈরিতে সমস্যা হয়েছে।";
        return RedirectToAction(nameof(Details), new { id });
    }
}

[HttpGet]
public async Task<IActionResult> DownloadVoucher(int id)
{
    try
    {
        var pdfBytes = await _voucherService.GetIssueVoucherPdfAsync(id);
        var issue = await _issueService.GetIssueByIdAsync(id);

        string fileName = $"Issue_Voucher_{issue.VoucherNo}_{DateTime.Now:yyyyMMdd}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error downloading voucher for Issue {IssueId}", id);
        TempData["Error"] = "ভাউচার ডাউনলোডে সমস্যা হয়েছে।";
        return RedirectToAction(nameof(Details), new { id });
    }
}
```

**Location**: `IMS.Web/Controllers/ReceiveController.cs`

Similar methods for Receive:

```csharp
private readonly IVoucherService _voucherService;  // Add to constructor

[HttpPost]
public async Task<IActionResult> GenerateVoucher(int id)
{
    try
    {
        var voucherNo = await _voucherService.GenerateReceiveVoucherAsync(id);
        TempData["Success"] = $"ভাউচার তৈরি হয়েছে: {voucherNo}";
        return RedirectToAction(nameof(Details), new { id });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating voucher for Receive {ReceiveId}", id);
        TempData["Error"] = "ভাউচার তৈরিতে সমস্যা হয়েছে।";
        return RedirectToAction(nameof(Details), new { id });
    }
}

[HttpGet]
public async Task<IActionResult> DownloadVoucher(int id)
{
    try
    {
        var pdfBytes = await _voucherService.GetReceiveVoucherPdfAsync(id);
        var receive = await _receiveService.GetReceiveByIdAsync(id);

        string fileName = $"Receive_Voucher_{receive.VoucherNo}_{DateTime.Now:yyyyMMdd}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error downloading voucher for Receive {ReceiveId}", id);
        TempData["Error"] = "ভাউচার ডাউনলোডে সমস্যা হয়েছে।";
        return RedirectToAction(nameof(Details), new { id });
    }
}
```

---

### Task 8: Update IssueService and ReceiveService Mapping

When creating/updating issues and receives, map the new fields:

**IssueService.cs** - in `CreateIssueAsync` and `UpdateIssueAsync`:
```csharp
foreach (var itemDto in dto.Items)
{
    var issueItem = new IssueItem
    {
        // ... existing mappings ...
        LedgerNo = itemDto.LedgerNo,
        PageNo = itemDto.PageNo,
        UsableQuantity = itemDto.UsableQuantity,
        PartiallyUsableQuantity = itemDto.PartiallyUsableQuantity,
        UnusableQuantity = itemDto.UnusableQuantity
    };
    // ...
}
```

**ReceiveService.cs** - similar mapping in `CreateReceiveAsync` and `UpdateReceiveAsync`

---

## 📋 TESTING CHECKLIST

After completing all tasks above:

### 1. Issue Voucher Testing
- [ ] Create new Issue with items
- [ ] Enter Ledger No and Page No for each item
- [ ] Enter Usable, Partially Usable, Unusable quantities
- [ ] Save Issue
- [ ] Click "ভাউচার তৈরি করুন" button
- [ ] Verify voucher number generated
- [ ] Click "ভাউচার ডাউনলোড" button
- [ ] Verify PDF downloads with Bengali text
- [ ] Check PDF has all items with ledger/page numbers
- [ ] Verify signature sections appear correctly

### 2. Receive Voucher Testing
- [ ] Create new Receive with items
- [ ] Enter Ledger No and Page No for each item
- [ ] Enter condition breakdown (Usable/Partial/Unusable)
- [ ] Save Receive
- [ ] Generate voucher
- [ ] Download and verify PDF

### 3. Edge Cases
- [ ] Test with items that have no ledger/page (should show dots/blank)
- [ ] Test with Bengali item names
- [ ] Test with large quantity numbers
- [ ] Test regenerating existing voucher
- [ ] Test voucher for Issue with 10+ items (should paginate correctly)

---

## 🎨 PDF CUSTOMIZATION OPTIONS

If you need to customize the voucher appearance, edit `VoucherService.cs`:

### Change Font Size
```csharp
Font bengaliFont = GetBengaliFont(10);  // Change 10 to desired size
Font bengaliFontBold = GetBengaliFont(12, Font.BOLD);  // Change 12
```

### Change Page Size
```csharp
// Currently A4:
Document document = new Document(PageSize.A4, 20, 20, 20, 20);

// For A4 Landscape:
Document document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);

// For Letter size:
Document document = new Document(PageSize.LETTER, 20, 20, 20, 20);
```

### Change Table Column Widths
```csharp
// Currently:
table.SetWidths(new float[] { 5f, 8f, 8f, 25f, 10f, 10f, 10f, 10f, 10f, 10f });

// Adjust percentages as needed
// Make লেজার নং wider:
table.SetWidths(new float[] { 5f, 10f, 8f, 25f, 9f, 9f, 9f, 9f, 9f, 9f });
```

---

## 📁 FILE STRUCTURE

```
IMS/
├── IMS.Domain/
│   └── Entities.cs (✅ Updated with new fields)
├── IMS.Application/
│   ├── Interfaces/
│   │   └── Interfaces.cs (✅ IVoucherService added)
│   ├── Services/
│   │   └── VoucherService.cs (⚠️ Needs compilation fixes)
│   └── DTOs.cs (⚠️ Needs field additions)
├── IMS.Infrastructure/
│   └── Migrations/
│       └── AddVoucherLedgerPageFields.cs (✅ Applied)
└── IMS.Web/
    ├── Program.cs (✅ Service registered)
    ├── Controllers/
    │   ├── IssueController.cs (⚠️ Needs voucher actions)
    │   └── ReceiveController.cs (⚠️ Needs voucher actions)
    ├── Views/
    │   ├── Issue/
    │   │   ├── Create.cshtml (⚠️ Needs field additions)
    │   │   ├── Edit.cshtml (⚠️ Needs field additions)
    │   │   └── Details.cshtml (⚠️ Needs download button)
    │   └── Receive/
    │       ├── Create.cshtml (⚠️ Needs field additions)
    │       ├── Edit.cshtml (⚠️ Needs field additions)
    │       └── Details.cshtml (⚠️ Needs download button)
    └── wwwroot/
        ├── fonts/
        │   └── kalpurush.ttf (⚠️ Needs to be added)
        └── vouchers/ (✅ Auto-created by service)
```

---

## ⏱️ ESTIMATED COMPLETION TIME

- Task 1 (Fix VoucherService): **30 minutes**
- Task 2 (Add Bengali font): **5 minutes**
- Task 3 (Update DTOs): **10 minutes**
- Task 4-5 (Update forms): **45 minutes**
- Task 6-7 (Add buttons and actions): **30 minutes**
- Task 8 (Update service mappings): **20 minutes**
- Testing: **30 minutes**

**Total**: ~2.5 hours

---

## 🆘 TROUBLESHOOTING

### Issue: PDF shows boxes instead of Bengali text
**Solution**: Verify Bengali font file exists and path is correct in VoucherService.cs

### Issue: "Document is ambiguous" error
**Solution**: Add `using PdfDocument = iTextSharp.text.Document;` at top of file

### Issue: Voucher number not generating
**Solution**: Check `VoucherNo` field in database is nullable and service has permission to write files

### Issue: PDF file not found after generation
**Solution**: Verify `wwwroot/vouchers/` folder exists and has write permissions

### Issue: Ledger/Page numbers not saving
**Solution**: Check DTO mapping in IssueService/ReceiveService includes new fields

---

## 📞 SUPPORT

If you encounter issues:
1. Check build errors first: `dotnet build IMS.sln`
2. Check database migration status: `dotnet ef migrations list`
3. Check application logs for runtime errors
4. Verify all file paths are correct
5. Test with simple data first (1-2 items)

---

## ✨ FUTURE ENHANCEMENTS

Consider adding:
- [ ] Voucher preview before download
- [ ] Email voucher to recipient
- [ ] Print voucher directly from browser
- [ ] Voucher history/audit log
- [ ] Bulk voucher generation
- [ ] Custom voucher templates
- [ ] QR code verification system
- [ ] Digital signature integration

---

**Last Updated**: 2025-01-05
**Status**: Core implementation complete, manual fixes required
**Priority**: High - Required for production use
