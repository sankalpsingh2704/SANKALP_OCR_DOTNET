USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTop5InvoiceItems]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetTop5InvoiceItems]
 @CurrentUserID int
AS
BEGIN
	SELECT top 5 I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,i.DateofAccount
	 FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
	 WHERE I.CurrentUserID = @CurrentUserID
	 ORDER BY I.InvoiceID DESC
END

GO
