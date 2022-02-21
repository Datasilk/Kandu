CREATE TABLE [dbo].[CardCommentsFlagged]
(
	[commentId] INT NOT NULL,
	[userId] INT NOT NULL,
	[datecreated] datetime2(7) NOT NULL DEFAULT GETUTCDATE(), 
    PRIMARY KEY ([commentId], [userId])
)
