SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPForumsUpdateRecount]
	@ForumId int = 2
AS
/*
	RECOUNTS THE CHILDREN MESSAGES AND TOPICS
*/
DECLARE @ForumTopicCount int, @ForumMessageCount int;

SELECT
	@ForumTopicCount = COUNT(TopicId)
	,@ForumMessageCount = SUM(TopicReplies)
FROM
	Topics
WHERE
	ForumId = @ForumId
	AND
	Active = 1;

UPDATE Forums
SET 
	ForumTopicCount = ISNULL(@ForumTopicCount, 0)
	,ForumMessageCount = ISNULL(@ForumMessageCount, 0)
WHERE	
	ForumId = @ForumId;

GO
