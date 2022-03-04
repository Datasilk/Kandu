CREATE TABLE [dbo].[Log_Boards]
(
	[logId] INT IDENTITY(1,1) PRIMARY KEY,
	[datecreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [userId] INT NOT NULL DEFAULT 0, 
    [boardId] INT NOT NULL,
	[action] INT NOT NULL, -- 0 = create, 1 = change color, 2 = change name, 3 = change card type, 4 = change type,
						   -- 5 = archived, 6 = deleted
	[scopeId] INT NULL -- type
)

GO

CREATE INDEX [IX_Log_Boards_DateCreated] ON [dbo].[Log_Boards] ([datecreated] DESC)
