USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getInvoiceDetails]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[sp_getInvoiceDetails]
( @InvoiceId int =0
)
AS
BEGIN

declare @Id varchar(10)
select @Id= cast(@InvoiceId as varchar(20))
--select ID,Vendor,InvoiceNo,InvoiceDate,PO,PODate,Amount,PAN from orderItems OI WHERE OI.ID=2
declare @columns varchar(max)
declare @convert varchar(max)
select   @columns = stuff (( select distinct'],[' +  UserType
                    from InvoiceStatus InS INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId
                  
                    for xml path('')), 1, 2, '') + ']'
set @convert =
'select * from (select OI.ID,OI.Vendor,InvoiceNo,InvoiceDate,PO,PODate,Amount,PAN, Usr.UserType,InS.InvoiceId,Usr.UserTypeID
FROM orderItems OI INNER JOIN [InvoiceStatus] InS ON InS.InvoiceId=OI.ID
  INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId
 WHERE OI.ID='+@Id+')SalesRpt
    pivot(AVG(UserTypeID) for UserType
    in ('+@columns+')) as pivottable'
execute (@convert)
----(select Usr.UserType,Usr.UserTypeID,InS.InvoiceId from InvoiceStatus InS INNER JOIN UserTypes Usr ON Usr.UserTypeID=InS.UserTypeId) 
END
GO
