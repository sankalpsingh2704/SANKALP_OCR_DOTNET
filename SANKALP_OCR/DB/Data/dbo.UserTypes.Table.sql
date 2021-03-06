USE [IQInvoiceItem]
GO
SET IDENTITY_INSERT [dbo].[UserTypes] ON 

INSERT [dbo].[UserTypes] ([UserTypeID], [UserType], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [UserID], [ColumnOrder]) VALUES (1, N'Initiator1', 0, NULL, CAST(0x0000A71C01332708 AS DateTime), NULL, NULL, 1, 0)
INSERT [dbo].[UserTypes] ([UserTypeID], [UserType], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [UserID], [ColumnOrder]) VALUES (2, N'AdminApproval', 0, NULL, CAST(0x0000A71C01332D0B AS DateTime), NULL, NULL, 1, 3)
INSERT [dbo].[UserTypes] ([UserTypeID], [UserType], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [UserID], [ColumnOrder]) VALUES (3, N'Approval', 0, NULL, CAST(0x0000A71C013339D7 AS DateTime), NULL, NULL, 2, 1)
INSERT [dbo].[UserTypes] ([UserTypeID], [UserType], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [UserID], [ColumnOrder]) VALUES (4, N'End User', 1, NULL, CAST(0x0000A71D00C22E4C AS DateTime), NULL, NULL, 3, 4)
INSERT [dbo].[UserTypes] ([UserTypeID], [UserType], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [UserID], [ColumnOrder]) VALUES (5, N'Intermediate', 1, NULL, CAST(0x0000A71D00C2724C AS DateTime), NULL, NULL, 4, 4)
INSERT [dbo].[UserTypes] ([UserTypeID], [UserType], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [UserID], [ColumnOrder]) VALUES (11, N'Intermediate1', 1, 1, CAST(0x0000A721010E169D AS DateTime), 1, CAST(0x0000A721010E169D AS DateTime), NULL, 1)
INSERT [dbo].[UserTypes] ([UserTypeID], [UserType], [Disabled], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [UserID], [ColumnOrder]) VALUES (17, N'FinalUser', 0, 1, CAST(0x0000A72B00FB9B70 AS DateTime), 1, CAST(0x0000A72B00FB9B70 AS DateTime), NULL, 2)
SET IDENTITY_INSERT [dbo].[UserTypes] OFF
