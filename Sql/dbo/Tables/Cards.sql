CREATE TABLE [dbo].[Cards]
(
	[cardId] INT NOT NULL PRIMARY KEY, 
	[listId] INT NOT NULL,
    [boardId] INT NOT NULL, 
    [sort] INT NOT NULL DEFAULT 999, 
    [color] INT NOT NULL DEFAULT 0, 
    [archived] BIT NOT NULL DEFAULT 0, 
    [name] NVARCHAR(64) NOT NULL, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [datedue] DATETIME NULL , 
    [description] NVARCHAR(MAX) NOT NULL DEFAULT ''
)
