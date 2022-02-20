CREATE PROCEDURE [dbo].[Card_GetDetails]
	@boardId int,
	@cardId int
AS
	/* [0] Card Info */
	SELECT b.orgId, c.*, u.[name] AS assignedName, cd.[description], cj.[json], l.[name] AS listName, l.archived AS listArchived, 
	b.[type] AS boardType, b.[name] AS boardName, b.color AS boardColor
	FROM Cards c
	LEFT JOIN CardDescriptions cd ON cd.cardId = c.cardId
	LEFT JOIN CardJson cj ON cj.cardId = c.cardId
	LEFT JOIN Lists l ON l.listId=c.listId
	LEFT JOIN Boards b ON b.boardId=@boardId
	LEFT JOIN Users u ON u.userId = c.userIdAssigned
	WHERE c.cardId=@cardId AND c.boardId=@boardId

	/* [1] Card Labels */
	SELECT l.labelId, l.[label] 
	FROM CardLabels cl
	JOIN Labels l ON l.labelId = cl.labelId 
	WHERE cl.cardId = @cardId

	/* [1] Card Checklist Items */
	SELECT i.itemId, i.sort, i.checked AS ischecked, i.[label]
	FROM CardChecklistItems i
	WHERE i.cardId = @cardId
	ORDER BY i.sort, i.datecreated DESC

	/* [2] Card Comments */
	SELECT * FROM View_CardComments
	WHERE cardId=@cardId
	ORDER BY datecreated DESC