using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsSystemDefined = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalThresholds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    RequiredRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalThresholds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentLocationId = table.Column<int>(type: "int", nullable: true),
                    LocationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Locations_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ranges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HeadquarterLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CommanderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CommanderRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CoverageArea = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RoleName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Permission = table.Column<int>(type: "int", nullable: false),
                    PermissionName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsReadOnly = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentTrackings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Carrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstimatedDelivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrackingUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentTrackings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SignatoryPresets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PresetName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PresetNameBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    SignatoryName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    SignatoryDesignation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryDesignationBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryPhone = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryEmail = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignatoryPresets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DefaultManagerRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    RequiresTemperatureControl = table.Column<bool>(type: "bit", nullable: false),
                    RequiresSecurityClearance = table.Column<bool>(type: "bit", nullable: false),
                    IsMainStore = table.Column<bool>(type: "bit", nullable: false),
                    AllowDirectIssue = table.Column<bool>(type: "bit", nullable: false),
                    AllowTransfer = table.Column<bool>(type: "bit", nullable: false),
                    RequiresMandatoryDocuments = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    BaseUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Abbreviation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    MetricName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MetricValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CalculatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CalculationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VendorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TIN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BIN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ModelNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemModels_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Battalions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CommanderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CommanderRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RangeId = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TotalPersonnel = table.Column<int>(type: "int", nullable: false),
                    OfficerCount = table.Column<int>(type: "int", nullable: false),
                    EnlistedCount = table.Column<int>(type: "int", nullable: false),
                    EstablishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OperationalStatus = table.Column<int>(type: "int", nullable: true),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battalions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Battalions_Ranges_RangeId",
                        column: x => x.RangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Zilas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NameBangla = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RangeId = table.Column<int>(type: "int", nullable: false),
                    Division = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DistrictOfficerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OfficeAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Area = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Population = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zilas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zilas_Ranges_RangeId",
                        column: x => x.RangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackingHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentTrackingId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Carrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstimatedDelivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrackingUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackingHistories_ShipmentTrackings_ShipmentTrackingId",
                        column: x => x.ShipmentTrackingId,
                        principalTable: "ShipmentTrackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreTypeCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreTypeId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    StoreTypeId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTypeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreTypeCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreTypeCategories_StoreTypes_StoreTypeId",
                        column: x => x.StoreTypeId,
                        principalTable: "StoreTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreTypeCategories_StoreTypes_StoreTypeId1",
                        column: x => x.StoreTypeId1,
                        principalTable: "StoreTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameBn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemModelId = table.Column<int>(type: "int", nullable: true),
                    BrandId = table.Column<int>(type: "int", nullable: true),
                    MinimumStock = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaximumStock = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ReorderLevel = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasExpiry = table.Column<bool>(type: "bit", nullable: false),
                    ShelfLife = table.Column<int>(type: "int", nullable: true),
                    StorageRequirements = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequiresSpecialHandling = table.Column<bool>(type: "bit", nullable: false),
                    SafetyInstructions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    WeightUnit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Dimensions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Material = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsHazardous = table.Column<bool>(type: "bit", nullable: false),
                    HazardClass = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemImage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BarcodePath = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QRCodeData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemControlType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AnsarLifeSpanMonths = table.Column<int>(type: "int", nullable: true),
                    VDPLifeSpanMonths = table.Column<int>(type: "int", nullable: true),
                    AnsarAlertBeforeDays = table.Column<int>(type: "int", nullable: true),
                    VDPAlertBeforeDays = table.Column<int>(type: "int", nullable: true),
                    LifeSpanMonths = table.Column<int>(type: "int", nullable: true),
                    AlertBeforeDays = table.Column<int>(type: "int", nullable: true),
                    IsAnsarAuthorized = table.Column<bool>(type: "bit", nullable: false),
                    IsVDPAuthorized = table.Column<bool>(type: "bit", nullable: false),
                    RequiresPersonalIssue = table.Column<bool>(type: "bit", nullable: false),
                    AnsarEntitlementQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VDPEntitlementQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EntitlementPeriodMonths = table.Column<int>(type: "int", nullable: true),
                    RequiresHigherApproval = table.Column<bool>(type: "bit", nullable: false),
                    ControlItemCategory = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_ItemModels_ItemModelId",
                        column: x => x.ItemModelId,
                        principalTable: "ItemModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Upazilas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NameBangla = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ZilaId = table.Column<int>(type: "int", nullable: false),
                    UpazilaOfficerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OfficerDesignation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OfficeAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Area = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Population = table.Column<int>(type: "int", nullable: true),
                    NumberOfUnions = table.Column<int>(type: "int", nullable: true),
                    NumberOfVillages = table.Column<int>(type: "int", nullable: true),
                    HasVDPUnit = table.Column<bool>(type: "bit", nullable: false),
                    VDPMemberCount = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpazilaChairmanName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VDPOfficerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Upazilas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Upazilas_Zilas_ZilaId",
                        column: x => x.ZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemSpecifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SpecificationName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SpecificationValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSpecifications_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Warranties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    WarrantyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarrantyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Terms = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ContactInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CoveredValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warranties_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warranties_Items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Warranties_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Unions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NameBangla = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpazilaId = table.Column<int>(type: "int", nullable: false),
                    ChairmanName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ChairmanContact = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SecretaryName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SecretaryContact = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VDPOfficerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VDPOfficerContact = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OfficeAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NumberOfWards = table.Column<int>(type: "int", nullable: true),
                    NumberOfVillages = table.Column<int>(type: "int", nullable: true),
                    NumberOfMouzas = table.Column<int>(type: "int", nullable: true),
                    Area = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Population = table.Column<int>(type: "int", nullable: true),
                    NumberOfHouseholds = table.Column<int>(type: "int", nullable: true),
                    HasVDPUnit = table.Column<bool>(type: "bit", nullable: false),
                    VDPMemberCountMale = table.Column<int>(type: "int", nullable: true),
                    VDPMemberCountFemale = table.Column<int>(type: "int", nullable: true),
                    AnsarMemberCount = table.Column<int>(type: "int", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: true),
                    IsRural = table.Column<bool>(type: "bit", nullable: false),
                    IsBorderArea = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unions_Upazilas_UpazilaId",
                        column: x => x.UpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BadgeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BattalionId = table.Column<int>(type: "int", nullable: true),
                    RangeId = table.Column<int>(type: "int", nullable: true),
                    ZilaId = table.Column<int>(type: "int", nullable: true),
                    UpazilaId = table.Column<int>(type: "int", nullable: true),
                    UnionId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Battalions_BattalionId",
                        column: x => x.BattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Ranges_RangeId",
                        column: x => x.RangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Unions_UnionId",
                        column: x => x.UnionId,
                        principalTable: "Unions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Upazilas_UpazilaId",
                        column: x => x.UpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Zilas_ZilaId",
                        column: x => x.ZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameBn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InCharge = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ManagerId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreKeeperName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreKeeperId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreKeeperContact = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreKeeperAssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatingHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Capacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UsedCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AvailableCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    StoreTypeId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    BattalionId = table.Column<int>(type: "int", nullable: true),
                    RangeId = table.Column<int>(type: "int", nullable: true),
                    ZilaId = table.Column<int>(type: "int", nullable: true),
                    UpazilaId = table.Column<int>(type: "int", nullable: true),
                    UnionId = table.Column<int>(type: "int", nullable: true),
                    RequiresTemperatureControl = table.Column<bool>(type: "bit", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MinTemperature = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MaxTemperature = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    SecurityLevel = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AccessRequirements = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsStockFrozen = table.Column<bool>(type: "bit", nullable: false),
                    StockFrozenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StockUnfrozenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StockFrozenReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_Battalions_BattalionId",
                        column: x => x.BattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Ranges_RangeId",
                        column: x => x.RangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_StoreTypes_StoreTypeId",
                        column: x => x.StoreTypeId,
                        principalTable: "StoreTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Unions_UnionId",
                        column: x => x.UnionId,
                        principalTable: "Unions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Upazilas_UpazilaId",
                        column: x => x.UpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Zilas_ZilaId",
                        column: x => x.ZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Module = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApprovalDelegations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ToUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalDelegations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalDelegations_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApprovalDelegations_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ApproverRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    MaxLevel = table.Column<int>(type: "int", nullable: false),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEscalated = table.Column<bool>(type: "bit", nullable: false),
                    EscalatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequiredRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TriggerCondition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    RequiredLevels = table.Column<int>(type: "int", nullable: false),
                    ApproverRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApproverUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ThresholdField = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalWorkflows_AspNetUsers_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    UserId1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entity = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Changes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DigitalSignatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    SignatureType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SignedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignatureData = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    DeviceInfo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    LocationInfo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignatureHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationCode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VerificationMethod = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VerificationFailReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalSignatures_AspNetUsers_SignedBy",
                        column: x => x.SignedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DigitalSignatures_AspNetUsers_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    UploadedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_AspNetUsers_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoginLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TargetRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PushSent = table.Column<bool>(type: "bit", nullable: true),
                    PushSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ActionUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RelatedEntity = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SignatureOTPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OTPCode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OTP = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignatureOTPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignatureOTPs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QualityRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    DeliveryRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    PriceRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ServiceRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    OverallRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EvaluatedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierEvaluations_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierEvaluations_AspNetUsers_EvaluatedByUserId",
                        column: x => x.EvaluatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierEvaluations_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfigurationKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfigurationValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsSystemLevel = table.Column<bool>(type: "bit", nullable: false),
                    RequiresRestart = table.Column<bool>(type: "bit", nullable: false),
                    ValidValues = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemConfigurations_AspNetUsers_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserNotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EmailEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SmsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PushEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LowStockAlerts = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryAlerts = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalAlerts = table.Column<bool>(type: "bit", nullable: false),
                    SystemAlerts = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotificationPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AllotmentLetters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllotmentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AllotmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedTo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedToType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedToBattalionId = table.Column<int>(type: "int", nullable: true),
                    IssuedToRangeId = table.Column<int>(type: "int", nullable: true),
                    IssuedToZilaId = table.Column<int>(type: "int", nullable: true),
                    IssuedToUpazilaId = table.Column<int>(type: "int", nullable: true),
                    FromStoreId = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DocumentPath = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SubjectBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BodyText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BodyTextBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CollectionDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignatoryName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryDesignation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryDesignationBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryPhone = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatoryEmail = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BengaliDate = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllotmentLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllotmentLetters_Battalions_IssuedToBattalionId",
                        column: x => x.IssuedToBattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllotmentLetters_Ranges_IssuedToRangeId",
                        column: x => x.IssuedToRangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllotmentLetters_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllotmentLetters_Upazilas_IssuedToUpazilaId",
                        column: x => x.IssuedToUpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllotmentLetters_Zilas_IssuedToZilaId",
                        column: x => x.IssuedToZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Barcodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarcodeNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BarcodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BarcodeData = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PrintedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PrintedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrintCount = table.Column<int>(type: "int", nullable: false),
                    LastScannedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastScannedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    LastScannedLocation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ScanCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barcodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Barcodes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Barcodes_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BatchTrackings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    InitialQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SupplierBatchNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ConsumedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RemainingQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    LastIssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    VendorId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VendorBatchNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QualityCheckStatus = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QualityCheckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QualityCheckBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    QuarantineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuarantinedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QuarantineReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TransferredFromBatchId = table.Column<int>(type: "int", nullable: true),
                    TransferReference = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchTrackings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchTrackings_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BattalionStores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BattalionId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    IsPrimaryStore = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattalionStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattalionStores_Battalions_BattalionId",
                        column: x => x.BattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattalionStores_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CycleCountSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ScheduleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NextScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastExecutedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ABCClass = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MinimumValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ItemsPerCount = table.Column<int>(type: "int", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CycleCountSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CycleCountSchedules_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CycleCountSchedules_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DamageReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    DamageType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Cause = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EstimatedLoss = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageReports_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DamageReports_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Damages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DamageNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DamageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: true),
                    DamageType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Cause = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ActionTaken = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReportedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EstimatedLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Damages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Damages_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Damages_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpiryTrackings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DisposalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisposalReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DisposedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsAlertSent = table.Column<bool>(type: "bit", nullable: false),
                    AlertSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisposedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpiryTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpiryTrackings_AspNetUsers_DisposedByUserId",
                        column: x => x.DisposedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpiryTrackings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpiryTrackings_Items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpiryTrackings_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCycleCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CountedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    CountedItems = table.Column<int>(type: "int", nullable: false),
                    VarianceItems = table.Column<int>(type: "int", nullable: false),
                    TotalVarianceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCycleCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCycleCounts_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryValuations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValuationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CalculationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValuationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CostingMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CalculatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ItemId1 = table.Column<int>(type: "int", nullable: true),
                    StoreId1 = table.Column<int>(type: "int", nullable: true),
                    CalculatedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryValuations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryValuations_AspNetUsers_CalculatedByUserId",
                        column: x => x.CalculatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryValuations_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryValuations_Items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryValuations_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryValuations_Stores_StoreId1",
                        column: x => x.StoreId1,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhysicalInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CountedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TotalSystemValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalPhysicalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    VarianceValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalItemsCounted = table.Column<int>(type: "int", nullable: false),
                    ItemsWithVariance = table.Column<int>(type: "int", nullable: false),
                    IsReconciled = table.Column<bool>(type: "bit", nullable: false),
                    ReconciliationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReconciliationBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsStockFrozen = table.Column<bool>(type: "bit", nullable: false),
                    StockFrozenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CountTeam = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SupervisorId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BattalionId = table.Column<int>(type: "int", nullable: true),
                    RangeId = table.Column<int>(type: "int", nullable: true),
                    ZilaId = table.Column<int>(type: "int", nullable: true),
                    UpazilaId = table.Column<int>(type: "int", nullable: true),
                    FiscalYear = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CountType = table.Column<int>(type: "int", nullable: false),
                    InitiatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InitiatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalReference = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReviewRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TotalSystemQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    TotalPhysicalQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    TotalVariance = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    TotalVarianceValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsAuditRequired = table.Column<bool>(type: "bit", nullable: false),
                    AuditOfficer = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AdjustmentStatus = table.Column<int>(type: "int", nullable: true),
                    AdjustedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CountEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdjustmentCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdjustmentNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VerifiedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalInventories_AspNetUsers_CountedByUserId",
                        column: x => x.CountedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhysicalInventories_AspNetUsers_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhysicalInventories_Battalions_BattalionId",
                        column: x => x.BattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhysicalInventories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhysicalInventories_Ranges_RangeId",
                        column: x => x.RangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhysicalInventories_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhysicalInventories_Upazilas_UpazilaId",
                        column: x => x.UpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhysicalInventories_Zilas_ZilaId",
                        column: x => x.ZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VendorId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PurchaseType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsMarketplacePurchase = table.Column<bool>(type: "bit", nullable: false),
                    MarketplaceUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProcurementType = table.Column<int>(type: "int", nullable: false),
                    ProcurementSource = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BudgetCode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Purchases_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Purchases_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AlertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    CurrentStock = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    MinimumStock = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ThresholdQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    AcknowledgedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AcknowledgedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAlerts_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAlerts_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockEntries_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    FromStoreId = table.Column<int>(type: "int", nullable: false),
                    ToStoreId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockOperations_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOperations_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOperations_Stores_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    ConfigKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConfigValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreConfigurations_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CurrentStock = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MinimumStock = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaximumStock = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ReorderLevel = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ReservedStock = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ReservedQuantity = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastStockUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastCountDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastIssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastAdjustmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastCountQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreItems_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MinQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaxQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReorderLevel = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AveragePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StoreId1 = table.Column<int>(type: "int", nullable: true),
                    ItemId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreStocks_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreStocks_Items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoreStocks_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreStocks_Stores_StoreId1",
                        column: x => x.StoreId1,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TemperatureLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Humidity = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsAlert = table.Column<bool>(type: "bit", nullable: false),
                    AlertReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RecordedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemperatureLogs_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TemperatureLogs_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TemperatureLogs_Stores_StoreId1",
                        column: x => x.StoreId1,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FromStoreId = table.Column<int>(type: "int", nullable: false),
                    ToStoreId = table.Column<int>(type: "int", nullable: false),
                    FromBattalionId = table.Column<int>(type: "int", nullable: true),
                    FromRangeId = table.Column<int>(type: "int", nullable: true),
                    FromZilaId = table.Column<int>(type: "int", nullable: true),
                    ToBattalionId = table.Column<int>(type: "int", nullable: true),
                    ToRangeId = table.Column<int>(type: "int", nullable: true),
                    ToZilaId = table.Column<int>(type: "int", nullable: true),
                    TransferType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransferredBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ShippedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ShippedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShipmentNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ShippingQRCode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiverSignature = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiptRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EstimatedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransportMode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VehicleNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DriverContact = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HasDiscrepancy = table.Column<bool>(type: "bit", nullable: false),
                    SenderSignature = table.Column<bool>(type: "bit", nullable: false),
                    IsReceiverSignature = table.Column<bool>(type: "bit", nullable: false),
                    ApproverSignature = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfer_Store_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfer_Store_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserStores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    UnassignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStores_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserStores_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WriteOffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WriteOffNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WriteOffDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalComments = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequiredApproverRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalThreshold = table.Column<int>(type: "int", nullable: false),
                    AttachmentPaths = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false),
                    NotificationSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotifiedUsers = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WriteOffs_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    ApproverRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ActionBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewStatus = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PreviousStatus = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalRequestId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalHistories_ApprovalRequests_ApprovalRequestId",
                        column: x => x.ApprovalRequestId,
                        principalTable: "ApprovalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalRequestId = table.Column<int>(type: "int", nullable: false),
                    StepLevel = table.Column<int>(type: "int", nullable: false),
                    ApproverRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsEscalated = table.Column<bool>(type: "bit", nullable: false),
                    EscalatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpecificApproverId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EscalatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EscalatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EscalationReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalSteps_ApprovalRequests_ApprovalRequestId",
                        column: x => x.ApprovalRequestId,
                        principalTable: "ApprovalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflowLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ApproverRole = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SpecificApproverId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ThresholdAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CanEscalate = table.Column<bool>(type: "bit", nullable: false),
                    TimeoutHours = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalWorkflowLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalWorkflowLevels_ApprovalWorkflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "ApprovalWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllotmentLetterItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllotmentLetterId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    AllottedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UnitBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemNameBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllotmentLetterItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllotmentLetterItems_AllotmentLetters_AllotmentLetterId",
                        column: x => x.AllotmentLetterId,
                        principalTable: "AllotmentLetters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllotmentLetterItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllotmentLetterRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllotmentLetterId = table.Column<int>(type: "int", nullable: false),
                    RecipientType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RangeId = table.Column<int>(type: "int", nullable: true),
                    BattalionId = table.Column<int>(type: "int", nullable: true),
                    ZilaId = table.Column<int>(type: "int", nullable: true),
                    UpazilaId = table.Column<int>(type: "int", nullable: true),
                    UnionId = table.Column<int>(type: "int", nullable: true),
                    RecipientName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RecipientNameBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StaffStrength = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SerialNo = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllotmentLetterRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllotmentLetterRecipients_AllotmentLetters_AllotmentLetterId",
                        column: x => x.AllotmentLetterId,
                        principalTable: "AllotmentLetters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllotmentLetterRecipients_Battalions_BattalionId",
                        column: x => x.BattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AllotmentLetterRecipients_Ranges_RangeId",
                        column: x => x.RangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AllotmentLetterRecipients_Unions_UnionId",
                        column: x => x.UnionId,
                        principalTable: "Unions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AllotmentLetterRecipients_Upazilas_UpazilaId",
                        column: x => x.UpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AllotmentLetterRecipients_Zilas_ZilaId",
                        column: x => x.ZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BatchMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    MovementType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BatchTrackingId = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    MovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BalanceBefore = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    NewBalance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    OldBalance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchMovement_BatchTracking_BatchId",
                        column: x => x.BatchId,
                        principalTable: "BatchTrackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatchMovement_BatchTracking_BatchTrackingId",
                        column: x => x.BatchTrackingId,
                        principalTable: "BatchTrackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchSignatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SignedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignatureData = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    SignatureHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    SignatureType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchSignatures_AspNetUsers_SignedBy",
                        column: x => x.SignedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatchSignatures_BatchTrackings_BatchId",
                        column: x => x.BatchId,
                        principalTable: "BatchTrackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpiredRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ExpiredQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DisposalMethod = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpiredRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpiredRecords_BatchTrackings_BatchId",
                        column: x => x.BatchId,
                        principalTable: "BatchTrackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpiredRecords_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpiredRecords_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DamageReportItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DamageReportId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    DamagedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    DamageType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DamageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscoveredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DamageDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PhotoUrls = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageReportItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageReportItems_DamageReports_DamageReportId",
                        column: x => x.DamageReportId,
                        principalTable: "DamageReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DamageReportItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WriteOffRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DamageReportId = table.Column<int>(type: "int", nullable: true),
                    DamageReportNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalReference = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExecutedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExecutedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisposalMethod = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DisposalRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WriteOffRequests_AspNetUsers_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WriteOffRequests_DamageReports_DamageReportId",
                        column: x => x.DamageReportId,
                        principalTable: "DamageReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WriteOffRequests_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCycleCountItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CycleCountId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SystemQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CountedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Variance = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    VarianceQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    VarianceValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VarianceReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CountedById = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CountedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRecounted = table.Column<bool>(type: "bit", nullable: false),
                    IsAdjusted = table.Column<bool>(type: "bit", nullable: false),
                    AdjustedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AdjustmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CountedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AdjustedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCycleCountItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCycleCountItems_AspNetUsers_AdjustedByUserId",
                        column: x => x.AdjustedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCycleCountItems_AspNetUsers_CountedByUserId",
                        column: x => x.CountedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCycleCountItems_InventoryCycleCounts_CycleCountId",
                        column: x => x.CycleCountId,
                        principalTable: "InventoryCycleCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryCycleCountItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    AuditType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditorName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Findings = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ComplianceStatus = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FiscalYear = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InventoryId = table.Column<int>(type: "int", nullable: true),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StoreLevel = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BattalionName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RangeName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ZilaName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpazilaName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TotalSystemValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPhysicalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalVarianceValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AuditFindingsJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PhysicalInventoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditReports_PhysicalInventories_PhysicalInventoryId",
                        column: x => x.PhysicalInventoryId,
                        principalTable: "PhysicalInventories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhysicalInventoryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicalInventoryId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SystemQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PhysicalQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Variance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    VarianceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CountedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CountedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BatchNumbers = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SerialNumbers = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SubCategoryId = table.Column<int>(type: "int", nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastCountDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VariancePercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    LastIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecountRequestedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RecountRequestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstCountQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    FirstCountBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FirstCountTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CountRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VarianceType = table.Column<int>(type: "int", nullable: false),
                    RecountQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RecountBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RecountTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlindCountQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    BlindCountBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    BlindCountTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlindCountCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalInventoryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalInventoryDetails_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhysicalInventoryDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhysicalInventoryDetails_PhysicalInventories_PhysicalInventoryId",
                        column: x => x.PhysicalInventoryId,
                        principalTable: "PhysicalInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalInventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicalInventoryId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SystemQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PhysicalQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Variance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SystemValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhysicalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VarianceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CountedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsRecounted = table.Column<bool>(type: "bit", nullable: false),
                    RecountQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdjustmentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalInventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalInventoryItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhysicalInventoryItems_PhysicalInventories_PhysicalInventoryId",
                        column: x => x.PhysicalInventoryId,
                        principalTable: "PhysicalInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjustmentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AdjustmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdjustmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    PhysicalInventoryId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    OldQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    NewQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    AdjustmentQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceDocument = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovalReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FiscalYear = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AdjustedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AdjustedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    RejectedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AuditTrailJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreId1 = table.Column<int>(type: "int", nullable: true),
                    ItemId1 = table.Column<int>(type: "int", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_PhysicalInventories_PhysicalInventoryId",
                        column: x => x.PhysicalInventoryId,
                        principalTable: "PhysicalInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Stores_StoreId1",
                        column: x => x.StoreId1,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ChallanNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsReceives_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AcceptedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RejectedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiveRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseItems_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseItems_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Requisitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequisitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TotalEstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalComments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromStoreId = table.Column<int>(type: "int", nullable: true),
                    ToStoreId = table.Column<int>(type: "int", nullable: true),
                    FulfillmentStatus = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AutoConvertToPO = table.Column<bool>(type: "bit", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApprovedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Level1ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Level1ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Level2ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Level2ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FinalApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requisitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requisitions_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requisitions_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requisitions_Purchases_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "Purchases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requisitions_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requisitions_Stores_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockEntryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockEntryId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BarcodesGenerated = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockEntryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockEntryItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockEntryItems_StockEntries_StockEntryId",
                        column: x => x.StockEntryId,
                        principalTable: "StockEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovementType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MovementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    SourceStoreId = table.Column<int>(type: "int", nullable: true),
                    DestinationStoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    OldBalance = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    NewBalance = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    MovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MovedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreItemId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_AspNetUsers_MovedByUserId",
                        column: x => x.MovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockMovements_StoreItems_StoreItemId",
                        column: x => x.StoreItemId,
                        principalTable: "StoreItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Stores_DestinationStoreId",
                        column: x => x.DestinationStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Stores_SourceStoreId",
                        column: x => x.SourceStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransferDiscrepancies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ExpectedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DiscrepancyQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ShippedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Variance = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    ReportedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReportedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferDiscrepancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferDiscrepancies_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferDiscrepancies_Transfers_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Transfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    ShippedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ItemCondition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedCondition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShippedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PackageCount = table.Column<int>(type: "int", nullable: false),
                    PackageDetails = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferItems_Transfers_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Transfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferShipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferId = table.Column<int>(type: "int", nullable: false),
                    ShipmentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrackingNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EstimatedDelivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDelivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ShippedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShippedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PackingListNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransportCompany = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VehicleNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SealNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DriverContact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstimatedArrival = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualArrival = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiptCondition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferShipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferShipments_Transfers_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Transfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllotmentLetterRecipientItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllotmentLetterRecipientId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    AllottedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UnitBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemNameBn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllotmentLetterRecipientItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllotmentLetterRecipientItems_AllotmentLetterRecipients_AllotmentLetterRecipientId",
                        column: x => x.AllotmentLetterRecipientId,
                        principalTable: "AllotmentLetterRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllotmentLetterRecipientItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchSignatureItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchSignatureId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchSignatureItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchSignatureItem_BatchSignatures_BatchSignatureId",
                        column: x => x.BatchSignatureId,
                        principalTable: "BatchSignatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisposalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WriteOffId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    DisposalMethod = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DisposalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisposalLocation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DisposalCompany = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DisposalCertificateNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DisposedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    WitnessedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PhotoUrls = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DisposalNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AuthorizedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisposalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisposalRecords_AspNetUsers_AuthorizedBy",
                        column: x => x.AuthorizedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisposalRecords_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisposalRecords_WriteOffRequests_WriteOffId",
                        column: x => x.WriteOffId,
                        principalTable: "WriteOffRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WriteOffItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WriteOffId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    WriteOffRequestId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    WriteOffRequestId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WriteOffItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WriteOffItems_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WriteOffItems_WriteOffRequests_WriteOffRequestId",
                        column: x => x.WriteOffRequestId,
                        principalTable: "WriteOffRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WriteOffItems_WriteOffRequests_WriteOffRequestId1",
                        column: x => x.WriteOffRequestId1,
                        principalTable: "WriteOffRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WriteOffItems_WriteOffs_WriteOffId",
                        column: x => x.WriteOffId,
                        principalTable: "WriteOffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockAdjustmentId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SystemQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PhysicalQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    AdjustmentQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    AdjustmentValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentItems_StockAdjustments_StockAdjustmentId",
                        column: x => x.StockAdjustmentId,
                        principalTable: "StockAdjustments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiveItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiveId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    AcceptedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    RejectedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    QualityCheckStatus = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiveItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsReceiveItems_GoodsReceives_GoodsReceiveId",
                        column: x => x.GoodsReceiveId,
                        principalTable: "GoodsReceives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiveItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CheckDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    PurchaseId = table.Column<int>(type: "int", nullable: true),
                    CheckType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CheckedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CheckedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PassedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FailedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FailureReasons = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiresRetest = table.Column<bool>(type: "bit", nullable: false),
                    RetestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GoodsReceiveId = table.Column<int>(type: "int", nullable: false),
                    CheckedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OverallStatus = table.Column<int>(type: "int", nullable: false),
                    CheckedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityChecks_AspNetUsers_CheckedByUserId",
                        column: x => x.CheckedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QualityChecks_GoodsReceives_GoodsReceiveId",
                        column: x => x.GoodsReceiveId,
                        principalTable: "GoodsReceives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualityChecks_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualityChecks_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequisitionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    EstimatedUnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedTotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Specification = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitionItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequisitionItems_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferShipmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PackageNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ShippedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PackageCount = table.Column<int>(type: "int", nullable: false),
                    PackageDetails = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferShipmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferShipmentItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferShipmentItems_TransferShipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "TransferShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityCheckItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QualityCheckId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CheckedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    PassedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    FailedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CheckParameters = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityCheckItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityCheckItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualityCheckItems_QualityChecks_QualityCheckId",
                        column: x => x.QualityCheckId,
                        principalTable: "QualityChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConditionCheckItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConditionCheckId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CheckedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    GoodQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    DamagedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    ExpiredQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Photos = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionCheckItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConditionCheckItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConditionChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    CheckedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CheckedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OverallCondition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CheckDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConditionChecks_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DamageRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DamagedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DamageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DamageReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DamageType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageRecords_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DamageRecords_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    HandoverRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueItems_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssuerSignatureId = table.Column<int>(type: "int", nullable: true),
                    ApproverSignatureId = table.Column<int>(type: "int", nullable: true),
                    ReceiverSignatureId = table.Column<int>(type: "int", nullable: true),
                    SignerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignerBadgeId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignerDesignation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IssueNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssueNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IssuedTo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedToType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalReferenceNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalComments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedByName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedByBadgeNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignaturePath = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    SignatureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiverDesignation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiverContact = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedToBattalionId = table.Column<int>(type: "int", nullable: true),
                    IssuedToRangeId = table.Column<int>(type: "int", nullable: true),
                    IssuedToZilaId = table.Column<int>(type: "int", nullable: true),
                    IssuedToUpazilaId = table.Column<int>(type: "int", nullable: true),
                    IssuedToUnionId = table.Column<int>(type: "int", nullable: true),
                    IssuedToIndividualName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedToIndividualBadgeNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedToIndividualMobile = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ToEntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ToEntityId = table.Column<int>(type: "int", nullable: false),
                    FromStoreId = table.Column<int>(type: "int", nullable: true),
                    DeliveryLocation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VoucherNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VoucherNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoucherGeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoucherQRCode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPartialIssue = table.Column<bool>(type: "bit", nullable: false),
                    ParentIssueId = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VoucherDocumentPath = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MemoNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MemoDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletionReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AllotmentLetterId = table.Column<int>(type: "int", nullable: true),
                    AllotmentLetterNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issues_AllotmentLetters_AllotmentLetterId",
                        column: x => x.AllotmentLetterId,
                        principalTable: "AllotmentLetters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issues_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Issues_Battalions_IssuedToBattalionId",
                        column: x => x.IssuedToBattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Issues_Issues_ParentIssueId",
                        column: x => x.ParentIssueId,
                        principalTable: "Issues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Issues_Ranges_IssuedToRangeId",
                        column: x => x.IssuedToRangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Issues_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issues_Unions_IssuedToUnionId",
                        column: x => x.IssuedToUnionId,
                        principalTable: "Unions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Issues_Upazilas_IssuedToUpazilaId",
                        column: x => x.IssuedToUpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Issues_Zilas_IssuedToZilaId",
                        column: x => x.IssuedToZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IssueVouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssueId = table.Column<int>(type: "int", nullable: false),
                    IssuedTo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AuthorizedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiverSignature = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    VoucherBarcode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueVouchers_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiveNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceiveNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReceivedFrom = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedFromType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceivedFromBattalionId = table.Column<int>(type: "int", nullable: true),
                    ReceivedFromRangeId = table.Column<int>(type: "int", nullable: true),
                    ReceivedFromZilaId = table.Column<int>(type: "int", nullable: true),
                    ReceivedFromUpazilaId = table.Column<int>(type: "int", nullable: true),
                    ReceivedFromUnionId = table.Column<int>(type: "int", nullable: true),
                    ReceivedFromIndividualName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedFromIndividualBadgeNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    OriginalIssueId = table.Column<int>(type: "int", nullable: true),
                    OriginalIssueNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OriginalVoucherNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiveType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiverSignature = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiverBadgeNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiverDesignation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsReceiverSignature = table.Column<bool>(type: "bit", nullable: false),
                    VerifierSignature = table.Column<bool>(type: "bit", nullable: false),
                    OverallCondition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AssessmentNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AssessedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receives_Battalions_ReceivedFromBattalionId",
                        column: x => x.ReceivedFromBattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receives_Issues_OriginalIssueId",
                        column: x => x.OriginalIssueId,
                        principalTable: "Issues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receives_Ranges_ReceivedFromRangeId",
                        column: x => x.ReceivedFromRangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receives_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receives_Unions_ReceivedFromUnionId",
                        column: x => x.ReceivedFromUnionId,
                        principalTable: "Unions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receives_Upazilas_ReceivedFromUpazilaId",
                        column: x => x.ReceivedFromUpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receives_Zilas_ReceivedFromZilaId",
                        column: x => x.ReceivedFromZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Returns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    ReturnedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReturnedByType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReturnType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    ToStoreId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiptRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsRestocked = table.Column<bool>(type: "bit", nullable: false),
                    RestockApprovalRequired = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReturnerSignature = table.Column<bool>(type: "bit", nullable: false),
                    IsReceiverSignature = table.Column<bool>(type: "bit", nullable: false),
                    ApproverSignature = table.Column<bool>(type: "bit", nullable: false),
                    OriginalIssueId = table.Column<int>(type: "int", nullable: true),
                    OriginalIssueNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceiveId = table.Column<int>(type: "int", nullable: true),
                    FromEntityType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FromEntityId = table.Column<int>(type: "int", nullable: false),
                    ConditionCheckId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Returns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Returns_Issues_OriginalIssueId",
                        column: x => x.OriginalIssueId,
                        principalTable: "Issues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Returns_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Returns_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Returns_Stores_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Signatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    SignatureType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SignatureData = table.Column<string>(type: "nvarchar(max)", maxLength: 4000, nullable: true),
                    SignerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignerBadgeId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignerDesignation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DeviceInfo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IssueId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signatures_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockReturns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OriginalIssueId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ReturnedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RestockApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockReturns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockReturns_Issues_OriginalIssueId",
                        column: x => x.OriginalIssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockReturns_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelItemIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PersonnelId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PersonnelType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PersonnelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PersonnelBadgeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PersonnelUnit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PersonnelDesignation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PersonnelMobile = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OriginalIssueId = table.Column<int>(type: "int", nullable: true),
                    ReceiveId = table.Column<int>(type: "int", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LifeExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AlertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemainingDays = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReplacedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacementReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReplacementIssueId = table.Column<int>(type: "int", nullable: true),
                    BattalionId = table.Column<int>(type: "int", nullable: true),
                    RangeId = table.Column<int>(type: "int", nullable: true),
                    ZilaId = table.Column<int>(type: "int", nullable: true),
                    UpazilaId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    IsAlertSent = table.Column<bool>(type: "bit", nullable: false),
                    LastAlertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AlertCount = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelItemIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelItemIssues_Battalions_BattalionId",
                        column: x => x.BattalionId,
                        principalTable: "Battalions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelItemIssues_Issues_OriginalIssueId",
                        column: x => x.OriginalIssueId,
                        principalTable: "Issues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonnelItemIssues_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelItemIssues_Ranges_RangeId",
                        column: x => x.RangeId,
                        principalTable: "Ranges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonnelItemIssues_Receives_ReceiveId",
                        column: x => x.ReceiveId,
                        principalTable: "Receives",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonnelItemIssues_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelItemIssues_Upazilas_UpazilaId",
                        column: x => x.UpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonnelItemIssues_Zilas_ZilaId",
                        column: x => x.ZilaId,
                        principalTable: "Zilas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReceiveItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiveId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DamageNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DamageDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DamagePhotoPath = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsScanned = table.Column<bool>(type: "bit", nullable: false),
                    ItemId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiveItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiveItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiveItems_Items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiveItems_Receives_ReceiveId",
                        column: x => x.ReceiveId,
                        principalTable: "Receives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiveItems_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReturnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ReturnQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    AcceptedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    RejectedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CheckedCondition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReturnReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnItems_Returns_ReturnId",
                        column: x => x.ReturnId,
                        principalTable: "Returns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterItems_AllotmentLetterId",
                table: "AllotmentLetterItems",
                column: "AllotmentLetterId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterItems_ItemId",
                table: "AllotmentLetterItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipientItems_AllotmentLetterRecipientId",
                table: "AllotmentLetterRecipientItems",
                column: "AllotmentLetterRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipientItems_ItemId",
                table: "AllotmentLetterRecipientItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_AllotmentLetterId",
                table: "AllotmentLetterRecipients",
                column: "AllotmentLetterId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_BattalionId",
                table: "AllotmentLetterRecipients",
                column: "BattalionId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_RangeId",
                table: "AllotmentLetterRecipients",
                column: "RangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_UnionId",
                table: "AllotmentLetterRecipients",
                column: "UnionId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_UpazilaId",
                table: "AllotmentLetterRecipients",
                column: "UpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetterRecipients_ZilaId",
                table: "AllotmentLetterRecipients",
                column: "ZilaId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetters_AllotmentNo",
                table: "AllotmentLetters",
                column: "AllotmentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetters_FromStoreId",
                table: "AllotmentLetters",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetters_IssuedToBattalionId",
                table: "AllotmentLetters",
                column: "IssuedToBattalionId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetters_IssuedToRangeId",
                table: "AllotmentLetters",
                column: "IssuedToRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetters_IssuedToUpazilaId",
                table: "AllotmentLetters",
                column: "IssuedToUpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentLetters_IssuedToZilaId",
                table: "AllotmentLetters",
                column: "IssuedToZilaId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDelegations_FromUserId",
                table: "ApprovalDelegations",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDelegations_ToUserId",
                table: "ApprovalDelegations",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistories_ApprovalRequestId",
                table: "ApprovalHistories",
                column: "ApprovalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLevel_Level_EntityType",
                table: "ApprovalLevels",
                columns: new[] { "Level", "EntityType" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_ApprovedByUserId",
                table: "ApprovalRequests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_RequestedByUserId",
                table: "ApprovalRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_ApprovalRequestId",
                table: "ApprovalSteps",
                column: "ApprovalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflowLevels_WorkflowId",
                table: "ApprovalWorkflowLevels",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflows_ApproverUserId",
                table: "ApprovalWorkflows",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId1",
                table: "AspNetUserRoles",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BadgeNumber",
                table: "AspNetUsers",
                column: "BadgeNumber",
                unique: true,
                filter: "[BadgeNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BattalionId",
                table: "AspNetUsers",
                column: "BattalionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RangeId",
                table: "AspNetUsers",
                column: "RangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UnionId",
                table: "AspNetUsers",
                column: "UnionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UpazilaId",
                table: "AspNetUsers",
                column: "UpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ZilaId",
                table: "AspNetUsers",
                column: "ZilaId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditReports_PhysicalInventoryId",
                table: "AuditReports",
                column: "PhysicalInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_UserId",
                table: "Audits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_BarcodeNumber",
                table: "Barcodes",
                column: "BarcodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_ItemId",
                table: "Barcodes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_StoreId",
                table: "Barcodes",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchMovements_BatchId",
                table: "BatchMovements",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchMovements_BatchTrackingId",
                table: "BatchMovements",
                column: "BatchTrackingId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchMovements_MovementDate",
                table: "BatchMovements",
                column: "MovementDate");

            migrationBuilder.CreateIndex(
                name: "IX_BatchSignatureItem_BatchSignatureId",
                table: "BatchSignatureItem",
                column: "BatchSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchSignatures_BatchId",
                table: "BatchSignatures",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchSignatures_SignedBy",
                table: "BatchSignatures",
                column: "SignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BatchTrackings_BatchNumber",
                table: "BatchTrackings",
                column: "BatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_BatchTrackings_ItemId_StoreId",
                table: "BatchTrackings",
                columns: new[] { "ItemId", "StoreId" });

            migrationBuilder.CreateIndex(
                name: "IX_BatchTrackings_StoreId",
                table: "BatchTrackings",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Battalions_Code",
                table: "Battalions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battalions_RangeId",
                table: "Battalions",
                column: "RangeId");

            migrationBuilder.CreateIndex(
                name: "IX_BattalionStores_BattalionId_StoreId",
                table: "BattalionStores",
                columns: new[] { "BattalionId", "StoreId" },
                unique: true,
                filter: "[EffectiveTo] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BattalionStores_StoreId",
                table: "BattalionStores",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Code",
                table: "Brands",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Code",
                table: "Categories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConditionCheckItems_ConditionCheckId",
                table: "ConditionCheckItems",
                column: "ConditionCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionCheckItems_ItemId",
                table: "ConditionCheckItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionChecks_ItemId",
                table: "ConditionChecks",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionChecks_ReturnId",
                table: "ConditionChecks",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCountSchedules_CategoryId",
                table: "CycleCountSchedules",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCountSchedules_NextScheduledDate",
                table: "CycleCountSchedules",
                column: "NextScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_CycleCountSchedules_StoreId",
                table: "CycleCountSchedules",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageRecords_ItemId",
                table: "DamageRecords",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageRecords_ReturnId",
                table: "DamageRecords",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageRecords_StoreId",
                table: "DamageRecords",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageReportItems_DamageReportId",
                table: "DamageReportItems",
                column: "DamageReportId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageReportItems_ItemId",
                table: "DamageReportItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageReports_ItemId",
                table: "DamageReports",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageReports_ReportNo",
                table: "DamageReports",
                column: "ReportNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DamageReports_StoreId",
                table: "DamageReports",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Damages_DamageNo",
                table: "Damages",
                column: "DamageNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Damages_ItemId",
                table: "Damages",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Damages_StoreId",
                table: "Damages",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignatures_EntityType_EntityId",
                table: "DigitalSignatures",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignatures_SignedAt",
                table: "DigitalSignatures",
                column: "SignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignatures_SignedBy",
                table: "DigitalSignatures",
                column: "SignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignatures_VerifiedBy",
                table: "DigitalSignatures",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_AuthorizedBy",
                table: "DisposalRecords",
                column: "AuthorizedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_ItemId",
                table: "DisposalRecords",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_WriteOffId",
                table: "DisposalRecords",
                column: "WriteOffId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UploadedByUserId",
                table: "Documents",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiredRecords_BatchId",
                table: "ExpiredRecords",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiredRecords_ItemId",
                table: "ExpiredRecords",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiredRecords_StoreId",
                table: "ExpiredRecords",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryTrackings_DisposedByUserId",
                table: "ExpiryTrackings",
                column: "DisposedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryTrackings_ExpiryDate",
                table: "ExpiryTrackings",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryTrackings_ItemId_StoreId",
                table: "ExpiryTrackings",
                columns: new[] { "ItemId", "StoreId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryTrackings_ItemId1",
                table: "ExpiryTrackings",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryTrackings_StoreId",
                table: "ExpiryTrackings",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiveItems_GoodsReceiveId",
                table: "GoodsReceiveItems",
                column: "GoodsReceiveId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiveItems_ItemId",
                table: "GoodsReceiveItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceives_PurchaseId",
                table: "GoodsReceives",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCycleCountItems_AdjustedByUserId",
                table: "InventoryCycleCountItems",
                column: "AdjustedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCycleCountItems_CountedByUserId",
                table: "InventoryCycleCountItems",
                column: "CountedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCycleCountItems_CycleCountId",
                table: "InventoryCycleCountItems",
                column: "CycleCountId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCycleCountItems_ItemId",
                table: "InventoryCycleCountItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCycleCounts_CountDate",
                table: "InventoryCycleCounts",
                column: "CountDate");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCycleCounts_CountNumber",
                table: "InventoryCycleCounts",
                column: "CountNumber",
                unique: true,
                filter: "[CountNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCycleCounts_StoreId",
                table: "InventoryCycleCounts",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuations_CalculatedByUserId",
                table: "InventoryValuations",
                column: "CalculatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuations_ItemId_StoreId_ValuationDate",
                table: "InventoryValuations",
                columns: new[] { "ItemId", "StoreId", "ValuationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuations_ItemId1",
                table: "InventoryValuations",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuations_StoreId",
                table: "InventoryValuations",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuations_StoreId1",
                table: "InventoryValuations",
                column: "StoreId1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryValuations_ValuationDate",
                table: "InventoryValuations",
                column: "ValuationDate");

            migrationBuilder.CreateIndex(
                name: "IX_IssueItems_IssueId",
                table: "IssueItems",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueItems_ItemId",
                table: "IssueItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueItems_StoreId",
                table: "IssueItems",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AllotmentLetterId",
                table: "Issues",
                column: "AllotmentLetterId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ApproverSignatureId",
                table: "Issues",
                column: "ApproverSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_CreatedByUserId",
                table: "Issues",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_FromStoreId",
                table: "Issues",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssueDate",
                table: "Issues",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssuedToBattalionId",
                table: "Issues",
                column: "IssuedToBattalionId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssuedToRangeId",
                table: "Issues",
                column: "IssuedToRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssuedToUnionId",
                table: "Issues",
                column: "IssuedToUnionId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssuedToUpazilaId",
                table: "Issues",
                column: "IssuedToUpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssuedToZilaId",
                table: "Issues",
                column: "IssuedToZilaId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssueNo",
                table: "Issues",
                column: "IssueNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssuerSignatureId",
                table: "Issues",
                column: "IssuerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ParentIssueId",
                table: "Issues",
                column: "ParentIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ReceiverSignatureId",
                table: "Issues",
                column: "ReceiverSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueVouchers_IssueId",
                table: "IssueVouchers",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueVouchers_VoucherNumber",
                table: "IssueVouchers",
                column: "VoucherNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemModels_BrandId",
                table: "ItemModels",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_BrandId",
                table: "Items",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Code",
                table: "Items",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsActive",
                table: "Items",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemCode",
                table: "Items",
                column: "ItemCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemModelId",
                table: "Items",
                column: "ItemModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                table: "Items",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SubCategoryId",
                table: "Items",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSpecifications_ItemId",
                table: "ItemSpecifications",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code",
                table: "Locations",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ParentLocationId",
                table: "Locations",
                column: "ParentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginLogs_UserId",
                table: "LoginLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_BattalionId",
                table: "PersonnelItemIssues",
                column: "BattalionId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_ItemId",
                table: "PersonnelItemIssues",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_LifeExpiryDate",
                table: "PersonnelItemIssues",
                column: "LifeExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_OriginalIssueId",
                table: "PersonnelItemIssues",
                column: "OriginalIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_PersonnelBadgeNo",
                table: "PersonnelItemIssues",
                column: "PersonnelBadgeNo");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_RangeId",
                table: "PersonnelItemIssues",
                column: "RangeId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_ReceiveId",
                table: "PersonnelItemIssues",
                column: "ReceiveId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_Status",
                table: "PersonnelItemIssues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_StoreId",
                table: "PersonnelItemIssues",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_UpazilaId",
                table: "PersonnelItemIssues",
                column: "UpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelItemIssues_ZilaId",
                table: "PersonnelItemIssues",
                column: "ZilaId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_BattalionId",
                table: "PhysicalInventories",
                column: "BattalionId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_CategoryId",
                table: "PhysicalInventories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_CountedByUserId",
                table: "PhysicalInventories",
                column: "CountedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_CountNo",
                table: "PhysicalInventories",
                column: "CountNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_RangeId",
                table: "PhysicalInventories",
                column: "RangeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_StoreId",
                table: "PhysicalInventories",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_UpazilaId",
                table: "PhysicalInventories",
                column: "UpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_VerifiedByUserId",
                table: "PhysicalInventories",
                column: "VerifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventories_ZilaId",
                table: "PhysicalInventories",
                column: "ZilaId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventoryDetails_CategoryId",
                table: "PhysicalInventoryDetails",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventoryDetails_ItemId",
                table: "PhysicalInventoryDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventoryDetails_PhysicalInventoryId",
                table: "PhysicalInventoryDetails",
                column: "PhysicalInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventoryItems_ItemId",
                table: "PhysicalInventoryItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalInventoryItems_PhysicalInventoryId",
                table: "PhysicalInventoryItems",
                column: "PhysicalInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_ItemId",
                table: "PurchaseItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_PurchaseId",
                table: "PurchaseItems",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_StoreId",
                table: "PurchaseItems",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_ItemId",
                table: "PurchaseOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderId",
                table: "PurchaseOrderItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_StoreId",
                table: "PurchaseOrders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_VendorId",
                table: "PurchaseOrders",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_CreatedByUserId",
                table: "Purchases",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchaseOrderNo",
                table: "Purchases",
                column: "PurchaseOrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_StoreId",
                table: "Purchases",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_VendorId",
                table: "Purchases",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityCheckItems_ItemId",
                table: "QualityCheckItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityCheckItems_QualityCheckId",
                table: "QualityCheckItems",
                column: "QualityCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_CheckDate",
                table: "QualityChecks",
                column: "CheckDate");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_CheckedByUserId",
                table: "QualityChecks",
                column: "CheckedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_CheckNumber",
                table: "QualityChecks",
                column: "CheckNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_GoodsReceiveId",
                table: "QualityChecks",
                column: "GoodsReceiveId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_ItemId",
                table: "QualityChecks",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_PurchaseId",
                table: "QualityChecks",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Ranges_Code",
                table: "Ranges",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveItems_ItemId",
                table: "ReceiveItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveItems_ItemId1",
                table: "ReceiveItems",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveItems_ReceiveId",
                table: "ReceiveItems",
                column: "ReceiveId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveItems_StoreId",
                table: "ReceiveItems",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_OriginalIssueId",
                table: "Receives",
                column: "OriginalIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_ReceivedFromBattalionId",
                table: "Receives",
                column: "ReceivedFromBattalionId");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_ReceivedFromRangeId",
                table: "Receives",
                column: "ReceivedFromRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_ReceivedFromUnionId",
                table: "Receives",
                column: "ReceivedFromUnionId");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_ReceivedFromUpazilaId",
                table: "Receives",
                column: "ReceivedFromUpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_ReceivedFromZilaId",
                table: "Receives",
                column: "ReceivedFromZilaId");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_ReceiveNo",
                table: "Receives",
                column: "ReceiveNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receives_StoreId",
                table: "Receives",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionItems_ItemId",
                table: "RequisitionItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionItems_RequisitionId",
                table: "RequisitionItems",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_ApprovedByUserId",
                table: "Requisitions",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_FromStoreId",
                table: "Requisitions",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_PurchaseOrderId",
                table: "Requisitions",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_RequestedByUserId",
                table: "Requisitions",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_RequisitionNumber",
                table: "Requisitions",
                column: "RequisitionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_ToStoreId",
                table: "Requisitions",
                column: "ToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ItemId",
                table: "ReturnItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ReturnId",
                table: "ReturnItems",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_ItemId",
                table: "Returns",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_OriginalIssueId",
                table: "Returns",
                column: "OriginalIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_ReturnNo",
                table: "Returns",
                column: "ReturnNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Returns_StoreId",
                table: "Returns",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_ToStoreId",
                table: "Returns",
                column: "ToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTrackings_TrackingCode",
                table: "ShipmentTrackings",
                column: "TrackingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignatureOTPs_UserId",
                table: "SignatureOTPs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_IssueId",
                table: "Signatures",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_ReferenceType_ReferenceId",
                table: "Signatures",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItems_ItemId",
                table: "StockAdjustmentItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItems_StockAdjustmentId",
                table: "StockAdjustmentItems",
                column: "StockAdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_AdjustmentDate",
                table: "StockAdjustments",
                column: "AdjustmentDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_AdjustmentNo",
                table: "StockAdjustments",
                column: "AdjustmentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ApprovedByUserId",
                table: "StockAdjustments",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ItemId",
                table: "StockAdjustments",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ItemId1",
                table: "StockAdjustments",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_PhysicalInventoryId",
                table: "StockAdjustments",
                column: "PhysicalInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_StoreId",
                table: "StockAdjustments",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_StoreId1",
                table: "StockAdjustments",
                column: "StoreId1");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlerts_AlertDate",
                table: "StockAlerts",
                column: "AlertDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlerts_ItemId",
                table: "StockAlerts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlerts_Status",
                table: "StockAlerts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlerts_StoreId",
                table: "StockAlerts",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_EntryDate",
                table: "StockEntries",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_EntryNo",
                table: "StockEntries",
                column: "EntryNo",
                unique: true,
                filter: "[EntryNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_StoreId",
                table: "StockEntries",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntryItems_ItemId",
                table: "StockEntryItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntryItems_StockEntryId",
                table: "StockEntryItems",
                column: "StockEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_DestinationStoreId",
                table: "StockMovements",
                column: "DestinationStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ItemId_MovementDate",
                table: "StockMovements",
                columns: new[] { "ItemId", "MovementDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_MovedByUserId",
                table: "StockMovements",
                column: "MovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SourceStoreId",
                table: "StockMovements",
                column: "SourceStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_StoreId",
                table: "StockMovements",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_StoreItemId",
                table: "StockMovements",
                column: "StoreItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOperations_CreatedAt",
                table: "StockOperations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StockOperations_FromStoreId",
                table: "StockOperations",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOperations_ItemId",
                table: "StockOperations",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOperations_ToStoreId",
                table: "StockOperations",
                column: "ToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockReturns_ItemId",
                table: "StockReturns",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockReturns_OriginalIssueId",
                table: "StockReturns",
                column: "OriginalIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_StockReturns_ReturnDate",
                table: "StockReturns",
                column: "ReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockReturns_ReturnNumber",
                table: "StockReturns",
                column: "ReturnNumber",
                unique: true,
                filter: "[ReturnNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StoreConfigurations_StoreId",
                table: "StoreConfigurations",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreItems_ItemId",
                table: "StoreItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreItems_StoreId_ItemId",
                table: "StoreItems",
                columns: new[] { "StoreId", "ItemId" },
                unique: true,
                filter: "[StoreId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BattalionId",
                table: "Stores",
                column: "BattalionId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Code",
                table: "Stores",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_IsActive",
                table: "Stores",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_LocationId",
                table: "Stores",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Name",
                table: "Stores",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_RangeId",
                table: "Stores",
                column: "RangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreTypeId",
                table: "Stores",
                column: "StoreTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_UnionId",
                table: "Stores",
                column: "UnionId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_UpazilaId",
                table: "Stores",
                column: "UpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_ZilaId",
                table: "Stores",
                column: "ZilaId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreStocks_ItemId",
                table: "StoreStocks",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreStocks_ItemId1",
                table: "StoreStocks",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_StoreStocks_StoreId_ItemId",
                table: "StoreStocks",
                columns: new[] { "StoreId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreStocks_StoreId1",
                table: "StoreStocks",
                column: "StoreId1");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTypeCategories_CategoryId",
                table: "StoreTypeCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTypeCategories_StoreTypeId_CategoryId",
                table: "StoreTypeCategories",
                columns: new[] { "StoreTypeId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreTypeCategories_StoreTypeId1",
                table: "StoreTypeCategories",
                column: "StoreTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTypes_Code",
                table: "StoreTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_Code",
                table: "SubCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEvaluations_ApprovedByUserId",
                table: "SupplierEvaluations",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEvaluations_EvaluatedByUserId",
                table: "SupplierEvaluations",
                column: "EvaluatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEvaluations_EvaluationDate",
                table: "SupplierEvaluations",
                column: "EvaluationDate");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEvaluations_VendorId",
                table: "SupplierEvaluations",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigurations_ModifiedByUserId",
                table: "SystemConfigurations",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureLogs_LogTime",
                table: "TemperatureLogs",
                column: "LogTime");

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureLogs_RecordedByUserId",
                table: "TemperatureLogs",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureLogs_StoreId",
                table: "TemperatureLogs",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureLogs_StoreId1",
                table: "TemperatureLogs",
                column: "StoreId1");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingHistories_LastUpdated",
                table: "TrackingHistories",
                column: "LastUpdated");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingHistories_ShipmentTrackingId",
                table: "TrackingHistories",
                column: "ShipmentTrackingId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferDiscrepancies_ItemId",
                table: "TransferDiscrepancies",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferDiscrepancies_TransferId",
                table: "TransferDiscrepancies",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_ItemId",
                table: "TransferItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_TransferId",
                table: "TransferItems",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_FromStoreId",
                table: "Transfers",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ToStoreId",
                table: "Transfers",
                column: "ToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferDate",
                table: "Transfers",
                column: "TransferDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferNo",
                table: "Transfers",
                column: "TransferNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferShipmentItems_ItemId",
                table: "TransferShipmentItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferShipmentItems_ShipmentId",
                table: "TransferShipmentItems",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferShipments_ShipmentNo",
                table: "TransferShipments",
                column: "ShipmentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferShipments_TransferId",
                table: "TransferShipments",
                column: "TransferId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unions_Code",
                table: "Unions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unions_UpazilaId",
                table: "Unions",
                column: "UpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Code",
                table: "Units",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Upazilas_Code",
                table: "Upazilas",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Upazilas_ZilaId",
                table: "Upazilas",
                column: "ZilaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationPreferences_UserId",
                table: "UserNotificationPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStores_StoreId",
                table: "UserStores",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStores_UserId_StoreId",
                table: "UserStores",
                columns: new[] { "UserId", "StoreId" },
                unique: true,
                filter: "[UnassignedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Email",
                table: "Vendors",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Name",
                table: "Vendors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_EndDate",
                table: "Warranties",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_ItemId",
                table: "Warranties",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_ItemId1",
                table: "Warranties",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_VendorId",
                table: "Warranties",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_WarrantyNumber",
                table: "Warranties",
                column: "WarrantyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffItems_ItemId",
                table: "WriteOffItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffItems_StoreId",
                table: "WriteOffItems",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffItems_WriteOffId",
                table: "WriteOffItems",
                column: "WriteOffId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffItems_WriteOffRequestId",
                table: "WriteOffItems",
                column: "WriteOffRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffItems_WriteOffRequestId1",
                table: "WriteOffItems",
                column: "WriteOffRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffRequests_DamageReportId",
                table: "WriteOffRequests",
                column: "DamageReportId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffRequests_RequestedBy",
                table: "WriteOffRequests",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffRequests_RequestNo",
                table: "WriteOffRequests",
                column: "RequestNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffRequests_StoreId",
                table: "WriteOffRequests",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffs_StoreId",
                table: "WriteOffs",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffs_WriteOffDate",
                table: "WriteOffs",
                column: "WriteOffDate");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffs_WriteOffNo",
                table: "WriteOffs",
                column: "WriteOffNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zilas_Code",
                table: "Zilas",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zilas_RangeId",
                table: "Zilas",
                column: "RangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConditionCheckItems_ConditionChecks_ConditionCheckId",
                table: "ConditionCheckItems",
                column: "ConditionCheckId",
                principalTable: "ConditionChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConditionChecks_Returns_ReturnId",
                table: "ConditionChecks",
                column: "ReturnId",
                principalTable: "Returns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DamageRecords_Returns_ReturnId",
                table: "DamageRecords",
                column: "ReturnId",
                principalTable: "Returns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueItems_Issues_IssueId",
                table: "IssueItems",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Signatures_ApproverSignatureId",
                table: "Issues",
                column: "ApproverSignatureId",
                principalTable: "Signatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Signatures_IssuerSignatureId",
                table: "Issues",
                column: "IssuerSignatureId",
                principalTable: "Signatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Signatures_ReceiverSignatureId",
                table: "Issues",
                column: "ReceiverSignatureId",
                principalTable: "Signatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_AspNetUsers_CreatedByUserId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_AllotmentLetters_AllotmentLetterId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Battalions_IssuedToBattalionId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Battalions_BattalionId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Ranges_IssuedToRangeId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Ranges_RangeId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Zilas_Ranges_RangeId",
                table: "Zilas");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Unions_IssuedToUnionId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Unions_UnionId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Upazilas_IssuedToUpazilaId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Upazilas_UpazilaId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Zilas_IssuedToZilaId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Zilas_ZilaId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Stores_FromStoreId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Signatures_Issues_IssueId",
                table: "Signatures");

            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "AllotmentLetterItems");

            migrationBuilder.DropTable(
                name: "AllotmentLetterRecipientItems");

            migrationBuilder.DropTable(
                name: "ApprovalDelegations");

            migrationBuilder.DropTable(
                name: "ApprovalHistories");

            migrationBuilder.DropTable(
                name: "ApprovalLevels");

            migrationBuilder.DropTable(
                name: "ApprovalSteps");

            migrationBuilder.DropTable(
                name: "ApprovalThresholds");

            migrationBuilder.DropTable(
                name: "ApprovalWorkflowLevels");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "AuditReports");

            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "Barcodes");

            migrationBuilder.DropTable(
                name: "BatchMovements");

            migrationBuilder.DropTable(
                name: "BatchSignatureItem");

            migrationBuilder.DropTable(
                name: "BattalionStores");

            migrationBuilder.DropTable(
                name: "ConditionCheckItems");

            migrationBuilder.DropTable(
                name: "CycleCountSchedules");

            migrationBuilder.DropTable(
                name: "DamageRecords");

            migrationBuilder.DropTable(
                name: "DamageReportItems");

            migrationBuilder.DropTable(
                name: "Damages");

            migrationBuilder.DropTable(
                name: "DigitalSignatures");

            migrationBuilder.DropTable(
                name: "DisposalRecords");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "ExpiredRecords");

            migrationBuilder.DropTable(
                name: "ExpiryTrackings");

            migrationBuilder.DropTable(
                name: "GoodsReceiveItems");

            migrationBuilder.DropTable(
                name: "InventoryCycleCountItems");

            migrationBuilder.DropTable(
                name: "InventoryValuations");

            migrationBuilder.DropTable(
                name: "IssueItems");

            migrationBuilder.DropTable(
                name: "IssueVouchers");

            migrationBuilder.DropTable(
                name: "ItemSpecifications");

            migrationBuilder.DropTable(
                name: "LoginLogs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PersonnelItemIssues");

            migrationBuilder.DropTable(
                name: "PhysicalInventoryDetails");

            migrationBuilder.DropTable(
                name: "PhysicalInventoryItems");

            migrationBuilder.DropTable(
                name: "PurchaseItems");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "QualityCheckItems");

            migrationBuilder.DropTable(
                name: "ReceiveItems");

            migrationBuilder.DropTable(
                name: "RequisitionItems");

            migrationBuilder.DropTable(
                name: "ReturnItems");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "SignatoryPresets");

            migrationBuilder.DropTable(
                name: "SignatureOTPs");

            migrationBuilder.DropTable(
                name: "StockAdjustmentItems");

            migrationBuilder.DropTable(
                name: "StockAlerts");

            migrationBuilder.DropTable(
                name: "StockEntryItems");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "StockOperations");

            migrationBuilder.DropTable(
                name: "StockReturns");

            migrationBuilder.DropTable(
                name: "StoreConfigurations");

            migrationBuilder.DropTable(
                name: "StoreStocks");

            migrationBuilder.DropTable(
                name: "StoreTypeCategories");

            migrationBuilder.DropTable(
                name: "SupplierEvaluations");

            migrationBuilder.DropTable(
                name: "SystemConfigurations");

            migrationBuilder.DropTable(
                name: "TemperatureLogs");

            migrationBuilder.DropTable(
                name: "TrackingHistories");

            migrationBuilder.DropTable(
                name: "TransferDiscrepancies");

            migrationBuilder.DropTable(
                name: "TransferItems");

            migrationBuilder.DropTable(
                name: "TransferShipmentItems");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "UsageStatistics");

            migrationBuilder.DropTable(
                name: "UserNotificationPreferences");

            migrationBuilder.DropTable(
                name: "UserStores");

            migrationBuilder.DropTable(
                name: "Warranties");

            migrationBuilder.DropTable(
                name: "WriteOffItems");

            migrationBuilder.DropTable(
                name: "AllotmentLetterRecipients");

            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DropTable(
                name: "ApprovalWorkflows");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BatchSignatures");

            migrationBuilder.DropTable(
                name: "ConditionChecks");

            migrationBuilder.DropTable(
                name: "InventoryCycleCounts");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "QualityChecks");

            migrationBuilder.DropTable(
                name: "Receives");

            migrationBuilder.DropTable(
                name: "Requisitions");

            migrationBuilder.DropTable(
                name: "StockAdjustments");

            migrationBuilder.DropTable(
                name: "StockEntries");

            migrationBuilder.DropTable(
                name: "StoreItems");

            migrationBuilder.DropTable(
                name: "ShipmentTrackings");

            migrationBuilder.DropTable(
                name: "TransferShipments");

            migrationBuilder.DropTable(
                name: "WriteOffRequests");

            migrationBuilder.DropTable(
                name: "WriteOffs");

            migrationBuilder.DropTable(
                name: "BatchTrackings");

            migrationBuilder.DropTable(
                name: "Returns");

            migrationBuilder.DropTable(
                name: "GoodsReceives");

            migrationBuilder.DropTable(
                name: "PhysicalInventories");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "DamageReports");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "ItemModels");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AllotmentLetters");

            migrationBuilder.DropTable(
                name: "Battalions");

            migrationBuilder.DropTable(
                name: "Ranges");

            migrationBuilder.DropTable(
                name: "Unions");

            migrationBuilder.DropTable(
                name: "Upazilas");

            migrationBuilder.DropTable(
                name: "Zilas");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "StoreTypes");

            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropTable(
                name: "Signatures");
        }
    }
}
