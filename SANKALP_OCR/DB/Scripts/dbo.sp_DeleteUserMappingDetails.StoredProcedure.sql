USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteUserMappingDetails]    Script Date: 17-05-2017 19:07:14 ******/
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
