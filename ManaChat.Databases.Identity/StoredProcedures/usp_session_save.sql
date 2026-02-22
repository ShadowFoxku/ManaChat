CREATE PROCEDURE [identity].[usp_session_save]
	@Id BIGINT,
	@UserId BIGINT,
	@Token VARCHAR(512),
	@StartedAt DATETIMEOFFSET,
	@ExpiresAt DATETIMEOFFSET
AS
BEGIN
	SET NOCOUNT ON;
	IF @Id = 0
	BEGIN
		INSERT INTO [identity].[sessions] (UserId, Token, CreatedAt, ExpiresAt)
		VALUES (@UserId, @Token, @StartedAt, @ExpiresAt);
		SET @Id = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		UPDATE [identity].[sessions]
		SET ExpiresAt = @ExpiresAt
		WHERE Id = @Id;
	END

	SELECT @Id as Id;
END