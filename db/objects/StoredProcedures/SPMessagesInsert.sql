SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE PROCEDURE [dbo].[SPMessagesInsert]
	@TopicId int
	,@MessageBody varchar(max)
	,@UserId int
	,@MessageId int OUTPUT
	,@Ip varchar(15)
	,@ParentId int
AS

UPDATE T
	SET
	MessagesIdentity = MessagesIdentity+1
	,@MessageId = MessagesIdentity+1
FROM
	Topics T
WHERE
	TopicId = @TopicId


BEGIN TRY
	BEGIN TRANSACTION

	INSERT INTO Messages
	(
	TopicId
	,MessageId
	,MessageBody
	,MessageCreationDate
	,MessageLastEditDate
	,MessageLastEditUser
	,UserId
	,Active
	,EditIp
	,ParentId
	)
	VALUES
	(
	@TopicId
	,@MessageId
	,@MessageBody
	,GETUTCDATE()
	,GETUTCDATE()
	,@UserId
	,@UserId
	,1
	,@Ip
	,@ParentId
	)


	
	--Update topic
	exec SPTopicsUpdateLastMessage @TopicId=@TopicId, @MessageId=@MessageId
	--Update forums
	exec SPForumsUpdateLastMessage @TopicId=@TopicId, @MessageId=@MessageId
	COMMIT

END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK

  -- Raise an error with the details of the exception
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
		 @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)

END CATCH








GO
