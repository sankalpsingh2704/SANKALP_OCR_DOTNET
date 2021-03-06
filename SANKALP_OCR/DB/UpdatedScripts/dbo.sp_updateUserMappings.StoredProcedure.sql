USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_updateUserMappings]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_updateUserMappings]
(
@userTypeID int,
@usrTable as UserMappingList READONLY

)
AS
BEGIN
delete from [dbo].[UserTypesMapping] WHERE UserTypeID = @userTypeID

    SET NOCOUNT ON;


    INSERT INTO UserTypesMapping(UserTypeID,UserID)
    SELECT @userTypeID, c.UserID
    FROM @usrTable c
	where c.UserID <>0
 ---   WHERE NOT EXISTS (SELECT 1 FROM UserTypesMapping WHERE UserID = c.UserID);
END
GO
