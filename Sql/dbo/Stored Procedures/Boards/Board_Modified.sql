CREATE PROCEDURE [dbo].[Board_Modified]
	@boardId int
AS
	UPDATE Boards SET lastmodified = GETDATE() WHERE boardId=@boardId