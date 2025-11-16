using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Application.Services.BackgroundServices;
using IMS.Domain.Entities;
using IMS.Infrastructure.Data;
using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using IMS.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static StoreTypeConversionHelper;
using OfficeOpenXml;
using Range = IMS.Domain.Entities.Range;

// Set EPPlus License Context
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Keep only essential settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddRoles<IdentityRole>(); // This is essential for roles to work

builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomUserClaimsPrincipalFactory>();
// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add memory cache
builder.Services.AddMemoryCache();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// AutoMapper Configuration - DISABLED (version 15.x compatibility issues)
// Return module now uses manual mapping instead
// TODO: Fix AutoMapper configuration or downgrade to compatible version
/*
builder.Services.AddScoped<AutoMapper.IMapper>(sp =>
{
    var configExpression = new AutoMapper.MapperConfigurationExpression();
    configExpression.AddMaps(AppDomain.CurrentDomain.GetAssemblies());

    // Find all Profile classes
    var profiles = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => typeof(AutoMapper.Profile).IsAssignableFrom(t) && !t.IsAbstract);

    foreach (var profileType in profiles)
    {
        configExpression.AddProfile(Activator.CreateInstance(profileType) as AutoMapper.Profile);
    }

    var config = new AutoMapper.Mapper(configExpression);
    return config;
});
*/

// Configure Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ========================================
// REGISTER REPOSITORIES AND UNIT OF WORK
// ========================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ========================================
// CORE BUSINESS SERVICES
// ========================================
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IItemModelService, ItemModelService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IAllotmentLetterService, AllotmentLetterService>();
builder.Services.AddScoped<ISignatoryPresetService, SignatoryPresetService>();

// ========================================
// STORE & STOCK MANAGEMENT SERVICES
// ========================================
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockAlertService, StockAlertService>();
builder.Services.AddScoped<IStoreItemService, StoreItemService>();
builder.Services.AddScoped<IStockEntryService, StockEntryService>();
builder.Services.AddScoped<IStoreTypeService, StoreTypeService>();
builder.Services.AddScoped<IUserStoreService, UserStoreService>();
builder.Services.AddScoped<IStoreConfigurationService, StoreConfigurationService>();
builder.Services.AddScoped<ILedgerBookService, LedgerBookService>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();
builder.Services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();

// ========================================
// TRANSACTION SERVICES
// ========================================
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<IReceiveService, ReceiveService>();
// Add these service registrations
builder.Services.AddScoped<IPersonnelItemLifeService, PersonnelItemLifeService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<IReturnService, ReturnService>();
builder.Services.AddScoped<ISignatureService, SignatureService>();

// ========================================
// REQUISITION & WORKFLOW SERVICES
// ========================================
builder.Services.AddScoped<IRequisitionService, RequisitionService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();

// ========================================
// LOSS & DAMAGE MANAGEMENT SERVICES
// ========================================
builder.Services.AddScoped<IDamageService, DamageService>();
builder.Services.AddScoped<IWriteOffService, WriteOffService>();

// ========================================
// TRACKING & MONITORING SERVICES
// ========================================
builder.Services.AddScoped<IBarcodeService, BarcodeService>();
builder.Services.AddScoped<IExpiryTrackingService, ExpiryTrackingService>();
builder.Services.AddScoped<IBatchTrackingService, BatchTrackingService>();
builder.Services.AddScoped<IDigitalSignatureService, DigitalSignatureService>();

// ========================================
// INVENTORY OPERATION SERVICES
// ========================================
builder.Services.AddScoped<IPhysicalInventoryService, PhysicalInventoryService>();
builder.Services.AddScoped<IInventoryCycleCountService, InventoryCycleCountService>();
builder.Services.AddScoped<ICycleCountSchedulingService, CycleCountSchedulingService>();

// ========================================
// NOTIFICATION & COMMUNICATION SERVICES
// ========================================
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ========================================
// REPORTING & ANALYTICS SERVICES
// ========================================
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();

// ========================================
// ORGANIZATION HIERARCHY SERVICES (Ansar & VDP Specific)
// ========================================
builder.Services.AddScoped<IBattalionService, BattalionService>();
builder.Services.AddScoped<IRangeService, RangeService>();
builder.Services.AddScoped<IZilaService, ZilaService>();
builder.Services.AddScoped<IUpazilaService, UpazilaService>();
builder.Services.AddScoped<IUnionService, UnionService>();

// ========================================
// UTILITY & HELPER SERVICES
// ========================================
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IFileService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileService(env.WebRootPath, env.ContentRootPath);
});

// ========================================
// BACKGROUND SERVICES
// ========================================
builder.Services.AddHostedService<StockAlertBackgroundService>();
builder.Services.AddHostedService<ExpiryAlertBackgroundService>();
builder.Services.AddHostedService<DailySummaryBackgroundService>();
builder.Services.AddHostedService<ApprovalEscalationBackgroundService>();
builder.Services.AddHostedService<LifeSpanAlertBackgroundService>();

// ========================================
// AUTHORIZATION & POLICIES
// ========================================
builder.Services.AddPermissionBasedAuthorization();
builder.Services.AddRoleBasedPolicies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add session support
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Seed data and create admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var rolePermissionService = services.GetRequiredService<IRolePermissionService>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Starting database initialization...");

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Create roles
        string[] roles = {
            "Admin",
            "Director",
            "FinanceManager",
            "DepartmentHead",
            "StoreManager",
            "StoreKeeper",
            "Operator",
            "Auditor",
            "Viewer",
            "BattalionCommander",
            "RangeCommander",
            "ZilaCommander",
            // ADD new roles
            "DDGAdmin",
            "ADStore",
            "DDStore",
            "DDProvision",
            "StorekeeperCentral",
            "StorekeeperProvision",
            "ZilaOfficer",
            "UpazilaOfficer"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation($"Created role: {role}");
            }
        }

        // Initialize default permissions for roles
        await rolePermissionService.InitializeDefaultPermissionsAsync();

        logger.LogInformation("Initialized default permissions for all roles");

        await SeedStoreTypes(context, logger);
        await SeedSignatoryPresets(context, logger);

        // TODO: Seed comprehensive test data for all tables (commented out due to compilation errors)
        // var dataSeederService = services.GetRequiredService<IDataSeederService>();
        // await dataSeederService.SeedComprehensiveDataAsync();
        // logger.LogInformation("Comprehensive test data seeding completed");

        // Create admin user with USERNAME
        var adminUsername = "admin";
        if (await userManager.FindByNameAsync(adminUsername) == null)
        {
            var adminUser = new User
            {
                UserName = adminUsername,  // CHANGED: username is now separate
                Email = "admin@ansar-vdp.gov.bd",  // Email is optional
                FullName = "System Administrator",
                Designation = "IT Administrator",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation($"Created admin user: {adminUsername}");
            }
        }

        // Create demo users with USERNAME
        var demoUsers = new[]
        {
            new { Username = "director", Email = "director@ansar-vdp.gov.bd", Password = "Director@123", Role = "Director", FullName = "Director General", Designation = "Director General" },
            new { Username = "finance", Email = "finance@ansar-vdp.gov.bd", Password = "Finance@123", Role = "FinanceManager", FullName = "Finance Manager", Designation = "Finance Manager" },
            new { Username = "storemanager", Email = "storemanager@ansar-vdp.gov.bd", Password = "Store@123", Role = "StoreManager", FullName = "Store Manager", Designation = "Store Manager" },
            new { Username = "storekeeper", Email = "storekeeper@ansar-vdp.gov.bd", Password = "Keeper@123", Role = "StoreKeeper", FullName = "Store Keeper", Designation = "Store Keeper" },
            new { Username = "operator", Email = "operator@ansar-vdp.gov.bd", Password = "Operator@123", Role = "Operator", FullName = "Store Operator", Designation = "Operator" },
            new { Username = "auditor", Email = "auditor@ansar-vdp.gov.bd", Password = "Auditor@123", Role = "Auditor", FullName = "Internal Auditor", Designation = "Auditor" },
            new { Username = "battalion", Email = "battalion@ansar-vdp.gov.bd", Password = "Battalion@123", Role = "BattalionCommander", FullName = "Battalion Commander", Designation = "Battalion Commander" },
            new { Username = "ddg-admin", Email = "ddg-admin@ansar-vdp.gov.bd", Password = "DDGAdmin@123", Role = "DDGAdmin", FullName = "DDG Admin", Designation = "Deputy Director General (Admin)" },
            new { Username = "ad-store", Email = "ad-store@ansar-vdp.gov.bd", Password = "ADStore@123", Role = "ADStore", FullName = "AD Store", Designation = "Assistant Director (Store)" },
            new { Username = "dd-store", Email = "dd-store@ansar-vdp.gov.bd", Password = "DDStore@123", Role = "DDStore", FullName = "DD Store", Designation = "Deputy Director (Store)" },
            new { Username = "dd-provision", Email = "dd-provision@ansar-vdp.gov.bd", Password = "DDProvision@123", Role = "DDProvision", FullName = "DD Provision", Designation = "Deputy Director (Provision)" },
            new { Username = "storekeeper-central", Email = "storekeeper-central@ansar-vdp.gov.bd", Password = "Central@123", Role = "StorekeeperCentral", FullName = "Storekeeper Central", Designation = "Storekeeper (Central Store)" },
            new { Username = "storekeeper-provision", Email = "storekeeper-provision@ansar-vdp.gov.bd", Password = "Provision@123", Role = "StorekeeperProvision", FullName = "Storekeeper Provision", Designation = "Storekeeper (Provision Store)" },
            new { Username = "range-commander", Email = "range-commander@ansar-vdp.gov.bd", Password = "Range@123", Role = "RangeCommander", FullName = "Range Commander", Designation = "Range DIG" },
            new { Username = "zila-officer", Email = "zila-officer@ansar-vdp.gov.bd", Password = "Zila@123", Role = "ZilaOfficer", FullName = "Zila Officer", Designation = "District Commandant" },
            new { Username = "upazila-officer", Email = "upazila-officer@ansar-vdp.gov.bd", Password = "Upazila@123", Role = "UpazilaOfficer", FullName = "Upazila Officer", Designation = "Upazila Ansar & VDP Officer" }
        };

        foreach (var demoUser in demoUsers)
        {
            if (await userManager.FindByNameAsync(demoUser.Username) == null)
            {
                var user = new User
                {
                    UserName = demoUser.Username,  // CHANGED: username is primary
                    Email = demoUser.Email,        // CHANGED: email is separate/optional
                    FullName = demoUser.FullName,
                    Designation = demoUser.Designation,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, demoUser.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, demoUser.Role);
                    logger.LogInformation($"Created demo user: {demoUser.Username}");
                }
            }
        }

        logger.LogInformation("Default approval workflows created");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
        throw;
    }
}

app.Run();


// ✅ ADD THIS METHOD at the bottom of Program.cs (after app.Run())
static async Task SeedStoreTypes(ApplicationDbContext context, ILogger logger)
{
    try
    {
        if (context.StoreTypes.Any())
        {
            logger.LogInformation("StoreTypes already exist. Updating icons and flags...");

            // Update existing records with proper Font Awesome icons
            var central = await context.StoreTypes.FirstOrDefaultAsync(st => st.Code == "CENTRAL");
            if (central != null && (string.IsNullOrEmpty(central.Icon) || central.Icon == "fa-warehouse"))
            {
                central.Icon = "fas fa-warehouse";
                context.StoreTypes.Update(central);
            }

            var provision = await context.StoreTypes.FirstOrDefaultAsync(st => st.Code == "PROVISION");
            if (provision != null)
            {
                provision.RequiresMandatoryDocuments = true;
                if (string.IsNullOrEmpty(provision.Icon) || provision.Icon == "fa-dolly")
                {
                    provision.Icon = "fas fa-dolly";
                }
                context.StoreTypes.Update(provision);
            }

            var other = await context.StoreTypes.FirstOrDefaultAsync(st => st.Code == "OTHER");
            if (other != null && (string.IsNullOrEmpty(other.Icon) || other.Icon == "fa-store"))
            {
                other.Icon = "fas fa-store";
                context.StoreTypes.Update(other);
            }

            await context.SaveChangesAsync();
            logger.LogInformation("Updated StoreType icons with proper Font Awesome classes");

            return;
        }

        logger.LogInformation("Seeding StoreTypes...");

        var storeTypes = new[]
        {
            new StoreType
            {
                Name = "Central Store",
                NameBn = "কেন্দ্রীয় স্টোর",
                Code = "CENTRAL",
                Description = "All items first received here for inspection",
                IsMainStore = true,
                AllowDirectIssue = false,
                AllowTransfer = true,
                RequiresMandatoryDocuments = false,
                DisplayOrder = 1,
                MaxCapacity = 0,
                RequiresTemperatureControl = false,
                RequiresSecurityClearance = false,
                IsActive = true,
                Icon = "fas fa-warehouse",
                Color = "#3498db",
                DefaultManagerRole = "StoreKeeper",
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            },
            new StoreType
            {
                Name = "Provision Store",
                NameBn = "প্রভিশন স্টোর",
                Code = "PROVISION",
                Description = "Items issued from here after inspection - Requires mandatory documents",
                IsMainStore = false,
                AllowDirectIssue = true,
                AllowTransfer = true,
                RequiresMandatoryDocuments = true, // ✅ KEY FLAG
                DisplayOrder = 2,
                MaxCapacity = 0,
                RequiresTemperatureControl = false,
                RequiresSecurityClearance = false,
                IsActive = true,
                Icon = "fas fa-dolly",
                Color = "#2ecc71",
                DefaultManagerRole = "StoreKeeper",
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            },
            new StoreType
            {
                Name = "Other Store",
                NameBn = "অন্যান্য স্টোর",
                Code = "OTHER",
                Description = "General purpose stores (Battalion/Range/Zila stores)",
                IsMainStore = false,
                AllowDirectIssue = true,
                AllowTransfer = true,
                RequiresMandatoryDocuments = false,
                DisplayOrder = 3,
                MaxCapacity = 0,
                RequiresTemperatureControl = false,
                RequiresSecurityClearance = false,
                IsActive = true,
                Icon = "fas fa-store",
                Color = "#95a5a6",
                DefaultManagerRole = "StoreKeeper",
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            }
        };

        context.StoreTypes.AddRange(storeTypes);
        await context.SaveChangesAsync();

        logger.LogInformation($"Successfully seeded {storeTypes.Length} StoreTypes");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding StoreTypes");
        throw;
    }
}

static async Task SeedSignatoryPresets(ApplicationDbContext context, ILogger logger)
{
    try
    {
        if (context.SignatoryPresets.Any())
        {
            logger.LogInformation("SignatoryPresets already exist. Skipping seed.");
            return;
        }

        logger.LogInformation("Seeding SignatoryPresets with Bengali support...");

        var presets = new[]
        {
            new SignatoryPreset
            {
                PresetName = "A.B.M. Forhad - Deputy Director (Store)",
                PresetNameBn = "এ.বি.এম. ফরহাদ - উপ-পরিচালক (ভাণ্ডার)",
                SignatoryName = "A.B.M. Forhad",
                SignatoryDesignation = "Deputy Director (Store)",
                SignatoryDesignationBn = "উপ-পরিচালক (ভাণ্ডার)",
                SignatoryId = "ANS-DD-001",
                SignatoryPhone = "+880-2-9898989",
                SignatoryEmail = "dd.store@ansar-vdp.gov.bd",
                Department = "Store",
                IsDefault = true,
                DisplayOrder = 1,
                IsActive = true,
                CreatedBy = "System",
                CreatedAt = DateTime.Now
            },
            new SignatoryPreset
            {
                PresetName = "Md. Abdul Karim - Deputy Director General (Admin)",
                PresetNameBn = "মোঃ আবদুল করিম - উপ-মহাপরিচালক (প্রশাসন)",
                SignatoryName = "Md. Abdul Karim",
                SignatoryDesignation = "Deputy Director General (Admin)",
                SignatoryDesignationBn = "উপ-মহাপরিচালক (প্রশাসন)",
                SignatoryId = "ANS-DDG-002",
                SignatoryPhone = "+880-2-9898990",
                SignatoryEmail = "ddg.admin@ansar-vdp.gov.bd",
                Department = "Admin",
                IsDefault = false,
                DisplayOrder = 2,
                IsActive = true,
                CreatedBy = "System",
                CreatedAt = DateTime.Now
            },
            new SignatoryPreset
            {
                PresetName = "Deputy Director (Provision)",
                PresetNameBn = "উপ-পরিচালক (প্রভিশন)",
                SignatoryName = "Mohammad Hasan",
                SignatoryDesignation = "Deputy Director (Provision)",
                SignatoryDesignationBn = "উপ-পরিচালক (প্রভিশন)",
                SignatoryId = "ANS-DD-003",
                SignatoryPhone = "+880-2-9898991",
                SignatoryEmail = "dd.provision@ansar-vdp.gov.bd",
                Department = "Provision",
                IsDefault = false,
                DisplayOrder = 3,
                IsActive = true,
                CreatedBy = "System",
                CreatedAt = DateTime.Now
            },
            new SignatoryPreset
            {
                PresetName = "Assistant Director (Store)",
                PresetNameBn = "সহকারী পরিচালক (ভাণ্ডার)",
                SignatoryName = "Md. Jahangir Alam",
                SignatoryDesignation = "Assistant Director (Store)",
                SignatoryDesignationBn = "সহকারী পরিচালক (ভাণ্ডার)",
                SignatoryId = "ANS-AD-004",
                SignatoryPhone = "+880-2-9898992",
                SignatoryEmail = "ad.store@ansar-vdp.gov.bd",
                Department = "Store",
                IsDefault = false,
                DisplayOrder = 4,
                IsActive = true,
                CreatedBy = "System",
                CreatedAt = DateTime.Now
            }
        };

        context.SignatoryPresets.AddRange(presets);
        await context.SaveChangesAsync();

        logger.LogInformation($"Successfully seeded {presets.Length} SignatoryPresets with proper Bengali encoding");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding SignatoryPresets");
        throw;
    }
}