USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUserMappingDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUserMappingDetails]
(
@userTypeID int,
@usrTable as UserMappingList READONLY

)
AS
BEGIN

    INSERT INTO UserTypesMapping(UserTypeID,UserID)
    SELECT @userTypeID, c.UserID
    FROM @usrTable c

END
GO
