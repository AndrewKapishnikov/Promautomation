SET IDENTITY_INSERT [dbo].[Categories] ON
INSERT INTO [dbo].[Categories] ([Id], [ParentId], [Name], [UrlSlug], [Description], [BoolArticle], [Level], [FullUrl]) 
VALUES (58, 46, N'Частотные преобразователи фирмы ОВЕН','oven_converters', N'Раздел о частотных преобразователях фирмы Овен', 'false', 3, '/electrical_engineering/frequency_converters/oven_converters')
SET IDENTITY_INSERT [dbo].[Categories] OFF
