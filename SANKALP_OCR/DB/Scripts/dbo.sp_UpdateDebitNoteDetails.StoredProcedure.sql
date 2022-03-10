USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateDebitNoteDetails]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_UpdateDebitNoteDetails]
( @amount float,
@Id int )
AS

BEGIN
UPDATE DebitNoteDetails SET DebitAmount= @amount WHERE DebitNoteId= @Id
END
GO
