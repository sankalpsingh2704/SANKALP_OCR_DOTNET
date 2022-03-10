USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetDebitData]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[SP_GetDebitData](@invoiceid int)
AS
BEGIN
			SELECT VR.InvoiceId,VR.DebitNoteId,VR.DebitFileName,VR.DebitAmount
		FROM DebitNoteDetails VR
		WHERE VR.DebitNoteId = @invoiceid
END

GO
