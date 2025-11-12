# Allotment Distribution List (অনুলিপি) - Dynamic Implementation

## Summary
Added dynamic distribution list functionality to Allotment Letters. The distribution list (অনুলিপি) now comes from the database instead of being hardcoded in the PDF template.

## Problem
The distribution list (অনুলিপি) section in Allotment PDF was hardcoded with the same 8 recipients for all letters. This should be dynamic and vary based on the specific allotment.

## Solution
Created a new entity `AllotmentLetterDistribution` to store distribution list entries for each allotment letter, with full Bengali support.

## Changes Made

### 1. Created New Entity: `AllotmentLetterDistribution`
**Location**: `IMS.Domain/Entities.cs`

```csharp
public class AllotmentLetterDistribution : BaseEntity
{
    public int AllotmentLetterId { get; set; }
    public int SerialNo { get; set; } // ক্রম: ১, ২, ৩...
    public string RecipientTitle { get; set; } // "মহাপরিচালক মহোদয়ের সচিবালয়"
    public string RecipientTitleBn { get; set; } // Bengali version
    public string Address { get; set; } // Full address
    public string AddressBn { get; set; } // Bengali address
    public string Purpose { get; set; } // "সময় অবগতির জন্য"
    public string PurposeBn { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation
    public virtual AllotmentLetter AllotmentLetter { get; set; }
}
```

### 2. Updated AllotmentLetter Entity
**Location**: `IMS.Domain/Entities.cs`

Added navigation property:
```csharp
// Distribution List (অনুলিপি) - who should receive copies
public virtual ICollection<AllotmentLetterDistribution> DistributionList { get; set; } = new List<AllotmentLetterDistribution>();
```

### 3. Updated ApplicationDbContext
**Location**: `IMS.Infrastructure/ApplicationDbContext.cs`

Added DbSet:
```csharp
public DbSet<AllotmentLetterDistribution> AllotmentLetterDistributions { get; set; }
```

### 4. Created DTO: `AllotmentLetterDistributionDto`
**Location**: `IMS.Application/NewDtos.cs`

```csharp
public class AllotmentLetterDistributionDto : BaseDto
{
    public int AllotmentLetterId { get; set; }
    public int SerialNo { get; set; } // ক্রম: ১, ২, ৩...
    public string RecipientTitle { get; set; }
    public string RecipientTitleBn { get; set; }
    public string Address { get; set; }
    public string AddressBn { get; set; }
    public string Purpose { get; set; }
    public string PurposeBn { get; set; }
    public int DisplayOrder { get; set; }
}
```

### 5. Updated AllotmentLetterDto
**Location**: `IMS.Application/NewDtos.cs`

Added property:
```csharp
// Distribution List (অনুলিপি) - who should receive copies
public List<AllotmentLetterDistributionDto> DistributionList { get; set; } = new();
```

### 6. Updated PrintPDF View
**Location**: `IMS.Web/Views/AllotmentLetter/PrintPDF.cshtml`

Changed from hardcoded list to dynamic rendering:

**Before:**
```html
<ul class="distribution-list">
    <li><strong>১।</strong> মহাপরিচালক মহোদয়ের সচিবালয়<br />
        &nbsp;&nbsp;&nbsp;&nbsp;বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা।</li>
    <!-- ... more hardcoded items ... -->
</ul>
```

**After:**
```razor
@if (Model.DistributionList != null && Model.DistributionList.Any())
{
    <ul class="distribution-list">
        @foreach (var item in Model.DistributionList.OrderBy(d => d.DisplayOrder).ThenBy(d => d.SerialNo))
        {
            <li>
                <strong>@BengaliDateHelper.ConvertToBengaliDigits(item.SerialNo)।</strong>
                @(item.RecipientTitleBn ?? item.RecipientTitle)
                @if (!string.IsNullOrEmpty(item.AddressBn ?? item.Address))
                {
                    <br />
                    <text>&nbsp;&nbsp;&nbsp;&nbsp;@(item.AddressBn ?? item.Address)</text>
                }
                @if (!string.IsNullOrEmpty(item.PurposeBn ?? item.Purpose))
                {
                    <text> @(item.PurposeBn ?? item.Purpose)।</text>
                }
            </li>
        }
    </ul>
}
else
{
    <!-- Falls back to default hardcoded list if none specified -->
}
```

## Features

### Dynamic Distribution List
- Each allotment letter can have its own unique distribution list
- Fully supports Bengali language
- Serial numbers automatically converted to Bengali digits (১, ২, ৩...)
- Flexible ordering using `DisplayOrder` and `SerialNo`

### Fallback Mechanism
- If no distribution list is specified, falls back to the default hardcoded list
- Ensures PDFs always render correctly even for old allotment letters

### Bengali Support
- Separate fields for English and Bengali versions
- `RecipientTitle` / `RecipientTitleBn`
- `Address` / `AddressBn`
- `Purpose` / `PurposeBn`
- Bengali takes precedence when rendering PDF

## Default Distribution List Entries

The following default entries should be seeded/created for standard allotments:

1. **মহাপরিচালক মহোদয়ের সচিবালয়**
   - Address: বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা।

2. **অতিরিক্ত মহাপরিচালক মহোদয়ের দপ্তর**
   - Address: বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা
   - Purpose: সময় অবগতির জন্য

3. **কমান্ডান্ট**
   - Address: বাংলাদেশ আনসার-ভিডিপি একাডেমী, সফিপুর, গাজীপুর

4. **উপমহাপরিচালক (প্রশাসন/অপারেশনস্/প্রশিক্ষণ)**
   - Address: বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা

5. **উপমহাপরিচালক/পরিচালক**
   - Address: বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী (সংশ্লিষ্ট রেঞ্জ)
   - Purpose: সময় অবগতি ও প্রয়োজনীয় ব্যবস্থা গ্রহণের জন্য

6. **সহকারী পরিচালক**
   - Title: সহকারী পরিচালক - কেন্দ্রীয় আনসার/ভিডিপি ভাণ্ডার
   - Address: আনসার-ভিডিপি একাডেমী, সফিপুর, গাজীপুর
   - Purpose: অবগতি ও প্রয়োজনীয় ব্যবস্থা গ্রহণের জন্য

7. **কোয়াটার মাস্টার**
   - Address: ভাণ্ডার শাখা, সদর দপ্তর, খিলগাঁও, ঢাকা

8. **দপ্তর/মাস্টার কপি**
   - Title: দপ্তর/মাস্টার কপি - উপপরিচালক (ভাণ্ডার)

## Next Steps

### 1. Create Database Migration
```bash
dotnet ef migrations add AddAllotmentLetterDistribution --project IMS.Infrastructure --startup-project IMS.Web
dotnet ef database update --project IMS.Infrastructure --startup-project IMS.Web
```

### 2. Update AllotmentLetterService
Add mapping logic to include DistributionList when fetching AllotmentLetterDto:

```csharp
allotmentDto.DistributionList = allotment.DistributionList
    .Select(d => new AllotmentLetterDistributionDto
    {
        Id = d.Id,
        AllotmentLetterId = d.AllotmentLetterId,
        SerialNo = d.SerialNo,
        RecipientTitle = d.RecipientTitle,
        RecipientTitleBn = d.RecipientTitleBn,
        Address = d.Address,
        AddressBn = d.AddressBn,
        Purpose = d.Purpose,
        PurposeBn = d.PurposeBn,
        DisplayOrder = d.DisplayOrder
    })
    .OrderBy(d => d.DisplayOrder)
    .ThenBy(d => d.SerialNo)
    .ToList();
```

### 3. Add UI for Managing Distribution List
Create interface in Allotment Letter Create/Edit forms to:
- Add/Remove distribution list entries
- Reorder entries
- Edit recipient details
- Use default template or custom entries

### 4. Create Distribution List Template Service (Optional)
Create a service to manage reusable distribution list templates that can be applied to multiple allotments.

## Files Modified

1. ✅ `IMS.Domain/Entities.cs` - Added `AllotmentLetterDistribution` entity
2. ✅ `IMS.Infrastructure/ApplicationDbContext.cs` - Added DbSet
3. ✅ `IMS.Application/NewDtos.cs` - Added DTOs
4. ✅ `IMS.Web/Views/AllotmentLetter/PrintPDF.cshtml` - Dynamic rendering
5. ✅ `IMS.Application/Services/AllotmentLetterService.cs` - Added DistributionList mapping
6. ✅ `IMS.Web/Views/AllotmentLetter/Create.cshtml` - Added distribution list UI
7. ✅ `IMS.Web/Views/AllotmentLetter/Edit.cshtml` - Added distribution list UI with pre-population
8. ✅ Migration `20251110112624_AddAllotmentLetterDistribution.cs` - Created and applied

## All Tasks Completed ✅

All implementation tasks have been successfully completed:
- Database migration created and applied
- Service layer mapping implemented
- UI for Create form with full functionality
- UI for Edit form with existing entry pre-population
- JavaScript functions for managing distribution entries
- Default distribution list template (8 standard entries)

## Testing

1. Create migration and update database
2. Add distribution list entries to an existing allotment letter (manually via DB)
3. Generate PDF and verify distribution list renders correctly
4. Verify Bengali serial numbers (১, ২, ৩...)
5. Test with no distribution list (should show default hardcoded list)
6. Test with custom distribution list (should show custom entries)

## Benefits

✅ **Flexible**: Each allotment can have unique distribution lists
✅ **Bengali Support**: Full Bengali language support with proper digit conversion
✅ **Backward Compatible**: Falls back to default list for old allotments
✅ **Sortable**: Entries can be reordered using DisplayOrder
✅ **Maintainable**: Easy to add/edit/remove entries without code changes
