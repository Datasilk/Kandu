CREATE PROCEDURE [dbo].[Team_Create]
	@orgId int,
	@ownerId int,
	@name nvarchar(64),
	@description nvarchar(MAX) = ''
AS
	DECLARE @teamId int = NEXT VALUE FOR SequenceTeams
	INSERT INTO Teams (teamId, orgId, [name], datecreated, [description])
	VALUES (@teamId, @orgId, @name, GETDATE(), @description)

	INSERT INTO TeamMembers (teamId, userId) VALUES (@teamId, @ownerId)

	SELECT @teamId