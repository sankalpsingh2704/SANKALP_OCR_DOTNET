USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getDepartments]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getDepartments]
( @DepartmentID int = 0)
AS
if (@DepartmentID = 0)
BEGIN
select * from Departments
END
ELSE
BEGIN
select * from Department WHERE Departments = @DepartmentID
END
GO
