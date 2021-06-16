CREATE PROCEDURE [dbo].[Security_CreateGroup]
	@orgId int,
	@name nvarchar(32)
AS
	
	DECLARE @groupId int = NEXT VALUE FOR SequenceSecurityGroups
	INSERT INTO SecurityGroups (groupId, orgId, [name]) VALUES (@groupId, @orgId, @name)
