USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[ObjCategory]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ObjCategory](
	[ID] [int] NOT NULL,
	[Reference] [nvarchar](100) NULL,
	[DefaultClassification] [nvarchar](50) NULL,
 CONSTRAINT [PK_ObjCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
