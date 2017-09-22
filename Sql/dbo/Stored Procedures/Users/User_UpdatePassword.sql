CREATE PROCEDURE [dbo].[User_UpdatePassword]
	@userId int,
	@password nvarchar(255)
AS
	UPDATE Users SET [password]=@password WHERE userId=@userId