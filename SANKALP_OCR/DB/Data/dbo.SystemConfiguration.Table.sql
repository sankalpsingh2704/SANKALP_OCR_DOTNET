USE [IQInvoiceItem]
GO
SET IDENTITY_INSERT [dbo].[SystemConfiguration] ON 

INSERT [dbo].[SystemConfiguration] ([SystemID], [SmtpServer], [OutgoingPortNo], [CompanyEmailFrom], [CompanyEmailFromPassword], [SSL]) VALUES (1, N'http', 2345, N'bharath@iqss.co.in', N'xw2IaKGIzRxLWGIkSYIZIg==', 0)
SET IDENTITY_INSERT [dbo].[SystemConfiguration] OFF
