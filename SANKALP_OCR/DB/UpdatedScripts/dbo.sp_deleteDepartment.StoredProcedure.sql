USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_deleteDepartment]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_deleteDepartment]
( @DepartmentID int )
AS

BEGIN
delete from Departments WHERE DeptID = @DepartmentID
END
GO
