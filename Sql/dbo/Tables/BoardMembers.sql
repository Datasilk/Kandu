CREATE TABLE [dbo].[BoardMembers]
(
	[userId] INT NOT NULL PRIMARY KEY, 
    [boardId] INT NOT NULL,
    [favorite] BIT NOT NULL DEFAULT 0
)
