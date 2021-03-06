USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_getDashBoardetails]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getDashBoardetails]
as
BEGIN
SELECT 
	SUM(CASE WHEN (DATEDIFF(DAY ,InvoiceDueDate,Dateofpayment) >= 1  AND Dateofpayment IS NOT NULL) then 1 else 0 end ) as OVERDUE, 
	SUM(CASE WHEN (DATEDIFF(DAY ,InvoiceDueDate,GETDATE()) =0 ) then 1 else 0 end) as DUETODAY,
	SUM(CASE WHEN Dateofpayment IS NULL then 1 else 0 end) as PENDING ,
	SUM(CASE WHEN Dateofpayment IS NOT NULL then 1 else 0 end) as PAID ,
	
	SUM(CASE WHEN (InvoiceDueDate <  CAST(GETDATE() as date) AND Dateofpayment IS NOT NULL) THEN InvoiceAmount ELSE 0 END) OSOVERDUE,
	SUM(CASE WHEN (DATEDIFF(DAY ,InvoiceDueDate,GETDATE()) =0 AND Dateofpayment IS NOT NULL)  THEN InvoiceAmount ELSE 0 END) OSTODAY,
	SUM(CASE WHEN 
		(DATEDIFF(MONTH ,InvoiceDueDate,GETDATE()) =0) AND (DATEDIFF(YEAR ,InvoiceDueDate,GETDATE()) = 0) AND Dateofpayment IS NOT NULL
		THEN InvoiceAmount ELSE 0 END) 
		 OSTHISMONTH,
	SUM (CASE WHEN DATEDIFF(DAY ,InvoiceDueDate,Dateofpayment) >= 0 AND Dateofpayment IS NOT NULL THEN InvoiceAmount ELSE 0 END) TotalOS
FROM InVoice 

END


select SUM(CASE WHEN (CAST(InvoiceDueDate as date) <  CAST(GETDATE() as date) AND Dateofpayment IS NOT NULL) then 1 else 0 end ) as OVERDUE from InVoice

select SUM(CASE WHEN (DateDiff(DAY ,InvoiceDueDate,GETDATE()) =0 ) then 1 else 0 end ) as OVERDUE from InVoice


GO
