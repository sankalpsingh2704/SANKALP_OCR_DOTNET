USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[CostCenters]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CostCenters](
	[CostCenterID] [int] IDENTITY(1,1) NOT NULL,
	[CostCenterName] [nvarchar](100) NOT NULL,
	[CostCenterDescription] [nvarchar](100) NOT NULL,
	[Disabled] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_CostCenters] PRIMARY KEY CLUSTERED 
(
	[CostCenterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
