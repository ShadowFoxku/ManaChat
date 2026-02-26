CREATE PROCEDURE [identity].[usp_user_verify_details_available]
	@username NVARCHAR(256),
	@email NVARCHAR(256) = NULL,
	@phoneNumber NVARCHAR(20) = NULL
AS
BEGIN
	IF EXISTS (SELECT 1 FROM [identity].[users] WHERE Username = @username AND Deleted = 0)
	BEGIN
		SELECT 0;
		RETURN;
	END

	IF @email IS NOT NULL AND EXISTS (SELECT 1 FROM [identity].[users] WHERE Email = @email AND Deleted = 0)
	BEGIN 
		SELECT 0;
		RETURN;
	END

	IF @phoneNumber IS NOT NULL AND EXISTS (SELECT 1 FROM [identity].[users] WHERE PhoneNumber = @phoneNumber AND Deleted = 0)
	BEGIN
		SELECT 0;
		RETURN;
	END

	SELECT 1;
END