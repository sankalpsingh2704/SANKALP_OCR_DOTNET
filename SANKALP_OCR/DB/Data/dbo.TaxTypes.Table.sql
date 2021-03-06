USE [IQInvoiceItem]
GO
SET IDENTITY_INSERT [dbo].[TaxTypes] ON 

INSERT [dbo].[TaxTypes] ([TaxTypeID], [TaxTypeName], [TaxTypeDescription], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Tax Type Name', N'Tax Type Description', 0, NULL, NULL, NULL, NULL)
INSERT [dbo].[TaxTypes] ([TaxTypeID], [TaxTypeName], [TaxTypeDescription], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'Service Tax', N'Service Tax', 0, NULL, NULL, NULL, NULL)
INSERT [dbo].[TaxTypes] ([TaxTypeID], [TaxTypeName], [TaxTypeDescription], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'TDS ', N'Tax deduction at source', 0, NULL, NULL, NULL, NULL)
INSERT [dbo].[TaxTypes] ([TaxTypeID], [TaxTypeName], [TaxTypeDescription], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'CST', N'Central Sale tax', 0, NULL, NULL, NULL, NULL)
INSERT [dbo].[TaxTypes] ([TaxTypeID], [TaxTypeName], [TaxTypeDescription], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'Execise duty', N'Execise duty', 0, NULL, NULL, NULL, NULL)
INSERT [dbo].[TaxTypes] ([TaxTypeID], [TaxTypeName], [TaxTypeDescription], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'VAT', N'Vat added tax', 0, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[TaxTypes] OFF
