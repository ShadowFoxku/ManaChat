CREATE PROCEDURE [identity].[usp_relationship_get_between_users]
	@UserId BIGINT,
	@RecipientUserId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		r.Id,
		r.UserId,
		r.RecipientUserId,
		r.Relationship
	FROM [identity].[Relationships] r
	WHERE r.UserId = @UserId
	AND r.RecipientUserId = @RecipientUserId;
END
