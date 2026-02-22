CREATE PROCEDURE [identity].[usp_user_save]
	@Id BIGINT,
	@Username NVARCHAR(256),
	@Email NVARCHAR(256) NULL,
	@PhoneNumber NVARCHAR(20) NULL,
	@ServerId BIGINT NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @Id = 0 
	BEGIN
		INSERT INTO [identity].[users] (Username, Email, PhoneNumber, ServerId)
		VALUES (@Username, @Email, @PhoneNumber, @ServerId);
		SET @Id = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		UPDATE [identity].[users]
		SET 
			Username = @Username,
			Email = @Email,
			PhoneNumber = @PhoneNumber,
			ServerId = @ServerId
		WHERE Id = @Id;
	END

	SELECT @Id as Id;
END
