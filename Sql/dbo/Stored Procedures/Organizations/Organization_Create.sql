CREATE PROCEDURE [dbo].[Organization_Create]
	@ownerId int,
	@name nvarchar(64),
	@website nvarchar(255) = '',
	@description nvarchar(MAX) = '',
	@isprivate bit = 1,
	@cardtype varchar(16) = ''
AS
	DECLARE @orgId int = NEXT VALUE FOR SequenceOrganizations
	INSERT INTO Organizations(orgId, ownerId, [name], datecreated, website, [description], [isprivate], cardtype)
	VALUES (@orgId, @ownerId, @name, GETUTCDATE(), @website, @description, @isprivate, @cardtype)
	
	DECLARE @groupId int = NEXT VALUE FOR SequenceSecurityGroups
	INSERT INTO SecurityGroups (groupId, orgId, [name]) VALUES (@groupId, @orgId, 'Administrators')

	INSERT INTO SecurityUsers (groupId, userId) VALUES (@groupId, @ownerId)

	INSERT INTO [Security] (orgId, groupId, [key], [enabled]) 
	VALUES (@orgId, @groupId, 'Owner', 1)
	
	SELECT @orgId