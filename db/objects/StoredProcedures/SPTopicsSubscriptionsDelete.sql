SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPTopicsSubscriptionsDelete]
	@TopicId int
	,@UserId int
	,@Userguid char(32)
AS
DELETE S
FROM 
	TopicsSubscriptions S
	INNER JOIN Users U ON U.UserId = S.UserId
WHERE
	S.TopicId = @TopicId
	AND
	S.UserId = @UserId	
	AND
	U.UserGuid = @UserGuid
	

GO
