CREATE PROCEDURE [dbo].[User_Create]
	@name nvarchar(64),
	@email nvarchar(64),
	@password nvarchar(255),
	@photo bit = 0
AS
	DECLARE @userId int = NEXT VALUE FOR SequenceUsers
	INSERT INTO Users (userId, [name], email, [password], photo, datecreated)
	VALUES (@userId, @name, @email, @password, @photo, GETDATE())
	
	-- create organization for user (which also creates admin security group for user/owner in org)
	DECLARE @tmp TABLE (id int)
	INSERT INTO @tmp EXEC Organization_Create @ownerId=@userId, @name='My Organization', @website='', @description='Personal Organization', @isprivate=1
	DECLARE @orgId int
	SELECT @orgId = id FROM @tmp

	-- create a team for the user
	EXEC Team_Create @orgId=@orgId, @ownerId=@userId, @name='My Team', @description='Personal Team'
	DECLARE @teamId int
	SELECT @teamId FROM Teams WHERE orgId=@orgId AND ownerId=@userId