USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateInvoiceStatus]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_UpdateInvoiceStatus] -- 'A',null,9388,35,null
(
	@Status varchar(20) = '',
	@Comment varchar(max) = null,
	@InvoiceID int,
	@userID int,
	@ReAssignUserID int = null,
	@Rating int,
	@VendorID int = null,
	@VendorComment varchar(max) = null
	)
AS 

BEGIN

DECLARE @colOrder int 
DECLARE @curUsrID int 
DECLARE @payApprovalUsrID int 
DECLARE @MaxcolOrder int 
DECLARE @UserName varchar(200)
--select @colOrder= UT.ColumnOrder from UserTypes UT INNER JOIN Users USR ON USR.UserTypeID=UT.UserTypeID
--where USR.UserID=@userID AND UT.Disabled=0
	select @UserName= UserName from Users WHERE UserID = @userID
select @colOrder = UT.ColumnOrder from UserTypes UT INNER JOIN InvoiceStatus InS ON InS.UserTypeID=UT.UserTypeID
		   where InS.UserId=@userID AND InS.InvoiceId=@InvoiceID AND UT.Disabled=0 
select @MaxcolOrder = MAX(UT.ColumnOrder) from UserTypes UT INNER JOIN InvoiceStatus InS ON InS.UserTypeID=UT.UserTypeID
		   where InS.InvoiceId=@InvoiceID AND UT.Disabled=0 

---Update Invoice SET CurrentStatus= @Status WHERE InvoiceID=@InvoiceID
Update InvoiceStatus SET InvoiceStatus= @Status WHERE InvoiceID=@InvoiceID AND UserId=@userID
--select @colOrder
IF( @colOrder != @MaxcolOrder)
BEGIN
Update Invoice SET CurrentStatus= 'Pending Approval' WHERE InvoiceID=@InvoiceID
	--IF (@Status ='R')  Update Invoice SET CurrentStatus= 'Rejected' WHERE InvoiceID=@InvoiceID
	--IF (@Status ='H')  Update Invoice SET CurrentStatus= 'On Hold' WHERE InvoiceID=@InvoiceID
	--IF (@Status ='ReAssign')  Update Invoice SET CurrentStatus= 'ReAssigned' WHERE InvoiceID=@InvoiceID


	select @colOrder=
	CASE WHEN @Status ='A' THEN  @colOrder+1
			WHEN @Status ='R' THEN  @colOrder-1 
			WHEN @Status ='H' THEN  @colOrder
			WHEN @Status ='ReAssign' THEN  -1
			ELSE -2
	END

	select TOP 1 @curUsrID = InS.UserID from UserTypes UT INNER JOIN InvoiceStatus InS ON InS.UserTypeID=UT.UserTypeID
	where UT.ColumnOrder=@colOrder AND UT.Disabled=0 AND InS.InvoiceId=@InvoiceID

	IF( @colOrder >=0)
		BEGIN
			Update Invoice SET CurrentUserID = @curUsrID WHERE InvoiceID=@InvoiceID

		END
	ELSE IF( @colOrder = -1)
		BEGIN
			Update Invoice SET CurrentUserID = @ReAssignUserID WHERE InvoiceID=@InvoiceID

		END
END
ELSE
BEGIN
	select Top 1 @payApprovalUsrID = UserID from Users Where IsPaymentApproval = 1 AND Disabled=0
	IF (@Status ='A')  Update Invoice SET CurrentStatus= 'Pending For Payment' WHERE InvoiceID=@InvoiceID
	Update Invoice SET CurrentUserID = @payApprovalUsrID WHERE InvoiceID=@InvoiceID

END

declare @cmt varchar(max) = @UserName + ' has commented: ' +ISNULL(@Comment, '')
INSERT INTO [dbo].[Comments]
           ([InVoiceID]
           ,[Comment]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModidfiedBy]
           ,[ModifiedDate],[AssignedTo])
     VALUES
           (@InvoiceID,@cmt,@userID,GETDATE(),@userID,GETDATE(),@curUsrID)
---------------------------
INSERT INTO [dbo].[VendorRating]
           ([VendorID]
           ,[InvoiceID]
           ,[Rating]
           ,[Comment]
           ,[CreatedBy]
           ,[CreatedDate])
     VALUES
           (@VendorID ,@InvoiceID ,@Rating,@Comment, @userID,GETDATE())
-----------------------------------		   	

declare @invoiceAssignedToUsrId int
select @invoiceAssignedToUsrId = CurrentUserID FROM InVoice WHERE InvoiceID=@InvoiceID
select @invoiceAssignedToUsrId
END



GO
