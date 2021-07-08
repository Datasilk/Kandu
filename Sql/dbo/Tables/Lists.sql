CREATE TABLE [dbo].[Lists]
(
	[listId] INT NOT NULL PRIMARY KEY, 
    [boardId] INT NOT NULL, 
    [name] NVARCHAR(64) NOT NULL, 
    [sort] INT NOT NULL DEFAULT 99, 
    [archived] BIT NOT NULL DEFAULT 0, 
    [cardtype] VARCHAR(16) NOT NULL DEFAULT '' --default card type to use when creating a new card
)
