USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[GetOrderItems]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[GetOrderItems]

as
select * from dbo.OrderItems

GO
