USE [IQInvoiceItem]
GO
/****** Object:  UserDefinedTableType [dbo].[TableInvoiceStatus]    Script Date: 17-05-2017 19:07:14 ******/
CREATE TYPE [dbo].[TableInvoiceStatus] AS TABLE(
	[UserID] [int] NULL,
	[UserTypeID] [int] NULL
)
GO
