CREATE TABLE [dbo].[BoardMembers]
(
    [boardId] INT NOT NULL DEFAULT 0,
    [teamId] INT NOT NULL DEFAULT 0,
	[userId] INT NOT NULL DEFAULT 0, 
    [favorite] BIT DEFAULT 0
)
