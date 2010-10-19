SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SPTemplatesInsert]
	@TemplateKey varchar(16)
	,@TemplateDescription varchar(256)
	,@TemplateId int OUTPUT
AS
INSERT INTO Templates
(
	TemplateKey
	,TemplateDescription
	,TemplateDate
	,TemplateIsCurrent
)
VALUES
(
	@TemplateKey
	,@TemplateDescription
	,GETUTCDATE()
	,0
)

SELECT @TemplateId = @@IDENTITY


GO
