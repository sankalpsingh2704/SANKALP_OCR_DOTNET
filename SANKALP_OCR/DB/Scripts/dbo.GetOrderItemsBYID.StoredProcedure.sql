USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[GetOrderItemsBYID]    Script Date: 17-05-2017 19:07:14 ******/
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
