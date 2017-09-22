CREATE PROCEDURE [dbo].[User_UpdateEmail]
	@userId int,
	@email nvarchar(64),
	@password nvarchar(255)
AS
	UPDATE Users 
	SET email=@email, [password]=@password 
	WHERE userId=@userId