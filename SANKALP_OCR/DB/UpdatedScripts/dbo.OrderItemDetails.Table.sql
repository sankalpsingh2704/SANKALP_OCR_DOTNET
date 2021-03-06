USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[OrderItemDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderItemDetails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemID] [int] NULL,
	[ItemName] [varchar](200) NULL,
	[Price] [varchar](200) NULL,
	[Qty] [varchar](200) NULL,
	[Total] [varchar](200) NULL,
	[Tax] [varchar](200) NULL,
	[Asset] [bit] NULL,
	[Code] [varchar](10) NULL,
 CONSTRAINT [PK_ProductItemDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
