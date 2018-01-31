CREATE PROCEDURE [dbo].[Board_GetLists]
	@boardId int
AS
	SELECT * FROM Boards WHERE boardId=@boardId

	SELECT * FROM Lists WHERE boardId=@boardId ORDER BY sort

	SELECT * FROM Cards WHERE listId IN (SELECT listId FROM Lists WHERE boardId=@boardId) ORDER BY listId, sort ASC