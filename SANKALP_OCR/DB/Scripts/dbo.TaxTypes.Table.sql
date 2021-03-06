USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[TaxTypes]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxTypes](
	[TaxTypeID] [int] IDENTITY(1,1) NOT NULL,
	[TaxTypeName] [nvarchar](100) NOT NULL,
	[TaxTypeDescription] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_TaxTypes] PRIMARY KEY CLUSTERED 
(
	[TaxTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
