USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[ItemCodes]    Script Date: 03-10-2017 14:44:40 ******/
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
	[LastIndex] [int] NULL CONSTRAINT [DF_ItemCodes_LastIndex]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
