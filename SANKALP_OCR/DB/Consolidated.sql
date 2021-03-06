USE [master]
GO
/****** Object:  Database [IQInvoiceItemNew]    Script Date: 03-10-2017 14:43:40 ******/
CREATE DATABASE [IQInvoiceItemNew]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'IQInvoiceItem', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL12.SQLSERVER2014\MSSQL\DATA\IQInvoiceItemNew.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'IQInvoiceItem_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL12.SQLSERVER2014\MSSQL\DATA\IQInvoiceItemNew_log.ldf' , SIZE = 3456KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [IQInvoiceItemNew] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [IQInvoiceItemNew].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [IQInvoiceItemNew] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET ARITHABORT OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [IQInvoiceItemNew] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [IQInvoiceItemNew] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET  DISABLE_BROKER 
GO
ALTER DATABASE [IQInvoiceItemNew] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [IQInvoiceItemNew] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [IQInvoiceItemNew] SET  MULTI_USER 
GO
ALTER DATABASE [IQInvoiceItemNew] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [IQInvoiceItemNew] SET DB_CHAINING OFF 
GO
ALTER DATABASE [IQInvoiceItemNew] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [IQInvoiceItemNew] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [IQInvoiceItemNew] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'IQInvoiceItemNew', N'ON'
GO
USE [IQInvoiceItemNew]
GO
/****** Object:  UserDefinedTableType [dbo].[ItemDetailValuedType]    Script Date: 03-10-2017 14:43:40 ******/
CREATE TYPE [dbo].[ItemDetailValuedType] AS TABLE(
	[ID] [int] NULL,
	[ItemName] [varchar](200) NULL,
	[Price] [varchar](200) NULL,
	[Qty] [varchar](200) NULL,
	[Tax] [varchar](200) NULL,
	[Total] [varchar](200) NULL,
	[Asset] [bit] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[TableInvoiceStatus]    Script Date: 03-10-2017 14:43:40 ******/
CREATE TYPE [dbo].[TableInvoiceStatus] AS TABLE(
	[UserID] [int] NULL,
	[UserTypeID] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[TableValuedType]    Script Date: 03-10-2017 14:43:40 ******/
CREATE TYPE [dbo].[TableValuedType] AS TABLE(
	[ItemName] [varchar](200) NULL,
	[Price] [varchar](200) NULL,
	[Qty] [varchar](200) NULL,
	[Tax] [varchar](200) NULL,
	[Total] [varchar](200) NULL,
	[Asset] [bit] NULL,
	[code] [varchar](10) NULL,
	[LastCodeIndex] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UserMappingList]    Script Date: 03-10-2017 14:43:40 ******/
CREATE TYPE [dbo].[UserMappingList] AS TABLE(
	[UserID] [int] NULL
)
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetCode]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_GetCode] (@Code varchar(10)='DF')
RETURNS VARCHAR(10)    
AS    
BEGIN    
 DECLARE @strCode VARCHAR(10)       
--declare @Code varchar(10)



--declare @Code varchar(10) ='AF'
declare @CodeIndex int 
select  @CodeIndex =  MAX(LastCodeIndex) + 1 FROM OrderItemDetails where Code= @Code 
--update OrderItemDetails SET LastCodeIndex = @CodeIndex where Code= @Code 
SELECT  top 1 @strCode =  @Code + CAST(@CodeIndex as varchar(10))  FROM OrderItemDetails where Code= @Code 
RETURN @strcode
END 

GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetCodeIndex]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_GetCodeIndex] (@Code varchar(100)='DF')
RETURNS int    
AS    
BEGIN    
 DECLARE @strCode VARCHAR(100)       

declare @CodeIndex int 
--declare @qty int
--select @quantity = LEFT(@quantity, CHARINDEX('.', @quantity) - 1) WHERE CHARINDEX('.', @quantity) > 0
--select @qty = Cast(@quantity as int)
select  @CodeIndex =  MAX(LastIndex) + 1 FROM ItemCodes where Code= @Code 

select @strCode = @Code + cast(@CodeIndex as varchar(100))
RETURN  ISNULL(@CodeIndex,1)
END 
GO
/****** Object:  UserDefinedFunction [dbo].[splitstring]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[splitstring] ( @stringToSplit VARCHAR(MAX) )
RETURNS
 @returnList TABLE ([Name] [nvarchar] (500))
AS
BEGIN

 DECLARE @name NVARCHAR(255)
 DECLARE @pos INT

 WHILE CHARINDEX(',', @stringToSplit) > 0
 BEGIN
  SELECT @pos  = CHARINDEX(',', @stringToSplit)  
  SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)

  INSERT INTO @returnList 
  SELECT @name

  SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
 END

 INSERT INTO @returnList
 SELECT @stringToSplit

 RETURN
END
GO
/****** Object:  Table [dbo].[Audit]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Audit](
	[AuditID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[CreatedBy] [int] NULL,
	[CommentID] [int] NULL,
	[AssignedTo] [int] NULL,
	[CreatedDate] [datetime] NULL CONSTRAINT [DF_Audit_ModifiedaDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ColumnMapping]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ColumnMapping](
	[ColumnID] [int] IDENTITY(1,1) NOT NULL,
	[ColumnName] [varchar](200) NULL,
 CONSTRAINT [PK_ColumnMapping] PRIMARY KEY CLUSTERED 
(
	[ColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Comments]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Comments](
	[CommentID] [int] IDENTITY(1,1) NOT NULL,
	[InVoiceID] [int] NULL,
	[Comment] [varchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL CONSTRAINT [DF_Comments_CreatedDate]  DEFAULT (getdate()),
	[ModidfiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL CONSTRAINT [DF_Comments_ModifiedDate]  DEFAULT (getdate()),
	[AssignedTo] [int] NULL,
 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CostCenters]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CostCenters](
	[CostCenterID] [int] IDENTITY(1,1) NOT NULL,
	[CostCenterName] [nvarchar](100) NOT NULL,
	[CostCenterDescription] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_CostCenters] PRIMARY KEY CLUSTERED 
(
	[CostCenterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CostUnits]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CostUnits](
	[CostUnitID] [int] IDENTITY(1,1) NOT NULL,
	[CostUnitName] [nvarchar](100) NOT NULL,
	[CostUnitDescription] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_CostUnits] PRIMARY KEY CLUSTERED 
(
	[CostUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CreditNoteDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CreditNoteDetails](
	[CreditNoteId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NULL,
	[CreditFileName] [nvarchar](max) NULL,
	[CreditAmount] [bigint] NULL,
 CONSTRAINT [PK_CreditNoteDetails] PRIMARY KEY CLUSTERED 
(
	[CreditNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DebitNoteDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DebitNoteDetails](
	[DebitNoteId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NULL,
	[DebitFileName] [nvarchar](max) NULL,
	[DebitAmount] [bigint] NULL,
 CONSTRAINT [PK_DebitNoteDetails] PRIMARY KEY CLUSTERED 
(
	[DebitNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Departments]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[DeptID] [int] IDENTITY(1,1) NOT NULL,
	[DeptName] [nvarchar](100) NOT NULL,
	[DeptDescription] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[DeptID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Employees]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[EmployeeID] [int] NOT NULL,
	[CorporateId] [nvarchar](50) NULL,
	[Code] [nvarchar](50) NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Initials] [nvarchar](20) NULL,
	[Gender] [nvarchar](1) NULL,
	[JobTitle] [nvarchar](50) NULL,
	[CompanyCode] [nvarchar](20) NULL,
	[DepartmentName] [nvarchar](80) NULL,
	[LanguageCode] [nvarchar](4) NULL,
	[WorkSchemeReference] [nvarchar](50) NULL,
	[ContractTypeCode] [nvarchar](20) NULL,
	[CostCenterCode] [nvarchar](20) NULL,
	[GradeReference] [nvarchar](60) NULL,
	[FTE] [float] NULL,
	[DateHired] [datetime] NULL,
	[DateOutofDuty] [datetime] NULL,
	[Manager] [nvarchar](50) NULL,
	[Secretary] [nvarchar](50) NULL,
	[AdditionalInfo] [nvarchar](255) NULL,
	[DateofBirth] [datetime] NULL,
	[SocialStatus] [nvarchar](50) NULL,
	[NumberofChildren] [int] NULL,
	[MeansofTransport] [nvarchar](50) NULL,
	[LicensePlate] [nvarchar](50) NULL,
	[DistanceWorkCompany] [float] NULL,
	[OverrideLocation] [bit] NULL,
	[OverridenLocation] [nvarchar](50) NULL,
	[Excludefromstandardsearchlists] [bit] NULL,
	[IsHelpdeskOperator] [bit] NULL,
	[EnabledHelpdeskOperator] [bit] NULL,
	[IsCaller] [bit] NULL,
	[EnabledCaller] [bit] NULL,
	[IsWorkResource] [bit] NULL,
	[EnabledWorkResource] [bit] NULL,
	[IsReceptionist] [bit] NULL,
	[IsSecretary] [bit] NULL,
	[IsContactPerson] [bit] NULL,
	[DefaultActivityType] [nvarchar](50) NULL,
	[FixedProductorService] [nvarchar](50) NULL,
	[Address1] [nvarchar](50) NULL,
	[Address2] [nvarchar](50) NULL,
	[ZipCode] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[State/Province] [nvarchar](50) NULL,
	[Country (ISO code)] [nvarchar](50) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[PhoneExtension] [nvarchar](50) NULL,
	[PhoneDescription] [nvarchar](50) NULL,
	[MobilePhone] [nvarchar](50) NULL,
	[MobilePhoneExtension] [nvarchar](50) NULL,
	[MobilePhoneDescription] [nvarchar](50) NULL,
	[FaxNumber] [nvarchar](50) NULL,
	[FaxExtension] [nvarchar](50) NULL,
	[FaxDescription] [nvarchar](50) NULL,
	[E-mail] [nvarchar](50) NULL,
	[E-mailDescription] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InVoice]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InVoice](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [varchar](50) NULL,
	[PANNumber] [varchar](50) NULL,
	[PONumber] [varchar](50) NULL,
	[InvoiceDate] [datetime] NULL,
	[InvoiceReceivedDate] [datetime] NULL,
	[InvoiceDueDate] [datetime] NULL,
	[TaxTypeID] [int] NULL,
	[VendorID] [int] NULL,
	[DepartmentID] [int] NULL,
	[CostCenterID] [int] NULL,
	[CostUnitID] [int] NULL,
	[EndUserStatus] [varchar](5) NULL,
	[EndUserApprovalStatus] [varchar](5) NULL,
	[InvoiceAmount] [float] NULL,
	[FinalApprovalStatus] [varchar](5) NULL,
	[InitiatorVerificationStatus] [varchar](5) NULL,
	[WFStatus] [int] NULL,
	[Initiator] [varchar](5) NULL,
	[Dateofpayment] [datetime] NULL,
	[DateofAccount] [datetime] NULL,
	[Navisionvoucherno] [varchar](25) NULL,
	[EndUserID] [int] NULL CONSTRAINT [DF_InVoice_EndUserID]  DEFAULT ((0)),
	[EndUserApprovalID] [int] NULL CONSTRAINT [DF_InVoice_EndUserApprovalID]  DEFAULT ((0)),
	[FinalApprovalID] [int] NULL CONSTRAINT [DF_InVoice_FinalApprovalID]  DEFAULT ((0)),
	[CreatedBy] [int] NULL CONSTRAINT [DF_InVoice_CreatedBy]  DEFAULT ((0)),
	[CreatedDate] [datetime] NULL CONSTRAINT [DF_InVoice_CreatedDate]  DEFAULT (getdate()),
	[ReceiptNo] [varchar](50) NULL,
	[CurrentUserID] [int] NULL,
	[CurrentStatus] [varchar](50) NULL,
	[FilePath] [varchar](max) NULL,
	[GSTIN] [varchar](50) NULL,
	[PlaceOfSupply] [varchar](50) NULL,
	[ReverseCharges] [varchar](50) NULL,
	[EcommerceGSTIN] [varchar](50) NULL,
	[TaxPercent] [varchar](50) NULL,
	[TaxableValue] [varchar](50) NULL,
	[CessAmount] [varchar](50) NULL,
 CONSTRAINT [PK_InVoice] PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InVoiceFiles]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InVoiceFiles](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[FileName] [varchar](50) NULL,
	[FileType] [varchar](10) NULL,
 CONSTRAINT [PK_InVoiceFiles] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InvoiceItems]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [varchar](50) NULL,
	[Code] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
	[Qty] [varchar](50) NULL,
	[Rate] [varchar](50) NULL,
	[Amount] [varchar](50) NULL,
	[IGST] [varchar](50) NULL,
	[SGST] [varchar](50) NULL,
	[CGST] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InvoiceStatus]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NULL,
	[UserTypeId] [int] NULL,
	[UserId] [int] NULL,
	[StatusId] [int] NULL,
	[InvoiceStatus] [varchar](50) NULL,
 CONSTRAINT [PK_InvoiceStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InvoiceStatusType]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceStatusType](
	[StatudID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](100) NOT NULL,
 CONSTRAINT [PK_InvoiceStatusType] PRIMARY KEY CLUSTERED 
(
	[StatudID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InvoiceTax]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceTax](
	[InvoiceTaxTypeID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[TaxtypeID] [int] NULL,
	[TaxValue] [int] NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_InvoiceTax] PRIMARY KEY CLUSTERED 
(
	[InvoiceTaxTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemCodes]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemCodes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Reference] [varchar](200) NULL,
	[Default Classification_Code] [varchar](200) NULL,
	[LastIndex] [int] NULL CONSTRAINT [DF_ItemCodes_LastIndex]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ObjCategory]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ObjCategory](
	[ID] [int] NOT NULL,
	[Reference] [nvarchar](100) NULL,
	[DefaultClassification] [nvarchar](50) NULL,
 CONSTRAINT [PK_ObjCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ObjectGroups]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ObjectGroups](
	[ID] [int] NOT NULL,
	[ObjectGroupDescription] [nvarchar](100) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderItemDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderItemDetails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemID] [int] NULL,
	[ItemName] [varchar](200) NULL,
	[Price] [varchar](200) NULL,
	[Qty] [varchar](200) NULL,
	[Total] [varchar](200) NULL,
	[Tax] [varchar](200) NULL,
	[Asset] [bit] NULL,
	[Code] [varchar](10) NULL,
 CONSTRAINT [PK_ProductItemDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderItemDetailsNew]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderItemDetailsNew](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemId] [int] NULL,
	[ColumnName] [varchar](100) NULL,
	[RowNo] [int] NULL,
	[ColumnValue] [varchar](200) NULL,
 CONSTRAINT [PK_OrderItemDetailsNew] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Vendor] [nvarchar](200) NULL,
	[InvoiceNo] [nvarchar](100) NULL,
	[InvoiceDate] [datetime] NULL,
	[PO] [nvarchar](100) NULL,
	[PODate] [datetime] NULL,
	[Amount] [float] NULL,
	[PAN] [nvarchar](100) NULL,
	[BUYERCSTNO] [nvarchar](100) NULL,
	[BUYERVATTIN] [nvarchar](100) NULL,
	[COMPANYCSTNO] [nvarchar](100) NULL,
	[COMPANYVATTIN] [nvarchar](100) NULL,
	[ImageFilePath] [nvarchar](2000) NULL,
	[Dateofpayment] [date] NULL,
	[InvoiceDueDate] [date] NULL,
 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrder](
	[POID] [int] IDENTITY(1,1) NOT NULL,
	[PODate] [datetime] NULL,
	[PONumber] [nvarchar](max) NULL,
	[POAmount] [decimal](18, 0) NULL,
	[Createdby] [int] NULL,
	[Createddate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[POFileName] [nvarchar](max) NULL,
 CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY CLUSTERED 
(
	[POID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SiteReferenceDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiteReferenceDetails](
	[SiteID] [int] NULL,
	[SiteReference] [nvarchar](255) NULL,
	[BuildingReference] [nvarchar](255) NULL,
	[FloorReference] [nvarchar](255) NULL,
	[RoomReference] [nvarchar](255) NULL,
	[Room Reference] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suppliers](
	[ID] [float] NULL,
	[Code] [nvarchar](50) NULL,
	[SupplierName] [nvarchar](100) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SystemConfiguration]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfiguration](
	[SystemID] [int] IDENTITY(1,1) NOT NULL,
	[SmtpServer] [nvarchar](255) NULL,
	[OutgoingPortNo] [int] NULL,
	[CompanyEmailFrom] [nvarchar](255) NULL,
	[CompanyEmailFromPassword] [nvarchar](255) NULL,
	[SSL] [bit] NULL,
 CONSTRAINT [PK_SystemConfiguration] PRIMARY KEY CLUSTERED 
(
	[SystemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TaxTypes]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxTypes](
	[TaxTypeID] [int] IDENTITY(1,1) NOT NULL,
	[TaxTypeName] [nvarchar](100) NOT NULL,
	[TaxTypeDescription] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_TaxTypes] PRIMARY KEY CLUSTERED 
(
	[TaxTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[UserPassword] [varbinary](255) NULL,
	[UserTypeID] [int] NOT NULL,
	[DepartmentID] [int] NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[MiddleName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[EmailAddress] [nvarchar](75) NULL,
	[IsAdmin] [bit] NOT NULL CONSTRAINT [DF__Users__IsAdmin__0AD2A005]  DEFAULT ((0)),
	[Disabled] [bit] NOT NULL CONSTRAINT [DF__Users__Disabled__0BC6C43E]  DEFAULT ((0)),
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime2](7) NOT NULL CONSTRAINT [DF__Users__CreatedDa__0CBAE877]  DEFAULT (getdate()),
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime2](7) NOT NULL CONSTRAINT [DF__Users__ModifiedD__0DAF0CB0]  DEFAULT (getdate()),
	[UserImage] [nvarchar](100) NULL,
	[IsFinance] [bit] NULL,
	[IsVendor] [bit] NULL,
	[IsPaymentApproval] [bit] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserTypes]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTypes](
	[UserTypeID] [int] IDENTITY(1,1) NOT NULL,
	[UserType] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL DEFAULT ((0)),
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT (getdate()),
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[UserID] [int] NULL,
	[ColumnOrder] [int] NULL,
 CONSTRAINT [PK_UserTypes] PRIMARY KEY CLUSTERED 
(
	[UserTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserTypesMapping]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTypesMapping](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserTypeID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_UserTypesMapping] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[VendorCodes]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VendorCodes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NULL,
	[Name] [varchar](200) NULL,
 CONSTRAINT [PK_VendorCodes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VendorRating]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VendorRating](
	[VendorRatingID] [int] IDENTITY(1,1) NOT NULL,
	[VendorID] [int] NULL,
	[InvoiceID] [int] NULL,
	[Rating] [int] NOT NULL,
	[Comment] [varchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_VendorRating] PRIMARY KEY CLUSTERED 
(
	[VendorRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendors](
	[VendorID] [int] IDENTITY(1,1) NOT NULL,
	[VendorName] [nvarchar](100) NOT NULL,
	[VendorDescription] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED 
(
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_CostCenters] FOREIGN KEY([CostCenterID])
REFERENCES [dbo].[CostCenters] ([CostCenterID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_CostCenters]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_CostUnits] FOREIGN KEY([CostUnitID])
REFERENCES [dbo].[CostUnits] ([CostUnitID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_CostUnits]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([DeptID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_Departments]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_Users] FOREIGN KEY([EndUserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_Users]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_Users1] FOREIGN KEY([EndUserApprovalID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_Users1]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_Vendors] FOREIGN KEY([VendorID])
REFERENCES [dbo].[Vendors] ([VendorID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_Vendors]
GO
ALTER TABLE [dbo].[InVoiceFiles]  WITH CHECK ADD  CONSTRAINT [FK_InVoiceFiles_InVoice] FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[InVoice] ([InvoiceID])
GO
ALTER TABLE [dbo].[InVoiceFiles] CHECK CONSTRAINT [FK_InVoiceFiles_InVoice]
GO
ALTER TABLE [dbo].[InvoiceTax]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceTax_TaxTypes] FOREIGN KEY([TaxtypeID])
REFERENCES [dbo].[TaxTypes] ([TaxTypeID])
GO
ALTER TABLE [dbo].[InvoiceTax] CHECK CONSTRAINT [FK_InvoiceTax_TaxTypes]
GO
ALTER TABLE [dbo].[UserTypesMapping]  WITH CHECK ADD  CONSTRAINT [FK_UserTypesMapping_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserTypesMapping] CHECK CONSTRAINT [FK_UserTypesMapping_Users]
GO
ALTER TABLE [dbo].[UserTypesMapping]  WITH CHECK ADD  CONSTRAINT [FK_UserTypesMapping_UserTypes] FOREIGN KEY([UserTypeID])
REFERENCES [dbo].[UserTypes] ([UserTypeID])
GO
ALTER TABLE [dbo].[UserTypesMapping] CHECK CONSTRAINT [FK_UserTypesMapping_UserTypes]
GO
/****** Object:  StoredProcedure [dbo].[CheckCredential]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Script Template  
-- =============================================  
CREATE PROCEDURE [dbo].[CheckCredential]        
 @UserName nvarchar(50),  
 @UserPassword nvarchar(255)
         
  
AS            
       declare @isVendor  bit =0  
 SET NOCOUNT ON            
 SET ANSI_WARNINGS OFF     
  --Declare @exists int;         
  --Select @exists = COUNT(*) from Users where UserName=@UserName  and PWDCOMPARE(@UserPassword,UserPassword) = 1 
  --if(@exists = 1)   
  --begin 
  if exists(Select 1 From Users where UserName=@UserName and [Disabled]=0  )
  BEGIN
Select PWDCOMPARE (@UserPassword,UserPassword) as IsValid,  
     UserID,  
     UserName,  
     DepartmentID,  
     UserTypeID,  
     FirstName,  
     MiddleName,  
     LastName,  
     EmailAddress,  
     UserImage,  
     [Disabled],  
     IsAdmin,
	 IsFinance,
	@isVendor as IsVendor ,
	IsPaymentApproval   
From Users where UserName=@UserName and [Disabled]=0  
END
ELSE
SELECT 0 as IsValid
--end 
--else
--begin
--select null
--end

GO
/****** Object:  StoredProcedure [dbo].[CheckInvoiceAmount]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[CheckInvoiceAmount]    Script Date: 24-04-2017 16:10:30 ******/
CREATE procedure [dbo].[CheckInvoiceAmount] 
(
	@PONumber varchar(max),
	@poamount decimal = 0
)
as

Declare @Total decimal,@POTotal decimal

select @Total=Sum(InvoiceAmount) from invoice where PONumber=@PONumber

set @Total=@Total +isnull(@poamount,0)
select @POTotal=POAmount from PurchaseOrder where  PONumber=@PONumber
if isnull(@POTotal,0)= 0
begin
	select 3
end
else if isnull(@Total,0) > isnull(@POTotal,0)
begin
	select 0
end
else
begin
	select 1
end




GO
/****** Object:  StoredProcedure [dbo].[DeleteProductItemDetailsValue]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteProductItemDetailsValue]
(
	@ID int
)
AS

DECLARE @OrderItemID int
SELECT @OrderItemID = OrderItemID from OrderItemDetails WHERE ID=@ID
DELETE FROM OrderItemDetails WHERE ID = @ID
select @OrderItemID AS OrderItemID



--select * from OrderItems
--select * from OrderItemDetails
GO
/****** Object:  StoredProcedure [dbo].[GetAllOrderwithItemDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetAllOrderwithItemDetails]
as
select 
o.ID as [Maintenance Part ID],
'' as [Maintenance Object (ID)],
'' as [Parent Part (Reference)],
oid.ItemName as [Part Type (Reference)],
'' as [Code],
oid.ItemName as [Reference],
'Existing' as [Status (Code)],
'' as [Nature (Code)],	
'' as [Power],
'' as [Warranty],	
'' as [Warranty Date],
'Location' as [Site (Reference)],
'' as [Building (Reference)],
'' as [Floor (Reference)],
'' as [Room (Reference)],
'' as [Land (Reference)],
'' as [Outside Location (Reference)],
'' as [Symbol (Reference)],
'' as [Investment Date],
'' as [Construction Date],
'' as [Expected Lifetime (Years)],
'' as [Overridden Replacement Year],
'' as [Expected Replacement Amount],
'' as [Expected Replacement Currency],
'' as [Yearly Cost],
'' as [Yearly Cost Currency],
o.PO as [udi.U_PO_NUMBER],
o.InvoiceDate as [udi.U_PO_DATE],
o.InvoiceNo as [udi.U_INVOICE_NUMBER],
o.InvoiceDate as [udi.U_INVOICE_DATE],
oid.Total as [udi.U_PO_AMOUNT],
oid.Total as [udi.U_INVOICE_AMOUNT]


--o.Vendor, o.InvoiceNo, o.InvoiceDate, o.PO, o.Amount, o.PAN, o.BUYERCSTNO,
--o.BUYERVATTIN, o.COMPANYCSTNO, o.COMPANYVATTIN,
--oid.ItemName, oid.Price, oid.Qty,oid.Tax, oid.Total, oid.asset
from OrderItems o
inner join Orderitemdetails oid
on o.ID = oid.orderitemid

GO
/****** Object:  StoredProcedure [dbo].[GetAllOrderwithItemDetails1]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[GetAllOrderwithItemDetails1]
as
select 
o.ID, o.Vendor, o.InvoiceNo, o.InvoiceDate, o.PO, o.Amount, o.PAN, o.BUYERCSTNO,
o.BUYERVATTIN, o.COMPANYCSTNO, o.COMPANYVATTIN,
oid.ItemName, oid.Price, oid.Qty,oid.Tax, oid.Total, oid.asset
from OrderItems o
inner join Orderitemdetails oid
on o.ID = oid.orderitemid

GO
/****** Object:  StoredProcedure [dbo].[GetAssetItemDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[GetAssetItemDetails]
as

create table #IDs(
pid int IDENTITY (1,1),
id int
)
declare @indexTbl TABLE (pid int,id int,i_index int)
declare @tempCodes TABLE (Id int ,code  varchar(10), indexNo int)
declare @results varchar(max)
select @results = coalesce(@results + '', '') +  replicate(cast(ID as varchar(10)) + ',',Round(convert(float , ISNULL(Qty,0)),0))
from orderitemdetails where asset=1
--order by col
PRINT @results

insert into #IDs(id) select Name from dbo.splitstring(@results)
insert into @indexTbl(pid,id,i_index) Select pid,id,ROW_NUMBER() OVER(PARTITION BY [id] ORDER BY [id]) from #IDs
--insert into @tempCodes(Id,code,indexNo) select i.id,oid.Code,ROW_NUMBER() OVER(PARTITION BY oid.LastCodeIndex ORDER BY oid.LastCodeIndex) from orderitemdetails oid inner join @indexTbl i on oid.ID = i.id
--select * from @tempCodes
insert into @tempCodes(Id,code,indexNo)
SELECT i.id, Code,
    s_index = [dbo].[fn_GetCodeIndex] (code) +  ROW_NUMBER() OVER(PARTITION BY [Code] ORDER BY [Code])
	--,    t_index = DENSE_RANK() OVER (ORDER BY [Code])
	from orderitemdetails oid inner join #IDs i on oid.ID = i.id --and Code is not null
select 
oid.ID as [Maintenance Object ID]
,oid.ItemName as [Category (Reference)]
--,LEFT(oid.ItemName, 2) + CAST(i.i_index as varchar(20)) as [Code]
,ISNULL(oid.Code, 'DEFAULT') + CAST(i.indexNo as varchar(20)) as [Code]
,oid.ItemName as [Reference]
,oid.ItemName as [Description]
,'' as [Remarks]	
,'' as [Prevention Notes]
,'Technical Object' as [Class]
,'EXISTING' as [Status (Code)]
,'NA' as [Default Nature (Code)]
,'' as [Maintenance Object Group (Code)]
,'' as [Serial Number]
,'' as [Brand]
,'' as [Power]	
,'' as [Bluetooth Tag]
,'' as [Warranty]
,'' as [Warranty Date]
---,'SUP_SSMSPL' as [Supplier (Code)]
,CASE WHEN S.SupplierName IS NULL THEN 'n/a' ELSE S.Code END as [Supplier (Code)]
,'Jigni' as [Site (Reference)]	
,'Ground Floor' as [Building (Reference)]
,'' as [Floor (Reference)]
,'' as [Room (Reference)]	
,'' as [Land (Reference)]	
,'' as [Outside Location (Reference)]
,'' as [Energy Object (Reference)]
,'' as [Stock (Description)]
,'' as [Symbol (Reference)]
,'' as [Classification (Code)]
,'' as [Planner Type]
,'' as [Default Planner (Corporate Id)]
,'' as [Operator Type]
,'' as [Default Operator (Key Field Value)]
,'' as [Default Task Force (Code)]
,'' as [Default Project (Code)]
,'' as [Default Project Part (Code)]
,'' as [Cost Center (Code)]
,'' as [Cost Center Fiscal Entity (Code)]
,'' as [Default Cost Category (Code)]
,'' as [Investment Date]
,'' as [Construction Date]
,'' as [Expected Lifetime (Years)]
,'' as [Overridden Replacement Year]
,'' as [Expected Replacement Amount]
,'' as [Expected Replacement Currency]
,'' as [Yearly Cost]
,'' as [Yearly Cost Currency]
,'' as [Contract Code]
,'' as [Contract Reference]
,'' as [Contract Status (Reference)]
,'' as [Contract Number]
,'' as [Contractor (Code)]
,'' as [Contact Person 1]	
,'' as [Contract Remark 1]	
,'' as [Contact Person 2]	
,'' as [Contract Remark 2]	
,'' as [Scan Code]
,'' as [Criticality (Code)]
,'' as [Access Instruction]	
,o.InvoiceNo as [udi.U_INVOICE_NO_]	
,o.PO as [udi.U_PO_NUMBER]
,o.InvoiceDate as [udi.U_PO_DATE]
,'' as [udi.U_INITIAL_BOOK_VALUE]
,'Purchased' as [udi.U_OBTAINED_BY]
,'INR' as [udi.U_CURRENCY]
,o.InvoiceDate as [udi.U_DATE_BOUGHT]
,oid.Price as [udi.U_LEASE_AMOUNT]	
,'' as [udi.U_END_LEASE_DATE]


--o.Vendor, o.InvoiceNo, o.InvoiceDate, o.PO, o.Amount, o.PAN, o.BUYERCSTNO,
--o.BUYERVATTIN, o.COMPANYCSTNO, o.COMPANYVATTIN,
--oid.ItemName, oid.Price, oid.Qty,oid.Tax, oid.Total, oid.asset
from orderitemdetails oid inner join @tempCodes i on oid.ID = i.id
inner join OrderItems o on o.ID = oid.orderitemid 
LEFT JOIN Suppliers S ON S.SupplierName= o.Vendor
where oid.asset=1
drop table #IDs

GO
/****** Object:  StoredProcedure [dbo].[GetAssetItemDetails_1]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[GetAssetItemDetails_1]
as

create table #IDs(
pid int IDENTITY (1,1),
id int
)
declare @indexTbl TABLE (pid int,id int,i_index int)
declare @tempCodes TABLE (Id int ,code  varchar(10), indexNo int)
declare @results varchar(max)
select @results = coalesce(@results + '', '') +  replicate(cast(ID as varchar(10)) + ',',Round(convert(float , ISNULL(Qty,0)),0))
from orderitemdetails where asset=1
--order by col
PRINT @results

insert into #IDs(id) select Name from dbo.splitstring(@results)
insert into @indexTbl(pid,id,i_index) Select pid,id,ROW_NUMBER() OVER(PARTITION BY [id] ORDER BY [id]) from #IDs
--insert into @tempCodes(Id,code,indexNo) select i.id,oid.Code,ROW_NUMBER() OVER(PARTITION BY oid.LastCodeIndex ORDER BY oid.LastCodeIndex) from orderitemdetails oid inner join @indexTbl i on oid.ID = i.id
--select * from @tempCodes
insert into @tempCodes(Id,code,indexNo)
SELECT i.id, Code,
    s_index = [dbo].[fn_GetCodeIndex] (code) +  ROW_NUMBER() OVER(PARTITION BY [Code] ORDER BY [Code])
	--,    t_index = DENSE_RANK() OVER (ORDER BY [Code])
	from orderitemdetails oid inner join #IDs i on oid.ID = i.id --and Code is not null
select 
oid.ID as [Maintenance Object ID]
,oid.ItemName as [Category (Reference)]
--,LEFT(oid.ItemName, 2) + CAST(i.i_index as varchar(20)) as [Code]
,ISNULL(oid.Code, 'DEFAULT') + CAST(i.indexNo as varchar(20)) as [Code]
,oid.ItemName as [Reference]
,oid.ItemName as [Description]
,'' as [Remarks]	
,'' as [Prevention Notes]
,'Technical Object' as [Class]
,'EXISTING' as [Status (Code)]
,'NA' as [Default Nature (Code)]
,'' as [Maintenance Object Group (Code)]
,'' as [Serial Number]
,'' as [Brand]
,'' as [Power]	
,'' as [Bluetooth Tag]
,'' as [Warranty]
,'' as [Warranty Date]
---,'SUP_SSMSPL' as [Supplier (Code)]
,CASE WHEN S.SupplierName IS NULL THEN 'n/a' ELSE S.Code END as [Supplier (Code)]
,'Jigni' as [Site (Reference)]	
,'Ground Floor' as [Building (Reference)]
,'' as [Floor (Reference)]
,'' as [Room (Reference)]	
,'' as [Land (Reference)]	
,'' as [Outside Location (Reference)]
,'' as [Energy Object (Reference)]
,'' as [Stock (Description)]
,'' as [Symbol (Reference)]
,'' as [Classification (Code)]
,'' as [Planner Type]
,'' as [Default Planner (Corporate Id)]
,'' as [Operator Type]
,'' as [Default Operator (Key Field Value)]
,'' as [Default Task Force (Code)]
,'' as [Default Project (Code)]
,'' as [Default Project Part (Code)]
,'' as [Cost Center (Code)]
,'' as [Cost Center Fiscal Entity (Code)]
,'' as [Default Cost Category (Code)]
,'' as [Investment Date]
,'' as [Construction Date]
,'' as [Expected Lifetime (Years)]
,'' as [Overridden Replacement Year]
,'' as [Expected Replacement Amount]
,'' as [Expected Replacement Currency]
,'' as [Yearly Cost]
,'' as [Yearly Cost Currency]
,'' as [Contract Code]
,'' as [Contract Reference]
,'' as [Contract Status (Reference)]
,'' as [Contract Number]
,'' as [Contractor (Code)]
,'' as [Contact Person 1]	
,'' as [Contract Remark 1]	
,'' as [Contact Person 2]	
,'' as [Contract Remark 2]	
,'' as [Scan Code]
,'' as [Criticality (Code)]
,'' as [Access Instruction]	
,o.InvoiceNo as [udi.U_INVOICE_NO_]	
,o.PO as [udi.U_PO_NUMBER]
,o.InvoiceDate as [udi.U_PO_DATE]
,'' as [udi.U_INITIAL_BOOK_VALUE]
,'Purchased' as [udi.U_OBTAINED_BY]
,'INR' as [udi.U_CURRENCY]
,o.InvoiceDate as [udi.U_DATE_BOUGHT]
,oid.Price as [udi.U_LEASE_AMOUNT]	
,'' as [udi.U_END_LEASE_DATE]


--o.Vendor, o.InvoiceNo, o.InvoiceDate, o.PO, o.Amount, o.PAN, o.BUYERCSTNO,
--o.BUYERVATTIN, o.COMPANYCSTNO, o.COMPANYVATTIN,
--oid.ItemName, oid.Price, oid.Qty,oid.Tax, oid.Total, oid.asset
from orderitemdetails oid inner join @tempCodes i on oid.ID = i.id
inner join OrderItems o on o.ID = oid.orderitemid 
LEFT JOIN Suppliers S ON S.SupplierName= o.Vendor
where oid.asset=1
drop table #IDs

GO
/****** Object:  StoredProcedure [dbo].[GetOrderDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[GetOrderDetails]
as
select ID as OrderID, Vendor, InvoiceNo, InvoiceDate, PO, Amount, PAN, BUYERCSTNO, BUYERVATTIN, COMPANYCSTNO, COMPANYVATTIN from dbo.OrderItems

Select  OrderItemID as OrderID, ItemName, Price, Qty, Total from dbo.OrderItemDetails

GO
/****** Object:  StoredProcedure [dbo].[GetOrderDetailsGST]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[GetOrderDetailsGST]
as
Select GSTIN as [GSTIN/UIN of Recipient], InvoiceNumber as [Invoice Number], InvoiceDate as [Invoice date], InvoiceAmount as  [Invoice Value],PlaceOfSupply as [Place Of Supply],CASE ReverseCharges WHEN 0 Then 'N' ELSE 'Y' END as [Reverse Charge] ,'Regular' as [Invoice Type], EcommerceGSTIN as [E-Commerce GSTIN],TaxPercent as Rate,TaxableValue as [Taxable Value],CessAmount as [Cess Amount] from InVoice where invoiceid>10430

--Select  OrderItemID as OrderID, ItemName, Price, Qty, Total from dbo.OrderItemDetails

										



--select * from invoice where invoiceid=10431





GO
/****** Object:  StoredProcedure [dbo].[GetOrderItems]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[GetOrderItems]

as
select * from dbo.OrderItems

GO
/****** Object:  StoredProcedure [dbo].[GetOrderItemsBYID]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[GetOrderItemsBYID]
(@ID int
) 
as
select * from dbo.OrderItems Where ID=@ID
Select * from dbo.OrderItemdetails where OrderitemID=@ID

GO
/****** Object:  StoredProcedure [dbo].[GetOrderItemsBYIDNew]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--USE [IQInvoiceItem]
--GO
--/****** Object:  StoredProcedure [dbo].[GetOrderItemsBYIDNew]    Script Date: 06-Feb-17 3:56:32 PM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
CREATE Procedure [dbo].[GetOrderItemsBYIDNew] 
(
	@ID int
) 
as
--declare @ID int= 1
DECLARE @rowCnt int
select @rowCnt= max(RowNo) From OrderItemDetailsNew WHERE OrderItemId=@id


select * from dbo.OrderItems where ID=@ID
declare @scriptTemplate varchar(MAX)
declare @script varchar(MAX)
declare @tableTemplate varchar(MAX)
SET @tableTemplate = 'create table ##tmptable (?)'
SET @scriptTemplate = '? varchar(500),'
SET @script = 'OrderItemID int, RowNo int , '
Select @script = @script + Replace(@scriptTemplate, '?', [ColumnName]) From ColumnMapping
SET @script = LEFT(@script, LEN(@script) - 1)
SET @script = Replace(@tableTemplate, '?', @script)
--Select @script
exec(@script)

declare @counter int =1

WHILE (@counter <= @rowCnt)
BEGIN

declare @scriptTemplate1 varchar(MAX)
declare @scriptTemplate2 varchar(MAX)
declare @script1 varchar(MAX)
declare @script2 varchar(MAX)
declare @tableTemplate1 varchar(MAX)
SET @tableTemplate1 = 'insert into  ##tmptable(OrderItemID,RowNo,?) values($)'
SET @scriptTemplate1 = '?, '
SET @scriptTemplate2 = '$, '
SET @script1 = ''--'OrderItemID , '
SET @script2= ''--@ID

Select @script1 = @script1 + Replace(@scriptTemplate1, '?', [ColumnName]) From ColumnMapping
Select @script2 = @script2  + Replace(@scriptTemplate2, '$', ''''+  ISNULL([ColumnValue],'') + '''') From OrderItemDetailsNew where RowNo=@counter
Select @script2 =  Cast(@ID as varchar(10)) +',' + Cast(@counter as varchar(10)) +',' + @script2 
SET @script1 = LEFT(@script1, LEN(@script1) - 1)

--SET @script2 = Replace(@script2, ',', ''',''') 
SET @script2 = LEFT(@script2, LEN(@script2) - 1)
SET @script1 = Replace(@tableTemplate1, '?', @script1)
SET @script1 = Replace(@script1, '$', @script2)
SET @script1 = Replace(@script1,'null', '')

exec(@script1)

set @counter =@counter+1
END

Select * from ##tmptable
Drop table ##tmptable

GO
/****** Object:  StoredProcedure [dbo].[getUserTypes]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[getUserTypes]
AS
select UserTypeID,UserType from UserTypes WHERE Disabled=0
GO
/****** Object:  StoredProcedure [dbo].[InsertInvoiceItemDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[InsertInvoiceItemDetails] 
(

@InvoiceID varchar(100) = null,  
@InvoiceNumber varchar(100) = null,
@InvoiceAmount varchar(100) = null,
@PONumber varchar(100) = null,
@PANNumber varchar(100) = null,
@VendorName varchar(100) = null, 
@InvoiceDate datetime = null,
@InvoiceDueDate datetime = null,
@InvoiceReceiveddate datetime = null,
@Dateofpayment datetime = null,
@DateofAccount datetime = null,
@ItemTable as TableInvoiceStatus READONLY
)
as
--select  [dbo].[fn_GetCodeIndex] ('af', '10')
Declare @ID int
INSERT INTO [dbo].[InVoice]([InvoiceNumber],[PANNumber],[PONumber],[InvoiceDate],[InvoiceDueDate]      ,[VendorID]
           ,[Dateofpayment]
           ,[DateofAccount]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[InvoiceReceivedDate])
     VALUES(@InvoiceNumber,@PANNumber,@PONumber,@InvoiceDate,@InvoiceDueDate, 111,@Dateofpayment,@DateofAccount,1,getdate(),@InvoiceReceiveddate)
select  @ID =SCOPE_IDENTITY()

Select * into #temptable from @ItemTable
--select * from #temptable

--update #temptable set LastCodeIndex = [dbo].[fn_GetCodeIndex] (code,Qty) 
insert into [dbo].[InvoiceStatus] ([InvoiceId],[UserId],[UserTypeId]) select @ID, UserId,UserTypeId from #temptable









GO
/****** Object:  StoredProcedure [dbo].[InsertOrderItemDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[InsertOrderItemDetails] 
(
@rowNo int,
@colId int,
@columnVal varchar(200)=null


)
as
BEGIN

declare @ColumnName varchar(50)
select @ColumnName= ColumnName from ColumnMapping where ColumnID=@colId
INSERT INTO [dbo].[OrderItemDetailsNew]
           ([OrderItemId]
           ,[ColumnName]
           ,[RowNo]
           ,[ColumnValue])
     VALUES
           (2,@ColumnName,@rowNo,@columnVal)

END
GO
/****** Object:  StoredProcedure [dbo].[InsertProductItemDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE procedure [dbo].[InsertProductItemDetails] 
(

@Vendor nvarchar(200)=null,
@InvoiceNo nvarchar(100)=null,
@InvoiceDate datetime =null,
@PO nvarchar(100) =null,
@PODate datetime =null,
@Amount nvarchar(100)=null,
@PAN  nvarchar(100)=null,
@ItemTable as TableValuedType READONLY,
@BUYERCSTNO  nvarchar(100)=null,
@BUYERVATTIN  nvarchar(100)=null,
@COMPANYCSTNO  nvarchar(100)=null,
@COMPANYVATTIN  nvarchar(100)=null,
@ImageFilePath nvarchar(2000) =null

)
as
--select  [dbo].[fn_GetCodeIndex] ('af')
Declare @OrderItemID int

Insert into OrderItems( Vendor,InvoiceNo, InvoiceDate, PO,PODate,Amount,PAN,BUYERCSTNO,BUYERVATTIN,COMPANYCSTNO,COMPANYVATTIN,ImageFilePath) values
(@Vendor,@InvoiceNo,@InvoiceDate,@PO,@PODate,@Amount,@PAN,@BUYERCSTNO,@BUYERVATTIN,@COMPANYCSTNO,@COMPANYVATTIN,@ImageFilePath)

Set @OrderItemID =SCOPE_IDENTITY()

Select * into #temptable from @ItemTable
--select * from #temptable
insert into [dbo].[OrderItemDetails]([OrderItemID],[ItemName],[Price],[Qty],[Tax],[Total],[Asset],[Code],[LastCodeIndex]) select @OrderItemID, * from #temptable








GO
/****** Object:  StoredProcedure [dbo].[InsertProductItemDetails_1]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE procedure [dbo].[InsertProductItemDetails_1] 
(

@Vendor nvarchar(200)=null,
@InvoiceNo nvarchar(100)=null,
@InvoiceDate datetime =null,
@PO nvarchar(100) =null,
@PODate datetime =null,
@Amount nvarchar(100)=null,
@PAN  nvarchar(100)=null,
@ItemTable as TableValuedType READONLY,
@BUYERCSTNO  nvarchar(100)=null,
@BUYERVATTIN  nvarchar(100)=null,
@COMPANYCSTNO  nvarchar(100)=null,
@COMPANYVATTIN  nvarchar(100)=null,
@ImageFilePath nvarchar(2000) =null

)
as
--select  [dbo].[fn_GetCodeIndex] ('af', '10')
Declare @OrderItemID int

Insert into OrderItems( Vendor,InvoiceNo, InvoiceDate, PO,PODate,Amount,PAN,BUYERCSTNO,BUYERVATTIN,COMPANYCSTNO,COMPANYVATTIN,ImageFilePath) values
(@Vendor,@InvoiceNo,@InvoiceDate,@PO,@PODate,@Amount,@PAN,@BUYERCSTNO,@BUYERVATTIN,@COMPANYCSTNO,@COMPANYVATTIN,@ImageFilePath)

Set @OrderItemID =SCOPE_IDENTITY()

Select * into #temptable from @ItemTable
select * from #temptable

update #temptable set LastCodeIndex = [dbo].[fn_GetCodeIndex] (code,Qty) 
insert into [dbo].[OrderItemDetails]([OrderItemID],[ItemName],[Price],[Qty],[Tax],[Total],[Asset],[Code],[LastCodeIndex]) select @OrderItemID, * from #temptable








GO
/****** Object:  StoredProcedure [dbo].[InsertProductItemDetailsValue]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[InsertProductItemDetailsValue]
(
	@OrderItemID int
)
as
INSERT INTO [dbo].[OrderItemDetails]
           ([OrderItemID]
           ,[ItemName]
           ,[Price]
           ,[Qty]
           ,[Total]
           ,[Tax]
           ,[Asset])
     VALUES
	 (@OrderItemID,null,null,null,null,null,0)


GO
/****** Object:  StoredProcedure [dbo].[InsertUserTypeDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[InsertUserTypeDetails] 
(

@InvoiceID int,
@ItemTable as TableInvoiceStatus READONLY
)
as

Declare @status int = 0;
Select * into #temptable from @ItemTable
--select * from #temptable
INSERT INTO [dbo].[InvoiceStatus]
           ([InvoiceId],[UserTypeId],[UserId])
select @InvoiceID, * from #temptable


GO
/****** Object:  StoredProcedure [dbo].[PasswordReset]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Script Template
-- =============================================
CREATE PROCEDURE [dbo].[PasswordReset]                   
 @UserName nvarchar(50),
 @UserEmail nvarchar(50)        

AS          
          
 SET NOCOUNT ON          
 SET ANSI_WARNINGS OFF          

DECLARE @UserID as int
DECLARE @NewPassword varchar(50)


SELECT @UserID = UserID FROM Users WHERE EmailAddress=@UserEmail Or UserName=@UserName
IF (@UserID is NULL)
set @NewPassword='0'
	
IF @UserID is not NULL
Begin
	select @NewPassword = char(rand()*26+65)+char(rand()*26+65)+char(rand()*26+65)+char(rand()*26+65)+char(rand()*26+65)+char(rand()*26+65)+convert(varchar, 1 + CONVERT(INT, (250-1+1)*RAND()))
	Update Users SET UserPassword = pwdencrypt(@NewPassword) WHERE UserID=@UserID
	select @UserEmail = EmailAddress from Users where UserID=@UserID
End
select @NewPassword as ReturnValue, @UserID as UserID,@UserEmail as EmailID

GO
/****** Object:  StoredProcedure [dbo].[sp_CheckDuplicateInvoiceNumber]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--select * from OrderItems



CREATE PROCEDURE [dbo].[sp_CheckDuplicateInvoiceNumber] 

(

@VendorName varchar(200) = '',

@InvoiceNumber Varchar(200)

)

AS

--SELECT * FROM Vendors WHERE VendorName='SAMEER FABRICATORS'
IF EXISTS (SELECT 1 FROM InVoice WHERE InvoiceNumber=@InvoiceNumber) --AND Vendors=@VendorName)

BEGIN 

	SELECT 1 AS DuplicateInvoiceNumber

END

ELSE

BEGIN

SELECT 0 DuplicateInvoiceNumber

END

GO
/****** Object:  StoredProcedure [dbo].[sp_deleteDebitNotes]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_deleteDebitNotes]
( @DebitNoteId int )
AS

BEGIN
delete from DebitNoteDetails WHERE DebitNoteId = @DebitNoteId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_deleteDepartment]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_deleteDepartment]
( @DepartmentID int )
AS

BEGIN
delete from Departments WHERE DeptID = @DepartmentID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_deleteInvoiceFileByName]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_deleteInvoiceFileByName]
@FileName varchar(50)
AS
BEGIN
delete from InVoiceFiles where FileName = @FileName

END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteUserMappingDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_DeleteUserMappingDetails]
(
@userTypeID int,
@usrTable as UserMappingList READONLY

)
AS
BEGIN

   DELETE FROM UserTypesMapping WHERE ID IN (
  SELECT ID FROM UserTypesMapping WHERE UserTypeID = @userTypeID AND UserID in(Select c.UserID from @usrTable c))
    --SELECT ID FROM UserTypesMapping WHERE UserTypeID = @userTypeID AND c.UserID
    --FROM @usrTable c

END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllInvoiceFiles]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllInvoiceFiles]

AS
BEGIN
select * from InVoiceFiles

END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllInvoiceFilesById]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllInvoiceFilesById]
@InVoiceId int
AS
BEGIN
select * from InVoiceFiles where InvoiceID = @InVoiceId 

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllInvoiceItemsForUser]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAllInvoiceItemsForUser]
 @UserID int
AS
BEGIN
	SELECT I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,i.DateofAccount,I.CurrentStatus,U.UserName
	 FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
	 INNER JOIN Users U ON U.UserID = I.CurrentUserID
	 WHERE I.InvoiceID IN(select DISTINCT InvoiceId FROM InvoiceStatus WHERE UserId=@UserID)
	 OR I.CreatedBy=@UserID  OR I.CurrentUserID=@UserID
	 ORDER BY I.InvoiceID DESC

END

GO
/****** Object:  StoredProcedure [dbo].[sp_getAllUsers]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getAllUsers]
As
select USR.*,UT.UserType FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID=USR.UserTypeID 

GO
/****** Object:  StoredProcedure [dbo].[sp_getAssignedToUserID]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[sp_getAssignedToUserID] 
(
@InvoiceId int ,@UserId int
)
AS
BEGIN

--DECLARE @invoiceid int =9413,@userid int =37
----DECLARE @colOrder int 
----select @colOrder = UT.ColumnOrder from UserTypes UT 
----INNER JOIN InvoiceStatus InS ON InS.UserTypeId=UT.UserTypeID 
----WHERE InS.InvoiceId=@InvoiceId AND InS.UserId=@UserId

----set @colOrder = @colOrder+1
----select UserID from InvoiceStatus where UserTypeId=(select UserTypeID from UserTypes where ColumnOrder=@colOrder) AND invoiceid=@InvoiceId
select InS.UserId from UserTypes UT 
INNER JOIN InvoiceStatus InS ON InS.UserTypeId=UT.UserTypeID 
WHERE InS.InvoiceId=@InvoiceId AND UT.ColumnOrder=0
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getChartDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getChartDetails]
as
BEGIN
	--SELECT DATEPART(MONTH,Dateofpayment) as monthVal ,DATEPART(YEAR,Dateofpayment) as yearVal,
	--Count(1) as PaidInvoices
	--FROM orderItems 
	--WHERE Dateofpayment is not null 
	--GROUP BY DATEPART(MONTH, Dateofpayment),DATEPART(YEAR, Dateofpayment)
	SELECT DATEPART(MONTH,Dateofpayment) as monthVal ,DATEPART(YEAR,Dateofpayment) as yearVal,
	Count(1) as PaidInvoices
	FROM InVoice 
	WHERE Dateofpayment is not null 
	GROUP BY DATEPART(MONTH, Dateofpayment),DATEPART(YEAR, Dateofpayment)

	SELECT DATEPART(MONTH,InvoiceDueDate) as monthVal ,DATEPART(YEAR,InvoiceDueDate) as yearVal,
	Count(1) as DueInvoices
	FROM InVoice 
	WHERE InvoiceDueDate > GETDATE()
	GROUP BY DATEPART(MONTH, InvoiceDueDate),DATEPART(YEAR, InvoiceDueDate)
END


GO
/****** Object:  StoredProcedure [dbo].[sp_getChartDetails_2]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	CREATE PROCEDURE 	[dbo].[sp_getChartDetails_2]
	As
	BEGIN
	declare @mthtable TABLE
	(
	monthVal int,
	yearVal int
	)
--INSERT INTO @mthtable(monthVal,yearVal)(
	--SELECT DATEPART(MONTH,Dateofpayment) as monthVal ,DATEPART(YEAR,Dateofpayment) as yearVal
	--FROM InVoice WHERE Dateofpayment is not null 
	--GROUP BY DATEPART(MONTH, Dateofpayment),DATEPART(YEAR, Dateofpayment)
	--UNION 
	--SELECT DATEPART(MONTH,InvoiceDueDate) as monthVal ,DATEPART(YEAR,InvoiceDueDate) as yearVal
	--FROM InVoice WHERE InvoiceDueDate is not null 
	--GROUP BY DATEPART(MONTH, InvoiceDueDate),DATEPART(YEAR, InvoiceDueDate))
	declare @iMonth int
	declare @sYear varchar(4)
	declare @sMonth varchar(2)
	set @iMonth = 0
	while @iMonth > -12
	begin
	set @sYear = year(DATEADD(month,@iMonth,GETDATE()))
	set @sMonth = right('0'+cast(month(DATEADD(month,@iMonth,GETDATE())) as varchar(2)),2)
	--select @sYear as YY ,@sMonth as MM
	INSERT INTO @mthtable(monthVal,yearVal)Values(@sMonth,@sYear)
	set @iMonth = @iMonth - 1
	end
---------------------------------------------------------------------
declare @table TABLE
	(
	monthVal int,
	yearVal int,
	PaidInvoices int
	)
	INSERT INTO @table(monthVal,yearVal,PaidInvoices)(
	SELECT monthVal,yearVal,
	Count(1) as PaidInvoices
	FROM @mthtable t  LEFT JOIN InVoice ON T.monthVal = DATEPART(MONTH, Dateofpayment) AND T.yearVal=DATEPART(YEAR, Dateofpayment)
	WHERE Dateofpayment is not null 
	GROUP BY monthVal,yearVal)

declare @table2 TABLE
	(
	monthVal int,
	yearVal int,
	DueInvoices int
	)
	INSERT INTO @table2(monthVal,yearVal,DueInvoices)(
	SELECT monthVal,yearVal,
	Count(1) as DueInvoices
	FROM @mthtable t  LEFT JOIN InVoice ON T.monthVal = DATEPART(MONTH, InvoiceDueDate) AND T.yearVal=DATEPART(YEAR, InvoiceDueDate)
	WHERE InvoiceDueDate is not null 
	GROUP BY monthVal,yearVal)

	--select * from @table2

	select MT.monthVal,MT.yearVal,ISNULL(T.PaidInvoices,0) AS PaidInvoices,ISNULL(T2.DueInvoices,0) AS DueInvoices from @mthtable MT
    left JOIN @table T on MT.monthVal=t.monthVal AND t.yearVal=MT.yearVal
	left JOIN @table2 T2 on MT.monthVal=T2.monthVal AND T2.yearVal=MT.yearVal
	ORDER BY MT.yearVal ,MT.monthVal
	END





GO
/****** Object:  StoredProcedure [dbo].[SP_GetCommentsbyInvoiceID]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetCommentsbyInvoiceID] 
	@invoiceid int
AS
BEGIN
	select fUsr.UserName as FromUser,tUsr.UserName as ToUser, isnull(a.Comment,'') as Comment from Comments a LEFT JOIN USERS fUsr ON fUsr.UserID=a.CreatedBy
	LEFT JOIN USERS tUsr ON tUsr.UserID=a.AssignedTo  where a.InVoiceID = @invoiceid
		--select (select Username from users where userid = a.CreatedBy) as FromUser,(select Username from users where userid = a.AssignedTo) as ToUser,
		--(select isnull(Comment,'')  from Comments where CommentID = a.CommentID) as Comment from Comments as a  where a.InVoiceID = 9413

END


GO
/****** Object:  StoredProcedure [dbo].[sp_getCostCenters]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getCostCenters]
( @CostCenterID int = 0)
AS
if (@CostCenterID=0)
BEGIN
select * from CostCenters
END
ELSE
BEGIN
select * from CostCenters WHERE CostCenterID = @CostCenterID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getDashBoardetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getDashBoardetails]
as
BEGIN
SELECT 
	SUM(CASE WHEN (DATEDIFF(DAY ,InvoiceDueDate,Dateofpayment) >= 1  AND Dateofpayment IS NOT NULL) then 1 else 0 end ) as OVERDUE, 
	SUM(CASE WHEN (DATEDIFF(DAY ,InvoiceDueDate,GETDATE()) =0 ) then 1 else 0 end) as DUETODAY,
	SUM(CASE WHEN Dateofpayment IS NULL then 1 else 0 end) as PENDING ,
	SUM(CASE WHEN Dateofpayment IS NOT NULL then 1 else 0 end) as PAID ,
	
	SUM(CASE WHEN (InvoiceDueDate <  CAST(GETDATE() as date) AND Dateofpayment IS NOT NULL) THEN InvoiceAmount ELSE 0 END) OSOVERDUE,
	SUM(CASE WHEN (DATEDIFF(DAY ,InvoiceDueDate,GETDATE()) =0 AND Dateofpayment IS NULL)  THEN InvoiceAmount ELSE 0 END) OSTODAY,
	SUM(CASE WHEN 
		(DATEDIFF(MONTH ,InvoiceDueDate,GETDATE()) =0) AND (DATEDIFF(YEAR ,InvoiceDueDate,GETDATE()) = 0) AND Dateofpayment IS NULL
		THEN InvoiceAmount ELSE 0 END) 
		 OSTHISMONTH,
	SUM (CASE WHEN DATEDIFF(DAY ,InvoiceDueDate,Dateofpayment) >= 0 AND Dateofpayment IS NOT NULL THEN InvoiceAmount ELSE 0 END) TotalOS
FROM InVoice 

END


select SUM(CASE WHEN (CAST(InvoiceDueDate as date) <  CAST(GETDATE() as date) AND Dateofpayment IS NOT NULL) then 1 else 0 end ) as OVERDUE from InVoice

select SUM(CASE WHEN (DateDiff(DAY ,InvoiceDueDate,GETDATE()) =0 ) then 1 else 0 end ) as OVERDUE from InVoice


GO
/****** Object:  StoredProcedure [dbo].[SP_GetDebitData]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[SP_GetDebitData](@invoiceid int)
AS
BEGIN
			SELECT VR.InvoiceId,VR.DebitNoteId,VR.DebitFileName,VR.DebitAmount
		FROM DebitNoteDetails VR
		WHERE VR.DebitNoteId = @invoiceid
END

GO
/****** Object:  StoredProcedure [dbo].[SP_GetDebitDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_GetDebitDetails](@invoiceid int)
AS
BEGIN
			SELECT VR.InvoiceId,VR.DebitNoteId,VR.DebitFileName,VR.DebitAmount
		FROM Invoice V INNER JOIN DebitNoteDetails VR
		ON V.InvoiceID = VR.InvoiceId
		WHERE VR.InvoiceId = @invoiceid
END

GO
/****** Object:  StoredProcedure [dbo].[sp_getDepartments]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getDepartments]
( @DepartmentID int = 0)
AS
if (@DepartmentID = 0)
BEGIN
select * from Departments
END
ELSE
BEGIN
select * from Department WHERE Departments = @DepartmentID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getFileteredUsers]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getFileteredUsers] 
(
@UserTypeID int

)
AS
BEGIN
select DISTINCT U.UserID,U.UserName, U.UserTypeID from Users U
LEFT JOIN UserTypesMapping UT ON UT.UserID=u.UserID
WHERE U.Disabled=0 AND U.UserID not in(select UserID from UserTypesMapping UTM where UserTypeID = @UserTypeID)
END



GO
/****** Object:  StoredProcedure [dbo].[sp_getInvoiceDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[sp_getInvoiceDetails]
( @InvoiceId int =0
)
AS
BEGIN

declare @Id varchar(10)
select @Id= cast(@InvoiceId as varchar(20))
--select ID,Vendor,InvoiceNo,InvoiceDate,PO,PODate,Amount,PAN from orderItems OI WHERE OI.ID=2
declare @columns varchar(max)
declare @convert varchar(max)
select   @columns = stuff (( select distinct'],[' +  UserType
                    from InvoiceStatus InS INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId
                  
                    for xml path('')), 1, 2, '') + ']'
set @convert =
'select * from (select OI.ID,OI.Vendor,InvoiceNo,InvoiceDate,PO,PODate,Amount,PAN, Usr.UserType,InS.InvoiceId,Usr.UserTypeID
FROM orderItems OI INNER JOIN [InvoiceStatus] InS ON InS.InvoiceId=OI.ID
  INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId
 WHERE OI.ID='+@Id+')SalesRpt
    pivot(AVG(UserTypeID) for UserType
    in ('+@columns+')) as pivottable'
execute (@convert)
----(select Usr.UserType,Usr.UserTypeID,InS.InvoiceId from InvoiceStatus InS INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId) 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getInvoiceDetails_2]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getInvoiceDetails_2] 
(
 @InvoiceId int =0
)
AS
BEGIN

declare @Id varchar(10)
select @Id= cast(@InvoiceId as varchar(20))
--select ID,Vendor,InvoiceNo,InvoiceDate,PO,PODate,Amount,PAN from orderItems OI WHERE OI.ID=2
declare @columns varchar(max)
declare @convert varchar(max)
select   @columns = stuff (( select  '],[' +  UserType
                    from   UserTypes Usr  order by ColumnOrder  
                  
                    for xml path('')), 1, 2, '') + ']'
set @convert =
'select * from (select I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,i.DateofAccount
,Usr.UserType,Usr.UserTypeID FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
  INNER JOIN [InvoiceStatus] InS ON InS.InvoiceId=I.InvoiceID
  INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId
where I.InvoiceID ='+@Id+')SalesRpt
    pivot(SUM(UserTypeID) for UserType
    in ('+@columns+')) as pivottable'
	print (@convert)
execute (@convert)
----(select Usr.UserType,Usr.UserTypeID,InS.InvoiceId from InvoiceStatus InS INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId) 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getInvoiceDetails_Model]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getInvoiceDetails_Model] 
(
 @InvoiceId int =0
)
AS
BEGIN

--declare @InvoiceId int =9410

select I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorID,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,I.DateofAccount,I.FilePath,I.GSTIN,I.PlaceOfSupply,I.ReverseCharges,I.EcommerceGSTIN,I.TaxableValue,I.TaxPercent,I.CessAmount
 FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
 where I.InvoiceID = @InvoiceId

 DECLARE @TempTable TABLE 
 (
 UserTypeID int,
 UserTypeName varchar(100), 
 ColumnOrder int,
 UserID int
 )
 --select UserTypeID,UserType as UserTypeName from [UserTypes] WHERE Disabled=0  order by ColumnOrder
 Insert into @TempTable (UserTypeID,UserTypeName,ColumnOrder) (select UserTypeID,UserType,ColumnOrder from [UserTypes] WHERE Disabled=0)

 END

 DECLARE @tmpUserId int = 0
WHILE(1 = 1)
BEGIN
  SELECT @tmpUserId = MIN(UserTypeID)
  FROM InvoiceStatus WHERE UserTypeID > @tmpUserId AND InvoiceId=@InvoiceId
  IF @tmpUserId IS NULL BREAK
   --SELECT @tmpUserId as TTTTT
  update @TempTable set UserID = (select top 1 UserID from InvoiceStatus where UserTypeID = @tmpUserId AND InvoiceId=@InvoiceId) WHERE UserTypeID=@tmpUserId
END

select t.UserTypeID,t.UserTypeName,t.UserID,U.UserName from @TempTable t
left join Users U ON U.UserID= t.UserID
order by ColumnOrder
GO
/****** Object:  StoredProcedure [dbo].[sp_GetInvoiceItems]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetInvoiceItems]
 @CurrentUserID int
AS
BEGIN
	SELECT I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,i.DateofAccount
	 FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
	 WHERE I.CurrentUserID = @CurrentUserID
	 ORDER BY I.InvoiceID DESC
END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetInvoiceNumber]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetInvoiceNumber]
(
@invoice int
)
AS
BEGIN
DECLARE @invoiceId int
select top 1 @invoiceId =  InvoiceID from InVoice order by InvoiceID desc
select @invoiceId + 1
END


GO
/****** Object:  StoredProcedure [dbo].[sp_getPurchaseOrder]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getPurchaseOrder]
AS

BEGIN
select * from PurchaseOrder
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetRatingsByUser]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetRatingsByUser]
(
	@vendorID int,
	@invoiceID int,
	@userID int
)
AS
BEGIN
	SELECT VR.VendorRatingID
			,U.UserID
			,V.VendorName
			,I.InvoiceID
			,I.InvoiceNumber
			,VR.Rating
			,VR.Comment	
			,U.UserName AS [Commented By]
		FROM Vendors V 
		INNER JOIN VendorRating VR
		ON V.VendorID = VR.VendorID
		INNER JOIN InVoice I
		ON VR.InvoiceID = I.InvoiceID
		INNER JOIN Users U
		ON VR.CreatedBy = U.UserID
		WHERE VR.VendorID = @vendorID
		AND I.InvoiceID = @invoiceID
		AND U.UserID = @userID
		ORDER BY VR.InvoiceID DESC
END

GO
/****** Object:  StoredProcedure [dbo].[sp_getSystemConfiguration]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getSystemConfiguration]
AS

BEGIN
select * from SystemConfiguration
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTop5InvoiceItems]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetTop5InvoiceItems]
 @CurrentUserID int
AS
BEGIN
	SELECT top 5 I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,i.DateofAccount
	 FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
	 WHERE I.CurrentUserID = @CurrentUserID
	 ORDER BY I.InvoiceID DESC
END

GO
/****** Object:  StoredProcedure [dbo].[sp_getUserById]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getUserById]
(
@UserId int
)
AS
select USR.*,UT.UserType FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID = USR.UserTypeID WHERE USR.UserID = @UserId
GO
/****** Object:  StoredProcedure [dbo].[sp_getUsers]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getUsers]
As
select USR.UserID,USR.UserName,USR.Disabled,usr.UserTypeID,UT.UserType FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID = USR.UserTypeID AND UT.Disabled = 0 AND Usr.UserID <> 3 AND usr.Disabled=0
GO
/****** Object:  StoredProcedure [dbo].[sp_getUsersForDropdown]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getUsersForDropdown]
As
select UTM.ID,USR.UserID,USR.UserName,UTM.UserTypeID from UserTypesMapping UTM INNER JOIN Users USR ON USR.UserID=UTM.UserID WHERE usr.IsAdmin <> 1

GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserTypesMappingDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[sp_GetUserTypesMappingDetails]
AS
BEGIN

select  DENSE_RANK() OVER(ORDER BY UTM.UserTypeID ASC) AS RowNo, 
UT.UserTypeID,U.UserID ,U.UserName,UT.UserType as UserTypeName
from UserTypes UT 
LEFT JOIN UserTypesMapping UTM ON UT.UserTypeID=UTM.UserTypeID
LEFT JOIN Users U ON U.UserID=UTM.UserID
WHERE UT.Disabled=0

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetVendorRatingByID]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetVendorRatingByID]
@vendorID int
AS
BEGIN
	SELECT V.VendorName
		,I.InvoiceNumber
		,CONVERT(VARCHAR(12), I.InvoiceDate, 106) AS [Date of Invoice]
		,SUM(CASE WHEN u.UserTypeID = 2 THEN VR.Rating ELSE 0 END) AS [End User Rating]
		,MAX(CASE WHEN u.UserTypeID = 2 THEN VR.Comment ELSE '' END) AS [End User Comments]
		,SUM(CASE WHEN u.UserTypeID = 3 THEN VR.Rating ELSE 0 END) AS [Ultimate Approver Rating]
		,MAX(CASE WHEN u.UserTypeID = 3 THEN VR.Comment ELSE '' END) AS [Ultimate Approver Comments]
	FROM VendorRating VR
	INNER JOIN Vendors V
	ON VR.VendorID = V.VendorID
	INNER JOIN InVoice I
	ON VR.InvoiceID = I.InvoiceID
	INNER JOIN Users U
	ON VR.CreatedBy = U.UserID
	WHERE VR.VendorID = @vendorID
	GROUP BY I.InvoiceNumber, I.InvoiceDate, V.VendorName
	--ORDER BY VR.InvoiceID DESC
END


GO
/****** Object:  StoredProcedure [dbo].[sp_GetVendorRatings]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetVendorRatings]
AS
BEGIN
	SELECT VR.VendorID, V.VendorName
	,AVG(VR.Rating) AS [Average Rating]
	,COUNT(DISTINCT(VR.InvoiceID)) AS [No. of Invoices]
	FROM VendorRating VR INNER JOIN Vendors V
	ON V.VendorID = VR.VendorID
	GROUP BY VR.VendorID, V.VendorName
	HAVING AVG(VR.Rating) > 0
END


GO
/****** Object:  StoredProcedure [dbo].[sp_getvendors]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getvendors]
AS

BEGIN
select * from vendors
END
GO
/****** Object:  StoredProcedure [dbo].[sp_insertDebitNoteDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[sp_insertDebitNoteDetails]
(
@InvoiceId int,
@DebitFileName varchar(200),
@DebitAmount decimal
)
AS
BEGIN

INSERT INTO [dbo].[DebitNoteDetails]
           ([InvoiceId]
           ,[DebitFileName]
           ,[DebitAmount])
     VALUES
           (@InvoiceId
           ,@DebitFileName
           ,@DebitAmount)

END


GO
/****** Object:  StoredProcedure [dbo].[sp_InsertFile]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertFile]
(
 @invoiceid int,
 @filename varchar(50),
 @filetype varchar(10)
)
AS
BEGIN


insert into invoicefiles(InvoiceID,[FileName],FileType) values(@invoiceid,@filename,@filetype)


END

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertInvoiceItems]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertInvoiceItems]
(
@Id varchar(50),
@Code varchar(50)= null,
@Name varchar(50) = null,
@Qty varchar(50) = null,
@Rate varchar(50) = null,
@Amount varchar(50) = null,
@IGST varchar(50) = null,
@SGST varchar(50) = null,
@CGST varchar(50) = null
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[InvoiceItems]
           ([ItemId]
           ,[Code]
           ,[Name]
		   ,[Qty]
		   ,[Rate]
		   ,[Amount]
		   ,[IGST]
		   ,[SGST]
		   ,[CGST]
		   )
     VALUES
           (@Id,@Code,@Name,@Qty,@Rate,@Amount,@IGST,@SGST,@CGST)
END

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertIQInvoiceItem]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertIQInvoiceItem]
(
@InvoiceNumber varchar(100) = null,
@PANNumber varchar(100)= null,
@PONumber varchar(100) = null,
@InvoiceDate datetime =null,
@InvoiceReceivedDate  datetime =null,
@InvoiceDueDate  datetime = null,
--@TaxTypeID int = 0,
--@DepartmentID int = 0,
--@CostCenterID int = 0,
--@CostUnitID int = 0,
@InvoiceAmount float = 0,
@VendorID int = 0,
@VendorName varchar(500) = '',
@Dateofpayment  datetime = null,
@DateofAccount  datetime = null,
@CreatedBy int = 1,
@filePath varchar(max),
@GSTIN varchar(50),
@PlaceOfSupply varchar(50),
@ReverseCharges varchar(50),
@EcommerceGSTIN varchar(50),
@TaxPercent varchar(50),
@TaxableValue varchar(50),
@CessAmount varchar(50),
@UserTypesTbl AS TableInvoiceStatus READONLY
)

AS
BEGIN

DECLARE @curUsrID INT
DECLARE @InvoiceID INT
--select top 1 @curUsrID = USR.UserID from UserTypes UT INNER JOIN Users USR ON USR.UserTypeID=UT.UserTypeID
--where UT.ColumnOrder=0 AND UT.Disabled=0
----Vendors
IF(@VendorID = 0)
BEGIN
	INSERT INTO [dbo].[Vendors]
           ([VendorName]
           ,[VendorDescription]
           ,[Disabled]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
     VALUES
           (@VendorName,@VendorName,0,@CreatedBy,GETDATE(),@CreatedBy,GETDATE())

Set @VendorID =SCOPE_IDENTITY()
END
---END
INSERT INTO [dbo].[InVoice]
           ([InvoiceNumber]
           ,[PANNumber]
           ,[PONumber]
           ,[InvoiceDate]
           ,[InvoiceReceivedDate]
           ,[InvoiceDueDate]
           --,[TaxTypeID]
           ,[VendorID]
           --,[DepartmentID]
           --,[CostCenterID]
           --,[CostUnitID]
           --,[EndUserStatus]
           --,[EndUserApprovalStatus]
           ,[InvoiceAmount]
           --,[FinalApprovalStatus]
           --,[InitiatorVerificationStatus]
           --,[WFStatus]
           --,[Initiator]
           ,[Dateofpayment]
           ,[DateofAccount]
          -- ,[Navisionvoucherno]
           ,[EndUserID]
           ,[EndUserApprovalID]
           ,[FinalApprovalID]
           ,[CreatedBy]
           ,[CreatedDate]
         --  ,[ReceiptNo]
           ,[CurrentUserID],
		   [FilePath],
		   [GSTIN],
		   [PlaceOfSupply],
		   [ReverseCharges],
		   [EcommerceGSTIN],
		   [TaxPercent],
		   [TaxableValue],
		   [CessAmount]
		   )
     VALUES
           (@InvoiceNumber,@PANNumber,@PONumber,@InvoiceDate
           ,@InvoiceReceivedDate, @InvoiceDueDate, @VendorID, --@DepartmentID, @CostCenterID, @CostUnitID, 
		   @InvoiceAmount, @Dateofpayment, @DateofAccount, null,null,null,@CreatedBy, GETDATE(), null,@filePath,@GSTIN,
		   @PlaceOfSupply,@ReverseCharges,@EcommerceGSTIN,@TaxPercent,@TaxableValue,@CessAmount)

		   Set @InvoiceID =SCOPE_IDENTITY()
		   Select * into #temptable from @UserTypesTbl
		   INSERT INTO [dbo].[InvoiceStatus] ([InvoiceId],[UserTypeId],[UserId]) select @InvoiceID, * from #temptable

		   select top 1 @curUsrID = InS.UserID from UserTypes UT INNER JOIN InvoiceStatus InS ON InS.UserTypeID=UT.UserTypeID
		   where UT.ColumnOrder=0 AND InS.InvoiceId=@InvoiceID AND UT.Disabled=0 
		   
		   declare @UserName varchar(200)
		   select @UserName= UserName from Users WHERE UserID = @CreatedBy -- @curUsrID
		   declare @cmt varchar(max) = @UserName + ' has Initiated the Process '

INSERT INTO [dbo].[Comments]
           ([InVoiceID],[Comment],[CreatedBy],[CreatedDate],[ModidfiedBy],[ModifiedDate],[AssignedTo])
     VALUES
           (@InvoiceID,@cmt,@CreatedBy,GETDATE(),@CreatedBy,GETDATE(),@curUsrID)

		   UPDATE InVoice SET CurrentUserID=@curUsrID WHERE InvoiceID=@InvoiceID

SELECT @InvoiceID

END

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertPurchaseOrder]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertPurchaseOrder]
(
@PODate datetime,
@PONumber varchar(max),
@POAmount float,
@Createdby int,
@POFileName varchar(max)
)
AS 
BEGIN

INSERT INTO [dbo].[PurchaseOrder]
           ([PODate]
           ,[PONumber]
           ,[POAmount]
           ,[Createdby]
           ,[Createddate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[POFileName])
     VALUES
           (@PODate,@PONumber,@POAmount,@Createdby,GETDATE(),@Createdby,GETDATE(),@POFileName)
END



GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdateUserTypes]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUpdateUserTypes]
(
@Id int = null,
@Description Varchar(200) = null,
@ColOrder int
)
AS
BEGIN
UPDATE [dbo].[UserTypes]
   SET [UserType] = @Description,
       [ColumnOrder] = @ColOrder,
	   [Disabled] = 0
 WHERE UserTypeID = @Id
 IF (@Id <= 0)
 BEGIN
 INSERT INTO [dbo].[UserTypes]
           ([UserType]
           ,[Disabled]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[UserID]
           ,[ColumnOrder])
     VALUES
           (@Description,0,1,GETDATE(),1,GETDATE(),null,@ColOrder)
		   END
END

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUserDetail]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUserDetail]
(
	@UserName Varchar(100),
	@UserPassword  varchar(200),---varbinary(255),
	@UserTypeID  int ,
	@DepartmentID int = null,
	@FirstName varchar(100),
	@MiddleName varchar(100) = null,
	@LastName varchar(100) = null,
	@EmailAddress varchar(200),
	@Disabled bit,
	@UserImage varchar(100) = null,
	@IsFinance bit = null,
	@IsVendor bit = null,
	@UserId int = null
	)
AS
BEGIN


INSERT INTO [dbo].[Users]
           ([UserName]
           ,[UserPassword]
           ,[UserTypeID]
           ,[DepartmentID]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[EmailAddress]
           ,[IsAdmin]
           ,[Disabled]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[UserImage]
           ,[IsFinance]
           ,[IsVendor])
     VALUES
           (@UserName,PWDENCRYPT(@UserPassword),@UserTypeID,@DepartmentID, @FirstName, @MiddleName, @LastName, @EmailAddress, 0, 0,
		   @UserId,GETDATE()  ,@UserId,GETDATE(),@UserImage, @IsFinance, @IsVendor)


END



GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUserMappingDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUserMappingDetails]
(
@userTypeID int,
@usrTable as UserMappingList READONLY

)
AS
BEGIN

    INSERT INTO UserTypesMapping(UserTypeID,UserID)
    SELECT @userTypeID, c.UserID
    FROM @usrTable c

END
GO
/****** Object:  StoredProcedure [dbo].[sp_IsPoNumberExists]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_IsPoNumberExists] 
(
@PONumber varchar(250)

)
AS
BEGIN 
declare @isDuplicate bit
IF EXISTS (SELECT 1 FROM Purchaseorder WHERE PONumber = @PoNumber)
	BEGIN
		SELECT @isDuplicate = 1
	END
	ELSE
	BEGIN
		SELECT @isDuplicate =0 
	END 
	SELECT @isDuplicate AS IsDuplicate
END

GO
/****** Object:  StoredProcedure [dbo].[sp_updateDateofpayment]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_updateDateofpayment]
(
@Dateofpayment datetime,
@InvoiceID int
)
As
BEGIN

UPDATE InVoice set Dateofpayment =@Dateofpayment, CurrentStatus = 'Paid' from InVoice where InvoiceID = @InvoiceID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateDebitNoteDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_UpdateDebitNoteDetails]
( @amount float,
@Id int )
AS

BEGIN
UPDATE DebitNoteDetails SET DebitAmount= @amount WHERE DebitNoteId= @Id
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateInvoiceStatus]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_UpdateInvoiceStatus] -- 'A',null,9388,35,null
(
	@Status varchar(20) = '',
	@Comment varchar(max) = null,
	@InvoiceID int,
	@userID int,
	@ReAssignUserID int = null,
	@Rating int,
	@VendorID int = null,
	@VendorComment varchar(max) = null
	)
AS 

BEGIN

DECLARE @colOrder int 
DECLARE @curUsrID int 
DECLARE @payApprovalUsrID int 
DECLARE @MaxcolOrder int 
DECLARE @UserName varchar(200)
--select @colOrder= UT.ColumnOrder from UserTypes UT INNER JOIN Users USR ON USR.UserTypeID=UT.UserTypeID
--where USR.UserID=@userID AND UT.Disabled=0
	select @UserName= UserName from Users WHERE UserID = @userID
select @colOrder = UT.ColumnOrder from UserTypes UT INNER JOIN InvoiceStatus InS ON InS.UserTypeID=UT.UserTypeID
		   where InS.UserId=@userID AND InS.InvoiceId=@InvoiceID AND UT.Disabled=0 
select @MaxcolOrder = MAX(UT.ColumnOrder) from UserTypes UT INNER JOIN InvoiceStatus InS ON InS.UserTypeID=UT.UserTypeID
		   where InS.InvoiceId=@InvoiceID AND UT.Disabled=0 

---Update Invoice SET CurrentStatus= @Status WHERE InvoiceID=@InvoiceID
Update InvoiceStatus SET InvoiceStatus= @Status WHERE InvoiceID=@InvoiceID AND UserId=@userID
--select @colOrder
IF( @colOrder != @MaxcolOrder)
BEGIN
Update Invoice SET CurrentStatus= 'Pending Approval' WHERE InvoiceID=@InvoiceID
	--IF (@Status ='R')  Update Invoice SET CurrentStatus= 'Rejected' WHERE InvoiceID=@InvoiceID
	--IF (@Status ='H')  Update Invoice SET CurrentStatus= 'On Hold' WHERE InvoiceID=@InvoiceID
	--IF (@Status ='ReAssign')  Update Invoice SET CurrentStatus= 'ReAssigned' WHERE InvoiceID=@InvoiceID


	select @colOrder=
	CASE WHEN @Status ='A' THEN  @colOrder+1
			WHEN @Status ='R' THEN  @colOrder-1 
			WHEN @Status ='H' THEN  @colOrder
			WHEN @Status ='ReAssign' THEN  -1
			ELSE -2
	END

	select TOP 1 @curUsrID = InS.UserID from UserTypes UT INNER JOIN InvoiceStatus InS ON InS.UserTypeID=UT.UserTypeID
	where UT.ColumnOrder=@colOrder AND UT.Disabled=0 AND InS.InvoiceId=@InvoiceID

	IF( @colOrder >=0)
		BEGIN
			Update Invoice SET CurrentUserID = @curUsrID WHERE InvoiceID=@InvoiceID

		END
	ELSE IF( @colOrder = -1)
		BEGIN
			Update Invoice SET CurrentUserID = @ReAssignUserID WHERE InvoiceID=@InvoiceID

		END
END
ELSE
BEGIN
	select Top 1 @payApprovalUsrID = UserID from Users Where IsPaymentApproval = 1 AND Disabled=0
	IF (@Status ='A')  Update Invoice SET CurrentStatus= 'Pending For Payment' WHERE InvoiceID=@InvoiceID
	Update Invoice SET CurrentUserID = @payApprovalUsrID WHERE InvoiceID=@InvoiceID

END

declare @cmt varchar(max) = @UserName + ' has commented: ' +ISNULL(@Comment, '')
INSERT INTO [dbo].[Comments]
           ([InVoiceID]
           ,[Comment]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModidfiedBy]
           ,[ModifiedDate],[AssignedTo])
     VALUES
           (@InvoiceID,@cmt,@userID,GETDATE(),@userID,GETDATE(),@curUsrID)
---------------------------
INSERT INTO [dbo].[VendorRating]
           ([VendorID]
           ,[InvoiceID]
           ,[Rating]
           ,[Comment]
           ,[CreatedBy]
           ,[CreatedDate])
     VALUES
           (@VendorID ,@InvoiceID ,@Rating,@Comment, @userID,GETDATE())
-----------------------------------		   	

declare @invoiceAssignedToUsrId int
select @invoiceAssignedToUsrId = CurrentUserID FROM InVoice WHERE InvoiceID=@InvoiceID
select @invoiceAssignedToUsrId
END



GO
/****** Object:  StoredProcedure [dbo].[sp_updateUserMappings]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_updateUserMappings]
(
@userTypeID int,
@usrTable as UserMappingList READONLY

)
AS
BEGIN
delete from [dbo].[UserTypesMapping] WHERE UserTypeID = @userTypeID

    SET NOCOUNT ON;


    INSERT INTO UserTypesMapping(UserTypeID,UserID)
    SELECT @userTypeID, c.UserID
    FROM @usrTable c
	where c.UserID <>0
 ---   WHERE NOT EXISTS (SELECT 1 FROM UserTypesMapping WHERE UserID = c.UserID);
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateOrderItemDetailsNew]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[UpdateOrderItemDetailsNew]
(
@orderItemId int,
@rowNo int,
@colName varchar(200)=null,
@columnVal varchar(200)=null

)
as

 Begin
 UPDATE OrderItemDetailsNew SET ColumnValue=@columnVal WHERE OrderItemId=@OrderItemId AND RowNo=@rowNo AND ColumnName=@colName
 
End







GO
/****** Object:  StoredProcedure [dbo].[UpdateProductItemDetails]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[UpdateProductItemDetails]
(
	@ID int,
	@Vendor nvarchar(200)=null,
	@InvoiceNo nvarchar(100)=null,
	@InvoiceDate datetime =null,
	@PO nvarchar(100) =null,
	@Amount float =null,
	@PAN  nvarchar(100)=null,
	@ItemTable as ItemDetailValuedType READONLY,
	@BUYERCSTNO  nvarchar(100)=null,
	@BUYERVATTIN  nvarchar(100)=null,
	@COMPANYCSTNO  nvarchar(100)=null,
	@COMPANYVATTIN  nvarchar(100)=null

)
as

 Begin
 
Update OrderItems set Vendor=@Vendor,InvoiceNo=@InvoiceNo,InvoiceDate=@InvoiceDate,
PO=@PO ,Amount=@Amount,PAN=@PAN,BUYERCSTNO=@BUYERCSTNO,BUYERVATTIN=@BUYERVATTIN,
COMPANYCSTNO=@COMPANYCSTNO, COMPANYVATTIN= @COMPANYVATTIN Where ID=@ID

Select * into #temptable from @ItemTable

DECLARE @OrderId int,@ItemName nvarchar(100),@Price float,@Qty int,@Total float 
 
 
 
 update OD set OD.ItemName=t.ItemName ,OD.Price =t.Price,OD.Qty=t.Qty ,OD.Total=t.Total,OD.Tax=t.Tax,OD.Asset=t.Asset  from 
 dbo.OrderItemDetails OD inner join #temptable t on t.ID=OD.ID
 
 
  
End



GO
/****** Object:  StoredProcedure [dbo].[UpdateProductItemDetails1]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[UpdateProductItemDetails1]
(
@ID int,
@Vendor nvarchar(200)=null,
@InvoiceNo nvarchar(100)=null,
@InvoiceDate datetime =null,
@PO nvarchar(100) =null,
@Amount float =null,
@PAN  nvarchar(100)=null,
@ItemTable as ItemDetailValuedType READONLY,
@BUYERCSTNO  nvarchar(100)=null,
@BUYERVATTIN  nvarchar(100)=null,
@COMPANYCSTNO  nvarchar(100)=null,
@COMPANYVATTIN  nvarchar(100)=null

)
as

 Begin
 
Update OrderItems set Vendor=@Vendor,InvoiceNo=@InvoiceNo,InvoiceDate=@InvoiceDate,
PO=@PO ,Amount=@Amount,PAN=@PAN,BUYERCSTNO=@BUYERCSTNO,BUYERVATTIN=@BUYERVATTIN,
COMPANYCSTNO=@COMPANYCSTNO, COMPANYVATTIN= @COMPANYVATTIN Where ID=@ID

Select * into #temptable from @ItemTable

DECLARE @OrderId int,@ItemName nvarchar(100),@Price float,@Qty int,@Total float 
 
 
 
 update OD set OD.ItemName=t.ItemName ,OD.Price =t.Price,OD.Qty=t.Qty ,OD.Total=t.Total,OD.Tax=t.Tax,OD.Asset=t.Asset  from 
 dbo.OrderItemDetails OD inner join #temptable t on t.ID=OD.ID
 
 
  
End







GO
/****** Object:  StoredProcedure [dbo].[UpdateProductItemDetailsNew]    Script Date: 03-10-2017 14:43:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[UpdateProductItemDetailsNew]
(
@ID int,
@Vendor nvarchar(200)=null,
@InvoiceNo nvarchar(100)=null,
@InvoiceDate nvarchar(100)=null,
@PO nvarchar(100) =null,
@PODate nvarchar(100) =null,
@Amount  nvarchar(100) =null,
@PAN  nvarchar(100)=null,
@BUYERCSTNO  nvarchar(100)=null,
@BUYERVATTIN  nvarchar(100)=null,
@COMPANYCSTNO  nvarchar(100)=null,
@COMPANYVATTIN  nvarchar(100)=null

)
as

 Begin
 
Update OrderItems set Vendor=@Vendor,InvoiceNo=@InvoiceNo,InvoiceDate=@InvoiceDate,
PO=@PO ,Amount=@Amount,PAN=@PAN,BUYERCSTNO=@BUYERCSTNO,BUYERVATTIN=@BUYERVATTIN,
COMPANYCSTNO=@COMPANYCSTNO, COMPANYVATTIN= @COMPANYVATTIN Where ID=@ID
 
End

select * from OrderItems






GO
USE [master]
GO
ALTER DATABASE [IQInvoiceItemNew] SET  READ_WRITE 
GO
