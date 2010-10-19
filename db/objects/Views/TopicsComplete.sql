SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW dbo.TopicsComplete 
AS
	SELECT
		T.TopicId
		,T.TopicTitle
		,T.TopicShortName
		,T.TopicDescription
		,T.TopicCreationDate
		,T.TopicViews
		,T.TopicReplies
		,T.UserId
		,T.TopicTags
		,T.TopicIsClose
		,T.TopicOrder
		,T.LastMessageId
		,U.UserName
		,F.ForumId
		,F.ForumName
		,F.ForumShortName
	FROM
		Topics T
		INNER JOIN Users U ON U.UserId = T.UserId
		INNER JOIN Forums F ON F.ForumId = T.ForumId
	WHERE
		T.Active = 1
GO
