USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getUsersForDropdown]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getUsersForDropdown]
As
select UTM.ID,USR.UserID,USR.UserName,UTM.UserTypeID from UserTypesMapping UTM INNER JOIN Users USR ON USR.UserID=UTM.UserID WHERE usr.IsAdmin <> 1

GO
