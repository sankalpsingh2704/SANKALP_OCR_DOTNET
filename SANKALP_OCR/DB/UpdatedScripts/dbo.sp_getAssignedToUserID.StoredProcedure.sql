USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getAssignedToUserID]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[sp_getAssignedToUserID] 
(
@InvoiceId int ,@UserId int
)
AS
BEGIN

--DECLARE @invoiceid int =9413,@userid int =37
----DECLARE @colOrder int 
----select @colOrder = UT.ColumnOrder from UserTypes UT 
----INNER JOIN InvoiceStatus InS ON InS.UserTypeId=UT.UserTypeID 
----WHERE InS.InvoiceId=@InvoiceId AND InS.UserId=@UserId

----set @colOrder = @colOrder+1
----select UserID from InvoiceStatus where UserTypeId=(select UserTypeID from UserTypes where ColumnOrder=@colOrder) AND invoiceid=@InvoiceId
select InS.UserId from UserTypes UT 
INNER JOIN InvoiceStatus InS ON InS.UserTypeId=UT.UserTypeID 
WHERE InS.InvoiceId=@InvoiceId AND UT.ColumnOrder=0
END
GO
