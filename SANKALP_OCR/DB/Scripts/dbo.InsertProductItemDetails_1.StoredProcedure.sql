USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[InsertProductItemDetails_1]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE procedure [dbo].[InsertProductItemDetails_1] 
(

@Vendor nvarchar(200)=null,
@InvoiceNo nvarchar(100)=null,
@InvoiceDate datetime =null,
@PO nvarchar(100) =null,
@PODate datetime =null,
@Amount nvarchar(100)=null,
@PAN  nvarchar(100)=null,
@ItemTable as TableValuedType READONLY,
@BUYERCSTNO  nvarchar(100)=null,
@BUYERVATTIN  nvarchar(100)=null,
@COMPANYCSTNO  nvarchar(100)=null,
@COMPANYVATTIN  nvarchar(100)=null,
@ImageFilePath nvarchar(2000) =null

)
as
--select  [dbo].[fn_GetCodeIndex] ('af', '10')
Declare @OrderItemID int

Insert into OrderItems( Vendor,InvoiceNo, InvoiceDate, PO,PODate,Amount,PAN,BUYERCSTNO,BUYERVATTIN,COMPANYCSTNO,COMPANYVATTIN,ImageFilePath) values
(@Vendor,@InvoiceNo,@InvoiceDate,@PO,@PODate,@Amount,@PAN,@BUYERCSTNO,@BUYERVATTIN,@COMPANYCSTNO,@COMPANYVATTIN,@ImageFilePath)

Set @OrderItemID =SCOPE_IDENTITY()

Select * into #temptable from @ItemTable
select * from #temptable

update #temptable set LastCodeIndex = [dbo].[fn_GetCodeIndex] (code,Qty) 
insert into [dbo].[OrderItemDetails]([OrderItemID],[ItemName],[Price],[Qty],[Tax],[Total],[Asset],[Code],[LastCodeIndex]) select @OrderItemID, * from #temptable








GO
