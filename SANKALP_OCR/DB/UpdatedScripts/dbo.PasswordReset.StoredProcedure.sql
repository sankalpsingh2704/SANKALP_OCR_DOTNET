USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[PasswordReset]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Script Template
-- =============================================
CREATE PROCEDURE [dbo].[PasswordReset]                   
 @UserName nvarchar(50),
 @UserEmail nvarchar(50)        

AS          
          
 SET NOCOUNT ON          
 SET ANSI_WARNINGS OFF          

DECLARE @UserID as int
DECLARE @NewPassword varchar(50)


SELECT @UserID = UserID FROM Users WHERE EmailAddress=@UserEmail Or UserName=@UserName
IF (@UserID is NULL)
set @NewPassword='0'
	
IF @UserID is not NULL
Begin
	select @NewPassword = char(rand()*26+65)+char(rand()*26+65)+char(rand()*26+65)+char(rand()*26+65)+char(rand()*26+65)+char(rand()*26+65)+convert(varchar, 1 + CONVERT(INT, (250-1+1)*RAND()))
	Update Users SET UserPassword = pwdencrypt(@NewPassword) WHERE UserID=@UserID
	select @UserEmail = EmailAddress from Users where UserID=@UserID
End
select @NewPassword as ReturnValue, @UserID as UserID,@UserEmail as EmailID

GO
