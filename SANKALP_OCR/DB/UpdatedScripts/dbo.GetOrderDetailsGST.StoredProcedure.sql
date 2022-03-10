USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[GetOrderDetailsGST]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[GetOrderDetailsGST]
as
Select GSTIN as [GSTIN/UIN of Recipient], InvoiceNumber as [Invoice Number], InvoiceDate as [Invoice date], InvoiceAmount as  [Invoice Value],PlaceOfSupply as [Place Of Supply],CASE ReverseCharges WHEN 0 Then 'N' ELSE 'Y' END as [Reverse Charge] ,'Regular' as [Invoice Type], EcommerceGSTIN as [E-Commerce GSTIN],TaxPercent as Rate,TaxableValue as [Taxable Value],CessAmount as [Cess Amount] from InVoice where invoiceid>10430

--Select  OrderItemID as OrderID, ItemName, Price, Qty, Total from dbo.OrderItemDetails

										



--select * from invoice where invoiceid=10431





GO
