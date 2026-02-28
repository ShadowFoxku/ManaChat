CREATE PROCEDURE [identity].[usp_identity_delete]
	@Id BIGINT,
	@ReplacementName CHAR(100)
AS
BEGIN
	UPDATE [identity].[identities]
	SET 
		Deleted = 1,
		[Name] = @ReplacementName
	WHERE Id = @Id;
END
