SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SPTemplatesDelete]
	@TemplateId int
AS
DELETE FROM Templates WHERE TemplateId = @TemplateId
GO
