CREATE PROCEDURE [dbo].[Board_GetLists]
	@boardId int
AS
	SELECT * FROM Boards WHERE boardId=@boardId

	/* [2] Lists */
	SELECT * FROM Lists WHERE boardId=@boardId AND archived=0 ORDER BY sort ASC

	/* [3] Cards */
	EXEC Cards_GetList @boardId=@boardId, @archivedOnly=0