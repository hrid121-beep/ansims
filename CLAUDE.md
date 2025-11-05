# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Ansar VDP Inventory Management System (IMS)** - A comprehensive inventory management system for the Bangladesh Ansar & VDP organization. Built with ASP.NET Core 9.0 MVC, Entity Framework Core, and SQL Server.

## Architecture

This project follows **Clean Architecture** with four distinct layers:

### 1. IMS.Domain (Core Layer)
- Contains all domain entities, enums, and business rules
- No dependencies on other layers
- Key entities: Item, Store, User, Battalion, Range, Zila, Upazila, Union
- Entity base class: `BaseEntity` with audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsActive)

### 2. IMS.Application (Business Logic Layer)
- Dependencies: IMS.Domain only
- Contains:
  - **Services/**: All business logic services (60+ services)
  - **Services/BackgroundServices/**: Hosted services for alerts and notifications
  - **Interfaces/**: Service contracts (in Interfaces.cs)
  - **DTOs/**: Data transfer objects
  - **Helpers/**: Utility classes
  - **Mappings/**: AutoMapper profiles (currently disabled - see Known Issues)

### 3. IMS.Infrastructure (Data Access Layer)
- Dependencies: IMS.Application, IMS.Domain
- Contains:
  - **ApplicationDbContext.cs**: Main EF Core context (100+ DbSets)
  - **Repositories/**: Generic repository pattern with UnitOfWork
  - **Migrations/**: EF Core database migrations
  - Uses ASP.NET Core Identity for authentication

### 4. IMS.Web (Presentation Layer)
- Dependencies: IMS.Infrastructure (transitive to all layers)
- ASP.NET Core MVC application
- Contains:
  - **Controllers/**: 46 MVC controllers
  - **Views/**: Razor views
  - **Models/**: View models
  - **Attributes/**: Custom attributes and extension methods (includes UserContext, PermissionRequirement, AuthorizationExtensions)
  - **wwwroot/**: Static files (AdminLTE theme)

## Database Setup

**Connection String Location**: `IMS.Web/appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=ADMIN\\SQLEXPRESS;Database=ansvdp_ims;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"
}
```

**Note**: Update server name and database name according to your SQL Server instance.

### Database Commands

**Apply migrations:**
```bash
dotnet ef database update --project IMS.Infrastructure --startup-project IMS.Web
```

**Create new migration:**
```bash
dotnet ef migrations add MigrationName --project IMS.Infrastructure --startup-project IMS.Web
```

**Remove last migration:**
```bash
dotnet ef migrations remove --project IMS.Infrastructure --startup-project IMS.Web
```

**List migrations:**
```bash
dotnet ef migrations list --project IMS.Infrastructure --startup-project IMS.Web
```

**Note**: The DbContext factory is in `IMS.Infrastructure/ApplicationDbContextFactory.cs`

## Building and Running

### Build the solution:
```bash
dotnet build IMS.sln
```

### Run the web application:
```bash
dotnet run --project IMS.Web
```

### Run with hot reload (watch mode):
```bash
dotnet watch --project IMS.Web
```

### Clean build artifacts:
```bash
dotnet clean IMS.sln
```

### Restore NuGet packages:
```bash
dotnet restore IMS.sln
```

### Build in Release mode:
```bash
dotnet build IMS.sln --configuration Release
```

### Run tests (when test project exists):
```bash
dotnet test IMS.sln
```

### PowerShell-Specific Commands (Windows)

**Check for build errors/warnings:**
```powershell
dotnet build IMS.sln | Select-String -Pattern "error|warning"
```

**List all controllers:**
```powershell
Get-ChildItem -Path "IMS.Web\Controllers" -Filter "*.cs" | Select-Object Name
```

**Find migrations:**
```powershell
Get-ChildItem -Path "IMS.Infrastructure\Migrations" -Filter "*.cs"
```

**Monitor application logs in real-time:**
```powershell
dotnet run --project IMS.Web | Select-String -Pattern "error|warning|information"
```

## Key Business Concepts

### Organization Hierarchy (Ansar & VDP Specific)
The system follows Bangladesh Ansar & VDP organizational structure:
- **Headquarters** → **Range** → **Battalion** → **Zila** → **Upazila** → **Union**
- Each level can have stores and users assigned to it
- Transfers and issues follow this hierarchy

### Store Types
Three main store types (seeded on startup):
1. **Central Store** (CENTRAL) - Initial receiving point, requires inspection
2. **Provision Store** (PROVISION) - Issues items, requires mandatory documents
3. **Other Store** (OTHER) - General purpose (Battalion/Range/Zila stores)

Store types have flags: `RequiresMandatoryDocuments`, `AllowDirectIssue`, `AllowTransfer`

### Personnel Item Lifecycle
- Items can be issued to Ansar or VDP personnel
- Separate lifespan tracking: `AnsarLifeSpanMonths`, `VDPLifeSpanMonths`
- Separate alert periods: `AnsarAlertBeforeDays`, `VDPAlertBeforeDays`
- Entitlement quantities: `AnsarEntitlementQuantity`, `VDPEntitlementQuantity`

### Transaction Flow
1. **Purchase** → Quality inspection → Store entry (Central Store)
2. **Transfer** → Central Store to Provision Store
3. **Issue** → Provision Store to Battalion/Range/Zila (requires allotment letter if from Provision Store)
4. **Receive** → Items received back
5. **Return** → Items returned with condition check

### Approval Workflow
- Multi-level approval system (Level 1-5)
- Role-based approvers: DDGAdmin, ADStore, DDStore, DDProvision
- Purchase approval at Level 3 (DDG Admin)
- Inspection approval at Level 2 (AD/DD Store)
- Provision store issues require DD Provision approval

## User Roles and Default Credentials

The system seeds demo users on first run (see `Program.cs` lines 293-358):

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |
| director | Director@123 | Director |
| finance | Finance@123 | FinanceManager |
| storemanager | Store@123 | StoreManager |
| storekeeper | Keeper@123 | StoreKeeper |
| ddg-admin | DDGAdmin@123 | DDGAdmin |
| ad-store | ADStore@123 | ADStore |
| dd-store | DDStore@123 | DDStore |
| dd-provision | DDProvision@123 | DDProvision |

All 18 roles are seeded in `Program.cs` lines 252-274:
- Admin, Director, FinanceManager, DepartmentHead, StoreManager, StoreKeeper
- Operator, Auditor, Viewer
- BattalionCommander, RangeCommander, ZilaCommander
- DDGAdmin, ADStore, DDStore, DDProvision
- StorekeeperCentral, StorekeeperProvision
- ZilaOfficer, UpazilaOfficer

## Permission System

- Enum-based permissions defined in `IMS.Domain/Enums.cs` (Permission enum)
- 475+ granular permissions (0-3100 range)
- Custom authorization:
  - `PermissionRequirement` - Single permission check
  - `MultiplePermissionsRequirement` - Multiple permissions (AND/OR logic)
  - `StoreBasedPermissionRequirement` - Store-scoped permissions
  - `ValueBasedPermissionRequirement` - Value threshold checks
- Authorization setup in `IMS.Web/Attributes/AuthorizationExtensions.cs`
- Permission-role mapping stored in `RolePermission` table

## Repository Pattern (Unit of Work)

**Access repositories through IUnitOfWork:**
```csharp
public class SomeService
{
    private readonly IUnitOfWork _unitOfWork;

    public SomeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Item> GetItemAsync(int id)
    {
        return await _unitOfWork.Items.GetByIdAsync(id);
    }

    public async Task SaveAsync()
    {
        await _unitOfWork.CompleteAsync();
    }
}
```

**Available repository methods** (from `IRepository<T>`):
- `GetByIdAsync(id)`, `GetAllAsync()`, `FindAsync(predicate)`
- `AddAsync(entity)`, `AddRangeAsync(entities)`
- `Update(entity)`, `UpdateRange(entities)`
- `Remove(entity)`, `RemoveRange(entities)`

**Transaction support:**
```csharp
await _unitOfWork.BeginTransactionAsync();
try
{
    // operations
    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    await _unitOfWork.RollbackTransactionAsync();
}
```

## Background Services

Five hosted services run continuously (registered in `Program.cs` lines 199-203):

1. **StockAlertBackgroundService** - Monitors low stock levels
2. **ExpiryAlertBackgroundService** - Tracks item expiry
3. **DailySummaryBackgroundService** - Generates daily reports
4. **ApprovalEscalationBackgroundService** - Escalates pending approvals
5. **LifeSpanAlertBackgroundService** - Alerts for personnel item lifespan

Located in: `IMS.Application/Services/BackgroundServices/`

These services are registered with `AddHostedService<T>()` and run on configured intervals (see `appsettings.json` for interval settings).

## Known Issues

### AutoMapper Disabled
AutoMapper is currently disabled in `Program.cs` (lines 60-82) due to version 15.x compatibility issues. The Return module uses manual mapping instead.

**TODO**: Fix AutoMapper configuration or downgrade to compatible version

### Nullable Reference Types
`<Nullable>disable</Nullable>` is set in all `.csproj` files. Consider enabling nullable reference types for better null safety.

### EF Core Tools Version
The project uses .NET 9.0 and Entity Framework Core 9.0.7, but EF Core Tools is version 7.0.5. To update:
```bash
dotnet tool update --global dotnet-ef
```
Check version with: `dotnet ef --version`

## Important Patterns

### Service Registration
All services are registered with Scoped lifetime in `Program.cs` (lines 88-198). Services are organized by category:
- Core Business Services
- Store & Stock Management
- Transaction Services (Purchase, Issue, Receive, Transfer, Return)
- Requisition & Workflow
- Alert & Notification Services
- Reporting Services
- User & Permission Services

All services follow the pattern: `builder.Services.AddScoped<IServiceName, ServiceImplementation>();`

### User Context
Use `IUserContext` service to get current user information:
```csharp
var userId = await _userContext.GetUserIdAsync();
var userName = await _userContext.GetUserNameAsync();
var storeId = await _userContext.GetUserStoreIdAsync();
```

### File Uploads
Files are stored in `wwwroot/uploads/` with subdirectories:
- Item images: `wwwroot/uploads/items/`
- Max file size: 10 MB (10485760 bytes)
- Allowed extensions: .jpg, .jpeg, .png, .pdf, .doc, .docx
- Configuration in `appsettings.json` under `FileService`

### Barcode/QR Code Generation
Services available:
- `IBarcodeService` - Generate barcodes and QR codes
- Libraries: BarcodeLib, QRCoder, ZXing.Net
- Prefix for barcodes: "AVDP" (configurable in appsettings.json)

## Database Seeding

On first run, the application automatically seeds:
1. **Roles** - All application roles
2. **Users** - Demo users for each role
3. **Permissions** - Default role permissions via `RolePermissionService.InitializeDefaultPermissionsAsync()`
4. **Store Types** - Three default store types

See: `Program.cs` lines 237-368 and method `SeedStoreTypes` (lines 374-474)

## Common Development Patterns

### Creating a New Entity
1. Add entity class to `IMS.Domain/Entities.cs` (inherit from `BaseEntity`)
2. Add DbSet to `ApplicationDbContext.cs`
3. Add repository property to `UnitOfWork.cs` and `IUnitOfWork` interface
4. Create service interface in `IMS.Application/Interfaces/Interfaces.cs`
5. Implement service in `IMS.Application/Services/`
6. Register service in `Program.cs`
7. Create migration: `dotnet ef migrations add AddNewEntity --project IMS.Infrastructure --startup-project IMS.Web`

### Adding a New Permission
1. Add to `Permission` enum in `IMS.Domain/Enums.cs`
2. Update `RolePermissionService.InitializeDefaultPermissionsAsync()` to assign to roles
3. Use in controller: `[Authorize(Policy = "PolicyName")]` or `[PermissionAuthorize(Permission.YourPermission)]`

### Stock Movement Tracking
Every transaction that affects stock (Purchase, Issue, Transfer, Return, etc.) creates a `StockMovement` record with:
- `StockMovementType` enum value
- Source/destination stores
- Item quantities
- Reference to source transaction

## Technology Stack

- **.NET 9.0** - Latest version (Note: LTS versions are even-numbered like 8.0)
- **Entity Framework Core 9.0.7** - ORM
- **SQL Server** - Database
- **ASP.NET Core Identity 9.0.7** - Authentication/Authorization
- **AutoMapper 15.0.1** (disabled)
- **AdminLTE 3** - UI theme
- **Barcode/QR**: BarcodeLib, QRCoder, ZXing.Net
- **PDF/Excel**: iTextSharp, EPPlus, ClosedXML
- **Image Processing**: SixLabors.ImageSharp, System.Drawing.Common

## Application Configuration

Key configuration sections in `appsettings.json`:

### Email Settings
```json
"EmailSettings": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "your-email@gmail.com",
  "SmtpPassword": "your-app-specific-password",
  "FromEmail": "noreply@ansar-vdp.gov.bd",
  "EnableSsl": true
}
```

### App Settings
- `BarcodePrefix`: "AVDP" - Prefix for all generated barcodes
- `DefaultPageSize`: 20 - Default pagination size
- `MaxPageSize`: 100 - Maximum items per page
- `SessionTimeout`: 30 minutes
- `LowStockAlertEnabled`: true - Enable low stock monitoring
- `LowStockCheckInterval`: 24 hours - How often to check stock levels

### EPPlus License
```json
"EPPlus": {
  "ExcelPackage": {
    "LicenseContext": "NonCommercial"
  }
}
```
**Note**: EPPlus is configured for non-commercial use. Update if deploying commercially.

## Smart Stock Adjustment (Intelligent Merge)

**New Feature**: Stock Adjustment module now intelligently handles both simple adjustments and write-offs.

### How It Works

The system automatically detects the reason for adjustment and triggers appropriate workflows:

#### Count Corrections (No WriteOff)
```
Reasons: Physical Count Mismatch, Data Entry Error, System Reconciliation, Found Items
→ Simple stock adjustment
→ No WriteOff created
→ Standard approval workflow
```

#### Damage/Loss/Expiry (Auto WriteOff)
```
Reasons: Damaged, Expired, Lost, Stolen, Obsolete, Quality Issues, Fire/Water Damage, Natural Disaster
→ Stock adjustment created
→ WriteOff automatically created
→ If value ≥ ₹10,000: Requires multi-level approval
→ If value < ₹10,000: Standard processing
```

### Technical Implementation

**Service Layer** (`StockAdjustmentService.cs:502-636`):
- `ShouldTriggerWriteOff()` - Checks if reason matches write-off triggers
- `CreateWriteOffFromAdjustmentAsync()` - Auto-creates WriteOff with proper linking
- `NotifyApproversAsync()` - Alerts approvers for high-value write-offs

**View Layer** (`Views/StockAdjustment/Create.cshtml`):
- Categorized reason dropdown with visual indicators
- Real-time WriteOff detection alert
- Value threshold warning (₹10,000)

### Benefits

1. **Single Entry Point** - Users don't need to decide between adjustment vs write-off
2. **Smart Routing** - System automatically creates write-off when needed
3. **Transparent** - Users see exactly what will happen before submitting
4. **Audit Trail** - Both adjustment and write-off records linked via ReferenceNo
5. **Better UX** - Clear categorization with emoji indicators

### Write-Off Triggers

The following reasons automatically create WriteOff:
- Damaged
- Expired
- Lost
- Stolen / Theft
- Obsolete
- Quality Issues
- Fire Damage
- Water Damage
- Natural Disaster

**Threshold**: WriteOffs ≥ ₹10,000 require DDGAdmin/DDStore/ADStore approval

## Testing

**Note**: No test projects are currently configured. Consider adding:
- `IMS.Tests` - Unit tests
- `IMS.IntegrationTests` - Integration tests

## Troubleshooting

### Build Errors

**Issue**: "The type or namespace name 'X' could not be found"
- **Solution**: Run `dotnet restore IMS.sln` to restore NuGet packages

**Issue**: Migration fails with "A network-related or instance-specific error"
- **Solution**: Verify SQL Server is running and connection string in `appsettings.json` matches your instance

### Database Issues

**Issue**: Database doesn't exist
- **Solution**: Run `dotnet ef database update --project IMS.Infrastructure --startup-project IMS.Web`

**Issue**: Migrations out of sync
- **Solution**:
  ```bash
  # List all migrations
  dotnet ef migrations list --project IMS.Infrastructure --startup-project IMS.Web
  # Remove last migration if needed
  dotnet ef migrations remove --project IMS.Infrastructure --startup-project IMS.Web
  ```

**Issue**: EF Core Tools version mismatch
- **Solution**: Update tools to match project version:
  ```bash
  dotnet tool update --global dotnet-ef
  ```

### Runtime Issues

**Issue**: "Unable to resolve service" or dependency injection errors
- **Solution**: Check that service is registered in `Program.cs` (lines 88-196)

**Issue**: Authorization/Permission denied
- **Solution**:
  - Verify user has required role assigned
  - Check `RolePermission` table for role-permission mappings
  - Ensure `InitializeDefaultPermissionsAsync()` was called during seeding

**Issue**: AutoMapper errors
- **Solution**: AutoMapper is disabled. Use manual mapping or fix AutoMapper configuration (see Known Issues)

### Performance Issues

**Issue**: Background services consuming too much CPU/memory
- **Solution**: Adjust intervals in `appsettings.json` under `AppSettings` section:
  - `LowStockCheckInterval`: Default 24 hours
  - Other service intervals may be hardcoded in respective service classes

## Environment Setup

### Prerequisites
- .NET 9.0 SDK (check with `dotnet --version`)
- SQL Server (Express or higher)
- Visual Studio 2022 (17.0+) or VS Code with C# extension
- Entity Framework Core Tools (check with `dotnet ef --version`)

### First-Time Setup
1. Clone the repository
2. Update connection string in `IMS.Web/appsettings.json`
3. Install/update EF Core tools: `dotnet tool update --global dotnet-ef`
4. Restore packages: `dotnet restore IMS.sln`
5. Apply migrations: `dotnet ef database update --project IMS.Infrastructure --startup-project IMS.Web`
6. Build solution: `dotnet build IMS.sln`
7. Run application: `dotnet run --project IMS.Web`
8. Login with default admin credentials (see User Roles section)
