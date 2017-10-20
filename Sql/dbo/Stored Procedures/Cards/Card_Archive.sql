CREATE PROCEDURE [dbo].[Card_Archive]
	@cardId int
AS
DECLARE @boardId int
SELECT @boardId=boardId FROM cards WHERE cardId=@cardId
EXEC Board_Modified @boardId=@boardId
UPDATE Cards SET archived=1 WHERE cardId=@cardId
