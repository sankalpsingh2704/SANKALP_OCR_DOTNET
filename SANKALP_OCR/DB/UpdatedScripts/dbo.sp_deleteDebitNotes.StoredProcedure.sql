USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_deleteDebitNotes]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_deleteDebitNotes]
( @DebitNoteId int )
AS

BEGIN
delete from DebitNoteDetails WHERE DebitNoteId = @DebitNoteId
END
GO
