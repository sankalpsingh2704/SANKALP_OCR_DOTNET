USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[InsertInvoiceItemDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[InsertInvoiceItemDetails] 
(

@InvoiceID varchar(100) = null,  
@InvoiceNumber varchar(100) = null,
@InvoiceAmount varchar(100) = null,
@PONumber varchar(100) = null,
@PANNumber varchar(100) = null,
@VendorName varchar(100) = null, 
@InvoiceDate datetime = null,
@InvoiceDueDate datetime = null,
@InvoiceReceiveddate datetime = null,
@Dateofpayment datetime = null,
@DateofAccount datetime = null,
@ItemTable as TableInvoiceStatus READONLY
)
as
--select  [dbo].[fn_GetCodeIndex] ('af', '10')
Declare @ID int
INSERT INTO [dbo].[InVoice]([InvoiceNumber],[PANNumber],[PONumber],[InvoiceDate],[InvoiceDueDate]      ,[VendorID]
           ,[Dateofpayment]
           ,[DateofAccount]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[InvoiceReceivedDate])
     VALUES(@InvoiceNumber,@PANNumber,@PONumber,@InvoiceDate,@InvoiceDueDate, 111,@Dateofpayment,@DateofAccount,1,getdate(),@InvoiceReceiveddate)
select  @ID =SCOPE_IDENTITY()

Select * into #temptable from @ItemTable
--select * from #temptable

--update #temptable set LastCodeIndex = [dbo].[fn_GetCodeIndex] (code,Qty) 
insert into [dbo].[InvoiceStatus] ([InvoiceId],[UserId],[UserTypeId]) select @ID, UserId,UserTypeId from #temptable









GO
