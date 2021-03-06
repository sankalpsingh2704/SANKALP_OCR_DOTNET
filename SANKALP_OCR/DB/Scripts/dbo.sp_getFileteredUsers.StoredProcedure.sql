USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getFileteredUsers]    Script Date: 17-05-2017 19:07:14 ******/
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
