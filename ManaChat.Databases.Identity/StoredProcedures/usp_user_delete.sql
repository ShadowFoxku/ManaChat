CREATE PROCEDURE [identity].[usp_user_delete]
	@UserId BIGINT,
	@ReplacementName CHAR(100)
AS
BEGIN
	UPDATE [identity].[users]
	SET 
		Deleted = 1,
		[Username] = @ReplacementName,
		[Email] = @ReplacementName + '@deleted.users',
		[PhoneNumber] = ''
	WHERE Id = @UserId;
END
