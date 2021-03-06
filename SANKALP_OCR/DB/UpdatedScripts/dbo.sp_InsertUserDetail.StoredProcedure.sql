USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUserDetail]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUserDetail]
(
	@UserName Varchar(100),
	@UserPassword  varchar(200),---varbinary(255),
	@UserTypeID  int ,
	@DepartmentID int = null,
	@FirstName varchar(100),
	@MiddleName varchar(100) = null,
	@LastName varchar(100) = null,
	@EmailAddress varchar(200),
	@Disabled bit,
	@UserImage varchar(100) = null,
	@IsFinance bit = null,
	@IsVendor bit = null,
	@UserId int = null
	)
AS
BEGIN


INSERT INTO [dbo].[Users]
           ([UserName]
           ,[UserPassword]
           ,[UserTypeID]
           ,[DepartmentID]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[EmailAddress]
           ,[IsAdmin]
           ,[Disabled]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[UserImage]
           ,[IsFinance]
           ,[IsVendor])
     VALUES
           (@UserName,PWDENCRYPT(@UserPassword),@UserTypeID,@DepartmentID, @FirstName, @MiddleName, @LastName, @EmailAddress, 0, 0,
		   @UserId,GETDATE()  ,@UserId,GETDATE(),@UserImage, @IsFinance, @IsVendor)


END



GO
