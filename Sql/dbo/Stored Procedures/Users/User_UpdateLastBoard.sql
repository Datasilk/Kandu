CREATE PROCEDURE [dbo].[User_UpdateLastBoard]
	@userId int = 0,
	@boardId int
AS
	UPDATE Users SET lastboard=@boardId WHERE userId=@userId
