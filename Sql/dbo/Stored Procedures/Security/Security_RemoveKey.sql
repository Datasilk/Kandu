CREATE PROCEDURE [dbo].[Security_RemoveKey]
	@orgId int,
	@groupId int,
	@key varchar(32),
	@scope int,
	@scopeId int
AS
	DELETE FROM [Security] WHERE orgId=@orgId AND groupId=@groupId AND [key]=@key AND scope=@scope AND scopeId=@scopeId

