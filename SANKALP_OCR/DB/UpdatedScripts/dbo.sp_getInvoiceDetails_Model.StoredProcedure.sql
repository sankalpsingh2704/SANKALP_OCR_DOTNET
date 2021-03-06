USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getInvoiceDetails_Model]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getInvoiceDetails_Model] 
(
 @InvoiceId int =0
)
AS
BEGIN

--declare @InvoiceId int =9410

select I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorID,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,I.DateofAccount,I.FilePath,I.GSTIN,I.PlaceOfSupply,I.ReverseCharges,I.EcommerceGSTIN,I.TaxableValue,I.TaxPercent,I.CessAmount
 FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
 where I.InvoiceID = @InvoiceId

 DECLARE @TempTable TABLE 
 (
 UserTypeID int,
 UserTypeName varchar(100), 
 ColumnOrder int,
 UserID int
 )
 --select UserTypeID,UserType as UserTypeName from [UserTypes] WHERE Disabled=0  order by ColumnOrder
 Insert into @TempTable (UserTypeID,UserTypeName,ColumnOrder) (select UserTypeID,UserType,ColumnOrder from [UserTypes] WHERE Disabled=0)

 END

 DECLARE @tmpUserId int = 0
WHILE(1 = 1)
BEGIN
  SELECT @tmpUserId = MIN(UserTypeID)
  FROM InvoiceStatus WHERE UserTypeID > @tmpUserId AND InvoiceId=@InvoiceId
  IF @tmpUserId IS NULL BREAK
   --SELECT @tmpUserId as TTTTT
  update @TempTable set UserID = (select top 1 UserID from InvoiceStatus where UserTypeID = @tmpUserId AND InvoiceId=@InvoiceId) WHERE UserTypeID=@tmpUserId
END

select t.UserTypeID,t.UserTypeName,t.UserID,U.UserName from @TempTable t
left join Users U ON U.UserID= t.UserID
order by ColumnOrder
GO
