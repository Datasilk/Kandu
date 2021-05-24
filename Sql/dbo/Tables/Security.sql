CREATE TABLE [dbo].[Security]
(
	[orgId] INT NOT NULL, 
    [groupId] INT NOT NULL,
	[key] VARCHAR(32) NOT NULL,
	[enabled] BIT NOT NULL DEFAULT 0,
	PRIMARY KEY ([orgId], [groupId], [key])
)
