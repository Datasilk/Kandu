CREATE TABLE [dbo].[Teams]
(
	[teamId] INT NOT NULL PRIMARY KEY, 
    [ownerId] INT NOT NULL DEFAULT 0, 
    [security] BIT NOT NULL DEFAULT 0, 
    [name] NVARCHAR(64) NOT NULL DEFAULT '', 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [website] NVARCHAR(255) NOT NULL DEFAULT '', 
    [description] NVARCHAR(MAX) NOT NULL DEFAULT ''
)
