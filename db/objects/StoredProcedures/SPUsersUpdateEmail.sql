SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.SPUsersUpdateEmail
	@UserId int
	,@UserEmail varchar(100)
	,@UserEmailPolicy int
AS
UPDATE Users
SET
	UserEmail = @UserEmail
	,UserEmailPolicy = @UserEmailPolicy
WHERE
	UserId = @UserId
GO
