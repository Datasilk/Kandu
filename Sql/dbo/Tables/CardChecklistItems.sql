CREATE TABLE [dbo].[CardChecklistItems]
(
	[itemId] INT NOT NULL, 
    [cardId] INT NOT NULL, 
    [sort] INT NOT NULL DEFAULT 999, 
    [label] NVARCHAR(255) NOT NULL DEFAULT '', 
    [ischecked] BIT NOT NULL DEFAULT 0,
    [datecreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
