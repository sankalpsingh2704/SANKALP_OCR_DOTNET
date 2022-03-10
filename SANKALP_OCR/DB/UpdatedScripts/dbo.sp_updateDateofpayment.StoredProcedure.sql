USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_updateDateofpayment]    Script Date: 03-10-2017 14:44:40 ******/
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
