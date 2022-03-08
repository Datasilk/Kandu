CREATE PROCEDURE [dbo].[Card_GetBoard]
	@cardId int
AS
	SELECT c.*, b.[name] AS boardName
	FROM Cards c
	JOIN Boards b ON b.boardId = c.boardId
	WHERE c.cardId = @cardId
