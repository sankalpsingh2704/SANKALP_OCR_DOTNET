USE [IQInvoiceItem]
GO
/****** Object:  UserDefinedTableType [dbo].[TableValuedType]    Script Date: 17-05-2017 19:07:14 ******/
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
