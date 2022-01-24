CREATE TABLE [dbo].[Security]
(
	[orgId] INT NOT NULL, 
    [groupId] INT NOT NULL,
	[key] VARCHAR(32) NOT NULL,
	[scope] INT NOT NULL DEFAULT 0, -- 0 = all, 1 = organization, 2 = security group, 3 = team, 4 = board, 5 = list, 6 = card
    [scopeId] INT NOT NULL DEFAULT 0, -- ID of entity based on scope > 0 (orgId, groupId, teamId, boardId, listId, cardId)
	[enabled] BIT NOT NULL DEFAULT 0
    PRIMARY KEY ([orgId], [groupId], [key], [scope], [scopeId])
)
