USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Vendor] [nvarchar](200) NULL,
	[InvoiceNo] [nvarchar](100) NULL,
	[InvoiceDate] [datetime] NULL,
	[PO] [nvarchar](100) NULL,
	[PODate] [datetime] NULL,
	[Amount] [float] NULL,
	[PAN] [nvarchar](100) NULL,
	[BUYERCSTNO] [nvarchar](100) NULL,
	[BUYERVATTIN] [nvarchar](100) NULL,
	[COMPANYCSTNO] [nvarchar](100) NULL,
	[COMPANYVATTIN] [nvarchar](100) NULL,
	[ImageFilePath] [nvarchar](2000) NULL,
	[Dateofpayment] [date] NULL,
	[InvoiceDueDate] [date] NULL,
 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
