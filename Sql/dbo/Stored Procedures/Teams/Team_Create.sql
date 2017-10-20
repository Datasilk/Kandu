CREATE PROCEDURE [dbo].[Team_Create]
	@ownerId int,
	@security bit, 
	@name nvarchar(64),
	@website nvarchar(255) = '',
	@description nvarchar(MAX) = ''
AS
	DECLARE @id int = NEXT VALUE FOR SequenceTeams
	INSERT INTO Teams (teamId, ownerId, [security], [name], datecreated, website, [description])
	VALUES (@id, @ownerId, @security, @name, GETDATE(), @website, @description)

	INSERT INTO TeamMembers (userId, teamId) VALUES (@ownerId, @id)

	SELECT @id