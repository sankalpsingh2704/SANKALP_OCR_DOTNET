USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_insertDebitNoteDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[sp_insertDebitNoteDetails]
(
@InvoiceId int,
@DebitFileName varchar(200),
@DebitAmount decimal
)
AS
BEGIN

INSERT INTO [dbo].[DebitNoteDetails]
           ([InvoiceId]
           ,[DebitFileName]
           ,[DebitAmount])
     VALUES
           (@InvoiceId
           ,@DebitFileName
           ,@DebitAmount)

END


GO
