USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getvendors]    Script Date: 03-10-2017 14:44:40 ******/
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
