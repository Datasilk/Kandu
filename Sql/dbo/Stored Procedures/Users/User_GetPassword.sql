CREATE PROCEDURE [dbo].[User_GetPassword]
	@email nvarchar(100)
AS
	SELECT [password] FROM Users WHERE email=@email