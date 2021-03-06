USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[UserTypesMapping]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTypesMapping](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserTypeID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_UserTypesMapping] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[UserTypesMapping]  WITH CHECK ADD  CONSTRAINT [FK_UserTypesMapping_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserTypesMapping] CHECK CONSTRAINT [FK_UserTypesMapping_Users]
GO
ALTER TABLE [dbo].[UserTypesMapping]  WITH CHECK ADD  CONSTRAINT [FK_UserTypesMapping_UserTypes] FOREIGN KEY([UserTypeID])
REFERENCES [dbo].[UserTypes] ([UserTypeID])
GO
ALTER TABLE [dbo].[UserTypesMapping] CHECK CONSTRAINT [FK_UserTypesMapping_UserTypes]
GO
