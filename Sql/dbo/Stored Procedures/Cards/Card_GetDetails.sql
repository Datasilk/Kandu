CREATE PROCEDURE [dbo].[Card_GetDetails]
	@boardId int,
	@cardId int
AS
	/* [0] Card Info */
	SELECT c.*, l.[name] AS listName, l.archived AS listArchived, 
	b.[type] AS boardType, b.[name] AS boardName, b.color AS boardColor
	FROM Cards c
	LEFT JOIN Lists l ON l.listId=c.listId
	LEFT JOIN Boards b ON b.boardId=@boardId
	WHERE c.cardId=@cardId AND c.boardId=@boardId

	/* [1] Card Members */
	SELECT * FROM View_CardMembers
	WHERE cardId=@cardId
	ORDER BY [name] ASC

	/* [2] Card Comments */
	SELECT * FROM View_CardComments
	WHERE cardId=@cardId
	ORDER BY datecreated DESC