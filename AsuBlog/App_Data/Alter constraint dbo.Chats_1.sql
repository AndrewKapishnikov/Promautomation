USE [AsuBlog]
GO

ALTER TABLE [dbo].[Chats]
DROP CONSTRAINT [FK_dbo.Chats_dbo.AspNetUsers_UserId]

GO
/****** Object: Table [dbo].[Chats] Script Date: 12.12.2022 18:47:16 ******/
ALTER TABLE [dbo].[Chats]
ADD CONSTRAINT [FK_dbo.Chats_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION;


