CREATE PROCEDURE [dbo].[Board_GetInfo]
	@boardId int
AS
	SELECT * FROM Boards WHERE boardId=@boardId
