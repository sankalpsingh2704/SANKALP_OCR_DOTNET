USE [IQInvoiceItemNew]
GO
/****** Object:  Table [dbo].[InvoiceTax]    Script Date: 03-10-2017 14:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceTax](
	[InvoiceTaxTypeID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[TaxtypeID] [int] NULL,
	[TaxValue] [int] NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_InvoiceTax] PRIMARY KEY CLUSTERED 
(
	[InvoiceTaxTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[InvoiceTax]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceTax_TaxTypes] FOREIGN KEY([TaxtypeID])
REFERENCES [dbo].[TaxTypes] ([TaxTypeID])
GO
ALTER TABLE [dbo].[InvoiceTax] CHECK CONSTRAINT [FK_InvoiceTax_TaxTypes]
GO
