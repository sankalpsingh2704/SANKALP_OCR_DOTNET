USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetDebitDetails]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_GetDebitDetails](@invoiceid int)
AS
BEGIN
			SELECT VR.InvoiceId,VR.DebitNoteId,VR.DebitFileName,VR.DebitAmount
		FROM Invoice V INNER JOIN DebitNoteDetails VR
		ON V.InvoiceID = VR.InvoiceId
		WHERE VR.InvoiceId = @invoiceid
END

GO
