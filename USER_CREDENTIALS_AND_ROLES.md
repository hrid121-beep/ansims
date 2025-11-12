# IMS Ansar & VDP - User Credentials এবং Role-wise কাজকর্ম

## 📋 সব User Credentials (Demo Users)

Database-এ **9 জন demo user** আছে যারা first run-এ automatically seed হয়েছে:

| # | Username | Password | Role | বাংলা নাম |
|---|----------|----------|------|-----------|
| 1 | admin | Admin@123 | Admin | প্রশাসক |
| 2 | director | Director@123 | Director | পরিচালক |
| 3 | finance | Finance@123 | FinanceManager | অর্থ ব্যবস্থাপক |
| 4 | storemanager | Store@123 | StoreManager | ভান্ডার ব্যবস্থাপক |
| 5 | storekeeper | Keeper@123 | StoreKeeper | ভান্ডার রক্ষক |
| 6 | ddg-admin | DDGAdmin@123 | DDGAdmin | উপ-মহাপরিচালক (প্রশাসন) |
| 7 | ad-store | ADStore@123 | ADStore | অতিরিক্ত পরিচালক (ভান্ডার) |
| 8 | dd-store | DDStore@123 | DDStore | উপ-পরিচালক (ভান্ডার) |
| 9 | dd-provision | DDProvision@123 | DDProvision | উপ-পরিচালক (রসদ) |

---

## 🎭 Total 18টি Role আছে System-এ:

### **1️⃣ Admin (প্রশাসক)**
**User:** admin | **Password:** Admin@123

**Permissions:** সব কিছু (Full Access)

**কাজকর্ম:**
- ✅ সব module access করতে পারে
- ✅ User create, edit, delete করতে পারে
- ✅ Role এবং Permission manage করতে পারে
- ✅ System configuration পরিবর্তন করতে পারে
- ✅ সব approval করতে পারে
- ✅ সব report দেখতে এবং export করতে পারে
- ✅ Data backup এবং restore করতে পারে
- ✅ Audit logs দেখতে পারে

---

### **2️⃣ Director (পরিচালক)**
**User:** director | **Password:** Director@123

**কাজকর্ম:**
- ✅ Dashboard এবং সব reports দেখতে পারে
- ✅ Strategic decisions এর জন্য data analysis
- ✅ High-level approvals (বড় ক্রয়, বড় স্থানান্তর)
- ✅ Performance monitoring
- ✅ Budget এবং expenditure tracking
- ⛔ Direct data entry করতে পারে না
- ⛔ User management করতে পারে না

---

### **3️⃣ FinanceManager (অর্থ ব্যবস্থাপক)**
**User:** finance | **Password:** Finance@123

**কাজকর্ম:**
- ✅ Purchase orders approve করা
- ✅ Budget allocation এবং tracking
- ✅ Financial reports দেখা
- ✅ Vendor payment tracking
- ✅ Cost analysis এবং variance reports
- ✅ Purchase requisitions review করা
- ⛔ Store operations করতে পারে না
- ⛔ Stock entry করতে পারে না

---

### **4️⃣ StoreManager (ভান্ডার ব্যবস্থাপক)**
**User:** storemanager | **Password:** Store@123

**কাজকর্ম:**
- ✅ Store management (create, edit stores)
- ✅ Stock levels monitoring
- ✅ Issue এবং Receive approvals
- ✅ Store personnel assignment
- ✅ Inventory reports generation
- ✅ Stock adjustment approve করা
- ✅ Low stock alerts manage করা
- ✅ Physical inventory planning
- ⛔ Purchase orders create করতে পারে না
- ⛔ Financial approvals করতে পারে না

---

### **5️⃣ StoreKeeper (ভান্ডার রক্ষক)**
**User:** storekeeper | **Password:** Keeper@123

**কাজকর্ম:**
- ✅ Daily stock entry (receive items)
- ✅ Issue items to users/departments
- ✅ Stock movement recording
- ✅ Item tracking (barcode scanning)
- ✅ Physical count (cycle counting)
- ✅ Voucher generation (issue/receive)
- ✅ Store requisitions create করা
- ✅ Daily stock reports
- ⛔ Approval করতে পারে না
- ⛔ Store configuration পরিবর্তন করতে পারে না
- ⛔ Purchase orders করতে পারে না

---

### **6️⃣ DDGAdmin (উপ-মহাপরিচালক - প্রশাসন)**
**User:** ddg-admin | **Password:** DDGAdmin@123

**Level:** High-level Management

**কাজকর্ম:**
- ✅ **Purchase Approval (Level 3)** - বড় ক্রয় approve করা (final approval)
- ✅ Write-off approvals (₹10,000+ এর উপরে)
- ✅ High-value stock adjustments approve
- ✅ Policy decisions
- ✅ সব reports access
- ✅ Strategic planning এর জন্য dashboard
- ⛔ Day-to-day operations করে না
- ⛔ Direct stock entry করে না

---

### **7️⃣ ADStore (অতিরিক্ত পরিচালক - ভান্ডার)**
**User:** ad-store | **Password:** ADStore@123

**Level:** Mid-level Management (Store Operations)

**কাজকর্ম:**
- ✅ **Inspection Approval (Level 2)** - Quality inspection approve করা
- ✅ Store transfer approvals
- ✅ Stock adjustment approvals
- ✅ Store performance monitoring
- ✅ Inventory reports review
- ✅ Store SOPs implementation
- ✅ Write-off recommendations
- ⛔ Purchase approvals করতে পারে না (DDGAdmin করে)
- ⛔ Direct stock entry করে না

---

### **8️⃣ DDStore (উপ-পরিচালক - ভান্ডার)**
**User:** dd-store | **Password:** DDStore@123

**Level:** Mid-level Management (Store Operations)

**কাজকর্ম:**
- ✅ **Inspection Approval (Level 2)** - ADStore এর সাথে jointly
- ✅ Central Store operations supervision
- ✅ Quality control monitoring
- ✅ Stock level reviews
- ✅ Physical inventory coordination
- ✅ Store audit participation
- ⛔ Final purchase approval করতে পারে না
- ⛔ Provision store operations করে না

---

### **9️⃣ DDProvision (উপ-পরিচালক - রসদ)**
**User:** dd-provision | **Password:** DDProvision@123

**Level:** Mid-level Management (Provision Operations)

**কাজকর্ম:**
- ✅ **Provision Store Issue Approvals** - Provision store থেকে issue approve করা (mandatory)
- ✅ Allotment letter approvals
- ✅ Distribution planning
- ✅ Battalion/Range/Zila requisitions review
- ✅ Distribution reports
- ✅ Stock allocation করা
- ⛔ Central Store operations করে না
- ⛔ Purchase approvals করতে পারে না

---

### **🔟 DepartmentHead (বিভাগীয় প্রধান)**
**User:** ❌ না থাকলে create করতে হবে

**কাজকর্ম:**
- ✅ Requisition approve করা
- ✅ Department stock needs planning
- ✅ Budget requests
- ✅ Department reports
- ✅ Staff supervision

---

### **1️⃣1️⃣ Operator (অপারেটর)**
**User:** ❌ না থাকলে create করতে হবে

**কাজকর্ম:**
- ✅ Data entry করা
- ✅ Basic transactions recording
- ✅ Item receiving
- ✅ Basic reports generate করা
- ⛔ Approval করতে পারে না
- ⛔ Configuration পরিবর্তন করতে পারে না

---

### **1️⃣2️⃣ Auditor (নিরীক্ষক)**
**User:** ❌ না থাকলে create করতে হবে

**কাজকর্ম:**
- ✅ সব records read-only access
- ✅ Audit reports generate করা
- ✅ Variance analysis
- ✅ Physical verification
- ✅ Discrepancy reporting
- ⛔ কোন data modify করতে পারে না
- ⛔ Transactions করতে পারে না

---

### **1️⃣3️⃣ Viewer (দর্শক/পর্যবেক্ষক)**
**User:** ❌ না থাকলে create করতে হবে

**কাজকর্ম:**
- ✅ Dashboard দেখা (read-only)
- ✅ Reports দেখা
- ✅ Stock levels দেখা
- ⛔ কোন data entry করতে পারে না
- ⛔ কোন approval করতে পারে না

---

### **1️⃣4️⃣ BattalionCommander (ব্যাটালিয়ন কমান্ডার)**
**User:** ❌ না থাকলে create করতে হবে

**কাজকর্ম:**
- ✅ Battalion store oversight
- ✅ Battalion requisitions approve
- ✅ Personnel item distribution
- ✅ Battalion stock reports
- ✅ Local procurement coordination

---

### **1️⃣5️⃣ RangeCommander (রেঞ্জ কমান্ডার)**
**User:** ❌ না থাকলে create করতে হবে

**কাজকর্ম:**
- ✅ Range-level store management
- ✅ Multiple battalion coordination
- ✅ Range requisitions
- ✅ Range stock distribution
- ✅ Range reports

---

### **1️⃣6️⃣ ZilaCommander (জেলা কমান্ডার)**
**User:** ❌ না থাকলে create করতে হবে

**কাজকর্ম:**
- ✅ Zila-level operations
- ✅ Zila store management
- ✅ Local distribution
- ✅ Zila reports
- ✅ Upazila coordination

---

### **1️⃣7️⃣ StorekeeperCentral (কেন্দ্রীয় ভান্ডার রক্ষক)**
**User:** ❌ না থাকলে create করতে হবে

**Specialized Role:** শুধু Central Store-এর জন্য

**কাজকর্ম:**
- ✅ Central Store stock receiving
- ✅ Quality inspection coordination
- ✅ Transfer to Provision Store
- ✅ Central Store reports
- ⛔ Provision Store operations করতে পারে না
- ⛔ Direct issue করতে পারে না (শুধু transfer)

---

### **1️⃣8️⃣ StorekeeperProvision (রসদ ভান্ডার রক্ষক)**
**User:** ❌ না থাকলে create করতে হবে

**Specialized Role:** শুধু Provision Store-এর জন্য

**কাজকর্ম:**
- ✅ Provision Store item issue
- ✅ Allotment letter processing
- ✅ Battalion/Range/Zila distribution
- ✅ Mandatory documents verification
- ✅ Provision Store reports
- ⛔ Central Store operations করতে পারে না
- ⛔ Direct receiving করতে পারে না

---

## 🔐 Important Security Notes:

1. **Default passwords পরিবর্তন করুন:**
   - প্রথম login এর পর সব users কে password change করতে বলুন
   - Strong password policy enforce করুন

2. **Production Environment:**
   - এই demo users শুধু testing এর জন্য
   - Production-এ real users create করুন
   - Demo users disable করুন

3. **Permission Structure:**
   - Permission-role mapping `RolePermission` table-এ stored
   - 475+ granular permissions আছে
   - Custom permissions define করা যায়

---

## 📊 Approval Workflow:

### **Purchase Approval Levels:**
1. **Level 1:** Requisition Creator
2. **Level 2:** StoreManager / DepartmentHead
3. **Level 3:** DDGAdmin (Final Approval)

### **Inspection Approval:**
1. **Level 1:** StoreKeeper (receives items)
2. **Level 2:** ADStore / DDStore (Quality Inspection)

### **Provision Issue Approval:**
1. **Mandatory:** DDProvision approval required
2. **Documents:** Allotment Letter required

### **Write-Off Approval:**
- **< ₹10,000:** StoreManager
- **≥ ₹10,000:** DDGAdmin + ADStore/DDStore

---

## 🎯 Testing এর জন্য:

### **Basic Operations Test:**
1. **Login as:** storekeeper (Keeper@123)
2. **Test:** Stock entry, issue, receive

### **Approval Test:**
1. **Login as:** ddg-admin (DDGAdmin@123)
2. **Test:** Purchase approval

### **Provision Store Test:**
1. **Login as:** dd-provision (DDProvision@123)
2. **Test:** Allotment letter, issue approval

### **Reports Test:**
1. **Login as:** director (Director@123)
2. **Test:** View all reports

---

## 📝 নতুন User Create করার জন্য:

1. **Admin হিসেবে login করুন** (admin / Admin@123)
2. **Admin → Users** এ যান
3. **Create New User** click করুন
4. **Form পূরণ করুন:**
   - Username
   - Email
   - Password
   - Role selection
   - Store assignment (if needed)
5. **Save** করুন

---

## 🆘 যদি Password ভুলে যান:

Database-এ গিয়ে AspNetUsers table check করুন অথবা Admin user দিয়ে reset করুন।
