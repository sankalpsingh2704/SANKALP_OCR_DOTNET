USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[getUserTypes]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[getUserTypes]
AS
select UserTypeID,UserType from UserTypes WHERE Disabled=0
GO
