USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_IsPoNumberExists]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_IsPoNumberExists] 
(
@PONumber varchar(250)

)
AS
BEGIN 
declare @isDuplicate bit
IF EXISTS (SELECT 1 FROM Purchaseorder WHERE PONumber = @PoNumber)
	BEGIN
		SELECT @isDuplicate = 1
	END
	ELSE
	BEGIN
		SELECT @isDuplicate =0 
	END 
	SELECT @isDuplicate AS IsDuplicate
END

GO
