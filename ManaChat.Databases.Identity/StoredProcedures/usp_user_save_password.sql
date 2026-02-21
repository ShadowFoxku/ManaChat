CREATE PROCEDURE [identity].[usp_user_save_password]
	@UserId BIGINT,
	@PasswordHash NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [identity].[users]
	SET PasswordHash = @PasswordHash
	WHERE Id = @UserId;
END