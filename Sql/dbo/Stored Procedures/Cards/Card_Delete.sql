CREATE PROCEDURE [dbo].[Card_Delete]
	@boardId int,
	@cardId int
AS
	DELETE FROM CardChecklistItems WHERE cardId=@cardId
	DELETE FROM CardChecklists WHERE cardId=@cardId
	DELETE FROM CardComments WHERE cardId=@cardId
	DELETE FROM CardDescriptions WHERE cardId=@cardId
	DELETE FROM CardJson WHERE cardId=@cardId
	DELETE FROM CardMembers WHERE cardId=@cardId
	DELETE FROM Cards WHERE cardId=@cardId AND boardId=@boardId
	EXEC Board_Modified @boardId=@boardId
