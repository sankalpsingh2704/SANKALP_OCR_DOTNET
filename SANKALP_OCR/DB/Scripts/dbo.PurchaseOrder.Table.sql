USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrder](
	[POID] [int] IDENTITY(1,1) NOT NULL,
	[PODate] [datetime] NULL,
	[PONumber] [nvarchar](max) NULL,
	[POAmount] [decimal](18, 0) NULL,
	[Createdby] [int] NULL,
	[Createddate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[POFileName] [nvarchar](max) NULL,
 CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY CLUSTERED 
(
	[POID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
