USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetVendorRatings]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetVendorRatings]
AS
BEGIN
	SELECT VR.VendorID, V.VendorName
	,AVG(VR.Rating) AS [Average Rating]
	,COUNT(DISTINCT(VR.InvoiceID)) AS [No. of Invoices]
	FROM VendorRating VR INNER JOIN Vendors V
	ON V.VendorID = VR.VendorID
	GROUP BY VR.VendorID, V.VendorName
	HAVING AVG(VR.Rating) > 0
END


GO
