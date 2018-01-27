CREATE PROCEDURE [dbo].[Card_GetDetails]
	@boardId int,
	@cardId int
AS
	/* [0] Card Info */
	SELECT * FROM Cards WHERE cardId=@cardId AND boardId=@boardId

	/* [1] Card Members */
	SELECT * FROM View_CardMembers
	WHERE cardId=@cardId
	ORDER BY [name] ASC

	/* [2] Card Comments */
	SELECT * FROM View_CardComments
	WHERE cardId=@cardId
	ORDER BY datecreated DESC