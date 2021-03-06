USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetVendorRatingByID]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetVendorRatingByID]
@vendorID int
AS
BEGIN
	SELECT V.VendorName
		,I.InvoiceNumber
		,CONVERT(VARCHAR(12), I.InvoiceDate, 106) AS [Date of Invoice]
		,SUM(CASE WHEN u.UserTypeID = 2 THEN VR.Rating ELSE 0 END) AS [End User Rating]
		,MAX(CASE WHEN u.UserTypeID = 2 THEN VR.Comment ELSE '' END) AS [End User Comments]
		,SUM(CASE WHEN u.UserTypeID = 3 THEN VR.Rating ELSE 0 END) AS [Ultimate Approver Rating]
		,MAX(CASE WHEN u.UserTypeID = 3 THEN VR.Comment ELSE '' END) AS [Ultimate Approver Comments]
	FROM VendorRating VR
	INNER JOIN Vendors V
	ON VR.VendorID = V.VendorID
	INNER JOIN InVoice I
	ON VR.InvoiceID = I.InvoiceID
	INNER JOIN Users U
	ON VR.CreatedBy = U.UserID
	WHERE VR.VendorID = @vendorID
	GROUP BY I.InvoiceNumber, I.InvoiceDate, V.VendorName
	--ORDER BY VR.InvoiceID DESC
END


GO
