USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertPurchaseOrder]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertPurchaseOrder]
(
@PODate datetime,
@PONumber varchar(max),
@POAmount float,
@Createdby int,
@POFileName varchar(max)
)
AS 
BEGIN

INSERT INTO [dbo].[PurchaseOrder]
           ([PODate]
           ,[PONumber]
           ,[POAmount]
           ,[Createdby]
           ,[Createddate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[POFileName])
     VALUES
           (@PODate,@PONumber,@POAmount,@Createdby,GETDATE(),@Createdby,GETDATE(),@POFileName)
END



GO
