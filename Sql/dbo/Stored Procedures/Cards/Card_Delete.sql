CREATE PROCEDURE [dbo].[Card_Delete]
	@boardId int,
	@cardId int
AS
	DELETE FROM Cards WHERE cardId=@cardId AND boardId=@boardId
	EXEC Board_Modified @boardId=@boardId
