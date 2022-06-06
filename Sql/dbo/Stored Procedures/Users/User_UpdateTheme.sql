CREATE PROCEDURE [dbo].[User_UpdateTheme]
	@userId int,
	@theme nvarchar(32)
AS
	UPDATE Users SET theme=@theme WHERE userId=@userId
