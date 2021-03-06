USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[GetAllOrderwithItemDetails1]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[GetAllOrderwithItemDetails1]
as
select 
o.ID, o.Vendor, o.InvoiceNo, o.InvoiceDate, o.PO, o.Amount, o.PAN, o.BUYERCSTNO,
o.BUYERVATTIN, o.COMPANYCSTNO, o.COMPANYVATTIN,
oid.ItemName, oid.Price, oid.Qty,oid.Tax, oid.Total, oid.asset
from OrderItems o
inner join Orderitemdetails oid
on o.ID = oid.orderitemid

GO
