USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_CheckDuplicateInvoiceNumber]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--select * from OrderItems



CREATE PROCEDURE [dbo].[sp_CheckDuplicateInvoiceNumber] 

(

@VendorName varchar(200) = '',

@InvoiceNumber Varchar(200)

)

AS

--SELECT * FROM Vendors WHERE VendorName='SAMEER FABRICATORS'
IF EXISTS (SELECT 1 FROM InVoice WHERE InvoiceNumber=@InvoiceNumber) --AND Vendors=@VendorName)

BEGIN 

	SELECT 1 AS DuplicateInvoiceNumber

END

ELSE

BEGIN

SELECT 0 DuplicateInvoiceNumber

END

GO
