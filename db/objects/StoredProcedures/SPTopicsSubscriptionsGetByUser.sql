SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.SPTopicsSubscriptionsGetByUser
	@UserId int=21
AS
SELECT
	T.TopicId
	,T.TopicTitle
	,T.TopicShortName
	,T.ForumId
	,T.ForumName
	,T.ForumShortName
FROM
	TopicsSubscriptions S
	INNER JOIN TopicsComplete T ON T.TopicId = S.TopicId
WHERE
	S.UserId = @UserId
ORDER BY
	S.TopicId DESC
GO
