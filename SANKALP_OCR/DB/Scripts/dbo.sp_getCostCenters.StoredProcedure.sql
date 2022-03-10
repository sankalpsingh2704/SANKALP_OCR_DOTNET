USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getCostCenters]    Script Date: 17-05-2017 19:07:14 ******/
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
