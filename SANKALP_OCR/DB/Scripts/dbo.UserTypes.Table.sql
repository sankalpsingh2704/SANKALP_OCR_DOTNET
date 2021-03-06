USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[UserTypes]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTypes](
	[UserTypeID] [int] IDENTITY(1,1) NOT NULL,
	[UserType] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[UserID] [int] NULL,
	[ColumnOrder] [int] NULL,
 CONSTRAINT [PK_UserTypes] PRIMARY KEY CLUSTERED 
(
	[UserTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[UserTypes] ADD  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[UserTypes] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
