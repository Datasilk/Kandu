CREATE PROCEDURE [dbo].[Boards_GetList]
	@userId int
AS
	SELECT * FROM View_Boards b
	WHERE b.boardId IN (
		SELECT boardId 
		FROM BoardMembers bm
		WHERE bm.userId=@userId)
	OR b.teamId IN (
		SELECT teamId 
		FROM TeamMembers tm
		WHERE tm.userId=@userId)
	ORDER BY favorite DESC, lastmodified DESC