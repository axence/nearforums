SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Forums](
	[ForumId] [int] IDENTITY(1,1) NOT NULL,
	[ForumName] [varchar](255) NOT NULL,
	[ForumShortName] [varchar](32) NOT NULL,
	[ForumDescription] [varchar](max) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[ForumCreationDate] [datetime] NOT NULL,
	[ForumLastEditDate] [datetime] NOT NULL,
	[ForumLastEditUser] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[ForumTopicCount] [int] NOT NULL,
	[ForumMessageCount] [int] NOT NULL,
	[ForumOrder] [int] NOT NULL,
 CONSTRAINT [PK_Forums] PRIMARY KEY CLUSTERED 
(
	[ForumId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Forums_ForumShortName] ON [dbo].[Forums] 
(
	[ForumShortName] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Forums]  WITH CHECK ADD  CONSTRAINT [FK_Forums_ForumsCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[ForumsCategories] ([CategoryId])
GO
ALTER TABLE [dbo].[Forums]  WITH CHECK ADD  CONSTRAINT [FK_Forums_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Forums]  WITH CHECK ADD  CONSTRAINT [FK_Forums_Users_LastEdit] FOREIGN KEY([ForumLastEditUser])
REFERENCES [dbo].[Users] ([UserId])
GO
