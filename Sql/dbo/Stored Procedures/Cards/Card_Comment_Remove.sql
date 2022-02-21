CREATE PROCEDURE [dbo].[Card_Comment_Remove]
	@commentId int,
	@cardId int,
	@userId int
AS
	DELETE FROM CardComments WHERE commentId=@commentId AND cardId=@cardId
