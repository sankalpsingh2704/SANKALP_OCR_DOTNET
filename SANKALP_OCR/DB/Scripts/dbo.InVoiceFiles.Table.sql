USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[InVoiceFiles]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InVoiceFiles](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[FileName] [varchar](50) NULL,
	[FileType] [varchar](10) NULL,
 CONSTRAINT [PK_InVoiceFiles] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[InVoiceFiles]  WITH CHECK ADD  CONSTRAINT [FK_InVoiceFiles_InVoice] FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[InVoice] ([InvoiceID])
GO
ALTER TABLE [dbo].[InVoiceFiles] CHECK CONSTRAINT [FK_InVoiceFiles_InVoice]
GO
