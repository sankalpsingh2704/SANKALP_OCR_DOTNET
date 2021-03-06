USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteUserMappingDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_DeleteUserMappingDetails]
(
@userTypeID int,
@usrTable as UserMappingList READONLY

)
AS
BEGIN

   DELETE FROM UserTypesMapping WHERE ID IN (
  SELECT ID FROM UserTypesMapping WHERE UserTypeID = @userTypeID AND UserID in(Select c.UserID from @usrTable c))
    --SELECT ID FROM UserTypesMapping WHERE UserTypeID = @userTypeID AND c.UserID
    --FROM @usrTable c

END

GO
