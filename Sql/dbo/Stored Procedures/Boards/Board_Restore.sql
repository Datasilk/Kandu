CREATE PROCEDURE [dbo].[Board_Restore]
	@boardId int
AS
UPDATE Boards SET archived=0 WHERE boardId=@boardId