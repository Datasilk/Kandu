CREATE PROCEDURE [dbo].[Card_GetInfo]
	@cardId int
AS
	SELECT b.orgId, c.*, u.[name] AS assignedName
	FROM Cards c
	JOIN Boards b ON b.boardId = c.boardId
	LEFT JOIN Users u ON u.userId = c.userIdAssigned
	WHERE cardId=@cardId
