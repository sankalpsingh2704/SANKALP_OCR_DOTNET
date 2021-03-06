USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_getChartDetails_2]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	CREATE PROCEDURE 	[dbo].[sp_getChartDetails_2]
	As
	BEGIN
	declare @mthtable TABLE
	(
	monthVal int,
	yearVal int
	)
--INSERT INTO @mthtable(monthVal,yearVal)(
	--SELECT DATEPART(MONTH,Dateofpayment) as monthVal ,DATEPART(YEAR,Dateofpayment) as yearVal
	--FROM InVoice WHERE Dateofpayment is not null 
	--GROUP BY DATEPART(MONTH, Dateofpayment),DATEPART(YEAR, Dateofpayment)
	--UNION 
	--SELECT DATEPART(MONTH,InvoiceDueDate) as monthVal ,DATEPART(YEAR,InvoiceDueDate) as yearVal
	--FROM InVoice WHERE InvoiceDueDate is not null 
	--GROUP BY DATEPART(MONTH, InvoiceDueDate),DATEPART(YEAR, InvoiceDueDate))
	declare @iMonth int
	declare @sYear varchar(4)
	declare @sMonth varchar(2)
	set @iMonth = 0
	while @iMonth > -12
	begin
	set @sYear = year(DATEADD(month,@iMonth,GETDATE()))
	set @sMonth = right('0'+cast(month(DATEADD(month,@iMonth,GETDATE())) as varchar(2)),2)
	--select @sYear as YY ,@sMonth as MM
	INSERT INTO @mthtable(monthVal,yearVal)Values(@sMonth,@sYear)
	set @iMonth = @iMonth - 1
	end
---------------------------------------------------------------------
declare @table TABLE
	(
	monthVal int,
	yearVal int,
	PaidInvoices int
	)
	INSERT INTO @table(monthVal,yearVal,PaidInvoices)(
	SELECT monthVal,yearVal,
	Count(1) as PaidInvoices
	FROM @mthtable t  LEFT JOIN InVoice ON T.monthVal = DATEPART(MONTH, Dateofpayment) AND T.yearVal=DATEPART(YEAR, Dateofpayment)
	WHERE Dateofpayment is not null 
	GROUP BY monthVal,yearVal)

declare @table2 TABLE
	(
	monthVal int,
	yearVal int,
	DueInvoices int
	)
	INSERT INTO @table2(monthVal,yearVal,DueInvoices)(
	SELECT monthVal,yearVal,
	Count(1) as DueInvoices
	FROM @mthtable t  LEFT JOIN InVoice ON T.monthVal = DATEPART(MONTH, InvoiceDueDate) AND T.yearVal=DATEPART(YEAR, InvoiceDueDate)
	WHERE InvoiceDueDate is not null 
	GROUP BY monthVal,yearVal)

	--select * from @table2

	select MT.monthVal,MT.yearVal,ISNULL(T.PaidInvoices,0) AS PaidInvoices,ISNULL(T2.DueInvoices,0) AS DueInvoices from @mthtable MT
    left JOIN @table T on MT.monthVal=t.monthVal AND t.yearVal=MT.yearVal
	left JOIN @table2 T2 on MT.monthVal=T2.monthVal AND T2.yearVal=MT.yearVal
	ORDER BY MT.yearVal ,MT.monthVal
	END





GO
