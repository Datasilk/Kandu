CREATE PROCEDURE [dbo].[User_GetPassword]
	@userId int
AS
	SELECT [password] FROM Users WHERE userId=@userId