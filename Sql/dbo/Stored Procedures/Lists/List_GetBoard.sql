CREATE PROCEDURE [dbo].[List_GetBoard]
	@listId int
AS
	SELECT l.*, b.[name] AS boardName
	FROM Lists l
	JOIN Boards b ON b.boardId = l.boardId
	WHERE l.listId=@listId
