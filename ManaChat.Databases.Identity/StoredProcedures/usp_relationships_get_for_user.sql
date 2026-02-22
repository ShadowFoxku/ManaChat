CREATE PROCEDURE [identity].[usp_relationships_get_for_user]
	@UserId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		r.Id,
		r.UserId,
		r.RecipientUserId,
		r.Relationship
	FROM [identity].[relationships] r
	WHERE r.UserId = @UserId
END