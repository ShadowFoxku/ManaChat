CREATE PROCEDURE [identity].[usp_user_save_password]
	@UserId BIGINT,
	@PasswordHash VARCHAR(255)
AS
BEGIN
	UPDATE [identity].[users]
	SET 
		PasswordHash = @PasswordHash
	WHERE Id = @UserId;

	RETURN @@ROWCOUNT;
END