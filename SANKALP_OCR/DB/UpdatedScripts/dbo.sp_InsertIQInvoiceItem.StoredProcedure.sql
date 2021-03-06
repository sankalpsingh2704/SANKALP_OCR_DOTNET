USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertIQInvoiceItem]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertIQInvoiceItem]
(
@InvoiceNumber varchar(100) = null,
@PANNumber varchar(100)= null,
@PONumber varchar(100) = null,
@InvoiceDate datetime =null,
@InvoiceReceivedDate  datetime =null,
@InvoiceDueDate  datetime = null,
--@TaxTypeID int = 0,
--@DepartmentID int = 0,
--@CostCenterID int = 0,
--@CostUnitID int = 0,
@InvoiceAmount float = 0,
@VendorID int = 0,
@VendorName varchar(500) = '',
@Dateofpayment  datetime = null,
@DateofAccount  datetime = null,
@CreatedBy int = 1,
@filePath varchar(max),
@GSTIN varchar(50),
@PlaceOfSupply varchar(50),
@ReverseCharges varchar(50),
@EcommerceGSTIN varchar(50),
@TaxPercent varchar(50),
@TaxableValue varchar(50),
@CessAmount varchar(50),
@UserTypesTbl AS TableInvoiceStatus READONLY
)

AS
BEGIN

DECLARE @curUsrID INT
DECLARE @InvoiceID INT
--select top 1 @curUsrID = USR.UserID from UserTypes UT INNER JOIN Users USR ON USR.UserTypeID=UT.UserTypeID
--where UT.ColumnOrder=0 AND UT.Disabled=0
----Vendors
IF(@VendorID = 0)
BEGIN
	INSERT INTO [dbo].[Vendors]
           ([VendorName]
           ,[VendorDescription]
           ,[Disabled]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
     VALUES
           (@VendorName,@VendorName,0,@CreatedBy,GETDATE(),@CreatedBy,GETDATE())

Set @VendorID =SCOPE_IDENTITY()
END
---END
INSERT INTO [dbo].[InVoice]
           ([InvoiceNumber]
           ,[PANNumber]
           ,[PONumber]
           ,[InvoiceDate]
           ,[InvoiceReceivedDate]
           ,[InvoiceDueDate]
           --,[TaxTypeID]
           ,[VendorID]
           --,[DepartmentID]
           --,[CostCenterID]
           --,[CostUnitID]
           --,[EndUserStatus]
           --,[EndUserApprovalStatus]
           ,[InvoiceAmount]
           --,[FinalApprovalStatus]
           --,[InitiatorVerificationStatus]
           --,[WFStatus]
           --,[Initiator]
           ,[Dateofpayment]
           ,[DateofAccount]
          -- ,[Navisionvoucherno]
           ,[EndUserID]
           ,[EndUserApprovalID]
           ,[FinalApprovalID]
           ,[CreatedBy]
           ,[CreatedDate]
         --  ,[ReceiptNo]
           ,[CurrentUserID],
		   [FilePath],
		   [GSTIN],
		   [PlaceOfSupply],
		   [ReverseCharges],
		   [EcommerceGSTIN],
		   [TaxPercent],
		   [TaxableValue],
		   [CessAmount]
		   )
     VALUES
           (@InvoiceNumber,@PANNumber,@PONumber,@InvoiceDate
           ,@InvoiceReceivedDate, @InvoiceDueDate, @VendorID, --@DepartmentID, @CostCenterID, @CostUnitID, 
		   @InvoiceAmount, @Dateofpayment, @DateofAccount, null,null,null,@CreatedBy, GETDATE(), null,@filePath,@GSTIN,
		   @PlaceOfSupply,@ReverseCharges,@EcommerceGSTIN,@TaxPercent,@TaxableValue,@CessAmount)

		   Set @InvoiceID =SCOPE_IDENTITY()
		   Select * into #temptable from @UserTypesTbl
		   INSERT INTO [dbo].[InvoiceStatus] ([InvoiceId],[UserTypeId],[UserId]) select @InvoiceID, * from #temptable

		   select top 1 @curUsrID = InS.UserID from UserTypes UT INNER JOIN InvoiceStatus InS ON InS.UserTypeID=UT.UserTypeID
		   where UT.ColumnOrder=0 AND InS.InvoiceId=@InvoiceID AND UT.Disabled=0 
		   
		   declare @UserName varchar(200)
		   select @UserName= UserName from Users WHERE UserID = @CreatedBy -- @curUsrID
		   declare @cmt varchar(max) = @UserName + ' has Initiated the Process '

INSERT INTO [dbo].[Comments]
           ([InVoiceID],[Comment],[CreatedBy],[CreatedDate],[ModidfiedBy],[ModifiedDate],[AssignedTo])
     VALUES
           (@InvoiceID,@cmt,@CreatedBy,GETDATE(),@CreatedBy,GETDATE(),@curUsrID)

		   UPDATE InVoice SET CurrentUserID=@curUsrID WHERE InvoiceID=@InvoiceID

SELECT @InvoiceID

END

GO
