CREATE PROCEDURE [dbo].[User_KeepMenuOpen]
	@userId int,
	@keepMenu bit = 0
AS
	UPDATE Users SET keepmenu=@keepMenu WHERE userId=@userId
