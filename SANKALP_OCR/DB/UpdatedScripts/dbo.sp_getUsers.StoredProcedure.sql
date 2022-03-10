USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getUsers]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getUsers]
As
select USR.UserID,USR.UserName,USR.Disabled,usr.UserTypeID,UT.UserType FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID = USR.UserTypeID AND UT.Disabled = 0 AND Usr.UserID <> 3 AND usr.Disabled=0
GO
