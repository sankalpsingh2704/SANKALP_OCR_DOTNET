USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetRatingsByUser]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetRatingsByUser]
(
	@vendorID int,
	@invoiceID int,
	@userID int
)
AS
BEGIN
	SELECT VR.VendorRatingID
			,U.UserID
			,V.VendorName
			,I.InvoiceID
			,I.InvoiceNumber
			,VR.Rating
			,VR.Comment	
			,U.UserName AS [Commented By]
		FROM Vendors V 
		INNER JOIN VendorRating VR
		ON V.VendorID = VR.VendorID
		INNER JOIN InVoice I
		ON VR.InvoiceID = I.InvoiceID
		INNER JOIN Users U
		ON VR.CreatedBy = U.UserID
		WHERE VR.VendorID = @vendorID
		AND I.InvoiceID = @invoiceID
		AND U.UserID = @userID
		ORDER BY VR.InvoiceID DESC
END

GO
