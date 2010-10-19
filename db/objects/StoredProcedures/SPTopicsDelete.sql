SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SPTopicsDelete]
	@TopicId int
	,@UserId int
	,@Ip varchar(15)
AS
/*
- SETS THE TOPIC ACTIVE=0
- UPDATES RECOUNT ON FORUM
*/

DECLARE @ForumId int;
SELECT @ForumId = ForumId FROM Topics WHERE TopicId = @TopicId;

UPDATE Topics
SET
	Active = 0
	,TopicLastEditDate = GETUTCDATE()
	,TopicLastEditUser = @UserId
	,TopicLastEditIp = @Ip
WHERE
	TopicId = @TopicId;

exec dbo.SPForumsUpdateRecount @ForumId;
GO
