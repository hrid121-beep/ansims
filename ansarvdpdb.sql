USE [master]
GO
/****** Object:  Database [ansvdp_ims]    Script Date: 11/6/2025 1:16:42 PM ******/
CREATE DATABASE [ansvdp_ims]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ansvdp_ims', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ansvdp_ims.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ansvdp_ims_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ansvdp_ims_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [ansvdp_ims] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ansvdp_ims].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ansvdp_ims] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ansvdp_ims] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ansvdp_ims] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ansvdp_ims] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ansvdp_ims] SET ARITHABORT OFF 
GO
ALTER DATABASE [ansvdp_ims] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [ansvdp_ims] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ansvdp_ims] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ansvdp_ims] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ansvdp_ims] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ansvdp_ims] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ansvdp_ims] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ansvdp_ims] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ansvdp_ims] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ansvdp_ims] SET  ENABLE_BROKER 
GO
ALTER DATABASE [ansvdp_ims] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ansvdp_ims] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ansvdp_ims] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ansvdp_ims] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ansvdp_ims] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ansvdp_ims] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [ansvdp_ims] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ansvdp_ims] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ansvdp_ims] SET  MULTI_USER 
GO
ALTER DATABASE [ansvdp_ims] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ansvdp_ims] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ansvdp_ims] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ansvdp_ims] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ansvdp_ims] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ansvdp_ims] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [ansvdp_ims] SET QUERY_STORE = OFF
GO
USE [ansvdp_ims]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActivityLogs]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [nvarchar](50) NOT NULL,
	[ActionDate] [datetime2](7) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[Details] [nvarchar](1000) NULL,
	[UserId] [nvarchar](4000) NULL,
	[UserName] [nvarchar](4000) NULL,
	[IPAddress] [nvarchar](4000) NULL,
	[EntityType] [nvarchar](50) NOT NULL,
	[Entity] [nvarchar](4000) NULL,
	[EntityName] [nvarchar](4000) NULL,
	[EntityId] [int] NULL,
	[Module] [nvarchar](4000) NULL,
	[OldValue] [nvarchar](4000) NULL,
	[NewValue] [nvarchar](4000) NULL,
	[OldValues] [nvarchar](4000) NULL,
	[NewValues] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ActivityLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AllotmentLetterItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AllotmentLetterItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AllotmentLetterId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[AllottedQuantity] [decimal](18, 2) NOT NULL,
	[IssuedQuantity] [decimal](18, 2) NOT NULL,
	[RemainingQuantity] [decimal](18, 2) NOT NULL,
	[Unit] [nvarchar](4000) NULL,
	[UnitBn] [nvarchar](4000) NULL,
	[ItemNameBn] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AllotmentLetterItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AllotmentLetterRecipientItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AllotmentLetterRecipientItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AllotmentLetterRecipientId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[AllottedQuantity] [decimal](18, 2) NOT NULL,
	[IssuedQuantity] [decimal](18, 2) NOT NULL,
	[Unit] [nvarchar](4000) NULL,
	[UnitBn] [nvarchar](4000) NULL,
	[ItemNameBn] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AllotmentLetterRecipientItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AllotmentLetterRecipients]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AllotmentLetterRecipients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AllotmentLetterId] [int] NOT NULL,
	[RecipientType] [nvarchar](4000) NULL,
	[RangeId] [int] NULL,
	[BattalionId] [int] NULL,
	[ZilaId] [int] NULL,
	[UpazilaId] [int] NULL,
	[UnionId] [int] NULL,
	[RecipientName] [nvarchar](4000) NULL,
	[RecipientNameBn] [nvarchar](4000) NULL,
	[StaffStrength] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[SerialNo] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AllotmentLetterRecipients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AllotmentLetters]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AllotmentLetters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AllotmentNo] [nvarchar](50) NOT NULL,
	[AllotmentDate] [datetime2](7) NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidUntil] [datetime2](7) NOT NULL,
	[IssuedTo] [nvarchar](4000) NULL,
	[IssuedToType] [nvarchar](4000) NULL,
	[IssuedToBattalionId] [int] NULL,
	[IssuedToRangeId] [int] NULL,
	[IssuedToZilaId] [int] NULL,
	[IssuedToUpazilaId] [int] NULL,
	[FromStoreId] [int] NOT NULL,
	[Purpose] [nvarchar](4000) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[RejectedBy] [nvarchar](4000) NULL,
	[RejectedDate] [datetime2](7) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[DocumentPath] [nvarchar](4000) NULL,
	[ReferenceNo] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[Subject] [nvarchar](4000) NULL,
	[SubjectBn] [nvarchar](4000) NULL,
	[BodyText] [nvarchar](4000) NULL,
	[BodyTextBn] [nvarchar](4000) NULL,
	[CollectionDeadline] [datetime2](7) NULL,
	[SignatoryName] [nvarchar](4000) NULL,
	[SignatoryDesignation] [nvarchar](4000) NULL,
	[SignatoryDesignationBn] [nvarchar](4000) NULL,
	[SignatoryId] [nvarchar](4000) NULL,
	[SignatoryPhone] [nvarchar](4000) NULL,
	[SignatoryEmail] [nvarchar](4000) NULL,
	[BengaliDate] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AllotmentLetters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApprovalDelegations]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApprovalDelegations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FromUserId] [nvarchar](4000) NULL,
	[ToUserId] [nvarchar](4000) NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[EntityType] [nvarchar](4000) NULL,
	[Reason] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApprovalDelegations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApprovalHistories]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApprovalHistories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityType] [nvarchar](4000) NULL,
	[EntityId] [int] NOT NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NOT NULL,
	[Comments] [nvarchar](4000) NULL,
	[Action] [nvarchar](4000) NULL,
	[ApprovalLevel] [int] NOT NULL,
	[ApproverRole] [nvarchar](4000) NULL,
	[ActionBy] [nvarchar](4000) NULL,
	[ActionDate] [datetime2](7) NOT NULL,
	[NewStatus] [nvarchar](4000) NULL,
	[PreviousStatus] [nvarchar](4000) NULL,
	[ApprovalRequestId] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApprovalHistories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApprovalLevels]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApprovalLevels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Level] [int] NOT NULL,
	[Role] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[EntityType] [nvarchar](100) NULL,
	[IsSystemDefined] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApprovalLevels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApprovalRequests]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApprovalRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestType] [nvarchar](4000) NULL,
	[ApprovalType] [nvarchar](4000) NULL,
	[EntityId] [int] NOT NULL,
	[EntityType] [nvarchar](4000) NULL,
	[RequestedBy] [nvarchar](4000) NULL,
	[RequestedDate] [datetime2](7) NOT NULL,
	[RequestDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](4000) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[RejectedBy] [nvarchar](4000) NULL,
	[RejectedDate] [datetime2](7) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[ApprovalValue] [decimal](18, 2) NULL,
	[Amount] [decimal](18, 2) NULL,
	[ApproverRole] [nvarchar](4000) NULL,
	[ApprovalLevel] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[CurrentLevel] [int] NOT NULL,
	[MaxLevel] [int] NOT NULL,
	[WorkflowId] [int] NOT NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[IsEscalated] [bit] NOT NULL,
	[EscalatedDate] [datetime2](7) NULL,
	[CompletedDate] [datetime2](7) NULL,
	[Priority] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[Comments] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[EntityData] [nvarchar](4000) NULL,
	[Role] [nvarchar](4000) NULL,
	[RequiredRole] [nvarchar](4000) NULL,
	[RequestedByUserId] [nvarchar](4000) NULL,
	[ApprovedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApprovalRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApprovalSteps]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApprovalSteps](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalRequestId] [int] NOT NULL,
	[StepLevel] [int] NOT NULL,
	[ApproverRole] [nvarchar](4000) NULL,
	[AssignedTo] [nvarchar](4000) NULL,
	[Status] [int] NOT NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[Comments] [nvarchar](4000) NULL,
	[IsEscalated] [bit] NOT NULL,
	[EscalatedAt] [datetime2](7) NULL,
	[SpecificApproverId] [nvarchar](4000) NULL,
	[EscalatedBy] [nvarchar](4000) NULL,
	[EscalatedDate] [datetime2](7) NOT NULL,
	[EscalationReason] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApprovalSteps] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApprovalThresholds]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApprovalThresholds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityType] [nvarchar](4000) NULL,
	[MinAmount] [decimal](18, 2) NOT NULL,
	[MaxAmount] [decimal](18, 2) NULL,
	[ApprovalLevel] [int] NOT NULL,
	[RequiredRole] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApprovalThresholds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApprovalWorkflowLevels]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApprovalWorkflowLevels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkflowId] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[ApproverRole] [nvarchar](4000) NULL,
	[SpecificApproverId] [nvarchar](4000) NULL,
	[ThresholdAmount] [decimal](18, 2) NULL,
	[CanEscalate] [bit] NOT NULL,
	[TimeoutHours] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApprovalWorkflowLevels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApprovalWorkflows]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApprovalWorkflows](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkflowName] [nvarchar](4000) NULL,
	[Name] [nvarchar](4000) NULL,
	[EntityType] [nvarchar](4000) NULL,
	[TriggerCondition] [nvarchar](4000) NULL,
	[StepOrder] [int] NOT NULL,
	[RequiredLevels] [int] NOT NULL,
	[ApproverRole] [nvarchar](4000) NULL,
	[ApproverUserId] [nvarchar](4000) NULL,
	[IsRequired] [bit] NOT NULL,
	[ThresholdValue] [decimal](18, 2) NULL,
	[ThresholdField] [nvarchar](4000) NULL,
	[MinAmount] [decimal](18, 2) NULL,
	[MaxAmount] [decimal](18, 2) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ApprovalWorkflows] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](4000) NOT NULL,
	[ClaimType] [nvarchar](4000) NULL,
	[ClaimValue] [nvarchar](4000) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](4000) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](4000) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](4000) NOT NULL,
	[ClaimType] [nvarchar](4000) NULL,
	[ClaimValue] [nvarchar](4000) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](4000) NOT NULL,
	[ProviderKey] [nvarchar](4000) NOT NULL,
	[ProviderDisplayName] [nvarchar](4000) NULL,
	[UserId] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](4000) NOT NULL,
	[RoleId] [nvarchar](4000) NOT NULL,
	[UserId1] [nvarchar](4000) NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](4000) NOT NULL,
	[FullName] [nvarchar](200) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Department] [nvarchar](100) NULL,
	[Designation] [nvarchar](100) NULL,
	[BadgeNumber] [nvarchar](50) NULL,
	[BattalionId] [int] NULL,
	[RangeId] [int] NULL,
	[ZilaId] [int] NULL,
	[UpazilaId] [int] NULL,
	[UnionId] [int] NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](4000) NULL,
	[SecurityStamp] [nvarchar](4000) NULL,
	[ConcurrencyStamp] [nvarchar](4000) NULL,
	[PhoneNumber] [nvarchar](4000) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](4000) NOT NULL,
	[LoginProvider] [nvarchar](4000) NOT NULL,
	[Name] [nvarchar](4000) NOT NULL,
	[Value] [nvarchar](4000) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditLogs]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](4000) NULL,
	[UserName] [nvarchar](4000) NULL,
	[Action] [nvarchar](4000) NULL,
	[EntityName] [nvarchar](4000) NULL,
	[EntityId] [int] NULL,
	[OldValues] [nvarchar](4000) NULL,
	[NewValues] [nvarchar](4000) NULL,
	[IPAddress] [nvarchar](4000) NULL,
	[UserAgent] [nvarchar](4000) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[Changes] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditReports]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditReports](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceNumber] [nvarchar](4000) NULL,
	[EntityType] [nvarchar](4000) NULL,
	[EntityId] [int] NOT NULL,
	[AuditType] [nvarchar](4000) NULL,
	[AuditDate] [datetime2](7) NOT NULL,
	[AuditorName] [nvarchar](4000) NULL,
	[Findings] [nvarchar](4000) NULL,
	[Recommendations] [nvarchar](4000) NULL,
	[ComplianceStatus] [nvarchar](4000) NULL,
	[FiscalYear] [nvarchar](4000) NULL,
	[InventoryId] [int] NULL,
	[GeneratedDate] [datetime2](7) NULL,
	[StoreLevel] [nvarchar](4000) NULL,
	[BattalionName] [nvarchar](4000) NULL,
	[RangeName] [nvarchar](4000) NULL,
	[ZilaName] [nvarchar](4000) NULL,
	[UpazilaName] [nvarchar](4000) NULL,
	[TotalSystemValue] [decimal](18, 2) NOT NULL,
	[TotalPhysicalValue] [decimal](18, 2) NOT NULL,
	[TotalVarianceValue] [decimal](18, 2) NOT NULL,
	[AuditFindingsJson] [nvarchar](4000) NULL,
	[PhysicalInventoryId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AuditReports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Audits]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Audits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Entity] [nvarchar](4000) NULL,
	[EntityId] [int] NOT NULL,
	[Action] [nvarchar](4000) NULL,
	[Changes] [nvarchar](4000) NULL,
	[UserId] [nvarchar](4000) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[IsRead] [bit] NOT NULL,
	[ReadAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Audits] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Barcodes]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Barcodes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BarcodeNumber] [nvarchar](100) NOT NULL,
	[BarcodeType] [nvarchar](50) NULL,
	[BarcodeData] [nvarchar](max) NULL,
	[ReferenceType] [nvarchar](50) NULL,
	[ReferenceId] [int] NULL,
	[ItemId] [int] NULL,
	[StoreId] [int] NULL,
	[BatchNumber] [nvarchar](50) NULL,
	[SerialNumber] [nvarchar](100) NULL,
	[Location] [nvarchar](4000) NULL,
	[Notes] [nvarchar](4000) NULL,
	[GeneratedDate] [datetime2](7) NOT NULL,
	[GeneratedBy] [nvarchar](4000) NULL,
	[PrintedBy] [nvarchar](4000) NULL,
	[PrintedDate] [datetime2](7) NULL,
	[PrintCount] [int] NOT NULL,
	[LastScannedDate] [datetime2](7) NULL,
	[LastScannedBy] [nvarchar](4000) NULL,
	[LastScannedLocation] [nvarchar](4000) NULL,
	[ScanCount] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Barcodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchMovements]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchMovements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchId] [int] NOT NULL,
	[MovementType] [nvarchar](4000) NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[BalanceAfter] [decimal](18, 3) NOT NULL,
	[MovementDate] [datetime2](7) NOT NULL,
	[ReferenceType] [nvarchar](4000) NULL,
	[ReferenceNo] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[BatchTrackingId] [int] NOT NULL,
	[ReferenceId] [int] NOT NULL,
	[MovedBy] [nvarchar](4000) NULL,
	[BalanceBefore] [decimal](18, 3) NOT NULL,
	[NewBalance] [decimal](18, 3) NOT NULL,
	[OldBalance] [decimal](18, 3) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_BatchMovements] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchSignatureItem]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchSignatureItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchSignatureId] [int] NOT NULL,
	[EntityType] [nvarchar](4000) NULL,
	[EntityId] [int] NOT NULL,
	[ItemDescription] [nvarchar](4000) NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[Remarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_BatchSignatureItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchSignatures]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchSignatures](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchNo] [nvarchar](50) NOT NULL,
	[SignedBy] [nvarchar](4000) NULL,
	[SignedDate] [datetime2](7) NOT NULL,
	[TotalItems] [int] NOT NULL,
	[Purpose] [nvarchar](4000) NULL,
	[SignatureData] [nvarchar](max) NULL,
	[SignatureHash] [nvarchar](500) NULL,
	[BatchId] [int] NOT NULL,
	[SignatureType] [nvarchar](4000) NULL,
	[SignedAt] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_BatchSignatures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchTrackings]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchTrackings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchNumber] [nvarchar](50) NOT NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[InitialQuantity] [decimal](18, 3) NOT NULL,
	[ManufactureDate] [datetime2](7) NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[SupplierBatchNo] [nvarchar](4000) NULL,
	[CostPrice] [decimal](18, 2) NOT NULL,
	[SellingPrice] [decimal](18, 2) NOT NULL,
	[Location] [nvarchar](4000) NULL,
	[Status] [nvarchar](4000) NULL,
	[ConsumedDate] [datetime2](7) NULL,
	[Notes] [nvarchar](4000) NULL,
	[CurrentQuantity] [decimal](18, 3) NOT NULL,
	[ReservedQuantity] [decimal](18, 3) NOT NULL,
	[Supplier] [nvarchar](4000) NULL,
	[UnitCost] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[RemainingQuantity] [decimal](18, 3) NULL,
	[LastIssueDate] [datetime2](7) NOT NULL,
	[ReceivedDate] [datetime2](7) NOT NULL,
	[ReceivedQuantity] [decimal](18, 3) NOT NULL,
	[VendorId] [nvarchar](4000) NULL,
	[VendorBatchNo] [nvarchar](4000) NULL,
	[PurchaseOrderNo] [nvarchar](4000) NULL,
	[QualityCheckStatus] [nvarchar](4000) NULL,
	[QualityCheckDate] [datetime2](7) NULL,
	[QualityCheckBy] [nvarchar](4000) NULL,
	[StorageLocation] [nvarchar](4000) NULL,
	[Temperature] [decimal](5, 2) NULL,
	[Humidity] [decimal](5, 2) NULL,
	[QuarantineDate] [datetime2](7) NULL,
	[QuarantinedBy] [nvarchar](4000) NULL,
	[QuarantineReason] [nvarchar](4000) NULL,
	[TransferredFromBatchId] [int] NULL,
	[TransferReference] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_BatchTrackings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Battalions]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Battalions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Type] [int] NOT NULL,
	[Location] [nvarchar](200) NULL,
	[CommanderName] [nvarchar](100) NULL,
	[CommanderRank] [nvarchar](50) NULL,
	[ContactNumber] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[RangeId] [int] NULL,
	[Remarks] [nvarchar](4000) NULL,
	[TotalPersonnel] [int] NOT NULL,
	[OfficerCount] [int] NOT NULL,
	[EnlistedCount] [int] NOT NULL,
	[EstablishedDate] [datetime2](7) NULL,
	[OperationalStatus] [int] NULL,
	[NameBn] [nvarchar](100) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Battalions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BattalionStores]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BattalionStores](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BattalionId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[IsPrimaryStore] [bit] NOT NULL,
	[EffectiveFrom] [datetime2](7) NOT NULL,
	[EffectiveTo] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_BattalionStores] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Brands]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Code] [nvarchar](20) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Brands] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[NameBn] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NULL,
	[Code] [nvarchar](20) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConditionCheckItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConditionCheckItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConditionCheckId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[CheckedQuantity] [decimal](18, 2) NOT NULL,
	[GoodQuantity] [decimal](18, 2) NOT NULL,
	[DamagedQuantity] [decimal](18, 2) NOT NULL,
	[ExpiredQuantity] [decimal](18, 2) NOT NULL,
	[Condition] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[Photos] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ConditionCheckItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConditionChecks]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConditionChecks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReturnId] [int] NOT NULL,
	[CheckedBy] [nvarchar](450) NULL,
	[CheckedDate] [datetime2](7) NOT NULL,
	[OverallCondition] [nvarchar](100) NULL,
	[ItemId] [int] NOT NULL,
	[Condition] [nvarchar](100) NULL,
	[CheckDate] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ConditionChecks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CycleCountSchedules]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CycleCountSchedules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleCode] [nvarchar](50) NULL,
	[ScheduleName] [nvarchar](100) NULL,
	[StoreId] [int] NOT NULL,
	[Frequency] [nvarchar](50) NULL,
	[CountMethod] [nvarchar](50) NULL,
	[NextScheduledDate] [datetime2](7) NOT NULL,
	[LastExecutedDate] [datetime2](7) NULL,
	[ABCClass] [nvarchar](20) NULL,
	[MinimumValue] [decimal](18, 2) NULL,
	[ItemsPerCount] [int] NULL,
	[AssignedTo] [nvarchar](450) NULL,
	[Notes] [nvarchar](500) NULL,
	[Description] [nvarchar](500) NULL,
	[DayOfWeek] [int] NOT NULL,
	[Status] [nvarchar](20) NULL,
	[CategoryId] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_CycleCountSchedules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DamageRecords]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DamageRecords](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NOT NULL,
	[DamagedQuantity] [decimal](18, 2) NOT NULL,
	[DamageDate] [datetime2](7) NOT NULL,
	[DamageReason] [nvarchar](4000) NULL,
	[ReferenceType] [nvarchar](4000) NULL,
	[ReferenceNo] [nvarchar](4000) NULL,
	[Status] [nvarchar](4000) NULL,
	[ReturnId] [int] NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[DamageType] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DamageRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DamageReportItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DamageReportItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DamageReportId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[DamagedQuantity] [decimal](18, 2) NOT NULL,
	[DamageType] [nvarchar](4000) NULL,
	[DamageDate] [datetime2](7) NOT NULL,
	[DiscoveredDate] [datetime2](7) NOT NULL,
	[DamageDescription] [nvarchar](4000) NULL,
	[EstimatedValue] [decimal](18, 2) NOT NULL,
	[PhotoUrls] [nvarchar](max) NULL,
	[BatchNo] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DamageReportItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DamageReports]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DamageReports](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReportNo] [nvarchar](50) NOT NULL,
	[StoreId] [int] NOT NULL,
	[ReportDate] [datetime2](7) NOT NULL,
	[ReportedBy] [nvarchar](4000) NULL,
	[Status] [int] NOT NULL,
	[TotalValue] [decimal](18, 2) NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[DamageType] [nvarchar](4000) NULL,
	[Cause] [nvarchar](4000) NULL,
	[EstimatedLoss] [decimal](18, 2) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DamageReports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Damages]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Damages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DamageNo] [nvarchar](50) NOT NULL,
	[DamageDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](4000) NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[Quantity] [decimal](18, 2) NULL,
	[DamageType] [nvarchar](4000) NULL,
	[Cause] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[ActionTaken] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[ReportedBy] [nvarchar](4000) NULL,
	[PhotoPath] [nvarchar](4000) NULL,
	[EstimatedLoss] [decimal](18, 2) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Damages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DigitalSignatures]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DigitalSignatures](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceType] [nvarchar](4000) NULL,
	[ReferenceId] [int] NOT NULL,
	[SignatureType] [nvarchar](50) NULL,
	[SignedBy] [nvarchar](4000) NULL,
	[SignedAt] [datetime2](7) NOT NULL,
	[SignatureData] [nvarchar](max) NULL,
	[DeviceInfo] [nvarchar](4000) NULL,
	[IPAddress] [nvarchar](4000) NULL,
	[LocationInfo] [nvarchar](4000) NULL,
	[IsVerified] [bit] NOT NULL,
	[VerifiedBy] [nvarchar](4000) NULL,
	[VerificationDate] [datetime2](7) NULL,
	[EntityType] [nvarchar](50) NULL,
	[EntityId] [int] NOT NULL,
	[SignedDate] [datetime2](7) NOT NULL,
	[SignatureHash] [nvarchar](500) NULL,
	[DeviceId] [nvarchar](4000) NULL,
	[Location] [nvarchar](4000) NULL,
	[VerifiedDate] [datetime2](7) NULL,
	[VerificationCode] [nvarchar](4000) NULL,
	[VerificationMethod] [nvarchar](4000) NULL,
	[VerificationFailReason] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DigitalSignatures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DisposalRecords]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DisposalRecords](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WriteOffId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[DisposalMethod] [nvarchar](4000) NULL,
	[DisposalDate] [datetime2](7) NOT NULL,
	[DisposalLocation] [nvarchar](4000) NULL,
	[DisposalCompany] [nvarchar](4000) NULL,
	[DisposalCertificateNo] [nvarchar](4000) NULL,
	[DisposedBy] [nvarchar](4000) NULL,
	[WitnessedBy] [nvarchar](4000) NULL,
	[PhotoUrls] [nvarchar](max) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[DisposalNo] [nvarchar](50) NULL,
	[AuthorizedBy] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DisposalRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Documents]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Documents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocumentName] [nvarchar](4000) NULL,
	[FileName] [nvarchar](4000) NULL,
	[FilePath] [nvarchar](4000) NULL,
	[ContentType] [nvarchar](4000) NULL,
	[FileSize] [bigint] NOT NULL,
	[EntityType] [nvarchar](4000) NULL,
	[EntityId] [int] NOT NULL,
	[DocumentType] [nvarchar](4000) NULL,
	[UploadedBy] [nvarchar](4000) NULL,
	[UploadDate] [datetime2](7) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[IsPublic] [bit] NOT NULL,
	[UploadedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExpiredRecords]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpiredRecords](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NOT NULL,
	[ExpiredQuantity] [decimal](18, 3) NOT NULL,
	[ExpiryDate] [datetime2](7) NOT NULL,
	[ReferenceType] [nvarchar](4000) NULL,
	[ReferenceNo] [nvarchar](4000) NULL,
	[Status] [nvarchar](4000) NULL,
	[BatchId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[DisposalMethod] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ExpiredRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExpiryTrackings]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpiryTrackings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExpiryDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](4000) NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[BatchNumber] [nvarchar](50) NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[DisposalDate] [datetime2](7) NULL,
	[DisposalReason] [nvarchar](4000) NULL,
	[DisposedBy] [nvarchar](4000) NULL,
	[IsAlertSent] [bit] NOT NULL,
	[AlertSentDate] [datetime2](7) NULL,
	[DisposedByUserId] [nvarchar](4000) NULL,
	[ItemId1] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ExpiryTrackings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoodsReceiveItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoodsReceiveItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GoodsReceiveId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[ReceivedQuantity] [decimal](18, 3) NOT NULL,
	[AcceptedQuantity] [decimal](18, 3) NULL,
	[RejectedQuantity] [decimal](18, 3) NULL,
	[QualityCheckStatus] [int] NOT NULL,
	[BatchNo] [nvarchar](4000) NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[ManufactureDate] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_GoodsReceiveItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoodsReceives]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoodsReceives](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseId] [int] NOT NULL,
	[ReceiveDate] [datetime2](7) NOT NULL,
	[ReceivedBy] [nvarchar](4000) NULL,
	[InvoiceNo] [nvarchar](4000) NULL,
	[ChallanNo] [nvarchar](4000) NULL,
	[Status] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_GoodsReceives] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InventoryCycleCountItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryCycleCountItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CycleCountId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[SystemQuantity] [decimal](18, 3) NULL,
	[CountedQuantity] [decimal](18, 3) NULL,
	[Variance] [decimal](18, 3) NULL,
	[VarianceQuantity] [decimal](18, 3) NOT NULL,
	[VarianceValue] [decimal](18, 2) NOT NULL,
	[VarianceReason] [nvarchar](4000) NULL,
	[Comments] [nvarchar](4000) NULL,
	[CountedById] [nvarchar](4000) NULL,
	[CountedDate] [datetime2](7) NULL,
	[IsRecounted] [bit] NOT NULL,
	[IsAdjusted] [bit] NOT NULL,
	[AdjustedBy] [nvarchar](4000) NULL,
	[AdjustmentDate] [datetime2](7) NULL,
	[CountedByUserId] [nvarchar](4000) NULL,
	[AdjustedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_InventoryCycleCountItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InventoryCycleCounts]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryCycleCounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountNumber] [nvarchar](50) NULL,
	[CountDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[CountType] [nvarchar](50) NULL,
	[StoreId] [int] NULL,
	[CreatedById] [nvarchar](450) NULL,
	[ApprovedById] [nvarchar](450) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[CountedBy] [nvarchar](450) NULL,
	[VerifiedBy] [nvarchar](450) NULL,
	[StartTime] [datetime2](7) NULL,
	[EndTime] [datetime2](7) NULL,
	[Duration] [time](7) NULL,
	[TotalItems] [int] NOT NULL,
	[CountedItems] [int] NOT NULL,
	[VarianceItems] [int] NOT NULL,
	[TotalVarianceValue] [decimal](18, 2) NOT NULL,
	[Notes] [nvarchar](1000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_InventoryCycleCounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InventoryValuations]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryValuations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ValuationDate] [datetime2](7) NOT NULL,
	[CalculationDate] [datetime2](7) NOT NULL,
	[ValuationType] [nvarchar](50) NULL,
	[CostingMethod] [nvarchar](50) NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[Quantity] [decimal](18, 3) NULL,
	[UnitCost] [decimal](18, 2) NULL,
	[TotalValue] [decimal](18, 2) NULL,
	[CalculatedBy] [nvarchar](450) NULL,
	[ItemId1] [int] NULL,
	[StoreId1] [int] NULL,
	[CalculatedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_InventoryValuations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IssueItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IssueItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IssueId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[IssuedQuantity] [decimal](18, 3) NOT NULL,
	[RequestedQuantity] [decimal](18, 3) NOT NULL,
	[ApprovedQuantity] [decimal](18, 3) NOT NULL,
	[Unit] [nvarchar](4000) NULL,
	[BatchNumber] [nvarchar](4000) NULL,
	[Condition] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[HandoverRemarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
	[LedgerNo] [nvarchar](4000) NULL,
	[PageNo] [nvarchar](4000) NULL,
	[PartiallyUsableQuantity] [decimal](18, 2) NULL,
	[UnusableQuantity] [decimal](18, 2) NULL,
	[UsableQuantity] [decimal](18, 2) NULL,
 CONSTRAINT [PK_IssueItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Issues]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Issues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IssuerSignatureId] [int] NULL,
	[ApproverSignatureId] [int] NULL,
	[ReceiverSignatureId] [int] NULL,
	[SignerName] [nvarchar](4000) NULL,
	[SignerBadgeId] [nvarchar](4000) NULL,
	[SignerDesignation] [nvarchar](4000) NULL,
	[SignedDate] [datetime2](7) NULL,
	[IssueNo] [nvarchar](50) NOT NULL,
	[IssueNumber] [nvarchar](4000) NULL,
	[IssueDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[IssuedTo] [nvarchar](4000) NULL,
	[IssuedToType] [nvarchar](4000) NULL,
	[Purpose] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[RequestedBy] [nvarchar](4000) NULL,
	[RequestedDate] [datetime2](7) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[ApprovalRemarks] [nvarchar](4000) NULL,
	[ApprovalReferenceNo] [nvarchar](4000) NULL,
	[ApprovalComments] [nvarchar](4000) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[ApprovedByName] [nvarchar](4000) NULL,
	[ApprovedByBadgeNo] [nvarchar](4000) NULL,
	[SignaturePath] [nvarchar](max) NULL,
	[SignatureDate] [datetime2](7) NULL,
	[ReceivedDate] [datetime2](7) NOT NULL,
	[ReceivedBy] [nvarchar](4000) NULL,
	[ReceiverDesignation] [nvarchar](4000) NULL,
	[ReceiverContact] [nvarchar](4000) NULL,
	[IssuedToBattalionId] [int] NULL,
	[IssuedToRangeId] [int] NULL,
	[IssuedToZilaId] [int] NULL,
	[IssuedToUpazilaId] [int] NULL,
	[IssuedToUnionId] [int] NULL,
	[IssuedToIndividualName] [nvarchar](4000) NULL,
	[IssuedToIndividualBadgeNo] [nvarchar](4000) NULL,
	[IssuedToIndividualMobile] [nvarchar](4000) NULL,
	[ToEntityType] [nvarchar](4000) NULL,
	[ToEntityId] [int] NOT NULL,
	[FromStoreId] [int] NULL,
	[DeliveryLocation] [nvarchar](4000) NULL,
	[VoucherNo] [nvarchar](4000) NULL,
	[VoucherNumber] [nvarchar](4000) NULL,
	[VoucherDate] [datetime2](7) NULL,
	[VoucherGeneratedDate] [datetime2](7) NULL,
	[VoucherQRCode] [nvarchar](4000) NULL,
	[QRCode] [nvarchar](4000) NULL,
	[IssuedBy] [nvarchar](4000) NULL,
	[IssuedDate] [datetime2](7) NULL,
	[IsPartialIssue] [bit] NOT NULL,
	[ParentIssueId] [int] NULL,
	[CreatedByUserId] [nvarchar](4000) NULL,
	[RejectedBy] [nvarchar](4000) NULL,
	[RejectedDate] [datetime2](7) NOT NULL,
	[VoucherDocumentPath] [nvarchar](4000) NULL,
	[MemoNo] [nvarchar](4000) NULL,
	[MemoDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedBy] [nvarchar](4000) NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletionReason] [nvarchar](4000) NULL,
	[AllotmentLetterId] [int] NULL,
	[AllotmentLetterNo] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Issues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IssueVouchers]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IssueVouchers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VoucherNumber] [nvarchar](50) NOT NULL,
	[VoucherNo] [nvarchar](50) NULL,
	[IssueDate] [datetime2](7) NOT NULL,
	[IssueId] [int] NOT NULL,
	[IssuedTo] [nvarchar](200) NULL,
	[Department] [nvarchar](100) NULL,
	[Purpose] [nvarchar](500) NULL,
	[AuthorizedBy] [nvarchar](100) NULL,
	[ReceivedBy] [nvarchar](100) NULL,
	[ReceiverSignature] [varbinary](max) NULL,
	[VoucherBarcode] [nvarchar](200) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_IssueVouchers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ItemModels]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemModels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](4000) NULL,
	[ModelNumber] [nvarchar](4000) NULL,
	[BrandId] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ItemModels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Items]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Items](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[NameBn] [nvarchar](200) NULL,
	[ItemCode] [nvarchar](50) NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Description] [nvarchar](1000) NULL,
	[Unit] [nvarchar](20) NOT NULL,
	[Type] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[SubCategoryId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[ItemModelId] [int] NULL,
	[BrandId] [int] NULL,
	[MinimumStock] [decimal](18, 3) NULL,
	[MaximumStock] [decimal](18, 3) NULL,
	[ReorderLevel] [decimal](18, 3) NOT NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[UnitCost] [decimal](18, 2) NULL,
	[Manufacturer] [nvarchar](4000) NULL,
	[ManufactureDate] [datetime2](7) NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[HasExpiry] [bit] NOT NULL,
	[ShelfLife] [int] NULL,
	[StorageRequirements] [nvarchar](4000) NULL,
	[RequiresSpecialHandling] [bit] NOT NULL,
	[SafetyInstructions] [nvarchar](4000) NULL,
	[Weight] [decimal](18, 3) NULL,
	[WeightUnit] [nvarchar](4000) NULL,
	[Dimensions] [nvarchar](4000) NULL,
	[Color] [nvarchar](4000) NULL,
	[Material] [nvarchar](4000) NULL,
	[IsHazardous] [bit] NOT NULL,
	[HazardClass] [nvarchar](4000) NULL,
	[ItemImage] [nvarchar](4000) NULL,
	[ImagePath] [nvarchar](4000) NULL,
	[Barcode] [nvarchar](4000) NULL,
	[BarcodePath] [nvarchar](4000) NULL,
	[QRCodeData] [nvarchar](4000) NULL,
	[ItemControlType] [nvarchar](4000) NULL,
	[AnsarLifeSpanMonths] [int] NULL,
	[VDPLifeSpanMonths] [int] NULL,
	[AnsarAlertBeforeDays] [int] NULL,
	[VDPAlertBeforeDays] [int] NULL,
	[LifeSpanMonths] [int] NULL,
	[AlertBeforeDays] [int] NULL,
	[IsAnsarAuthorized] [bit] NOT NULL,
	[IsVDPAuthorized] [bit] NOT NULL,
	[RequiresPersonalIssue] [bit] NOT NULL,
	[AnsarEntitlementQuantity] [decimal](18, 2) NULL,
	[VDPEntitlementQuantity] [decimal](18, 2) NULL,
	[EntitlementPeriodMonths] [int] NULL,
	[RequiresHigherApproval] [bit] NOT NULL,
	[ControlItemCategory] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ItemSpecifications]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSpecifications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[SpecificationName] [nvarchar](4000) NULL,
	[SpecificationValue] [nvarchar](4000) NULL,
	[Unit] [nvarchar](4000) NULL,
	[Category] [nvarchar](4000) NULL,
	[IsRequired] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ItemSpecifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LedgerBooks]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LedgerBooks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LedgerNo] [nvarchar](4000) NOT NULL,
	[BookName] [nvarchar](4000) NOT NULL,
	[BookType] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[StoreId] [int] NULL,
	[TotalPages] [int] NOT NULL,
	[CurrentPageNo] [int] NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NULL,
	[IsClosed] [bit] NOT NULL,
	[Location] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_LedgerBooks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Locations]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Locations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Description] [nvarchar](500) NULL,
	[ParentLocationId] [int] NULL,
	[LocationType] [nvarchar](50) NULL,
	[Address] [nvarchar](500) NULL,
	[Latitude] [decimal](10, 6) NULL,
	[Longitude] [decimal](10, 6) NULL,
	[ContactPerson] [nvarchar](100) NULL,
	[ContactPhone] [nvarchar](50) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoginLogs]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoginLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](4000) NULL,
	[IpAddress] [nvarchar](4000) NULL,
	[LoginTime] [datetime2](7) NOT NULL,
	[LogoutTime] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_LoginLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Message] [nvarchar](4000) NOT NULL,
	[Type] [nvarchar](50) NULL,
	[Priority] [nvarchar](4000) NULL,
	[Category] [nvarchar](4000) NULL,
	[UserId] [nvarchar](4000) NULL,
	[TargetRole] [nvarchar](4000) NULL,
	[IsRead] [bit] NOT NULL,
	[ReadAt] [datetime2](7) NULL,
	[IsSent] [bit] NOT NULL,
	[SentAt] [datetime2](7) NOT NULL,
	[PushSent] [bit] NULL,
	[PushSentAt] [datetime2](7) NULL,
	[Url] [nvarchar](4000) NULL,
	[ActionUrl] [nvarchar](4000) NULL,
	[RelatedEntity] [nvarchar](4000) NULL,
	[RelatedEntityId] [int] NULL,
	[ReferenceType] [nvarchar](4000) NULL,
	[ReferenceId] [nvarchar](4000) NULL,
	[Metadata] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PersonnelItemIssues]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonnelItemIssues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IssueNo] [nvarchar](50) NOT NULL,
	[PersonnelId] [nvarchar](4000) NULL,
	[PersonnelType] [nvarchar](10) NOT NULL,
	[PersonnelName] [nvarchar](100) NOT NULL,
	[PersonnelBadgeNo] [nvarchar](50) NOT NULL,
	[PersonnelUnit] [nvarchar](4000) NULL,
	[PersonnelDesignation] [nvarchar](4000) NULL,
	[PersonnelMobile] [nvarchar](4000) NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Unit] [nvarchar](4000) NULL,
	[OriginalIssueId] [int] NULL,
	[ReceiveId] [int] NULL,
	[IssueDate] [datetime2](7) NOT NULL,
	[ReceivedDate] [datetime2](7) NULL,
	[LifeExpiryDate] [datetime2](7) NULL,
	[AlertDate] [datetime2](7) NULL,
	[RemainingDays] [int] NULL,
	[Status] [nvarchar](20) NOT NULL,
	[ReplacedDate] [datetime2](7) NULL,
	[ReplacementReason] [nvarchar](4000) NULL,
	[ReplacementIssueId] [int] NULL,
	[BattalionId] [int] NULL,
	[RangeId] [int] NULL,
	[ZilaId] [int] NULL,
	[UpazilaId] [int] NULL,
	[StoreId] [int] NULL,
	[IsAlertSent] [bit] NOT NULL,
	[LastAlertDate] [datetime2](7) NULL,
	[AlertCount] [int] NOT NULL,
	[Remarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PersonnelItemIssues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhysicalInventories]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhysicalInventories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountNo] [nvarchar](50) NOT NULL,
	[CountDate] [datetime2](7) NOT NULL,
	[StoreId] [int] NOT NULL,
	[CategoryId] [int] NULL,
	[CountedBy] [nvarchar](4000) NULL,
	[VerifiedBy] [nvarchar](4000) NULL,
	[StartTime] [datetime2](7) NULL,
	[EndTime] [datetime2](7) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[TotalSystemValue] [decimal](18, 2) NULL,
	[TotalPhysicalValue] [decimal](18, 2) NULL,
	[VarianceValue] [decimal](18, 2) NOT NULL,
	[TotalItemsCounted] [int] NOT NULL,
	[ItemsWithVariance] [int] NOT NULL,
	[IsReconciled] [bit] NOT NULL,
	[ReconciliationDate] [datetime2](7) NULL,
	[ReconciliationBy] [nvarchar](4000) NULL,
	[IsStockFrozen] [bit] NOT NULL,
	[StockFrozenAt] [datetime2](7) NULL,
	[CountTeam] [nvarchar](4000) NULL,
	[SupervisorId] [nvarchar](4000) NULL,
	[ReferenceNumber] [nvarchar](4000) NULL,
	[BattalionId] [int] NULL,
	[RangeId] [int] NULL,
	[ZilaId] [int] NULL,
	[UpazilaId] [int] NULL,
	[FiscalYear] [nvarchar](4000) NULL,
	[Status] [int] NOT NULL,
	[CountType] [int] NOT NULL,
	[InitiatedBy] [nvarchar](4000) NULL,
	[InitiatedDate] [datetime2](7) NULL,
	[CompletedBy] [nvarchar](4000) NULL,
	[CompletedDate] [datetime2](7) NULL,
	[ReviewedBy] [nvarchar](4000) NULL,
	[ReviewedDate] [datetime2](7) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[RejectedBy] [nvarchar](4000) NULL,
	[RejectedDate] [datetime2](7) NULL,
	[CancelledBy] [nvarchar](4000) NULL,
	[CancelledDate] [datetime2](7) NULL,
	[ApprovalReference] [nvarchar](4000) NULL,
	[ApprovalRemarks] [nvarchar](4000) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[CancellationReason] [nvarchar](4000) NULL,
	[ReviewRemarks] [nvarchar](4000) NULL,
	[TotalSystemQuantity] [decimal](18, 3) NULL,
	[TotalPhysicalQuantity] [decimal](18, 3) NULL,
	[TotalVariance] [decimal](18, 3) NULL,
	[TotalVarianceValue] [decimal](18, 2) NULL,
	[IsAuditRequired] [bit] NOT NULL,
	[AuditOfficer] [nvarchar](4000) NULL,
	[AdjustmentStatus] [int] NULL,
	[AdjustedDate] [datetime2](7) NULL,
	[CountEndTime] [datetime2](7) NOT NULL,
	[AdjustmentCreatedDate] [datetime2](7) NOT NULL,
	[AdjustmentNo] [nvarchar](4000) NULL,
	[PostedDate] [datetime2](7) NOT NULL,
	[CountedByUserId] [nvarchar](4000) NULL,
	[VerifiedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PhysicalInventories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhysicalInventoryDetails]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhysicalInventoryDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PhysicalInventoryId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[CategoryId] [int] NULL,
	[SystemQuantity] [decimal](18, 3) NOT NULL,
	[PhysicalQuantity] [decimal](18, 3) NOT NULL,
	[Variance] [decimal](18, 3) NOT NULL,
	[VarianceValue] [decimal](18, 2) NULL,
	[Status] [int] NOT NULL,
	[CountedBy] [nvarchar](450) NULL,
	[CountedDate] [datetime2](7) NULL,
	[VerifiedBy] [nvarchar](450) NULL,
	[VerifiedDate] [datetime2](7) NULL,
	[LocationCode] [nvarchar](100) NULL,
	[BatchNumbers] [nvarchar](500) NULL,
	[SerialNumbers] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[SubCategoryId] [int] NULL,
	[BatchNo] [nvarchar](50) NULL,
	[Location] [nvarchar](200) NULL,
	[LastCountDate] [datetime2](7) NULL,
	[VariancePercentage] [decimal](5, 2) NULL,
	[LastIssueDate] [datetime2](7) NULL,
	[LastReceiveDate] [datetime2](7) NULL,
	[RecountRequestedBy] [nvarchar](450) NULL,
	[RecountRequestedDate] [datetime2](7) NULL,
	[FirstCountQuantity] [decimal](18, 3) NOT NULL,
	[FirstCountBy] [nvarchar](450) NULL,
	[FirstCountTime] [datetime2](7) NOT NULL,
	[CountLocation] [nvarchar](200) NULL,
	[CountRemarks] [nvarchar](500) NULL,
	[VarianceType] [int] NOT NULL,
	[RecountQuantity] [decimal](18, 3) NOT NULL,
	[RecountBy] [nvarchar](450) NULL,
	[RecountTime] [datetime2](7) NOT NULL,
	[BlindCountQuantity] [decimal](18, 3) NOT NULL,
	[BlindCountBy] [nvarchar](450) NULL,
	[BlindCountTime] [datetime2](7) NOT NULL,
	[BlindCountCompleted] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PhysicalInventoryDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhysicalInventoryItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhysicalInventoryItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PhysicalInventoryId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[SystemQuantity] [decimal](18, 3) NOT NULL,
	[PhysicalQuantity] [decimal](18, 3) NOT NULL,
	[Variance] [decimal](18, 3) NOT NULL,
	[UnitCost] [decimal](18, 2) NOT NULL,
	[SystemValue] [decimal](18, 2) NOT NULL,
	[PhysicalValue] [decimal](18, 2) NOT NULL,
	[VarianceValue] [decimal](18, 2) NOT NULL,
	[Location] [nvarchar](200) NULL,
	[BatchNumber] [nvarchar](50) NULL,
	[CountedAt] [datetime2](7) NULL,
	[CountedBy] [nvarchar](450) NULL,
	[IsRecounted] [bit] NOT NULL,
	[RecountQuantity] [decimal](18, 3) NULL,
	[Notes] [nvarchar](500) NULL,
	[AdjustmentStatus] [nvarchar](50) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PhysicalInventoryItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[ReceivedQuantity] [decimal](18, 2) NULL,
	[AcceptedQuantity] [decimal](18, 2) NULL,
	[RejectedQuantity] [decimal](18, 2) NULL,
	[BatchNo] [nvarchar](4000) NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[ReceiveRemarks] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrderItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[ReceivedQuantity] [decimal](18, 3) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseOrderItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrders]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNumber] [nvarchar](4000) NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[VendorId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[Status] [nvarchar](4000) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[ExpectedDeliveryDate] [datetime2](7) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[RejectedBy] [nvarchar](4000) NULL,
	[RejectedDate] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Purchases]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Purchases](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderNo] [nvarchar](50) NOT NULL,
	[PurchaseDate] [datetime2](7) NOT NULL,
	[ExpectedDeliveryDate] [datetime2](7) NULL,
	[DeliveryDate] [datetime2](7) NULL,
	[ReceivedDate] [datetime2](7) NULL,
	[VendorId] [int] NULL,
	[StoreId] [int] NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[Discount] [decimal](18, 2) NOT NULL,
	[PurchaseType] [nvarchar](4000) NULL,
	[Status] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[IsMarketplacePurchase] [bit] NOT NULL,
	[MarketplaceUrl] [nvarchar](4000) NULL,
	[ProcurementType] [int] NOT NULL,
	[ProcurementSource] [nvarchar](4000) NULL,
	[BudgetCode] [nvarchar](4000) NULL,
	[CreatedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Purchases] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QualityCheckItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QualityCheckItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QualityCheckId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[CheckedQuantity] [decimal](18, 2) NOT NULL,
	[PassedQuantity] [decimal](18, 2) NOT NULL,
	[FailedQuantity] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[CheckParameters] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_QualityCheckItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QualityChecks]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QualityChecks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CheckNumber] [nvarchar](50) NOT NULL,
	[CheckDate] [datetime2](7) NOT NULL,
	[ItemId] [int] NOT NULL,
	[PurchaseId] [int] NULL,
	[CheckType] [nvarchar](50) NULL,
	[CheckedBy] [nvarchar](4000) NULL,
	[Status] [nvarchar](20) NULL,
	[Comments] [nvarchar](1000) NULL,
	[CheckedQuantity] [decimal](18, 2) NOT NULL,
	[PassedQuantity] [decimal](18, 2) NOT NULL,
	[FailedQuantity] [decimal](18, 2) NOT NULL,
	[FailureReasons] [nvarchar](500) NULL,
	[CorrectiveActions] [nvarchar](500) NULL,
	[RequiresRetest] [bit] NOT NULL,
	[RetestDate] [datetime2](7) NULL,
	[GoodsReceiveId] [int] NOT NULL,
	[CheckedDate] [datetime2](7) NOT NULL,
	[OverallStatus] [int] NOT NULL,
	[CheckedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_QualityChecks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ranges]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ranges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[HeadquarterLocation] [nvarchar](200) NULL,
	[CommanderName] [nvarchar](100) NULL,
	[CommanderRank] [nvarchar](50) NULL,
	[ContactNumber] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[CoverageArea] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[NameBn] [nvarchar](100) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Ranges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReceiveItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReceiveItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReceiveId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[Quantity] [decimal](18, 3) NULL,
	[IssuedQuantity] [decimal](18, 3) NOT NULL,
	[ReceivedQuantity] [decimal](18, 3) NULL,
	[BatchNumber] [nvarchar](4000) NULL,
	[Condition] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[DamageNotes] [nvarchar](4000) NULL,
	[DamageDescription] [nvarchar](4000) NULL,
	[DamagePhotoPath] [nvarchar](4000) NULL,
	[IsScanned] [bit] NOT NULL,
	[ItemId1] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
	[LedgerNo] [nvarchar](4000) NULL,
	[PageNo] [nvarchar](4000) NULL,
	[PartiallyUsableQuantity] [decimal](18, 2) NULL,
	[UnusableQuantity] [decimal](18, 2) NULL,
	[UsableQuantity] [decimal](18, 2) NULL,
 CONSTRAINT [PK_ReceiveItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Receives]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Receives](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReceiveNo] [nvarchar](50) NOT NULL,
	[ReceiveNumber] [nvarchar](4000) NULL,
	[ReceiveDate] [datetime2](7) NOT NULL,
	[ReceivedDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[ReceivedFrom] [nvarchar](4000) NULL,
	[ReceivedFromType] [nvarchar](4000) NULL,
	[ReceivedBy] [nvarchar](4000) NULL,
	[Source] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](500) NULL,
	[ReceivedFromBattalionId] [int] NULL,
	[ReceivedFromRangeId] [int] NULL,
	[ReceivedFromZilaId] [int] NULL,
	[ReceivedFromUpazilaId] [int] NULL,
	[ReceivedFromUnionId] [int] NULL,
	[ReceivedFromIndividualName] [nvarchar](4000) NULL,
	[ReceivedFromIndividualBadgeNo] [nvarchar](4000) NULL,
	[StoreId] [int] NULL,
	[OriginalIssueId] [int] NULL,
	[OriginalIssueNo] [nvarchar](4000) NULL,
	[OriginalVoucherNo] [nvarchar](4000) NULL,
	[ReceiveType] [nvarchar](4000) NULL,
	[ReceiverSignature] [nvarchar](4000) NULL,
	[ReceiverName] [nvarchar](4000) NULL,
	[ReceiverBadgeNo] [nvarchar](4000) NULL,
	[ReceiverDesignation] [nvarchar](4000) NULL,
	[IsReceiverSignature] [bit] NOT NULL,
	[VerifierSignature] [bit] NOT NULL,
	[OverallCondition] [nvarchar](4000) NULL,
	[AssessmentNotes] [nvarchar](4000) NULL,
	[AssessedBy] [nvarchar](4000) NULL,
	[AssessmentDate] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
	[VoucherDate] [datetime2](7) NULL,
	[VoucherDocumentPath] [nvarchar](4000) NULL,
	[VoucherGeneratedDate] [datetime2](7) NULL,
	[VoucherNo] [nvarchar](4000) NULL,
	[VoucherNumber] [nvarchar](4000) NULL,
	[VoucherQRCode] [nvarchar](4000) NULL,
 CONSTRAINT [PK_Receives] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequisitionItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequisitionItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequisitionId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[RequestedQuantity] [decimal](18, 3) NOT NULL,
	[ApprovedQuantity] [decimal](18, 3) NULL,
	[EstimatedUnitPrice] [decimal](18, 2) NOT NULL,
	[EstimatedTotalPrice] [decimal](18, 2) NOT NULL,
	[Justification] [nvarchar](4000) NULL,
	[Status] [nvarchar](4000) NULL,
	[IssuedQuantity] [decimal](18, 3) NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[Specification] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_RequisitionItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Requisitions]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Requisitions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequisitionNumber] [nvarchar](50) NOT NULL,
	[RequisitionDate] [datetime2](7) NOT NULL,
	[RequestedBy] [nvarchar](4000) NULL,
	[Status] [nvarchar](4000) NULL,
	[RequiredDate] [datetime2](7) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[RejectedBy] [nvarchar](4000) NULL,
	[TotalEstimatedCost] [decimal](18, 2) NOT NULL,
	[Notes] [nvarchar](4000) NULL,
	[RequestDate] [datetime2](7) NOT NULL,
	[ApprovalComments] [nvarchar](4000) NULL,
	[RejectedDate] [datetime2](7) NOT NULL,
	[Priority] [nvarchar](4000) NULL,
	[Department] [nvarchar](4000) NULL,
	[Purpose] [nvarchar](4000) NULL,
	[RequiredByDate] [datetime2](7) NOT NULL,
	[FromStoreId] [int] NULL,
	[ToStoreId] [int] NULL,
	[FulfillmentStatus] [nvarchar](4000) NULL,
	[AutoConvertToPO] [bit] NOT NULL,
	[PurchaseOrderId] [int] NULL,
	[EstimatedValue] [decimal](18, 2) NOT NULL,
	[ApprovedValue] [decimal](18, 2) NOT NULL,
	[Level1ApprovedBy] [nvarchar](4000) NULL,
	[Level1ApprovedDate] [datetime2](7) NULL,
	[Level2ApprovedBy] [nvarchar](4000) NULL,
	[Level2ApprovedDate] [datetime2](7) NULL,
	[FinalApprovedBy] [nvarchar](4000) NULL,
	[FinalApprovedDate] [datetime2](7) NULL,
	[CurrentApprovalLevel] [int] NOT NULL,
	[RequestedByUserId] [nvarchar](4000) NULL,
	[ApprovedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Requisitions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReturnItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReturnItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReturnId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[ReturnQuantity] [decimal](18, 3) NOT NULL,
	[ApprovedQuantity] [decimal](18, 3) NULL,
	[ReceivedQuantity] [decimal](18, 3) NULL,
	[AcceptedQuantity] [decimal](18, 3) NULL,
	[RejectedQuantity] [decimal](18, 3) NULL,
	[Condition] [nvarchar](4000) NULL,
	[CheckedCondition] [nvarchar](4000) NULL,
	[ReturnReason] [nvarchar](4000) NULL,
	[BatchNo] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[ApprovalRemarks] [nvarchar](4000) NULL,
	[ReceivedDate] [datetime2](7) NULL,
	[ReceivedBy] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ReturnItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Returns]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Returns](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReturnNo] [nvarchar](50) NOT NULL,
	[ReturnDate] [datetime2](7) NOT NULL,
	[Status] [int] NOT NULL,
	[ReturnedBy] [nvarchar](4000) NULL,
	[ReturnedByType] [nvarchar](4000) NULL,
	[Reason] [nvarchar](500) NULL,
	[ReturnType] [nvarchar](4000) NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[ToStoreId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[RequestedBy] [nvarchar](4000) NULL,
	[RequestedDate] [datetime2](7) NOT NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[ApprovalRemarks] [nvarchar](4000) NULL,
	[ReceivedBy] [nvarchar](4000) NULL,
	[ReceivedDate] [datetime2](7) NULL,
	[CompletedDate] [datetime2](7) NULL,
	[ReceiptRemarks] [nvarchar](4000) NULL,
	[IsRestocked] [bit] NOT NULL,
	[RestockApprovalRequired] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[ReturnerSignature] [bit] NOT NULL,
	[IsReceiverSignature] [bit] NOT NULL,
	[ApproverSignature] [bit] NOT NULL,
	[OriginalIssueId] [int] NULL,
	[OriginalIssueNo] [nvarchar](4000) NULL,
	[ReceiveId] [int] NULL,
	[FromEntityType] [nvarchar](4000) NULL,
	[FromEntityId] [int] NOT NULL,
	[ConditionCheckId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Returns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](4000) NULL,
	[RoleName] [nvarchar](4000) NULL,
	[Permission] [int] NOT NULL,
	[PermissionName] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[IsGranted] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](4000) NULL,
	[Value] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[Category] [nvarchar](4000) NULL,
	[DataType] [nvarchar](4000) NULL,
	[IsReadOnly] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShipmentTrackings]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentTrackings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceType] [nvarchar](50) NULL,
	[ReferenceId] [int] NOT NULL,
	[TrackingCode] [nvarchar](100) NOT NULL,
	[Status] [nvarchar](50) NULL,
	[QRCode] [nvarchar](500) NULL,
	[LastLocation] [nvarchar](200) NULL,
	[LastUpdated] [datetime2](7) NULL,
	[Carrier] [nvarchar](100) NULL,
	[EstimatedDelivery] [datetime2](7) NULL,
	[TrackingUrl] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ShipmentTrackings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SignatoryPresets]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SignatoryPresets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PresetName] [nvarchar](4000) NOT NULL,
	[PresetNameBn] [nvarchar](4000) NOT NULL,
	[SignatoryName] [nvarchar](4000) NOT NULL,
	[SignatoryDesignation] [nvarchar](4000) NULL,
	[SignatoryDesignationBn] [nvarchar](4000) NULL,
	[SignatoryId] [nvarchar](4000) NULL,
	[SignatoryPhone] [nvarchar](4000) NULL,
	[SignatoryEmail] [nvarchar](4000) NULL,
	[Department] [nvarchar](4000) NULL,
	[IsDefault] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SignatoryPresets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SignatureOTPs]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SignatureOTPs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](4000) NULL,
	[OTPCode] [nvarchar](4000) NULL,
	[Purpose] [nvarchar](4000) NULL,
	[GeneratedAt] [datetime2](7) NOT NULL,
	[ExpiresAt] [datetime2](7) NOT NULL,
	[IsUsed] [bit] NOT NULL,
	[UsedAt] [datetime2](7) NULL,
	[OTP] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SignatureOTPs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Signatures]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Signatures](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceType] [nvarchar](50) NOT NULL,
	[ReferenceId] [int] NOT NULL,
	[SignatureType] [nvarchar](50) NULL,
	[SignatureData] [nvarchar](max) NULL,
	[SignerName] [nvarchar](4000) NULL,
	[SignerBadgeId] [nvarchar](4000) NULL,
	[SignerDesignation] [nvarchar](4000) NULL,
	[SignedDate] [datetime2](7) NOT NULL,
	[IPAddress] [nvarchar](4000) NULL,
	[DeviceInfo] [nvarchar](4000) NULL,
	[IssueId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Signatures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockAdjustmentItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockAdjustmentItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StockAdjustmentId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[SystemQuantity] [decimal](18, 3) NOT NULL,
	[ActualQuantity] [decimal](18, 3) NOT NULL,
	[PhysicalQuantity] [decimal](18, 3) NOT NULL,
	[AdjustmentQuantity] [decimal](18, 3) NOT NULL,
	[AdjustmentValue] [decimal](18, 2) NULL,
	[BatchNo] [nvarchar](50) NULL,
	[Reason] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StockAdjustmentItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockAdjustments]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockAdjustments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdjustmentNo] [nvarchar](50) NOT NULL,
	[AdjustmentDate] [datetime2](7) NOT NULL,
	[AdjustmentType] [nvarchar](50) NULL,
	[Status] [nvarchar](20) NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[PhysicalInventoryId] [int] NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[OldQuantity] [decimal](18, 3) NULL,
	[NewQuantity] [decimal](18, 3) NULL,
	[AdjustmentQuantity] [decimal](18, 3) NULL,
	[TotalValue] [decimal](18, 2) NULL,
	[Reason] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[ReferenceNumber] [nvarchar](50) NULL,
	[ReferenceDocument] [nvarchar](200) NULL,
	[ApprovalReference] [nvarchar](50) NULL,
	[FiscalYear] [nvarchar](20) NULL,
	[AdjustedBy] [nvarchar](450) NULL,
	[AdjustedDate] [datetime2](7) NULL,
	[ApprovedBy] [nvarchar](450) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[ApprovalRemarks] [nvarchar](500) NULL,
	[IsApproved] [bit] NOT NULL,
	[RejectedBy] [nvarchar](450) NULL,
	[RejectedDate] [datetime2](7) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[AuditTrailJson] [nvarchar](4000) NULL,
	[StoreId1] [int] NULL,
	[ItemId1] [int] NULL,
	[ApprovedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StockAdjustments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockAlerts]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockAlerts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AlertType] [nvarchar](50) NULL,
	[AlertDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[Message] [nvarchar](1000) NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[CurrentStock] [decimal](18, 3) NULL,
	[MinimumStock] [decimal](18, 3) NULL,
	[CurrentQuantity] [decimal](18, 3) NULL,
	[ThresholdQuantity] [decimal](18, 3) NULL,
	[AcknowledgedBy] [nvarchar](450) NULL,
	[AcknowledgedDate] [datetime2](7) NULL,
	[IsResolved] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StockAlerts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockEntries]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockEntries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryNo] [nvarchar](50) NULL,
	[EntryDate] [datetime2](7) NOT NULL,
	[EntryType] [nvarchar](50) NULL,
	[Status] [nvarchar](20) NULL,
	[Remarks] [nvarchar](500) NULL,
	[StoreId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StockEntries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockEntryItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockEntryItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StockEntryId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[UnitCost] [decimal](18, 2) NOT NULL,
	[Location] [nvarchar](200) NULL,
	[BatchNumber] [nvarchar](50) NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[BarcodesGenerated] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StockEntryItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockMovements]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockMovements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MovementType] [nvarchar](4000) NULL,
	[MovementDate] [datetime2](7) NOT NULL,
	[Reason] [nvarchar](4000) NULL,
	[Notes] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[SourceStoreId] [int] NULL,
	[DestinationStoreId] [int] NULL,
	[Quantity] [decimal](18, 3) NULL,
	[OldBalance] [decimal](18, 3) NOT NULL,
	[NewBalance] [decimal](18, 3) NOT NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[TotalValue] [decimal](18, 2) NULL,
	[ReferenceType] [nvarchar](4000) NULL,
	[ReferenceNo] [nvarchar](4000) NULL,
	[ReferenceId] [int] NULL,
	[MovedBy] [nvarchar](4000) NULL,
	[MovedByUserId] [nvarchar](4000) NULL,
	[StoreItemId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StockMovements] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockOperations]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockOperations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OperationType] [nvarchar](50) NULL,
	[ReferenceNumber] [nvarchar](50) NULL,
	[Status] [nvarchar](20) NULL,
	[Remarks] [nvarchar](500) NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[FromStoreId] [int] NOT NULL,
	[ToStoreId] [int] NULL,
	[ApprovedBy] [nvarchar](450) NULL,
	[ApprovedAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StockOperations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockReturns]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockReturns](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReturnNumber] [nvarchar](50) NULL,
	[ReturnDate] [datetime2](7) NOT NULL,
	[Reason] [nvarchar](500) NULL,
	[Condition] [nvarchar](100) NULL,
	[OriginalIssueId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[ReturnedBy] [nvarchar](450) NULL,
	[RestockApproved] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](450) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StockReturns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreConfigurations]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreConfigurations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NULL,
	[ConfigKey] [nvarchar](50) NOT NULL,
	[ConfigValue] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StoreConfigurations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[Quantity] [decimal](18, 3) NULL,
	[CurrentStock] [decimal](18, 3) NOT NULL,
	[MinimumStock] [decimal](18, 3) NOT NULL,
	[MaximumStock] [decimal](18, 3) NOT NULL,
	[ReorderLevel] [decimal](18, 3) NOT NULL,
	[ReservedStock] [decimal](18, 3) NULL,
	[ReservedQuantity] [int] NOT NULL,
	[Location] [nvarchar](200) NULL,
	[Status] [int] NOT NULL,
	[LastUpdated] [datetime2](7) NOT NULL,
	[LastStockUpdate] [datetime2](7) NOT NULL,
	[LastCountDate] [datetime2](7) NOT NULL,
	[LastIssueDate] [datetime2](7) NOT NULL,
	[LastReceiveDate] [datetime2](7) NOT NULL,
	[LastTransferDate] [datetime2](7) NOT NULL,
	[LastAdjustmentDate] [datetime2](7) NOT NULL,
	[LastCountQuantity] [decimal](18, 3) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StoreItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stores]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stores](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[NameBn] [nvarchar](100) NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Type] [nvarchar](50) NULL,
	[Status] [nvarchar](20) NULL,
	[Location] [nvarchar](200) NULL,
	[Address] [nvarchar](500) NULL,
	[Description] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[InCharge] [nvarchar](4000) NULL,
	[ContactNumber] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](4000) NULL,
	[ManagerName] [nvarchar](4000) NULL,
	[ManagerId] [nvarchar](4000) NULL,
	[StoreKeeperName] [nvarchar](4000) NULL,
	[StoreKeeperId] [nvarchar](4000) NULL,
	[StoreKeeperContact] [nvarchar](4000) NULL,
	[StoreKeeperAssignedDate] [datetime2](7) NOT NULL,
	[OperatingHours] [nvarchar](100) NULL,
	[Capacity] [decimal](18, 2) NULL,
	[TotalCapacity] [decimal](18, 2) NULL,
	[UsedCapacity] [decimal](18, 2) NULL,
	[AvailableCapacity] [decimal](18, 2) NULL,
	[Level] [int] NOT NULL,
	[StoreTypeId] [int] NULL,
	[LocationId] [int] NULL,
	[BattalionId] [int] NULL,
	[RangeId] [int] NULL,
	[ZilaId] [int] NULL,
	[UpazilaId] [int] NULL,
	[UnionId] [int] NULL,
	[RequiresTemperatureControl] [bit] NOT NULL,
	[Temperature] [decimal](5, 2) NULL,
	[Humidity] [decimal](5, 2) NULL,
	[MinTemperature] [decimal](5, 2) NULL,
	[MaxTemperature] [decimal](5, 2) NULL,
	[SecurityLevel] [nvarchar](4000) NULL,
	[AccessRequirements] [nvarchar](4000) NULL,
	[IsStockFrozen] [bit] NOT NULL,
	[StockFrozenAt] [datetime2](7) NOT NULL,
	[StockUnfrozenAt] [datetime2](7) NULL,
	[StockFrozenReason] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Stores] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreStocks]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreStocks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[MinQuantity] [decimal](18, 3) NOT NULL,
	[MaxQuantity] [decimal](18, 3) NOT NULL,
	[LastUpdated] [datetime2](7) NOT NULL,
	[ReorderLevel] [decimal](18, 3) NOT NULL,
	[LastUpdatedBy] [nvarchar](4000) NULL,
	[LastPurchasePrice] [decimal](18, 2) NULL,
	[AveragePrice] [decimal](18, 2) NULL,
	[StoreId1] [int] NULL,
	[ItemId1] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StoreStocks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreTypeCategories]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreTypeCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreTypeId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[IsAllowed] [bit] NOT NULL,
	[StoreTypeId1] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StoreTypeCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreTypes]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[NameBn] [nvarchar](4000) NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Icon] [nvarchar](50) NULL,
	[Color] [nvarchar](20) NULL,
	[DefaultManagerRole] [nvarchar](50) NULL,
	[DisplayOrder] [int] NOT NULL,
	[MaxCapacity] [int] NOT NULL,
	[RequiresTemperatureControl] [bit] NOT NULL,
	[RequiresSecurityClearance] [bit] NOT NULL,
	[IsMainStore] [bit] NOT NULL,
	[AllowDirectIssue] [bit] NOT NULL,
	[AllowTransfer] [bit] NOT NULL,
	[RequiresMandatoryDocuments] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StoreTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubCategories]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[NameBn] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NULL,
	[Code] [nvarchar](20) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SubCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SupplierEvaluations]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierEvaluations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NOT NULL,
	[EvaluationDate] [datetime2](7) NOT NULL,
	[EvaluatedBy] [nvarchar](4000) NULL,
	[QualityRating] [decimal](3, 2) NOT NULL,
	[DeliveryRating] [decimal](3, 2) NOT NULL,
	[PriceRating] [decimal](3, 2) NOT NULL,
	[ServiceRating] [decimal](3, 2) NOT NULL,
	[OverallRating] [decimal](3, 2) NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Recommendations] [nvarchar](1000) NULL,
	[IsApproved] [bit] NOT NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[EvaluatedByUserId] [nvarchar](4000) NULL,
	[ApprovedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierEvaluations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemConfigurations]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfigurations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleName] [nvarchar](100) NOT NULL,
	[ConfigurationKey] [nvarchar](100) NOT NULL,
	[ConfigurationValue] [nvarchar](4000) NOT NULL,
	[DataType] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[IsSystemLevel] [bit] NOT NULL,
	[RequiresRestart] [bit] NOT NULL,
	[ValidValues] [nvarchar](1000) NULL,
	[DefaultValue] [nvarchar](4000) NULL,
	[LastModified] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](4000) NULL,
	[ModifiedByUserId] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SystemConfigurations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemperatureLogs]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemperatureLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LogTime] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[StoreId] [int] NULL,
	[Temperature] [decimal](5, 2) NOT NULL,
	[Humidity] [decimal](5, 2) NOT NULL,
	[Unit] [nvarchar](20) NULL,
	[RecordedBy] [nvarchar](450) NULL,
	[Equipment] [nvarchar](100) NULL,
	[IsAlert] [bit] NOT NULL,
	[AlertReason] [nvarchar](4000) NULL,
	[RecordedByUserId] [nvarchar](4000) NULL,
	[StoreId1] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TemperatureLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrackingHistories]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrackingHistories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShipmentTrackingId] [int] NOT NULL,
	[Status] [nvarchar](50) NULL,
	[Location] [nvarchar](200) NULL,
	[LastUpdated] [datetime2](7) NULL,
	[Carrier] [nvarchar](100) NULL,
	[EstimatedDelivery] [datetime2](7) NULL,
	[TrackingUrl] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TrackingHistories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransferDiscrepancies]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferDiscrepancies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransferId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[ExpectedQuantity] [decimal](18, 3) NOT NULL,
	[ActualQuantity] [decimal](18, 3) NOT NULL,
	[DiscrepancyQuantity] [decimal](18, 3) NOT NULL,
	[Reason] [nvarchar](4000) NULL,
	[Resolution] [nvarchar](4000) NULL,
	[IsResolved] [bit] NOT NULL,
	[ShippedQuantity] [decimal](18, 3) NULL,
	[ReceivedQuantity] [decimal](18, 3) NULL,
	[Variance] [decimal](18, 3) NULL,
	[ReportedBy] [nvarchar](4000) NULL,
	[ReportedDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TransferDiscrepancies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransferItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransferId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[RequestedQuantity] [decimal](18, 3) NULL,
	[ApprovedQuantity] [decimal](18, 3) NULL,
	[ShippedQuantity] [decimal](18, 3) NULL,
	[ReceivedQuantity] [decimal](18, 3) NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[TotalValue] [decimal](18, 2) NULL,
	[ItemCondition] [nvarchar](4000) NULL,
	[BatchNo] [nvarchar](4000) NULL,
	[Remarks] [nvarchar](4000) NULL,
	[ReceivedCondition] [nvarchar](4000) NULL,
	[ReceivedDate] [datetime2](7) NOT NULL,
	[ShippedDate] [datetime2](7) NOT NULL,
	[PackageCount] [int] NOT NULL,
	[PackageDetails] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TransferItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transfers]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transfers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransferNo] [nvarchar](50) NOT NULL,
	[TransferDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[FromStoreId] [int] NOT NULL,
	[ToStoreId] [int] NOT NULL,
	[FromBattalionId] [int] NULL,
	[FromRangeId] [int] NULL,
	[FromZilaId] [int] NULL,
	[ToBattalionId] [int] NULL,
	[ToRangeId] [int] NULL,
	[ToZilaId] [int] NULL,
	[TransferType] [nvarchar](50) NULL,
	[Purpose] [nvarchar](200) NULL,
	[Remarks] [nvarchar](500) NULL,
	[RequestedBy] [nvarchar](4000) NULL,
	[RequestedDate] [datetime2](7) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[TransferredBy] [nvarchar](4000) NULL,
	[ShippedBy] [nvarchar](4000) NULL,
	[ShippedDate] [datetime2](7) NULL,
	[ShipmentNo] [nvarchar](4000) NULL,
	[ShippingQRCode] [nvarchar](4000) NULL,
	[ReceivedBy] [nvarchar](4000) NULL,
	[ReceivedDate] [datetime2](7) NULL,
	[ReceiverSignature] [nvarchar](4000) NULL,
	[ReceiptRemarks] [nvarchar](4000) NULL,
	[EstimatedDeliveryDate] [datetime2](7) NULL,
	[TransportMode] [nvarchar](4000) NULL,
	[VehicleNo] [nvarchar](4000) NULL,
	[DriverName] [nvarchar](4000) NULL,
	[DriverContact] [nvarchar](4000) NULL,
	[TotalValue] [decimal](18, 2) NULL,
	[HasDiscrepancy] [bit] NOT NULL,
	[SenderSignature] [bit] NOT NULL,
	[IsReceiverSignature] [bit] NOT NULL,
	[ApproverSignature] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Transfers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransferShipmentItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferShipmentItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShipmentId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 3) NOT NULL,
	[PackageNo] [nvarchar](50) NULL,
	[Condition] [nvarchar](50) NULL,
	[ShippedQuantity] [decimal](18, 3) NOT NULL,
	[PackageCount] [int] NOT NULL,
	[PackageDetails] [nvarchar](max) NULL,
	[BatchNo] [nvarchar](50) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TransferShipmentItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransferShipments]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferShipments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransferId] [int] NOT NULL,
	[ShipmentNo] [nvarchar](50) NOT NULL,
	[ShipmentDate] [datetime2](7) NOT NULL,
	[Carrier] [nvarchar](100) NULL,
	[TrackingNo] [nvarchar](100) NULL,
	[Status] [nvarchar](20) NULL,
	[EstimatedDelivery] [datetime2](7) NULL,
	[ActualDelivery] [datetime2](7) NULL,
	[Notes] [nvarchar](4000) NULL,
	[ShippedDate] [datetime2](7) NOT NULL,
	[ShippedBy] [nvarchar](4000) NULL,
	[PackingListNo] [nvarchar](200) NULL,
	[TransportCompany] [nvarchar](100) NULL,
	[VehicleNo] [nvarchar](50) NULL,
	[DriverName] [nvarchar](100) NULL,
	[SealNo] [nvarchar](50) NULL,
	[DriverContact] [nvarchar](50) NULL,
	[EstimatedArrival] [datetime2](7) NOT NULL,
	[ActualArrival] [datetime2](7) NOT NULL,
	[ReceivedBy] [nvarchar](4000) NULL,
	[ReceiptCondition] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TransferShipments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Unions]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Unions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[NameBangla] [nvarchar](4000) NULL,
	[UpazilaId] [int] NOT NULL,
	[ChairmanName] [nvarchar](4000) NULL,
	[ChairmanContact] [nvarchar](4000) NULL,
	[SecretaryName] [nvarchar](4000) NULL,
	[SecretaryContact] [nvarchar](4000) NULL,
	[VDPOfficerName] [nvarchar](4000) NULL,
	[VDPOfficerContact] [nvarchar](4000) NULL,
	[Email] [nvarchar](4000) NULL,
	[OfficeAddress] [nvarchar](4000) NULL,
	[NumberOfWards] [int] NULL,
	[NumberOfVillages] [int] NULL,
	[NumberOfMouzas] [int] NULL,
	[Area] [decimal](10, 2) NULL,
	[Population] [int] NULL,
	[NumberOfHouseholds] [int] NULL,
	[HasVDPUnit] [bit] NOT NULL,
	[VDPMemberCountMale] [int] NULL,
	[VDPMemberCountFemale] [int] NULL,
	[AnsarMemberCount] [int] NULL,
	[Latitude] [decimal](10, 6) NULL,
	[Longitude] [decimal](10, 6) NULL,
	[IsRural] [bit] NOT NULL,
	[IsBorderArea] [bit] NOT NULL,
	[Remarks] [nvarchar](4000) NULL,
	[NameBn] [nvarchar](100) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Unions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Units]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Units](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Symbol] [nvarchar](4000) NULL,
	[Type] [nvarchar](4000) NULL,
	[ConversionFactor] [decimal](18, 6) NOT NULL,
	[BaseUnit] [nvarchar](50) NULL,
	[NameBn] [nvarchar](4000) NULL,
	[Abbreviation] [nvarchar](4000) NULL,
	[Description] [nvarchar](200) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Upazilas]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Upazilas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[NameBangla] [nvarchar](4000) NULL,
	[ZilaId] [int] NOT NULL,
	[UpazilaOfficerName] [nvarchar](4000) NULL,
	[OfficerDesignation] [nvarchar](4000) NULL,
	[ContactNumber] [nvarchar](4000) NULL,
	[Email] [nvarchar](4000) NULL,
	[OfficeAddress] [nvarchar](4000) NULL,
	[Area] [decimal](10, 2) NULL,
	[Population] [int] NULL,
	[NumberOfUnions] [int] NULL,
	[NumberOfVillages] [int] NULL,
	[HasVDPUnit] [bit] NOT NULL,
	[VDPMemberCount] [int] NULL,
	[Remarks] [nvarchar](4000) NULL,
	[UpazilaChairmanName] [nvarchar](4000) NULL,
	[VDPOfficerName] [nvarchar](4000) NULL,
	[NameBn] [nvarchar](100) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Upazilas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsageStatistics]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsageStatistics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityType] [nvarchar](4000) NULL,
	[EntityId] [int] NOT NULL,
	[MetricName] [nvarchar](4000) NULL,
	[MetricValue] [decimal](18, 4) NOT NULL,
	[PeriodStart] [datetime2](7) NOT NULL,
	[PeriodEnd] [datetime2](7) NOT NULL,
	[Period] [nvarchar](4000) NULL,
	[CalculatedBy] [nvarchar](4000) NULL,
	[CalculationDate] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_UsageStatistics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserNotificationPreferences]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserNotificationPreferences](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](4000) NULL,
	[EmailEnabled] [bit] NOT NULL,
	[SmsEnabled] [bit] NOT NULL,
	[PushEnabled] [bit] NOT NULL,
	[LowStockAlerts] [bit] NOT NULL,
	[ExpiryAlerts] [bit] NOT NULL,
	[ApprovalAlerts] [bit] NOT NULL,
	[SystemAlerts] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_UserNotificationPreferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserStores]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserStores](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](4000) NULL,
	[StoreId] [int] NULL,
	[AssignedDate] [datetime2](7) NULL,
	[AssignedBy] [nvarchar](450) NULL,
	[IsPrimary] [bit] NOT NULL,
	[UnassignedDate] [datetime2](7) NULL,
	[AssignedAt] [datetime2](7) NOT NULL,
	[RemovedDate] [datetime2](7) NULL,
	[Role] [nvarchar](50) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_UserStores] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[VendorType] [nvarchar](50) NULL,
	[ContactPerson] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](100) NULL,
	[Mobile] [nvarchar](20) NULL,
	[Address] [nvarchar](500) NULL,
	[TIN] [nvarchar](50) NULL,
	[BIN] [nvarchar](50) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Warranties]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Warranties](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[WarrantyNumber] [nvarchar](50) NOT NULL,
	[VendorId] [int] NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[WarrantyType] [nvarchar](50) NULL,
	[Terms] [nvarchar](2000) NULL,
	[ContactInfo] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NULL,
	[CoveredValue] [decimal](18, 2) NOT NULL,
	[SerialNumber] [nvarchar](100) NULL,
	[ItemId1] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Warranties] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WriteOffItems]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WriteOffItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WriteOffId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[StoreId] [int] NULL,
	[WriteOffRequestId] [int] NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[Value] [decimal](18, 2) NOT NULL,
	[UnitCost] [decimal](18, 2) NOT NULL,
	[TotalCost] [decimal](18, 2) NOT NULL,
	[Reason] [nvarchar](4000) NULL,
	[BatchNo] [nvarchar](4000) NULL,
	[WriteOffRequestId1] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_WriteOffItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WriteOffRequests]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WriteOffRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestNo] [nvarchar](50) NOT NULL,
	[DamageReportId] [int] NULL,
	[DamageReportNo] [nvarchar](4000) NULL,
	[StoreId] [int] NOT NULL,
	[RequestDate] [datetime2](7) NOT NULL,
	[RequestedBy] [nvarchar](4000) NULL,
	[TotalValue] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[Justification] [nvarchar](4000) NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NULL,
	[ApprovalRemarks] [nvarchar](4000) NULL,
	[ApprovalReference] [nvarchar](4000) NULL,
	[ExecutedBy] [nvarchar](4000) NULL,
	[ExecutedDate] [datetime2](7) NULL,
	[DisposalMethod] [nvarchar](4000) NULL,
	[DisposalRemarks] [nvarchar](4000) NULL,
	[Reason] [nvarchar](4000) NULL,
	[RejectedBy] [nvarchar](4000) NULL,
	[RejectedDate] [datetime2](7) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_WriteOffRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WriteOffs]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WriteOffs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WriteOffNo] [nvarchar](50) NOT NULL,
	[WriteOffDate] [datetime2](7) NOT NULL,
	[Reason] [nvarchar](4000) NULL,
	[Status] [nvarchar](4000) NULL,
	[TotalValue] [decimal](18, 2) NOT NULL,
	[StoreId] [int] NULL,
	[ApprovedBy] [nvarchar](4000) NULL,
	[ApprovedDate] [datetime2](7) NOT NULL,
	[ApprovalComments] [nvarchar](max) NULL,
	[RejectedBy] [nvarchar](4000) NULL,
	[RejectionDate] [datetime2](7) NULL,
	[RejectionReason] [nvarchar](4000) NULL,
	[RequiredApproverRole] [nvarchar](4000) NULL,
	[ApprovalThreshold] [int] NOT NULL,
	[AttachmentPaths] [nvarchar](max) NULL,
	[NotificationSent] [bit] NOT NULL,
	[NotificationSentDate] [datetime2](7) NULL,
	[NotifiedUsers] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_WriteOffs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Zilas]    Script Date: 11/6/2025 1:16:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Zilas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[NameBangla] [nvarchar](4000) NULL,
	[RangeId] [int] NOT NULL,
	[Division] [nvarchar](4000) NULL,
	[DistrictOfficerName] [nvarchar](4000) NULL,
	[ContactNumber] [nvarchar](4000) NULL,
	[Email] [nvarchar](4000) NULL,
	[OfficeAddress] [nvarchar](4000) NULL,
	[Area] [decimal](10, 2) NULL,
	[Population] [int] NULL,
	[Remarks] [nvarchar](4000) NULL,
	[NameBn] [nvarchar](100) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](4000) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](4000) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Zilas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251103201950_InitialCreate', N'9.0.7')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251104201042_MakeWriteOffRequestIdNullable', N'9.0.7')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251105112826_AddVoucherLedgerPageFields', N'9.0.7')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251105172929_UpdateLedgerBookAndPendingChanges', N'9.0.7')
GO
SET IDENTITY_INSERT [dbo].[AllotmentLetterItems] ON 
GO
INSERT [dbo].[AllotmentLetterItems] ([Id], [AllotmentLetterId], [ItemId], [AllottedQuantity], [IssuedQuantity], [RemainingQuantity], [Unit], [UnitBn], [ItemNameBn], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 2, 5, CAST(5.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), N'Piece', NULL, NULL, NULL, CAST(N'2025-10-25T03:33:45.4833333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[AllotmentLetterItems] ([Id], [AllotmentLetterId], [ItemId], [AllottedQuantity], [IssuedQuantity], [RemainingQuantity], [Unit], [UnitBn], [ItemNameBn], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 2, 6, CAST(10.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), N'Piece', NULL, NULL, NULL, CAST(N'2025-10-25T03:33:45.4833333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[AllotmentLetterItems] OFF
GO
SET IDENTITY_INSERT [dbo].[AllotmentLetters] ON 
GO
INSERT [dbo].[AllotmentLetters] ([Id], [AllotmentNo], [AllotmentDate], [ValidFrom], [ValidUntil], [IssuedTo], [IssuedToType], [IssuedToBattalionId], [IssuedToRangeId], [IssuedToZilaId], [IssuedToUpazilaId], [FromStoreId], [Purpose], [Status], [ApprovedBy], [ApprovedDate], [RejectedBy], [RejectedDate], [RejectionReason], [DocumentPath], [ReferenceNo], [Remarks], [Subject], [SubjectBn], [BodyText], [BodyTextBn], [CollectionDeadline], [SignatoryName], [SignatoryDesignation], [SignatoryDesignationBn], [SignatoryId], [SignatoryPhone], [SignatoryEmail], [BengaliDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'AL-2024-001', CAST(N'2025-10-25T03:33:22.0933333' AS DateTime2), CAST(N'2025-10-25T03:33:22.0933333' AS DateTime2), CAST(N'2025-11-24T03:33:22.0933333' AS DateTime2), N'1st Ansar Battalion', N'Battalion', 1, NULL, NULL, NULL, 2, N'IT Equipment allotment for battalion office', N'Approved', NULL, NULL, NULL, NULL, NULL, NULL, N'HQ/Store/2024/001', NULL, N'Allotment of IT Equipment', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-25T03:33:22.0933333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[AllotmentLetters] OFF
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'0b090190-3eca-411d-af25-bcfe48aa4189', N'FinanceManager', N'FINANCEMANAGER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'11dd88e0-13aa-4462-9cb3-63a97214ac08', N'ZilaCommander', N'ZILACOMMANDER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'179f1506-dddf-4db0-87f8-3a4965302f57', N'DDGAdmin', N'DDGADMIN', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'33c30eda-e9af-46b5-8394-c529842c3fc6', N'RangeCommander', N'RANGECOMMANDER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'55f8fac2-c3fd-4b00-9898-56e57dee79f4', N'Director', N'DIRECTOR', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'56fdb1dc-ebdf-4dca-8dbf-0edc0e0c7e3a', N'StorekeeperCentral', N'STOREKEEPERCENTRAL', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'611a69d7-516d-4477-9ea1-c004c09a4481', N'ZilaOfficer', N'ZILAOFFICER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'63c797a0-830f-493c-8ac1-0bf27155ec40', N'StorekeeperProvision', N'STOREKEEPERPROVISION', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'6fef6a8d-bcfb-4aba-ab87-14bc79e4a035', N'DepartmentHead', N'DEPARTMENTHEAD', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'7ba83006-d432-4061-a19b-2370cebd97b2', N'ADStore', N'ADSTORE', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'9a205f26-dee7-4bde-84de-bdef7d9d1fdd', N'StoreKeeper', N'STOREKEEPER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', N'Auditor', N'AUDITOR', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'a9177cb2-a9f9-4a83-8b51-e26d47a94e98', N'BattalionCommander', N'BATTALIONCOMMANDER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', N'STOREMANAGER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'bc59ea92-5958-4e9e-8954-cb5966eba9ea', N'Viewer', N'VIEWER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'c1709a52-59ac-4522-bf69-65eb39cda7c1', N'DDProvision', N'DDPROVISION', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'c33b442a-2a59-4407-ad6b-a5913be397b3', N'UpazilaOfficer', N'UPAZILAOFFICER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', N'ADMIN', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'df479557-c211-4428-ae93-88c5caf9492b', N'DDStore', N'DDSTORE', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', N'OPERATOR', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'30d3d3cf-2bc3-4a51-a0b5-ad7b4dc4b32c', N'7ba83006-d432-4061-a19b-2370cebd97b2', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'3d68de6d-464d-43fd-b286-f521a56fdc0f', N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'450daed5-0c4e-44ee-b8a2-837e34d91682', N'56fdb1dc-ebdf-4dca-8dbf-0edc0e0c7e3a', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'54238526-69cc-4f2e-bab5-b1ad77667c26', N'33c30eda-e9af-46b5-8394-c529842c3fc6', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'5a137760-dd3f-4378-b93f-2f1a45939db1', N'0b090190-3eca-411d-af25-bcfe48aa4189', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'6335c2e8-2381-4ad4-a299-709b4fc97763', N'611a69d7-516d-4477-9ea1-c004c09a4481', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'76434284-0949-4b92-a07d-4dd8282f90d6', N'a9177cb2-a9f9-4a83-8b51-e26d47a94e98', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', N'9a205f26-dee7-4bde-84de-bdef7d9d1fdd', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'82bb60d7-5d61-43a1-9566-bb3825d5dbdf', N'c1709a52-59ac-4522-bf69-65eb39cda7c1', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'a27a83b4-740f-42b5-b06e-8e438e209f17', N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'abe360ff-5e14-4823-bcb5-952143e92f10', N'c33b442a-2a59-4407-ad6b-a5913be397b3', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', N'd8891b59-b8e2-4af1-9d90-02e0545c4149', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'c226edb3-01b2-4332-b336-ffe9f03b01d8', N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'cc42bb39-4fbc-4c69-9cf5-f755293a5a0e', N'63c797a0-830f-493c-8ac1-0bf27155ec40', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'd37bb77d-b213-4129-9fb9-5c3ebe5855eb', N'df479557-c211-4428-ae93-88c5caf9492b', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'f00d87c9-5414-4214-8503-f76e202bde83', N'55f8fac2-c3fd-4b00-9898-56e57dee79f4', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId], [UserId1]) VALUES (N'f53bcfaf-f8d1-4dea-ad23-cbd70cdb0d1f', N'179f1506-dddf-4db0-87f8-3a4965302f57', NULL)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'30d3d3cf-2bc3-4a51-a0b5-ad7b4dc4b32c', N'AD Store', CAST(N'2025-11-04T02:45:40.4921110' AS DateTime2), 1, NULL, N'Assistant Director (Store)', NULL, NULL, NULL, NULL, NULL, NULL, N'ad-store', N'AD-STORE', N'ad-store@ansar-vdp.gov.bd', N'AD-STORE@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEAYG0avtACgzoZxthmapiYtud970WUL3Uub+ohanCMiANgoF7jQWRkU7bumdZ9Fsxw==', N'GQCLQTPVXLKOPNC2WLF4HI4S7SHCWNST', N'0953c3f3-7339-4e32-9d80-c8fbba9351d9', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'3d68de6d-464d-43fd-b286-f521a56fdc0f', N'Store Operator', CAST(N'2025-11-04T02:45:40.0537147' AS DateTime2), 1, NULL, N'Operator', NULL, NULL, NULL, NULL, NULL, NULL, N'operator', N'OPERATOR', N'operator@ansar-vdp.gov.bd', N'OPERATOR@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEDh1D4p6hKiP7kvuQdLqgQkPsd6qU/5263Wp5Y1WyYGTYVZlICv5bGhmw2076kAWtA==', N'ONWHW6A6Q2EEVVCPQAF6AB7NX3IWVY7H', N'68977625-5361-46bc-81bd-538a22a59c29', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'450daed5-0c4e-44ee-b8a2-837e34d91682', N'Storekeeper Central', CAST(N'2025-11-04T02:45:40.8002792' AS DateTime2), 1, NULL, N'Storekeeper (Central Store)', NULL, NULL, NULL, NULL, NULL, NULL, N'storekeeper-central', N'STOREKEEPER-CENTRAL', N'storekeeper-central@ansar-vdp.gov.bd', N'STOREKEEPER-CENTRAL@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEEqvIPE/XGeJHH1sJWQgHK8Hp3A863SG9r79rX4TI8jH2NHlxlWeFqwIaW2PnVASNg==', N'TLDYL6UCIJLE7X5C752OBHEHYEHN2FWZ', N'5b24437e-b45d-43db-bda2-f26f5373a327', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'54238526-69cc-4f2e-bab5-b1ad77667c26', N'Range Commander', CAST(N'2025-11-04T02:45:41.0326172' AS DateTime2), 1, NULL, N'Range DIG', NULL, NULL, NULL, NULL, NULL, NULL, N'range-commander', N'RANGE-COMMANDER', N'range-commander@ansar-vdp.gov.bd', N'RANGE-COMMANDER@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEGlu89JPp92Jy+1RZHOAYyjR2DlBEGoNlw68wM+KZjKsdt9Px2Eko9p+ax9az+ahoA==', N'DJHQ5TUMTXXGDTRP64FX7YN337IUSFHY', N'53d62aa7-cd50-4604-9b1f-6ef8407acb22', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'5a137760-dd3f-4378-b93f-2f1a45939db1', N'Finance Manager', CAST(N'2025-11-04T02:45:39.7384334' AS DateTime2), 1, NULL, N'Finance Manager', NULL, NULL, NULL, NULL, NULL, NULL, N'finance', N'FINANCE', N'finance@ansar-vdp.gov.bd', N'FINANCE@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEH6GgeABwNAimuLb6aOAUhRhRnUhdoa+vscSca3y/w3cQkzfcFLDSaLUCRb/hm3tzg==', N'T3R2WNNSBS45EQRVNMHPXIFLEYFSAPCY', N'77328d57-12c6-41c8-a40b-8a2089414501', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'6335c2e8-2381-4ad4-a299-709b4fc97763', N'Zila Officer', CAST(N'2025-11-04T02:45:41.1337452' AS DateTime2), 1, NULL, N'District Commandant', NULL, NULL, NULL, NULL, NULL, NULL, N'zila-officer', N'ZILA-OFFICER', N'zila-officer@ansar-vdp.gov.bd', N'ZILA-OFFICER@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAECDVHdWFHdvp98oAfaWUfUgrZJaByw5xt4fS11e8NjLRiYILH5IzxsNXA4DeHrbJ5w==', N'EKTQCY5CM5M6VN4UYUWF2L675Y4KTZJ3', N'fc6737c9-40fc-46c2-b0ee-b2f13bf7436c', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'76434284-0949-4b92-a07d-4dd8282f90d6', N'Battalion Commander', CAST(N'2025-11-04T02:45:40.2628132' AS DateTime2), 1, NULL, N'Battalion Commander', NULL, NULL, NULL, NULL, NULL, NULL, N'battalion', N'BATTALION', N'battalion@ansar-vdp.gov.bd', N'BATTALION@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEDVTJFtSsJm39+HeFHt1V+706On6nmajFAKcKepVsVTNezZedJ0m5GKR9vTqmJmY9g==', N'ERJPWKI7QLKUUWKOSKB4OPS4ERBWZDT5', N'428b7cc5-3a24-4233-8fd5-25222dde3a55', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', N'Store Keeper', CAST(N'2025-11-04T02:45:39.9451094' AS DateTime2), 1, NULL, N'Store Keeper', NULL, NULL, NULL, NULL, NULL, NULL, N'storekeeper', N'STOREKEEPER', N'storekeeper@ansar-vdp.gov.bd', N'STOREKEEPER@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEK7s02CjSiso9MJuUjXfHmCNDEVmrp64Fk/MGuCA8+Rde0cnhraJUWIEKSdPTBFOgA==', N'4TVLBT5FYO5AXYDB4CIWUM3R2ZADTROT', N'a33397bc-c638-47a1-9f4a-9b29db2ad8c0', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'82bb60d7-5d61-43a1-9566-bb3825d5dbdf', N'DD Provision', CAST(N'2025-11-04T02:45:40.6830935' AS DateTime2), 1, NULL, N'Deputy Director (Provision)', NULL, NULL, NULL, NULL, NULL, NULL, N'dd-provision', N'DD-PROVISION', N'dd-provision@ansar-vdp.gov.bd', N'DD-PROVISION@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEAP6ASDWXZDhIoliyDVdFT6UXNGfgi5H80PKtoVQkFtH4Ok204vhN2SAKalYm3if1A==', N'KKRWC5BSEUO4JUG2M6D2P6Z7O6W2UHPX', N'616c6489-e15b-4dac-be71-68be3fb2017e', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'a27a83b4-740f-42b5-b06e-8e438e209f17', N'Store Manager', CAST(N'2025-11-04T02:45:39.8340125' AS DateTime2), 1, NULL, N'Store Manager', NULL, NULL, NULL, NULL, NULL, NULL, N'storemanager', N'STOREMANAGER', N'storemanager@ansar-vdp.gov.bd', N'STOREMANAGER@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAECpqkjXny50wlTCrCt7jsGIasYPTzGU8mytu34qFrqB2/1NXyUX4cCZN0u4HIYCbVQ==', N'2DKY3ONCBKACZ5DQLLB2T43PDTE4LABJ', N'c2991e42-4bde-45d2-b007-c875f9edf40a', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'abe360ff-5e14-4823-bcb5-952143e92f10', N'Upazila Officer', CAST(N'2025-11-04T02:45:41.2539477' AS DateTime2), 1, NULL, N'Upazila Ansar & VDP Officer', NULL, NULL, NULL, NULL, NULL, NULL, N'upazila-officer', N'UPAZILA-OFFICER', N'upazila-officer@ansar-vdp.gov.bd', N'UPAZILA-OFFICER@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEATTs+8CbDaYy1woLi+9z6eGan7/CZQjLOQ9HAwwK0sAc1OCP7UO081A/5ofU9HyhA==', N'H5U2QM2FUK6XZPQ7KCXDAL4UOMVXJZJQ', N'84c05435-7bf3-4cd1-b926-2110c61cc9ef', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', N'System Administrator', CAST(N'2025-11-04T02:45:39.2187656' AS DateTime2), 1, NULL, N'IT Administrator', NULL, NULL, NULL, NULL, NULL, NULL, N'admin', N'ADMIN', N'admin@ansar-vdp.gov.bd', N'ADMIN@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEECQX1dZnzIwb311e7/M5jQdYdf/rnNO83IY0NB1O4G6O2d8gq/EbTw0bg+KTpCxAw==', N'XWGO6YTPCTV2FANLUPMVCDVFJXRQ2HRG', N'98a12343-3a05-49e8-8a30-74c7b81c7aa5', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'c226edb3-01b2-4332-b336-ffe9f03b01d8', N'Internal Auditor', CAST(N'2025-11-04T02:45:40.1648496' AS DateTime2), 1, NULL, N'Auditor', NULL, NULL, NULL, NULL, NULL, NULL, N'auditor', N'AUDITOR', N'auditor@ansar-vdp.gov.bd', N'AUDITOR@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEI4bA+xhl8fzEjWCS0xRs2GJqce8Qb6GDSx8XInYToiambhBoXOfnP/H/Yc7SAvRVA==', N'EAZUDRAXZDQGMGQAGVMT2BEP67PEA2CN', N'52960d34-30d9-4fb5-be26-7f1aaceb07ae', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'cc42bb39-4fbc-4c69-9cf5-f755293a5a0e', N'Storekeeper Provision', CAST(N'2025-11-04T02:45:40.9195683' AS DateTime2), 1, NULL, N'Storekeeper (Provision Store)', NULL, NULL, NULL, NULL, NULL, NULL, N'storekeeper-provision', N'STOREKEEPER-PROVISION', N'storekeeper-provision@ansar-vdp.gov.bd', N'STOREKEEPER-PROVISION@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEBwxICYKEIdXk5d9I9cgJ07KdvfWtMwLAHIf8fky9pXWNAy8dHx0tbSprdf1PbUSOA==', N'X3G6SVWGKFPLM3SAJCBI276KXGFLQ7RZ', N'25284d4d-c4ec-40ba-8e22-ba07e995aeea', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'd37bb77d-b213-4129-9fb9-5c3ebe5855eb', N'DD Store', CAST(N'2025-11-04T02:45:40.5844090' AS DateTime2), 1, NULL, N'Deputy Director (Store)', NULL, NULL, NULL, NULL, NULL, NULL, N'dd-store', N'DD-STORE', N'dd-store@ansar-vdp.gov.bd', N'DD-STORE@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEMifF1+iJrMAeH5Bxp62xXsc/sWi9uAqGRZbNibDMtfBgjMul5UjQQObyblZmw+Jwg==', N'7NM2SZSXJ6XZACKHKV6DLH227SO2J3T6', N'6ab9af36-fbb9-4382-be41-4defb77762c6', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'f00d87c9-5414-4214-8503-f76e202bde83', N'Director General', CAST(N'2025-11-04T02:45:39.6260686' AS DateTime2), 1, NULL, N'Director General', NULL, NULL, NULL, NULL, NULL, NULL, N'director', N'DIRECTOR', N'director@ansar-vdp.gov.bd', N'DIRECTOR@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAELou52gEBOQnCUdtooPfUhPHD/1epw8jI77sFXMKRsZ4GCa0Gxe4tPlPDjnQtbTxlQ==', N'YX66EPYOMVYZKZJ57W6SLBX5MRCG4ABH', N'79fd8326-7a71-48a5-9ae8-17c49cccec77', NULL, 0, 0, NULL, 1, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [CreatedAt], [IsActive], [Department], [Designation], [BadgeNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'f53bcfaf-f8d1-4dea-ad23-cbd70cdb0d1f', N'DDG Admin', CAST(N'2025-11-04T02:45:40.3674048' AS DateTime2), 1, NULL, N'Deputy Director General (Admin)', NULL, NULL, NULL, NULL, NULL, NULL, N'ddg-admin', N'DDG-ADMIN', N'ddg-admin@ansar-vdp.gov.bd', N'DDG-ADMIN@ANSAR-VDP.GOV.BD', 1, N'AQAAAAIAAYagAAAAEIwpu9T1cxFXeOSQAZPeDVSbmia6uH9rJiLkUy+GZdfFRCe7OOBxUe78FndY2X5Arw==', N'SU6SCIWFHMFHGHFZHDTBC5ZHLODBXKND', N'2e55706a-8368-4834-b4b0-1b33d1d15530', NULL, 0, 0, NULL, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Audits] ON 
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'Purchase', 1, N'Created', N'New purchase order PO-2024-001 created for vendor Computer Point Ltd. Total amount: ?1,170,000', N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', CAST(N'2025-10-05T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-05T07:02:38.1666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Purchase', 1, N'Approved', N'Purchase order PO-2024-001 approved by DDG Admin', N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', CAST(N'2025-10-06T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-06T07:02:38.1666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'GoodsReceive', 1, N'Created', N'Goods received for PO-2024-001. GRN: GRN-2024-001. Total items: 19', N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', CAST(N'2025-10-07T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-07T07:02:38.1666667' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'QualityCheck', 1, N'Completed', N'Quality inspection completed for GRN-2024-001. All items approved.', N'a27a83b4-740f-42b5-b06e-8e438e209f17', CAST(N'2025-10-08T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-08T07:02:38.1666667' AS DateTime2), N'a27a83b4-740f-42b5-b06e-8e438e209f17', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'Issue', 1, N'Created', N'Issue voucher ISS-2024-001 created for Battalion AB-01. Items: 2', N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', CAST(N'2025-10-15T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-15T07:02:38.1666667' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'Transfer', 1, N'Created', N'Transfer TRN-2024-001 created. Items transferred between stores.', N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', CAST(N'2025-10-17T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-17T07:02:38.1666667' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'PhysicalInventory', 1, N'Completed', N'Physical inventory PHY-2024-001 completed. Total items counted: 19. No variance.', N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', CAST(N'2025-10-20T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-20T07:02:38.1666667' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'StockAdjustment', 1, N'Created', N'Stock adjustment ADJ-2024-001 created. Reason: Physical Count Mismatch', N'a27a83b4-740f-42b5-b06e-8e438e209f17', CAST(N'2025-10-23T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-23T07:02:38.1666667' AS DateTime2), N'a27a83b4-740f-42b5-b06e-8e438e209f17', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'WriteOff', 1, N'Approved', N'Write-off request WO-2024-001 approved. Damaged items disposed.', N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', CAST(N'2025-10-25T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-25T07:02:38.1666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Audits] ([Id], [Entity], [EntityId], [Action], [Changes], [UserId], [Timestamp], [IsRead], [ReadAt], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'Return', 1, N'Created', N'Return voucher RET-2024-001 created. Items returned to store.', N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', CAST(N'2025-10-27T07:02:38.1666667' AS DateTime2), 1, NULL, CAST(N'2025-10-27T07:02:38.1666667' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Audits] OFF
GO
SET IDENTITY_INSERT [dbo].[Barcodes] ON 
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'AVDP00005002', N'EAN13', N'AVDP-IT-DESK-001', N'StoreItem', 2, 5, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'AVDP00006002', N'EAN13', N'AVDP-IT-PRNT-001', N'StoreItem', 3, 6, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'AVDP00007002', N'EAN13', N'AVDP-ST-PAPR-001', N'StoreItem', 4, 7, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'AVDP00008002', N'EAN13', N'AVDP-ST-PEN-001', N'StoreItem', 5, 8, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'AVDP00009002', N'EAN13', N'AVDP-ST-PEN-002', N'StoreItem', 6, 9, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'AVDP00010002', N'EAN13', N'AVDP-ST-PENC-001', N'StoreItem', 7, 10, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'AVDP00011002', N'EAN13', N'AVDP-ST-NOTE-001', N'StoreItem', 8, 11, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'AVDP00012002', N'EAN13', N'AVDP-ST-FILE-001', N'StoreItem', 9, 12, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'AVDP00013002', N'EAN13', N'AVDP-UC-SHIRT-001', N'StoreItem', 10, 13, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, N'AVDP00014002', N'EAN13', N'AVDP-UC-PANT-001', N'StoreItem', 11, 14, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, N'AVDP00015002', N'EAN13', N'AVDP-UC-SHIRT-002', N'StoreItem', 12, 15, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, N'AVDP00016002', N'EAN13', N'AVDP-UC-PANT-002', N'StoreItem', 13, 16, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, N'AVDP00017002', N'EAN13', N'AVDP-UC-BELT-001', N'StoreItem', 14, 17, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, N'AVDP00018002', N'EAN13', N'AVDP-UC-CAP-001', N'StoreItem', 15, 18, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, N'AVDP00019002', N'EAN13', N'AVDP-IT-HDD-001', N'StoreItem', 16, 19, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, N'AVDP00020002', N'EAN13', N'AVDP-IT-USB-001', N'StoreItem', 17, 20, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, N'AVDP00021002', N'EAN13', N'AVDP-IT-CABL-001', N'StoreItem', 18, 21, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, N'AVDP00022002', N'EAN13', N'AVDP-IT-CABL-002', N'StoreItem', 19, 22, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Barcodes] ([Id], [BarcodeNumber], [BarcodeType], [BarcodeData], [ReferenceType], [ReferenceId], [ItemId], [StoreId], [BatchNumber], [SerialNumber], [Location], [Notes], [GeneratedDate], [GeneratedBy], [PrintedBy], [PrintedDate], [PrintCount], [LastScannedDate], [LastScannedBy], [LastScannedLocation], [ScanCount], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (21, N'AVDP00023002', N'EAN13', N'AVDP-IT-WCAM-001', N'StoreItem', 20, 23, 2, NULL, NULL, NULL, NULL, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 0, NULL, NULL, NULL, 0, CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Barcodes] OFF
GO
SET IDENTITY_INSERT [dbo].[BatchTrackings] ON 
GO
INSERT [dbo].[BatchTrackings] ([Id], [BatchNumber], [ItemId], [StoreId], [Quantity], [InitialQuantity], [ManufactureDate], [ExpiryDate], [SupplierBatchNo], [CostPrice], [SellingPrice], [Location], [Status], [ConsumedDate], [Notes], [CurrentQuantity], [ReservedQuantity], [Supplier], [UnitCost], [Remarks], [RemainingQuantity], [LastIssueDate], [ReceivedDate], [ReceivedQuantity], [VendorId], [VendorBatchNo], [PurchaseOrderNo], [QualityCheckStatus], [QualityCheckDate], [QualityCheckBy], [StorageLocation], [Temperature], [Humidity], [QuarantineDate], [QuarantinedBy], [QuarantineReason], [TransferredFromBatchId], [TransferReference], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'BATCH-EXP-001', 7, 2, CAST(0.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(N'2022-11-04T07:03:27.8100000' AS DateTime2), CAST(N'2025-09-04T07:03:27.8100000' AS DateTime2), NULL, CAST(450.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), NULL, N'Expired', NULL, NULL, CAST(0.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), NULL, NULL, NULL, CAST(0.000 AS Decimal(18, 3)), CAST(N'2025-09-04T07:03:27.8100000' AS DateTime2), CAST(N'2023-11-04T07:03:27.8100000' AS DateTime2), CAST(50.000 AS Decimal(18, 3)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2023-11-04T07:03:27.8100000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[BatchTrackings] OFF
GO
SET IDENTITY_INSERT [dbo].[Battalions] ON 
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'1st Ansar Battalion', N'AB-01', 0, N'Mirpur, Dhaka', N'Lt. Colonel Md. Jahangir Alam', N'Lt. Colonel', N'01700-101001', N'ab01@ansar.gov.bd', 2, NULL, 500, 25, 475, NULL, NULL, N'১ম আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T03:12:26.8466667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'2nd Ansar Battalion', N'AB-02', 0, N'Savar, Dhaka', N'Lt. Colonel Md. Monirul Islam', N'Lt. Colonel', N'01700-102001', N'ab02@ansar.gov.bd', 2, NULL, 450, 22, 428, NULL, NULL, N'২য় আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'3rd Ansar Battalion', N'AB-03', 0, N'Gazipur', N'Lt. Colonel Md. Aminul Haque', N'Lt. Colonel', N'01700-103001', N'ab03@ansar.gov.bd', 2, NULL, 480, 24, 456, NULL, NULL, N'৩য় আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'1st VDP Battalion', N'VB-01', 1, N'Narsingdi', N'Major Md. Rafiqul Islam', N'Major', N'01700-201001', N'vb01@vdp.gov.bd', 2, NULL, 300, 15, 285, NULL, NULL, N'১ম ভিডিপি ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'1st Rajshahi Ansar Battalion', N'AB-RAJ-01', 0, N'Rajshahi Cantonment', N'Lt. Colonel Md. Kamal Uddin', N'Lt. Colonel', N'01700-301001', N'ab-raj01@ansar.gov.bd', 3, NULL, 420, 20, 400, NULL, NULL, N'১ম রাজশাহী আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'2nd Rajshahi Ansar Battalion', N'AB-RAJ-02', 0, N'Bogura', N'Major Md. Shahjahan Ali', N'Major', N'01700-302001', N'ab-raj02@ansar.gov.bd', 3, NULL, 380, 18, 362, NULL, NULL, N'২য় রাজশাহী আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'1st Chattogram Ansar Battalion', N'AB-CTG-01', 0, N'Chattogram Cantonment', N'Lt. Colonel Md. Nurul Amin', N'Lt. Colonel', N'01700-401001', N'ab-ctg01@ansar.gov.bd', 4, NULL, 520, 26, 494, NULL, NULL, N'১ম চট্টগ্রাম আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'2nd Chattogram Ansar Battalion', N'AB-CTG-02', 0, N'Coxs Bazar', N'Major Md. Ashraful Alam', N'Major', N'01700-402001', N'ab-ctg02@ansar.gov.bd', 4, NULL, 450, 22, 428, NULL, NULL, N'২য় চট্টগ্রাম আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'1st Khulna Ansar Battalion', N'AB-KHL-01', 0, N'Khulna Cantonment', N'Lt. Colonel Md. Mizanur Rahman', N'Lt. Colonel', N'01700-501001', N'ab-khl01@ansar.gov.bd', 5, NULL, 400, 20, 380, NULL, NULL, N'১ম খুলনা আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'1st Barisal Ansar Battalion', N'AB-BAR-01', 0, N'Barisal Cantonment', N'Major Md. Abdul Latif', N'Major', N'01700-601001', N'ab-bar01@ansar.gov.bd', 6, NULL, 350, 18, 332, NULL, NULL, N'১ম বরিশাল আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Battalions] ([Id], [Name], [Code], [Type], [Location], [CommanderName], [CommanderRank], [ContactNumber], [Email], [RangeId], [Remarks], [TotalPersonnel], [OfficerCount], [EnlistedCount], [EstablishedDate], [OperationalStatus], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'1st Sylhet Ansar Battalion', N'AB-SYL-01', 0, N'Sylhet Cantonment', N'Lt. Colonel Md. Delwar Hossain', N'Lt. Colonel', N'01700-701001', N'ab-syl01@ansar.gov.bd', 7, NULL, 420, 21, 399, NULL, NULL, N'১ম সিলেট আনসার ব্যাটালিয়ন', CAST(N'2025-11-04T06:35:56.0366667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Battalions] OFF
GO
SET IDENTITY_INSERT [dbo].[BattalionStores] ON 
GO
INSERT [dbo].[BattalionStores] ([Id], [BattalionId], [StoreId], [IsPrimaryStore], [EffectiveFrom], [EffectiveTo], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 1, 2, 1, CAST(N'2025-08-06T05:04:17.1200000' AS DateTime2), NULL, CAST(N'2025-08-06T05:04:17.1200000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[BattalionStores] ([Id], [BattalionId], [StoreId], [IsPrimaryStore], [EffectiveFrom], [EffectiveTo], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 1, 3, 0, CAST(N'2025-09-05T05:04:17.1200000' AS DateTime2), NULL, CAST(N'2025-09-05T05:04:17.1200000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[BattalionStores] OFF
GO
SET IDENTITY_INSERT [dbo].[Brands] ON 
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'Dell', N'Dell Computer Corporation', N'BRAND-DELL', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'HP', N'Hewlett-Packard', N'BRAND-HP', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Lenovo', N'Lenovo Group Limited', N'BRAND-LENO', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'Samsung', N'Samsung Electronics', N'BRAND-SAMS', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'Canon', N'Canon Inc.', N'BRAND-CANO', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'Epson', N'Seiko Epson Corporation', N'BRAND-EPSO', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'TP-Link', N'TP-Link Technologies', N'BRAND-TPL', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'Logitech', N'Logitech International', N'BRAND-LOGI', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'SanDisk', N'SanDisk Corporation', N'BRAND-SAND', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'Seagate', N'Seagate Technology', N'BRAND-SEAG', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Brands] ([Id], [Name], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'Generic', N'Generic Brand', N'BRAND-GENE', CAST(N'2025-11-04T04:23:16.3100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Brands] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 
GO
INSERT [dbo].[Categories] ([Id], [Name], [NameBn], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'IT Equipment', N'আইটি সরঞ্জাম', N'Information Technology Equipment', N'CAT-IT', CAST(N'2025-11-04T03:12:26.8533333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Categories] ([Id], [Name], [NameBn], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Office Equipment', N'অফিস সরঞ্জাম', N'General Office Equipment', N'CAT-OE', CAST(N'2025-11-04T03:12:26.8533333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Categories] ([Id], [Name], [NameBn], [Description], [Code], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Uniforms & Clothing', N'ইউনিফর্ম ও পোশাক', N'Uniforms, Clothing and Accessories', N'CAT-UC', CAST(N'2025-11-04T03:12:26.8533333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[DamageReportItems] ON 
GO
INSERT [dbo].[DamageReportItems] ([Id], [DamageReportId], [ItemId], [DamagedQuantity], [DamageType], [DamageDate], [DiscoveredDate], [DamageDescription], [EstimatedValue], [PhotoUrls], [BatchNo], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 4, 6, CAST(1.00 AS Decimal(18, 2)), N'Physical', CAST(N'2025-10-26T05:42:26.6400000' AS DateTime2), CAST(N'2025-10-27T05:42:26.6400000' AS DateTime2), N'Water damage to printer during monsoon season', CAST(18000.00 AS Decimal(18, 2)), NULL, N'BATCH-2-6', N'Item beyond repair', CAST(N'2025-10-27T05:42:26.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[DamageReportItems] OFF
GO
SET IDENTITY_INSERT [dbo].[DamageReports] ON 
GO
INSERT [dbo].[DamageReports] ([Id], [ReportNo], [StoreId], [ReportDate], [ReportedBy], [Status], [TotalValue], [ItemId], [Quantity], [DamageType], [Cause], [EstimatedLoss], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'DR-2024-001', 2, CAST(N'2025-10-27T04:53:11.4200000' AS DateTime2), N'Store Keeper - Abdul Karim', 0, CAST(18000.00 AS Decimal(18, 2)), 6, CAST(1.00 AS Decimal(18, 2)), N'Physical', N'Water damage during monsoon', CAST(18000.00 AS Decimal(18, 2)), CAST(N'2025-10-27T04:53:11.4200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[DamageReports] OFF
GO
SET IDENTITY_INSERT [dbo].[DisposalRecords] ON 
GO
INSERT [dbo].[DisposalRecords] ([Id], [WriteOffId], [ItemId], [Quantity], [DisposalMethod], [DisposalDate], [DisposalLocation], [DisposalCompany], [DisposalCertificateNo], [DisposedBy], [WitnessedBy], [PhotoUrls], [Remarks], [DisposalNo], [AuthorizedBy], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 2, 5, CAST(1.00 AS Decimal(18, 2)), N'Auction', CAST(N'2025-10-27T07:03:54.2633333' AS DateTime2), N'Central Store Yard', N'Bangladesh Disposal Services Ltd.', N'DISP-CERT-2024-001', N'Md. Altaf Hossain', N'Abdul Karim, Md. Shahin', NULL, N'Damaged desktop computer disposed through authorized auction', N'DISP-2024-001', N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', CAST(N'2025-10-27T07:03:54.2633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[DisposalRecords] OFF
GO
SET IDENTITY_INSERT [dbo].[ExpiredRecords] ON 
GO
INSERT [dbo].[ExpiredRecords] ([Id], [ItemId], [StoreId], [ExpiredQuantity], [ExpiryDate], [ReferenceType], [ReferenceNo], [Status], [BatchId], [Quantity], [DisposalMethod], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 7, 2, CAST(50.000 AS Decimal(18, 3)), CAST(N'2025-09-04T07:03:38.9100000' AS DateTime2), N'Purchase', N'PO-2024-001', N'Disposed', 1, CAST(50.000 AS Decimal(18, 3)), N'Incineration', CAST(N'2025-09-04T07:03:38.9100000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[ExpiredRecords] OFF
GO
SET IDENTITY_INSERT [dbo].[GoodsReceiveItems] ON 
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 2, 5, CAST(10.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-2-5', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 2, 6, CAST(20.000 AS Decimal(18, 3)), CAST(20.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-2-6', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 2, 7, CAST(800.000 AS Decimal(18, 3)), CAST(800.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-2-7', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 3, 8, CAST(2000.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-3-8', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 3, 9, CAST(2000.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-3-9', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 3, 10, CAST(1000.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-3-10', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, 3, 11, CAST(500.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-3-11', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, 3, 12, CAST(300.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-3-12', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, 4, 13, CAST(200.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-4-13', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, 4, 14, CAST(200.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-4-14', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, 4, 15, CAST(100.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-4-15', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, 4, 16, CAST(100.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-4-16', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, 4, 17, CAST(150.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-4-17', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, 4, 18, CAST(150.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-4-18', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, 5, 19, CAST(15.000 AS Decimal(18, 3)), CAST(15.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-5-19', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, 5, 20, CAST(50.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-5-20', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, 5, 21, CAST(25.000 AS Decimal(18, 3)), CAST(25.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-5-21', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, 5, 22, CAST(40.000 AS Decimal(18, 3)), CAST(40.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-5-22', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceiveItems] ([Id], [GoodsReceiveId], [ItemId], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [QualityCheckStatus], [BatchNo], [ExpiryDate], [ManufactureDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, 5, 23, CAST(8.000 AS Decimal(18, 3)), CAST(8.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), 1, N'BATCH-5-23', NULL, NULL, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[GoodsReceiveItems] OFF
GO
SET IDENTITY_INSERT [dbo].[GoodsReceives] ON 
GO
INSERT [dbo].[GoodsReceives] ([Id], [PurchaseId], [ReceiveDate], [ReceivedBy], [InvoiceNo], [ChallanNo], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 2, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'Store Keeper - Abdul Karim', NULL, NULL, 2, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceives] ([Id], [PurchaseId], [ReceiveDate], [ReceivedBy], [InvoiceNo], [ChallanNo], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 3, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'Store Keeper - Abdul Karim', NULL, NULL, 2, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceives] ([Id], [PurchaseId], [ReceiveDate], [ReceivedBy], [InvoiceNo], [ChallanNo], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 4, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'Store Keeper - Abdul Karim', NULL, NULL, 2, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[GoodsReceives] ([Id], [PurchaseId], [ReceiveDate], [ReceivedBy], [InvoiceNo], [ChallanNo], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 5, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), N'Store Keeper - Abdul Karim', NULL, NULL, 2, CAST(N'2025-10-08T04:26:44.7133333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[GoodsReceives] OFF
GO
SET IDENTITY_INSERT [dbo].[InventoryCycleCounts] ON 
GO
INSERT [dbo].[InventoryCycleCounts] ([Id], [CountNumber], [CountDate], [Status], [CountType], [StoreId], [CreatedById], [ApprovedById], [ApprovedDate], [CountedBy], [VerifiedBy], [StartTime], [EndTime], [Duration], [TotalItems], [CountedItems], [VarianceItems], [TotalVarianceValue], [Notes], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'CYC-2024-001', CAST(N'2025-10-15T07:01:54.8733333' AS DateTime2), N'Completed', N'A Items', 2, N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, N'Abdul Karim', N'Md. Altaf Hossain', CAST(N'2025-10-15T04:01:54.8733333' AS DateTime2), CAST(N'2025-10-15T06:01:54.8733333' AS DateTime2), NULL, 5, 5, 0, CAST(0.00 AS Decimal(18, 2)), N'Quarterly A-class items cycle count completed', CAST(N'2025-10-15T07:01:54.8733333' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[InventoryCycleCounts] ([Id], [CountNumber], [CountDate], [Status], [CountType], [StoreId], [CreatedById], [ApprovedById], [ApprovedDate], [CountedBy], [VerifiedBy], [StartTime], [EndTime], [Duration], [TotalItems], [CountedItems], [VarianceItems], [TotalVarianceValue], [Notes], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'CYC-2024-002', CAST(N'2025-10-25T07:01:54.8733333' AS DateTime2), N'Completed', N'B Items', 2, N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, N'Abdul Karim', N'Md. Altaf Hossain', CAST(N'2025-10-25T03:01:54.8733333' AS DateTime2), CAST(N'2025-10-25T06:01:54.8733333' AS DateTime2), NULL, 10, 10, 1, CAST(450.00 AS Decimal(18, 2)), N'Monthly B-class items cycle count - minor variance found', CAST(N'2025-10-25T07:01:54.8733333' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[InventoryCycleCounts] ([Id], [CountNumber], [CountDate], [Status], [CountType], [StoreId], [CreatedById], [ApprovedById], [ApprovedDate], [CountedBy], [VerifiedBy], [StartTime], [EndTime], [Duration], [TotalItems], [CountedItems], [VarianceItems], [TotalVarianceValue], [Notes], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'CYC-2024-003', CAST(N'2025-11-01T07:01:54.8733333' AS DateTime2), N'In Progress', N'C Items', 2, N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, N'Abdul Karim', NULL, CAST(N'2025-11-01T05:01:54.8733333' AS DateTime2), NULL, NULL, 19, 8, 0, CAST(0.00 AS Decimal(18, 2)), N'Weekly C-class items cycle count in progress', CAST(N'2025-11-01T07:01:54.8733333' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[InventoryCycleCounts] OFF
GO
SET IDENTITY_INSERT [dbo].[IssueItems] ON 
GO
INSERT [dbo].[IssueItems] ([Id], [IssueId], [ItemId], [StoreId], [Quantity], [IssuedQuantity], [RequestedQuantity], [ApprovedQuantity], [Unit], [BatchNumber], [Condition], [Remarks], [HandoverRemarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive], [LedgerNo], [PageNo], [PartiallyUsableQuantity], [UnusableQuantity], [UsableQuantity]) VALUES (1, 5, 5, 2, CAST(5.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), N'Piece', NULL, NULL, NULL, NULL, CAST(N'2025-10-30T03:35:15.1133333' AS DateTime2), NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[IssueItems] ([Id], [IssueId], [ItemId], [StoreId], [Quantity], [IssuedQuantity], [RequestedQuantity], [ApprovedQuantity], [Unit], [BatchNumber], [Condition], [Remarks], [HandoverRemarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive], [LedgerNo], [PageNo], [PartiallyUsableQuantity], [UnusableQuantity], [UsableQuantity]) VALUES (2, 5, 6, 2, CAST(10.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), N'Piece', NULL, NULL, NULL, NULL, CAST(N'2025-10-30T03:35:15.1133333' AS DateTime2), NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[IssueItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Issues] ON 
GO
INSERT [dbo].[Issues] ([Id], [IssuerSignatureId], [ApproverSignatureId], [ReceiverSignatureId], [SignerName], [SignerBadgeId], [SignerDesignation], [SignedDate], [IssueNo], [IssueNumber], [IssueDate], [Status], [IssuedTo], [IssuedToType], [Purpose], [Remarks], [RequestedBy], [RequestedDate], [ApprovedBy], [ApprovedDate], [ApprovalRemarks], [ApprovalReferenceNo], [ApprovalComments], [RejectionReason], [ApprovedByName], [ApprovedByBadgeNo], [SignaturePath], [SignatureDate], [ReceivedDate], [ReceivedBy], [ReceiverDesignation], [ReceiverContact], [IssuedToBattalionId], [IssuedToRangeId], [IssuedToZilaId], [IssuedToUpazilaId], [IssuedToUnionId], [IssuedToIndividualName], [IssuedToIndividualBadgeNo], [IssuedToIndividualMobile], [ToEntityType], [ToEntityId], [FromStoreId], [DeliveryLocation], [VoucherNo], [VoucherNumber], [VoucherDate], [VoucherGeneratedDate], [VoucherQRCode], [QRCode], [IssuedBy], [IssuedDate], [IsPartialIssue], [ParentIssueId], [CreatedByUserId], [RejectedBy], [RejectedDate], [VoucherDocumentPath], [MemoNo], [MemoDate], [IsDeleted], [DeletedBy], [DeletedAt], [DeletionReason], [AllotmentLetterId], [AllotmentLetterNo], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-30T03:34:51.9066667' AS DateTime2), N'ISS-2024-001', NULL, CAST(N'2025-10-30T03:34:51.9066667' AS DateTime2), N'Completed', N'1st Ansar Battalion', N'Battalion', N'IT Equipment issue', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-31T03:34:51.9066667' AS DateTime2), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, CAST(N'2025-11-04T03:34:51.9066667' AS DateTime2), NULL, NULL, NULL, 0, NULL, NULL, NULL, 2, NULL, CAST(N'2025-10-30T03:34:51.9066667' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Issues] OFF
GO
SET IDENTITY_INSERT [dbo].[ItemModels] ON 
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'OptiPlex 3080', N'OPTIPLEX-3080', 1, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Vostro 3491', N'VOSTRO-3491', 1, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'LaserJet Pro M404', N'M404DN', 2, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'LaserJet Pro M15w', N'M15W', 2, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'ThinkCentre M70q', N'M70Q-GEN2', 3, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'PIXMA G3010', N'PIXMA-G3010', 5, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'L3150 EcoTank', N'L3150', 6, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'M185 Wireless Mouse', N'M185', 8, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'K120 Keyboard', N'K120', 8, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'Ultra USB 3.0 32GB', N'SDCZ48-032G', 9, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'Backup Plus 1TB', N'STDR1000300', 10, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, N'Webcam C270', N'C270', 8, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ItemModels] ([Id], [Name], [ModelNumber], [BrandId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, N'Standard Model', N'STD-001', 11, CAST(N'2025-11-04T04:23:51.5966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[ItemModels] OFF
GO
SET IDENTITY_INSERT [dbo].[Items] ON 
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'Desktop Computer Core i5', N'ডেস্কটপ কম্পিউটার কোর আই৫', N'IT-DESK-001', NULL, NULL, N'Piece', 1, 0, 1, 1, 2, 1, CAST(5.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(45000.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T03:14:13.5800000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'Laser Printer A4', N'লেজার প্রিন্টার এ৪', N'IT-PRNT-001', NULL, NULL, N'Piece', 1, 0, 2, 1, 3, 2, CAST(3.000 AS Decimal(18, 3)), CAST(30.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(18000.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T03:14:23.7466667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'A4 Paper 80GSM (Ream)', N'এ৪ কাগজ ৮০ জিএসএম (রিম)', N'ST-PAPR-001', NULL, NULL, N'Ream', 0, 0, 3, 2, 13, 11, CAST(200.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(450.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T03:14:23.7500000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'Ballpoint Pen Blue', N'বলপয়েন্ট কলম নীল', N'ST-PEN-001', NULL, NULL, N'Piece', 0, 0, 4, 2, 13, 11, CAST(500.000 AS Decimal(18, 3)), CAST(5000.000 AS Decimal(18, 3)), CAST(750.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:04:48.9100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'Ballpoint Pen Black', N'বলপয়েন্ট কলম কালো', N'ST-PEN-002', NULL, NULL, N'Piece', 0, 0, 4, 2, 13, 11, CAST(500.000 AS Decimal(18, 3)), CAST(5000.000 AS Decimal(18, 3)), CAST(750.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:04:48.9100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'Pencil HB', N'পেন্সিল এইচবি', N'ST-PENC-001', NULL, NULL, N'Piece', 0, 0, 4, 2, 13, 11, CAST(300.000 AS Decimal(18, 3)), CAST(3000.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(3.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:04:48.9100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'Notebook 100 Pages', N'স্টিকি নোট প্যাড', N'ST-NOTE-001', NULL, NULL, N'Piece', 0, 0, 4, 2, 13, 11, CAST(200.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(45.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:04:48.9100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, N'File Folder', N'ফাইল ফোল্ডার এ৪', N'ST-FILE-001', NULL, NULL, N'Piece', 0, 0, 4, 2, 13, 11, CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(25.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:04:48.9100000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, N'Ansar Uniform Shirt', N'আনসার শার্ট', N'UC-SHIRT-001', NULL, NULL, N'Piece', 1, 0, 5, 3, 13, 11, CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(850.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 1, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:01.3600000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, N'Ansar Uniform Pant', N'আনসার প্যান্ট', N'UC-PANT-001', NULL, NULL, N'Piece', 1, 0, 5, 3, 13, 11, CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(950.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 1, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:01.3600000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, N'VDP Uniform Shirt', N'ভিডিপি ইউনিফর্ম শার্ট', N'UC-SHIRT-002', NULL, NULL, N'Piece', 1, 0, 5, 3, 13, 11, CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(750.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 1, 1, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:01.3600000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, N'VDP Uniform Pant', N'ভিডিপি ইউনিফর্ম প্যান্ট', N'UC-PANT-002', NULL, NULL, N'Piece', 1, 0, 5, 3, 13, 11, CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(850.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 1, 1, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:01.3600000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, N'Belt Black', N'আনসার বেল্ট', N'UC-BELT-001', NULL, NULL, N'Piece', 1, 0, 5, 3, 13, 11, CAST(50.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(75.000 AS Decimal(18, 3)), CAST(350.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 1, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:01.3600000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, N'Cap Ansar', N'আনসার টুপি', N'UC-CAP-001', NULL, NULL, N'Piece', 1, 0, 5, 3, 13, 11, CAST(50.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(75.000 AS Decimal(18, 3)), CAST(250.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 1, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:01.3600000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, N'External Hard Drive 1TB', N'এক্সটার্নাল হার্ড ড্রাইভ ১টিবি', N'IT-HDD-001', NULL, NULL, N'Piece', 1, 0, 6, 1, 11, 10, CAST(10.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(15.000 AS Decimal(18, 3)), CAST(4500.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:14.1166667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, N'USB Flash Drive 32GB', N'ইউএসবি ফ্ল্যাশ ড্রাইভ ৩২জিবি', N'IT-USB-001', NULL, NULL, N'Piece', 0, 0, 6, 1, 10, 9, CAST(20.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(30.000 AS Decimal(18, 3)), CAST(650.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:14.1166667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (21, N'HDMI Cable 2m', N'এইচডিএমআই ক্যাবল ২মি', N'IT-CABL-001', NULL, NULL, N'Piece', 1, 0, 6, 1, NULL, NULL, CAST(15.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(25.000 AS Decimal(18, 3)), CAST(350.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:14.1166667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (22, N'Network Cable Cat6 5m', N'নেটওয়ার্ক ক্যাবল ক্যাট৬ ৫মি', N'IT-CABL-002', NULL, NULL, N'Piece', 1, 0, 6, 1, NULL, NULL, CAST(30.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(250.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:14.1166667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Items] ([Id], [Name], [NameBn], [ItemCode], [Code], [Description], [Unit], [Type], [Status], [SubCategoryId], [CategoryId], [ItemModelId], [BrandId], [MinimumStock], [MaximumStock], [ReorderLevel], [UnitPrice], [UnitCost], [Manufacturer], [ManufactureDate], [ExpiryDate], [HasExpiry], [ShelfLife], [StorageRequirements], [RequiresSpecialHandling], [SafetyInstructions], [Weight], [WeightUnit], [Dimensions], [Color], [Material], [IsHazardous], [HazardClass], [ItemImage], [ImagePath], [Barcode], [BarcodePath], [QRCodeData], [ItemControlType], [AnsarLifeSpanMonths], [VDPLifeSpanMonths], [AnsarAlertBeforeDays], [VDPAlertBeforeDays], [LifeSpanMonths], [AlertBeforeDays], [IsAnsarAuthorized], [IsVDPAuthorized], [RequiresPersonalIssue], [AnsarEntitlementQuantity], [VDPEntitlementQuantity], [EntitlementPeriodMonths], [RequiresHigherApproval], [ControlItemCategory], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (23, N'Webcam HD', N'ওয়েবক্যাম এইচডি', N'IT-WCAM-001', NULL, NULL, N'Piece', 1, 0, 6, 1, 12, 8, CAST(5.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(2500.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 0, NULL, NULL, NULL, 0, NULL, CAST(N'2025-11-04T04:05:14.1166667' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Items] OFF
GO
SET IDENTITY_INSERT [dbo].[ItemSpecifications] ON 
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, 5, N'Processor', N'Intel Core i5-12400', N'GHz', N'Technical', 1, 1, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, 5, N'RAM', N'16', N'GB', N'Technical', 1, 2, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, 5, N'Storage', N'512', N'GB SSD', N'Technical', 1, 3, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, 5, N'Weight', N'8.5', N'KG', N'Physical', 0, 4, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, 5, N'Operating System', N'Windows 11 Pro', NULL, N'Software', 1, 5, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, 6, N'Print Speed', N'38', N'PPM', N'Technical', 1, 1, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, 6, N'Resolution', N'1200x1200', N'DPI', N'Technical', 1, 2, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, 6, N'Paper Capacity', N'250', N'Sheets', N'Technical', 0, 3, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, 6, N'Connectivity', N'USB, Ethernet, WiFi', NULL, N'Technical', 1, 4, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, 7, N'Paper Weight', N'80', N'GSM', N'Physical', 1, 1, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, 7, N'Sheet Count', N'500', N'Sheets/Ream', N'Packaging', 1, 2, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[ItemSpecifications] ([Id], [ItemId], [SpecificationName], [SpecificationValue], [Unit], [Category], [IsRequired], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, 7, N'Brightness', N'95', N'Percentage', N'Quality', 0, 3, CAST(N'2025-11-04T07:00:13.1433333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[ItemSpecifications] OFF
GO
SET IDENTITY_INSERT [dbo].[Locations] ON 
GO
INSERT [dbo].[Locations] ([Id], [Name], [Code], [Description], [ParentLocationId], [LocationType], [Address], [Latitude], [Longitude], [ContactPerson], [ContactPhone], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'Central Store Main Warehouse', N'LOC-CS-MAIN', N'Main storage area for Central Store', NULL, N'Warehouse', N'Ansar & VDP HQ, Building A, Dhaka-1000', NULL, NULL, N'Abdul Karim', N'01700-501001', CAST(N'2025-11-04T05:42:54.7333333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Locations] ([Id], [Name], [Code], [Description], [ParentLocationId], [LocationType], [Address], [Latitude], [Longitude], [ContactPerson], [ContactPhone], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Central Store IT Section', N'LOC-CS-IT', N'IT equipment storage section', NULL, N'Section', N'Ansar & VDP HQ, Building A, Floor 2', NULL, NULL, N'Abdul Karim', N'01700-501001', CAST(N'2025-11-04T05:42:54.7333333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Locations] ([Id], [Name], [Code], [Description], [ParentLocationId], [LocationType], [Address], [Latitude], [Longitude], [ContactPerson], [ContactPhone], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Provision Store Main Area', N'LOC-PS-MAIN', N'Main storage for provision items', NULL, N'Warehouse', N'Dhaka Provision Store, Tejgaon', NULL, NULL, N'Store Keeper', N'01700-502001', CAST(N'2025-11-04T05:42:54.7333333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Locations] ([Id], [Name], [Code], [Description], [ParentLocationId], [LocationType], [Address], [Latitude], [Longitude], [ContactPerson], [ContactPhone], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'Provision Store Uniform Section', N'LOC-PS-UNI', N'Uniform and clothing storage', NULL, N'Section', N'Dhaka Provision Store, Section B', NULL, NULL, N'Store Keeper', N'01700-502001', CAST(N'2025-11-04T05:42:54.7333333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Locations] OFF
GO
SET IDENTITY_INSERT [dbo].[PersonnelItemIssues] ON 
GO
INSERT [dbo].[PersonnelItemIssues] ([Id], [IssueNo], [PersonnelId], [PersonnelType], [PersonnelName], [PersonnelBadgeNo], [PersonnelUnit], [PersonnelDesignation], [PersonnelMobile], [ItemId], [Quantity], [Unit], [OriginalIssueId], [ReceiveId], [IssueDate], [ReceivedDate], [LifeExpiryDate], [AlertDate], [RemainingDays], [Status], [ReplacedDate], [ReplacementReason], [ReplacementIssueId], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [StoreId], [IsAlertSent], [LastAlertDate], [AlertCount], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'PIE-2024-001', N'1001', N'0', N'Md. Rafiqul Islam', N'ANSAR-1001', N'1st Ansar Battalion', N'Ansar Member', N'01711-111001', 13, CAST(2.00 AS Decimal(18, 2)), N'Piece', NULL, NULL, CAST(N'2025-05-08T04:54:03.5166667' AS DateTime2), NULL, CAST(N'2027-05-03T04:54:03.5166667' AS DateTime2), CAST(N'2027-04-03T04:54:03.5166667' AS DateTime2), 515, N'0', NULL, NULL, NULL, 1, 2, NULL, NULL, NULL, 0, NULL, 0, NULL, CAST(N'2025-05-08T04:54:03.5166667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PersonnelItemIssues] ([Id], [IssueNo], [PersonnelId], [PersonnelType], [PersonnelName], [PersonnelBadgeNo], [PersonnelUnit], [PersonnelDesignation], [PersonnelMobile], [ItemId], [Quantity], [Unit], [OriginalIssueId], [ReceiveId], [IssueDate], [ReceivedDate], [LifeExpiryDate], [AlertDate], [RemainingDays], [Status], [ReplacedDate], [ReplacementReason], [ReplacementIssueId], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [StoreId], [IsAlertSent], [LastAlertDate], [AlertCount], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'PIE-2024-002', N'1002', N'0', N'Abdul Hakim Khan', N'ANSAR-1002', N'1st Ansar Battalion', N'Lance Corporal', N'01722-222002', 14, CAST(2.00 AS Decimal(18, 2)), N'Piece', NULL, NULL, CAST(N'2025-05-28T04:54:13.1633333' AS DateTime2), NULL, CAST(N'2027-05-23T04:54:13.1633333' AS DateTime2), CAST(N'2027-04-23T04:54:13.1633333' AS DateTime2), 535, N'0', NULL, NULL, NULL, 1, 2, NULL, NULL, NULL, 0, NULL, 0, NULL, CAST(N'2025-05-28T04:54:13.1633333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PersonnelItemIssues] ([Id], [IssueNo], [PersonnelId], [PersonnelType], [PersonnelName], [PersonnelBadgeNo], [PersonnelUnit], [PersonnelDesignation], [PersonnelMobile], [ItemId], [Quantity], [Unit], [OriginalIssueId], [ReceiveId], [IssueDate], [ReceivedDate], [LifeExpiryDate], [AlertDate], [RemainingDays], [Status], [ReplacedDate], [ReplacementReason], [ReplacementIssueId], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [StoreId], [IsAlertSent], [LastAlertDate], [AlertCount], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'PIE-2024-003', N'2001', N'1', N'Sharmin Akter', N'VDP-2001', N'Dhaka Zila VDP', N'VDP Member', N'01733-333003', 18, CAST(1.00 AS Decimal(18, 2)), N'Piece', NULL, NULL, CAST(N'2025-08-06T04:54:23.4200000' AS DateTime2), NULL, CAST(N'2026-08-06T04:54:23.4200000' AS DateTime2), CAST(N'2026-07-07T04:54:23.4200000' AS DateTime2), 245, N'0', NULL, NULL, NULL, NULL, 2, 1, NULL, NULL, 0, NULL, 0, NULL, CAST(N'2025-08-06T04:54:23.4200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[PersonnelItemIssues] OFF
GO
SET IDENTITY_INSERT [dbo].[PhysicalInventories] ON 
GO
INSERT [dbo].[PhysicalInventories] ([Id], [CountNo], [CountDate], [StoreId], [CategoryId], [CountedBy], [VerifiedBy], [StartTime], [EndTime], [Remarks], [TotalSystemValue], [TotalPhysicalValue], [VarianceValue], [TotalItemsCounted], [ItemsWithVariance], [IsReconciled], [ReconciliationDate], [ReconciliationBy], [IsStockFrozen], [StockFrozenAt], [CountTeam], [SupervisorId], [ReferenceNumber], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [FiscalYear], [Status], [CountType], [InitiatedBy], [InitiatedDate], [CompletedBy], [CompletedDate], [ReviewedBy], [ReviewedDate], [ApprovedBy], [ApprovedDate], [RejectedBy], [RejectedDate], [CancelledBy], [CancelledDate], [ApprovalReference], [ApprovalRemarks], [RejectionReason], [CancellationReason], [ReviewRemarks], [TotalSystemQuantity], [TotalPhysicalQuantity], [TotalVariance], [TotalVarianceValue], [IsAuditRequired], [AuditOfficer], [AdjustmentStatus], [AdjustedDate], [CountEndTime], [AdjustmentCreatedDate], [AdjustmentNo], [PostedDate], [CountedByUserId], [VerifiedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'PHY-2024-001', CAST(N'2025-10-20T07:00:51.1266667' AS DateTime2), 2, NULL, N'Abdul Karim', N'Md. Altaf Hossain', CAST(N'2025-10-20T03:00:51.1266667' AS DateTime2), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(18, 2)), 19, 0, 1, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 3, 0, N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', CAST(N'2025-10-20T07:00:51.1266667' AS DateTime2), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, NULL, CAST(N'2025-10-20T07:00:51.1266667' AS DateTime2), CAST(N'2025-11-04T07:00:51.1266667' AS DateTime2), NULL, CAST(N'2025-11-04T07:00:51.1266667' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', CAST(N'2025-10-20T07:00:51.1266667' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[PhysicalInventories] OFF
GO
SET IDENTITY_INSERT [dbo].[PhysicalInventoryItems] ON 
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 1, 5, CAST(10.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(45000.00 AS Decimal(18, 2)), CAST(450000.00 AS Decimal(18, 2)), CAST(450000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 1, 6, CAST(20.000 AS Decimal(18, 3)), CAST(20.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(18000.00 AS Decimal(18, 2)), CAST(360000.00 AS Decimal(18, 2)), CAST(360000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 1, 7, CAST(800.000 AS Decimal(18, 3)), CAST(800.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(450.00 AS Decimal(18, 2)), CAST(360000.00 AS Decimal(18, 2)), CAST(360000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 1, 8, CAST(2000.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 1, 9, CAST(2000.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 1, 10, CAST(1000.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(3.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, 1, 11, CAST(500.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(45.00 AS Decimal(18, 2)), CAST(22500.00 AS Decimal(18, 2)), CAST(22500.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, 1, 12, CAST(300.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(25.00 AS Decimal(18, 2)), CAST(7500.00 AS Decimal(18, 2)), CAST(7500.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, 1, 13, CAST(200.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(850.00 AS Decimal(18, 2)), CAST(170000.00 AS Decimal(18, 2)), CAST(170000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
INSERT [dbo].[PhysicalInventoryItems] ([Id], [PhysicalInventoryId], [ItemId], [SystemQuantity], [PhysicalQuantity], [Variance], [UnitCost], [SystemValue], [PhysicalValue], [VarianceValue], [Location], [BatchNumber], [CountedAt], [CountedBy], [IsRecounted], [RecountQuantity], [Notes], [AdjustmentStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, 1, 14, CAST(200.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(950.00 AS Decimal(18, 2)), CAST(190000.00 AS Decimal(18, 2)), CAST(190000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'Abdul Karim', 0, NULL, NULL, NULL, CAST(N'2025-10-20T07:01:23.0900000' AS DateTime2), N'7853d7da-baac-4dfd-9bb5-ae6fc0f16c6d', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[PhysicalInventoryItems] OFF
GO
SET IDENTITY_INSERT [dbo].[PurchaseItems] ON 
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 2, 5, NULL, CAST(10.000 AS Decimal(18, 3)), CAST(45000.00 AS Decimal(18, 2)), CAST(450000.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-05T03:14:43.5500000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 2, 6, NULL, CAST(20.000 AS Decimal(18, 3)), CAST(18000.00 AS Decimal(18, 2)), CAST(360000.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-05T03:14:43.5500000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 2, 7, NULL, CAST(800.000 AS Decimal(18, 3)), CAST(450.00 AS Decimal(18, 2)), CAST(360000.00 AS Decimal(18, 2)), CAST(800.00 AS Decimal(18, 2)), CAST(800.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-05T03:14:43.5500000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 3, 8, NULL, CAST(2000.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-15T04:06:06.7333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 3, 9, NULL, CAST(2000.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-15T04:06:06.7333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, 3, 10, NULL, CAST(1000.000 AS Decimal(18, 3)), CAST(3.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-15T04:06:06.7333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, 3, 11, NULL, CAST(500.000 AS Decimal(18, 3)), CAST(45.00 AS Decimal(18, 2)), CAST(22500.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-15T04:06:06.7333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, 3, 12, NULL, CAST(300.000 AS Decimal(18, 3)), CAST(25.00 AS Decimal(18, 2)), CAST(7500.00 AS Decimal(18, 2)), CAST(300.00 AS Decimal(18, 2)), CAST(300.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-15T04:06:06.7333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, 4, 13, NULL, CAST(200.000 AS Decimal(18, 3)), CAST(850.00 AS Decimal(18, 2)), CAST(170000.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-20T04:06:32.1400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, 4, 14, NULL, CAST(200.000 AS Decimal(18, 3)), CAST(950.00 AS Decimal(18, 2)), CAST(190000.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-20T04:06:32.1400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, 4, 15, NULL, CAST(100.000 AS Decimal(18, 3)), CAST(750.00 AS Decimal(18, 2)), CAST(75000.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-20T04:06:32.1400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, 4, 16, NULL, CAST(100.000 AS Decimal(18, 3)), CAST(850.00 AS Decimal(18, 2)), CAST(85000.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-20T04:06:32.1400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, 4, 17, NULL, CAST(150.000 AS Decimal(18, 3)), CAST(350.00 AS Decimal(18, 2)), CAST(52500.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-20T04:06:32.1400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, 4, 18, NULL, CAST(150.000 AS Decimal(18, 3)), CAST(250.00 AS Decimal(18, 2)), CAST(37500.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-20T04:06:32.1400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, 5, 19, NULL, CAST(15.000 AS Decimal(18, 3)), CAST(4500.00 AS Decimal(18, 2)), CAST(67500.00 AS Decimal(18, 2)), CAST(15.00 AS Decimal(18, 2)), CAST(15.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-23T04:06:54.1333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, 5, 20, NULL, CAST(50.000 AS Decimal(18, 3)), CAST(650.00 AS Decimal(18, 2)), CAST(32500.00 AS Decimal(18, 2)), CAST(50.00 AS Decimal(18, 2)), CAST(50.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-23T04:06:54.1333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, 5, 21, NULL, CAST(25.000 AS Decimal(18, 3)), CAST(350.00 AS Decimal(18, 2)), CAST(8750.00 AS Decimal(18, 2)), CAST(25.00 AS Decimal(18, 2)), CAST(25.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-23T04:06:54.1333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, 5, 22, NULL, CAST(40.000 AS Decimal(18, 3)), CAST(250.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(40.00 AS Decimal(18, 2)), CAST(40.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-23T04:06:54.1333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[PurchaseItems] ([Id], [PurchaseId], [ItemId], [StoreId], [Quantity], [UnitPrice], [TotalPrice], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [BatchNo], [ExpiryDate], [ReceiveRemarks], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, 5, 23, NULL, CAST(8.000 AS Decimal(18, 3)), CAST(2500.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), CAST(8.00 AS Decimal(18, 2)), CAST(8.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-23T04:06:54.1333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[PurchaseItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Purchases] ON 
GO
INSERT [dbo].[Purchases] ([Id], [PurchaseOrderNo], [PurchaseDate], [ExpectedDeliveryDate], [DeliveryDate], [ReceivedDate], [VendorId], [StoreId], [TotalAmount], [Discount], [PurchaseType], [Status], [Remarks], [ApprovedBy], [ApprovedDate], [RejectionReason], [IsMarketplacePurchase], [MarketplaceUrl], [ProcurementType], [ProcurementSource], [BudgetCode], [CreatedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'PO-2024-001', CAST(N'2025-10-05T03:14:33.8000000' AS DateTime2), CAST(N'2025-10-20T03:14:33.8000000' AS DateTime2), NULL, NULL, 1, 2, CAST(1170000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Direct', N'Received', N'IT Equipment for HQ', NULL, NULL, NULL, 0, NULL, 1, NULL, NULL, NULL, CAST(N'2025-10-05T03:14:33.8000000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Purchases] ([Id], [PurchaseOrderNo], [PurchaseDate], [ExpectedDeliveryDate], [DeliveryDate], [ReceivedDate], [VendorId], [StoreId], [TotalAmount], [Discount], [PurchaseType], [Status], [Remarks], [ApprovedBy], [ApprovedDate], [RejectionReason], [IsMarketplacePurchase], [MarketplaceUrl], [ProcurementType], [ProcurementSource], [BudgetCode], [CreatedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'PO-2024-002', CAST(N'2025-10-15T04:05:54.3233333' AS DateTime2), CAST(N'2025-10-25T04:05:54.3233333' AS DateTime2), NULL, NULL, 2, 2, CAST(175000.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), N'Direct', N'Received', N'Office supplies purchase', NULL, NULL, NULL, 0, NULL, 1, NULL, NULL, NULL, CAST(N'2025-10-15T04:05:54.3233333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Purchases] ([Id], [PurchaseOrderNo], [PurchaseDate], [ExpectedDeliveryDate], [DeliveryDate], [ReceivedDate], [VendorId], [StoreId], [TotalAmount], [Discount], [PurchaseType], [Status], [Remarks], [ApprovedBy], [ApprovedDate], [RejectionReason], [IsMarketplacePurchase], [MarketplaceUrl], [ProcurementType], [ProcurementSource], [BudgetCode], [CreatedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'PO-2024-003', CAST(N'2025-10-20T04:06:17.2400000' AS DateTime2), CAST(N'2025-10-30T04:06:17.2400000' AS DateTime2), NULL, NULL, 3, 2, CAST(550000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Direct', N'Received', N'Ansar & VDP Uniform purchase', NULL, NULL, NULL, 0, NULL, 1, NULL, NULL, NULL, CAST(N'2025-10-20T04:06:17.2400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Purchases] ([Id], [PurchaseOrderNo], [PurchaseDate], [ExpectedDeliveryDate], [DeliveryDate], [ReceivedDate], [VendorId], [StoreId], [TotalAmount], [Discount], [PurchaseType], [Status], [Remarks], [ApprovedBy], [ApprovedDate], [RejectionReason], [IsMarketplacePurchase], [MarketplaceUrl], [ProcurementType], [ProcurementSource], [BudgetCode], [CreatedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'PO-2024-004', CAST(N'2025-10-23T04:06:42.7766667' AS DateTime2), CAST(N'2025-11-01T04:06:42.7766667' AS DateTime2), NULL, NULL, 1, 2, CAST(125000.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), N'Direct', N'Received', N'IT Accessories purchase', NULL, NULL, NULL, 0, NULL, 1, NULL, NULL, NULL, CAST(N'2025-10-23T04:06:42.7766667' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Purchases] OFF
GO
SET IDENTITY_INSERT [dbo].[QualityCheckItems] ON 
GO
INSERT [dbo].[QualityCheckItems] ([Id], [QualityCheckId], [ItemId], [CheckedQuantity], [PassedQuantity], [FailedQuantity], [Status], [Remarks], [CheckParameters], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 2, 5, CAST(10.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, N'Quality check passed - all items meet specifications', NULL, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[QualityCheckItems] ([Id], [QualityCheckId], [ItemId], [CheckedQuantity], [PassedQuantity], [FailedQuantity], [Status], [Remarks], [CheckParameters], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 2, 6, CAST(20.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, N'Quality check passed - all items meet specifications', NULL, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[QualityCheckItems] ([Id], [QualityCheckId], [ItemId], [CheckedQuantity], [PassedQuantity], [FailedQuantity], [Status], [Remarks], [CheckParameters], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 2, 7, CAST(800.00 AS Decimal(18, 2)), CAST(800.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, N'Quality check passed - all items meet specifications', NULL, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[QualityCheckItems] ([Id], [QualityCheckId], [ItemId], [CheckedQuantity], [PassedQuantity], [FailedQuantity], [Status], [Remarks], [CheckParameters], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 3, 5, CAST(10.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, N'Quality check passed - all items meet specifications', NULL, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[QualityCheckItems] ([Id], [QualityCheckId], [ItemId], [CheckedQuantity], [PassedQuantity], [FailedQuantity], [Status], [Remarks], [CheckParameters], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 3, 6, CAST(20.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, N'Quality check passed - all items meet specifications', NULL, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[QualityCheckItems] ([Id], [QualityCheckId], [ItemId], [CheckedQuantity], [PassedQuantity], [FailedQuantity], [Status], [Remarks], [CheckParameters], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 3, 7, CAST(800.00 AS Decimal(18, 2)), CAST(800.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, N'Quality check passed - all items meet specifications', NULL, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[QualityCheckItems] OFF
GO
SET IDENTITY_INSERT [dbo].[QualityChecks] ON 
GO
INSERT [dbo].[QualityChecks] ([Id], [CheckNumber], [CheckDate], [ItemId], [PurchaseId], [CheckType], [CheckedBy], [Status], [Comments], [CheckedQuantity], [PassedQuantity], [FailedQuantity], [FailureReasons], [CorrectiveActions], [RequiresRetest], [RetestDate], [GoodsReceiveId], [CheckedDate], [OverallStatus], [CheckedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'QC-2024-001', CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), 5, NULL, NULL, N'Quality Inspector - Md. Rahman', NULL, NULL, CAST(10.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, 0, NULL, 2, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), 1, NULL, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[QualityChecks] ([Id], [CheckNumber], [CheckDate], [ItemId], [PurchaseId], [CheckType], [CheckedBy], [Status], [Comments], [CheckedQuantity], [PassedQuantity], [FailedQuantity], [FailureReasons], [CorrectiveActions], [RequiresRetest], [RetestDate], [GoodsReceiveId], [CheckedDate], [OverallStatus], [CheckedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'QC-2024-002', CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), 6, NULL, NULL, N'Quality Inspector - Md. Rahman', NULL, NULL, CAST(20.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, 0, NULL, 2, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), 1, NULL, CAST(N'2025-10-09T04:27:13.2833333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[QualityChecks] OFF
GO
SET IDENTITY_INSERT [dbo].[Ranges] ON 
GO
INSERT [dbo].[Ranges] ([Id], [Name], [Code], [HeadquarterLocation], [CommanderName], [CommanderRank], [ContactNumber], [Email], [CoverageArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Dhaka Range', N'DR-01', N'Dhaka Cantonment', N'Brigadier General Md. Kamrul Hassan', N'Brigadier General', N'01700-111111', N'dhaka.range@ansar.gov.bd', N'Dhaka Division', NULL, N'ঢাকা রেঞ্জ', CAST(N'2025-11-04T03:12:26.8400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Ranges] ([Id], [Name], [Code], [HeadquarterLocation], [CommanderName], [CommanderRank], [ContactNumber], [Email], [CoverageArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Rajshahi Range', N'RR-01', N'Rajshahi Cantonment', N'Colonel Md. Habibur Rahman', N'Colonel', N'01700-222222', N'rajshahi.range@ansar.gov.bd', N'Rajshahi Division', NULL, N'রাজশাহী রেঞ্জ', CAST(N'2025-11-04T03:12:26.8400000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Ranges] ([Id], [Name], [Code], [HeadquarterLocation], [CommanderName], [CommanderRank], [ContactNumber], [Email], [CoverageArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'Chattogram Range', N'CTG-01', N'Chattogram Cantonment', N'Brigadier General Md. Asaduzzaman', N'Brigadier General', N'01700-333333', N'chattogram.range@ansar.gov.bd', N'Chattogram Division', NULL, N'চট্টগ্রাম রেঞ্জ', CAST(N'2025-11-04T06:35:24.3533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Ranges] ([Id], [Name], [Code], [HeadquarterLocation], [CommanderName], [CommanderRank], [ContactNumber], [Email], [CoverageArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'Khulna Range', N'KHL-01', N'Khulna Cantonment', N'Colonel Md. Azizur Rahman', N'Colonel', N'01700-444444', N'khulna.range@ansar.gov.bd', N'Khulna Division', NULL, N'খুলনা রেঞ্জ', CAST(N'2025-11-04T06:35:24.3533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Ranges] ([Id], [Name], [Code], [HeadquarterLocation], [CommanderName], [CommanderRank], [ContactNumber], [Email], [CoverageArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'Barisal Range', N'BAR-01', N'Barisal Cantonment', N'Colonel Md. Hafizur Rahman', N'Colonel', N'01700-555555', N'barisal.range@ansar.gov.bd', N'Barisal Division', NULL, N'বরিশাল রেঞ্জ', CAST(N'2025-11-04T06:35:24.3533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Ranges] ([Id], [Name], [Code], [HeadquarterLocation], [CommanderName], [CommanderRank], [ContactNumber], [Email], [CoverageArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'Sylhet Range', N'SYL-01', N'Sylhet Cantonment', N'Colonel Md. Nuruzzaman', N'Colonel', N'01700-666666', N'sylhet.range@ansar.gov.bd', N'Sylhet Division', NULL, N'সিলেট রেঞ্জ', CAST(N'2025-11-04T06:35:24.3533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Ranges] ([Id], [Name], [Code], [HeadquarterLocation], [CommanderName], [CommanderRank], [ContactNumber], [Email], [CoverageArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'Rangpur Range', N'RNG-01', N'Rangpur Cantonment', N'Colonel Md. Mahbubur Rahman', N'Colonel', N'01700-777777', N'rangpur.range@ansar.gov.bd', N'Rangpur Division', NULL, N'রংপুর রেঞ্জ', CAST(N'2025-11-04T06:35:24.3533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Ranges] ([Id], [Name], [Code], [HeadquarterLocation], [CommanderName], [CommanderRank], [ContactNumber], [Email], [CoverageArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'Mymensingh Range', N'MYM-01', N'Mymensingh Cantonment', N'Colonel Md. Shafiqul Islam', N'Colonel', N'01700-888888', N'mymensingh.range@ansar.gov.bd', N'Mymensingh Division', NULL, N'ময়মনসিংহ রেঞ্জ', CAST(N'2025-11-04T06:35:24.3533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Ranges] OFF
GO
SET IDENTITY_INSERT [dbo].[ReceiveItems] ON 
GO
INSERT [dbo].[ReceiveItems] ([Id], [ReceiveId], [ItemId], [StoreId], [Quantity], [IssuedQuantity], [ReceivedQuantity], [BatchNumber], [Condition], [Remarks], [DamageNotes], [DamageDescription], [DamagePhotoPath], [IsScanned], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive], [LedgerNo], [PageNo], [PartiallyUsableQuantity], [UnusableQuantity], [UsableQuantity]) VALUES (1, 3, 13, NULL, CAST(2.000 AS Decimal(18, 3)), CAST(2.000 AS Decimal(18, 3)), CAST(2.000 AS Decimal(18, 3)), NULL, N'Good', N'Uniform in good condition', NULL, NULL, NULL, 0, NULL, CAST(N'2025-10-30T04:55:56.2966667' AS DateTime2), NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[ReceiveItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Receives] ON 
GO
INSERT [dbo].[Receives] ([Id], [ReceiveNo], [ReceiveNumber], [ReceiveDate], [ReceivedDate], [Status], [ReceivedFrom], [ReceivedFromType], [ReceivedBy], [Source], [Remarks], [ReceivedFromBattalionId], [ReceivedFromRangeId], [ReceivedFromZilaId], [ReceivedFromUpazilaId], [ReceivedFromUnionId], [ReceivedFromIndividualName], [ReceivedFromIndividualBadgeNo], [StoreId], [OriginalIssueId], [OriginalIssueNo], [OriginalVoucherNo], [ReceiveType], [ReceiverSignature], [ReceiverName], [ReceiverBadgeNo], [ReceiverDesignation], [IsReceiverSignature], [VerifierSignature], [OverallCondition], [AssessmentNotes], [AssessedBy], [AssessmentDate], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive], [VoucherDate], [VoucherDocumentPath], [VoucherGeneratedDate], [VoucherNo], [VoucherNumber], [VoucherQRCode]) VALUES (3, N'RCV-2024-001', N'RCV-NO-001', CAST(N'2025-10-30T04:55:42.9533333' AS DateTime2), CAST(N'2025-10-30T04:55:42.9533333' AS DateTime2), N'2', N'1st Ansar Battalion', N'Battalion', N'Store Keeper - Abdul Karim', N'Return', N'Items returned after personnel retirement', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Return', NULL, N'Abdul Karim', NULL, N'Store Keeper', 1, 1, N'Good', NULL, N'Abdul Karim', CAST(N'2025-10-30T04:55:42.9533333' AS DateTime2), CAST(N'2025-10-30T04:55:42.9533333' AS DateTime2), NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Receives] OFF
GO
SET IDENTITY_INSERT [dbo].[RequisitionItems] ON 
GO
INSERT [dbo].[RequisitionItems] ([Id], [RequisitionId], [ItemId], [RequestedQuantity], [ApprovedQuantity], [EstimatedUnitPrice], [EstimatedTotalPrice], [Justification], [Status], [IssuedQuantity], [UnitPrice], [TotalPrice], [Specification], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 3, 5, CAST(5.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(45000.00 AS Decimal(18, 2)), CAST(225000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(45000.00 AS Decimal(18, 2)), CAST(225000.00 AS Decimal(18, 2)), NULL, CAST(N'2025-10-10T04:25:58.5066667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[RequisitionItems] ([Id], [RequisitionId], [ItemId], [RequestedQuantity], [ApprovedQuantity], [EstimatedUnitPrice], [EstimatedTotalPrice], [Justification], [Status], [IssuedQuantity], [UnitPrice], [TotalPrice], [Specification], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 3, 6, CAST(2.000 AS Decimal(18, 3)), CAST(2.000 AS Decimal(18, 3)), CAST(18000.00 AS Decimal(18, 2)), CAST(36000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(18000.00 AS Decimal(18, 2)), CAST(36000.00 AS Decimal(18, 2)), NULL, CAST(N'2025-10-10T04:25:58.5066667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[RequisitionItems] ([Id], [RequisitionId], [ItemId], [RequestedQuantity], [ApprovedQuantity], [EstimatedUnitPrice], [EstimatedTotalPrice], [Justification], [Status], [IssuedQuantity], [UnitPrice], [TotalPrice], [Specification], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 4, 8, CAST(1000.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(5.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), NULL, CAST(N'2025-10-17T04:25:58.5200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[RequisitionItems] ([Id], [RequisitionId], [ItemId], [RequestedQuantity], [ApprovedQuantity], [EstimatedUnitPrice], [EstimatedTotalPrice], [Justification], [Status], [IssuedQuantity], [UnitPrice], [TotalPrice], [Specification], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 4, 7, CAST(100.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(450.00 AS Decimal(18, 2)), CAST(45000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(450.00 AS Decimal(18, 2)), CAST(45000.00 AS Decimal(18, 2)), NULL, CAST(N'2025-10-17T04:25:58.5200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[RequisitionItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Requisitions] ON 
GO
INSERT [dbo].[Requisitions] ([Id], [RequisitionNumber], [RequisitionDate], [RequestedBy], [Status], [RequiredDate], [ApprovedBy], [ApprovedDate], [RejectionReason], [RejectedBy], [TotalEstimatedCost], [Notes], [RequestDate], [ApprovalComments], [RejectedDate], [Priority], [Department], [Purpose], [RequiredByDate], [FromStoreId], [ToStoreId], [FulfillmentStatus], [AutoConvertToPO], [PurchaseOrderId], [EstimatedValue], [ApprovedValue], [Level1ApprovedBy], [Level1ApprovedDate], [Level2ApprovedBy], [Level2ApprovedDate], [FinalApprovedBy], [FinalApprovedDate], [CurrentApprovalLevel], [RequestedByUserId], [ApprovedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'REQ-2024-001', CAST(N'2025-10-10T04:25:19.1766667' AS DateTime2), N'Battalion Commander - 1st Battalion', N'Approved', NULL, NULL, NULL, NULL, NULL, CAST(250000.00 AS Decimal(18, 2)), NULL, CAST(N'2025-10-10T04:25:19.1766667' AS DateTime2), NULL, CAST(N'2025-11-04T04:25:19.1766667' AS DateTime2), N'High', NULL, N'IT Equipment for Battalion Office', CAST(N'2025-10-25T04:25:19.1766667' AS DateTime2), 2, NULL, NULL, 0, NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(250000.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, CAST(N'2025-10-10T04:25:19.1766667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Requisitions] ([Id], [RequisitionNumber], [RequisitionDate], [RequestedBy], [Status], [RequiredDate], [ApprovedBy], [ApprovedDate], [RejectionReason], [RejectedBy], [TotalEstimatedCost], [Notes], [RequestDate], [ApprovalComments], [RejectedDate], [Priority], [Department], [Purpose], [RequiredByDate], [FromStoreId], [ToStoreId], [FulfillmentStatus], [AutoConvertToPO], [PurchaseOrderId], [EstimatedValue], [ApprovedValue], [Level1ApprovedBy], [Level1ApprovedDate], [Level2ApprovedBy], [Level2ApprovedDate], [FinalApprovedBy], [FinalApprovedDate], [CurrentApprovalLevel], [RequestedByUserId], [ApprovedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'REQ-2024-002', CAST(N'2025-10-17T04:25:19.1766667' AS DateTime2), N'Range Commander - Dhaka Range', N'Approved', NULL, NULL, NULL, NULL, NULL, CAST(75000.00 AS Decimal(18, 2)), NULL, CAST(N'2025-10-17T04:25:19.1766667' AS DateTime2), NULL, CAST(N'2025-11-04T04:25:19.1766667' AS DateTime2), N'Medium', NULL, N'Office Stationary Requirement', CAST(N'2025-10-30T04:25:19.1766667' AS DateTime2), 2, NULL, NULL, 0, NULL, CAST(75000.00 AS Decimal(18, 2)), CAST(75000.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, CAST(N'2025-10-17T04:25:19.1766667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Requisitions] ([Id], [RequisitionNumber], [RequisitionDate], [RequestedBy], [Status], [RequiredDate], [ApprovedBy], [ApprovedDate], [RejectionReason], [RejectedBy], [TotalEstimatedCost], [Notes], [RequestDate], [ApprovalComments], [RejectedDate], [Priority], [Department], [Purpose], [RequiredByDate], [FromStoreId], [ToStoreId], [FulfillmentStatus], [AutoConvertToPO], [PurchaseOrderId], [EstimatedValue], [ApprovedValue], [Level1ApprovedBy], [Level1ApprovedDate], [Level2ApprovedBy], [Level2ApprovedDate], [FinalApprovedBy], [FinalApprovedDate], [CurrentApprovalLevel], [RequestedByUserId], [ApprovedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'REQ-2024-003', CAST(N'2025-10-27T04:25:19.1766667' AS DateTime2), N'Zila Commander - Dhaka Zila', N'Pending', NULL, NULL, NULL, NULL, NULL, CAST(120000.00 AS Decimal(18, 2)), NULL, CAST(N'2025-10-27T04:25:19.1766667' AS DateTime2), NULL, CAST(N'2025-11-04T04:25:19.1766667' AS DateTime2), N'High', NULL, N'Uniform and Equipment', CAST(N'2025-11-09T04:25:19.1766667' AS DateTime2), 2, NULL, NULL, 0, NULL, CAST(120000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, CAST(N'2025-10-27T04:25:19.1766667' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Requisitions] OFF
GO
SET IDENTITY_INSERT [dbo].[ReturnItems] ON 
GO
INSERT [dbo].[ReturnItems] ([Id], [ReturnId], [ItemId], [ReturnQuantity], [ApprovedQuantity], [ReceivedQuantity], [AcceptedQuantity], [RejectedQuantity], [Condition], [CheckedCondition], [ReturnReason], [BatchNo], [Remarks], [ApprovalRemarks], [ReceivedDate], [ReceivedBy], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 5, 5, CAST(1.000 AS Decimal(18, 3)), CAST(1.000 AS Decimal(18, 3)), CAST(1.000 AS Decimal(18, 3)), CAST(1.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), N'Good', N'Good', N'Surplus', NULL, N'In good working condition', NULL, CAST(N'2025-11-01T04:57:06.9233333' AS DateTime2), N'Abdul Karim', CAST(N'2025-11-01T04:57:06.9233333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[ReturnItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Returns] ON 
GO
INSERT [dbo].[Returns] ([Id], [ReturnNo], [ReturnDate], [Status], [ReturnedBy], [ReturnedByType], [Reason], [ReturnType], [ItemId], [StoreId], [ToStoreId], [Quantity], [RequestedBy], [RequestedDate], [ApprovedBy], [ApprovedDate], [ApprovalRemarks], [ReceivedBy], [ReceivedDate], [CompletedDate], [ReceiptRemarks], [IsRestocked], [RestockApprovalRequired], [Remarks], [ReturnerSignature], [IsReceiverSignature], [ApproverSignature], [OriginalIssueId], [OriginalIssueNo], [ReceiveId], [FromEntityType], [FromEntityId], [ConditionCheckId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'RET-2024-001', CAST(N'2025-11-01T04:56:52.2000000' AS DateTime2), 2, N'1st Ansar Battalion', N'Battalion', N'Surplus', N'Surplus', 5, 2, 3, CAST(1.000 AS Decimal(18, 3)), N'Md. Jahangir Alam', CAST(N'2025-10-31T04:56:52.2000000' AS DateTime2), N'Md. Altaf Hossain', CAST(N'2025-11-01T04:56:52.2000000' AS DateTime2), NULL, N'Abdul Karim', CAST(N'2025-11-01T04:56:52.2000000' AS DateTime2), CAST(N'2025-11-01T04:56:52.2000000' AS DateTime2), NULL, 1, 0, NULL, 1, 1, 1, NULL, NULL, NULL, N'Battalion', 1, NULL, CAST(N'2025-10-31T04:56:52.2000000' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Returns] OFF
GO
SET IDENTITY_INSERT [dbo].[RolePermissions] ON 
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1, N'ViewDashboard', N'Can view dashboard', 1, CAST(N'2025-11-03T20:45:37.7384121' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 100, N'ViewCategory', N'Can view categories', 1, CAST(N'2025-11-03T20:45:37.7837557' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 101, N'CreateCategory', N'Can create categories', 1, CAST(N'2025-11-03T20:45:37.7899071' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 102, N'UpdateCategory', N'Can update categories', 1, CAST(N'2025-11-03T20:45:37.7947045' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 103, N'DeleteCategory', N'Can delete categories', 1, CAST(N'2025-11-03T20:45:37.7988156' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 200, N'ViewSubCategory', N'Can view subcategories', 1, CAST(N'2025-11-03T20:45:37.8042867' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 201, N'CreateSubCategory', N'Can create subcategories', 1, CAST(N'2025-11-03T20:45:37.8081076' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 202, N'UpdateSubCategory', N'Can update subcategories', 1, CAST(N'2025-11-03T20:45:37.8119524' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 203, N'DeleteSubCategory', N'Can delete subcategories', 1, CAST(N'2025-11-03T20:45:37.8159122' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 300, N'ViewBrand', N'Can view brands', 1, CAST(N'2025-11-03T20:45:37.8186307' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 301, N'CreateBrand', N'Can create brands', 1, CAST(N'2025-11-03T20:45:37.8209110' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 302, N'UpdateBrand', N'Can update brands', 1, CAST(N'2025-11-03T20:45:37.8232024' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 303, N'DeleteBrand', N'Can delete brands', 1, CAST(N'2025-11-03T20:45:37.8260272' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 400, N'ViewItem', N'Can view items', 1, CAST(N'2025-11-03T20:45:37.8289759' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 401, N'CreateItem', N'Can create items', 1, CAST(N'2025-11-03T20:45:37.8313770' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 402, N'EditItem', N'Can edit items', 1, CAST(N'2025-11-03T20:45:37.8335010' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 403, N'DeleteItem', N'Can delete items', 1, CAST(N'2025-11-03T20:45:37.8356845' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 404, N'GenerateItemCode', N'Can generate item codes', 1, CAST(N'2025-11-03T20:45:37.8377373' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 405, N'UpdateItem', N'Can update items', 1, CAST(N'2025-11-03T20:45:37.8400345' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 406, N'ViewItemModel', N'Can view item models', 1, CAST(N'2025-11-03T20:45:37.8423857' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (21, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 407, N'CreateItemModel', N'Can create item models', 1, CAST(N'2025-11-03T20:45:37.8450833' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (22, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 408, N'UpdateItemModel', N'Can update item models', 1, CAST(N'2025-11-03T20:45:37.8473739' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (23, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 409, N'DeleteItemModel', N'Can delete item models', 1, CAST(N'2025-11-03T20:45:37.8495057' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (24, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 500, N'ViewStore', N'Can view all stores', 1, CAST(N'2025-11-03T20:45:37.8515866' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (25, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 501, N'CreateStore', N'Can create stores', 1, CAST(N'2025-11-03T20:45:37.8538103' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (26, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 502, N'EditStore', N'Can edit stores', 1, CAST(N'2025-11-03T20:45:37.8559453' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (27, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 503, N'DeleteStore', N'Can delete stores', 1, CAST(N'2025-11-03T20:45:37.8581810' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (28, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 504, N'AssignStoreKeeper', N'Can assign storekeepers', 1, CAST(N'2025-11-03T20:45:37.8608652' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (29, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 505, N'ViewAllStores', N'Can view all stores', 1, CAST(N'2025-11-03T20:45:37.8629948' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (30, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 506, N'ViewOwnStore', N'Can view own assigned store only', 1, CAST(N'2025-11-03T20:45:37.8649320' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (31, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 507, N'UpdateStore', N'Can update stores', 1, CAST(N'2025-11-03T20:45:37.8668375' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (32, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 600, N'ViewVendor', N'Can view vendors', 1, CAST(N'2025-11-03T20:45:37.8688865' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (33, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 601, N'CreateVendor', N'Can create vendors', 1, CAST(N'2025-11-03T20:45:37.8709311' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (34, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 602, N'UpdateVendor', N'Can update vendors', 1, CAST(N'2025-11-03T20:45:37.8731639' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (35, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 603, N'DeleteVendor', N'Can delete vendors', 1, CAST(N'2025-11-03T20:45:37.8754508' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (36, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 610, N'ViewExpiryTracking', N'Can view expiry tracking', 1, CAST(N'2025-11-03T20:45:37.8782102' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (37, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 611, N'CreateExpiryTracking', N'Can create expiry records', 1, CAST(N'2025-11-03T20:45:37.8803585' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (38, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 612, N'ProcessExpiredItems', N'Can process expired items', 1, CAST(N'2025-11-03T20:45:37.8824492' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (39, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 613, N'SendAlerts', N'Can send expiry alerts', 1, CAST(N'2025-11-03T20:45:37.8845572' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (40, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 620, N'ViewCycleCount', N'Can view cycle counts', 1, CAST(N'2025-11-03T20:45:37.8866391' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (41, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 621, N'CreateCycleCount', N'Can create cycle counts', 1, CAST(N'2025-11-03T20:45:37.8887770' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (42, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 622, N'StartCycleCount', N'Can start cycle counts', 1, CAST(N'2025-11-03T20:45:37.8909888' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (43, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 623, N'PerformCycleCount', N'Can perform cycle counts', 1, CAST(N'2025-11-03T20:45:37.8935764' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (44, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 624, N'CompleteCycleCount', N'Can complete cycle counts', 1, CAST(N'2025-11-03T20:45:37.8959599' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (45, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 625, N'ApproveCycleCount', N'Can approve cycle counts', 1, CAST(N'2025-11-03T20:45:37.8982477' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (46, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 630, N'ViewAuditTrail', N'Can view audit trail', 1, CAST(N'2025-11-03T20:45:37.9004262' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (47, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 660, N'ViewTemperatureLogs', N'Can view temperature logs', 1, CAST(N'2025-11-03T20:45:37.9025684' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (48, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 661, N'CreateTemperatureLog', N'Can create temperature logs', 1, CAST(N'2025-11-03T20:45:37.9047765' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (49, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 662, N'ExportTemperatureReport', N'Can export temperature reports', 1, CAST(N'2025-11-03T20:45:37.9069597' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (50, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 700, N'ViewPurchase', N'Can view purchases', 1, CAST(N'2025-11-03T20:45:37.9091127' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (51, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 701, N'CreatePurchase', N'Can create purchases', 1, CAST(N'2025-11-03T20:45:37.9116155' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (52, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 702, N'EditPurchase', N'Can edit purchases', 1, CAST(N'2025-11-03T20:45:37.9138490' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (53, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 703, N'DeletePurchase', N'Can delete purchases', 1, CAST(N'2025-11-03T20:45:37.9160376' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (54, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 704, N'ApprovePurchase', N'Can approve purchases (Level 3 - DDG Admin)', 1, CAST(N'2025-11-03T20:45:37.9183462' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (55, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 705, N'RejectPurchase', N'Can reject purchases', 1, CAST(N'2025-11-03T20:45:37.9219026' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (56, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 706, N'ViewAllPurchases', N'Can view all purchases', 1, CAST(N'2025-11-03T20:45:37.9251092' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (57, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 707, N'CreateMarketplacePurchase', N'Can create marketplace purchases', 1, CAST(N'2025-11-03T20:45:37.9284231' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (58, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 708, N'ReceivePurchase', N'Can receive purchases in store', 1, CAST(N'2025-11-03T20:45:37.9654998' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (59, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 709, N'UpdatePurchase', N'Can update purchases', 1, CAST(N'2025-11-03T20:45:37.9736059' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (60, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 710, N'InspectPurchase', N'Can inspect purchases (Level 2 - AD/DD Store)', 1, CAST(N'2025-11-03T20:45:37.9780732' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (61, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 800, N'ViewIssue', N'Can view issues', 1, CAST(N'2025-11-03T20:45:37.9837825' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (62, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 801, N'CreateIssue', N'Can create issues', 1, CAST(N'2025-11-03T20:45:37.9879980' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (63, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 802, N'EditIssue', N'Can edit issues', 1, CAST(N'2025-11-03T20:45:37.9912276' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (64, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 803, N'DeleteIssue', N'Can delete issues', 1, CAST(N'2025-11-03T20:45:37.9957018' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (65, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 804, N'ApproveIssue', N'Can approve issues', 1, CAST(N'2025-11-03T20:45:38.0290495' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (66, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 805, N'ViewAllIssues', N'Can view all issues', 1, CAST(N'2025-11-03T20:45:38.0324819' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (67, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 806, N'SubmitIssue', N'Can submit issues', 1, CAST(N'2025-11-03T20:45:38.0350024' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (68, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 807, N'ProcessIssue', N'Can process issues', 1, CAST(N'2025-11-03T20:45:38.0373049' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (69, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 808, N'UpdateIssue', N'Can update issues', 1, CAST(N'2025-11-03T20:45:38.0396688' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (70, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 809, N'ProcessPhysicalIssue', N'Can process physical issues', 1, CAST(N'2025-11-03T20:45:38.0421950' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (71, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 810, N'CancelIssue', N'Can cancel issues', 1, CAST(N'2025-11-03T20:45:38.0454058' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (72, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 811, N'ExportIssues', N'Can export issues', 1, CAST(N'2025-11-03T20:45:38.0480201' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (73, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 900, N'ViewReceive', N'Can view receives', 1, CAST(N'2025-11-03T20:45:38.0504211' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (74, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 901, N'CreateReceive', N'Can create receives', 1, CAST(N'2025-11-03T20:45:38.0527744' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (75, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 902, N'EditReceive', N'Can edit receives', 1, CAST(N'2025-11-03T20:45:38.0551349' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (76, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 903, N'DeleteReceive', N'Can delete receives', 1, CAST(N'2025-11-03T20:45:38.0575104' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (77, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 904, N'ReceiveTransfer', N'Can receive transfers', 1, CAST(N'2025-11-03T20:45:38.0605344' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (78, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1000, N'ViewTransfer', N'Can view transfers', 1, CAST(N'2025-11-03T20:45:38.0633391' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (79, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1001, N'CreateTransfer', N'Can create transfers', 1, CAST(N'2025-11-03T20:45:38.0659198' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (80, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1002, N'EditTransfer', N'Can edit transfers', 1, CAST(N'2025-11-03T20:45:38.0683859' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (81, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1003, N'DeleteTransfer', N'Can delete transfers', 1, CAST(N'2025-11-03T20:45:38.0707886' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (82, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1004, N'ApproveTransfer', N'Can approve transfers', 1, CAST(N'2025-11-03T20:45:38.0731934' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (83, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1005, N'ProcessTransfer', N'Can process transfers', 1, CAST(N'2025-11-03T20:45:38.0755912' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (84, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1100, N'ViewDamage', N'Can view damage reports', 1, CAST(N'2025-11-03T20:45:38.0782166' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (85, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1101, N'CreateDamage', N'Can create damage reports', 1, CAST(N'2025-11-03T20:45:38.0802788' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (86, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1102, N'UpdateDamage', N'Can update damage reports', 1, CAST(N'2025-11-03T20:45:38.0822660' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (87, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1103, N'DeleteDamage', N'Can delete damage reports', 1, CAST(N'2025-11-03T20:45:38.0843637' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (88, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1104, N'ApproveDamage', N'Can approve damage reports', 1, CAST(N'2025-11-03T20:45:38.0864533' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (89, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1200, N'ViewWriteOff', N'Can view write-offs', 1, CAST(N'2025-11-03T20:45:38.0884357' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (90, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1201, N'CreateWriteOff', N'Can create write-offs', 1, CAST(N'2025-11-03T20:45:38.0906406' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (91, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1202, N'UpdateWriteOff', N'Can update write-offs', 1, CAST(N'2025-11-03T20:45:38.0928753' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (92, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1203, N'DeleteWriteOff', N'Can delete write-offs', 1, CAST(N'2025-11-03T20:45:38.0953364' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (93, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1204, N'ApproveWriteOff', N'Can approve write-offs', 1, CAST(N'2025-11-03T20:45:38.0975438' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (94, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1205, N'ExecuteWriteOff', N'Can execute write-offs', 1, CAST(N'2025-11-03T20:45:38.0995933' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (95, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1206, N'WriteOffStock', N'Can write-off stock', 1, CAST(N'2025-11-03T20:45:38.1015948' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (96, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1300, N'ViewReturn', N'Can view returns', 1, CAST(N'2025-11-03T20:45:38.1035635' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (97, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1301, N'CreateReturn', N'Can create returns', 1, CAST(N'2025-11-03T20:45:38.1056627' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (98, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1302, N'EditReturn', N'Can edit returns', 1, CAST(N'2025-11-03T20:45:38.1082055' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (99, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1303, N'DeleteReturn', N'Can delete returns', 1, CAST(N'2025-11-03T20:45:38.1109113' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (100, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1304, N'ApproveReturn', N'Can approve returns', 1, CAST(N'2025-11-03T20:45:38.1133741' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (101, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1305, N'UpdateReturn', N'Can update returns', 1, CAST(N'2025-11-03T20:45:38.1156233' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (102, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1400, N'ViewBarcode', N'Can view barcodes', 1, CAST(N'2025-11-03T20:45:38.1177051' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (103, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1401, N'GenerateBarcode', N'Can generate barcodes', 1, CAST(N'2025-11-03T20:45:38.1198103' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (104, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1402, N'PrintBarcode', N'Can print barcodes', 1, CAST(N'2025-11-03T20:45:38.1533335' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (105, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1403, N'ExportBarcode', N'Can export barcodes', 1, CAST(N'2025-11-03T20:45:38.1592407' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (106, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1500, N'ViewStockReport', N'Can view stock reports', 1, CAST(N'2025-11-03T20:45:38.1648723' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (107, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1501, N'ViewPurchaseReport', N'Can view purchase reports', 1, CAST(N'2025-11-03T20:45:38.1692948' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (108, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1502, N'ViewIssueReport', N'Can view issue reports', 1, CAST(N'2025-11-03T20:45:38.1737585' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (109, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1503, N'ViewTransferReport', N'Can view transfer reports', 1, CAST(N'2025-11-03T20:45:38.1778223' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (110, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1504, N'ViewDamageReport', N'Can view damage reports', 1, CAST(N'2025-11-03T20:45:38.1809268' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (111, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1505, N'ViewWriteOffReport', N'Can view write-off reports', 1, CAST(N'2025-11-03T20:45:38.1830843' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (112, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1506, N'ViewReturnReport', N'Can view return reports', 1, CAST(N'2025-11-03T20:45:38.1853949' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (113, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1507, N'ViewInventoryMovementReport', N'Can view inventory movement reports', 1, CAST(N'2025-11-03T20:45:38.1876579' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (114, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1508, N'ExportReports', N'Can export reports', 1, CAST(N'2025-11-03T20:45:38.1898311' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (115, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1509, N'ViewAllReports', N'Can view all reports', 1, CAST(N'2025-11-03T20:45:38.1919316' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (116, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1510, N'ViewReports', N'Can view reports', 1, CAST(N'2025-11-03T20:45:38.1945347' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (117, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1600, N'ViewUsers', N'Can view all users', 1, CAST(N'2025-11-03T20:45:38.1967179' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (118, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1601, N'ViewUser', N'Can view user details', 1, CAST(N'2025-11-03T20:45:38.1989303' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (119, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1602, N'CreateUser', N'Can create users', 1, CAST(N'2025-11-03T20:45:38.2011066' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (120, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1603, N'EditUser', N'Can edit users', 1, CAST(N'2025-11-03T20:45:38.2033646' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (121, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1604, N'DeleteUser', N'Can delete users', 1, CAST(N'2025-11-03T20:45:38.2055253' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (122, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1605, N'AssignRoles', N'Can assign roles to users', 1, CAST(N'2025-11-03T20:45:38.2076196' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (123, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1606, N'AssignRole', N'Can assign role', 1, CAST(N'2025-11-03T20:45:38.2097665' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (124, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1607, N'ResetPassword', N'Can reset user passwords', 1, CAST(N'2025-11-03T20:45:38.2125024' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (125, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1608, N'UpdateUser', N'Can update users', 1, CAST(N'2025-11-03T20:45:38.2147099' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (126, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1700, N'ViewRole', N'Can view roles', 1, CAST(N'2025-11-03T20:45:38.2168447' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (127, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1701, N'CreateRole', N'Can create roles', 1, CAST(N'2025-11-03T20:45:38.2191248' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (128, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1702, N'UpdateRole', N'Can update roles', 1, CAST(N'2025-11-03T20:45:38.2211917' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (129, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1703, N'DeleteRole', N'Can delete roles', 1, CAST(N'2025-11-03T20:45:38.2238877' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (130, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1704, N'AssignPermission', N'Can assign permissions to roles', 1, CAST(N'2025-11-03T20:45:38.2262447' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (131, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1800, N'ViewSettings', N'Can view settings', 1, CAST(N'2025-11-03T20:45:38.2288744' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (132, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1801, N'UpdateSettings', N'Can update settings', 1, CAST(N'2025-11-03T20:45:38.2309598' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (133, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1802, N'ViewSystemInfo', N'Can view system info', 1, CAST(N'2025-11-03T20:45:38.2330920' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (134, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1803, N'CreateBackup', N'Can create backups', 1, CAST(N'2025-11-03T20:45:38.2359011' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (135, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1804, N'RestoreBackup', N'Can restore backups', 1, CAST(N'2025-11-03T20:45:38.2387814' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (136, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1805, N'SystemConfiguration', N'Can configure system', 1, CAST(N'2025-11-03T20:45:38.2415834' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (137, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1900, N'ViewAuditLog', N'Can view audit logs', 1, CAST(N'2025-11-03T20:45:38.2456003' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (138, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1901, N'ViewActivityLog', N'Can view activity logs', 1, CAST(N'2025-11-03T20:45:38.2827874' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (139, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1902, N'ViewLoginLog', N'Can view login logs', 1, CAST(N'2025-11-03T20:45:38.3219456' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (140, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 1903, N'ExportAuditLog', N'Can export audit logs', 1, CAST(N'2025-11-03T20:45:38.3250811' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (141, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2000, N'ViewNotifications', N'Can view notifications', 1, CAST(N'2025-11-03T20:45:38.3281115' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (142, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2001, N'CreateNotification', N'Can create notifications', 1, CAST(N'2025-11-03T20:45:38.3303470' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (143, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2002, N'SendNotification', N'Can send notifications', 1, CAST(N'2025-11-03T20:45:38.3323750' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (144, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2003, N'ManageNotifications', N'Can manage notifications', 1, CAST(N'2025-11-03T20:45:38.3344470' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (145, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2004, N'DeleteNotifications', N'Can delete notifications', 1, CAST(N'2025-11-03T20:45:38.3364389' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (146, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2100, N'ViewStockAdjustment', N'Can view stock adjustments', 1, CAST(N'2025-11-03T20:45:38.3384463' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (147, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2101, N'CreateStockAdjustment', N'Can create stock adjustments', 1, CAST(N'2025-11-03T20:45:38.3406246' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (148, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2102, N'ApproveStockAdjustment', N'Can approve stock adjustments', 1, CAST(N'2025-11-03T20:45:38.3427433' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (149, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2103, N'AdjustStock', N'Can adjust stock', 1, CAST(N'2025-11-03T20:45:38.3451739' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (150, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2104, N'CreateAdjustment', N'Can create adjustments', 1, CAST(N'2025-11-03T20:45:38.3472363' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (151, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2200, N'ViewAllBattalions', N'Can view all battalions', 1, CAST(N'2025-11-03T20:45:38.3492347' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (152, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2201, N'ViewOwnBattalion', N'Can view own battalion only', 1, CAST(N'2025-11-03T20:45:38.3512014' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (153, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2202, N'ViewAllRanges', N'Can view all ranges', 1, CAST(N'2025-11-03T20:45:38.3531856' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (154, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2203, N'ViewOwnRange', N'Can view own range only', 1, CAST(N'2025-11-03T20:45:38.3552193' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (155, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2204, N'CrossBattalionTransfer', N'Can transfer across battalions', 1, CAST(N'2025-11-03T20:45:38.3582880' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (156, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2205, N'CrossRangeTransfer', N'Can transfer across ranges', 1, CAST(N'2025-11-03T20:45:38.3618843' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (157, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2206, N'EmergencyOverride', N'Can override in emergencies', 1, CAST(N'2025-11-03T20:45:38.3644642' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (158, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2300, N'PerformQualityCheck', N'Can perform quality checks', 1, CAST(N'2025-11-03T20:45:38.3666048' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (159, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2301, N'ApproveQualityCheck', N'Can approve quality checks', 1, CAST(N'2025-11-03T20:45:38.3686547' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (160, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 2302, N'QualityCheck', N'Can perform quality checks', 1, CAST(N'2025-11-03T20:45:38.3705854' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (161, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3000, N'ViewStock', N'Can view stock', 1, CAST(N'2025-11-03T20:45:38.3728101' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (162, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3001, N'CreateStock', N'Can create stock entries', 1, CAST(N'2025-11-03T20:45:38.3749694' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (163, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3002, N'EditStock', N'Can edit stock', 1, CAST(N'2025-11-03T20:45:38.3776825' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (164, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3003, N'DeleteStock', N'Can delete stock', 1, CAST(N'2025-11-03T20:45:38.3798817' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (165, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3004, N'ApproveStock', N'Can approve stock entries', 1, CAST(N'2025-11-03T20:45:38.3819937' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (166, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3005, N'ExportStock', N'Can export stock data', 1, CAST(N'2025-11-03T20:45:38.3840453' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (167, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3006, N'ViewApproval', N'Can view approvals', 1, CAST(N'2025-11-03T20:45:38.3862086' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (168, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3007, N'ProcessApproval', N'Can process approvals', 1, CAST(N'2025-11-03T20:45:38.3883839' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (169, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3008, N'CreateEmergencyRequest', N'Can create emergency requests', 1, CAST(N'2025-11-03T20:45:38.3904110' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (170, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3009, N'ViewInventory', N'Can view inventory', 1, CAST(N'2025-11-03T20:45:38.3925602' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (171, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3010, N'CreateInventory', N'Can create inventory', 1, CAST(N'2025-11-03T20:45:38.3951198' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (172, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3011, N'PerformInventoryCount', N'Can perform inventory counts', 1, CAST(N'2025-11-03T20:45:38.3971393' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (173, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3012, N'ExportData', N'Can export data', 1, CAST(N'2025-11-03T20:45:38.3990487' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (174, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3013, N'ImportData', N'Can import data', 1, CAST(N'2025-11-03T20:45:38.4010064' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (175, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3014, N'BulkUpload', N'Can bulk upload data', 1, CAST(N'2025-11-03T20:45:38.4030637' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (176, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3015, N'ViewPhysicalInventory', N'Can view physical inventory', 1, CAST(N'2025-11-03T20:45:38.4053030' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (177, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3016, N'CreatePhysicalInventory', N'Can create physical inventory', 1, CAST(N'2025-11-03T20:45:38.4072258' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (178, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3017, N'UpdatePhysicalInventory', N'Can update physical inventory', 1, CAST(N'2025-11-03T20:45:38.4094110' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (179, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3018, N'ApprovePhysicalInventory', N'Can approve physical inventory', 1, CAST(N'2025-11-03T20:45:38.4119950' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (180, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3019, N'CancelPhysicalInventory', N'Can cancel physical inventory', 1, CAST(N'2025-11-03T20:45:38.4140757' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (181, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3020, N'ExportPhysicalInventory', N'Can export physical inventory', 1, CAST(N'2025-11-03T20:45:38.4160453' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (182, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3021, N'ReviewPhysicalInventory', N'Can review physical inventory', 1, CAST(N'2025-11-03T20:45:38.4179967' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (183, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3033, N'ViewReconciliation', N'Can view reconciliation', 1, CAST(N'2025-11-03T20:45:38.4199373' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (184, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3034, N'CreateReconciliation', N'Can create reconciliation', 1, CAST(N'2025-11-03T20:45:38.4218006' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (185, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3035, N'ProcessReconciliation', N'Can process reconciliation', 1, CAST(N'2025-11-03T20:45:38.4237874' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (186, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3036, N'CompleteReconciliation', N'Can complete reconciliation', 1, CAST(N'2025-11-03T20:45:38.4258586' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (187, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3037, N'ApproveReconciliation', N'Can approve reconciliation', 1, CAST(N'2025-11-03T20:45:38.4282424' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (188, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3039, N'ViewStockMovement', N'Can view stock movement', 1, CAST(N'2025-11-03T20:45:38.4302576' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (189, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3040, N'ExportStockMovement', N'Can export stock movement', 1, CAST(N'2025-11-03T20:45:38.4321447' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (190, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3041, N'UpdateStock', N'Can update stock', 1, CAST(N'2025-11-03T20:45:38.4342884' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (191, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3100, N'ViewAllotmentLetter', N'Can view allotment letters', 1, CAST(N'2025-11-03T20:45:38.4366101' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (192, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3101, N'CreateAllotmentLetter', N'Can create allotment letters', 1, CAST(N'2025-11-03T20:45:38.4388087' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (193, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3102, N'ApproveAllotmentLetter', N'Can approve allotment letters (DD Provision)', 1, CAST(N'2025-11-03T20:45:38.4408913' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (194, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3103, N'UpdateAllotmentLetter', N'Can update allotment letters', 1, CAST(N'2025-11-03T20:45:38.4435794' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (195, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3104, N'DeleteAllotmentLetter', N'Can delete allotment letters', 1, CAST(N'2025-11-03T20:45:38.4459248' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (196, N'd8891b59-b8e2-4af1-9d90-02e0545c4149', N'Admin', 3105, N'EditAllotmentLetter', N'Can edit draft allotment letters', 1, CAST(N'2025-11-03T20:45:38.4482509' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (197, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1, N'ViewDashboard', N'Can view dashboard', 1, CAST(N'2025-11-03T20:45:38.7167882' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (198, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 100, N'ViewCategory', N'Can view categories', 1, CAST(N'2025-11-03T20:45:38.7193327' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (199, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 200, N'ViewSubCategory', N'Can view subcategories', 1, CAST(N'2025-11-03T20:45:38.7215457' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (200, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 300, N'ViewBrand', N'Can view brands', 1, CAST(N'2025-11-03T20:45:38.7235100' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (201, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 400, N'ViewItem', N'Can view items', 1, CAST(N'2025-11-03T20:45:38.7257227' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (202, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 401, N'CreateItem', N'Can create items', 1, CAST(N'2025-11-03T20:45:38.7284278' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (203, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 405, N'UpdateItem', N'Can update items', 1, CAST(N'2025-11-03T20:45:38.7309012' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (204, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 500, N'ViewStore', N'Can view all stores', 1, CAST(N'2025-11-03T20:45:38.7336804' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (205, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 506, N'ViewOwnStore', N'Can view own assigned store only', 1, CAST(N'2025-11-03T20:45:38.7363882' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (206, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 600, N'ViewVendor', N'Can view vendors', 1, CAST(N'2025-11-03T20:45:38.7386469' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (207, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 601, N'CreateVendor', N'Can create vendors', 1, CAST(N'2025-11-03T20:45:38.7408881' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (208, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 700, N'ViewPurchase', N'Can view purchases', 1, CAST(N'2025-11-03T20:45:38.7437543' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (209, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 701, N'CreatePurchase', N'Can create purchases', 1, CAST(N'2025-11-03T20:45:38.7462066' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (210, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 800, N'ViewIssue', N'Can view issues', 1, CAST(N'2025-11-03T20:45:38.7486408' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (211, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 801, N'CreateIssue', N'Can create issues', 1, CAST(N'2025-11-03T20:45:38.7509821' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (212, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1000, N'ViewTransfer', N'Can view transfers', 1, CAST(N'2025-11-03T20:45:38.7531010' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (213, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1001, N'CreateTransfer', N'Can create transfers', 1, CAST(N'2025-11-03T20:45:38.7551607' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (214, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1100, N'ViewDamage', N'Can view damage reports', 1, CAST(N'2025-11-03T20:45:38.7572519' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (215, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1101, N'CreateDamage', N'Can create damage reports', 1, CAST(N'2025-11-03T20:45:38.7593409' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (216, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1200, N'ViewWriteOff', N'Can view write-offs', 1, CAST(N'2025-11-03T20:45:38.7619635' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (217, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1201, N'CreateWriteOff', N'Can create write-offs', 1, CAST(N'2025-11-03T20:45:38.7642408' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (218, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1300, N'ViewReturn', N'Can view returns', 1, CAST(N'2025-11-03T20:45:38.7665619' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (219, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1301, N'CreateReturn', N'Can create returns', 1, CAST(N'2025-11-03T20:45:38.7691187' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (220, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1400, N'ViewBarcode', N'Can view barcodes', 1, CAST(N'2025-11-03T20:45:38.7712432' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (221, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1401, N'GenerateBarcode', N'Can generate barcodes', 1, CAST(N'2025-11-03T20:45:38.7733833' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (222, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1402, N'PrintBarcode', N'Can print barcodes', 1, CAST(N'2025-11-03T20:45:38.7757349' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (223, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1500, N'ViewStockReport', N'Can view stock reports', 1, CAST(N'2025-11-03T20:45:38.7784410' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (224, N'b7dd8637-e0fb-45e0-9ffa-4cc1b2c98ba8', N'StoreManager', 1508, N'ExportReports', N'Can export reports', 1, CAST(N'2025-11-03T20:45:38.7808548' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (225, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 1, N'ViewDashboard', N'Can view dashboard', 1, CAST(N'2025-11-03T20:45:38.8312865' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (226, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 400, N'ViewItem', N'Can view items', 1, CAST(N'2025-11-03T20:45:38.8340447' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (227, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 500, N'ViewStore', N'Can view all stores', 1, CAST(N'2025-11-03T20:45:38.8362266' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (228, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 800, N'ViewIssue', N'Can view issues', 1, CAST(N'2025-11-03T20:45:38.8382492' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (229, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 801, N'CreateIssue', N'Can create issues', 1, CAST(N'2025-11-03T20:45:38.8402474' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (230, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 900, N'ViewReceive', N'Can view receives', 1, CAST(N'2025-11-03T20:45:38.8426065' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (231, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 901, N'CreateReceive', N'Can create receives', 1, CAST(N'2025-11-03T20:45:38.8452492' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (232, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 1400, N'ViewBarcode', N'Can view barcodes', 1, CAST(N'2025-11-03T20:45:38.8473183' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (233, N'fa6b3c8e-2dfd-477a-a1ee-0cff9b6393d9', N'Operator', 1402, N'PrintBarcode', N'Can print barcodes', 1, CAST(N'2025-11-03T20:45:38.8492758' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (234, N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', N'Auditor', 1, N'ViewDashboard', N'Can view dashboard', 1, CAST(N'2025-11-03T20:45:38.8673097' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (235, N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', N'Auditor', 1509, N'ViewAllReports', N'Can view all reports', 1, CAST(N'2025-11-03T20:45:38.8693417' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (236, N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', N'Auditor', 1508, N'ExportReports', N'Can export reports', 1, CAST(N'2025-11-03T20:45:38.8713442' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (237, N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', N'Auditor', 1900, N'ViewAuditLog', N'Can view audit logs', 1, CAST(N'2025-11-03T20:45:38.8733111' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (238, N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', N'Auditor', 1901, N'ViewActivityLog', N'Can view activity logs', 1, CAST(N'2025-11-03T20:45:38.8753945' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (239, N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', N'Auditor', 505, N'ViewAllStores', N'Can view all stores', 1, CAST(N'2025-11-03T20:45:38.8777076' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (240, N'a1e92fab-a21f-4051-a5da-ebe72fb1e956', N'Auditor', 2200, N'ViewAllBattalions', N'Can view all battalions', 1, CAST(N'2025-11-03T20:45:38.8797474' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (241, N'bc59ea92-5958-4e9e-8954-cb5966eba9ea', N'Viewer', 1, N'ViewDashboard', N'Can view dashboard', 1, CAST(N'2025-11-03T20:45:38.8986780' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (242, N'bc59ea92-5958-4e9e-8954-cb5966eba9ea', N'Viewer', 400, N'ViewItem', N'Can view items', 1, CAST(N'2025-11-03T20:45:38.9012853' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (243, N'bc59ea92-5958-4e9e-8954-cb5966eba9ea', N'Viewer', 500, N'ViewStore', N'Can view all stores', 1, CAST(N'2025-11-03T20:45:38.9034978' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[RolePermissions] ([Id], [RoleId], [RoleName], [Permission], [PermissionName], [Description], [IsGranted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (244, N'bc59ea92-5958-4e9e-8954-cb5966eba9ea', N'Viewer', 1500, N'ViewStockReport', N'Can view stock reports', 1, CAST(N'2025-11-03T20:45:38.9054601' AS DateTime2), N'System', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[RolePermissions] OFF
GO
SET IDENTITY_INSERT [dbo].[Settings] ON 
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'SystemName', N'ANSAR VDP IMS', N'System Name', N'General', N'string', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'OrganizationName', N'Bangladesh Ansar & VDP', N'Organization Full Name', N'General', N'string', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'DefaultCurrency', N'BDT', N'Default Currency', N'Financial', N'string', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'LowStockThreshold', N'10', N'Low stock alert threshold percentage', N'Inventory', N'int', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'RequireApprovalAbove', N'10000', N'Require approval for transactions above this amount', N'Workflow', N'decimal', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'EnableEmailNotifications', N'true', N'Enable email notifications', N'Notifications', N'boolean', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'DefaultPageSize', N'20', N'Default page size for lists', N'UI', N'int', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'SessionTimeout', N'30', N'Session timeout in minutes', N'Security', N'int', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'MaxFileUploadSize', N'10485760', N'Maximum file upload size in bytes (10MB)', N'System', N'int', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Settings] ([Id], [Key], [Value], [Description], [Category], [DataType], [IsReadOnly], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'BarcodePrefix', N'AVDP', N'Barcode prefix for item codes', N'Inventory', N'string', 0, CAST(N'2025-11-04T05:07:58.0400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Settings] OFF
GO
SET IDENTITY_INSERT [dbo].[SignatoryPresets] ON 
GO
INSERT [dbo].[SignatoryPresets] ([Id], [PresetName], [PresetNameBn], [SignatoryName], [SignatoryDesignation], [SignatoryDesignationBn], [SignatoryId], [SignatoryPhone], [SignatoryEmail], [Department], [IsDefault], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'A.B.M. Forhad - Deputy Director (Store)', N'এ.বি.এম. ফরহাদ - উপ-পরিচালক (ভাণ্ডার)', N'A.B.M. Forhad', N'Deputy Director (Store)', N'উপ-পরিচালক (ভাণ্ডার)', N'ANS-DD-001', N'+880-2-9898989', N'dd.store@ansar-vdp.gov.bd', N'Store', 1, 1, CAST(N'2025-11-04T02:45:39.1316590' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[SignatoryPresets] ([Id], [PresetName], [PresetNameBn], [SignatoryName], [SignatoryDesignation], [SignatoryDesignationBn], [SignatoryId], [SignatoryPhone], [SignatoryEmail], [Department], [IsDefault], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Md. Abdul Karim - Deputy Director General (Admin)', N'মোঃ আবদুল করিম - উপ-মহাপরিচালক (প্রশাসন)', N'Md. Abdul Karim', N'Deputy Director General (Admin)', N'উপ-মহাপরিচালক (প্রশাসন)', N'ANS-DDG-002', N'+880-2-9898990', N'ddg.admin@ansar-vdp.gov.bd', N'Admin', 0, 2, CAST(N'2025-11-04T02:45:39.1316615' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[SignatoryPresets] ([Id], [PresetName], [PresetNameBn], [SignatoryName], [SignatoryDesignation], [SignatoryDesignationBn], [SignatoryId], [SignatoryPhone], [SignatoryEmail], [Department], [IsDefault], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Deputy Director (Provision)', N'উপ-পরিচালক (প্রভিশন)', N'Mohammad Hasan', N'Deputy Director (Provision)', N'উপ-পরিচালক (প্রভিশন)', N'ANS-DD-003', N'+880-2-9898991', N'dd.provision@ansar-vdp.gov.bd', N'Provision', 0, 3, CAST(N'2025-11-04T02:45:39.1316617' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[SignatoryPresets] ([Id], [PresetName], [PresetNameBn], [SignatoryName], [SignatoryDesignation], [SignatoryDesignationBn], [SignatoryId], [SignatoryPhone], [SignatoryEmail], [Department], [IsDefault], [DisplayOrder], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'Assistant Director (Store)', N'সহকারী পরিচালক (ভাণ্ডার)', N'Md. Jahangir Alam', N'Assistant Director (Store)', N'সহকারী পরিচালক (ভাণ্ডার)', N'ANS-AD-004', N'+880-2-9898992', N'ad.store@ansar-vdp.gov.bd', N'Store', 0, 4, CAST(N'2025-11-04T02:45:39.1316618' AS DateTime2), N'System', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[SignatoryPresets] OFF
GO
SET IDENTITY_INSERT [dbo].[StockAdjustmentItems] ON 
GO
INSERT [dbo].[StockAdjustmentItems] ([Id], [StockAdjustmentId], [ItemId], [SystemQuantity], [ActualQuantity], [PhysicalQuantity], [AdjustmentQuantity], [AdjustmentValue], [BatchNo], [Reason], [Remarks], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 4, 5, CAST(0.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(-10.000 AS Decimal(18, 3)), CAST(-450000.00 AS Decimal(18, 2)), NULL, N'Physical count mismatch', N'Adjusted after physical inventory count', CAST(N'2025-11-04T05:41:29.7766667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StockAdjustmentItems] OFF
GO
SET IDENTITY_INSERT [dbo].[StockAdjustments] ON 
GO
INSERT [dbo].[StockAdjustments] ([Id], [AdjustmentNo], [AdjustmentDate], [AdjustmentType], [Status], [ItemId], [StoreId], [PhysicalInventoryId], [Quantity], [OldQuantity], [NewQuantity], [AdjustmentQuantity], [TotalValue], [Reason], [Remarks], [ReferenceNumber], [ReferenceDocument], [ApprovalReference], [FiscalYear], [AdjustedBy], [AdjustedDate], [ApprovedBy], [ApprovedDate], [ApprovalRemarks], [IsApproved], [RejectedBy], [RejectedDate], [RejectionReason], [AuditTrailJson], [StoreId1], [ItemId1], [ApprovedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'SA-2024-001', CAST(N'2025-10-23T04:50:17.6700000' AS DateTime2), N'Increase', N'Approved', 7, 2, NULL, CAST(10.000 AS Decimal(18, 3)), CAST(800.000 AS Decimal(18, 3)), CAST(810.000 AS Decimal(18, 3)), NULL, CAST(4500.00 AS Decimal(18, 2)), N'Physical Count Mismatch', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-10-23T04:50:17.6700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StockAdjustments] OFF
GO
SET IDENTITY_INSERT [dbo].[StockEntries] ON 
GO
INSERT [dbo].[StockEntries] ([Id], [EntryNo], [EntryDate], [EntryType], [Status], [Remarks], [StoreId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'SE-2024-001', CAST(N'2025-10-07T03:32:22.6700000' AS DateTime2), N'Purchase', N'Completed', N'Stock entry from Purchase PO-2024-001', 2, CAST(N'2025-10-07T03:32:22.6700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntries] ([Id], [EntryNo], [EntryDate], [EntryType], [Status], [Remarks], [StoreId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'SE-2024-002', CAST(N'2025-10-17T04:07:13.2700000' AS DateTime2), N'Purchase', N'Completed', N'Stock entry from Purchase PO-2024-002', 2, CAST(N'2025-10-17T04:07:13.2700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntries] ([Id], [EntryNo], [EntryDate], [EntryType], [Status], [Remarks], [StoreId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'SE-2024-003', CAST(N'2025-10-22T04:07:13.2700000' AS DateTime2), N'Purchase', N'Completed', N'Stock entry from Purchase PO-2024-003', 2, CAST(N'2025-10-22T04:07:13.2700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntries] ([Id], [EntryNo], [EntryDate], [EntryType], [Status], [Remarks], [StoreId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'SE-2024-004', CAST(N'2025-10-25T04:07:13.2700000' AS DateTime2), N'Purchase', N'Completed', N'Stock entry from Purchase PO-2024-004', 2, CAST(N'2025-10-25T04:07:13.2700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntries] ([Id], [EntryNo], [EntryDate], [EntryType], [Status], [Remarks], [StoreId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'SE202511050001', CAST(N'2025-11-05T00:00:00.0000000' AS DateTime2), N'Direct', N'Draft', NULL, 2, CAST(N'2025-11-05T00:39:04.5040238' AS DateTime2), N'admin', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StockEntries] OFF
GO
SET IDENTITY_INSERT [dbo].[StockEntryItems] ON 
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 1, 5, CAST(10.000 AS Decimal(18, 3)), CAST(45000.00 AS Decimal(18, 2)), NULL, N'BATCH-IT-001-2024', NULL, 0, CAST(N'2025-10-07T03:32:32.0966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 1, 6, CAST(20.000 AS Decimal(18, 3)), CAST(18000.00 AS Decimal(18, 2)), NULL, N'BATCH-IT-002-2024', NULL, 0, CAST(N'2025-10-07T03:32:32.0966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 1, 7, CAST(800.000 AS Decimal(18, 3)), CAST(450.00 AS Decimal(18, 2)), NULL, N'BATCH-ST-001-2024', NULL, 0, CAST(N'2025-10-07T03:32:32.0966667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 2, 8, CAST(2000.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), NULL, N'BATCH-ST-PEN-001-2024', NULL, 0, CAST(N'2025-10-17T04:07:29.0200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 2, 9, CAST(2000.000 AS Decimal(18, 3)), CAST(5.00 AS Decimal(18, 2)), NULL, N'BATCH-ST-PEN-002-2024', NULL, 0, CAST(N'2025-10-17T04:07:29.0200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 2, 10, CAST(1000.000 AS Decimal(18, 3)), CAST(3.00 AS Decimal(18, 2)), NULL, N'BATCH-ST-PENC-001-2024', NULL, 0, CAST(N'2025-10-17T04:07:29.0200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, 2, 11, CAST(500.000 AS Decimal(18, 3)), CAST(45.00 AS Decimal(18, 2)), NULL, N'BATCH-ST-NOTE-001-2024', NULL, 0, CAST(N'2025-10-17T04:07:29.0200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, 2, 12, CAST(300.000 AS Decimal(18, 3)), CAST(25.00 AS Decimal(18, 2)), NULL, N'BATCH-ST-FILE-001-2024', NULL, 0, CAST(N'2025-10-17T04:07:29.0200000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, 3, 13, CAST(200.000 AS Decimal(18, 3)), CAST(850.00 AS Decimal(18, 2)), NULL, N'BATCH-UC-SHIRT-001-2024', NULL, 0, CAST(N'2025-10-22T04:07:40.5700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, 3, 14, CAST(200.000 AS Decimal(18, 3)), CAST(950.00 AS Decimal(18, 2)), NULL, N'BATCH-UC-PANT-001-2024', NULL, 0, CAST(N'2025-10-22T04:07:40.5700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, 3, 15, CAST(100.000 AS Decimal(18, 3)), CAST(750.00 AS Decimal(18, 2)), NULL, N'BATCH-UC-SHIRT-002-2024', NULL, 0, CAST(N'2025-10-22T04:07:40.5700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, 3, 16, CAST(100.000 AS Decimal(18, 3)), CAST(850.00 AS Decimal(18, 2)), NULL, N'BATCH-UC-PANT-002-2024', NULL, 0, CAST(N'2025-10-22T04:07:40.5700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, 3, 17, CAST(150.000 AS Decimal(18, 3)), CAST(350.00 AS Decimal(18, 2)), NULL, N'BATCH-UC-BELT-001-2024', NULL, 0, CAST(N'2025-10-22T04:07:40.5700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, 3, 18, CAST(150.000 AS Decimal(18, 3)), CAST(250.00 AS Decimal(18, 2)), NULL, N'BATCH-UC-CAP-001-2024', NULL, 0, CAST(N'2025-10-22T04:07:40.5700000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, 4, 19, CAST(15.000 AS Decimal(18, 3)), CAST(4500.00 AS Decimal(18, 2)), NULL, N'BATCH-IT-HDD-001-2024', NULL, 0, CAST(N'2025-10-25T04:07:51.7733333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, 4, 20, CAST(50.000 AS Decimal(18, 3)), CAST(650.00 AS Decimal(18, 2)), NULL, N'BATCH-IT-USB-001-2024', NULL, 0, CAST(N'2025-10-25T04:07:51.7733333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, 4, 21, CAST(25.000 AS Decimal(18, 3)), CAST(350.00 AS Decimal(18, 2)), NULL, N'BATCH-IT-CABL-001-2024', NULL, 0, CAST(N'2025-10-25T04:07:51.7733333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, 4, 22, CAST(40.000 AS Decimal(18, 3)), CAST(250.00 AS Decimal(18, 2)), NULL, N'BATCH-IT-CABL-002-2024', NULL, 0, CAST(N'2025-10-25T04:07:51.7733333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, 4, 23, CAST(8.000 AS Decimal(18, 3)), CAST(2500.00 AS Decimal(18, 2)), NULL, N'BATCH-IT-WCAM-001-2024', NULL, 0, CAST(N'2025-10-25T04:07:51.7733333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockEntryItems] ([Id], [StockEntryId], [ItemId], [Quantity], [UnitCost], [Location], [BatchNumber], [ExpiryDate], [BarcodesGenerated], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, 5, 5, CAST(100.000 AS Decimal(18, 3)), CAST(45000.00 AS Decimal(18, 2)), NULL, N'BN202511053453', NULL, 0, CAST(N'2025-11-05T00:39:04.7389583' AS DateTime2), N'admin', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StockEntryItems] OFF
GO
SET IDENTITY_INSERT [dbo].[StockMovements] ON 
GO
INSERT [dbo].[StockMovements] ([Id], [MovementType], [MovementDate], [Reason], [Notes], [Remarks], [ItemId], [StoreId], [SourceStoreId], [DestinationStoreId], [Quantity], [OldBalance], [NewBalance], [UnitPrice], [TotalValue], [ReferenceType], [ReferenceNo], [ReferenceId], [MovedBy], [MovedByUserId], [StoreItemId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'0', CAST(N'2025-10-05T04:57:22.5166667' AS DateTime2), N'Stock Entry from Purchase', NULL, NULL, 5, 2, 2, 2, CAST(10.000 AS Decimal(18, 3)), CAST(0.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(45000.00 AS Decimal(18, 2)), CAST(450000.00 AS Decimal(18, 2)), N'StockEntry', N'SE-2024-001', NULL, N'Abdul Karim', NULL, NULL, CAST(N'2025-10-05T04:57:22.5166667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockMovements] ([Id], [MovementType], [MovementDate], [Reason], [Notes], [Remarks], [ItemId], [StoreId], [SourceStoreId], [DestinationStoreId], [Quantity], [OldBalance], [NewBalance], [UnitPrice], [TotalValue], [ReferenceType], [ReferenceNo], [ReferenceId], [MovedBy], [MovedByUserId], [StoreItemId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'2', CAST(N'2025-10-25T04:57:34.1000000' AS DateTime2), N'Transfer to Provision Store', NULL, NULL, 6, 2, 2, 3, CAST(5.000 AS Decimal(18, 3)), CAST(20.000 AS Decimal(18, 3)), CAST(15.000 AS Decimal(18, 3)), CAST(18000.00 AS Decimal(18, 2)), CAST(90000.00 AS Decimal(18, 2)), N'Transfer', N'TRF-2024-001', NULL, N'Abdul Karim', NULL, NULL, CAST(N'2025-10-25T04:57:34.1000000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[StockMovements] ([Id], [MovementType], [MovementDate], [Reason], [Notes], [Remarks], [ItemId], [StoreId], [SourceStoreId], [DestinationStoreId], [Quantity], [OldBalance], [NewBalance], [UnitPrice], [TotalValue], [ReferenceType], [ReferenceNo], [ReferenceId], [MovedBy], [MovedByUserId], [StoreItemId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'1', CAST(N'2025-10-23T04:57:34.1000000' AS DateTime2), N'Issued to Battalion', NULL, NULL, 6, 3, 3, 3, CAST(2.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(3.000 AS Decimal(18, 3)), CAST(18000.00 AS Decimal(18, 2)), CAST(36000.00 AS Decimal(18, 2)), N'Issue', N'ISS-2024-001', NULL, N'Abdul Karim', NULL, NULL, CAST(N'2025-10-23T04:57:34.1000000' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StockMovements] OFF
GO
SET IDENTITY_INSERT [dbo].[StoreConfigurations] ON 
GO
INSERT [dbo].[StoreConfigurations] ([Id], [StoreId], [ConfigKey], [ConfigValue], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 2, N'AllowDirectIssue', N'false', N'Direct issue without approval not allowed', CAST(N'2025-11-04T05:04:43.8700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreConfigurations] ([Id], [StoreId], [ConfigKey], [ConfigValue], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 2, N'RequireInspection', N'true', N'All goods must be inspected', CAST(N'2025-11-04T05:04:43.8700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreConfigurations] ([Id], [StoreId], [ConfigKey], [ConfigValue], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 2, N'LowStockAlertEnabled', N'true', N'Enable low stock alerts', CAST(N'2025-11-04T05:04:43.8700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreConfigurations] ([Id], [StoreId], [ConfigKey], [ConfigValue], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 2, N'MaxStorageCapacity', N'10000', N'Maximum storage capacity in units', CAST(N'2025-11-04T05:04:43.8700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreConfigurations] ([Id], [StoreId], [ConfigKey], [ConfigValue], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 3, N'AllowDirectIssue', N'true', N'Direct issue allowed with approval', CAST(N'2025-11-04T05:04:43.8700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreConfigurations] ([Id], [StoreId], [ConfigKey], [ConfigValue], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 3, N'RequireInspection', N'false', N'Inspection not required', CAST(N'2025-11-04T05:04:43.8700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreConfigurations] ([Id], [StoreId], [ConfigKey], [ConfigValue], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, 3, N'LowStockAlertEnabled', N'true', N'Enable low stock alerts', CAST(N'2025-11-04T05:04:43.8700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreConfigurations] ([Id], [StoreId], [ConfigKey], [ConfigValue], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, 3, N'MaxStorageCapacity', N'5000', N'Maximum storage capacity in units', CAST(N'2025-11-04T05:04:43.8700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StoreConfigurations] OFF
GO
SET IDENTITY_INSERT [dbo].[StoreItems] ON 
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 5, 2, CAST(25.000 AS Decimal(18, 3)), CAST(25.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(10.000 AS Decimal(18, 3)), CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), N'admin', 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 6, 2, CAST(15.000 AS Decimal(18, 3)), CAST(15.000 AS Decimal(18, 3)), CAST(3.000 AS Decimal(18, 3)), CAST(30.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(20.000 AS Decimal(18, 3)), CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 7, 2, CAST(500.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(N'2025-11-04T03:33:01.0966667' AS DateTime2), CAST(800.000 AS Decimal(18, 3)), CAST(N'2025-10-07T03:33:01.0966667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 8, 2, CAST(2000.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(5000.000 AS Decimal(18, 3)), CAST(750.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(2000.000 AS Decimal(18, 3)), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 9, 2, CAST(2000.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(5000.000 AS Decimal(18, 3)), CAST(750.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(2000.000 AS Decimal(18, 3)), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, 10, 2, CAST(2000.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(3000.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(1000.000 AS Decimal(18, 3)), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, 11, 2, CAST(800.000 AS Decimal(18, 3)), CAST(800.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(500.000 AS Decimal(18, 3)), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, 12, 2, CAST(400.000 AS Decimal(18, 3)), CAST(400.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(N'2025-11-04T04:08:13.6066667' AS DateTime2), CAST(300.000 AS Decimal(18, 3)), CAST(N'2025-10-17T04:08:13.6066667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, 13, 2, CAST(300.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(200.000 AS Decimal(18, 3)), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, 14, 2, CAST(250.000 AS Decimal(18, 3)), CAST(250.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(200.000 AS Decimal(18, 3)), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, 15, 2, CAST(300.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(100.000 AS Decimal(18, 3)), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, 16, 2, CAST(250.000 AS Decimal(18, 3)), CAST(250.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(100.000 AS Decimal(18, 3)), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, 17, 2, CAST(150.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(75.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(150.000 AS Decimal(18, 3)), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, 18, 2, CAST(500.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(75.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(N'2025-11-04T04:08:25.0700000' AS DateTime2), CAST(150.000 AS Decimal(18, 3)), CAST(N'2025-10-22T04:08:25.0700000' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, 19, 2, CAST(50.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(15.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(15.000 AS Decimal(18, 3)), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, 20, 2, CAST(150.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(20.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(30.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(50.000 AS Decimal(18, 3)), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, 21, 2, CAST(200.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(15.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(25.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(25.000 AS Decimal(18, 3)), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, 22, 2, CAST(200.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(30.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(40.000 AS Decimal(18, 3)), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
INSERT [dbo].[StoreItems] ([Id], [ItemId], [StoreId], [Quantity], [CurrentStock], [MinimumStock], [MaximumStock], [ReorderLevel], [ReservedStock], [ReservedQuantity], [Location], [Status], [LastUpdated], [LastStockUpdate], [LastCountDate], [LastIssueDate], [LastReceiveDate], [LastTransferDate], [LastAdjustmentDate], [LastCountQuantity], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, 23, 2, CAST(20.000 AS Decimal(18, 3)), CAST(20.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), NULL, 0, NULL, 0, CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(N'2025-11-04T04:08:34.4666667' AS DateTime2), CAST(8.000 AS Decimal(18, 3)), CAST(N'2025-10-25T04:08:34.4666667' AS DateTime2), NULL, CAST(N'2025-11-05T02:06:02.9766667' AS DateTime2), NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StoreItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Stores] ON 
GO
INSERT [dbo].[Stores] ([Id], [Name], [NameBn], [Code], [Type], [Status], [Location], [Address], [Description], [Remarks], [InCharge], [ContactNumber], [Email], [Phone], [ManagerName], [ManagerId], [StoreKeeperName], [StoreKeeperId], [StoreKeeperContact], [StoreKeeperAssignedDate], [OperatingHours], [Capacity], [TotalCapacity], [UsedCapacity], [AvailableCapacity], [Level], [StoreTypeId], [LocationId], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [RequiresTemperatureControl], [Temperature], [Humidity], [MinTemperature], [MaxTemperature], [SecurityLevel], [AccessRequirements], [IsStockFrozen], [StockFrozenAt], [StockUnfrozenAt], [StockFrozenReason], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Central Store - Headquarters', N'কেন্দ্রীয় ভাণ্ডার - সদর দপ্তর', N'CS-HQ-01', NULL, N'Active', N'Headquarters', N'Ansar & VDP HQ, Dhaka-1000', N'Main Central Store', NULL, N'Md. Altaf Hossain', N'01700-501001', N'central.hq@ansar.gov.bd', NULL, NULL, NULL, N'Abdul Karim', N'SK001', NULL, CAST(N'2025-11-04T03:13:26.2300000' AS DateTime2), NULL, NULL, NULL, NULL, NULL, 1, 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, CAST(N'2025-11-04T03:13:26.2300000' AS DateTime2), NULL, NULL, CAST(N'2025-11-04T03:13:26.2300000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Stores] ([Id], [Name], [NameBn], [Code], [Type], [Status], [Location], [Address], [Description], [Remarks], [InCharge], [ContactNumber], [Email], [Phone], [ManagerName], [ManagerId], [StoreKeeperName], [StoreKeeperId], [StoreKeeperContact], [StoreKeeperAssignedDate], [OperatingHours], [Capacity], [TotalCapacity], [UsedCapacity], [AvailableCapacity], [Level], [StoreTypeId], [LocationId], [BattalionId], [RangeId], [ZilaId], [UpazilaId], [UnionId], [RequiresTemperatureControl], [Temperature], [Humidity], [MinTemperature], [MaxTemperature], [SecurityLevel], [AccessRequirements], [IsStockFrozen], [StockFrozenAt], [StockUnfrozenAt], [StockFrozenReason], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Provision Store - Dhaka', N'বিধান ভাণ্ডার - ঢাকা', N'PS-DH-01', NULL, N'Active', N'Dhaka Range', N'Dhaka Range Office, Mirpur-12', N'Provision Store for Dhaka Range', NULL, N'Md. Shahjahan Ali', N'01700-502001', N'provision.dhaka@ansar.gov.bd', NULL, NULL, NULL, N'Fazlul Haque', N'SK002', NULL, CAST(N'2025-11-04T03:35:43.2800000' AS DateTime2), NULL, NULL, NULL, NULL, NULL, 2, 2, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, CAST(N'2025-11-04T03:35:43.2800000' AS DateTime2), NULL, NULL, CAST(N'2025-11-04T03:35:43.2800000' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Stores] OFF
GO
SET IDENTITY_INSERT [dbo].[StoreStocks] ON 
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 2, 5, CAST(10.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(10.000 AS Decimal(18, 3)), NULL, CAST(45000.00 AS Decimal(18, 2)), CAST(45000.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 2, 6, CAST(20.000 AS Decimal(18, 3)), CAST(3.000 AS Decimal(18, 3)), CAST(30.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(5.000 AS Decimal(18, 3)), NULL, CAST(18000.00 AS Decimal(18, 2)), CAST(18000.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 2, 7, CAST(800.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(300.000 AS Decimal(18, 3)), NULL, CAST(450.00 AS Decimal(18, 2)), CAST(450.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 2, 8, CAST(2000.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(5000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(750.000 AS Decimal(18, 3)), NULL, CAST(5.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 2, 9, CAST(2000.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(5000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(750.000 AS Decimal(18, 3)), NULL, CAST(5.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 2, 10, CAST(1000.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(3000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(500.000 AS Decimal(18, 3)), NULL, CAST(3.00 AS Decimal(18, 2)), CAST(3.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, 2, 11, CAST(500.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(2000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(300.000 AS Decimal(18, 3)), NULL, CAST(45.00 AS Decimal(18, 2)), CAST(45.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, 2, 12, CAST(300.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(150.000 AS Decimal(18, 3)), NULL, CAST(25.00 AS Decimal(18, 2)), CAST(25.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, 2, 13, CAST(200.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(150.000 AS Decimal(18, 3)), NULL, CAST(850.00 AS Decimal(18, 2)), CAST(850.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, 2, 14, CAST(200.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(150.000 AS Decimal(18, 3)), NULL, CAST(950.00 AS Decimal(18, 2)), CAST(950.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, 2, 15, CAST(100.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(150.000 AS Decimal(18, 3)), NULL, CAST(750.00 AS Decimal(18, 2)), CAST(750.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, 2, 16, CAST(100.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(1000.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(150.000 AS Decimal(18, 3)), NULL, CAST(850.00 AS Decimal(18, 2)), CAST(850.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, 2, 17, CAST(150.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(75.000 AS Decimal(18, 3)), NULL, CAST(350.00 AS Decimal(18, 2)), CAST(350.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, 2, 18, CAST(150.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(500.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(75.000 AS Decimal(18, 3)), NULL, CAST(250.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, 2, 19, CAST(15.000 AS Decimal(18, 3)), CAST(10.000 AS Decimal(18, 3)), CAST(100.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(15.000 AS Decimal(18, 3)), NULL, CAST(4500.00 AS Decimal(18, 2)), CAST(4500.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, 2, 20, CAST(50.000 AS Decimal(18, 3)), CAST(20.000 AS Decimal(18, 3)), CAST(200.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(30.000 AS Decimal(18, 3)), NULL, CAST(650.00 AS Decimal(18, 2)), CAST(650.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, 2, 21, CAST(25.000 AS Decimal(18, 3)), CAST(15.000 AS Decimal(18, 3)), CAST(150.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(25.000 AS Decimal(18, 3)), NULL, CAST(350.00 AS Decimal(18, 2)), CAST(350.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, 2, 22, CAST(40.000 AS Decimal(18, 3)), CAST(30.000 AS Decimal(18, 3)), CAST(300.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(50.000 AS Decimal(18, 3)), NULL, CAST(250.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreStocks] ([Id], [StoreId], [ItemId], [Quantity], [MinQuantity], [MaxQuantity], [LastUpdated], [ReorderLevel], [LastUpdatedBy], [LastPurchasePrice], [AveragePrice], [StoreId1], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, 2, 23, CAST(8.000 AS Decimal(18, 3)), CAST(5.000 AS Decimal(18, 3)), CAST(50.000 AS Decimal(18, 3)), CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), CAST(10.000 AS Decimal(18, 3)), NULL, CAST(2500.00 AS Decimal(18, 2)), CAST(2500.00 AS Decimal(18, 2)), NULL, NULL, CAST(N'2025-11-04T05:06:25.0833333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StoreStocks] OFF
GO
SET IDENTITY_INSERT [dbo].[StoreTypeCategories] ON 
GO
INSERT [dbo].[StoreTypeCategories] ([Id], [StoreTypeId], [CategoryId], [IsPrimary], [IsAllowed], [StoreTypeId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 1, 1, 1, 1, NULL, CAST(N'2025-11-04T05:05:26.4200000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreTypeCategories] ([Id], [StoreTypeId], [CategoryId], [IsPrimary], [IsAllowed], [StoreTypeId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 1, 2, 1, 1, NULL, CAST(N'2025-11-04T05:05:26.4200000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreTypeCategories] ([Id], [StoreTypeId], [CategoryId], [IsPrimary], [IsAllowed], [StoreTypeId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 1, 3, 1, 1, NULL, CAST(N'2025-11-04T05:05:26.4200000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreTypeCategories] ([Id], [StoreTypeId], [CategoryId], [IsPrimary], [IsAllowed], [StoreTypeId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 2, 1, 0, 1, NULL, CAST(N'2025-11-04T05:05:26.4200000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreTypeCategories] ([Id], [StoreTypeId], [CategoryId], [IsPrimary], [IsAllowed], [StoreTypeId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, 2, 2, 1, 1, NULL, CAST(N'2025-11-04T05:05:26.4200000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[StoreTypeCategories] ([Id], [StoreTypeId], [CategoryId], [IsPrimary], [IsAllowed], [StoreTypeId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, 2, 3, 1, 1, NULL, CAST(N'2025-11-04T05:05:26.4200000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StoreTypeCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[StoreTypes] ON 
GO
INSERT [dbo].[StoreTypes] ([Id], [Name], [NameBn], [Code], [Description], [Icon], [Color], [DefaultManagerRole], [DisplayOrder], [MaxCapacity], [RequiresTemperatureControl], [RequiresSecurityClearance], [IsMainStore], [AllowDirectIssue], [AllowTransfer], [RequiresMandatoryDocuments], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'Central Store', N'কেন্দ্রীয় স্টোর', N'CENTRAL', N'All items first received here for inspection', N'fa-warehouse', N'#3498db', N'StoreKeeper', 1, 0, 0, 0, 1, 0, 1, 0, CAST(N'2025-11-04T02:45:38.9681398' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[StoreTypes] ([Id], [Name], [NameBn], [Code], [Description], [Icon], [Color], [DefaultManagerRole], [DisplayOrder], [MaxCapacity], [RequiresTemperatureControl], [RequiresSecurityClearance], [IsMainStore], [AllowDirectIssue], [AllowTransfer], [RequiresMandatoryDocuments], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Provision Store', N'প্রভিশন স্টোর', N'PROVISION', N'Items issued from here after inspection - Requires mandatory documents', N'fa-dolly', N'#2ecc71', N'StoreKeeper', 2, 0, 0, 0, 0, 1, 1, 1, CAST(N'2025-11-04T02:45:38.9695302' AS DateTime2), N'System', NULL, NULL, 1)
GO
INSERT [dbo].[StoreTypes] ([Id], [Name], [NameBn], [Code], [Description], [Icon], [Color], [DefaultManagerRole], [DisplayOrder], [MaxCapacity], [RequiresTemperatureControl], [RequiresSecurityClearance], [IsMainStore], [AllowDirectIssue], [AllowTransfer], [RequiresMandatoryDocuments], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Other Store', N'অন্যান্য স্টোর', N'OTHER', N'General purpose stores (Battalion/Range/Zila stores)', N'fa-store', N'#95a5a6', N'StoreKeeper', 3, 0, 0, 0, 0, 1, 1, 0, CAST(N'2025-11-04T02:45:38.9695320' AS DateTime2), N'System', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[StoreTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[SubCategories] ON 
GO
INSERT [dbo].[SubCategories] ([Id], [Name], [NameBn], [Description], [Code], [CategoryId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'Computers', N'কম্পিউটার ও ল্যাপটপ', NULL, N'SC-COMP', 1, CAST(N'2025-11-04T03:14:03.4566667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[SubCategories] ([Id], [Name], [NameBn], [Description], [Code], [CategoryId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Printers', N'প্রিন্টার ও স্ক্যানার', NULL, N'SC-PRNT', 1, CAST(N'2025-11-04T03:14:03.4566667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[SubCategories] ([Id], [Name], [NameBn], [Description], [Code], [CategoryId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Paper', N'কাগজ', NULL, N'SC-PAPR', 2, CAST(N'2025-11-04T03:14:03.4600000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[SubCategories] ([Id], [Name], [NameBn], [Description], [Code], [CategoryId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'Office Supplies', N'অফিস সরবরাহ', NULL, N'SC-SUPP', 2, CAST(N'2025-11-04T04:04:34.8766667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[SubCategories] ([Id], [Name], [NameBn], [Description], [Code], [CategoryId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'Uniforms', N'ইউনিফর্ম', NULL, N'SC-UNIF', 3, CAST(N'2025-11-04T04:04:34.8766667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[SubCategories] ([Id], [Name], [NameBn], [Description], [Code], [CategoryId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'Accessories', N'আনুষাঙ্গিক', NULL, N'SC-ACCE', 1, CAST(N'2025-11-04T04:04:34.8766667' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[SubCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[SupplierEvaluations] ON 
GO
INSERT [dbo].[SupplierEvaluations] ([Id], [VendorId], [EvaluationDate], [EvaluatedBy], [QualityRating], [DeliveryRating], [PriceRating], [ServiceRating], [OverallRating], [Comments], [Recommendations], [IsApproved], [ApprovedDate], [ApprovedBy], [EvaluatedByUserId], [ApprovedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 1, CAST(N'2025-08-04T07:02:13.5466667' AS DateTime2), N'Md. Altaf Hossain', CAST(4.50 AS Decimal(3, 2)), CAST(4.80 AS Decimal(3, 2)), CAST(4.20 AS Decimal(3, 2)), CAST(4.60 AS Decimal(3, 2)), CAST(4.50 AS Decimal(3, 2)), N'Excellent quality IT equipment. Timely delivery. Good after-sales support.', N'Continue partnership. Consider for future tenders.', 1, CAST(N'2025-08-04T07:02:13.5466667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, CAST(N'2025-08-04T07:02:13.5466667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[SupplierEvaluations] ([Id], [VendorId], [EvaluationDate], [EvaluatedBy], [QualityRating], [DeliveryRating], [PriceRating], [ServiceRating], [OverallRating], [Comments], [Recommendations], [IsApproved], [ApprovedDate], [ApprovedBy], [EvaluatedByUserId], [ApprovedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 1, CAST(N'2025-05-04T07:02:13.5466667' AS DateTime2), N'Md. Altaf Hossain', CAST(4.30 AS Decimal(3, 2)), CAST(4.50 AS Decimal(3, 2)), CAST(4.00 AS Decimal(3, 2)), CAST(4.40 AS Decimal(3, 2)), CAST(4.30 AS Decimal(3, 2)), N'Good quality products. Minor delay in one delivery but resolved quickly.', N'Reliable supplier. Recommend for IT equipment.', 1, CAST(N'2025-05-04T07:02:13.5466667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, CAST(N'2025-05-04T07:02:13.5466667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[SupplierEvaluations] ([Id], [VendorId], [EvaluationDate], [EvaluatedBy], [QualityRating], [DeliveryRating], [PriceRating], [ServiceRating], [OverallRating], [Comments], [Recommendations], [IsApproved], [ApprovedDate], [ApprovedBy], [EvaluatedByUserId], [ApprovedByUserId], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 1, CAST(N'2025-02-04T07:02:13.5466667' AS DateTime2), N'Md. Altaf Hossain', CAST(4.70 AS Decimal(3, 2)), CAST(4.90 AS Decimal(3, 2)), CAST(4.30 AS Decimal(3, 2)), CAST(4.80 AS Decimal(3, 2)), CAST(4.70 AS Decimal(3, 2)), N'Outstanding performance. Early delivery. Technical support very responsive.', N'Excellent vendor. Highly recommended.', 1, CAST(N'2025-02-04T07:02:13.5466667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, CAST(N'2025-02-04T07:02:13.5466667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[SupplierEvaluations] OFF
GO
SET IDENTITY_INSERT [dbo].[TransferItems] ON 
GO
INSERT [dbo].[TransferItems] ([Id], [TransferId], [ItemId], [Quantity], [RequestedQuantity], [ApprovedQuantity], [ShippedQuantity], [ReceivedQuantity], [UnitPrice], [TotalValue], [ItemCondition], [BatchNo], [Remarks], [ReceivedCondition], [ReceivedDate], [ShippedDate], [PackageCount], [PackageDetails], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 4, 7, CAST(100.000 AS Decimal(18, 3)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-11-02T03:37:16.5933333' AS DateTime2), CAST(N'2025-11-01T03:37:16.5933333' AS DateTime2), 5, NULL, CAST(N'2025-11-01T03:37:16.5933333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[TransferItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Transfers] ON 
GO
INSERT [dbo].[Transfers] ([Id], [TransferNo], [TransferDate], [Status], [FromStoreId], [ToStoreId], [FromBattalionId], [FromRangeId], [FromZilaId], [ToBattalionId], [ToRangeId], [ToZilaId], [TransferType], [Purpose], [Remarks], [RequestedBy], [RequestedDate], [ApprovedBy], [ApprovedDate], [TransferredBy], [ShippedBy], [ShippedDate], [ShipmentNo], [ShippingQRCode], [ReceivedBy], [ReceivedDate], [ReceiverSignature], [ReceiptRemarks], [EstimatedDeliveryDate], [TransportMode], [VehicleNo], [DriverName], [DriverContact], [TotalValue], [HasDiscrepancy], [SenderSignature], [IsReceiverSignature], [ApproverSignature], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'TRF-2024-001', CAST(N'2025-11-01T03:36:22.2566667' AS DateTime2), N'Completed', 2, 3, NULL, NULL, NULL, NULL, NULL, NULL, N'Store to Store', N'Transfer to Provision Store', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 1, 0, 1, CAST(N'2025-11-01T03:36:22.2566667' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Transfers] OFF
GO
SET IDENTITY_INSERT [dbo].[Unions] ON 
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Mirpur Union', N'UN-MIR-01', NULL, 1, N'Md. Selim Reza', N'01711-444001', NULL, NULL, N'Md. Hanif Khan', N'01722-555001', N'mirpur.union@vdp.gov.bd', NULL, 9, 12, NULL, NULL, 45000, NULL, 1, 50, 30, 40, NULL, NULL, 0, 0, NULL, N'মিরপুর ইউনিয়ন', CAST(N'2025-11-04T05:07:20.6933333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Pallabi Union', N'UN-PAL-01', NULL, 1, N'Mrs. Rahima Begum', N'01733-666001', NULL, NULL, N'Md. Kamal Uddin', N'01744-777001', N'pallabi.union@vdp.gov.bd', NULL, 9, 10, NULL, NULL, 38000, NULL, 1, 45, 35, 35, NULL, NULL, 0, 0, NULL, N'পল্লবী ইউনিয়ন', CAST(N'2025-11-04T05:07:20.6933333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'Dhamsona Union', N'UN-DHA-01', NULL, 3, N'Md. Habibur Rahman', N'01711-501001', NULL, NULL, N'Md. Ashraful Alam', N'01722-601001', N'dhamsona.union@vdp.gov.bd', NULL, 9, 15, NULL, NULL, 32000, NULL, 1, 60, 40, 50, NULL, NULL, 1, 0, NULL, N'ধামসোনা ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'Tetuljhora Union', N'UN-TET-01', NULL, 3, N'Mrs. Rahima Khatun', N'01733-701001', NULL, NULL, N'Md. Jahidul Islam', N'01744-801001', N'tetuljhora.union@vdp.gov.bd', NULL, 9, 14, NULL, NULL, 28000, NULL, 1, 55, 45, 48, NULL, NULL, 1, 0, NULL, N'তেঁতুলঝোড়া ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'Ashulia Union', N'UN-ASH-01', NULL, 3, N'Md. Shahjahan Ali', N'01755-901001', NULL, NULL, N'Md. Kamrul Hasan', N'01766-001001', N'ashulia.union@vdp.gov.bd', NULL, 9, 18, NULL, NULL, 52000, NULL, 1, 70, 50, 60, NULL, NULL, 0, 0, NULL, N'আশুলিয়া ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'Baipayl Union', N'UN-BAI-01', NULL, 4, N'Md. Abdul Mannan', N'01777-101001', NULL, NULL, N'Md. Shafiqul Islam', N'01788-201001', N'baipayl.union@vdp.gov.bd', NULL, 9, 11, NULL, NULL, 26000, NULL, 1, 52, 38, 45, NULL, NULL, 1, 0, NULL, N'বাইপাইল ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'Kalindi Union', N'UN-KAL-01', NULL, 5, N'Md. Monirul Haque', N'01799-301001', NULL, NULL, N'Md. Hafizur Rahman', N'01711-401001', N'kalindi.union@vdp.gov.bd', NULL, 9, 13, NULL, NULL, 24000, NULL, 1, 58, 42, 52, NULL, NULL, 1, 0, NULL, N'কালিন্দী ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'Basta Union', N'UN-BAS-01', NULL, 6, N'Mrs. Nasima Begum', N'01722-501001', NULL, NULL, N'Md. Nurul Amin', N'01733-601001', N'basta.union@vdp.gov.bd', NULL, 9, 16, NULL, NULL, 30000, NULL, 1, 65, 45, 55, NULL, NULL, 1, 0, NULL, N'বাস্তা ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'Tongi Union', N'UN-TON-01', NULL, 6, N'Md. Alamgir Kabir', N'01744-701001', NULL, NULL, N'Md. Mizanur Rahman', N'01755-801001', N'tongi.union@vdp.gov.bd', NULL, 9, 8, NULL, NULL, 48000, NULL, 1, 75, 55, 65, NULL, NULL, 0, 0, NULL, N'টঙ্গী ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'Kapasia Union', N'UN-KAP-01', NULL, 7, N'Md. Faruk Ahmed', N'01766-901001', NULL, NULL, N'Md. Anwar Hossain', N'01777-002001', N'kapasia.union@vdp.gov.bd', NULL, 9, 20, NULL, NULL, 35000, NULL, 1, 48, 32, 40, NULL, NULL, 1, 0, NULL, N'কাপাসিয়া ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, N'Targaon Union', N'UN-TAR-01', NULL, 8, N'Md. Selim Reza', N'01788-102001', NULL, NULL, N'Md. Hanif Khan', N'01799-202001', N'targaon.union@vdp.gov.bd', NULL, 9, 17, NULL, NULL, 29000, NULL, 1, 50, 35, 42, NULL, NULL, 1, 0, NULL, N'তারগাঁও ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, N'Fatulla Union', N'UN-FAT-01', NULL, 9, N'Md. Mahbub Alam', N'01711-302001', NULL, NULL, N'Md. Shahabuddin', N'01722-402001', N'fatulla.union@vdp.gov.bd', NULL, 9, 9, NULL, NULL, 42000, NULL, 1, 62, 48, 58, NULL, NULL, 0, 0, NULL, N'ফতুল্লা ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, N'Rupganj Union', N'UN-RUP-01', NULL, 10, N'Mrs. Shahina Akter', N'01733-502001', NULL, NULL, N'Md. Kamal Uddin', N'01744-602001', N'rupganj.union@vdp.gov.bd', NULL, 9, 14, NULL, NULL, 38000, NULL, 1, 55, 40, 50, NULL, NULL, 1, 0, NULL, N'রূপগঞ্জ ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, N'Santosh Union', N'UN-SAN-01', NULL, 11, N'Md. Jalal Uddin', N'01755-702001', NULL, NULL, N'Md. Ruhul Amin', N'01766-802001', N'santosh.union@vdp.gov.bd', NULL, 9, 12, NULL, NULL, 31000, NULL, 1, 58, 42, 52, NULL, NULL, 1, 0, NULL, N'সান্তোষ ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Unions] ([Id], [Name], [Code], [NameBangla], [UpazilaId], [ChairmanName], [ChairmanContact], [SecretaryName], [SecretaryContact], [VDPOfficerName], [VDPOfficerContact], [Email], [OfficeAddress], [NumberOfWards], [NumberOfVillages], [NumberOfMouzas], [Area], [Population], [NumberOfHouseholds], [HasVDPUnit], [VDPMemberCountMale], [VDPMemberCountFemale], [AnsarMemberCount], [Latitude], [Longitude], [IsRural], [IsBorderArea], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, N'Kalihati Union', N'UN-KLH-01', NULL, 12, N'Md. Abdur Rahman', N'01777-902001', NULL, NULL, N'Md. Shamsul Haque', N'01788-003001', N'kalihati.union@vdp.gov.bd', NULL, 9, 15, NULL, NULL, 27000, NULL, 1, 52, 38, 48, NULL, NULL, 1, 0, NULL, N'কালিহাতী ইউনিয়ন', CAST(N'2025-11-04T06:38:29.9533333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Unions] OFF
GO
SET IDENTITY_INSERT [dbo].[Units] ON 
GO
INSERT [dbo].[Units] ([Id], [Code], [Name], [Symbol], [Type], [ConversionFactor], [BaseUnit], [NameBn], [Abbreviation], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'UN-PC', N'Piece', N'pcs', N'Count', CAST(1.000000 AS Decimal(18, 6)), NULL, NULL, N'pc', N'Individual unit/piece', CAST(N'2025-11-04T05:07:38.9633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Units] ([Id], [Code], [Name], [Symbol], [Type], [ConversionFactor], [BaseUnit], [NameBn], [Abbreviation], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'UN-RM', N'Ream', N'ream', N'Count', CAST(500.000000 AS Decimal(18, 6)), NULL, NULL, N'ream', N'500 sheets of paper', CAST(N'2025-11-04T05:07:38.9633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Units] ([Id], [Code], [Name], [Symbol], [Type], [ConversionFactor], [BaseUnit], [NameBn], [Abbreviation], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'UN-BOX', N'Box', N'box', N'Count', CAST(1.000000 AS Decimal(18, 6)), NULL, NULL, N'box', N'Box containing items', CAST(N'2025-11-04T05:07:38.9633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Units] ([Id], [Code], [Name], [Symbol], [Type], [ConversionFactor], [BaseUnit], [NameBn], [Abbreviation], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'UN-SET', N'Set', N'set', N'Count', CAST(1.000000 AS Decimal(18, 6)), NULL, NULL, N'set', N'Set of items', CAST(N'2025-11-04T05:07:38.9633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Units] ([Id], [Code], [Name], [Symbol], [Type], [ConversionFactor], [BaseUnit], [NameBn], [Abbreviation], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'UN-KG', N'Kilogram', N'kg', N'Weight', CAST(1.000000 AS Decimal(18, 6)), NULL, NULL, N'kg', N'Weight in kilograms', CAST(N'2025-11-04T05:07:38.9633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Units] ([Id], [Code], [Name], [Symbol], [Type], [ConversionFactor], [BaseUnit], [NameBn], [Abbreviation], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'UN-LTR', N'Liter', N'L', N'Volume', CAST(1.000000 AS Decimal(18, 6)), NULL, NULL, N'L', N'Volume in liters', CAST(N'2025-11-04T05:07:38.9633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Units] ([Id], [Code], [Name], [Symbol], [Type], [ConversionFactor], [BaseUnit], [NameBn], [Abbreviation], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'UN-MTR', N'Meter', N'm', N'Length', CAST(1.000000 AS Decimal(18, 6)), NULL, NULL, N'm', N'Length in meters', CAST(N'2025-11-04T05:07:38.9633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Units] ([Id], [Code], [Name], [Symbol], [Type], [ConversionFactor], [BaseUnit], [NameBn], [Abbreviation], [Description], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'UN-PAIR', N'Pair', N'pair', N'Count', CAST(2.000000 AS Decimal(18, 6)), NULL, NULL, N'pair', N'Pair of items', CAST(N'2025-11-04T05:07:38.9633333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Units] OFF
GO
SET IDENTITY_INSERT [dbo].[Upazilas] ON 
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'Mirpur Upazila', N'MU-01', N'মিরপুর উপজেলা', 1, N'Md. Sohel Rana', NULL, N'01700-301001', N'mirpur.upazila@ansar.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মিরপুর উপজেলা', CAST(N'2025-11-04T03:31:58.6900000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Uttara Upazila', N'UU-01', N'উত্তরা উপজেলা', 1, N'Md. Jahidul Islam', NULL, N'01700-301002', N'uttara.upazila@ansar.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'উত্তরা উপজেলা', CAST(N'2025-11-04T03:31:58.6900000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Savar Upazila', N'DU-SAV', N'সাভার উপজেলা', 1, N'Md. Nazrul Islam', NULL, N'01700-311001', N'savar.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 250, NULL, NULL, NULL, N'সাভার উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'Keraniganj Upazila', N'DU-KER', N'কেরানীগঞ্জ উপজেলা', 1, N'Md. Mizanur Rahman', NULL, N'01700-312001', N'keraniganj.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 180, NULL, NULL, NULL, N'কেরানীগঞ্জ উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'Dhamrai Upazila', N'DU-DHA', N'ধামরাই উপজেলা', 1, N'Md. Shahjahan Ali', NULL, N'01700-313001', N'dhamrai.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 200, NULL, NULL, NULL, N'ধামরাই উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'Gazipur Sadar Upazila', N'GU-SAD', N'গাজীপুর সদর উপজেলা', 4, N'Md. Abdul Karim', NULL, N'01700-321001', N'gazipur-sadar.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 300, NULL, NULL, NULL, N'গাজীপুর সদর উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'Kapasia Upazila', N'GU-KAP', N'কাপাসিয়া উপজেলা', 4, N'Md. Ruhul Amin', NULL, N'01700-322001', N'kapasia.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 220, NULL, NULL, NULL, N'কাপাসিয়া উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'Kaliakair Upazila', N'GU-KAL', N'কালিয়াকৈর উপজেলা', 4, N'Md. Shahidul Islam', NULL, N'01700-323001', N'kaliakair.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 240, NULL, NULL, NULL, N'কালিয়াকৈর উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'Narayanganj Sadar', N'NU-SAD', N'নারায়ণগঞ্জ সদর', 5, N'Md. Hanif Khan', NULL, N'01700-331001', N'narayanganj-sadar.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 280, NULL, NULL, NULL, N'নারায়ণগঞ্জ সদর', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'Rupganj Upazila', N'NU-RUP', N'রূপগঞ্জ উপজেলা', 5, N'Md. Kamrul Hasan', NULL, N'01700-332001', N'rupganj.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 190, NULL, NULL, NULL, N'রূপগঞ্জ উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'Tangail Sadar', N'TU-SAD', N'টাঙ্গাইল সদর', 6, N'Md. Alamgir Hossain', NULL, N'01700-341001', N'tangail-sadar.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 260, NULL, NULL, NULL, N'টাঙ্গাইল সদর', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, N'Kalihati Upazila', N'TU-KAL', N'কালিহাতী উপজেলা', 6, N'Md. Faruk Ahmed', NULL, N'01700-342001', N'kalihati.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 170, NULL, NULL, NULL, N'কালিহাতী উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, N'Bogura Sadar', N'BU-SAD', N'বগুড়া সদর', 7, N'Md. Nurul Amin', NULL, N'01700-351001', N'bogura-sadar.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 290, NULL, NULL, NULL, N'বগুড়া সদর', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, N'Sherpur Upazila', N'BU-SHE', N'শেরপুর উপজেলা', 7, N'Md. Anisur Rahman', NULL, N'01700-352001', N'sherpur-bogura.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 160, NULL, NULL, NULL, N'শেরপুর উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, N'Cumilla Sadar', N'COU-SAD', N'কুমিল্লা সদর', 10, N'Md. Jahangir Alam', NULL, N'01700-361001', N'cumilla-sadar.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 310, NULL, NULL, NULL, N'কুমিল্লা সদর', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, N'Burichang Upazila', N'COU-BUR', N'বুড়িচং উপজেলা', 10, N'Md. Rafiqul Islam', NULL, N'01700-362001', N'burichang.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 150, NULL, NULL, NULL, N'বুড়িচং উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, N'Noakhali Sadar', N'NOU-SAD', N'নোয়াখালী সদর', 11, N'Md. Motaleb Miah', NULL, N'01700-371001', N'noakhali-sadar.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 270, NULL, NULL, NULL, N'নোয়াখালী সদর', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, N'Companiganj Upazila', N'NOU-COM', N'কোম্পানীগঞ্জ উপজেলা', 11, N'Md. Abdul Mannan', NULL, N'01700-372001', N'companiganj.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 180, NULL, NULL, NULL, N'কোম্পানীগঞ্জ উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, N'Jashore Sadar', N'JU-SAD', N'যশোর সদর', 13, N'Md. Shamsul Haque', NULL, N'01700-381001', N'jashore-sadar.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 250, NULL, NULL, NULL, N'যশোর সদর', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, N'Jhikargacha Upazila', N'JU-JHI', N'ঝিকরগাছা উপজেলা', 13, N'Md. Mahabubur Rahman', NULL, N'01700-382001', N'jhikargacha.upazila@vdp.gov.bd', NULL, NULL, NULL, NULL, NULL, 1, 200, NULL, NULL, NULL, N'ঝিকরগাছা উপজেলা', CAST(N'2025-11-04T06:37:52.3133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (26, N'Kishoreganj Sadar', N'KIS-SAD', N'কিশোরগঞ্জ সদর', 25, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কিশোরগঞ্জ সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (27, N'Bhairab', N'KIS-BHA', N'ভৈরব', 25, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ভৈরব', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (28, N'Manikganj Sadar', N'MAN-SAD', N'মানিকগঞ্জ সদর', 26, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মানিকগঞ্জ সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (29, N'Singair', N'MAN-SIN', N'সিঙ্গাইর', 26, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'সিঙ্গাইর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (30, N'Munshiganj Sadar', N'MUN-SAD', N'মুন্সিগঞ্জ সদর', 27, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মুন্সিগঞ্জ সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (31, N'Sreenagar', N'MUN-SRE', N'শ্রীনগর', 27, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'শ্রীনগর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (32, N'Narsingdi Sadar', N'NRS-SAD', N'নরসিংদী সদর', 28, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'নরসিংদী সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (33, N'Raipura', N'NRS-RAI', N'রায়পুরা', 28, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রায়পুরা', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (34, N'Faridpur Sadar', N'FAR-SAD', N'ফরিদপুর সদর', 31, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ফরিদপুর সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (35, N'Bhanga', N'FAR-BHA', N'ভাঙ্গা', 31, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ভাঙ্গা', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (36, N'Gopalganj Sadar', N'GOP-SAD', N'গোপালগঞ্জ সদর', 32, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'গোপালগঞ্জ সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (37, N'Tungipara', N'GOP-TUN', N'টুঙ্গিপাড়া', 32, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'টুঙ্গিপাড়া', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (38, N'Madaripur Sadar', N'MAD-SAD', N'মাদারীপুর সদর', 33, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মাদারীপুর সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (39, N'Rajoir', N'MAD-RAJ', N'রাজৈর', 33, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রাজৈর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (40, N'Rajbari Sadar', N'RAJB-SAD', N'রাজবাড়ী সদর', 29, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রাজবাড়ী সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (41, N'Goalanda', N'RAJB-GOA', N'গোয়ালন্দ', 29, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'গোয়ালন্দ', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (42, N'Shariatpur Sadar', N'SHA-SAD', N'শরীয়তপুর সদর', 30, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'শরীয়তপুর সদর', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (43, N'Naria', N'SHA-NAR', N'নড়িয়া', 30, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'নড়িয়া', CAST(N'2025-11-04T07:29:02.6133333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (44, N'Chattogram Sadar', N'CHT-SAD', N'চট্টগ্রাম সদর', 34, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'চট্টগ্রাম সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (45, N'Pahartali', N'CHT-PAH', N'পাহাড়তলী', 34, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'পাহাড়তলী', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (46, N'Brahmanbaria Sadar', N'BRA-SAD', N'ব্রাহ্মণবাড়িয়া সদর', 44, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ব্রাহ্মণবাড়িয়া সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (47, N'Kasba', N'BRA-KAS', N'কসবা', 44, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কসবা', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (48, N'Chandpur Sadar', N'CHA-SAD', N'চাঁদপুর সদর', 39, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'চাঁদপুর সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (49, N'Matlab', N'CHA-MAT', N'মতলব', 39, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মতলব', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (50, N'Coxs Bazar Sadar', N'COX-SAD', N'কক্সবাজার সদর', 40, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কক্সবাজার সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (51, N'Teknaf', N'COX-TEK', N'টেকনাফ', 40, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'টেকনাফ', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (52, N'Feni Sadar', N'FEN-SAD', N'ফেনী সদর', 37, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ফেনী সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (53, N'Chhagalnaiya', N'FEN-CHA', N'ছাগলনাইয়া', 37, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ছাগলনাইয়া', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (54, N'Lakshmipur Sadar', N'LAK-SAD', N'লক্ষ্মীপুর সদর', 38, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'লক্ষ্মীপুর সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (55, N'Raipur', N'LAK-RAI', N'রায়পুর', 38, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রায়পুর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (56, N'Rangamati Sadar', N'RAN-SAD', N'রাঙ্গামাটি সদর', 41, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রাঙ্গামাটি সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (57, N'Kaptai', N'RAN-KAP', N'কাপ্তাই', 41, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কাপ্তাই', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (58, N'Khagrachari Sadar', N'KHA-SAD', N'খাগড়াছড়ি সদর', 42, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'খাগড়াছড়ি সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (59, N'Manikchhari', N'KHA-MAN', N'মানিকছড়ি', 42, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মানিকছড়ি', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (60, N'Bandarban Sadar', N'BAN-SAD', N'বান্দরবান সদর', 43, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'বান্দরবান সদর', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (61, N'Thanchi', N'BAN-THA', N'থানচি', 43, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'থানচি', CAST(N'2025-11-04T07:29:25.7000000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (62, N'Naogaon Sadar', N'NAO-SAD', N'নওগাঁ সদর', 47, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'নওগাঁ সদর', CAST(N'2025-11-04T07:29:50.2666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (63, N'Porsha', N'NAO-POR', N'পোরশা', 47, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'পোরশা', CAST(N'2025-11-04T07:29:50.2666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (64, N'Sirajganj Sadar', N'SIR-SAD', N'সিরাজগঞ্জ সদর', 48, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'সিরাজগঞ্জ সদর', CAST(N'2025-11-04T07:29:50.2666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (65, N'Shahjadpur', N'SIR-SHA', N'শাহজাদপুর', 48, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'শাহজাদপুর', CAST(N'2025-11-04T07:29:50.2666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (66, N'Chapainawabganj Sadar', N'CHP-SAD', N'চাঁপাইনবাবগঞ্জ সদর', 49, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'চাঁপাইনবাবগঞ্জ সদর', CAST(N'2025-11-04T07:29:50.2666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (67, N'Shibganj', N'CHP-SHI', N'শিবগঞ্জ', 49, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'শিবগঞ্জ', CAST(N'2025-11-04T07:29:50.2666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (68, N'Joypurhat Sadar', N'JOY-SAD', N'জয়পুরহাট সদর', 50, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'জয়পুরহাট সদর', CAST(N'2025-11-04T07:29:50.2666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (69, N'Khetlal', N'JOY-KHE', N'ক্ষেতলাল', 50, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ক্ষেতলাল', CAST(N'2025-11-04T07:29:50.2666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (70, N'Satkhira Sadar', N'SAT-SAD', N'সাতক্ষীরা সদর', 69, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'সাতক্ষীরা সদর', CAST(N'2025-11-04T07:29:50.2700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (71, N'Kalaroa', N'SAT-KAL', N'কলারোয়া', 69, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কলারোয়া', CAST(N'2025-11-04T07:29:50.2700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (72, N'Bagerhat Sadar', N'BAG-SAD', N'বাগেরহাট সদর', 70, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'বাগেরহাট সদর', CAST(N'2025-11-04T07:29:50.2700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (73, N'Mollahat', N'BAG-MOL', N'মোল্লাহাট', 70, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মোল্লাহাট', CAST(N'2025-11-04T07:29:50.2700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (74, N'Jhenaidah Sadar', N'JHE-SAD', N'ঝিনাইদহ সদর', 71, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ঝিনাইদহ সদর', CAST(N'2025-11-04T07:29:50.2700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (75, N'Kaliganj', N'JHE-KAL', N'কালীগঞ্জ', 71, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কালীগঞ্জ', CAST(N'2025-11-04T07:29:50.2700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (76, N'Kushtia Sadar', N'KUST-SAD', N'কুষ্টিয়া সদর', 53, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কুষ্টিয়া সদর', CAST(N'2025-11-04T07:29:50.2700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (77, N'Kumarkhali', N'KUST-KUM', N'কুমারখালী', 53, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কুমারখালী', CAST(N'2025-11-04T07:29:50.2700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (91, N'Barisal Sadar', N'BAR-SAD', N'বরিশাল সদর', 54, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'বরিশাল সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (92, N'Babuganj', N'BAR-BAB', N'বাবুগঞ্জ', 54, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'বাবুগঞ্জ', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (93, N'Jhalokathi Sadar', N'JHA-SAD', N'ঝালকাঠি সদর', 55, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ঝালকাঠি সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (94, N'Rajapur', N'JHA-RAJ', N'রাজাপুর', 55, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রাজাপুর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (95, N'Pirojpur Sadar', N'PIR-SAD', N'পিরোজপুর সদর', 56, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'পিরোজপুর সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (96, N'Mathbaria', N'PIR-MAT', N'মঠবাড়িয়া', 56, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মঠবাড়িয়া', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (97, N'Barguna Sadar', N'BARG-SAD', N'বরগুনা সদর', 57, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'বরগুনা সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (98, N'Patharghata', N'BARG-PAT', N'পাথরঘাটা', 57, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'পাথরঘাটা', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (99, N'Sylhet Sadar', N'SYL-SAD', N'সিলেট সদর', 58, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'সিলেট সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (100, N'Sylhet Companiganj', N'SYL-COM', N'কোম্পানীগঞ্জ', 58, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কোম্পানীগঞ্জ', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (101, N'Sunamganj Sadar', N'SUN-SAD', N'সুনামগঞ্জ সদর', 59, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'সুনামগঞ্জ সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (102, N'Tahirpur', N'SUN-TAH', N'তাহিরপুর', 59, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'তাহিরপুর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (103, N'Rangpur Sadar', N'RGP-SAD', N'রংপুর সদর', 60, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রংপুর সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (104, N'Gangachara', N'RGP-GAN', N'গঙ্গাচড়া', 60, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'গঙ্গাচড়া', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (105, N'Gaibandha Sadar', N'GAI-SAD', N'গাইবান্ধা সদর', 61, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'গাইবান্ধা সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (106, N'Sundarganj', N'GAI-SUN', N'সুন্দরগঞ্জ', 61, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'সুন্দরগঞ্জ', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (107, N'Kurigram Sadar', N'KUR-SAD', N'কুড়িগ্রাম সদর', 62, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কুড়িগ্রাম সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (108, N'Ulipur', N'KUR-ULI', N'উলিপুর', 62, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'উলিপুর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (109, N'Lalmonirhat Sadar', N'LAL-SAD', N'লালমনিরহাট সদর', 63, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'লালমনিরহাট সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (110, N'Aditmari', N'LAL-ADI', N'আদিতমারী', 63, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'আদিতমারী', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (111, N'Nilphamari Sadar', N'NIL-SAD', N'নীলফামারী সদর', 64, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'নীলফামারী সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (112, N'Domar', N'NIL-DOM', N'ডোমার', 64, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ডোমার', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (113, N'Panchagarh Sadar', N'PAN-SAD', N'পঞ্চগড় সদর', 65, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'পঞ্চগড় সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (114, N'Debiganj', N'PAN-DEB', N'দেবীগঞ্জ', 65, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'দেবীগঞ্জ', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (115, N'Mymensingh Sadar', N'MYM-SAD', N'ময়মনসিংহ সদর', 66, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ময়মনসিংহ সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (116, N'Trishal', N'MYM-TRI', N'ত্রিশাল', 66, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ত্রিশাল', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (117, N'Sherpur Sadar', N'SHEP-SAD', N'শেরপুর সদর', 67, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'শেরপুর সদর', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (118, N'Nalitabari', N'SHEP-NAL', N'নালিতাবাড়ী', 67, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'নালিতাবাড়ী', CAST(N'2025-11-04T07:30:50.5700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (119, N'Bhola Sadar', N'BHO-SAD', N'ভোলা সদর', 16, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ভোলা সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (120, N'Daulatkhan', N'BHO-DAU', N'দৌলতখান', 16, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'দৌলতখান', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (121, N'Patuakhali Sadar', N'PAT-SAD', N'পটুয়াখালী সদর', 15, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'পটুয়াখালী সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (122, N'Kalapara', N'PAT-KAL', N'কলাপাড়া', 15, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কলাপাড়া', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (123, N'Cumilla Sadar South', N'COM-SADS', N'কুমিল্লা সদর দক্ষিণ', 35, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কুমিল্লা সদর দক্ষিণ', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (124, N'Laksam', N'COM-LAK', N'লাকসাম', 35, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'লাকসাম', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (125, N'Noakhali Sadar South', N'NOA-SADS', N'নোয়াখালী সদর', 36, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'নোয়াখালী সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (126, N'Begumganj', N'NOA-BEG', N'বেগমগঞ্জ', 36, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'বেগমগঞ্জ', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (127, N'Coxs Bazar Sadar South', N'COXS-SADS', N'কক্সবাজার সদর', 12, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কক্সবাজার সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (128, N'Ukhiya', N'COXS-UKH', N'উখিয়া', 12, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'উখিয়া', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (129, N'Gazipur Sadar South', N'GAZ-SADS', N'গাজীপুর সদর', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'গাজীপুর সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (130, N'Tongi', N'GAZ-TON', N'টঙ্গী', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'টঙ্গী', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (131, N'Kushtia Sadar South', N'KUS-SADS', N'কুষ্টিয়া সদর', 14, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কুষ্টিয়া সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (132, N'Mirpur', N'KUS-MIR', N'মিরপুর', 14, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মিরপুর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (133, N'Jamalpur Sadar', N'JAM-SAD', N'জামালপুর সদর', 22, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'জামালপুর সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (134, N'Melandaha', N'JAM-MEL', N'মেলান্দহ', 22, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মেলান্দহ', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (135, N'Netrokona Sadar', N'NET-SAD', N'নেত্রকোনা সদর', 21, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'নেত্রকোনা সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (136, N'Kendua', N'NET-KEN', N'কেন্দুয়া', 21, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'কেন্দুয়া', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (137, N'Natore Sadar', N'NAT-SAD', N'নাটোর সদর', 8, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'নাটোর সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (138, N'Singra', N'NAT-SIN', N'সিংড়া', 8, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'সিংড়া', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (139, N'Pabna Sadar', N'PAB-SAD', N'পাবনা সদর', 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'পাবনা সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (140, N'Ishwardi', N'PAB-ISH', N'ঈশ্বরদী', 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ঈশ্বরদী', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (141, N'Rajshahi Sadar South', N'RAJ-SADS', N'রাজশাহী সদর', 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রাজশাহী সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (142, N'Godagari', N'RAJ-GOD', N'গোদাগাড়ী', 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'গোদাগাড়ী', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (143, N'Dinajpur Sadar', N'DIN-SAD', N'দিনাজপুর সদর', 19, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'দিনাজপুর সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (144, N'Birganj', N'DIN-BIR', N'বীরগঞ্জ', 19, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'বীরগঞ্জ', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (145, N'Thakurgaon Sadar', N'THA-SAD', N'ঠাকুরগাঁও সদর', 20, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'ঠাকুরগাঁও সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (146, N'Ranisankail', N'THA-RAN', N'রাণীশংকৈল', 20, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'রাণীশংকৈল', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (147, N'Habiganj Sadar', N'HAB-SAD', N'হবিগঞ্জ সদর', 18, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'হবিগঞ্জ সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (148, N'Chunarughat', N'HAB-CHU', N'চুনারুঘাট', 18, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'চুনারুঘাট', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (149, N'Moulvibazar Sadar', N'MOU-SAD', N'মৌলভীবাজার সদর', 17, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'মৌলভীবাজার সদর', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Upazilas] ([Id], [Name], [Code], [NameBangla], [ZilaId], [UpazilaOfficerName], [OfficerDesignation], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [NumberOfUnions], [NumberOfVillages], [HasVDPUnit], [VDPMemberCount], [Remarks], [UpazilaChairmanName], [VDPOfficerName], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (150, N'Sreemangal', N'MOU-SRE', N'শ্রীমঙ্গল', 17, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, N'শ্রীমঙ্গল', CAST(N'2025-11-04T07:31:37.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Upazilas] OFF
GO
SET IDENTITY_INSERT [dbo].[UserStores] ON 
GO
INSERT [dbo].[UserStores] ([Id], [UserId], [StoreId], [AssignedDate], [AssignedBy], [IsPrimary], [UnassignedDate], [AssignedAt], [RemovedDate], [Role], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'450daed5-0c4e-44ee-b8a2-837e34d91682', 2, CAST(N'2025-09-05T05:03:43.3366667' AS DateTime2), NULL, 1, NULL, CAST(N'2025-09-05T05:03:43.3366667' AS DateTime2), NULL, N'StoreKeeper', CAST(N'2025-09-05T05:03:43.3366667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[UserStores] ([Id], [UserId], [StoreId], [AssignedDate], [AssignedBy], [IsPrimary], [UnassignedDate], [AssignedAt], [RemovedDate], [Role], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'cc42bb39-4fbc-4c69-9cf5-f755293a5a0e', 3, CAST(N'2025-09-15T05:03:43.3366667' AS DateTime2), NULL, 1, NULL, CAST(N'2025-09-15T05:03:43.3366667' AS DateTime2), NULL, N'StoreKeeper', CAST(N'2025-09-15T05:03:43.3366667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[UserStores] ([Id], [UserId], [StoreId], [AssignedDate], [AssignedBy], [IsPrimary], [UnassignedDate], [AssignedAt], [RemovedDate], [Role], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'a27a83b4-740f-42b5-b06e-8e438e209f17', 2, CAST(N'2025-09-05T05:03:43.3366667' AS DateTime2), NULL, 1, NULL, CAST(N'2025-09-05T05:03:43.3366667' AS DateTime2), NULL, N'StoreManager', CAST(N'2025-09-05T05:03:43.3366667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[UserStores] ([Id], [UserId], [StoreId], [AssignedDate], [AssignedBy], [IsPrimary], [UnassignedDate], [AssignedAt], [RemovedDate], [Role], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'a27a83b4-740f-42b5-b06e-8e438e209f17', 3, CAST(N'2025-09-15T05:03:43.3366667' AS DateTime2), NULL, 0, NULL, CAST(N'2025-09-15T05:03:43.3366667' AS DateTime2), NULL, N'StoreManager', CAST(N'2025-09-15T05:03:43.3366667' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[UserStores] OFF
GO
SET IDENTITY_INSERT [dbo].[Vendors] ON 
GO
INSERT [dbo].[Vendors] ([Id], [Name], [VendorType], [ContactPerson], [Phone], [Email], [Mobile], [Address], [TIN], [BIN], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'Computer Point Ltd.', N'IT Equipment Supplier', N'Md. Shahabuddin Ahmed', N'02-9876543', N'info@computerpoint.com.bd', N'01711-123456', N'45/2 Eskaton Garden, Dhaka-1000', N'123456789012', N'000123456789', CAST(N'2025-11-04T03:12:26.8833333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Vendors] ([Id], [Name], [VendorType], [ContactPerson], [Phone], [Email], [Mobile], [Address], [TIN], [BIN], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Stationary World Ltd.', N'Office Supplies Supplier', N'Mrs. Nasima Akter', N'02-9123456', N'sales@stationaryworld.com.bd', N'01711-234567', N'23 Bangla Motor, Dhaka-1000', N'234567890123', N'000234567890', CAST(N'2025-11-04T04:05:33.8333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Vendors] ([Id], [Name], [VendorType], [ContactPerson], [Phone], [Email], [Mobile], [Address], [TIN], [BIN], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Uniform Industries Ltd.', N'Uniform Manufacturer', N'Md. Kamal Uddin', N'02-7654321', N'contact@uniformindustries.com', N'01713-345678', N'123 Tejgaon I/A, Dhaka-1208', N'345678901234', N'000345678901', CAST(N'2025-11-04T04:05:33.8333333' AS DateTime2), NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Vendors] OFF
GO
SET IDENTITY_INSERT [dbo].[Warranties] ON 
GO
INSERT [dbo].[Warranties] ([Id], [ItemId], [WarrantyNumber], [VendorId], [StartDate], [EndDate], [WarrantyType], [Terms], [ContactInfo], [Status], [CoveredValue], [SerialNumber], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, 5, N'WRT-DESK-2024-001', 1, CAST(N'2025-10-05T07:00:32.9800000' AS DateTime2), CAST(N'2028-10-05T07:00:32.9800000' AS DateTime2), N'Manufacturer', N'3 Years Parts and Labor, On-site service', N'support@computerpoint.com.bd, 02-9876543', N'Active', CAST(45000.00 AS Decimal(18, 2)), N'DSKCOMP-2024-001', NULL, CAST(N'2025-10-05T07:00:32.9800000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Warranties] ([Id], [ItemId], [WarrantyNumber], [VendorId], [StartDate], [EndDate], [WarrantyType], [Terms], [ContactInfo], [Status], [CoveredValue], [SerialNumber], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, 5, N'WRT-DESK-2024-002', 1, CAST(N'2025-10-10T07:00:32.9800000' AS DateTime2), CAST(N'2028-10-10T07:00:32.9800000' AS DateTime2), N'Manufacturer', N'3 Years Parts and Labor, On-site service', N'support@computerpoint.com.bd, 02-9876543', N'Active', CAST(45000.00 AS Decimal(18, 2)), N'DSKCOMP-2024-002', NULL, CAST(N'2025-10-10T07:00:32.9800000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Warranties] ([Id], [ItemId], [WarrantyNumber], [VendorId], [StartDate], [EndDate], [WarrantyType], [Terms], [ContactInfo], [Status], [CoveredValue], [SerialNumber], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, 6, N'WRT-PRNT-2024-001', 1, CAST(N'2025-10-05T07:00:32.9800000' AS DateTime2), CAST(N'2027-10-05T07:00:32.9800000' AS DateTime2), N'Extended', N'2 Years Parts, 1 Year Labor, Toner included', N'support@computerpoint.com.bd, 02-9876543', N'Active', CAST(18000.00 AS Decimal(18, 2)), N'PRNT-2024-001', NULL, CAST(N'2025-10-05T07:00:32.9800000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Warranties] ([Id], [ItemId], [WarrantyNumber], [VendorId], [StartDate], [EndDate], [WarrantyType], [Terms], [ContactInfo], [Status], [CoveredValue], [SerialNumber], [ItemId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, 6, N'WRT-PRNT-2024-002', 1, CAST(N'2025-10-07T07:00:32.9800000' AS DateTime2), CAST(N'2027-10-07T07:00:32.9800000' AS DateTime2), N'Extended', N'2 Years Parts, 1 Year Labor, Toner included', N'support@computerpoint.com.bd, 02-9876543', N'Active', CAST(18000.00 AS Decimal(18, 2)), N'PRNT-2024-002', NULL, CAST(N'2025-10-07T07:00:32.9800000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Warranties] OFF
GO
SET IDENTITY_INSERT [dbo].[WriteOffItems] ON 
GO
INSERT [dbo].[WriteOffItems] ([Id], [WriteOffId], [ItemId], [StoreId], [WriteOffRequestId], [Quantity], [Value], [UnitCost], [TotalCost], [Reason], [BatchNo], [WriteOffRequestId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (33, 20, 13, 2, NULL, CAST(15.00 AS Decimal(18, 2)), CAST(18000.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), CAST(18000.00 AS Decimal(18, 2)), N'Torn and stained during transport', NULL, NULL, CAST(N'2025-10-21T02:12:01.5400000' AS DateTime2), N'1', NULL, NULL, 1)
GO
INSERT [dbo].[WriteOffItems] ([Id], [WriteOffId], [ItemId], [StoreId], [WriteOffRequestId], [Quantity], [Value], [UnitCost], [TotalCost], [Reason], [BatchNo], [WriteOffRequestId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (34, 21, 11, 2, NULL, CAST(100.00 AS Decimal(18, 2)), CAST(8000.00 AS Decimal(18, 2)), CAST(80.00 AS Decimal(18, 2)), CAST(8000.00 AS Decimal(18, 2)), N'Water damaged and unusable', NULL, NULL, CAST(N'2025-10-24T02:12:01.5433333' AS DateTime2), N'1', NULL, NULL, 1)
GO
INSERT [dbo].[WriteOffItems] ([Id], [WriteOffId], [ItemId], [StoreId], [WriteOffRequestId], [Quantity], [Value], [UnitCost], [TotalCost], [Reason], [BatchNo], [WriteOffRequestId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (35, 22, 8, 2, NULL, CAST(500.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), N'Missing during inventory count', NULL, NULL, CAST(N'2025-11-02T02:12:01.5466667' AS DateTime2), N'1', NULL, NULL, 1)
GO
INSERT [dbo].[WriteOffItems] ([Id], [WriteOffId], [ItemId], [StoreId], [WriteOffRequestId], [Quantity], [Value], [UnitCost], [TotalCost], [Reason], [BatchNo], [WriteOffRequestId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (36, 23, 13, 2, NULL, CAST(20.00 AS Decimal(18, 2)), CAST(24000.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), CAST(24000.00 AS Decimal(18, 2)), N'Burned in storage fire', NULL, NULL, CAST(N'2025-11-04T02:12:01.5466667' AS DateTime2), N'1', NULL, NULL, 1)
GO
INSERT [dbo].[WriteOffItems] ([Id], [WriteOffId], [ItemId], [StoreId], [WriteOffRequestId], [Quantity], [Value], [UnitCost], [TotalCost], [Reason], [BatchNo], [WriteOffRequestId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (37, 23, 14, 2, NULL, CAST(15.00 AS Decimal(18, 2)), CAST(15000.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), CAST(15000.00 AS Decimal(18, 2)), N'Burned in storage fire', NULL, NULL, CAST(N'2025-11-04T02:12:01.5466667' AS DateTime2), N'1', NULL, NULL, 1)
GO
INSERT [dbo].[WriteOffItems] ([Id], [WriteOffId], [ItemId], [StoreId], [WriteOffRequestId], [Quantity], [Value], [UnitCost], [TotalCost], [Reason], [BatchNo], [WriteOffRequestId1], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (38, 23, 11, 2, NULL, CAST(75.00 AS Decimal(18, 2)), CAST(6000.00 AS Decimal(18, 2)), CAST(80.00 AS Decimal(18, 2)), CAST(6000.00 AS Decimal(18, 2)), N'Burned in storage fire', NULL, NULL, CAST(N'2025-11-04T02:12:01.5466667' AS DateTime2), N'1', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[WriteOffItems] OFF
GO
SET IDENTITY_INSERT [dbo].[WriteOffRequests] ON 
GO
INSERT [dbo].[WriteOffRequests] ([Id], [RequestNo], [DamageReportId], [DamageReportNo], [StoreId], [RequestDate], [RequestedBy], [TotalValue], [Status], [Justification], [ApprovedBy], [ApprovedDate], [ApprovalRemarks], [ApprovalReference], [ExecutedBy], [ExecutedDate], [DisposalMethod], [DisposalRemarks], [Reason], [RejectedBy], [RejectedDate], [RejectionReason], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'WOR-2024-001', 4, NULL, 2, CAST(N'2025-10-27T05:42:10.3200000' AS DateTime2), N'a27a83b4-740f-42b5-b06e-8e438e209f17', CAST(36000.00 AS Decimal(18, 2)), 2, N'Items damaged beyond repair', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Damaged', NULL, NULL, NULL, CAST(N'2025-10-27T05:42:10.3200000' AS DateTime2), N'a27a83b4-740f-42b5-b06e-8e438e209f17', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[WriteOffRequests] OFF
GO
SET IDENTITY_INSERT [dbo].[WriteOffs] ON 
GO
INSERT [dbo].[WriteOffs] ([Id], [WriteOffNo], [WriteOffDate], [Reason], [Status], [TotalValue], [StoreId], [ApprovedBy], [ApprovedDate], [ApprovalComments], [RejectedBy], [RejectionDate], [RejectionReason], [RequiredApproverRole], [ApprovalThreshold], [AttachmentPaths], [NotificationSent], [NotificationSentDate], [NotifiedUsers], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, N'WO-2025-001', CAST(N'2025-10-21T02:12:01.5400000' AS DateTime2), N'Damaged', N'Approved', CAST(18000.00 AS Decimal(18, 2)), 2, N'1', CAST(N'2025-10-26T02:12:01.5400000' AS DateTime2), N'Uniforms damaged during transportation', NULL, NULL, NULL, NULL, 10000, NULL, 1, NULL, NULL, CAST(N'2025-10-21T02:12:01.5400000' AS DateTime2), N'1', NULL, NULL, 1)
GO
INSERT [dbo].[WriteOffs] ([Id], [WriteOffNo], [WriteOffDate], [Reason], [Status], [TotalValue], [StoreId], [ApprovedBy], [ApprovedDate], [ApprovalComments], [RejectedBy], [RejectionDate], [RejectionReason], [RequiredApproverRole], [ApprovalThreshold], [AttachmentPaths], [NotificationSent], [NotificationSentDate], [NotifiedUsers], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (21, N'WO-2025-002', CAST(N'2025-10-24T02:12:01.5433333' AS DateTime2), N'Expired', N'Approved', CAST(8000.00 AS Decimal(18, 2)), 2, N'1', CAST(N'2025-10-29T02:12:01.5433333' AS DateTime2), N'Notebooks damaged due to moisture', NULL, NULL, NULL, NULL, 0, NULL, 1, NULL, NULL, CAST(N'2025-10-24T02:12:01.5433333' AS DateTime2), N'1', NULL, NULL, 1)
GO
INSERT [dbo].[WriteOffs] ([Id], [WriteOffNo], [WriteOffDate], [Reason], [Status], [TotalValue], [StoreId], [ApprovedBy], [ApprovedDate], [ApprovalComments], [RejectedBy], [RejectionDate], [RejectionReason], [RequiredApproverRole], [ApprovalThreshold], [AttachmentPaths], [NotificationSent], [NotificationSentDate], [NotifiedUsers], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (22, N'WO-2025-003', CAST(N'2025-11-02T02:12:01.5466667' AS DateTime2), N'Lost', N'Pending', CAST(5000.00 AS Decimal(18, 2)), 2, NULL, CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, NULL, NULL, CAST(N'2025-11-02T02:12:01.5466667' AS DateTime2), N'1', NULL, NULL, 1)
GO
INSERT [dbo].[WriteOffs] ([Id], [WriteOffNo], [WriteOffDate], [Reason], [Status], [TotalValue], [StoreId], [ApprovedBy], [ApprovedDate], [ApprovalComments], [RejectedBy], [RejectionDate], [RejectionReason], [RequiredApproverRole], [ApprovalThreshold], [AttachmentPaths], [NotificationSent], [NotificationSentDate], [NotifiedUsers], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (23, N'WO-2025-004', CAST(N'2025-11-04T02:12:01.5466667' AS DateTime2), N'Fire Damage', N'Pending', CAST(45000.00 AS Decimal(18, 2)), 2, NULL, CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL, N'DDGAdmin', 10000, NULL, 0, NULL, NULL, CAST(N'2025-11-04T02:12:01.5466667' AS DateTime2), N'1', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[WriteOffs] OFF
GO
SET IDENTITY_INSERT [dbo].[Zilas] ON 
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (1, N'Dhaka', N'DZ-01', N'ঢাকা', 2, N'Dhaka', N'Captain Md. Aminul Islam', N'01700-201001', N'dhaka.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ঢাকা', CAST(N'2025-11-04T03:31:35.6566667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (2, N'Gazipur', N'GZ-01', N'গাজীপুর', 2, N'Dhaka', N'Captain Md. Shafiqul Islam', N'01700-201002', N'gazipur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'গাজীপুর', CAST(N'2025-11-04T03:31:35.6566667' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (3, N'Rajshahi', N'RZ-01', N'রাজশাহী', 3, N'Rajshahi', N'Captain Md. Abdul Quddus', N'01700-202001', N'rajshahi.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'রাজশাহী', CAST(N'2025-11-04T03:31:35.6600000' AS DateTime2), NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (4, N'Gazipur Zila', N'DZ-GAZ', N'গাজীপুর জেলা', 2, N'Dhaka', N'Captain Md. Selim Ahmed', N'01700-202001', N'gazipur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'গাজীপুর জেলা', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (5, N'Narayanganj Zila', N'DZ-NAR', N'নারায়ণগঞ্জ জেলা', 2, N'Dhaka', N'Captain Md. Faruk Hossain', N'01700-203001', N'narayanganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'নারায়ণগঞ্জ জেলা', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (6, N'Tangail Zila', N'DZ-TAN', N'টাঙ্গাইল জেলা', 2, N'Dhaka', N'Captain Md. Shamsul Alam', N'01700-204001', N'tangail.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'টাঙ্গাইল জেলা', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (7, N'Bogura', N'RZ-BOG', N'বগুড়া', 3, N'Rajshahi', N'Captain Md. Abul Kalam', N'01700-301001', N'bogura.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'বগুড়া', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (8, N'Natore', N'RZ-NAT', N'নাটোর', 3, N'Rajshahi', N'Lt. Md. Nazrul Islam', N'01700-302001', N'natore.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'নাটোর', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (9, N'Pabna', N'RZ-PAB', N'পাবনা', 3, N'Rajshahi', N'Lt. Md. Hafizul Islam', N'01700-303001', N'pabna.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'পাবনা', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (10, N'Cumilla', N'CZ-COM', N'কুমিল্লা', 4, N'Chattogram', N'Captain Md. Jahangir Kabir', N'01700-401001', N'cumilla.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'কুমিল্লা', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (11, N'Noakhali', N'CZ-NOA', N'নোয়াখালী', 4, N'Chattogram', N'Lt. Md. Mahbub Alam', N'01700-402001', N'noakhali.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'নোয়াখালী', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (12, N'Coxs Bazar', N'CZ-COX', N'কক্সবাজার', 4, N'Chattogram', N'Captain Md. Rafiqul Alam', N'01700-403001', N'coxsbazar.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'কক্সবাজার', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (13, N'Jashore', N'KZ-JAS', N'যশোর', 5, N'Khulna', N'Captain Md. Monirul Haque', N'01700-501001', N'jashore.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'যশোর', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (14, N'Kushtia', N'KZ-KUS', N'কুষ্টিয়া', 5, N'Khulna', N'Lt. Md. Sharifuzzaman', N'01700-502001', N'kushtia.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'কুষ্টিয়া', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (15, N'Patuakhali', N'BZ-PAT', N'পটুয়াখালী', 6, N'Barisal', N'Lt. Md. Alamgir Kabir', N'01700-601001', N'patuakhali.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'পটুয়াখালী', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (16, N'Bhola', N'BZ-BHO', N'ভোলা', 6, N'Barisal', N'Lt. Md. Anwar Hossain', N'01700-602001', N'bhola.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ভোলা', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (17, N'Moulvibazar', N'SZ-MOU', N'মৌলভীবাজার', 7, N'Sylhet', N'Captain Md. Humayun Kabir', N'01700-701001', N'moulvibazar.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'মৌলভীবাজার', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (18, N'Habiganj', N'SZ-HAB', N'হবিগঞ্জ', 7, N'Sylhet', N'Lt. Md. Kamrul Hassan', N'01700-702001', N'habiganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'হবিগঞ্জ', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (19, N'Dinajpur', N'RGZ-DIN', N'দিনাজপুর', 8, N'Rangpur', N'Captain Md. Abdur Rahim', N'01700-801001', N'dinajpur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'দিনাজপুর', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (20, N'Thakurgaon', N'RGZ-THA', N'ঠাকুরগাঁও', 8, N'Rangpur', N'Lt. Md. Jahidul Islam', N'01700-802001', N'thakurgaon.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ঠাকুরগাঁও', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (21, N'Netrokona', N'MZ-NET', N'নেত্রকোনা', 9, N'Mymensingh', N'Captain Md. Shahabuddin', N'01700-901001', N'netrokona.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'নেত্রকোনা', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (22, N'Jamalpur', N'MZ-JAM', N'জামালপুর', 9, N'Mymensingh', N'Lt. Md. Shafiqul Islam', N'01700-902001', N'jamalpur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'জামালপুর', CAST(N'2025-11-04T06:36:31.3033333' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (25, N'Kishoreganj', N'DZ-KIS', N'কিশোরগঞ্জ', 2, N'Dhaka', N'Md. Habibur Rahman', N'01700-301005', N'kishoreganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'কিশোরগঞ্জ', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (26, N'Manikganj', N'DZ-MAN', N'মানিকগঞ্জ', 2, N'Dhaka', N'Md. Nazrul Islam', N'01700-301006', N'manikganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'মানিকগঞ্জ', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (27, N'Munshiganj', N'DZ-MUN', N'মুন্সিগঞ্জ', 2, N'Dhaka', N'Md. Shahin Alam', N'01700-301007', N'munshiganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'মুন্সিগঞ্জ', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (28, N'Narsingdi', N'DZ-NRS', N'নরসিংদী', 2, N'Dhaka', N'Md. Azizul Haque', N'01700-301008', N'narsingdi.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'নরসিংদী', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (29, N'Rajbari', N'DZ-RAJB', N'রাজবাড়ী', 2, N'Dhaka', N'Md. Monirul Islam', N'01700-301009', N'rajbari.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'রাজবাড়ী', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (30, N'Shariatpur', N'DZ-SHA', N'শরীয়তপুর', 2, N'Dhaka', N'Md. Alamgir Hossain', N'01700-301010', N'shariatpur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'শরীয়তপুর', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (31, N'Faridpur', N'DZ-FAR', N'ফরিদপুর', 2, N'Dhaka', N'Md. Shamsul Alam', N'01700-301011', N'faridpur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ফরিদপুর', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (32, N'Gopalganj', N'DZ-GOP', N'গোপালগঞ্জ', 2, N'Dhaka', N'Md. Abdul Jalil', N'01700-301012', N'gopalganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'গোপালগঞ্জ', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (33, N'Madaripur', N'DZ-MAD', N'মাদারীপুর', 2, N'Dhaka', N'Md. Kamrul Hassan', N'01700-301013', N'madaripur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'মাদারীপুর', CAST(N'2025-11-04T07:12:36.6400000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (34, N'Chattogram', N'CTG-CHT', N'চট্টগ্রাম', 4, N'Chattogram', N'Md. Asaduzzaman', N'01700-401001', N'chattogram.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'চট্টগ্রাম', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (35, N'Cumilla', N'CTG-COM', N'কুমিল্লা', 4, N'Chattogram', N'Md. Mamunur Rashid', N'01700-401002', N'cumilla.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'কুমিল্লা', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (36, N'Noakhali', N'CTG-NOA', N'নোয়াখালী', 4, N'Chattogram', N'Md. Shahabuddin', N'01700-401003', N'noakhali.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'নোয়াখালী', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (37, N'Feni', N'CTG-FEN', N'ফেনী', 4, N'Chattogram', N'Md. Jahangir Alam', N'01700-401004', N'feni.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ফেনী', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (38, N'Lakshmipur', N'CTG-LAK', N'লক্ষ্মীপুর', 4, N'Chattogram', N'Md. Rashidul Haque', N'01700-401005', N'lakshmipur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'লক্ষ্মীপুর', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (39, N'Chandpur', N'CTG-CHA', N'চাঁদপুর', 4, N'Chattogram', N'Md. Mizanur Rahman', N'01700-401006', N'chandpur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'চাঁদপুর', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (40, N'Coxs Bazar', N'CTG-COX', N'কক্সবাজার', 4, N'Chattogram', N'Md. Nurul Islam', N'01700-401007', N'coxsbazar.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'কক্সবাজার', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (41, N'Rangamati', N'CTG-RAN', N'রাঙ্গামাটি', 4, N'Chattogram', N'Md. Shamim Ahmed', N'01700-401008', N'rangamati.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'রাঙ্গামাটি', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (42, N'Khagrachari', N'CTG-KHA', N'খাগড়াছড়ি', 4, N'Chattogram', N'Md. Harun-or-Rashid', N'01700-401009', N'khagrachari.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'খাগড়াছড়ি', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (43, N'Bandarban', N'CTG-BAN', N'বান্দরবান', 4, N'Chattogram', N'Md. Saiful Islam', N'01700-401010', N'bandarban.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'বান্দরবান', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (44, N'Brahmanbaria', N'CTG-BRA', N'ব্রাহ্মণবাড়িয়া', 4, N'Chattogram', N'Md. Abdul Quddus', N'01700-401011', N'brahmanbaria.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ব্রাহ্মণবাড়িয়া', CAST(N'2025-11-04T07:12:56.5266667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (47, N'Naogaon', N'RZ-NAO', N'নওগাঁ', 3, N'Rajshahi', N'Md. Azizul Haque', N'01700-501004', N'naogaon.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'নওগাঁ', CAST(N'2025-11-04T07:13:24.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (48, N'Sirajganj', N'RZ-SIR', N'সিরাজগঞ্জ', 3, N'Rajshahi', N'Md. Alamgir Kabir', N'01700-501006', N'sirajganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'সিরাজগঞ্জ', CAST(N'2025-11-04T07:13:24.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (49, N'Chapainawabganj', N'RZ-CHP', N'চাঁপাইনবাবগঞ্জ', 3, N'Rajshahi', N'Md. Nurul Islam', N'01700-501007', N'chapainawabganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'চাঁপাইনবাবগঞ্জ', CAST(N'2025-11-04T07:13:24.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (50, N'Joypurhat', N'RZ-JOY', N'জয়পুরহাট', 3, N'Rajshahi', N'Md. Kamrul Hassan', N'01700-501008', N'joypurhat.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'জয়পুরহাট', CAST(N'2025-11-04T07:13:24.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (53, N'Kushtia', N'KZ-KUST', N'কুষ্টিয়া', 5, N'Khulna', N'Md. Jahangir Alam', N'01700-601010', N'kushtia.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'কুষ্টিয়া', CAST(N'2025-11-04T07:13:53.1500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (54, N'Barisal', N'BZ-BAR', N'বরিশাল', 6, N'Barisal', N'Md. Shamsul Alam', N'01700-701001', N'barisal.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'বরিশাল', CAST(N'2025-11-04T07:14:07.2500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (55, N'Jhalokathi', N'BZ-JHA', N'ঝালকাঠি', 6, N'Barisal', N'Md. Nurul Islam', N'01700-701002', N'jhalokathi.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ঝালকাঠি', CAST(N'2025-11-04T07:14:07.2500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (56, N'Pirojpur', N'BZ-PIR', N'পিরোজপুর', 6, N'Barisal', N'Md. Kamal Hossain', N'01700-701003', N'pirojpur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'পিরোজপুর', CAST(N'2025-11-04T07:14:07.2500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (57, N'Barguna', N'BZ-BAG', N'বরগুনা', 6, N'Barisal', N'Md. Monirul Islam', N'01700-701004', N'barguna.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'বরগুনা', CAST(N'2025-11-04T07:14:07.2500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (58, N'Sylhet', N'SZ-SYL', N'সিলেট', 7, N'Sylhet', N'Md. Rafiqul Islam', N'01700-801001', N'sylhet.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'সিলেট', CAST(N'2025-11-04T07:14:07.2500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (59, N'Sunamganj', N'SZ-SUN', N'সুনামগঞ্জ', 7, N'Sylhet', N'Md. Abdul Jalil', N'01700-801002', N'sunamganj.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'সুনামগঞ্জ', CAST(N'2025-11-04T07:14:07.2500000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (60, N'Rangpur', N'RGZ-RAN', N'রংপুর', 8, N'Rangpur', N'Md. Kamrul Hassan', N'01700-901001', N'rangpur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'রংপুর', CAST(N'2025-11-04T07:14:22.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (61, N'Gaibandha', N'RGZ-GAI', N'গাইবান্ধা', 8, N'Rangpur', N'Md. Shahin Alam', N'01700-901002', N'gaibandha.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'গাইবান্ধা', CAST(N'2025-11-04T07:14:22.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (62, N'Kurigram', N'RGZ-KUR', N'কুড়িগ্রাম', 8, N'Rangpur', N'Md. Nazrul Islam', N'01700-901003', N'kurigram.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'কুড়িগ্রাম', CAST(N'2025-11-04T07:14:22.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (63, N'Lalmonirhat', N'RGZ-LAL', N'লালমনিরহাট', 8, N'Rangpur', N'Md. Habibur Rahman', N'01700-901004', N'lalmonirhat.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'লালমনিরহাট', CAST(N'2025-11-04T07:14:22.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (64, N'Nilphamari', N'RGZ-NIL', N'নীলফামারী', 8, N'Rangpur', N'Md. Azizul Haque', N'01700-901005', N'nilphamari.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'নীলফামারী', CAST(N'2025-11-04T07:14:22.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (65, N'Panchagarh', N'RGZ-PAN', N'পঞ্চগড়', 8, N'Rangpur', N'Md. Monirul Islam', N'01700-901006', N'panchagarh.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'পঞ্চগড়', CAST(N'2025-11-04T07:14:22.7666667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (66, N'Mymensingh', N'MZ-MYM', N'ময়মনসিংহ', 9, N'Mymensingh', N'Md. Shamsul Alam', N'01700-1001001', N'mymensingh.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ময়মনসিংহ', CAST(N'2025-11-04T07:14:22.7700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (67, N'Sherpur', N'MZ-SHE', N'শেরপুর', 9, N'Mymensingh', N'Md. Alamgir Kabir', N'01700-1001002', N'sherpur.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'শেরপুর', CAST(N'2025-11-04T07:14:22.7700000' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (69, N'Satkhira', N'KZ-SAT', N'সাতক্ষীরা', 5, N'Khulna', N'Md. Alamgir Hossain', N'01700-601003', N'satkhira.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'সাতক্ষীরা', CAST(N'2025-11-04T07:15:32.0766667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (70, N'Bagerhat', N'KZ-BAG', N'বাগেরহাট', 5, N'Khulna', N'Md. Kamrul Hassan', N'01700-601004', N'bagerhat.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'বাগেরহাট', CAST(N'2025-11-04T07:15:32.0766667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
INSERT [dbo].[Zilas] ([Id], [Name], [Code], [NameBangla], [RangeId], [Division], [DistrictOfficerName], [ContactNumber], [Email], [OfficeAddress], [Area], [Population], [Remarks], [NameBn], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsActive]) VALUES (71, N'Jhenaidah', N'KZ-JHE', N'ঝিনাইদহ', 5, N'Khulna', N'Md. Shahin Alam', N'01700-601005', N'jhenaidah.zila@ansar.gov.bd', NULL, NULL, NULL, NULL, N'ঝিনাইদহ', CAST(N'2025-11-04T07:15:32.0766667' AS DateTime2), N'b9d49f27-6a2b-4bca-9ea5-ddb95d476989', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Zilas] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ActivityLogs_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ActivityLogs_UserId] ON [dbo].[ActivityLogs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterItems_AllotmentLetterId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterItems_AllotmentLetterId] ON [dbo].[AllotmentLetterItems]
(
	[AllotmentLetterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterItems_ItemId] ON [dbo].[AllotmentLetterItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterRecipientItems_AllotmentLetterRecipientId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterRecipientItems_AllotmentLetterRecipientId] ON [dbo].[AllotmentLetterRecipientItems]
(
	[AllotmentLetterRecipientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterRecipientItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterRecipientItems_ItemId] ON [dbo].[AllotmentLetterRecipientItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterRecipients_AllotmentLetterId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterRecipients_AllotmentLetterId] ON [dbo].[AllotmentLetterRecipients]
(
	[AllotmentLetterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterRecipients_BattalionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterRecipients_BattalionId] ON [dbo].[AllotmentLetterRecipients]
(
	[BattalionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterRecipients_RangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterRecipients_RangeId] ON [dbo].[AllotmentLetterRecipients]
(
	[RangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterRecipients_UnionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterRecipients_UnionId] ON [dbo].[AllotmentLetterRecipients]
(
	[UnionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterRecipients_UpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterRecipients_UpazilaId] ON [dbo].[AllotmentLetterRecipients]
(
	[UpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetterRecipients_ZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetterRecipients_ZilaId] ON [dbo].[AllotmentLetterRecipients]
(
	[ZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AllotmentLetters_AllotmentNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AllotmentLetters_AllotmentNo] ON [dbo].[AllotmentLetters]
(
	[AllotmentNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetters_FromStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetters_FromStoreId] ON [dbo].[AllotmentLetters]
(
	[FromStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetters_IssuedToBattalionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetters_IssuedToBattalionId] ON [dbo].[AllotmentLetters]
(
	[IssuedToBattalionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetters_IssuedToRangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetters_IssuedToRangeId] ON [dbo].[AllotmentLetters]
(
	[IssuedToRangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetters_IssuedToUpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetters_IssuedToUpazilaId] ON [dbo].[AllotmentLetters]
(
	[IssuedToUpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AllotmentLetters_IssuedToZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AllotmentLetters_IssuedToZilaId] ON [dbo].[AllotmentLetters]
(
	[IssuedToZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApprovalDelegations_FromUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalDelegations_FromUserId] ON [dbo].[ApprovalDelegations]
(
	[FromUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApprovalDelegations_ToUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalDelegations_ToUserId] ON [dbo].[ApprovalDelegations]
(
	[ToUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApprovalHistories_ApprovalRequestId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalHistories_ApprovalRequestId] ON [dbo].[ApprovalHistories]
(
	[ApprovalRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApprovalLevel_Level_EntityType]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalLevel_Level_EntityType] ON [dbo].[ApprovalLevels]
(
	[Level] ASC,
	[EntityType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApprovalRequests_ApprovedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalRequests_ApprovedByUserId] ON [dbo].[ApprovalRequests]
(
	[ApprovedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApprovalRequests_RequestedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalRequests_RequestedByUserId] ON [dbo].[ApprovalRequests]
(
	[RequestedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApprovalSteps_ApprovalRequestId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalSteps_ApprovalRequestId] ON [dbo].[ApprovalSteps]
(
	[ApprovalRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApprovalWorkflowLevels_WorkflowId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalWorkflowLevels_WorkflowId] ON [dbo].[ApprovalWorkflowLevels]
(
	[WorkflowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApprovalWorkflows_ApproverUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ApprovalWorkflows_ApproverUserId] ON [dbo].[ApprovalWorkflows]
(
	[ApproverUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_UserId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_UserId1] ON [dbo].[AspNetUserRoles]
(
	[UserId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUsers_BadgeNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers_BadgeNumber] ON [dbo].[AspNetUsers]
(
	[BadgeNumber] ASC
)
WHERE ([BadgeNumber] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUsers_BattalionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_BattalionId] ON [dbo].[AspNetUsers]
(
	[BattalionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUsers_RangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_RangeId] ON [dbo].[AspNetUsers]
(
	[RangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUsers_UnionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_UnionId] ON [dbo].[AspNetUsers]
(
	[UnionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUsers_UpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_UpazilaId] ON [dbo].[AspNetUsers]
(
	[UpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUsers_ZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_ZilaId] ON [dbo].[AspNetUsers]
(
	[ZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditLogs_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuditReports_PhysicalInventoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_AuditReports_PhysicalInventoryId] ON [dbo].[AuditReports]
(
	[PhysicalInventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Audits_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Audits_UserId] ON [dbo].[Audits]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Barcodes_BarcodeNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Barcodes_BarcodeNumber] ON [dbo].[Barcodes]
(
	[BarcodeNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Barcodes_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Barcodes_ItemId] ON [dbo].[Barcodes]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Barcodes_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Barcodes_StoreId] ON [dbo].[Barcodes]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BatchMovements_BatchId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchMovements_BatchId] ON [dbo].[BatchMovements]
(
	[BatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BatchMovements_BatchTrackingId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchMovements_BatchTrackingId] ON [dbo].[BatchMovements]
(
	[BatchTrackingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BatchMovements_MovementDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchMovements_MovementDate] ON [dbo].[BatchMovements]
(
	[MovementDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BatchSignatureItem_BatchSignatureId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchSignatureItem_BatchSignatureId] ON [dbo].[BatchSignatureItem]
(
	[BatchSignatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BatchSignatures_BatchId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchSignatures_BatchId] ON [dbo].[BatchSignatures]
(
	[BatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_BatchSignatures_SignedBy]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchSignatures_SignedBy] ON [dbo].[BatchSignatures]
(
	[SignedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_BatchTrackings_BatchNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchTrackings_BatchNumber] ON [dbo].[BatchTrackings]
(
	[BatchNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BatchTrackings_ItemId_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchTrackings_ItemId_StoreId] ON [dbo].[BatchTrackings]
(
	[ItemId] ASC,
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BatchTrackings_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BatchTrackings_StoreId] ON [dbo].[BatchTrackings]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Battalions_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Battalions_Code] ON [dbo].[Battalions]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Battalions_RangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Battalions_RangeId] ON [dbo].[Battalions]
(
	[RangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BattalionStores_BattalionId_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_BattalionStores_BattalionId_StoreId] ON [dbo].[BattalionStores]
(
	[BattalionId] ASC,
	[StoreId] ASC
)
WHERE ([EffectiveTo] IS NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BattalionStores_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_BattalionStores_StoreId] ON [dbo].[BattalionStores]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Brands_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Brands_Code] ON [dbo].[Brands]
(
	[Code] ASC
)
WHERE ([Code] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Categories_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories_Code] ON [dbo].[Categories]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConditionCheckItems_ConditionCheckId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ConditionCheckItems_ConditionCheckId] ON [dbo].[ConditionCheckItems]
(
	[ConditionCheckId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConditionCheckItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ConditionCheckItems_ItemId] ON [dbo].[ConditionCheckItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConditionChecks_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ConditionChecks_ItemId] ON [dbo].[ConditionChecks]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConditionChecks_ReturnId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ConditionChecks_ReturnId] ON [dbo].[ConditionChecks]
(
	[ReturnId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CycleCountSchedules_CategoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_CycleCountSchedules_CategoryId] ON [dbo].[CycleCountSchedules]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CycleCountSchedules_NextScheduledDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_CycleCountSchedules_NextScheduledDate] ON [dbo].[CycleCountSchedules]
(
	[NextScheduledDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CycleCountSchedules_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_CycleCountSchedules_StoreId] ON [dbo].[CycleCountSchedules]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DamageRecords_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DamageRecords_ItemId] ON [dbo].[DamageRecords]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DamageRecords_ReturnId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DamageRecords_ReturnId] ON [dbo].[DamageRecords]
(
	[ReturnId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DamageRecords_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DamageRecords_StoreId] ON [dbo].[DamageRecords]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DamageReportItems_DamageReportId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DamageReportItems_DamageReportId] ON [dbo].[DamageReportItems]
(
	[DamageReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DamageReportItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DamageReportItems_ItemId] ON [dbo].[DamageReportItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DamageReports_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DamageReports_ItemId] ON [dbo].[DamageReports]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DamageReports_ReportNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_DamageReports_ReportNo] ON [dbo].[DamageReports]
(
	[ReportNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DamageReports_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DamageReports_StoreId] ON [dbo].[DamageReports]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Damages_DamageNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Damages_DamageNo] ON [dbo].[Damages]
(
	[DamageNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Damages_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Damages_ItemId] ON [dbo].[Damages]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Damages_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Damages_StoreId] ON [dbo].[Damages]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DigitalSignatures_EntityType_EntityId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DigitalSignatures_EntityType_EntityId] ON [dbo].[DigitalSignatures]
(
	[EntityType] ASC,
	[EntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DigitalSignatures_SignedAt]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DigitalSignatures_SignedAt] ON [dbo].[DigitalSignatures]
(
	[SignedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DigitalSignatures_SignedBy]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DigitalSignatures_SignedBy] ON [dbo].[DigitalSignatures]
(
	[SignedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DigitalSignatures_VerifiedBy]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DigitalSignatures_VerifiedBy] ON [dbo].[DigitalSignatures]
(
	[VerifiedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DisposalRecords_AuthorizedBy]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DisposalRecords_AuthorizedBy] ON [dbo].[DisposalRecords]
(
	[AuthorizedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DisposalRecords_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DisposalRecords_ItemId] ON [dbo].[DisposalRecords]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DisposalRecords_WriteOffId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_DisposalRecords_WriteOffId] ON [dbo].[DisposalRecords]
(
	[WriteOffId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Documents_UploadedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Documents_UploadedByUserId] ON [dbo].[Documents]
(
	[UploadedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpiredRecords_BatchId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpiredRecords_BatchId] ON [dbo].[ExpiredRecords]
(
	[BatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpiredRecords_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpiredRecords_ItemId] ON [dbo].[ExpiredRecords]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpiredRecords_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpiredRecords_StoreId] ON [dbo].[ExpiredRecords]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ExpiryTrackings_DisposedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpiryTrackings_DisposedByUserId] ON [dbo].[ExpiryTrackings]
(
	[DisposedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpiryTrackings_ExpiryDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpiryTrackings_ExpiryDate] ON [dbo].[ExpiryTrackings]
(
	[ExpiryDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpiryTrackings_ItemId_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpiryTrackings_ItemId_StoreId] ON [dbo].[ExpiryTrackings]
(
	[ItemId] ASC,
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpiryTrackings_ItemId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpiryTrackings_ItemId1] ON [dbo].[ExpiryTrackings]
(
	[ItemId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpiryTrackings_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpiryTrackings_StoreId] ON [dbo].[ExpiryTrackings]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_GoodsReceiveItems_GoodsReceiveId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_GoodsReceiveItems_GoodsReceiveId] ON [dbo].[GoodsReceiveItems]
(
	[GoodsReceiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_GoodsReceiveItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_GoodsReceiveItems_ItemId] ON [dbo].[GoodsReceiveItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_GoodsReceives_PurchaseId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_GoodsReceives_PurchaseId] ON [dbo].[GoodsReceives]
(
	[PurchaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_InventoryCycleCountItems_AdjustedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryCycleCountItems_AdjustedByUserId] ON [dbo].[InventoryCycleCountItems]
(
	[AdjustedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_InventoryCycleCountItems_CountedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryCycleCountItems_CountedByUserId] ON [dbo].[InventoryCycleCountItems]
(
	[CountedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryCycleCountItems_CycleCountId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryCycleCountItems_CycleCountId] ON [dbo].[InventoryCycleCountItems]
(
	[CycleCountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryCycleCountItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryCycleCountItems_ItemId] ON [dbo].[InventoryCycleCountItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryCycleCounts_CountDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryCycleCounts_CountDate] ON [dbo].[InventoryCycleCounts]
(
	[CountDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_InventoryCycleCounts_CountNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_InventoryCycleCounts_CountNumber] ON [dbo].[InventoryCycleCounts]
(
	[CountNumber] ASC
)
WHERE ([CountNumber] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryCycleCounts_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryCycleCounts_StoreId] ON [dbo].[InventoryCycleCounts]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_InventoryValuations_CalculatedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryValuations_CalculatedByUserId] ON [dbo].[InventoryValuations]
(
	[CalculatedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryValuations_ItemId_StoreId_ValuationDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryValuations_ItemId_StoreId_ValuationDate] ON [dbo].[InventoryValuations]
(
	[ItemId] ASC,
	[StoreId] ASC,
	[ValuationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryValuations_ItemId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryValuations_ItemId1] ON [dbo].[InventoryValuations]
(
	[ItemId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryValuations_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryValuations_StoreId] ON [dbo].[InventoryValuations]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryValuations_StoreId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryValuations_StoreId1] ON [dbo].[InventoryValuations]
(
	[StoreId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InventoryValuations_ValuationDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_InventoryValuations_ValuationDate] ON [dbo].[InventoryValuations]
(
	[ValuationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IssueItems_IssueId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_IssueItems_IssueId] ON [dbo].[IssueItems]
(
	[IssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IssueItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_IssueItems_ItemId] ON [dbo].[IssueItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IssueItems_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_IssueItems_StoreId] ON [dbo].[IssueItems]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_AllotmentLetterId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_AllotmentLetterId] ON [dbo].[Issues]
(
	[AllotmentLetterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_ApproverSignatureId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_ApproverSignatureId] ON [dbo].[Issues]
(
	[ApproverSignatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Issues_CreatedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_CreatedByUserId] ON [dbo].[Issues]
(
	[CreatedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_FromStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_FromStoreId] ON [dbo].[Issues]
(
	[FromStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_IssueDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_IssueDate] ON [dbo].[Issues]
(
	[IssueDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_IssuedToBattalionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_IssuedToBattalionId] ON [dbo].[Issues]
(
	[IssuedToBattalionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_IssuedToRangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_IssuedToRangeId] ON [dbo].[Issues]
(
	[IssuedToRangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_IssuedToUnionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_IssuedToUnionId] ON [dbo].[Issues]
(
	[IssuedToUnionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_IssuedToUpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_IssuedToUpazilaId] ON [dbo].[Issues]
(
	[IssuedToUpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_IssuedToZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_IssuedToZilaId] ON [dbo].[Issues]
(
	[IssuedToZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Issues_IssueNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Issues_IssueNo] ON [dbo].[Issues]
(
	[IssueNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_IssuerSignatureId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_IssuerSignatureId] ON [dbo].[Issues]
(
	[IssuerSignatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_ParentIssueId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_ParentIssueId] ON [dbo].[Issues]
(
	[ParentIssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issues_ReceiverSignatureId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issues_ReceiverSignatureId] ON [dbo].[Issues]
(
	[ReceiverSignatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IssueVouchers_IssueId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_IssueVouchers_IssueId] ON [dbo].[IssueVouchers]
(
	[IssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_IssueVouchers_VoucherNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_IssueVouchers_VoucherNumber] ON [dbo].[IssueVouchers]
(
	[VoucherNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ItemModels_BrandId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ItemModels_BrandId] ON [dbo].[ItemModels]
(
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Items_BrandId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Items_BrandId] ON [dbo].[Items]
(
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Items_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Items_Code] ON [dbo].[Items]
(
	[Code] ASC
)
WHERE ([Code] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Items_IsActive]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Items_IsActive] ON [dbo].[Items]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Items_ItemCode]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Items_ItemCode] ON [dbo].[Items]
(
	[ItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Items_ItemModelId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Items_ItemModelId] ON [dbo].[Items]
(
	[ItemModelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Items_Name]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Items_Name] ON [dbo].[Items]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Items_SubCategoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Items_SubCategoryId] ON [dbo].[Items]
(
	[SubCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ItemSpecifications_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ItemSpecifications_ItemId] ON [dbo].[ItemSpecifications]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LedgerBooks_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_LedgerBooks_StoreId] ON [dbo].[LedgerBooks]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Locations_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Locations_Code] ON [dbo].[Locations]
(
	[Code] ASC
)
WHERE ([Code] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Locations_ParentLocationId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Locations_ParentLocationId] ON [dbo].[Locations]
(
	[ParentLocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_LoginLogs_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_LoginLogs_UserId] ON [dbo].[LoginLogs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Notifications_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Notifications_UserId] ON [dbo].[Notifications]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_BattalionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_BattalionId] ON [dbo].[PersonnelItemIssues]
(
	[BattalionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_ItemId] ON [dbo].[PersonnelItemIssues]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_LifeExpiryDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_LifeExpiryDate] ON [dbo].[PersonnelItemIssues]
(
	[LifeExpiryDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_OriginalIssueId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_OriginalIssueId] ON [dbo].[PersonnelItemIssues]
(
	[OriginalIssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PersonnelItemIssues_PersonnelBadgeNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_PersonnelBadgeNo] ON [dbo].[PersonnelItemIssues]
(
	[PersonnelBadgeNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_RangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_RangeId] ON [dbo].[PersonnelItemIssues]
(
	[RangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_ReceiveId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_ReceiveId] ON [dbo].[PersonnelItemIssues]
(
	[ReceiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PersonnelItemIssues_Status]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_Status] ON [dbo].[PersonnelItemIssues]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_StoreId] ON [dbo].[PersonnelItemIssues]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_UpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_UpazilaId] ON [dbo].[PersonnelItemIssues]
(
	[UpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PersonnelItemIssues_ZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PersonnelItemIssues_ZilaId] ON [dbo].[PersonnelItemIssues]
(
	[ZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventories_BattalionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventories_BattalionId] ON [dbo].[PhysicalInventories]
(
	[BattalionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventories_CategoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventories_CategoryId] ON [dbo].[PhysicalInventories]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PhysicalInventories_CountedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventories_CountedByUserId] ON [dbo].[PhysicalInventories]
(
	[CountedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PhysicalInventories_CountNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_PhysicalInventories_CountNo] ON [dbo].[PhysicalInventories]
(
	[CountNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventories_RangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventories_RangeId] ON [dbo].[PhysicalInventories]
(
	[RangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventories_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventories_StoreId] ON [dbo].[PhysicalInventories]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventories_UpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventories_UpazilaId] ON [dbo].[PhysicalInventories]
(
	[UpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PhysicalInventories_VerifiedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventories_VerifiedByUserId] ON [dbo].[PhysicalInventories]
(
	[VerifiedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventories_ZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventories_ZilaId] ON [dbo].[PhysicalInventories]
(
	[ZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventoryDetails_CategoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventoryDetails_CategoryId] ON [dbo].[PhysicalInventoryDetails]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventoryDetails_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventoryDetails_ItemId] ON [dbo].[PhysicalInventoryDetails]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventoryDetails_PhysicalInventoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventoryDetails_PhysicalInventoryId] ON [dbo].[PhysicalInventoryDetails]
(
	[PhysicalInventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventoryItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventoryItems_ItemId] ON [dbo].[PhysicalInventoryItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PhysicalInventoryItems_PhysicalInventoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PhysicalInventoryItems_PhysicalInventoryId] ON [dbo].[PhysicalInventoryItems]
(
	[PhysicalInventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_ItemId] ON [dbo].[PurchaseItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseItems_PurchaseId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_PurchaseId] ON [dbo].[PurchaseItems]
(
	[PurchaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseItems_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_StoreId] ON [dbo].[PurchaseItems]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseOrderItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderItems_ItemId] ON [dbo].[PurchaseOrderItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseOrderItems_PurchaseOrderId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderItems_PurchaseOrderId] ON [dbo].[PurchaseOrderItems]
(
	[PurchaseOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseOrders_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_StoreId] ON [dbo].[PurchaseOrders]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseOrders_VendorId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_VendorId] ON [dbo].[PurchaseOrders]
(
	[VendorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Purchases_CreatedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Purchases_CreatedByUserId] ON [dbo].[Purchases]
(
	[CreatedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Purchases_PurchaseOrderNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Purchases_PurchaseOrderNo] ON [dbo].[Purchases]
(
	[PurchaseOrderNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Purchases_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Purchases_StoreId] ON [dbo].[Purchases]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Purchases_VendorId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Purchases_VendorId] ON [dbo].[Purchases]
(
	[VendorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QualityCheckItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_QualityCheckItems_ItemId] ON [dbo].[QualityCheckItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QualityCheckItems_QualityCheckId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_QualityCheckItems_QualityCheckId] ON [dbo].[QualityCheckItems]
(
	[QualityCheckId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QualityChecks_CheckDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_QualityChecks_CheckDate] ON [dbo].[QualityChecks]
(
	[CheckDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_QualityChecks_CheckedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_QualityChecks_CheckedByUserId] ON [dbo].[QualityChecks]
(
	[CheckedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_QualityChecks_CheckNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_QualityChecks_CheckNumber] ON [dbo].[QualityChecks]
(
	[CheckNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QualityChecks_GoodsReceiveId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_QualityChecks_GoodsReceiveId] ON [dbo].[QualityChecks]
(
	[GoodsReceiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QualityChecks_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_QualityChecks_ItemId] ON [dbo].[QualityChecks]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QualityChecks_PurchaseId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_QualityChecks_PurchaseId] ON [dbo].[QualityChecks]
(
	[PurchaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Ranges_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Ranges_Code] ON [dbo].[Ranges]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReceiveItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ReceiveItems_ItemId] ON [dbo].[ReceiveItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReceiveItems_ItemId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ReceiveItems_ItemId1] ON [dbo].[ReceiveItems]
(
	[ItemId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReceiveItems_ReceiveId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ReceiveItems_ReceiveId] ON [dbo].[ReceiveItems]
(
	[ReceiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReceiveItems_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ReceiveItems_StoreId] ON [dbo].[ReceiveItems]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Receives_OriginalIssueId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Receives_OriginalIssueId] ON [dbo].[Receives]
(
	[OriginalIssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Receives_ReceivedFromBattalionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Receives_ReceivedFromBattalionId] ON [dbo].[Receives]
(
	[ReceivedFromBattalionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Receives_ReceivedFromRangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Receives_ReceivedFromRangeId] ON [dbo].[Receives]
(
	[ReceivedFromRangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Receives_ReceivedFromUnionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Receives_ReceivedFromUnionId] ON [dbo].[Receives]
(
	[ReceivedFromUnionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Receives_ReceivedFromUpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Receives_ReceivedFromUpazilaId] ON [dbo].[Receives]
(
	[ReceivedFromUpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Receives_ReceivedFromZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Receives_ReceivedFromZilaId] ON [dbo].[Receives]
(
	[ReceivedFromZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Receives_ReceiveNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Receives_ReceiveNo] ON [dbo].[Receives]
(
	[ReceiveNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Receives_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Receives_StoreId] ON [dbo].[Receives]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RequisitionItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_RequisitionItems_ItemId] ON [dbo].[RequisitionItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RequisitionItems_RequisitionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_RequisitionItems_RequisitionId] ON [dbo].[RequisitionItems]
(
	[RequisitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Requisitions_ApprovedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Requisitions_ApprovedByUserId] ON [dbo].[Requisitions]
(
	[ApprovedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Requisitions_FromStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Requisitions_FromStoreId] ON [dbo].[Requisitions]
(
	[FromStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Requisitions_PurchaseOrderId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Requisitions_PurchaseOrderId] ON [dbo].[Requisitions]
(
	[PurchaseOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Requisitions_RequestedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Requisitions_RequestedByUserId] ON [dbo].[Requisitions]
(
	[RequestedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Requisitions_RequisitionNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Requisitions_RequisitionNumber] ON [dbo].[Requisitions]
(
	[RequisitionNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Requisitions_ToStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Requisitions_ToStoreId] ON [dbo].[Requisitions]
(
	[ToStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReturnItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ReturnItems_ItemId] ON [dbo].[ReturnItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReturnItems_ReturnId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_ReturnItems_ReturnId] ON [dbo].[ReturnItems]
(
	[ReturnId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Returns_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Returns_ItemId] ON [dbo].[Returns]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Returns_OriginalIssueId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Returns_OriginalIssueId] ON [dbo].[Returns]
(
	[OriginalIssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Returns_ReturnNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Returns_ReturnNo] ON [dbo].[Returns]
(
	[ReturnNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Returns_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Returns_StoreId] ON [dbo].[Returns]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Returns_ToStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Returns_ToStoreId] ON [dbo].[Returns]
(
	[ToStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ShipmentTrackings_TrackingCode]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ShipmentTrackings_TrackingCode] ON [dbo].[ShipmentTrackings]
(
	[TrackingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SignatureOTPs_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_SignatureOTPs_UserId] ON [dbo].[SignatureOTPs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Signatures_IssueId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Signatures_IssueId] ON [dbo].[Signatures]
(
	[IssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Signatures_ReferenceType_ReferenceId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Signatures_ReferenceType_ReferenceId] ON [dbo].[Signatures]
(
	[ReferenceType] ASC,
	[ReferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAdjustmentItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustmentItems_ItemId] ON [dbo].[StockAdjustmentItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAdjustmentItems_StockAdjustmentId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustmentItems_StockAdjustmentId] ON [dbo].[StockAdjustmentItems]
(
	[StockAdjustmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAdjustments_AdjustmentDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustments_AdjustmentDate] ON [dbo].[StockAdjustments]
(
	[AdjustmentDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockAdjustments_AdjustmentNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StockAdjustments_AdjustmentNo] ON [dbo].[StockAdjustments]
(
	[AdjustmentNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockAdjustments_ApprovedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustments_ApprovedByUserId] ON [dbo].[StockAdjustments]
(
	[ApprovedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAdjustments_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustments_ItemId] ON [dbo].[StockAdjustments]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAdjustments_ItemId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustments_ItemId1] ON [dbo].[StockAdjustments]
(
	[ItemId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAdjustments_PhysicalInventoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustments_PhysicalInventoryId] ON [dbo].[StockAdjustments]
(
	[PhysicalInventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAdjustments_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustments_StoreId] ON [dbo].[StockAdjustments]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAdjustments_StoreId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAdjustments_StoreId1] ON [dbo].[StockAdjustments]
(
	[StoreId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAlerts_AlertDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAlerts_AlertDate] ON [dbo].[StockAlerts]
(
	[AlertDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAlerts_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAlerts_ItemId] ON [dbo].[StockAlerts]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockAlerts_Status]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAlerts_Status] ON [dbo].[StockAlerts]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockAlerts_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockAlerts_StoreId] ON [dbo].[StockAlerts]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockEntries_EntryDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockEntries_EntryDate] ON [dbo].[StockEntries]
(
	[EntryDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockEntries_EntryNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StockEntries_EntryNo] ON [dbo].[StockEntries]
(
	[EntryNo] ASC
)
WHERE ([EntryNo] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockEntries_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockEntries_StoreId] ON [dbo].[StockEntries]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockEntryItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockEntryItems_ItemId] ON [dbo].[StockEntryItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockEntryItems_StockEntryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockEntryItems_StockEntryId] ON [dbo].[StockEntryItems]
(
	[StockEntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_DestinationStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_DestinationStoreId] ON [dbo].[StockMovements]
(
	[DestinationStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_ItemId_MovementDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_ItemId_MovementDate] ON [dbo].[StockMovements]
(
	[ItemId] ASC,
	[MovementDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockMovements_MovedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_MovedByUserId] ON [dbo].[StockMovements]
(
	[MovedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_SourceStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_SourceStoreId] ON [dbo].[StockMovements]
(
	[SourceStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_StoreId] ON [dbo].[StockMovements]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockMovements_StoreItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockMovements_StoreItemId] ON [dbo].[StockMovements]
(
	[StoreItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockOperations_CreatedAt]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockOperations_CreatedAt] ON [dbo].[StockOperations]
(
	[CreatedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockOperations_FromStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockOperations_FromStoreId] ON [dbo].[StockOperations]
(
	[FromStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockOperations_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockOperations_ItemId] ON [dbo].[StockOperations]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockOperations_ToStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockOperations_ToStoreId] ON [dbo].[StockOperations]
(
	[ToStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockReturns_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockReturns_ItemId] ON [dbo].[StockReturns]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockReturns_OriginalIssueId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockReturns_OriginalIssueId] ON [dbo].[StockReturns]
(
	[OriginalIssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StockReturns_ReturnDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StockReturns_ReturnDate] ON [dbo].[StockReturns]
(
	[ReturnDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StockReturns_ReturnNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StockReturns_ReturnNumber] ON [dbo].[StockReturns]
(
	[ReturnNumber] ASC
)
WHERE ([ReturnNumber] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreConfigurations_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StoreConfigurations_StoreId] ON [dbo].[StoreConfigurations]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StoreItems_ItemId] ON [dbo].[StoreItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreItems_StoreId_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StoreItems_StoreId_ItemId] ON [dbo].[StoreItems]
(
	[StoreId] ASC,
	[ItemId] ASC
)
WHERE ([StoreId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stores_BattalionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_BattalionId] ON [dbo].[Stores]
(
	[BattalionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Stores_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Stores_Code] ON [dbo].[Stores]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stores_IsActive]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_IsActive] ON [dbo].[Stores]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stores_LocationId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_LocationId] ON [dbo].[Stores]
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Stores_Name]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_Name] ON [dbo].[Stores]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stores_RangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_RangeId] ON [dbo].[Stores]
(
	[RangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stores_StoreTypeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_StoreTypeId] ON [dbo].[Stores]
(
	[StoreTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stores_UnionId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_UnionId] ON [dbo].[Stores]
(
	[UnionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stores_UpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_UpazilaId] ON [dbo].[Stores]
(
	[UpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stores_ZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stores_ZilaId] ON [dbo].[Stores]
(
	[ZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreStocks_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StoreStocks_ItemId] ON [dbo].[StoreStocks]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreStocks_ItemId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StoreStocks_ItemId1] ON [dbo].[StoreStocks]
(
	[ItemId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreStocks_StoreId_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StoreStocks_StoreId_ItemId] ON [dbo].[StoreStocks]
(
	[StoreId] ASC,
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreStocks_StoreId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StoreStocks_StoreId1] ON [dbo].[StoreStocks]
(
	[StoreId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreTypeCategories_CategoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StoreTypeCategories_CategoryId] ON [dbo].[StoreTypeCategories]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreTypeCategories_StoreTypeId_CategoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StoreTypeCategories_StoreTypeId_CategoryId] ON [dbo].[StoreTypeCategories]
(
	[StoreTypeId] ASC,
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StoreTypeCategories_StoreTypeId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_StoreTypeCategories_StoreTypeId1] ON [dbo].[StoreTypeCategories]
(
	[StoreTypeId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_StoreTypes_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StoreTypes_Code] ON [dbo].[StoreTypes]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubCategories_CategoryId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubCategories_CategoryId] ON [dbo].[SubCategories]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SubCategories_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_SubCategories_Code] ON [dbo].[SubCategories]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SupplierEvaluations_ApprovedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierEvaluations_ApprovedByUserId] ON [dbo].[SupplierEvaluations]
(
	[ApprovedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SupplierEvaluations_EvaluatedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierEvaluations_EvaluatedByUserId] ON [dbo].[SupplierEvaluations]
(
	[EvaluatedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SupplierEvaluations_EvaluationDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierEvaluations_EvaluationDate] ON [dbo].[SupplierEvaluations]
(
	[EvaluationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SupplierEvaluations_VendorId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierEvaluations_VendorId] ON [dbo].[SupplierEvaluations]
(
	[VendorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SystemConfigurations_ModifiedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_SystemConfigurations_ModifiedByUserId] ON [dbo].[SystemConfigurations]
(
	[ModifiedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TemperatureLogs_LogTime]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TemperatureLogs_LogTime] ON [dbo].[TemperatureLogs]
(
	[LogTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TemperatureLogs_RecordedByUserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TemperatureLogs_RecordedByUserId] ON [dbo].[TemperatureLogs]
(
	[RecordedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TemperatureLogs_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TemperatureLogs_StoreId] ON [dbo].[TemperatureLogs]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TemperatureLogs_StoreId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TemperatureLogs_StoreId1] ON [dbo].[TemperatureLogs]
(
	[StoreId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrackingHistories_LastUpdated]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TrackingHistories_LastUpdated] ON [dbo].[TrackingHistories]
(
	[LastUpdated] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrackingHistories_ShipmentTrackingId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TrackingHistories_ShipmentTrackingId] ON [dbo].[TrackingHistories]
(
	[ShipmentTrackingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransferDiscrepancies_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransferDiscrepancies_ItemId] ON [dbo].[TransferDiscrepancies]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransferDiscrepancies_TransferId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransferDiscrepancies_TransferId] ON [dbo].[TransferDiscrepancies]
(
	[TransferId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransferItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransferItems_ItemId] ON [dbo].[TransferItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransferItems_TransferId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransferItems_TransferId] ON [dbo].[TransferItems]
(
	[TransferId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Transfers_FromStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Transfers_FromStoreId] ON [dbo].[Transfers]
(
	[FromStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Transfers_ToStoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Transfers_ToStoreId] ON [dbo].[Transfers]
(
	[ToStoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Transfers_TransferDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Transfers_TransferDate] ON [dbo].[Transfers]
(
	[TransferDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transfers_TransferNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Transfers_TransferNo] ON [dbo].[Transfers]
(
	[TransferNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransferShipmentItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransferShipmentItems_ItemId] ON [dbo].[TransferShipmentItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransferShipmentItems_ShipmentId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransferShipmentItems_ShipmentId] ON [dbo].[TransferShipmentItems]
(
	[ShipmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransferShipments_ShipmentNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TransferShipments_ShipmentNo] ON [dbo].[TransferShipments]
(
	[ShipmentNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransferShipments_TransferId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TransferShipments_TransferId] ON [dbo].[TransferShipments]
(
	[TransferId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Unions_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Unions_Code] ON [dbo].[Unions]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Unions_UpazilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Unions_UpazilaId] ON [dbo].[Unions]
(
	[UpazilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Units_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Units_Code] ON [dbo].[Units]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Upazilas_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Upazilas_Code] ON [dbo].[Upazilas]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Upazilas_ZilaId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Upazilas_ZilaId] ON [dbo].[Upazilas]
(
	[ZilaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserNotificationPreferences_UserId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserNotificationPreferences_UserId] ON [dbo].[UserNotificationPreferences]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserStores_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserStores_StoreId] ON [dbo].[UserStores]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserStores_UserId_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserStores_UserId_StoreId] ON [dbo].[UserStores]
(
	[UserId] ASC,
	[StoreId] ASC
)
WHERE ([UnassignedDate] IS NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Vendors_Email]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Vendors_Email] ON [dbo].[Vendors]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Vendors_Name]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Vendors_Name] ON [dbo].[Vendors]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Warranties_EndDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Warranties_EndDate] ON [dbo].[Warranties]
(
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Warranties_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Warranties_ItemId] ON [dbo].[Warranties]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Warranties_ItemId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Warranties_ItemId1] ON [dbo].[Warranties]
(
	[ItemId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Warranties_VendorId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Warranties_VendorId] ON [dbo].[Warranties]
(
	[VendorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Warranties_WarrantyNumber]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Warranties_WarrantyNumber] ON [dbo].[Warranties]
(
	[WarrantyNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffItems_ItemId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffItems_ItemId] ON [dbo].[WriteOffItems]
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffItems_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffItems_StoreId] ON [dbo].[WriteOffItems]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffItems_WriteOffId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffItems_WriteOffId] ON [dbo].[WriteOffItems]
(
	[WriteOffId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffItems_WriteOffRequestId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffItems_WriteOffRequestId] ON [dbo].[WriteOffItems]
(
	[WriteOffRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffItems_WriteOffRequestId1]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffItems_WriteOffRequestId1] ON [dbo].[WriteOffItems]
(
	[WriteOffRequestId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffRequests_DamageReportId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffRequests_DamageReportId] ON [dbo].[WriteOffRequests]
(
	[DamageReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_WriteOffRequests_RequestedBy]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffRequests_RequestedBy] ON [dbo].[WriteOffRequests]
(
	[RequestedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_WriteOffRequests_RequestNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_WriteOffRequests_RequestNo] ON [dbo].[WriteOffRequests]
(
	[RequestNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffRequests_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffRequests_StoreId] ON [dbo].[WriteOffRequests]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffs_StoreId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffs_StoreId] ON [dbo].[WriteOffs]
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffs_WriteOffDate]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffs_WriteOffDate] ON [dbo].[WriteOffs]
(
	[WriteOffDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_WriteOffs_WriteOffNo]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_WriteOffs_WriteOffNo] ON [dbo].[WriteOffs]
(
	[WriteOffNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Zilas_Code]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Zilas_Code] ON [dbo].[Zilas]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Zilas_RangeId]    Script Date: 11/6/2025 1:16:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_Zilas_RangeId] ON [dbo].[Zilas]
(
	[RangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ApprovalLevels] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsSystemDefined]
GO
ALTER TABLE [dbo].[ActivityLogs]  WITH CHECK ADD  CONSTRAINT [FK_ActivityLogs_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ActivityLogs] CHECK CONSTRAINT [FK_ActivityLogs_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AllotmentLetterItems]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterItems_AllotmentLetters_AllotmentLetterId] FOREIGN KEY([AllotmentLetterId])
REFERENCES [dbo].[AllotmentLetters] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AllotmentLetterItems] CHECK CONSTRAINT [FK_AllotmentLetterItems_AllotmentLetters_AllotmentLetterId]
GO
ALTER TABLE [dbo].[AllotmentLetterItems]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetterItems] CHECK CONSTRAINT [FK_AllotmentLetterItems_Items_ItemId]
GO
ALTER TABLE [dbo].[AllotmentLetterRecipientItems]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterRecipientItems_AllotmentLetterRecipients_AllotmentLetterRecipientId] FOREIGN KEY([AllotmentLetterRecipientId])
REFERENCES [dbo].[AllotmentLetterRecipients] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AllotmentLetterRecipientItems] CHECK CONSTRAINT [FK_AllotmentLetterRecipientItems_AllotmentLetterRecipients_AllotmentLetterRecipientId]
GO
ALTER TABLE [dbo].[AllotmentLetterRecipientItems]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterRecipientItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AllotmentLetterRecipientItems] CHECK CONSTRAINT [FK_AllotmentLetterRecipientItems_Items_ItemId]
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterRecipients_AllotmentLetters_AllotmentLetterId] FOREIGN KEY([AllotmentLetterId])
REFERENCES [dbo].[AllotmentLetters] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients] CHECK CONSTRAINT [FK_AllotmentLetterRecipients_AllotmentLetters_AllotmentLetterId]
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterRecipients_Battalions_BattalionId] FOREIGN KEY([BattalionId])
REFERENCES [dbo].[Battalions] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients] CHECK CONSTRAINT [FK_AllotmentLetterRecipients_Battalions_BattalionId]
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterRecipients_Ranges_RangeId] FOREIGN KEY([RangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients] CHECK CONSTRAINT [FK_AllotmentLetterRecipients_Ranges_RangeId]
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterRecipients_Unions_UnionId] FOREIGN KEY([UnionId])
REFERENCES [dbo].[Unions] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients] CHECK CONSTRAINT [FK_AllotmentLetterRecipients_Unions_UnionId]
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterRecipients_Upazilas_UpazilaId] FOREIGN KEY([UpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients] CHECK CONSTRAINT [FK_AllotmentLetterRecipients_Upazilas_UpazilaId]
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetterRecipients_Zilas_ZilaId] FOREIGN KEY([ZilaId])
REFERENCES [dbo].[Zilas] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetterRecipients] CHECK CONSTRAINT [FK_AllotmentLetterRecipients_Zilas_ZilaId]
GO
ALTER TABLE [dbo].[AllotmentLetters]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetters_Battalions_IssuedToBattalionId] FOREIGN KEY([IssuedToBattalionId])
REFERENCES [dbo].[Battalions] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetters] CHECK CONSTRAINT [FK_AllotmentLetters_Battalions_IssuedToBattalionId]
GO
ALTER TABLE [dbo].[AllotmentLetters]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetters_Ranges_IssuedToRangeId] FOREIGN KEY([IssuedToRangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetters] CHECK CONSTRAINT [FK_AllotmentLetters_Ranges_IssuedToRangeId]
GO
ALTER TABLE [dbo].[AllotmentLetters]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetters_Stores_FromStoreId] FOREIGN KEY([FromStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetters] CHECK CONSTRAINT [FK_AllotmentLetters_Stores_FromStoreId]
GO
ALTER TABLE [dbo].[AllotmentLetters]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetters_Upazilas_IssuedToUpazilaId] FOREIGN KEY([IssuedToUpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetters] CHECK CONSTRAINT [FK_AllotmentLetters_Upazilas_IssuedToUpazilaId]
GO
ALTER TABLE [dbo].[AllotmentLetters]  WITH CHECK ADD  CONSTRAINT [FK_AllotmentLetters_Zilas_IssuedToZilaId] FOREIGN KEY([IssuedToZilaId])
REFERENCES [dbo].[Zilas] ([Id])
GO
ALTER TABLE [dbo].[AllotmentLetters] CHECK CONSTRAINT [FK_AllotmentLetters_Zilas_IssuedToZilaId]
GO
ALTER TABLE [dbo].[ApprovalDelegations]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalDelegations_AspNetUsers_FromUserId] FOREIGN KEY([FromUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ApprovalDelegations] CHECK CONSTRAINT [FK_ApprovalDelegations_AspNetUsers_FromUserId]
GO
ALTER TABLE [dbo].[ApprovalDelegations]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalDelegations_AspNetUsers_ToUserId] FOREIGN KEY([ToUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ApprovalDelegations] CHECK CONSTRAINT [FK_ApprovalDelegations_AspNetUsers_ToUserId]
GO
ALTER TABLE [dbo].[ApprovalHistories]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalHistories_ApprovalRequests_ApprovalRequestId] FOREIGN KEY([ApprovalRequestId])
REFERENCES [dbo].[ApprovalRequests] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApprovalHistories] CHECK CONSTRAINT [FK_ApprovalHistories_ApprovalRequests_ApprovalRequestId]
GO
ALTER TABLE [dbo].[ApprovalRequests]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalRequests_AspNetUsers_ApprovedByUserId] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ApprovalRequests] CHECK CONSTRAINT [FK_ApprovalRequests_AspNetUsers_ApprovedByUserId]
GO
ALTER TABLE [dbo].[ApprovalRequests]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalRequests_AspNetUsers_RequestedByUserId] FOREIGN KEY([RequestedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ApprovalRequests] CHECK CONSTRAINT [FK_ApprovalRequests_AspNetUsers_RequestedByUserId]
GO
ALTER TABLE [dbo].[ApprovalSteps]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalSteps_ApprovalRequests_ApprovalRequestId] FOREIGN KEY([ApprovalRequestId])
REFERENCES [dbo].[ApprovalRequests] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApprovalSteps] CHECK CONSTRAINT [FK_ApprovalSteps_ApprovalRequests_ApprovalRequestId]
GO
ALTER TABLE [dbo].[ApprovalWorkflowLevels]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalWorkflowLevels_ApprovalWorkflows_WorkflowId] FOREIGN KEY([WorkflowId])
REFERENCES [dbo].[ApprovalWorkflows] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApprovalWorkflowLevels] CHECK CONSTRAINT [FK_ApprovalWorkflowLevels_ApprovalWorkflows_WorkflowId]
GO
ALTER TABLE [dbo].[ApprovalWorkflows]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalWorkflows_AspNetUsers_ApproverUserId] FOREIGN KEY([ApproverUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ApprovalWorkflows] CHECK CONSTRAINT [FK_ApprovalWorkflows_AspNetUsers_ApproverUserId]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId1] FOREIGN KEY([UserId1])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId1]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Battalions_BattalionId] FOREIGN KEY([BattalionId])
REFERENCES [dbo].[Battalions] ([Id])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Battalions_BattalionId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Ranges_RangeId] FOREIGN KEY([RangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Ranges_RangeId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Unions_UnionId] FOREIGN KEY([UnionId])
REFERENCES [dbo].[Unions] ([Id])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Unions_UnionId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Upazilas_UpazilaId] FOREIGN KEY([UpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Upazilas_UpazilaId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Zilas_ZilaId] FOREIGN KEY([ZilaId])
REFERENCES [dbo].[Zilas] ([Id])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Zilas_ZilaId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AuditLogs] CHECK CONSTRAINT [FK_AuditLogs_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AuditReports]  WITH CHECK ADD  CONSTRAINT [FK_AuditReports_PhysicalInventories_PhysicalInventoryId] FOREIGN KEY([PhysicalInventoryId])
REFERENCES [dbo].[PhysicalInventories] ([Id])
GO
ALTER TABLE [dbo].[AuditReports] CHECK CONSTRAINT [FK_AuditReports_PhysicalInventories_PhysicalInventoryId]
GO
ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_Audits_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_Audits_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Barcodes]  WITH CHECK ADD  CONSTRAINT [FK_Barcodes_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[Barcodes] CHECK CONSTRAINT [FK_Barcodes_Items_ItemId]
GO
ALTER TABLE [dbo].[Barcodes]  WITH CHECK ADD  CONSTRAINT [FK_Barcodes_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Barcodes] CHECK CONSTRAINT [FK_Barcodes_Stores_StoreId]
GO
ALTER TABLE [dbo].[BatchMovements]  WITH CHECK ADD  CONSTRAINT [FK_BatchMovement_BatchTracking_BatchId] FOREIGN KEY([BatchId])
REFERENCES [dbo].[BatchTrackings] ([Id])
GO
ALTER TABLE [dbo].[BatchMovements] CHECK CONSTRAINT [FK_BatchMovement_BatchTracking_BatchId]
GO
ALTER TABLE [dbo].[BatchMovements]  WITH CHECK ADD  CONSTRAINT [FK_BatchMovement_BatchTracking_BatchTrackingId] FOREIGN KEY([BatchTrackingId])
REFERENCES [dbo].[BatchTrackings] ([Id])
GO
ALTER TABLE [dbo].[BatchMovements] CHECK CONSTRAINT [FK_BatchMovement_BatchTracking_BatchTrackingId]
GO
ALTER TABLE [dbo].[BatchSignatureItem]  WITH CHECK ADD  CONSTRAINT [FK_BatchSignatureItem_BatchSignatures_BatchSignatureId] FOREIGN KEY([BatchSignatureId])
REFERENCES [dbo].[BatchSignatures] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BatchSignatureItem] CHECK CONSTRAINT [FK_BatchSignatureItem_BatchSignatures_BatchSignatureId]
GO
ALTER TABLE [dbo].[BatchSignatures]  WITH CHECK ADD  CONSTRAINT [FK_BatchSignatures_AspNetUsers_SignedBy] FOREIGN KEY([SignedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[BatchSignatures] CHECK CONSTRAINT [FK_BatchSignatures_AspNetUsers_SignedBy]
GO
ALTER TABLE [dbo].[BatchSignatures]  WITH CHECK ADD  CONSTRAINT [FK_BatchSignatures_BatchTrackings_BatchId] FOREIGN KEY([BatchId])
REFERENCES [dbo].[BatchTrackings] ([Id])
GO
ALTER TABLE [dbo].[BatchSignatures] CHECK CONSTRAINT [FK_BatchSignatures_BatchTrackings_BatchId]
GO
ALTER TABLE [dbo].[BatchTrackings]  WITH CHECK ADD  CONSTRAINT [FK_BatchTrackings_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BatchTrackings] CHECK CONSTRAINT [FK_BatchTrackings_Items_ItemId]
GO
ALTER TABLE [dbo].[BatchTrackings]  WITH CHECK ADD  CONSTRAINT [FK_BatchTrackings_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BatchTrackings] CHECK CONSTRAINT [FK_BatchTrackings_Stores_StoreId]
GO
ALTER TABLE [dbo].[Battalions]  WITH CHECK ADD  CONSTRAINT [FK_Battalions_Ranges_RangeId] FOREIGN KEY([RangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[Battalions] CHECK CONSTRAINT [FK_Battalions_Ranges_RangeId]
GO
ALTER TABLE [dbo].[BattalionStores]  WITH CHECK ADD  CONSTRAINT [FK_BattalionStores_Battalions_BattalionId] FOREIGN KEY([BattalionId])
REFERENCES [dbo].[Battalions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BattalionStores] CHECK CONSTRAINT [FK_BattalionStores_Battalions_BattalionId]
GO
ALTER TABLE [dbo].[BattalionStores]  WITH CHECK ADD  CONSTRAINT [FK_BattalionStores_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[BattalionStores] CHECK CONSTRAINT [FK_BattalionStores_Stores_StoreId]
GO
ALTER TABLE [dbo].[ConditionCheckItems]  WITH CHECK ADD  CONSTRAINT [FK_ConditionCheckItems_ConditionChecks_ConditionCheckId] FOREIGN KEY([ConditionCheckId])
REFERENCES [dbo].[ConditionChecks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ConditionCheckItems] CHECK CONSTRAINT [FK_ConditionCheckItems_ConditionChecks_ConditionCheckId]
GO
ALTER TABLE [dbo].[ConditionCheckItems]  WITH CHECK ADD  CONSTRAINT [FK_ConditionCheckItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[ConditionCheckItems] CHECK CONSTRAINT [FK_ConditionCheckItems_Items_ItemId]
GO
ALTER TABLE [dbo].[ConditionChecks]  WITH CHECK ADD  CONSTRAINT [FK_ConditionChecks_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[ConditionChecks] CHECK CONSTRAINT [FK_ConditionChecks_Items_ItemId]
GO
ALTER TABLE [dbo].[ConditionChecks]  WITH CHECK ADD  CONSTRAINT [FK_ConditionChecks_Returns_ReturnId] FOREIGN KEY([ReturnId])
REFERENCES [dbo].[Returns] ([Id])
GO
ALTER TABLE [dbo].[ConditionChecks] CHECK CONSTRAINT [FK_ConditionChecks_Returns_ReturnId]
GO
ALTER TABLE [dbo].[CycleCountSchedules]  WITH CHECK ADD  CONSTRAINT [FK_CycleCountSchedules_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[CycleCountSchedules] CHECK CONSTRAINT [FK_CycleCountSchedules_Categories_CategoryId]
GO
ALTER TABLE [dbo].[CycleCountSchedules]  WITH CHECK ADD  CONSTRAINT [FK_CycleCountSchedules_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[CycleCountSchedules] CHECK CONSTRAINT [FK_CycleCountSchedules_Stores_StoreId]
GO
ALTER TABLE [dbo].[DamageRecords]  WITH CHECK ADD  CONSTRAINT [FK_DamageRecords_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[DamageRecords] CHECK CONSTRAINT [FK_DamageRecords_Items_ItemId]
GO
ALTER TABLE [dbo].[DamageRecords]  WITH CHECK ADD  CONSTRAINT [FK_DamageRecords_Returns_ReturnId] FOREIGN KEY([ReturnId])
REFERENCES [dbo].[Returns] ([Id])
GO
ALTER TABLE [dbo].[DamageRecords] CHECK CONSTRAINT [FK_DamageRecords_Returns_ReturnId]
GO
ALTER TABLE [dbo].[DamageRecords]  WITH CHECK ADD  CONSTRAINT [FK_DamageRecords_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[DamageRecords] CHECK CONSTRAINT [FK_DamageRecords_Stores_StoreId]
GO
ALTER TABLE [dbo].[DamageReportItems]  WITH CHECK ADD  CONSTRAINT [FK_DamageReportItems_DamageReports_DamageReportId] FOREIGN KEY([DamageReportId])
REFERENCES [dbo].[DamageReports] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DamageReportItems] CHECK CONSTRAINT [FK_DamageReportItems_DamageReports_DamageReportId]
GO
ALTER TABLE [dbo].[DamageReportItems]  WITH CHECK ADD  CONSTRAINT [FK_DamageReportItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[DamageReportItems] CHECK CONSTRAINT [FK_DamageReportItems_Items_ItemId]
GO
ALTER TABLE [dbo].[DamageReports]  WITH CHECK ADD  CONSTRAINT [FK_DamageReports_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[DamageReports] CHECK CONSTRAINT [FK_DamageReports_Items_ItemId]
GO
ALTER TABLE [dbo].[DamageReports]  WITH CHECK ADD  CONSTRAINT [FK_DamageReports_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[DamageReports] CHECK CONSTRAINT [FK_DamageReports_Stores_StoreId]
GO
ALTER TABLE [dbo].[Damages]  WITH CHECK ADD  CONSTRAINT [FK_Damages_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[Damages] CHECK CONSTRAINT [FK_Damages_Items_ItemId]
GO
ALTER TABLE [dbo].[Damages]  WITH CHECK ADD  CONSTRAINT [FK_Damages_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Damages] CHECK CONSTRAINT [FK_Damages_Stores_StoreId]
GO
ALTER TABLE [dbo].[DigitalSignatures]  WITH CHECK ADD  CONSTRAINT [FK_DigitalSignatures_AspNetUsers_SignedBy] FOREIGN KEY([SignedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[DigitalSignatures] CHECK CONSTRAINT [FK_DigitalSignatures_AspNetUsers_SignedBy]
GO
ALTER TABLE [dbo].[DigitalSignatures]  WITH CHECK ADD  CONSTRAINT [FK_DigitalSignatures_AspNetUsers_VerifiedBy] FOREIGN KEY([VerifiedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[DigitalSignatures] CHECK CONSTRAINT [FK_DigitalSignatures_AspNetUsers_VerifiedBy]
GO
ALTER TABLE [dbo].[DisposalRecords]  WITH CHECK ADD  CONSTRAINT [FK_DisposalRecords_AspNetUsers_AuthorizedBy] FOREIGN KEY([AuthorizedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[DisposalRecords] CHECK CONSTRAINT [FK_DisposalRecords_AspNetUsers_AuthorizedBy]
GO
ALTER TABLE [dbo].[DisposalRecords]  WITH CHECK ADD  CONSTRAINT [FK_DisposalRecords_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[DisposalRecords] CHECK CONSTRAINT [FK_DisposalRecords_Items_ItemId]
GO
ALTER TABLE [dbo].[DisposalRecords]  WITH CHECK ADD  CONSTRAINT [FK_DisposalRecords_WriteOffRequests_WriteOffId] FOREIGN KEY([WriteOffId])
REFERENCES [dbo].[WriteOffRequests] ([Id])
GO
ALTER TABLE [dbo].[DisposalRecords] CHECK CONSTRAINT [FK_DisposalRecords_WriteOffRequests_WriteOffId]
GO
ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_Documents_AspNetUsers_UploadedByUserId] FOREIGN KEY([UploadedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [FK_Documents_AspNetUsers_UploadedByUserId]
GO
ALTER TABLE [dbo].[ExpiredRecords]  WITH CHECK ADD  CONSTRAINT [FK_ExpiredRecords_BatchTrackings_BatchId] FOREIGN KEY([BatchId])
REFERENCES [dbo].[BatchTrackings] ([Id])
GO
ALTER TABLE [dbo].[ExpiredRecords] CHECK CONSTRAINT [FK_ExpiredRecords_BatchTrackings_BatchId]
GO
ALTER TABLE [dbo].[ExpiredRecords]  WITH CHECK ADD  CONSTRAINT [FK_ExpiredRecords_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[ExpiredRecords] CHECK CONSTRAINT [FK_ExpiredRecords_Items_ItemId]
GO
ALTER TABLE [dbo].[ExpiredRecords]  WITH CHECK ADD  CONSTRAINT [FK_ExpiredRecords_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[ExpiredRecords] CHECK CONSTRAINT [FK_ExpiredRecords_Stores_StoreId]
GO
ALTER TABLE [dbo].[ExpiryTrackings]  WITH CHECK ADD  CONSTRAINT [FK_ExpiryTrackings_AspNetUsers_DisposedByUserId] FOREIGN KEY([DisposedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ExpiryTrackings] CHECK CONSTRAINT [FK_ExpiryTrackings_AspNetUsers_DisposedByUserId]
GO
ALTER TABLE [dbo].[ExpiryTrackings]  WITH CHECK ADD  CONSTRAINT [FK_ExpiryTrackings_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[ExpiryTrackings] CHECK CONSTRAINT [FK_ExpiryTrackings_Items_ItemId]
GO
ALTER TABLE [dbo].[ExpiryTrackings]  WITH CHECK ADD  CONSTRAINT [FK_ExpiryTrackings_Items_ItemId1] FOREIGN KEY([ItemId1])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[ExpiryTrackings] CHECK CONSTRAINT [FK_ExpiryTrackings_Items_ItemId1]
GO
ALTER TABLE [dbo].[ExpiryTrackings]  WITH CHECK ADD  CONSTRAINT [FK_ExpiryTrackings_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[ExpiryTrackings] CHECK CONSTRAINT [FK_ExpiryTrackings_Stores_StoreId]
GO
ALTER TABLE [dbo].[GoodsReceiveItems]  WITH CHECK ADD  CONSTRAINT [FK_GoodsReceiveItems_GoodsReceives_GoodsReceiveId] FOREIGN KEY([GoodsReceiveId])
REFERENCES [dbo].[GoodsReceives] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GoodsReceiveItems] CHECK CONSTRAINT [FK_GoodsReceiveItems_GoodsReceives_GoodsReceiveId]
GO
ALTER TABLE [dbo].[GoodsReceiveItems]  WITH CHECK ADD  CONSTRAINT [FK_GoodsReceiveItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GoodsReceiveItems] CHECK CONSTRAINT [FK_GoodsReceiveItems_Items_ItemId]
GO
ALTER TABLE [dbo].[GoodsReceives]  WITH CHECK ADD  CONSTRAINT [FK_GoodsReceives_Purchases_PurchaseId] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchases] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GoodsReceives] CHECK CONSTRAINT [FK_GoodsReceives_Purchases_PurchaseId]
GO
ALTER TABLE [dbo].[InventoryCycleCountItems]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCycleCountItems_AspNetUsers_AdjustedByUserId] FOREIGN KEY([AdjustedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[InventoryCycleCountItems] CHECK CONSTRAINT [FK_InventoryCycleCountItems_AspNetUsers_AdjustedByUserId]
GO
ALTER TABLE [dbo].[InventoryCycleCountItems]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCycleCountItems_AspNetUsers_CountedByUserId] FOREIGN KEY([CountedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[InventoryCycleCountItems] CHECK CONSTRAINT [FK_InventoryCycleCountItems_AspNetUsers_CountedByUserId]
GO
ALTER TABLE [dbo].[InventoryCycleCountItems]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCycleCountItems_InventoryCycleCounts_CycleCountId] FOREIGN KEY([CycleCountId])
REFERENCES [dbo].[InventoryCycleCounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryCycleCountItems] CHECK CONSTRAINT [FK_InventoryCycleCountItems_InventoryCycleCounts_CycleCountId]
GO
ALTER TABLE [dbo].[InventoryCycleCountItems]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCycleCountItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[InventoryCycleCountItems] CHECK CONSTRAINT [FK_InventoryCycleCountItems_Items_ItemId]
GO
ALTER TABLE [dbo].[InventoryCycleCounts]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCycleCounts_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[InventoryCycleCounts] CHECK CONSTRAINT [FK_InventoryCycleCounts_Stores_StoreId]
GO
ALTER TABLE [dbo].[InventoryValuations]  WITH CHECK ADD  CONSTRAINT [FK_InventoryValuations_AspNetUsers_CalculatedByUserId] FOREIGN KEY([CalculatedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[InventoryValuations] CHECK CONSTRAINT [FK_InventoryValuations_AspNetUsers_CalculatedByUserId]
GO
ALTER TABLE [dbo].[InventoryValuations]  WITH CHECK ADD  CONSTRAINT [FK_InventoryValuations_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[InventoryValuations] CHECK CONSTRAINT [FK_InventoryValuations_Items_ItemId]
GO
ALTER TABLE [dbo].[InventoryValuations]  WITH CHECK ADD  CONSTRAINT [FK_InventoryValuations_Items_ItemId1] FOREIGN KEY([ItemId1])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[InventoryValuations] CHECK CONSTRAINT [FK_InventoryValuations_Items_ItemId1]
GO
ALTER TABLE [dbo].[InventoryValuations]  WITH CHECK ADD  CONSTRAINT [FK_InventoryValuations_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[InventoryValuations] CHECK CONSTRAINT [FK_InventoryValuations_Stores_StoreId]
GO
ALTER TABLE [dbo].[InventoryValuations]  WITH CHECK ADD  CONSTRAINT [FK_InventoryValuations_Stores_StoreId1] FOREIGN KEY([StoreId1])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[InventoryValuations] CHECK CONSTRAINT [FK_InventoryValuations_Stores_StoreId1]
GO
ALTER TABLE [dbo].[IssueItems]  WITH CHECK ADD  CONSTRAINT [FK_IssueItems_Issues_IssueId] FOREIGN KEY([IssueId])
REFERENCES [dbo].[Issues] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[IssueItems] CHECK CONSTRAINT [FK_IssueItems_Issues_IssueId]
GO
ALTER TABLE [dbo].[IssueItems]  WITH CHECK ADD  CONSTRAINT [FK_IssueItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[IssueItems] CHECK CONSTRAINT [FK_IssueItems_Items_ItemId]
GO
ALTER TABLE [dbo].[IssueItems]  WITH CHECK ADD  CONSTRAINT [FK_IssueItems_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[IssueItems] CHECK CONSTRAINT [FK_IssueItems_Stores_StoreId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_AllotmentLetters_AllotmentLetterId] FOREIGN KEY([AllotmentLetterId])
REFERENCES [dbo].[AllotmentLetters] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_AllotmentLetters_AllotmentLetterId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_AspNetUsers_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_AspNetUsers_CreatedByUserId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Battalions_IssuedToBattalionId] FOREIGN KEY([IssuedToBattalionId])
REFERENCES [dbo].[Battalions] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Battalions_IssuedToBattalionId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Issues_ParentIssueId] FOREIGN KEY([ParentIssueId])
REFERENCES [dbo].[Issues] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Issues_ParentIssueId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Ranges_IssuedToRangeId] FOREIGN KEY([IssuedToRangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Ranges_IssuedToRangeId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Signatures_ApproverSignatureId] FOREIGN KEY([ApproverSignatureId])
REFERENCES [dbo].[Signatures] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Signatures_ApproverSignatureId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Signatures_IssuerSignatureId] FOREIGN KEY([IssuerSignatureId])
REFERENCES [dbo].[Signatures] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Signatures_IssuerSignatureId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Signatures_ReceiverSignatureId] FOREIGN KEY([ReceiverSignatureId])
REFERENCES [dbo].[Signatures] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Signatures_ReceiverSignatureId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Stores_FromStoreId] FOREIGN KEY([FromStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Stores_FromStoreId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Unions_IssuedToUnionId] FOREIGN KEY([IssuedToUnionId])
REFERENCES [dbo].[Unions] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Unions_IssuedToUnionId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Upazilas_IssuedToUpazilaId] FOREIGN KEY([IssuedToUpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Upazilas_IssuedToUpazilaId]
GO
ALTER TABLE [dbo].[Issues]  WITH CHECK ADD  CONSTRAINT [FK_Issues_Zilas_IssuedToZilaId] FOREIGN KEY([IssuedToZilaId])
REFERENCES [dbo].[Zilas] ([Id])
GO
ALTER TABLE [dbo].[Issues] CHECK CONSTRAINT [FK_Issues_Zilas_IssuedToZilaId]
GO
ALTER TABLE [dbo].[IssueVouchers]  WITH CHECK ADD  CONSTRAINT [FK_IssueVouchers_Issues_IssueId] FOREIGN KEY([IssueId])
REFERENCES [dbo].[Issues] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[IssueVouchers] CHECK CONSTRAINT [FK_IssueVouchers_Issues_IssueId]
GO
ALTER TABLE [dbo].[ItemModels]  WITH CHECK ADD  CONSTRAINT [FK_ItemModels_Brands_BrandId] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brands] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ItemModels] CHECK CONSTRAINT [FK_ItemModels_Brands_BrandId]
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_Brands_BrandId] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brands] ([Id])
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_Brands_BrandId]
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_ItemModels_ItemModelId] FOREIGN KEY([ItemModelId])
REFERENCES [dbo].[ItemModels] ([Id])
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_ItemModels_ItemModelId]
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_SubCategories_SubCategoryId] FOREIGN KEY([SubCategoryId])
REFERENCES [dbo].[SubCategories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_SubCategories_SubCategoryId]
GO
ALTER TABLE [dbo].[ItemSpecifications]  WITH CHECK ADD  CONSTRAINT [FK_ItemSpecifications_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ItemSpecifications] CHECK CONSTRAINT [FK_ItemSpecifications_Items_ItemId]
GO
ALTER TABLE [dbo].[LedgerBooks]  WITH CHECK ADD  CONSTRAINT [FK_LedgerBooks_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[LedgerBooks] CHECK CONSTRAINT [FK_LedgerBooks_Stores_StoreId]
GO
ALTER TABLE [dbo].[Locations]  WITH CHECK ADD  CONSTRAINT [FK_Locations_Locations_ParentLocationId] FOREIGN KEY([ParentLocationId])
REFERENCES [dbo].[Locations] ([Id])
GO
ALTER TABLE [dbo].[Locations] CHECK CONSTRAINT [FK_Locations_Locations_ParentLocationId]
GO
ALTER TABLE [dbo].[LoginLogs]  WITH CHECK ADD  CONSTRAINT [FK_LoginLogs_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[LoginLogs] CHECK CONSTRAINT [FK_LoginLogs_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD  CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Notifications] CHECK CONSTRAINT [FK_Notifications_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[PersonnelItemIssues]  WITH CHECK ADD  CONSTRAINT [FK_PersonnelItemIssues_Battalions_BattalionId] FOREIGN KEY([BattalionId])
REFERENCES [dbo].[Battalions] ([Id])
GO
ALTER TABLE [dbo].[PersonnelItemIssues] CHECK CONSTRAINT [FK_PersonnelItemIssues_Battalions_BattalionId]
GO
ALTER TABLE [dbo].[PersonnelItemIssues]  WITH CHECK ADD  CONSTRAINT [FK_PersonnelItemIssues_Issues_OriginalIssueId] FOREIGN KEY([OriginalIssueId])
REFERENCES [dbo].[Issues] ([Id])
GO
ALTER TABLE [dbo].[PersonnelItemIssues] CHECK CONSTRAINT [FK_PersonnelItemIssues_Issues_OriginalIssueId]
GO
ALTER TABLE [dbo].[PersonnelItemIssues]  WITH CHECK ADD  CONSTRAINT [FK_PersonnelItemIssues_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[PersonnelItemIssues] CHECK CONSTRAINT [FK_PersonnelItemIssues_Items_ItemId]
GO
ALTER TABLE [dbo].[PersonnelItemIssues]  WITH CHECK ADD  CONSTRAINT [FK_PersonnelItemIssues_Ranges_RangeId] FOREIGN KEY([RangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[PersonnelItemIssues] CHECK CONSTRAINT [FK_PersonnelItemIssues_Ranges_RangeId]
GO
ALTER TABLE [dbo].[PersonnelItemIssues]  WITH CHECK ADD  CONSTRAINT [FK_PersonnelItemIssues_Receives_ReceiveId] FOREIGN KEY([ReceiveId])
REFERENCES [dbo].[Receives] ([Id])
GO
ALTER TABLE [dbo].[PersonnelItemIssues] CHECK CONSTRAINT [FK_PersonnelItemIssues_Receives_ReceiveId]
GO
ALTER TABLE [dbo].[PersonnelItemIssues]  WITH CHECK ADD  CONSTRAINT [FK_PersonnelItemIssues_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[PersonnelItemIssues] CHECK CONSTRAINT [FK_PersonnelItemIssues_Stores_StoreId]
GO
ALTER TABLE [dbo].[PersonnelItemIssues]  WITH CHECK ADD  CONSTRAINT [FK_PersonnelItemIssues_Upazilas_UpazilaId] FOREIGN KEY([UpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
GO
ALTER TABLE [dbo].[PersonnelItemIssues] CHECK CONSTRAINT [FK_PersonnelItemIssues_Upazilas_UpazilaId]
GO
ALTER TABLE [dbo].[PersonnelItemIssues]  WITH CHECK ADD  CONSTRAINT [FK_PersonnelItemIssues_Zilas_ZilaId] FOREIGN KEY([ZilaId])
REFERENCES [dbo].[Zilas] ([Id])
GO
ALTER TABLE [dbo].[PersonnelItemIssues] CHECK CONSTRAINT [FK_PersonnelItemIssues_Zilas_ZilaId]
GO
ALTER TABLE [dbo].[PhysicalInventories]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventories_AspNetUsers_CountedByUserId] FOREIGN KEY([CountedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventories] CHECK CONSTRAINT [FK_PhysicalInventories_AspNetUsers_CountedByUserId]
GO
ALTER TABLE [dbo].[PhysicalInventories]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventories_AspNetUsers_VerifiedByUserId] FOREIGN KEY([VerifiedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventories] CHECK CONSTRAINT [FK_PhysicalInventories_AspNetUsers_VerifiedByUserId]
GO
ALTER TABLE [dbo].[PhysicalInventories]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventories_Battalions_BattalionId] FOREIGN KEY([BattalionId])
REFERENCES [dbo].[Battalions] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventories] CHECK CONSTRAINT [FK_PhysicalInventories_Battalions_BattalionId]
GO
ALTER TABLE [dbo].[PhysicalInventories]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventories_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventories] CHECK CONSTRAINT [FK_PhysicalInventories_Categories_CategoryId]
GO
ALTER TABLE [dbo].[PhysicalInventories]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventories_Ranges_RangeId] FOREIGN KEY([RangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventories] CHECK CONSTRAINT [FK_PhysicalInventories_Ranges_RangeId]
GO
ALTER TABLE [dbo].[PhysicalInventories]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventories_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PhysicalInventories] CHECK CONSTRAINT [FK_PhysicalInventories_Stores_StoreId]
GO
ALTER TABLE [dbo].[PhysicalInventories]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventories_Upazilas_UpazilaId] FOREIGN KEY([UpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventories] CHECK CONSTRAINT [FK_PhysicalInventories_Upazilas_UpazilaId]
GO
ALTER TABLE [dbo].[PhysicalInventories]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventories_Zilas_ZilaId] FOREIGN KEY([ZilaId])
REFERENCES [dbo].[Zilas] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventories] CHECK CONSTRAINT [FK_PhysicalInventories_Zilas_ZilaId]
GO
ALTER TABLE [dbo].[PhysicalInventoryDetails]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventoryDetails_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventoryDetails] CHECK CONSTRAINT [FK_PhysicalInventoryDetails_Categories_CategoryId]
GO
ALTER TABLE [dbo].[PhysicalInventoryDetails]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventoryDetails_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventoryDetails] CHECK CONSTRAINT [FK_PhysicalInventoryDetails_Items_ItemId]
GO
ALTER TABLE [dbo].[PhysicalInventoryDetails]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventoryDetails_PhysicalInventories_PhysicalInventoryId] FOREIGN KEY([PhysicalInventoryId])
REFERENCES [dbo].[PhysicalInventories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PhysicalInventoryDetails] CHECK CONSTRAINT [FK_PhysicalInventoryDetails_PhysicalInventories_PhysicalInventoryId]
GO
ALTER TABLE [dbo].[PhysicalInventoryItems]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventoryItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[PhysicalInventoryItems] CHECK CONSTRAINT [FK_PhysicalInventoryItems_Items_ItemId]
GO
ALTER TABLE [dbo].[PhysicalInventoryItems]  WITH CHECK ADD  CONSTRAINT [FK_PhysicalInventoryItems_PhysicalInventories_PhysicalInventoryId] FOREIGN KEY([PhysicalInventoryId])
REFERENCES [dbo].[PhysicalInventories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PhysicalInventoryItems] CHECK CONSTRAINT [FK_PhysicalInventoryItems_PhysicalInventories_PhysicalInventoryId]
GO
ALTER TABLE [dbo].[PurchaseItems]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PurchaseItems] CHECK CONSTRAINT [FK_PurchaseItems_Items_ItemId]
GO
ALTER TABLE [dbo].[PurchaseItems]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseItems_Purchases_PurchaseId] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchases] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PurchaseItems] CHECK CONSTRAINT [FK_PurchaseItems_Purchases_PurchaseId]
GO
ALTER TABLE [dbo].[PurchaseItems]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseItems_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[PurchaseItems] CHECK CONSTRAINT [FK_PurchaseItems_Stores_StoreId]
GO
ALTER TABLE [dbo].[PurchaseOrderItems]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PurchaseOrderItems] CHECK CONSTRAINT [FK_PurchaseOrderItems_Items_ItemId]
GO
ALTER TABLE [dbo].[PurchaseOrderItems]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[PurchaseOrders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PurchaseOrderItems] CHECK CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId]
GO
ALTER TABLE [dbo].[PurchaseOrders]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrders_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[PurchaseOrders] CHECK CONSTRAINT [FK_PurchaseOrders_Stores_StoreId]
GO
ALTER TABLE [dbo].[PurchaseOrders]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrders_Vendors_VendorId] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendors] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PurchaseOrders] CHECK CONSTRAINT [FK_PurchaseOrders_Vendors_VendorId]
GO
ALTER TABLE [dbo].[Purchases]  WITH CHECK ADD  CONSTRAINT [FK_Purchases_AspNetUsers_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Purchases] CHECK CONSTRAINT [FK_Purchases_AspNetUsers_CreatedByUserId]
GO
ALTER TABLE [dbo].[Purchases]  WITH CHECK ADD  CONSTRAINT [FK_Purchases_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Purchases] CHECK CONSTRAINT [FK_Purchases_Stores_StoreId]
GO
ALTER TABLE [dbo].[Purchases]  WITH CHECK ADD  CONSTRAINT [FK_Purchases_Vendors_VendorId] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendors] ([Id])
GO
ALTER TABLE [dbo].[Purchases] CHECK CONSTRAINT [FK_Purchases_Vendors_VendorId]
GO
ALTER TABLE [dbo].[QualityCheckItems]  WITH CHECK ADD  CONSTRAINT [FK_QualityCheckItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[QualityCheckItems] CHECK CONSTRAINT [FK_QualityCheckItems_Items_ItemId]
GO
ALTER TABLE [dbo].[QualityCheckItems]  WITH CHECK ADD  CONSTRAINT [FK_QualityCheckItems_QualityChecks_QualityCheckId] FOREIGN KEY([QualityCheckId])
REFERENCES [dbo].[QualityChecks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QualityCheckItems] CHECK CONSTRAINT [FK_QualityCheckItems_QualityChecks_QualityCheckId]
GO
ALTER TABLE [dbo].[QualityChecks]  WITH CHECK ADD  CONSTRAINT [FK_QualityChecks_AspNetUsers_CheckedByUserId] FOREIGN KEY([CheckedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[QualityChecks] CHECK CONSTRAINT [FK_QualityChecks_AspNetUsers_CheckedByUserId]
GO
ALTER TABLE [dbo].[QualityChecks]  WITH CHECK ADD  CONSTRAINT [FK_QualityChecks_GoodsReceives_GoodsReceiveId] FOREIGN KEY([GoodsReceiveId])
REFERENCES [dbo].[GoodsReceives] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QualityChecks] CHECK CONSTRAINT [FK_QualityChecks_GoodsReceives_GoodsReceiveId]
GO
ALTER TABLE [dbo].[QualityChecks]  WITH CHECK ADD  CONSTRAINT [FK_QualityChecks_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QualityChecks] CHECK CONSTRAINT [FK_QualityChecks_Items_ItemId]
GO
ALTER TABLE [dbo].[QualityChecks]  WITH CHECK ADD  CONSTRAINT [FK_QualityChecks_Purchases_PurchaseId] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchases] ([Id])
GO
ALTER TABLE [dbo].[QualityChecks] CHECK CONSTRAINT [FK_QualityChecks_Purchases_PurchaseId]
GO
ALTER TABLE [dbo].[ReceiveItems]  WITH CHECK ADD  CONSTRAINT [FK_ReceiveItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[ReceiveItems] CHECK CONSTRAINT [FK_ReceiveItems_Items_ItemId]
GO
ALTER TABLE [dbo].[ReceiveItems]  WITH CHECK ADD  CONSTRAINT [FK_ReceiveItems_Items_ItemId1] FOREIGN KEY([ItemId1])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[ReceiveItems] CHECK CONSTRAINT [FK_ReceiveItems_Items_ItemId1]
GO
ALTER TABLE [dbo].[ReceiveItems]  WITH CHECK ADD  CONSTRAINT [FK_ReceiveItems_Receives_ReceiveId] FOREIGN KEY([ReceiveId])
REFERENCES [dbo].[Receives] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReceiveItems] CHECK CONSTRAINT [FK_ReceiveItems_Receives_ReceiveId]
GO
ALTER TABLE [dbo].[ReceiveItems]  WITH CHECK ADD  CONSTRAINT [FK_ReceiveItems_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[ReceiveItems] CHECK CONSTRAINT [FK_ReceiveItems_Stores_StoreId]
GO
ALTER TABLE [dbo].[Receives]  WITH CHECK ADD  CONSTRAINT [FK_Receives_Battalions_ReceivedFromBattalionId] FOREIGN KEY([ReceivedFromBattalionId])
REFERENCES [dbo].[Battalions] ([Id])
GO
ALTER TABLE [dbo].[Receives] CHECK CONSTRAINT [FK_Receives_Battalions_ReceivedFromBattalionId]
GO
ALTER TABLE [dbo].[Receives]  WITH CHECK ADD  CONSTRAINT [FK_Receives_Issues_OriginalIssueId] FOREIGN KEY([OriginalIssueId])
REFERENCES [dbo].[Issues] ([Id])
GO
ALTER TABLE [dbo].[Receives] CHECK CONSTRAINT [FK_Receives_Issues_OriginalIssueId]
GO
ALTER TABLE [dbo].[Receives]  WITH CHECK ADD  CONSTRAINT [FK_Receives_Ranges_ReceivedFromRangeId] FOREIGN KEY([ReceivedFromRangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[Receives] CHECK CONSTRAINT [FK_Receives_Ranges_ReceivedFromRangeId]
GO
ALTER TABLE [dbo].[Receives]  WITH CHECK ADD  CONSTRAINT [FK_Receives_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Receives] CHECK CONSTRAINT [FK_Receives_Stores_StoreId]
GO
ALTER TABLE [dbo].[Receives]  WITH CHECK ADD  CONSTRAINT [FK_Receives_Unions_ReceivedFromUnionId] FOREIGN KEY([ReceivedFromUnionId])
REFERENCES [dbo].[Unions] ([Id])
GO
ALTER TABLE [dbo].[Receives] CHECK CONSTRAINT [FK_Receives_Unions_ReceivedFromUnionId]
GO
ALTER TABLE [dbo].[Receives]  WITH CHECK ADD  CONSTRAINT [FK_Receives_Upazilas_ReceivedFromUpazilaId] FOREIGN KEY([ReceivedFromUpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
GO
ALTER TABLE [dbo].[Receives] CHECK CONSTRAINT [FK_Receives_Upazilas_ReceivedFromUpazilaId]
GO
ALTER TABLE [dbo].[Receives]  WITH CHECK ADD  CONSTRAINT [FK_Receives_Zilas_ReceivedFromZilaId] FOREIGN KEY([ReceivedFromZilaId])
REFERENCES [dbo].[Zilas] ([Id])
GO
ALTER TABLE [dbo].[Receives] CHECK CONSTRAINT [FK_Receives_Zilas_ReceivedFromZilaId]
GO
ALTER TABLE [dbo].[RequisitionItems]  WITH CHECK ADD  CONSTRAINT [FK_RequisitionItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RequisitionItems] CHECK CONSTRAINT [FK_RequisitionItems_Items_ItemId]
GO
ALTER TABLE [dbo].[RequisitionItems]  WITH CHECK ADD  CONSTRAINT [FK_RequisitionItems_Requisitions_RequisitionId] FOREIGN KEY([RequisitionId])
REFERENCES [dbo].[Requisitions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RequisitionItems] CHECK CONSTRAINT [FK_RequisitionItems_Requisitions_RequisitionId]
GO
ALTER TABLE [dbo].[Requisitions]  WITH CHECK ADD  CONSTRAINT [FK_Requisitions_AspNetUsers_ApprovedByUserId] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Requisitions] CHECK CONSTRAINT [FK_Requisitions_AspNetUsers_ApprovedByUserId]
GO
ALTER TABLE [dbo].[Requisitions]  WITH CHECK ADD  CONSTRAINT [FK_Requisitions_AspNetUsers_RequestedByUserId] FOREIGN KEY([RequestedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Requisitions] CHECK CONSTRAINT [FK_Requisitions_AspNetUsers_RequestedByUserId]
GO
ALTER TABLE [dbo].[Requisitions]  WITH CHECK ADD  CONSTRAINT [FK_Requisitions_Purchases_PurchaseOrderId] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[Purchases] ([Id])
GO
ALTER TABLE [dbo].[Requisitions] CHECK CONSTRAINT [FK_Requisitions_Purchases_PurchaseOrderId]
GO
ALTER TABLE [dbo].[Requisitions]  WITH CHECK ADD  CONSTRAINT [FK_Requisitions_Stores_FromStoreId] FOREIGN KEY([FromStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Requisitions] CHECK CONSTRAINT [FK_Requisitions_Stores_FromStoreId]
GO
ALTER TABLE [dbo].[Requisitions]  WITH CHECK ADD  CONSTRAINT [FK_Requisitions_Stores_ToStoreId] FOREIGN KEY([ToStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Requisitions] CHECK CONSTRAINT [FK_Requisitions_Stores_ToStoreId]
GO
ALTER TABLE [dbo].[ReturnItems]  WITH CHECK ADD  CONSTRAINT [FK_ReturnItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[ReturnItems] CHECK CONSTRAINT [FK_ReturnItems_Items_ItemId]
GO
ALTER TABLE [dbo].[ReturnItems]  WITH CHECK ADD  CONSTRAINT [FK_ReturnItems_Returns_ReturnId] FOREIGN KEY([ReturnId])
REFERENCES [dbo].[Returns] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReturnItems] CHECK CONSTRAINT [FK_ReturnItems_Returns_ReturnId]
GO
ALTER TABLE [dbo].[Returns]  WITH CHECK ADD  CONSTRAINT [FK_Returns_Issues_OriginalIssueId] FOREIGN KEY([OriginalIssueId])
REFERENCES [dbo].[Issues] ([Id])
GO
ALTER TABLE [dbo].[Returns] CHECK CONSTRAINT [FK_Returns_Issues_OriginalIssueId]
GO
ALTER TABLE [dbo].[Returns]  WITH CHECK ADD  CONSTRAINT [FK_Returns_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[Returns] CHECK CONSTRAINT [FK_Returns_Items_ItemId]
GO
ALTER TABLE [dbo].[Returns]  WITH CHECK ADD  CONSTRAINT [FK_Returns_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Returns] CHECK CONSTRAINT [FK_Returns_Stores_StoreId]
GO
ALTER TABLE [dbo].[Returns]  WITH CHECK ADD  CONSTRAINT [FK_Returns_Stores_ToStoreId] FOREIGN KEY([ToStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Returns] CHECK CONSTRAINT [FK_Returns_Stores_ToStoreId]
GO
ALTER TABLE [dbo].[SignatureOTPs]  WITH CHECK ADD  CONSTRAINT [FK_SignatureOTPs_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[SignatureOTPs] CHECK CONSTRAINT [FK_SignatureOTPs_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Signatures]  WITH CHECK ADD  CONSTRAINT [FK_Signatures_Issues_IssueId] FOREIGN KEY([IssueId])
REFERENCES [dbo].[Issues] ([Id])
GO
ALTER TABLE [dbo].[Signatures] CHECK CONSTRAINT [FK_Signatures_Issues_IssueId]
GO
ALTER TABLE [dbo].[StockAdjustmentItems]  WITH CHECK ADD  CONSTRAINT [FK_StockAdjustmentItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[StockAdjustmentItems] CHECK CONSTRAINT [FK_StockAdjustmentItems_Items_ItemId]
GO
ALTER TABLE [dbo].[StockAdjustmentItems]  WITH CHECK ADD  CONSTRAINT [FK_StockAdjustmentItems_StockAdjustments_StockAdjustmentId] FOREIGN KEY([StockAdjustmentId])
REFERENCES [dbo].[StockAdjustments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StockAdjustmentItems] CHECK CONSTRAINT [FK_StockAdjustmentItems_StockAdjustments_StockAdjustmentId]
GO
ALTER TABLE [dbo].[StockAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_StockAdjustments_AspNetUsers_ApprovedByUserId] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[StockAdjustments] CHECK CONSTRAINT [FK_StockAdjustments_AspNetUsers_ApprovedByUserId]
GO
ALTER TABLE [dbo].[StockAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_StockAdjustments_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[StockAdjustments] CHECK CONSTRAINT [FK_StockAdjustments_Items_ItemId]
GO
ALTER TABLE [dbo].[StockAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_StockAdjustments_Items_ItemId1] FOREIGN KEY([ItemId1])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[StockAdjustments] CHECK CONSTRAINT [FK_StockAdjustments_Items_ItemId1]
GO
ALTER TABLE [dbo].[StockAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_StockAdjustments_PhysicalInventories_PhysicalInventoryId] FOREIGN KEY([PhysicalInventoryId])
REFERENCES [dbo].[PhysicalInventories] ([Id])
GO
ALTER TABLE [dbo].[StockAdjustments] CHECK CONSTRAINT [FK_StockAdjustments_PhysicalInventories_PhysicalInventoryId]
GO
ALTER TABLE [dbo].[StockAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_StockAdjustments_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockAdjustments] CHECK CONSTRAINT [FK_StockAdjustments_Stores_StoreId]
GO
ALTER TABLE [dbo].[StockAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_StockAdjustments_Stores_StoreId1] FOREIGN KEY([StoreId1])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockAdjustments] CHECK CONSTRAINT [FK_StockAdjustments_Stores_StoreId1]
GO
ALTER TABLE [dbo].[StockAlerts]  WITH CHECK ADD  CONSTRAINT [FK_StockAlerts_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[StockAlerts] CHECK CONSTRAINT [FK_StockAlerts_Items_ItemId]
GO
ALTER TABLE [dbo].[StockAlerts]  WITH CHECK ADD  CONSTRAINT [FK_StockAlerts_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockAlerts] CHECK CONSTRAINT [FK_StockAlerts_Stores_StoreId]
GO
ALTER TABLE [dbo].[StockEntries]  WITH NOCHECK ADD  CONSTRAINT [FK_StockEntries_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockEntries] CHECK CONSTRAINT [FK_StockEntries_Stores_StoreId]
GO
ALTER TABLE [dbo].[StockEntryItems]  WITH CHECK ADD  CONSTRAINT [FK_StockEntryItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[StockEntryItems] CHECK CONSTRAINT [FK_StockEntryItems_Items_ItemId]
GO
ALTER TABLE [dbo].[StockEntryItems]  WITH CHECK ADD  CONSTRAINT [FK_StockEntryItems_StockEntries_StockEntryId] FOREIGN KEY([StockEntryId])
REFERENCES [dbo].[StockEntries] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StockEntryItems] CHECK CONSTRAINT [FK_StockEntryItems_StockEntries_StockEntryId]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_AspNetUsers_MovedByUserId] FOREIGN KEY([MovedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_AspNetUsers_MovedByUserId]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_Items_ItemId]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_StoreItems_StoreItemId] FOREIGN KEY([StoreItemId])
REFERENCES [dbo].[StoreItems] ([Id])
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_StoreItems_StoreItemId]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_Stores_DestinationStoreId] FOREIGN KEY([DestinationStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_Stores_DestinationStoreId]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_Stores_SourceStoreId] FOREIGN KEY([SourceStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_Stores_SourceStoreId]
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD  CONSTRAINT [FK_StockMovements_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockMovements] CHECK CONSTRAINT [FK_StockMovements_Stores_StoreId]
GO
ALTER TABLE [dbo].[StockOperations]  WITH CHECK ADD  CONSTRAINT [FK_StockOperations_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[StockOperations] CHECK CONSTRAINT [FK_StockOperations_Items_ItemId]
GO
ALTER TABLE [dbo].[StockOperations]  WITH CHECK ADD  CONSTRAINT [FK_StockOperations_Stores_FromStoreId] FOREIGN KEY([FromStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockOperations] CHECK CONSTRAINT [FK_StockOperations_Stores_FromStoreId]
GO
ALTER TABLE [dbo].[StockOperations]  WITH CHECK ADD  CONSTRAINT [FK_StockOperations_Stores_ToStoreId] FOREIGN KEY([ToStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StockOperations] CHECK CONSTRAINT [FK_StockOperations_Stores_ToStoreId]
GO
ALTER TABLE [dbo].[StockReturns]  WITH CHECK ADD  CONSTRAINT [FK_StockReturns_Issues_OriginalIssueId] FOREIGN KEY([OriginalIssueId])
REFERENCES [dbo].[Issues] ([Id])
GO
ALTER TABLE [dbo].[StockReturns] CHECK CONSTRAINT [FK_StockReturns_Issues_OriginalIssueId]
GO
ALTER TABLE [dbo].[StockReturns]  WITH CHECK ADD  CONSTRAINT [FK_StockReturns_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[StockReturns] CHECK CONSTRAINT [FK_StockReturns_Items_ItemId]
GO
ALTER TABLE [dbo].[StoreConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_StoreConfigurations_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoreConfigurations] CHECK CONSTRAINT [FK_StoreConfigurations_Stores_StoreId]
GO
ALTER TABLE [dbo].[StoreItems]  WITH NOCHECK ADD  CONSTRAINT [FK_StoreItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoreItems] CHECK CONSTRAINT [FK_StoreItems_Items_ItemId]
GO
ALTER TABLE [dbo].[StoreItems]  WITH NOCHECK ADD  CONSTRAINT [FK_StoreItems_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoreItems] CHECK CONSTRAINT [FK_StoreItems_Stores_StoreId]
GO
ALTER TABLE [dbo].[Stores]  WITH CHECK ADD  CONSTRAINT [FK_Stores_Battalions_BattalionId] FOREIGN KEY([BattalionId])
REFERENCES [dbo].[Battalions] ([Id])
GO
ALTER TABLE [dbo].[Stores] CHECK CONSTRAINT [FK_Stores_Battalions_BattalionId]
GO
ALTER TABLE [dbo].[Stores]  WITH CHECK ADD  CONSTRAINT [FK_Stores_Locations_LocationId] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Locations] ([Id])
GO
ALTER TABLE [dbo].[Stores] CHECK CONSTRAINT [FK_Stores_Locations_LocationId]
GO
ALTER TABLE [dbo].[Stores]  WITH CHECK ADD  CONSTRAINT [FK_Stores_Ranges_RangeId] FOREIGN KEY([RangeId])
REFERENCES [dbo].[Ranges] ([Id])
GO
ALTER TABLE [dbo].[Stores] CHECK CONSTRAINT [FK_Stores_Ranges_RangeId]
GO
ALTER TABLE [dbo].[Stores]  WITH CHECK ADD  CONSTRAINT [FK_Stores_StoreTypes_StoreTypeId] FOREIGN KEY([StoreTypeId])
REFERENCES [dbo].[StoreTypes] ([Id])
GO
ALTER TABLE [dbo].[Stores] CHECK CONSTRAINT [FK_Stores_StoreTypes_StoreTypeId]
GO
ALTER TABLE [dbo].[Stores]  WITH CHECK ADD  CONSTRAINT [FK_Stores_Unions_UnionId] FOREIGN KEY([UnionId])
REFERENCES [dbo].[Unions] ([Id])
GO
ALTER TABLE [dbo].[Stores] CHECK CONSTRAINT [FK_Stores_Unions_UnionId]
GO
ALTER TABLE [dbo].[Stores]  WITH CHECK ADD  CONSTRAINT [FK_Stores_Upazilas_UpazilaId] FOREIGN KEY([UpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
GO
ALTER TABLE [dbo].[Stores] CHECK CONSTRAINT [FK_Stores_Upazilas_UpazilaId]
GO
ALTER TABLE [dbo].[Stores]  WITH CHECK ADD  CONSTRAINT [FK_Stores_Zilas_ZilaId] FOREIGN KEY([ZilaId])
REFERENCES [dbo].[Zilas] ([Id])
GO
ALTER TABLE [dbo].[Stores] CHECK CONSTRAINT [FK_Stores_Zilas_ZilaId]
GO
ALTER TABLE [dbo].[StoreStocks]  WITH CHECK ADD  CONSTRAINT [FK_StoreStocks_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoreStocks] CHECK CONSTRAINT [FK_StoreStocks_Items_ItemId]
GO
ALTER TABLE [dbo].[StoreStocks]  WITH CHECK ADD  CONSTRAINT [FK_StoreStocks_Items_ItemId1] FOREIGN KEY([ItemId1])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[StoreStocks] CHECK CONSTRAINT [FK_StoreStocks_Items_ItemId1]
GO
ALTER TABLE [dbo].[StoreStocks]  WITH CHECK ADD  CONSTRAINT [FK_StoreStocks_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoreStocks] CHECK CONSTRAINT [FK_StoreStocks_Stores_StoreId]
GO
ALTER TABLE [dbo].[StoreStocks]  WITH CHECK ADD  CONSTRAINT [FK_StoreStocks_Stores_StoreId1] FOREIGN KEY([StoreId1])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[StoreStocks] CHECK CONSTRAINT [FK_StoreStocks_Stores_StoreId1]
GO
ALTER TABLE [dbo].[StoreTypeCategories]  WITH CHECK ADD  CONSTRAINT [FK_StoreTypeCategories_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoreTypeCategories] CHECK CONSTRAINT [FK_StoreTypeCategories_Categories_CategoryId]
GO
ALTER TABLE [dbo].[StoreTypeCategories]  WITH CHECK ADD  CONSTRAINT [FK_StoreTypeCategories_StoreTypes_StoreTypeId] FOREIGN KEY([StoreTypeId])
REFERENCES [dbo].[StoreTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoreTypeCategories] CHECK CONSTRAINT [FK_StoreTypeCategories_StoreTypes_StoreTypeId]
GO
ALTER TABLE [dbo].[StoreTypeCategories]  WITH CHECK ADD  CONSTRAINT [FK_StoreTypeCategories_StoreTypes_StoreTypeId1] FOREIGN KEY([StoreTypeId1])
REFERENCES [dbo].[StoreTypes] ([Id])
GO
ALTER TABLE [dbo].[StoreTypeCategories] CHECK CONSTRAINT [FK_StoreTypeCategories_StoreTypes_StoreTypeId1]
GO
ALTER TABLE [dbo].[SubCategories]  WITH CHECK ADD  CONSTRAINT [FK_SubCategories_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubCategories] CHECK CONSTRAINT [FK_SubCategories_Categories_CategoryId]
GO
ALTER TABLE [dbo].[SupplierEvaluations]  WITH CHECK ADD  CONSTRAINT [FK_SupplierEvaluations_AspNetUsers_ApprovedByUserId] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[SupplierEvaluations] CHECK CONSTRAINT [FK_SupplierEvaluations_AspNetUsers_ApprovedByUserId]
GO
ALTER TABLE [dbo].[SupplierEvaluations]  WITH CHECK ADD  CONSTRAINT [FK_SupplierEvaluations_AspNetUsers_EvaluatedByUserId] FOREIGN KEY([EvaluatedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[SupplierEvaluations] CHECK CONSTRAINT [FK_SupplierEvaluations_AspNetUsers_EvaluatedByUserId]
GO
ALTER TABLE [dbo].[SupplierEvaluations]  WITH CHECK ADD  CONSTRAINT [FK_SupplierEvaluations_Vendors_VendorId] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendors] ([Id])
GO
ALTER TABLE [dbo].[SupplierEvaluations] CHECK CONSTRAINT [FK_SupplierEvaluations_Vendors_VendorId]
GO
ALTER TABLE [dbo].[SystemConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_SystemConfigurations_AspNetUsers_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[SystemConfigurations] CHECK CONSTRAINT [FK_SystemConfigurations_AspNetUsers_ModifiedByUserId]
GO
ALTER TABLE [dbo].[TemperatureLogs]  WITH CHECK ADD  CONSTRAINT [FK_TemperatureLogs_AspNetUsers_RecordedByUserId] FOREIGN KEY([RecordedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[TemperatureLogs] CHECK CONSTRAINT [FK_TemperatureLogs_AspNetUsers_RecordedByUserId]
GO
ALTER TABLE [dbo].[TemperatureLogs]  WITH CHECK ADD  CONSTRAINT [FK_TemperatureLogs_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[TemperatureLogs] CHECK CONSTRAINT [FK_TemperatureLogs_Stores_StoreId]
GO
ALTER TABLE [dbo].[TemperatureLogs]  WITH CHECK ADD  CONSTRAINT [FK_TemperatureLogs_Stores_StoreId1] FOREIGN KEY([StoreId1])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[TemperatureLogs] CHECK CONSTRAINT [FK_TemperatureLogs_Stores_StoreId1]
GO
ALTER TABLE [dbo].[TrackingHistories]  WITH CHECK ADD  CONSTRAINT [FK_TrackingHistories_ShipmentTrackings_ShipmentTrackingId] FOREIGN KEY([ShipmentTrackingId])
REFERENCES [dbo].[ShipmentTrackings] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TrackingHistories] CHECK CONSTRAINT [FK_TrackingHistories_ShipmentTrackings_ShipmentTrackingId]
GO
ALTER TABLE [dbo].[TransferDiscrepancies]  WITH CHECK ADD  CONSTRAINT [FK_TransferDiscrepancies_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[TransferDiscrepancies] CHECK CONSTRAINT [FK_TransferDiscrepancies_Items_ItemId]
GO
ALTER TABLE [dbo].[TransferDiscrepancies]  WITH CHECK ADD  CONSTRAINT [FK_TransferDiscrepancies_Transfers_TransferId] FOREIGN KEY([TransferId])
REFERENCES [dbo].[Transfers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransferDiscrepancies] CHECK CONSTRAINT [FK_TransferDiscrepancies_Transfers_TransferId]
GO
ALTER TABLE [dbo].[TransferItems]  WITH CHECK ADD  CONSTRAINT [FK_TransferItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[TransferItems] CHECK CONSTRAINT [FK_TransferItems_Items_ItemId]
GO
ALTER TABLE [dbo].[TransferItems]  WITH CHECK ADD  CONSTRAINT [FK_TransferItems_Transfers_TransferId] FOREIGN KEY([TransferId])
REFERENCES [dbo].[Transfers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransferItems] CHECK CONSTRAINT [FK_TransferItems_Transfers_TransferId]
GO
ALTER TABLE [dbo].[Transfers]  WITH CHECK ADD  CONSTRAINT [FK_Transfer_Store_FromStoreId] FOREIGN KEY([FromStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Transfers] CHECK CONSTRAINT [FK_Transfer_Store_FromStoreId]
GO
ALTER TABLE [dbo].[Transfers]  WITH CHECK ADD  CONSTRAINT [FK_Transfer_Store_ToStoreId] FOREIGN KEY([ToStoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[Transfers] CHECK CONSTRAINT [FK_Transfer_Store_ToStoreId]
GO
ALTER TABLE [dbo].[TransferShipmentItems]  WITH CHECK ADD  CONSTRAINT [FK_TransferShipmentItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[TransferShipmentItems] CHECK CONSTRAINT [FK_TransferShipmentItems_Items_ItemId]
GO
ALTER TABLE [dbo].[TransferShipmentItems]  WITH CHECK ADD  CONSTRAINT [FK_TransferShipmentItems_TransferShipments_ShipmentId] FOREIGN KEY([ShipmentId])
REFERENCES [dbo].[TransferShipments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransferShipmentItems] CHECK CONSTRAINT [FK_TransferShipmentItems_TransferShipments_ShipmentId]
GO
ALTER TABLE [dbo].[TransferShipments]  WITH CHECK ADD  CONSTRAINT [FK_TransferShipments_Transfers_TransferId] FOREIGN KEY([TransferId])
REFERENCES [dbo].[Transfers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransferShipments] CHECK CONSTRAINT [FK_TransferShipments_Transfers_TransferId]
GO
ALTER TABLE [dbo].[Unions]  WITH CHECK ADD  CONSTRAINT [FK_Unions_Upazilas_UpazilaId] FOREIGN KEY([UpazilaId])
REFERENCES [dbo].[Upazilas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Unions] CHECK CONSTRAINT [FK_Unions_Upazilas_UpazilaId]
GO
ALTER TABLE [dbo].[Upazilas]  WITH CHECK ADD  CONSTRAINT [FK_Upazilas_Zilas_ZilaId] FOREIGN KEY([ZilaId])
REFERENCES [dbo].[Zilas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Upazilas] CHECK CONSTRAINT [FK_Upazilas_Zilas_ZilaId]
GO
ALTER TABLE [dbo].[UserNotificationPreferences]  WITH CHECK ADD  CONSTRAINT [FK_UserNotificationPreferences_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[UserNotificationPreferences] CHECK CONSTRAINT [FK_UserNotificationPreferences_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[UserStores]  WITH CHECK ADD  CONSTRAINT [FK_UserStores_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserStores] CHECK CONSTRAINT [FK_UserStores_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[UserStores]  WITH CHECK ADD  CONSTRAINT [FK_UserStores_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserStores] CHECK CONSTRAINT [FK_UserStores_Stores_StoreId]
GO
ALTER TABLE [dbo].[Warranties]  WITH CHECK ADD  CONSTRAINT [FK_Warranties_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[Warranties] CHECK CONSTRAINT [FK_Warranties_Items_ItemId]
GO
ALTER TABLE [dbo].[Warranties]  WITH CHECK ADD  CONSTRAINT [FK_Warranties_Items_ItemId1] FOREIGN KEY([ItemId1])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[Warranties] CHECK CONSTRAINT [FK_Warranties_Items_ItemId1]
GO
ALTER TABLE [dbo].[Warranties]  WITH CHECK ADD  CONSTRAINT [FK_Warranties_Vendors_VendorId] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendors] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Warranties] CHECK CONSTRAINT [FK_Warranties_Vendors_VendorId]
GO
ALTER TABLE [dbo].[WriteOffItems]  WITH NOCHECK ADD  CONSTRAINT [FK_WriteOffItems_Items_ItemId] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[WriteOffItems] CHECK CONSTRAINT [FK_WriteOffItems_Items_ItemId]
GO
ALTER TABLE [dbo].[WriteOffItems]  WITH NOCHECK ADD  CONSTRAINT [FK_WriteOffItems_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[WriteOffItems] CHECK CONSTRAINT [FK_WriteOffItems_Stores_StoreId]
GO
ALTER TABLE [dbo].[WriteOffItems]  WITH NOCHECK ADD  CONSTRAINT [FK_WriteOffItems_WriteOffRequests_WriteOffRequestId] FOREIGN KEY([WriteOffRequestId])
REFERENCES [dbo].[WriteOffRequests] ([Id])
GO
ALTER TABLE [dbo].[WriteOffItems] CHECK CONSTRAINT [FK_WriteOffItems_WriteOffRequests_WriteOffRequestId]
GO
ALTER TABLE [dbo].[WriteOffItems]  WITH NOCHECK ADD  CONSTRAINT [FK_WriteOffItems_WriteOffRequests_WriteOffRequestId1] FOREIGN KEY([WriteOffRequestId1])
REFERENCES [dbo].[WriteOffRequests] ([Id])
GO
ALTER TABLE [dbo].[WriteOffItems] CHECK CONSTRAINT [FK_WriteOffItems_WriteOffRequests_WriteOffRequestId1]
GO
ALTER TABLE [dbo].[WriteOffItems]  WITH NOCHECK ADD  CONSTRAINT [FK_WriteOffItems_WriteOffs_WriteOffId] FOREIGN KEY([WriteOffId])
REFERENCES [dbo].[WriteOffs] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WriteOffItems] CHECK CONSTRAINT [FK_WriteOffItems_WriteOffs_WriteOffId]
GO
ALTER TABLE [dbo].[WriteOffRequests]  WITH CHECK ADD  CONSTRAINT [FK_WriteOffRequests_AspNetUsers_RequestedBy] FOREIGN KEY([RequestedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[WriteOffRequests] CHECK CONSTRAINT [FK_WriteOffRequests_AspNetUsers_RequestedBy]
GO
ALTER TABLE [dbo].[WriteOffRequests]  WITH CHECK ADD  CONSTRAINT [FK_WriteOffRequests_DamageReports_DamageReportId] FOREIGN KEY([DamageReportId])
REFERENCES [dbo].[DamageReports] ([Id])
GO
ALTER TABLE [dbo].[WriteOffRequests] CHECK CONSTRAINT [FK_WriteOffRequests_DamageReports_DamageReportId]
GO
ALTER TABLE [dbo].[WriteOffRequests]  WITH CHECK ADD  CONSTRAINT [FK_WriteOffRequests_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[WriteOffRequests] CHECK CONSTRAINT [FK_WriteOffRequests_Stores_StoreId]
GO
ALTER TABLE [dbo].[WriteOffs]  WITH NOCHECK ADD  CONSTRAINT [FK_WriteOffs_Stores_StoreId] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Stores] ([Id])
GO
ALTER TABLE [dbo].[WriteOffs] CHECK CONSTRAINT [FK_WriteOffs_Stores_StoreId]
GO
ALTER TABLE [dbo].[Zilas]  WITH CHECK ADD  CONSTRAINT [FK_Zilas_Ranges_RangeId] FOREIGN KEY([RangeId])
REFERENCES [dbo].[Ranges] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Zilas] CHECK CONSTRAINT [FK_Zilas_Ranges_RangeId]
GO
USE [master]
GO
ALTER DATABASE [ansvdp_ims] SET  READ_WRITE 
GO
