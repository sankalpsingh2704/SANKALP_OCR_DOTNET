USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetInvoiceNumber]    Script Date: 17-05-2017 19:07:14 ******/
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
