USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[CheckCredential]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Script Template  
-- =============================================  
CREATE PROCEDURE [dbo].[CheckCredential]        
 @UserName nvarchar(50),  
 @UserPassword nvarchar(255)
         
  
AS            
       declare @isVendor  bit =0  
 SET NOCOUNT ON            
 SET ANSI_WARNINGS OFF     
  --Declare @exists int;         
  --Select @exists = COUNT(*) from Users where UserName=@UserName  and PWDCOMPARE(@UserPassword,UserPassword) = 1 
  --if(@exists = 1)   
  --begin 
  if exists(Select 1 From Users where UserName=@UserName and [Disabled]=0  )
  BEGIN
Select PWDCOMPARE (@UserPassword,UserPassword) as IsValid,  
     UserID,  
     UserName,  
     DepartmentID,  
     UserTypeID,  
     FirstName,  
     MiddleName,  
     LastName,  
     EmailAddress,  
     UserImage,  
     [Disabled],  
     IsAdmin,
	 IsFinance,
	@isVendor as IsVendor ,
	IsPaymentApproval   
From Users where UserName=@UserName and [Disabled]=0  
END
ELSE
SELECT 0 as IsValid
--end 
--else
--begin
--select null
--end

GO
