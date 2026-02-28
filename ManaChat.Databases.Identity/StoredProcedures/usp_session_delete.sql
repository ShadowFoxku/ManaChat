CREATE PROCEDURE [identity].[usp_session_delete]
	@Token CHAR(64)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id 
	FROM [identity].[sessions]
	WHERE Token = @Token;

	DELETE FROM [identity].[sessions]
	WHERE Token = @Token;
END
