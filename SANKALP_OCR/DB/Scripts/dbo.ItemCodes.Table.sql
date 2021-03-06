USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[ItemCodes]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemCodes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Reference] [varchar](200) NULL,
	[Default Classification_Code] [varchar](200) NULL,
	[LastIndex] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ItemCodes] ADD  CONSTRAINT [DF_ItemCodes_LastIndex]  DEFAULT ((0)) FOR [LastIndex]
GO
