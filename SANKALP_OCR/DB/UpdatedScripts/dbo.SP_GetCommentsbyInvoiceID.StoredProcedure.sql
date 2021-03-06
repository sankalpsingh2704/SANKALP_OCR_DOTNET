USE [IQInvoiceItemNew]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetCommentsbyInvoiceID]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetCommentsbyInvoiceID] 
	@invoiceid int
AS
BEGIN
	select fUsr.UserName as FromUser,tUsr.UserName as ToUser, isnull(a.Comment,'') as Comment from Comments a LEFT JOIN USERS fUsr ON fUsr.UserID=a.CreatedBy
	LEFT JOIN USERS tUsr ON tUsr.UserID=a.AssignedTo  where a.InVoiceID = @invoiceid
		--select (select Username from users where userid = a.CreatedBy) as FromUser,(select Username from users where userid = a.AssignedTo) as ToUser,
		--(select isnull(Comment,'')  from Comments where CommentID = a.CommentID) as Comment from Comments as a  where a.InVoiceID = 9413

END


GO
