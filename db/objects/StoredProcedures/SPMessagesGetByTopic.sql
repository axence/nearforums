SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPMessagesGetByTopic]
	@TopicId int=2
AS
SELECT 
	ROW_NUMBER()
			OVER 
				(ORDER BY M.TopicId, M.MessageId)
			AS RowNumber
	,M.TopicId
	,M.MessageId
	,M.MessageBody
	,M.MessageCreationDate
	,M.MessageLastEditDate
	,M.ParentId
	,UserId
	,UserName
	,UserSignature
	,UserGroupId
	,UserGroupName
	,M.Active
FROM 
	dbo.MessagesComplete M
WHERE 
	M.TopicId = @TopicId


GO
