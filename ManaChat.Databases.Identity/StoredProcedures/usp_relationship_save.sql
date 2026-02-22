CREATE PROCEDURE [dbo].[usp_relationship_save]
	@Id BIGINT,
	@UserId BIGINT,
	@RecipientUserId BIGINT,
	@Relationship INT,
	@Bookmarked BIT
AS
BEGIN
	SET NOCOUNT ON;
	
	IF @Id = 0
	BEGIN
		INSERT INTO [identity].[relationships] (UserId, RecipientUserId, Relationship, Bookmarked)
		VALUES (@UserId, @RecipientUserId, @Relationship, @Bookmarked);
		SET @Id = SCOPE_IDENTITY();
	END 
	ELSE
	BEGIN
		UPDATE [identity].[relationships]
		SET 
			Relationship = @Relationship,
			Bookmarked = @Bookmarked
		WHERE Id = @Id
	END

	SELECT @Id as Id;
END