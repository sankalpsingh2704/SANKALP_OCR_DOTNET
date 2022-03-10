USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getCostCenters]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getCostCenters]
( @CostCenterID int = 0)
AS
if (@CostCenterID=0)
BEGIN
select * from CostCenters
END
ELSE
BEGIN
select * from CostCenters WHERE CostCenterID = @CostCenterID
END
GO
