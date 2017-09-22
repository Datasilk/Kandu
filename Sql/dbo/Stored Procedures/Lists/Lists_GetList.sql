CREATE PROCEDURE [dbo].[Lists_GetList]
	@boardId int
AS
	SELECT * FROM Lists 
	WHERE boardId=@boardId 
	ORDER BY sort ASC