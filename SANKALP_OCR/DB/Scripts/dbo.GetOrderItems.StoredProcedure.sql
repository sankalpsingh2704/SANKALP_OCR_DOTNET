USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[GetOrderItems]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[GetOrderItems]

as
select * from dbo.OrderItems

GO
