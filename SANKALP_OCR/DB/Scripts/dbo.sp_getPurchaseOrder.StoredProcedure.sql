USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getPurchaseOrder]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getPurchaseOrder]
AS

BEGIN
select * from PurchaseOrder
END
GO
