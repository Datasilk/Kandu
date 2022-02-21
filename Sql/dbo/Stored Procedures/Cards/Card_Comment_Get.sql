CREATE PROCEDURE [dbo].[Card_Comment_Get]
	@cardId int,
	@commentId int,
	@userId int
AS
	SELECT cc.*, CASE WHEN f.userId IS NOT NULL THEN 1 ELSE 0 END AS hasflagged 
	FROM View_CardComments cc
	LEFT JOIN CardCommentsFlagged f ON f.commentId = cc.commentId AND f.userId = @userId
	WHERE cc.commentId=@commentId AND cc.cardId=@cardId
