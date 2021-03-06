USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[InvoiceItems]    Script Date: 03-10-2017 14:44:40 ******/
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
