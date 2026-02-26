CREATE PROCEDURE [identity].[usp_user_search_by_username]
	@Username NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @UserId BIGINT = 
	(
		SELECT Id 
		FROM [identity].[users]
		WHERE Username LIKE @Username
		AND Deleted = 0
	);

	IF @UserId IS NULL OR @UserId = 0 
	BEGIN
		SET @UserId = (
			SELECT Id 
			FROM [identity].[users]
			WHERE Username LIKE @Username + '%'
			AND Deleted = 0
		);
	END

	EXEC [identity].[usp_user_get_by_id] @UserId = @UserId;
END
