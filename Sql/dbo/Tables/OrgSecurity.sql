CREATE TABLE [dbo].[OrgSecurity]
(
	[orgId] INT NOT NULL, 
    [userId] INT NOT NULL,
	[key] VARCHAR(32) NOT NULL,
	[enabled] BIT NOT NULL DEFAULT 0,
	PRIMARY KEY ([orgId], userId, [key])
)
