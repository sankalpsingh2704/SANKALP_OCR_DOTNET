USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[UpdateOrderItemDetailsNew]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE procedure [dbo].[UpdateOrderItemDetailsNew]
(
@orderItemId int,
@rowNo int,
@colName varchar(200)=null,
@columnVal varchar(200)=null

)
as

 Begin
 UPDATE OrderItemDetailsNew SET ColumnValue=@columnVal WHERE OrderItemId=@OrderItemId AND RowNo=@rowNo AND ColumnName=@colName
 
End







GO
