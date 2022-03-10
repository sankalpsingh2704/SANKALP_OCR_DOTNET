USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[UpdateProductItemDetailsNew]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[UpdateProductItemDetailsNew]
(
@ID int,
@Vendor nvarchar(200)=null,
@InvoiceNo nvarchar(100)=null,
@InvoiceDate nvarchar(100)=null,
@PO nvarchar(100) =null,
@PODate nvarchar(100) =null,
@Amount  nvarchar(100) =null,
@PAN  nvarchar(100)=null,
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
 
End

select * from OrderItems






GO
