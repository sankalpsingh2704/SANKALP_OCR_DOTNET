USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 03-10-2017 14:44:40 ******/
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
