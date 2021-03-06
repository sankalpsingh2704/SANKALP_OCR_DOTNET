USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[SystemConfiguration]    Script Date: 17-05-2017 19:07:15 ******/
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
