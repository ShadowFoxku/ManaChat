CREATE PROCEDURE [identity].[usp_identity_get]
	@UserId BIGINT,
	@IdentityId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		[Id],
		[Name],
		[UserId],
		[Default]
	FROM [identity].[Identities]
	WHERE UserId = @UserId
	AND Id = @IdentityId
	AND Deleted = 0;
END