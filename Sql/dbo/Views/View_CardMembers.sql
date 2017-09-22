CREATE VIEW [dbo].[View_CardMembers]
AS 
SELECT c.cardId, u.userId, u.[name], u.[email], u.photo
FROM CardMembers c
LEFT JOIN Users u ON u.userId=c.userId
