CREATE PROCEDURE [identity].[usp_identity_save]
	@Id BIGINT,
	@UserId BIGINT,
	@Name NVARCHAR(256),
	@Default BIT
AS
BEGIN
	SET NOCOUNT ON;
	IF @Id = 0
	BEGIN
		INSERT INTO [identity].[identities] (UserId, [Name], [Default])
		VALUES (@UserId, @Name, @Default);
		SET @Id = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		UPDATE [identity].[identities]
		SET [Name] = @Name,
			[Default] = @Default
		WHERE Id = @Id
		AND UserId = @UserId
		AND Deleted = 0;
	END

	IF @Default = 1
	BEGIN
		UPDATE [identity].[identities]
		SET [Default] = 0
		WHERE UserId = @UserId
		AND Id <> @Id
	END

	SELECT @Id as Id;
END