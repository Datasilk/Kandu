CREATE TABLE [dbo].[Cards]
(
	[cardId] INT NOT NULL PRIMARY KEY, 
	[listId] INT NOT NULL,
    [boardId] INT NOT NULL, 
    [sort] INT NOT NULL DEFAULT 999, 
    [layout] INT NOT NULL DEFAULT 0, 
    [archived] BIT NOT NULL DEFAULT 0,  
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [datedue] DATETIME NULL , 
    [name] NVARCHAR(MAX) NOT NULL, 
    [colors] NVARCHAR(128) NOT NULL DEFAULT '',
    [description] NVARCHAR(MAX) NOT NULL DEFAULT ''
)
