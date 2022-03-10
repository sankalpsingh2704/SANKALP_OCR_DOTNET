USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[InsertOrderItemDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[InsertOrderItemDetails] 
(
@rowNo int,
@colId int,
@columnVal varchar(200)=null


)
as
BEGIN

declare @ColumnName varchar(50)
select @ColumnName= ColumnName from ColumnMapping where ColumnID=@colId
INSERT INTO [dbo].[OrderItemDetailsNew]
           ([OrderItemId]
           ,[ColumnName]
           ,[RowNo]
           ,[ColumnValue])
     VALUES
           (2,@ColumnName,@rowNo,@columnVal)

END
GO
