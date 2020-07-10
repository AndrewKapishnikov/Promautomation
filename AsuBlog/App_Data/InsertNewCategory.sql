SET IDENTITY_INSERT [dbo].[Categories] ON
INSERT INTO [dbo].[Categories] ([Id], [ParentId], [Name], [UrlSlug], [Description], [BoolArticle], [Level], [FullUrl]) 
VALUES (63, 57, N'Micromaster 440','micromaster', N'Раздел о micromaster 440', 'false', 4, '/electrical_engineering/frequency_converters/siemens_converters/micromaster')
SET IDENTITY_INSERT [dbo].[Categories] OFF
