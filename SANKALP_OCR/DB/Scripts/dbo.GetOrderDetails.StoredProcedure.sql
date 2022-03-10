USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[GetOrderDetails]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[GetOrderDetails]
as
select ID as OrderID, Vendor, InvoiceNo, InvoiceDate, PO, Amount, PAN, BUYERCSTNO, BUYERVATTIN, COMPANYCSTNO, COMPANYVATTIN from dbo.OrderItems

Select  OrderItemID as OrderID, ItemName, Price, Qty, Total from dbo.OrderItemDetails

GO
