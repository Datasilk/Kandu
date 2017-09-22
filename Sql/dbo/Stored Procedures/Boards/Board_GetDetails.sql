CREATE PROCEDURE [dbo].[Board_GetDetails]
	@boardId int
AS
	/* [0] Board Info */
	SELECT * FROM Boards WHERE boardId=@boardId

	/* [1] Board Members */
	SELECT * FROM View_BoardMembers WHERE boardId=@boardId

	/* [2] Lists */
	SELECT * FROM Lists WHERE boardId=@boardId

	/* [3] Cards */
	EXEC Cards_GetList @boardId=@boardId