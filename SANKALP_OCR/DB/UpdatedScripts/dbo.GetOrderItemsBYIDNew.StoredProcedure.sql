USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[GetOrderItemsBYIDNew]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--USE [IQInvoiceItem]
--GO
--/****** Object:  StoredProcedure [dbo].[GetOrderItemsBYIDNew]    Script Date: 06-Feb-17 3:56:32 PM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
CREATE Procedure [dbo].[GetOrderItemsBYIDNew] 
(
	@ID int
) 
as
--declare @ID int= 1
DECLARE @rowCnt int
select @rowCnt= max(RowNo) From OrderItemDetailsNew WHERE OrderItemId=@id


select * from dbo.OrderItems where ID=@ID
declare @scriptTemplate varchar(MAX)
declare @script varchar(MAX)
declare @tableTemplate varchar(MAX)
SET @tableTemplate = 'create table ##tmptable (?)'
SET @scriptTemplate = '? varchar(500),'
SET @script = 'OrderItemID int, RowNo int , '
Select @script = @script + Replace(@scriptTemplate, '?', [ColumnName]) From ColumnMapping
SET @script = LEFT(@script, LEN(@script) - 1)
SET @script = Replace(@tableTemplate, '?', @script)
--Select @script
exec(@script)

declare @counter int =1

WHILE (@counter <= @rowCnt)
BEGIN

declare @scriptTemplate1 varchar(MAX)
declare @scriptTemplate2 varchar(MAX)
declare @script1 varchar(MAX)
declare @script2 varchar(MAX)
declare @tableTemplate1 varchar(MAX)
SET @tableTemplate1 = 'insert into  ##tmptable(OrderItemID,RowNo,?) values($)'
SET @scriptTemplate1 = '?, '
SET @scriptTemplate2 = '$, '
SET @script1 = ''--'OrderItemID , '
SET @script2= ''--@ID

Select @script1 = @script1 + Replace(@scriptTemplate1, '?', [ColumnName]) From ColumnMapping
Select @script2 = @script2  + Replace(@scriptTemplate2, '$', ''''+  ISNULL([ColumnValue],'') + '''') From OrderItemDetailsNew where RowNo=@counter
Select @script2 =  Cast(@ID as varchar(10)) +',' + Cast(@counter as varchar(10)) +',' + @script2 
SET @script1 = LEFT(@script1, LEN(@script1) - 1)

--SET @script2 = Replace(@script2, ',', ''',''') 
SET @script2 = LEFT(@script2, LEN(@script2) - 1)
SET @script1 = Replace(@tableTemplate1, '?', @script1)
SET @script1 = Replace(@script1, '$', @script2)
SET @script1 = Replace(@script1,'null', '')

exec(@script1)

set @counter =@counter+1
END

Select * from ##tmptable
Drop table ##tmptable

GO
