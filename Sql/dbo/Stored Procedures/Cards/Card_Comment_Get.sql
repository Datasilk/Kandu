CREATE PROCEDURE [dbo].[Card_Comment_Get]
	@cardId int,
	@commentId int
AS
	SELECT * FROM CardComments WHERE commentId=@commentId AND cardId=@cardId
