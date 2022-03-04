CREATE TABLE [dbo].[Log_Lists]
(
	[logId] INT IDENTITY(1,1) PRIMARY KEY,
	[datecreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [userId] INT NOT NULL DEFAULT 0, 
    [boardId] INT NOT NULL,
    [listId] INT NOT NULL,
	[action] INT NOT NULL, -- 0 = create, 1 = change sort, 2 = change name, 3 = change card type, 4 = archived, 5 = deleted, 6 = change board
	[scopeId] INT NULL -- sort, boardId
)

GO

CREATE INDEX [IX_Log_Lists_DateCreated] ON [dbo].[Log_Lists] ([datecreated] DESC)
