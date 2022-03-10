USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[GetOrderDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[GetOrderDetails]
as
select ID as OrderID, Vendor, InvoiceNo, InvoiceDate, PO, Amount, PAN, BUYERCSTNO, BUYERVATTIN, COMPANYCSTNO, COMPANYVATTIN from dbo.OrderItems

Select  OrderItemID as OrderID, ItemName, Price, Qty, Total from dbo.OrderItemDetails

GO
