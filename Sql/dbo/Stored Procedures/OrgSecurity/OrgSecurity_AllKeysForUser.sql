CREATE PROCEDURE [dbo].[OrgSecurity_AllKeysForUser]
	@userId int
AS
	SELECT orgId, [key], [enabled] FROM OrgSecurity
	WHERE userId=@userId
	ORDER BY orgId
