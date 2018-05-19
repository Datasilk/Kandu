CREATE PROCEDURE [dbo].[User_AuthenticateByToken]
	@token nvarchar(25)
AS
	DECLARE @userId int
	SELECT @userId = userId FROM User_AuthTokens WHERE token=@token AND expires > GETDATE()
	DELETE FROM User_AuthTokens WHERE token=@token
	IF @userId IS NOT NULL BEGIN
		SELECT * FROM Users WHERE userId = @userId
	END