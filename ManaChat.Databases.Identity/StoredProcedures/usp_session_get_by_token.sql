CREATE PROCEDURE [identity].[usp_session_get_by_token]
	@Token CHAR(64)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		s.Id,
		s.UserId,
		s.Token,
		s.CreatedAt,
		s.ExpiresAt
	FROM [identity].[sessions] s
	WHERE s.Token = @Token
END
