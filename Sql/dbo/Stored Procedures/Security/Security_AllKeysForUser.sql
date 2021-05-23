CREATE PROCEDURE [dbo].[Security_AllKeysForUser]
	@userId int
AS
	SELECT orgId, [key], [enabled] FROM Security
	WHERE userId=@userId
	ORDER BY orgId
