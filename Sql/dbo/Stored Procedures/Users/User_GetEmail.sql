CREATE PROCEDURE [dbo].[User_GetEmail]
	@userId int
AS
	SELECT email FROM Users WHERE userId=@userId