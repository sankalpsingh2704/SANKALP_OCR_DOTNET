USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getvendors]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getvendors]
AS

BEGIN
select * from vendors
END
GO
