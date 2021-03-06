USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllInvoiceItemsForUser]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAllInvoiceItemsForUser]
 @UserID int
AS
BEGIN
	SELECT I.InvoiceID,  I.InvoiceNumber,I.InvoiceAmount,I.PONumber,I.PANNumber,V.VendorName, I.InvoiceDate,I.InvoiceDueDate, I.InvoiceReceiveddate,I.Dateofpayment,i.DateofAccount,I.CurrentStatus,U.UserName
	 FROM [dbo].[InVoice] I INNER JOIN Vendors V ON V.VendorID = I.VendorID
	 INNER JOIN Users U ON U.UserID = I.CurrentUserID
	 WHERE I.InvoiceID IN(select DISTINCT InvoiceId FROM InvoiceStatus WHERE UserId=@UserID)
	 OR I.CreatedBy=@UserID  OR I.CurrentUserID=@UserID
	 ORDER BY I.InvoiceID DESC

END

GO
