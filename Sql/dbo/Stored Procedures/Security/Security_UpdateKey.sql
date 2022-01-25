CREATE PROCEDURE [dbo].[Security_UpdateKey]
	@orgId int,
	@groupId int,
	@key varchar(32),
	@value bit,
	@scope int,
	@scopeId int
AS
	IF EXISTS(SELECT * FROM Security WHERE orgId=@orgId AND groupId=@groupId AND [key]=@key AND scope=@scope AND scopeId=@scopeId) BEGIN
		UPDATE Security SET [enabled] = @value, scope=@scope, scopeId=@scopeId 
		WHERE orgId=orgId AND groupId=@groupId AND [key]=@key
	END ELSE BEGIN
		INSERT INTO Security (orgId, groupId, [key], [enabled], scope, scopeId) 
		VALUES (@orgId, @groupId, @key, @value, @scope, @scopeId)
	END