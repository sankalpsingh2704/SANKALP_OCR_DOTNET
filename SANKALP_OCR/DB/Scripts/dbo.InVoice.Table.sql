USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[InVoice]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InVoice](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [varchar](50) NULL,
	[PANNumber] [varchar](50) NULL,
	[PONumber] [varchar](50) NULL,
	[InvoiceDate] [datetime] NULL,
	[InvoiceReceivedDate] [datetime] NULL,
	[InvoiceDueDate] [datetime] NULL,
	[TaxTypeID] [int] NULL,
	[VendorID] [int] NULL,
	[DepartmentID] [int] NULL,
	[CostCenterID] [int] NULL,
	[CostUnitID] [int] NULL,
	[EndUserStatus] [varchar](5) NULL,
	[EndUserApprovalStatus] [varchar](5) NULL,
	[InvoiceAmount] [float] NULL,
	[FinalApprovalStatus] [varchar](5) NULL,
	[InitiatorVerificationStatus] [varchar](5) NULL,
	[WFStatus] [int] NULL,
	[Initiator] [varchar](5) NULL,
	[Dateofpayment] [datetime] NULL,
	[DateofAccount] [datetime] NULL,
	[Navisionvoucherno] [varchar](25) NULL,
	[EndUserID] [int] NULL,
	[EndUserApprovalID] [int] NULL,
	[FinalApprovalID] [int] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ReceiptNo] [varchar](50) NULL,
	[CurrentUserID] [int] NULL,
	[CurrentStatus] [varchar](50) NULL,
	[FilePath] [varchar](max) NULL,
 CONSTRAINT [PK_InVoice] PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[InVoice] ADD  CONSTRAINT [DF_InVoice_EndUserID]  DEFAULT ((0)) FOR [EndUserID]
GO
ALTER TABLE [dbo].[InVoice] ADD  CONSTRAINT [DF_InVoice_EndUserApprovalID]  DEFAULT ((0)) FOR [EndUserApprovalID]
GO
ALTER TABLE [dbo].[InVoice] ADD  CONSTRAINT [DF_InVoice_FinalApprovalID]  DEFAULT ((0)) FOR [FinalApprovalID]
GO
ALTER TABLE [dbo].[InVoice] ADD  CONSTRAINT [DF_InVoice_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[InVoice] ADD  CONSTRAINT [DF_InVoice_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_CostCenters] FOREIGN KEY([CostCenterID])
REFERENCES [dbo].[CostCenters] ([CostCenterID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_CostCenters]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_CostUnits] FOREIGN KEY([CostUnitID])
REFERENCES [dbo].[CostUnits] ([CostUnitID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_CostUnits]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([DeptID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_Departments]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_Users] FOREIGN KEY([EndUserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_Users]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_Users1] FOREIGN KEY([EndUserApprovalID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_Users1]
GO
ALTER TABLE [dbo].[InVoice]  WITH CHECK ADD  CONSTRAINT [FK_InVoice_Vendors] FOREIGN KEY([VendorID])
REFERENCES [dbo].[Vendors] ([VendorID])
GO
ALTER TABLE [dbo].[InVoice] CHECK CONSTRAINT [FK_InVoice_Vendors]
GO
