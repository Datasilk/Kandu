CREATE PROCEDURE [dbo].[List_Archive]
	@listId int
AS
	UPDATE Lists SET archived=1 WHERE listId=@listId
	DECLARE @boardId int
	SELECT @boardId = boardId FROM Lists WHERE listId=@listId
	EXEC Board_Modified @boardId=@boardId
