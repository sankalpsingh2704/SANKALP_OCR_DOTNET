USE [IQInvoiceItemNew]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetCode]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_GetCode] (@Code varchar(10)='DF')
RETURNS VARCHAR(10)    
AS    
BEGIN    
 DECLARE @strCode VARCHAR(10)       
--declare @Code varchar(10)



--declare @Code varchar(10) ='AF'
declare @CodeIndex int 
select  @CodeIndex =  MAX(LastCodeIndex) + 1 FROM OrderItemDetails where Code= @Code 
--update OrderItemDetails SET LastCodeIndex = @CodeIndex where Code= @Code 
SELECT  top 1 @strCode =  @Code + CAST(@CodeIndex as varchar(10))  FROM OrderItemDetails where Code= @Code 
RETURN @strcode
END 

GO
