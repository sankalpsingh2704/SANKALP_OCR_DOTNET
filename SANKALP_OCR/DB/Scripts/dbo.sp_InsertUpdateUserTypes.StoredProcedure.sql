USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdateUserTypes]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUpdateUserTypes]
(
@Id int = null,
@Description Varchar(200) = null,
@ColOrder int
)
AS
BEGIN
UPDATE [dbo].[UserTypes]
   SET [UserType] = @Description,
       [ColumnOrder] = @ColOrder,
	   [Disabled] = 0
 WHERE UserTypeID = @Id
 IF (@Id <= 0)
 BEGIN
 INSERT INTO [dbo].[UserTypes]
           ([UserType]
           ,[Disabled]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[UserID]
           ,[ColumnOrder])
     VALUES
           (@Description,0,1,GETDATE(),1,GETDATE(),null,@ColOrder)
		   END
END

GO
