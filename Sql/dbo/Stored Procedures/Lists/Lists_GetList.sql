CREATE PROCEDURE [dbo].[Lists_GetList]
	@boardId int,
	@userId int
AS
	/* check for security */
	IF (SELECT COUNT(*) FROM BoardMembers WHERE boardId=@boardId AND userId=@userId) > 0 BEGIN
		SELECT * FROM Lists WHERE boardId=@boardId ORDER BY sort ASC
	END