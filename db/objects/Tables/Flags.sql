SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Flags](
	[FlagId] [int] IDENTITY(1,1) NOT NULL,
	[TopicId] [int] NOT NULL,
	[MessageId] [int] NULL,
	[Ip] [varchar](15) NOT NULL,
	[FlagDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Flags] PRIMARY KEY CLUSTERED 
(
	[FlagId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_Flags] ON [dbo].[Flags] 
(
	[TopicId] ASC,
	[MessageId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Flags]  WITH CHECK ADD  CONSTRAINT [FK_Flags_Messages] FOREIGN KEY([TopicId], [MessageId])
REFERENCES [dbo].[Messages] ([TopicId], [MessageId])
GO
