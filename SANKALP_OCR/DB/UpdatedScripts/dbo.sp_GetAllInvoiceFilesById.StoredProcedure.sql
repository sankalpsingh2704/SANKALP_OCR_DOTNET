USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllInvoiceFilesById]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllInvoiceFilesById]
@InVoiceId int
AS
BEGIN
select * from InVoiceFiles where InvoiceID = @InVoiceId 

END
GO
