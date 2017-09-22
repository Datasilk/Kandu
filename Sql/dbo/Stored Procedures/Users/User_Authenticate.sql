CREATE PROCEDURE [dbo].[User_Authenticate] 
	@email nvarchar(64) = '',
	@password nvarchar(255) = ''
AS
BEGIN
	SELECT * FROM Users WHERE email=@email AND [password]=@password
END