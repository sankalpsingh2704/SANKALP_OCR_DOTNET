USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[CheckInvoiceAmount]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[CheckInvoiceAmount]    Script Date: 24-04-2017 16:10:30 ******/
CREATE procedure [dbo].[CheckInvoiceAmount] 
(
	@PONumber varchar(max),
	@poamount decimal = 0
)
as

Declare @Total decimal,@POTotal decimal

select @Total=Sum(InvoiceAmount) from invoice where PONumber=@PONumber

set @Total=@Total +isnull(@poamount,0)
select @POTotal=POAmount from PurchaseOrder where  PONumber=@PONumber
if isnull(@POTotal,0)= 0
begin
	select 3
end
else if isnull(@Total,0) > isnull(@POTotal,0)
begin
	select 0
end
else
begin
	select 1
end




GO
