CREATE PROCEDURE [dbo].[Security_CreateGroup]
	@orgId int,
	@name nvarchar(32),
	@personal bit = 0
AS
	
	DECLARE @groupId int = NEXT VALUE FOR SequenceSecurityGroups
	INSERT INTO SecurityGroups (groupId, orgId, personal, [name]) VALUES (@groupId, @orgId, @personal, @name)
	SELECT @groupId