CREATE PROCEDURE [dbo].[Card_Comment_Flag]
	@cardId int,
	@commentId int
AS
	UPDATE CardComments SET flagged += 1 WHERE commentId=@commentId AND cardId=@cardId
	
