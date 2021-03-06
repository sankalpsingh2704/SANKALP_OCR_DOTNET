USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getFileteredUsers]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getFileteredUsers] 
(
@UserTypeID int

)
AS
BEGIN
select DISTINCT U.UserID,U.UserName, U.UserTypeID from Users U
LEFT JOIN UserTypesMapping UT ON UT.UserID=u.UserID
WHERE U.Disabled=0 AND U.UserID not in(select UserID from UserTypesMapping UTM where UserTypeID = @UserTypeID)
END



GO
