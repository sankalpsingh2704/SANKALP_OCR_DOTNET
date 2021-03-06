USE [IQInvoiceItem]
GO
/****** Object:  Table [dbo].[SiteReferenceDetails]    Script Date: 17-05-2017 19:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiteReferenceDetails](
	[SiteID] [int] NULL,
	[SiteReference] [nvarchar](255) NULL,
	[BuildingReference] [nvarchar](255) NULL,
	[FloorReference] [nvarchar](255) NULL,
	[RoomReference] [nvarchar](255) NULL,
	[Room Reference] [nvarchar](255) NULL
) ON [PRIMARY]

GO
