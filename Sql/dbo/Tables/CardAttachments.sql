CREATE TABLE [dbo].[CardAttachments]
(
	[attachmentId] INT NOT NULL PRIMARY KEY, 
    [cardId] INT NOT NULL, 
    [userId] INT NOT NULL, 
    [filename] NVARCHAR(64) NOT NULL, 
    [datecreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
