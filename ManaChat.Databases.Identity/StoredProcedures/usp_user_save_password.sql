CREATE PROCEDURE [identity].[usp_user_save_password]
	@UserId BIGINT,
	@PasswordHash BINARY(32),
	@PasswordSalt BINARY(16)
AS
BEGIN
	UPDATE [identity].[users]
	SET 
		PasswordHash = @PasswordHash,
		PasswordSalt = @PasswordSalt
	WHERE Id = @UserId;

	RETURN @@ROWCOUNT;
END