CREATE PROCEDURE [identity].[usp_identities_get_for_user]
	@UserId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[Id],
		[Name],
		[UserId],
		[Default]
	FROM [identity].[identities]
	WHERE UserId = @UserId
	AND Deleted = 0;
END
