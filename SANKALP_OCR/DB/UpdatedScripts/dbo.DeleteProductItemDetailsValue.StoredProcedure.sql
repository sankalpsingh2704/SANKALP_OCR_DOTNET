USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[DeleteProductItemDetailsValue]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteProductItemDetailsValue]
(
	@ID int
)
AS

DECLARE @OrderItemID int
SELECT @OrderItemID = OrderItemID from OrderItemDetails WHERE ID=@ID
DELETE FROM OrderItemDetails WHERE ID = @ID
select @OrderItemID AS OrderItemID



--select * from OrderItems
--select * from OrderItemDetails
GO
