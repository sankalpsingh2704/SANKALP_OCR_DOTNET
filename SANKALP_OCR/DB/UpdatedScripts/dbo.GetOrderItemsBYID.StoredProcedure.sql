USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[GetOrderItemsBYID]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[GetOrderItemsBYID]
(@ID int
) 
as
select * from dbo.OrderItems Where ID=@ID
Select * from dbo.OrderItemdetails where OrderitemID=@ID

GO
