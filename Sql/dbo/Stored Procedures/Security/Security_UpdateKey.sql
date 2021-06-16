CREATE PROCEDURE [dbo].[Security_UpdateKey]
	@orgId int,
	@groupId int,
	@key varchar(32),
	@value bit
AS
	IF EXISTS(SELECT * FROM Security WHERE orgId=@orgId AND groupId=@groupId AND [key]=@key) BEGIN
		UPDATE Security SET [enabled] = @value WHERE orgId=orgId AND groupId=@groupId AND [key]=@key
	END ELSE BEGIN
		INSERT INTO Security (orgId, groupId, [key], [enabled]) 
		VALUES (@orgId, @groupId, @key, @value)
	END