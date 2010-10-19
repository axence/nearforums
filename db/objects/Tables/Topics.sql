SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Topics](
	[TopicId] [int] IDENTITY(1,1) NOT NULL,
	[TopicTitle] [varchar](256) NOT NULL,
	[TopicShortName] [varchar](64) NOT NULL,
	[TopicDescription] [varchar](max) NOT NULL,
	[TopicCreationDate] [datetime] NOT NULL,
	[TopicLastEditDate] [datetime] NOT NULL,
	[TopicViews] [int] NOT NULL,
	[TopicReplies] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[TopicTags] [varchar](256) NOT NULL,
	[ForumId] [int] NOT NULL,
	[TopicLastEditUser] [int] NOT NULL,
	[TopicLastEditIp] [varchar](15) NOT NULL,
	[Active] [bit] NOT NULL,
	[TopicIsClose] [bit] NOT NULL,
	[TopicOrder] [int] NULL,
	[LastMessageId] [int] NULL,
	[MessagesIdentity] [int] NOT NULL,
 CONSTRAINT [PK_Topics] PRIMARY KEY CLUSTERED 
(
	[TopicId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Topics]  WITH CHECK ADD  CONSTRAINT [FK_Topics_Forums] FOREIGN KEY([ForumId])
REFERENCES [dbo].[Forums] ([ForumId])
GO
ALTER TABLE [dbo].[Topics]  WITH CHECK ADD  CONSTRAINT [FK_Topics_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Topics]  WITH CHECK ADD  CONSTRAINT [FK_Topics_Users_LastEdit] FOREIGN KEY([TopicLastEditUser])
REFERENCES [dbo].[Users] ([UserId])
GO
