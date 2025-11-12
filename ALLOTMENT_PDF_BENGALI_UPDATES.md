# Allotment PDF Bengali Date Updates

## Summary
Updated the Allotment PDF generation to display dates in proper Bengali format with Bengali digits, matching the official government document format.

## Changes Made

### 1. Created New Helper Class: `BengaliDateHelper.cs`
**Location**: `IMS.Application/Helpers/BengaliDateHelper.cs`

**Features**:
- Converts English digits (0-9) to Bengali digits (০-৯)
- Converts Gregorian dates to Bengali formatted strings
- Converts dates to Bengali calendar (বাংলা সন/বঙ্গাব্দ)
- Provides both date formats as needed for government documents

**Methods**:
- `ConvertToBengaliDigits(string)` - Converts any number string to Bengali digits
- `ConvertToBengaliDigits(int)` - Converts integer to Bengali digit string
- `GetBengaliMonthNameGregorian(int)` - Gets Bengali month name for Gregorian calendar
- `ConvertToGregorianBengali(DateTime)` - Converts to format: "২৫ আগস্ট ২০২৫ খ্রিস্টাব্দ"
- `ConvertToBengaliCalendar(DateTime)` - Converts to format: "১০ ভাদ্র ১৪৩২ বঙ্গাব্দ"
- `GetBothFormats(DateTime)` - Returns tuple of both formats
- `GetCompleteDateString(DateTime)` - Returns both formats separated by newline

### 2. Updated AllotmentLetterController.cs
**Location**: `IMS.Web/Controllers/AllotmentLetterController.cs`

**Changes**:
- Added `using IMS.Application.Helpers;` import
- Updated `ConvertToBengaliDate()` method to use the new `BengaliDateHelper`
- Now returns complete date string with both Bengali calendar and Gregorian dates

### 3. Updated PrintPDF.cshtml View
**Location**: `IMS.Web/Views/AllotmentLetter/PrintPDF.cshtml`

**Changes**:
- Added `@using IMS.Application.Helpers` directive
- Removed old `bengaliMonths` dictionary
- Added Bengali date variables at the top:
  ```csharp
  var (bengaliCalendarDate, gregorianBengaliDate) = BengaliDateHelper.GetBothFormats(Model.AllotmentDate);
  ```
- Updated all memo date lines (3 places: Page 1, Page 2, Page 3) to use `@gregorianBengaliDate`
- Updated the Bengali dates section on Page 2 to display both formats:
  ```html
  <div class="bengali-dates">
      <div>@bengaliCalendarDate</div>
      <div>@gregorianBengaliDate</div>
  </div>
  ```

## Date Format Examples

### Before:
- Memo date: "তারিখ: 25 আগস্ট 2025" (English digits)
- Date section: "আগস্ট 2025 বঙ্গাব্দ" (incomplete)

### After:
- Memo date: "তারিখ: ২৫ আগস্ট ২০২৫ খ্রিস্টাব্দ" (all Bengali digits)
- Date section:
  ```
  ১০ ভাদ্র ১৪৩২ বঙ্গাব্দ
  ২৫ আগস্ট ২০২৫ খ্রিস্টাব্দ
  ```

## Bengali Calendar Conversion Logic

The helper uses proper Bengali calendar conversion:
- Bengali year = Gregorian year - 593/594 (depending on date)
- Bengali new year (Pahela Baishakh) starts on April 14/15
- First 6 months (Baishakh to Ashwin): 31 days each
- Next 5 months (Kartik to Falgun): 30 days each
- Last month (Chaitra): 30 days (31 in leap year)

## Bengali Month Names

### Gregorian Months (in Bengali):
জানুয়ারি, ফেব্রুয়ারি, মার্চ, এপ্রিল, মে, জুন, জুলাই, আগস্ট, সেপ্টেম্বর, অক্টোবর, নভেম্বর, ডিসেম্বর

### Bengali Calendar Months:
বৈশাখ, জ্যৈষ্ঠ, আষাঢ়, শ্রাবণ, ভাদ্র, আশ্বিন, কার্তিক, অগ্রহায়ণ, পৌষ, মাঘ, ফাল্গুন, চৈত্র

## Testing

To test the changes:
1. Stop the running application
2. Build the solution: `dotnet build IMS.sln`
3. Run the application: `dotnet run --project IMS.Web`
4. Navigate to Allotment Letter module
5. Create or view an existing allotment letter
6. Click "Export PDF" or "Print PDF"
7. Verify dates appear in Bengali format with Bengali digits

## Files Modified

1. ✅ `IMS.Application/Helpers/BengaliDateHelper.cs` (NEW)
2. ✅ `IMS.Web/Controllers/AllotmentLetterController.cs`
3. ✅ `IMS.Web/Views/AllotmentLetter/PrintPDF.cshtml`

## Build Status

✅ Code compiles successfully (no compilation errors)
⚠️ File locking errors due to running application (expected - not a code issue)

## Next Steps

1. Stop the running application
2. Rebuild the solution
3. Test the PDF generation with an allotment letter
4. Verify the PDF matches the sample format with proper Bengali dates
