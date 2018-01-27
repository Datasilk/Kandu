CREATE TABLE [dbo].[CardChecklists]
(
	[checklistId] INT NOT NULL,
	[cardId] INT NOT NULL, 
    [name] NVARCHAR(128) NOT NULL DEFAULT ''
)
