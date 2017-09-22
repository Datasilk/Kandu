CREATE PROCEDURE [dbo].[User_Create]
	@name nvarchar(64),
	@email nvarchar(64),
	@password nvarchar(255),
	@photo bit = 0
AS
	DECLARE @id int = NEXT VALUE FOR SequenceUsers
	INSERT INTO Users (userId, [name], email, [password], photo, datecreated)
	VALUES (@id, @name, @email, @password, @photo, GETDATE())

	SELECT @id