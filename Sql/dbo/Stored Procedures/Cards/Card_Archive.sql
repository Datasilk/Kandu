CREATE PROCEDURE [dbo].[Card_Archive]
	@boardId int,
	@cardId int
AS
	UPDATE Cards SET archived=1 WHERE cardId=@cardId AND boardId=@boardId
	EXEC Board_Modified @boardId=@boardId
