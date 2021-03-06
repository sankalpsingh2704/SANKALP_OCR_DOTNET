USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[GetAllOrderwithItemDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetAllOrderwithItemDetails]
as
select 
o.ID as [Maintenance Part ID],
'' as [Maintenance Object (ID)],
'' as [Parent Part (Reference)],
oid.ItemName as [Part Type (Reference)],
'' as [Code],
oid.ItemName as [Reference],
'Existing' as [Status (Code)],
'' as [Nature (Code)],	
'' as [Power],
'' as [Warranty],	
'' as [Warranty Date],
'Location' as [Site (Reference)],
'' as [Building (Reference)],
'' as [Floor (Reference)],
'' as [Room (Reference)],
'' as [Land (Reference)],
'' as [Outside Location (Reference)],
'' as [Symbol (Reference)],
'' as [Investment Date],
'' as [Construction Date],
'' as [Expected Lifetime (Years)],
'' as [Overridden Replacement Year],
'' as [Expected Replacement Amount],
'' as [Expected Replacement Currency],
'' as [Yearly Cost],
'' as [Yearly Cost Currency],
o.PO as [udi.U_PO_NUMBER],
o.InvoiceDate as [udi.U_PO_DATE],
o.InvoiceNo as [udi.U_INVOICE_NUMBER],
o.InvoiceDate as [udi.U_INVOICE_DATE],
oid.Total as [udi.U_PO_AMOUNT],
oid.Total as [udi.U_INVOICE_AMOUNT]


--o.Vendor, o.InvoiceNo, o.InvoiceDate, o.PO, o.Amount, o.PAN, o.BUYERCSTNO,
--o.BUYERVATTIN, o.COMPANYCSTNO, o.COMPANYVATTIN,
--oid.ItemName, oid.Price, oid.Qty,oid.Tax, oid.Total, oid.asset
from OrderItems o
inner join Orderitemdetails oid
on o.ID = oid.orderitemid

GO
