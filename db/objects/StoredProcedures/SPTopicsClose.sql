SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SPTopicsClose]
	@TopicId int
	,@UserId int
	,@Ip varchar(15)
AS
	UPDATE Topics
	SET
		TopicIsClose = 1
		,TopicLastEditDate = GETUTCDATE()
		,TopicLastEditUser = @UserId
		,TopicLastEditIp = @Ip
	WHERE
		TopicId = @TopicId
GO
