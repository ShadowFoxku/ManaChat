CREATE PROCEDURE [identity].[usp_identity_delete]
	@Id BIGINT
AS
BEGIN
	UPDATE [identity].[Identities]
	SET Deleted = 1
	WHERE Id = @Id;
END
