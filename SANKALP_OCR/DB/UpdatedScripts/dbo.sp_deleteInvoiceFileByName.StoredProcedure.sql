USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_deleteInvoiceFileByName]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_deleteInvoiceFileByName]
@FileName varchar(50)
AS
BEGIN
delete from InVoiceFiles where FileName = @FileName

END
GO
