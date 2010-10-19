SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.SPTagsInsert
	@Tags varchar(256),
	@TopicId int,
	@PreviousTags varchar(256)=NULL
AS

IF NOT @PreviousTags IS NULL
	BEGIN
	DELETE FROM Tags
	WHERE
		Tag IN (SELECT part FROM dbo.Split(@PreviousTags, ' '))
		AND
		TopicId = @TopicId
	END

INSERT INTO Tags
(Tag,TopicId)
SELECT part, @TopicId FROM dbo.Split(@Tags, ' ')
GO
