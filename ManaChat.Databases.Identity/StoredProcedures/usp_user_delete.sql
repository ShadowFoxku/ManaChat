CREATE PROCEDURE [identity].[usp_user_delete]
	@UserId BIGINT
AS
BEGIN
	UPDATE [identity].[users]
	SET Deleted = 1
	WHERE Id = @UserId;
END
