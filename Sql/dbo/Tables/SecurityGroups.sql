CREATE TABLE [dbo].[SecurityGroups]
(
	[groupId] INT NOT NULL, 
    [orgId] INT NOT NULL, 
    [name] NVARCHAR(32) NOT NULL
    PRIMARY KEY ([groupId])
)
