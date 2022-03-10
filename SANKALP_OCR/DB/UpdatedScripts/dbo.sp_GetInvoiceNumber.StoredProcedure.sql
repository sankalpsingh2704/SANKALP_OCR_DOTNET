USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetInvoiceNumber]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetInvoiceNumber]
(
@invoice int
)
AS
BEGIN
DECLARE @invoiceId int
select top 1 @invoiceId =  InvoiceID from InVoice order by InvoiceID desc
select @invoiceId + 1
END


GO
