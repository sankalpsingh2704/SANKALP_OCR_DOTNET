USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[VendorRating]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VendorRating](
	[VendorRatingID] [int] IDENTITY(1,1) NOT NULL,
	[VendorID] [int] NULL,
	[InvoiceID] [int] NULL,
	[Rating] [int] NOT NULL,
	[Comment] [varchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_VendorRating] PRIMARY KEY CLUSTERED 
(
	[VendorRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
