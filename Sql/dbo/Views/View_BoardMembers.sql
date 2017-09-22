CREATE VIEW [dbo].[View_BoardMembers]
AS 
SELECT b.boardId, u.userId, u.[name], u.[email], u.photo
FROM BoardMembers b
LEFT JOIN Users u ON u.userId=b.userId
