USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[getUserTypes]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[getUserTypes]
AS
select UserTypeID,UserType from UserTypes WHERE Disabled=0
GO
