USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getChartDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getChartDetails]
as
BEGIN
	--SELECT DATEPART(MONTH,Dateofpayment) as monthVal ,DATEPART(YEAR,Dateofpayment) as yearVal,
	--Count(1) as PaidInvoices
	--FROM orderItems 
	--WHERE Dateofpayment is not null 
	--GROUP BY DATEPART(MONTH, Dateofpayment),DATEPART(YEAR, Dateofpayment)
	SELECT DATEPART(MONTH,Dateofpayment) as monthVal ,DATEPART(YEAR,Dateofpayment) as yearVal,
	Count(1) as PaidInvoices
	FROM InVoice 
	WHERE Dateofpayment is not null 
	GROUP BY DATEPART(MONTH, Dateofpayment),DATEPART(YEAR, Dateofpayment)

	SELECT DATEPART(MONTH,InvoiceDueDate) as monthVal ,DATEPART(YEAR,InvoiceDueDate) as yearVal,
	Count(1) as DueInvoices
	FROM InVoice 
	WHERE InvoiceDueDate > GETDATE()
	GROUP BY DATEPART(MONTH, InvoiceDueDate),DATEPART(YEAR, InvoiceDueDate)
END


GO
