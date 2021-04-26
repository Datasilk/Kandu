CREATE PROCEDURE [dbo].[Organization_Create]
	@ownerId int,
	@name nvarchar(64),
	@website nvarchar(255) = '',
	@description nvarchar(MAX) = '',
	@isprivate bit = 1
AS
	DECLARE @orgId int = NEXT VALUE FOR SequenceOrganizations
	INSERT INTO Organizations(orgId, ownerId, [name], datecreated, website, [description], [isprivate])
	VALUES (@orgId, @ownerId, @name, GETUTCDATE(), @website, @description, @isprivate)
	SELECT @orgId

	INSERT INTO OrgSecurity (orgId, userId, [key], [enabled]) 
	VALUES (@orgId, @ownerId, 'owner', 1)