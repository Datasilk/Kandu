CREATE TABLE [dbo].[CardComments]
(
	[commentId] INT NOT NULL PRIMARY KEY, 
    [cardId] INT NOT NULL, 
    [userId] INT NOT NULL, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [comment] NVARCHAR(MAX) NOT NULL
)
