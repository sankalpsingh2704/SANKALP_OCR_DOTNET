USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getAllUsers]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getAllUsers]
As
select USR.*,UT.UserType FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID=USR.UserTypeID 

GO
