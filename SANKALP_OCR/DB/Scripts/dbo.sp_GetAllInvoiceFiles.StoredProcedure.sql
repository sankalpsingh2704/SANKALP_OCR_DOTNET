USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllInvoiceFiles]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllInvoiceFiles]

AS
BEGIN
select * from InVoiceFiles

END

GO
