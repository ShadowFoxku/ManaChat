CREATE TABLE [identity].[relationships]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserId] BIGINT NOT NULL REFERENCES [identity].[users](Id) ON DELETE CASCADE,
	[RecipientUserId] BIGINT NOT NULL REFERENCES [identity].[users](Id) ON DELETE CASCADE,
	[Relationship] INT NOT NULL,
	[Bookmarked] BIT NOT NULL
)
GO

CREATE INDEX IX_relationships_UserId_RecipientUserId ON [identity].[relationships](UserId, RecipientUserId);
GO

ALTER TABLE [identity].[relationships] ADD CONSTRAINT UQ_relationships_UserId_RecipientUserId UNIQUE (UserId, RecipientUserId);