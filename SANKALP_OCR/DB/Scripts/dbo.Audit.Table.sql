USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[Audit]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Audit](
	[AuditID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[CreatedBy] [int] NULL,
	[CommentID] [int] NULL,
	[AssignedTo] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Audit] ADD  CONSTRAINT [DF_Audit_ModifiedaDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
