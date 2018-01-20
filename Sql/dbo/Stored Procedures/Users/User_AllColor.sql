CREATE PROCEDURE [dbo].[User_AllColor]
	@userId int,
	@allcolor bit
AS
	UPDATE Users SET allcolor=@allcolor WHERE userId=@userId
