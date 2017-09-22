CREATE VIEW [dbo].[View_CardComments]
AS
SELECT c.cardId, c.commentId, c.userId, c.datecreated, 
		c.comment, u.[name], u.email, u.photo
FROM CardComments c
LEFT JOIN Users u ON u.userId=c.userId
