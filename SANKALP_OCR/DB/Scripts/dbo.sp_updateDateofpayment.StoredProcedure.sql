USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_updateDateofpayment]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_updateDateofpayment]
(
@Dateofpayment datetime,
@InvoiceID int
)
As
BEGIN

UPDATE InVoice set Dateofpayment =@Dateofpayment, CurrentStatus = 'Paid' from InVoice where InvoiceID = @InvoiceID
END
GO
