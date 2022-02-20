CREATE TABLE [dbo].[Cards]
(
	[cardId] INT NOT NULL PRIMARY KEY, 
	[listId] INT NOT NULL,
    [boardId] INT NOT NULL, 
    [userIdAssigned] INT NOT NULL DEFAULT 0, 
    [userIdCreated] INT NOT NULL DEFAULT 1,
    [sort] INT NOT NULL DEFAULT 999, 
    [layout] INT NOT NULL DEFAULT 0, 
    [archived] BIT NOT NULL DEFAULT 0,  
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [datemodified] DATETIME NOT NULL DEFAULT GETDATE(), 
    [datedue] DATETIME NULL , 
    [name] NVARCHAR(MAX) NOT NULL, 
    [type] VARCHAR(16) NOT NULL DEFAULT '', --used for plugins
    [colors] VARCHAR(128) NOT NULL DEFAULT '',
    [description] NVARCHAR(MAX) NOT NULL DEFAULT '', -- delete later
)

GO

CREATE INDEX [IX_Cards_Boards] ON [dbo].[Cards] (boardId, listId)

GO


CREATE INDEX [IX_Cards_BoardModified] ON [dbo].[Cards] (boardId, datemodified DESC)

GO

CREATE INDEX [IX_Cards_UserModified] ON [dbo].[Cards] ([userIdAssigned], datemodified DESC)
