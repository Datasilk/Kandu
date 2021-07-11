CREATE PROCEDURE [dbo].[User_UpdateName]
	@userId int,
	@name nvarchar(64)
AS
	UPDATE Users SET [name]=@name WHERE userId=@userId
