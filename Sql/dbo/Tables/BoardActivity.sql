CREATE TABLE [dbo].[BoardActivity]
(
	[boardId] INT NOT NULL, 
    [userId] INT NOT NULL, 
    [action] INT NOT NULL, 
    [listId] INT NULL, 
    [cardId] INT NULL, 
    [checklistId] INT NULL, 
    [checklistItemId] INT NULL, 
    [oldId] INT NULL, 
    [oldsort] INT NULL, 
    [newId] INT NULL, 
    [newsort] INT NULL
)
