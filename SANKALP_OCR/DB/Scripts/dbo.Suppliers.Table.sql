USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 17-05-2017 19:07:15 ******/
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
