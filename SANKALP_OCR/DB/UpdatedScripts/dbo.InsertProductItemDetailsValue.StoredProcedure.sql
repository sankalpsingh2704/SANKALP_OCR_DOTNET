USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[InsertProductItemDetailsValue]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[InsertProductItemDetailsValue]
(
	@OrderItemID int
)
as
INSERT INTO [dbo].[OrderItemDetails]
           ([OrderItemID]
           ,[ItemName]
           ,[Price]
           ,[Qty]
           ,[Total]
           ,[Tax]
           ,[Asset])
     VALUES
	 (@OrderItemID,null,null,null,null,null,0)


GO
