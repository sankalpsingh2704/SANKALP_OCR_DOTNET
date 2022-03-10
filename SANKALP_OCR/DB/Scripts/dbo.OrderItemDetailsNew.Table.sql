USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[OrderItemDetailsNew]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderItemDetailsNew](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemId] [int] NULL,
	[ColumnName] [varchar](100) NULL,
	[RowNo] [int] NULL,
	[ColumnValue] [varchar](200) NULL,
 CONSTRAINT [PK_OrderItemDetailsNew] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
