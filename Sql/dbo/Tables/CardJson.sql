CREATE TABLE [dbo].[CardJson]
(
	[cardId] INT NOT NULL PRIMARY KEY, 
    [json] NVARCHAR(MAX) NOT NULL DEFAULT '' -- C# object used by plugins
)
