CREATE PROCEDURE [identity].[usp_user_get_by_id]
	@UserId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		u.Id,
		u.Username,
		u.Email,
		u.PhoneNumber,
		u.PasswordHash,
		u.ServerId
	FROM [identity].[users] u
	WHERE u.Id = @UserId;
END
