# Quick Test Checklist - Item Edit Page

## Before Testing

### Step 1: Restart Application
```bash
# Stop application (Ctrl+C in terminal)
# Then restart:
dotnet run --project IMS.Web
```

### Step 2: Hard Refresh Browser
- **Chrome/Edge:** `Ctrl + Shift + R`
- **Firefox:** `Ctrl + Shift + R`
- **Alternative:** `Ctrl + F5`

---

## Quick Tests (5 minutes)

### ✅ Test 1: Item Type Dropdown (30 seconds)
**URL:** `https://localhost:7029/Item/Edit/11`

**Check:**
- [ ] Dropdown shows "Expendable" or "NonExpendable" selected
- [ ] Not blank/empty

**If PASS:** ✅ Move to Test 2
**If FAIL:** ❌ See troubleshooting below

---

### ✅ Test 2: Success Alert (1 minute)
**Steps:**
1. Edit any field (e.g., change Description)
2. Click **Save Changes**

**Check:**
- [ ] Green success alert appears (top-right)
- [ ] Message says "Item updated successfully!"

**If PASS:** ✅ Move to Test 3
**If FAIL:** ❌ See troubleshooting below

---

### ✅ Test 3: Validation (30 seconds)
**Steps:**
1. Clear the **Name** field
2. Click **Save Changes**

**Check:**
- [ ] Form does NOT submit
- [ ] Red error appears under Name field
- [ ] Message says "The Name field is required"

**If PASS:** ✅ Move to Test 4
**If FAIL:** ❌ See troubleshooting below

---

### ✅ Test 4: Console Logs (30 seconds)
**Steps:**
1. Press **F12** to open Browser Console
2. Refresh page

**Check:**
- [ ] See: `Item Type set to: 1`
- [ ] No JavaScript errors in red

**If PASS:** ✅ All tests passed!
**If FAIL:** ❌ See troubleshooting below

---

## 🐛 Quick Troubleshooting

### Dropdown Not Selecting?
1. Check **Application Terminal** for logs:
   ```
   Item Type: Expendable (raw value: 1)
   Value=1, Text=Expendable, Selected=True
   ```
2. If `Selected=False`: Problem in Controller
3. If `Selected=True`: Problem in View rendering

**Quick Fix:**
```bash
# Restart application
Ctrl+C
dotnet run --project IMS.Web

# Hard refresh browser
Ctrl+Shift+R
```

---

### Success Alert Not Showing?
1. Check if toastr is loaded:
   - F12 → Console → Type: `typeof toastr`
   - Should show: `"object"`
2. Check _Layout.cshtml has toastr.js

---

### Validation Not Working?
1. Check if jQuery is loaded:
   - F12 → Console → Type: `typeof $`
   - Should show: `"function"`
2. Hard refresh browser (Ctrl+Shift+R)

---

## ✅ All Tests Passed?

**Congratulations!** 🎉 Everything is working correctly!

### What's Fixed:
✅ Item Type dropdown shows selected value
✅ Success/Error alerts work
✅ Client-side validation enabled
✅ Server-side validation prevents invalid data

---

## ❌ Tests Failed?

### Get Help:
1. Copy **Application Terminal output**
2. Copy **Browser Console output** (F12)
3. Take screenshot of the issue
4. Check: `ITEM_EDIT_COMPLETE_FIX_SUMMARY.md` for detailed troubleshooting

---

## 📝 Console Commands for Debugging

Open Browser Console (F12) and run:

```javascript
// Check jQuery
console.log('jQuery loaded:', typeof $ !== 'undefined');

// Check toastr
console.log('toastr loaded:', typeof toastr !== 'undefined');

// Check Type dropdown value
console.log('Type value:', $('#Type').val());

// Check Type dropdown HTML
console.log('Type HTML:', $('#Type').html());

// Check if Type option is selected
console.log('Selected option:', $('#Type option:selected').text());
```

Copy and paste all output!

---

**Quick Checklist Complete!**
**For detailed documentation, see:** `ITEM_EDIT_COMPLETE_FIX_SUMMARY.md`
