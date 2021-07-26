CREATE TABLE [dbo].[EmailActions]
(
    [action] VARCHAR(32) NOT NULL PRIMARY KEY,
	[clientId] INT NOT NULL, 
    [subject] NVARCHAR(255) NOT NULL DEFAULT '',
    [fromName] NVARCHAR(64) NOT NULL, 
    [fromAddress] NVARCHAR(64) NOT NULL
)
