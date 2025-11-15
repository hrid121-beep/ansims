# Report Filters - Complete Implementation Guide

## 📊 COMPLETION STATUS

### ✅ COMPLETED & TESTED (5/9 Reports - 55%)

1. **StockReport.cshtml** - ✅ 10 comprehensive filters
2. **IssueReport.cshtml** - ✅ 10 comprehensive filters
3. **PurchaseReport.cshtml** - ✅ 10+ advanced filters
4. **TransferReport.cshtml** - ✅ 10+ advanced filters
5. **LossReport.cshtml** - ✅ 11 advanced filters

**Branch:** `claude/add-report-filters-01HwwX6xTrNbk2169cJdn2vB`
**Status:** All committed & pushed to remote

---

## 🔄 REMAINING REPORTS (4/9)

### 6. MovementHistory.cshtml
### 7. ConsumptionReport.cshtml
### 8. ExpiryReport.cshtml
### 9. AuditReport.cshtml

---

## 🎯 PROVEN PATTERN (Applied to All 5 Completed Reports)

### Step 1: Update Filter Card Header

```html
<!-- OLD -->
<div class="card-header">
    <h3 class="card-title">Filter Options</h3>
    ...
</div>

<!-- NEW -->
<div class="card-header bg-light">
    <h3 class="card-title"><i class="fas fa-filter"></i> <strong>Advanced Filter Options</strong></h3>
    <div class="card-tools">
        <span class="badge badge-info mr-2">Real-time Filtering</span>
        <button type="button" class="btn btn-tool" data-card-widget="collapse">
            <i class="fas fa-minus"></i>
        </button>
    </div>
</div>
```

### Step 2: Add Filter Form with ID and Enhanced Filters

```html
<form asp-action="ReportName" method="get" id="filterForm">
    <!-- Row 1: Server-side filters (existing + enhanced) -->
    <div class="row">
        <!-- Add icons to existing labels -->
        <label><i class="far fa-calendar-alt"></i> From Date:</label>

        <!-- Add select2 class to existing dropdowns -->
        <select class="form-control select2" id="existingId" name="existingName">
            <option value="">-- All Options --</option>
            ...
        </select>
    </div>

    <!-- Row 2: NEW Client-side filters -->
    <div class="row">
        <div class="col-md-3">
            <div class="form-group">
                <label><i class="fas fa-info-circle"></i> Status:</label>
                <select id="statusFilter" class="form-control select2">
                    <option value="">-- All Statuses --</option>
                    <option value="Draft">Draft</option>
                    <option value="Pending">Pending</option>
                    <option value="Approved">Approved</option>
                    <!-- Add other statuses based on report type -->
                </select>
            </div>
        </div>

        <div class="col-md-6">
            <div class="form-group">
                <label><i class="fas fa-search"></i> Search:</label>
                <input type="text" id="searchFilter" class="form-control"
                       placeholder="Search by [relevant columns]">
            </div>
        </div>

        <div class="col-md-3">
            <div class="form-group">
                <label><i class="fas fa-greater-than-equal"></i> Min Quantity:</label>
                <input type="number" id="minQtyFilter" class="form-control" placeholder="0">
            </div>
        </div>
    </div>

    <!-- Row 3: Additional filters + Reset button -->
    <div class="row">
        <!-- Add Min/Max filters as needed -->

        <div class="col-md-3">
            <div class="form-group">
                <label>&nbsp;</label>
                <button type="button" class="btn btn-secondary btn-block" onclick="clearFilters()">
                    <i class="fas fa-redo"></i> Reset Filters
                </button>
            </div>
        </div>
    </div>
</form>
```

### Step 3: Update Table (Add hover class)

```html
<!-- OLD -->
<table class="table table-bordered table-striped" id="reportTable">

<!-- NEW -->
<table class="table table-bordered table-striped table-hover" id="reportTable">
```

### Step 4: Enhance DataTable Initialization in @section Scripts

```javascript
@section Scripts {
    <script>
        $(function () {
            // Initialize Select2
            $('.select2').select2({
                theme: 'bootstrap4',
                allowClear: true
            });

            // Initialize DataTable with enhanced settings
            var table = $("#reportTable").DataTable({
                "responsive": true,
                "lengthChange": true,
                "autoWidth": false,
                "pageLength": 25,
                "order": [[1, "desc"]], // Adjust column index as needed
                "language": {
                    "search": "Quick Table Search:",
                    "lengthMenu": "Show _MENU_ [items] per page",
                    "info": "Showing _START_ to _END_ of _TOTAL_ [items]",
                    "infoEmpty": "No [items] available",
                    "infoFiltered": "(filtered from _MAX_ total [items])"
                }
            });

            // Apply advanced filters
            $('#statusFilter, #searchFilter, #minQtyFilter, #maxQtyFilter').on('change keyup', function() {
                applyAdvancedFilters();
            });

            function applyAdvancedFilters() {
                var statusFilter = $('#statusFilter').val().toLowerCase();
                var searchFilter = $('#searchFilter').val().toLowerCase();
                var minQty = parseFloat($('#minQtyFilter').val()) || 0;
                var maxQty = parseFloat($('#maxQtyFilter').val()) || Infinity;

                // Custom filtering function
                $.fn.dataTable.ext.search.push(
                    function(settings, data, dataIndex) {
                        if (settings.nTable.id !== 'reportTable') return true;

                        // IMPORTANT: Adjust column indices based on your table structure
                        // Example: data[0] = first column, data[1] = second column, etc.
                        var rowStatus = data[7].toLowerCase(); // Adjust index
                        var rowSearch1 = data[1].toLowerCase(); // Adjust index
                        var rowSearch2 = data[2].toLowerCase(); // Adjust index
                        var rowQty = parseFloat(data[6].replace(/[^0-9.-]+/g, '')) || 0; // Adjust index

                        // Status filter
                        if (statusFilter && !rowStatus.includes(statusFilter)) return false;

                        // Search filter (multiple columns)
                        if (searchFilter && !rowSearch1.includes(searchFilter) &&
                            !rowSearch2.includes(searchFilter)) return false;

                        // Quantity range filter
                        if (rowQty < minQty || rowQty > maxQty) return false;

                        return true;
                    }
                );

                table.draw();
                $.fn.dataTable.ext.search.pop();
            }

            // [Keep existing chart code here...]
        });

        // [Keep existing export functions...]

        // ADD THIS: Clear filters function
        function clearFilters() {
            $('#filterForm')[0].reset();
            $('.select2').val('').trigger('change');
            $('#searchFilter, #minQtyFilter, #maxQtyFilter').val('');
            window.location.href = '@Url.Action("ReportName", "Report")';
        }
    </script>
}
```

---

## 📝 REPORT-SPECIFIC IMPLEMENTATION DETAILS

### 6. MovementHistory.cshtml

**Current Filters:** From/To Date
**Table Columns:** Movement Type, Date, Item, Quantity, Store, Reference

**Filters to Add:**
```javascript
// Client-side filters:
1. Movement Type dropdown (Purchase, Issue, Transfer, Return, Loss)
2. Search (Item name, Reference number)
3. Min/Max Quantity range
4. Store filter (if not already present)
```

**Column Indices for Filtering:**
```javascript
// Adjust based on actual table structure:
var rowType = data[0].toLowerCase();       // Movement Type
var rowDate = data[1];                     // Date
var rowItem = data[2].toLowerCase();       // Item
var rowQty = parseFloat(data[3]) || 0;     // Quantity
var rowStore = data[4].toLowerCase();      // Store
var rowRef = data[5].toLowerCase();        // Reference
```

---

### 7. ConsumptionReport.cshtml

**Current Filters:** From/To Date, Store, Category
**Table Columns:** Item, Category, Opening Stock, Issues, Consumption, Closing Stock

**Filters to Add:**
```javascript
// Client-side filters:
1. Item search (text input)
2. Min/Max Consumption range
3. Period selector (Daily, Weekly, Monthly)
4. Usage pattern (High Usage, Medium, Low)
5. Sort by Consumption
```

**Column Indices for Filtering:**
```javascript
var rowItem = data[0].toLowerCase();
var rowCategory = data[1].toLowerCase();
var rowOpening = parseFloat(data[2]) || 0;
var rowIssues = parseFloat(data[3]) || 0;
var rowConsumption = parseFloat(data[4]) || 0;
var rowClosing = parseFloat(data[5]) || 0;
```

---

### 8. ExpiryReport.cshtml

**Current Filters:** Store, Days Ahead
**Table Columns:** Item, Category, Store, Quantity, Expiry Date, Days to Expire, Status

**Filters to Add:**
```javascript
// Client-side filters:
1. Category dropdown
2. Status dropdown (Expired, Expiring Soon, Safe)
3. Search (Item name)
4. Date range (From/To Expiry Date)
5. Min/Max Days to Expire
6. Sort by Expiry Date
```

**Column Indices for Filtering:**
```javascript
var rowItem = data[0].toLowerCase();
var rowCategory = data[1].toLowerCase();
var rowStore = data[2].toLowerCase();
var rowQty = parseFloat(data[3]) || 0;
var rowExpiryDate = data[4];
var rowDaysToExpire = parseInt(data[5]) || 0;
var rowStatus = data[6].toLowerCase();
```

---

### 9. AuditReport.cshtml

**Current Filters:** From/To Date, Transaction Type, Store
**Table Columns:** Date, Transaction Type, Entity, User, Action, Store, Details

**Filters to Add:**
```javascript
// Client-side filters:
1. User filter dropdown
2. Action type dropdown (Create, Update, Delete, Approve, Reject)
3. Entity type dropdown (Item, Purchase, Issue, Transfer, etc.)
4. Search (Entity name, Details, User)
5. Sort by Date/User/Action
```

**Column Indices for Filtering:**
```javascript
var rowDate = data[0];
var rowTxnType = data[1].toLowerCase();
var rowEntity = data[2].toLowerCase();
var rowUser = data[3].toLowerCase();
var rowAction = data[4].toLowerCase();
var rowStore = data[5].toLowerCase();
var rowDetails = data[6].toLowerCase();
```

---

## 🎨 COMMON FILTER OPTIONS TO ADD (Based on Table Columns)

Use this checklist for each report:

- ✅ **Date Range** - From/To Date (server-side)
- ✅ **Dropdowns** - Store, Category, Status, Type (add Select2)
- ✅ **Search Field** - Search across 2-3 main text columns
- ✅ **Number Ranges** - Min/Max for Quantity, Value, Amount
- ✅ **Sort Options** - Multiple sort criteria dropdown
- ✅ **Reset Button** - Clear all filters

---

## 🔧 TESTING CHECKLIST

For each enhanced report, test:

1. ✅ Server-side filters reload page with filtered data
2. ✅ Client-side filters work instantly without reload
3. ✅ Search filter works across multiple columns
4. ✅ Min/Max range filters work correctly
5. ✅ Sort options change table order
6. ✅ Reset button clears all filters
7. ✅ Select2 dropdowns have search functionality
8. ✅ DataTables pagination works
9. ✅ Export functions (CSV/PDF) still work
10. ✅ Mobile responsive design maintained

---

## 🚀 QUICK START FOR REMAINING 4 REPORTS

### For Each Report:

1. **Backup the file first:**
   ```bash
   cp IMS.Web/Views/Report/[ReportName].cshtml IMS.Web/Views/Report/[ReportName].cshtml.backup
   ```

2. **Follow the 4-step pattern above**

3. **Test thoroughly**

4. **Commit with descriptive message:**
   ```bash
   git add IMS.Web/Views/Report/[ReportName].cshtml
   git commit -m "enhance: Add comprehensive filters to [ReportName]"
   ```

5. **Push when all 4 are done:**
   ```bash
   git push -u origin claude/add-report-filters-01HwwX6xTrNbk2169cJdn2vB
   ```

---

## 📦 WHAT'S ALREADY DONE & PUSHED

All these files are already enhanced and available on the branch:

- ✅ `IMS.Web/Views/Report/StockReport.cshtml`
- ✅ `IMS.Web/Views/Report/IssueReport.cshtml`
- ✅ `IMS.Web/Views/Report/PurchaseReport.cshtml`
- ✅ `IMS.Web/Views/Report/TransferReport.cshtml`
- ✅ `IMS.Web/Views/Report/LossReport.cshtml`

**You can reference these files as working examples!**

---

## 💡 PRO TIPS

1. **Column Indices:** Always inspect the actual table HTML to get correct column indices (0-based)
2. **Test incrementally:** Add one filter type at a time, test, then add more
3. **Reuse code:** Copy-paste from completed reports and adjust indices
4. **Select2 required:** Make sure Select2 is initialized before DataTable
5. **clearFilters():** Update the action name in the function for each report

---

## 🎯 EXPECTED OUTCOME

When all 9 reports are enhanced:

- **Consistent UX** across all reports
- **Powerful filtering** capabilities for better data analysis
- **Improved productivity** for users
- **Professional appearance** with icons and badges
- **Real-time filtering** for instant results
- **Mobile responsive** design maintained

---

## 📞 NEED HELP?

**Reference these completed files for working examples:**
- Stock Report: Most comprehensive, has 10 filters with all patterns
- Loss Report: Has advanced sorting, value range filters
- Transfer Report: Has store-to-store filtering

**Pattern is proven to work** - 5 reports already successfully enhanced and tested!

---

**Branch:** `claude/add-report-filters-01HwwX6xTrNbk2169cJdn2vB`
**Status:** 5/9 Complete (55%)
**Estimated time for remaining 4:** 30-45 minutes following this guide
