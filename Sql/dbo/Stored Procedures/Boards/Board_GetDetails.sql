CREATE PROCEDURE [dbo].[Board_GetDetails]
	@boardId int
AS
	/* [0] Board Info */
	SELECT * FROM View_Boards WHERE boardId=@boardId

	/* [1] Board Members */
	SELECT * FROM View_BoardMembers WHERE boardId=@boardId

	/* [2] Lists */
	SELECT * FROM Lists WHERE boardId=@boardId AND archived=0 ORDER BY sort ASC

	/* [3] Cards */
	EXEC Cards_GetList @boardId=@boardId, @archivedOnly=0