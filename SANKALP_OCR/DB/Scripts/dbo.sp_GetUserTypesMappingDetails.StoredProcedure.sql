USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserTypesMappingDetails]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[sp_GetUserTypesMappingDetails]
AS
BEGIN

select  DENSE_RANK() OVER(ORDER BY UTM.UserTypeID ASC) AS RowNo, 
UT.UserTypeID,U.UserID ,U.UserName,UT.UserType as UserTypeName
from UserTypes UT 
LEFT JOIN UserTypesMapping UTM ON UT.UserTypeID=UTM.UserTypeID
LEFT JOIN Users U ON U.UserID=UTM.UserID
WHERE UT.Disabled=0

END
GO
