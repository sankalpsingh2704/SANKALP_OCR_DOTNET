USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getAllUsers]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getAllUsers]
As
select USR.*,UT.UserType FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID=USR.UserTypeID 

GO
