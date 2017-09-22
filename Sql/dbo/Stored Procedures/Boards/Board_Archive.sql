CREATE PROCEDURE [dbo].[Board_Archive]
	@boardId int
AS
	UPDATE Boards SET archived=1 WHERE boardId=@boardId