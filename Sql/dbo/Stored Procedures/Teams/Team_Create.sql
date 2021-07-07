CREATE PROCEDURE [dbo].[Team_Create]
	@orgId int,
	@ownerId int,
	@groupId int = NULL,
	@name nvarchar(64),
	@description nvarchar(MAX) = ''
AS
	DECLARE @teamId int = NEXT VALUE FOR SequenceTeams
	INSERT INTO Teams (teamId, orgId, groupId, [name], datecreated, [description])
	VALUES (@teamId, @orgId, @groupId, @name, GETDATE(), @description)

	INSERT INTO TeamMembers (teamId, userId) VALUES (@teamId, @ownerId)

	SELECT @teamId