CREATE TABLE [dbo].[CardChecklistItems]
(
	[itemId] INT NOT NULL, 
    [checklistId] INT NOT NULL, 
    [cardId] INT NOT NULL, 
    [sort] INT NOT NULL DEFAULT 999, 
    [description] NVARCHAR(255) NOT NULL DEFAULT ''
)
