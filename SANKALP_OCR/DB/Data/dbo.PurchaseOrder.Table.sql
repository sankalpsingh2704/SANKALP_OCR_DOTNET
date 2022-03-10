USE [IQInvoiceItem]
GO
SET IDENTITY_INSERT [dbo].[PurchaseOrder] ON 

INSERT [dbo].[PurchaseOrder] ([POID], [PODate], [PONumber], [POAmount], [Createdby], [Createddate], [ModifiedBy], [ModifiedDate], [POFileName]) VALUES (1, CAST(0x0000A78A00000000 AS DateTime), N'1234', CAST(9999 AS Decimal(18, 0)), 32, CAST(0x0000A74E00CE382E AS DateTime), 32, CAST(0x0000A74E00CE382E AS DateTime), N'205.pdf')
INSERT [dbo].[PurchaseOrder] ([POID], [PODate], [PONumber], [POAmount], [Createdby], [Createddate], [ModifiedBy], [ModifiedDate], [POFileName]) VALUES (3, CAST(0x0000A83F00000000 AS DateTime), N'111', CAST(123 AS Decimal(18, 0)), 42, CAST(0x0000A75400E8FB34 AS DateTime), 42, CAST(0x0000A75400E8FB34 AS DateTime), N'id786 (1).pdf')
INSERT [dbo].[PurchaseOrder] ([POID], [PODate], [PONumber], [POAmount], [Createdby], [Createddate], [ModifiedBy], [ModifiedDate], [POFileName]) VALUES (4, CAST(0x0000A76900000000 AS DateTime), N'0000003109', CAST(12122 AS Decimal(18, 0)), 38, CAST(0x0000A75400EB5CBC AS DateTime), 38, CAST(0x0000A75400EB5CBC AS DateTime), N'ibad20.pdf')
INSERT [dbo].[PurchaseOrder] ([POID], [PODate], [PONumber], [POAmount], [Createdby], [Createddate], [ModifiedBy], [ModifiedDate], [POFileName]) VALUES (5, CAST(0x0000A74B00000000 AS DateTime), N'565', CAST(1111 AS Decimal(18, 0)), 38, CAST(0x0000A75400EBCED3 AS DateTime), 38, CAST(0x0000A75400EBCED3 AS DateTime), N'ibad20.pdf')
INSERT [dbo].[PurchaseOrder] ([POID], [PODate], [PONumber], [POAmount], [Createdby], [Createddate], [ModifiedBy], [ModifiedDate], [POFileName]) VALUES (7, CAST(0x0000A83F00000000 AS DateTime), N'0000003109', CAST(323233 AS Decimal(18, 0)), 38, CAST(0x0000A75400F258EE AS DateTime), 38, CAST(0x0000A75400F258EE AS DateTime), N'1292.pdf')
SET IDENTITY_INSERT [dbo].[PurchaseOrder] OFF
