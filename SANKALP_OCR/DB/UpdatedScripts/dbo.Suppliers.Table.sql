USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 03-10-2017 14:44:40 ******/
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
