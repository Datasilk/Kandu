CREATE PROCEDURE [dbo].[Card_GetDetails]
	@cardId int,
	@userId int
AS
	/* [0] Card Info */
	SELECT b.orgId, c.*, u.[name] AS assignedName, cd.[description], cj.[json], l.[name] AS listName, l.archived AS listArchived, 
	b.[type] AS boardType, b.[name] AS boardName, b.color AS boardColor
	FROM Cards c
	LEFT JOIN CardDescriptions cd ON cd.cardId = c.cardId
	LEFT JOIN CardJson cj ON cj.cardId = c.cardId
	LEFT JOIN Lists l ON l.listId=c.listId
	LEFT JOIN Boards b ON b.boardId=c.boardId
	LEFT JOIN Users u ON u.userId = c.userIdAssigned
	WHERE c.cardId=@cardId

	/* [1] Card Labels */
	SELECT l.labelId, l.[label] 
	FROM CardLabels cl
	JOIN Labels l ON l.labelId = cl.labelId 
	WHERE cl.cardId = @cardId

	/* [2] Card Checklist Items */
	SELECT i.itemId, i.sort, i.ischecked, i.[label]
	FROM CardChecklistItems i
	WHERE i.cardId = @cardId
	ORDER BY i.sort, i.datecreated DESC

	/* [3] Card Attachments */
	SELECT a.attachmentId, a.userId, a.[filename], a.datecreated
	FROM CardAttachments a
	WHERE a.cardId = @cardId
	ORDER BY a.datecreated ASC

	/* [4] Card Comments */
	SELECT cc.*, CASE WHEN f.userId IS NOT NULL THEN 1 ELSE 0 END AS hasflagged 
	FROM View_CardComments cc
	LEFT JOIN CardCommentsFlagged f ON f.commentId = cc.commentId AND f.userId = @userId
	WHERE cc.cardId=@cardId
	ORDER BY cc.datecreated ASC