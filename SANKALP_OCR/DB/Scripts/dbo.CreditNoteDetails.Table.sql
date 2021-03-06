USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[CreditNoteDetails]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CreditNoteDetails](
	[CreditNoteId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NULL,
	[CreditFileName] [nvarchar](max) NULL,
	[CreditAmount] [bigint] NULL,
 CONSTRAINT [PK_CreditNoteDetails] PRIMARY KEY CLUSTERED 
(
	[CreditNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
