USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getInvoiceDetails_2]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getInvoiceDetails_2] 
(
 @InvoiceId int =0
)
AS
BEGIN

declare @Id varchar(10)
select @Id= cast(@InvoiceId as varchar(20))
--select ID,Vendor,InvoiceNo,InvoiceDate,PO,PODate,Amount,PAN from orderItems OI WHERE OI.ID=2
declare @columns varchar(max)
declare @convert varchar(max)
select   @columns = stuff (( select  '],[' +  UserType
                    from   UserTypes Usr  order by ColumnOrder  
                  
                    for xml path('')), 1, 2, '') + ']'
set @convert =
'select * from (select I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,i.DateofAccount
,Usr.UserType,Usr.UserTypeID FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
  INNER JOIN [InvoiceStatus] InS ON InS.InvoiceId=I.InvoiceID
  INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId
where I.InvoiceID ='+@Id+')SalesRpt
    pivot(SUM(UserTypeID) for UserType
    in ('+@columns+')) as pivottable'
	print (@convert)
execute (@convert)
----(select Usr.UserType,Usr.UserTypeID,InS.InvoiceId from InvoiceStatus InS INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId) 
END
GO
