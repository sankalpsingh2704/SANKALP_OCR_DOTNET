USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[UpdateProductItemDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[UpdateProductItemDetails]
(
	@ID int,
	@Vendor nvarchar(200)=null,
	@InvoiceNo nvarchar(100)=null,
	@InvoiceDate datetime =null,
	@PO nvarchar(100) =null,
	@Amount float =null,
	@PAN  nvarchar(100)=null,
	@ItemTable as ItemDetailValuedType READONLY,
	@BUYERCSTNO  nvarchar(100)=null,
	@BUYERVATTIN  nvarchar(100)=null,
	@COMPANYCSTNO  nvarchar(100)=null,
	@COMPANYVATTIN  nvarchar(100)=null

)
as

 Begin
 
Update OrderItems set Vendor=@Vendor,InvoiceNo=@InvoiceNo,InvoiceDate=@InvoiceDate,
PO=@PO ,Amount=@Amount,PAN=@PAN,BUYERCSTNO=@BUYERCSTNO,BUYERVATTIN=@BUYERVATTIN,
COMPANYCSTNO=@COMPANYCSTNO, COMPANYVATTIN= @COMPANYVATTIN Where ID=@ID

Select * into #temptable from @ItemTable

DECLARE @OrderId int,@ItemName nvarchar(100),@Price float,@Qty int,@Total float 
 
 
 
 update OD set OD.ItemName=t.ItemName ,OD.Price =t.Price,OD.Qty=t.Qty ,OD.Total=t.Total,OD.Tax=t.Tax,OD.Asset=t.Asset  from 
 dbo.OrderItemDetails OD inner join #temptable t on t.ID=OD.ID
 
 
  
End



GO
