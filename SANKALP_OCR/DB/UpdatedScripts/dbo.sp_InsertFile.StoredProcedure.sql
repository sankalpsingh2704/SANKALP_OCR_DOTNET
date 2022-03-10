USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertFile]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertFile]
(
 @invoiceid int,
 @filename varchar(50),
 @filetype varchar(10)
)
AS
BEGIN


insert into invoicefiles(InvoiceID,[FileName],FileType) values(@invoiceid,@filename,@filetype)


END

GO
