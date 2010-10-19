SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TopicsSubscriptions](
	[TopicId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_TopicsSubscriptions] PRIMARY KEY CLUSTERED 
(
	[TopicId] ASC,
	[UserId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[TopicsSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_TopicsSubscriptions_Topics] FOREIGN KEY([TopicId])
REFERENCES [dbo].[Topics] ([TopicId])
GO
ALTER TABLE [dbo].[TopicsSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_TopicsSubscriptions_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
