CREATE TABLE [dbo].[SecurityGroups]
(
	[groupId] INT NOT NULL, 
    [orgId] INT NOT NULL, 
    [personal] BIT NOT NULL DEFAULT 0,
    [name] NVARCHAR(32) NOT NULL
    PRIMARY KEY ([groupId])
)
