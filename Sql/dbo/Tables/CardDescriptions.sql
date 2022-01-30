CREATE TABLE [dbo].[CardDescriptions]
(
	[cardId] INT NOT NULL PRIMARY KEY,
    [description] NVARCHAR(MAX) NOT NULL DEFAULT '',
)