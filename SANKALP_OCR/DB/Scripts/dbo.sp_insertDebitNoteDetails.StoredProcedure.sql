USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_insertDebitNoteDetails]    Script Date: 17-05-2017 19:07:14 ******/
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
