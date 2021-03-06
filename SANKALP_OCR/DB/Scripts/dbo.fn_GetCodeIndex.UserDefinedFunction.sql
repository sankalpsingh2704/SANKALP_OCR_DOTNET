USE [IQInvoiceItem]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetCodeIndex]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_GetCodeIndex] (@Code varchar(100)='DF')
RETURNS int    
AS    
BEGIN    
 DECLARE @strCode VARCHAR(100)       

declare @CodeIndex int 
--declare @qty int
--select @quantity = LEFT(@quantity, CHARINDEX('.', @quantity) - 1) WHERE CHARINDEX('.', @quantity) > 0
--select @qty = Cast(@quantity as int)
select  @CodeIndex =  MAX(LastIndex) + 1 FROM ItemCodes where Code= @Code 

select @strCode = @Code + cast(@CodeIndex as varchar(100))
RETURN  ISNULL(@CodeIndex,1)
END 
GO
