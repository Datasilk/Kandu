CREATE TABLE [dbo].[Organizations]
(
	[orgId] INT NOT NULL PRIMARY KEY,
	[ownerId] INT NOT NULL,
	[name] NVARCHAR(64) NOT NULL,
	[datecreated] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [website] NVARCHAR(255) NOT NULL DEFAULT '', 
    [description] NVARCHAR(MAX) NOT NULL DEFAULT '',
    [banner] BIT NOT NULL DEFAULT 0, 
    [photo] BIT NOT NULL DEFAULT 0,
    [enabled] BIT NOT NULL DEFAULT 1, 
    [isprivate] BIT NOT NULL DEFAULT 1
)
