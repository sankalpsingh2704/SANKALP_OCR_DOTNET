USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertInvoiceItems]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertInvoiceItems]
(
@Id varchar(50),
@Code varchar(50)= null,
@Name varchar(50) = null,
@Qty varchar(50) = null,
@Rate varchar(50) = null,
@Amount varchar(50) = null,
@IGST varchar(50) = null,
@SGST varchar(50) = null,
@CGST varchar(50) = null
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[InvoiceItems]
           ([ItemId]
           ,[Code]
           ,[Name]
		   ,[Qty]
		   ,[Rate]
		   ,[Amount]
		   ,[IGST]
		   ,[SGST]
		   ,[CGST]
		   )
     VALUES
           (@Id,@Code,@Name,@Qty,@Rate,@Amount,@IGST,@SGST,@CGST)
END

GO
