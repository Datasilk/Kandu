CREATE PROCEDURE [dbo].[Card_Restore]
	@boardId int,
	@cardId int
AS
UPDATE Cards SET archived=0 WHERE cardId=@cardId AND boardId=@boardId
EXEC Board_Modified @boardId=@boardId
