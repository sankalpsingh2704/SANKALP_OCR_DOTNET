USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[InsertUserTypeDetails]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[InsertUserTypeDetails] 
(

@InvoiceID int,
@ItemTable as TableInvoiceStatus READONLY
)
as

Declare @status int = 0;
Select * into #temptable from @ItemTable
--select * from #temptable
INSERT INTO [dbo].[InvoiceStatus]
           ([InvoiceId],[UserTypeId],[UserId])
select @InvoiceID, * from #temptable


GO
