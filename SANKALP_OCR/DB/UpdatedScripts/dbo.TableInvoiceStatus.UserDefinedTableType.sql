USE [IQInvoiceItemNew]
GO
/****** Object:  UserDefinedTableType [dbo].[TableInvoiceStatus]    Script Date: 03-10-2017 14:44:40 ******/
CREATE TYPE [dbo].[TableInvoiceStatus] AS TABLE(
	[UserID] [int] NULL,
	[UserTypeID] [int] NULL
)
GO
