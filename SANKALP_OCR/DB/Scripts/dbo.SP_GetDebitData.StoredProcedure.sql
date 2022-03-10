USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetDebitData]    Script Date: 17-05-2017 19:07:14 ******/
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
