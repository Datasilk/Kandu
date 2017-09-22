CREATE TABLE [dbo].[LogBoard]
(
	[boardId] INT NOT NULL, 
    [cardId] INT NOT NULL DEFAULT 0, 
	[userId] INT NOT NULL DEFAULT 0,
    [action] INT NOT NULL DEFAULT 0, 
    [fromId] INT NULL, 
    [toId] INT NULL, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)
