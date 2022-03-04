CREATE TABLE [dbo].[Log_Cards]
(
	[logId] INT IDENTITY(1,1) PRIMARY KEY,
	[datecreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [userId] INT NOT NULL DEFAULT 0, 
    [boardId] INT NOT NULL,
    [listId] INT NOT NULL,
    [cardId] INT NOT NULL,
	[action] INT NOT NULL, -- 0 = create, 1 = change sort, 2 = change name, 3 = change card type, 4 = archived, 5 = deleted,
                           -- 6 = change list, 7 = change board, 8 = assign card, 9 = set due date, 10 = change layout, 11 = change colors,
                           -- 12 = change description, 13 = invite people, 14 = add checklist item, 15 = update checklist item, 
                           -- 16 = delete checklist item, 17 = add comment, 18 = update comment, 19 = flag comment, 20 = delete comment,
    [scopeId] INT NULL, -- sort, boardId, listId, assigned userId, layout
)

GO

CREATE INDEX [IX_Log_Cards_DateCreated] ON [dbo].[Log_Cards] ([datecreated] DESC)
