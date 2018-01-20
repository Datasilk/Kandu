CREATE PROCEDURE [dbo].[User_GetInfo]
	@userId int
AS
	SELECT * FROM Users WHERE userId=@userId
