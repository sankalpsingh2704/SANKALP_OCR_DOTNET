USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 17-05-2017 19:07:15 ******/
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
