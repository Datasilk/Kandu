CREATE PROCEDURE [dbo].[Card_Comment_Flag]
	@cardId int,
	@commentId int,
	@userId int
AS
IF NOT EXISTS(SELECT * FROM CardCommentsFlagged WHERE commentId=@commentId AND userId=@userId) BEGIN
	UPDATE CardComments SET flagged += 1 WHERE commentId=@commentId AND cardId=@cardId
	INSERT INTO CardCommentsFlagged (commentId, userId) VALUES (@commentId, @userId)
END
	
	
