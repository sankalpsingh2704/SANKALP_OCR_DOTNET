USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[DebitNoteDetails]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DebitNoteDetails](
	[DebitNoteId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NULL,
	[DebitFileName] [nvarchar](max) NULL,
	[DebitAmount] [bigint] NULL,
 CONSTRAINT [PK_DebitNoteDetails] PRIMARY KEY CLUSTERED 
(
	[DebitNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
