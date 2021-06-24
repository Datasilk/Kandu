CREATE PROCEDURE [dbo].[Security_AllKeysForUser]
	@userId int
AS
	-- include "User" key on for all organizations the user is a member of
	SELECT DISTINCT sg.orgId, 'User' AS [key], 1 AS [enabled], 0 AS groupId, 0 AS scope, 0 AS scopeId
	FROM SecurityUsers su
	JOIN SecurityGroups sg ON sg.groupId = su.groupId
	WHERE su.userId = @userId
	UNION
	-- get all security keys for all organizations that the user belongs to
	SELECT DISTINCT s.orgId, s.[key], s.[enabled], s.groupId, s.scope, s.scopeId
	FROM SecurityUsers su
	JOIN SecurityGroups sg ON sg.groupId = su.groupId
	JOIN [Security] s ON s.groupId = sg.groupId
	WHERE su.userId = @userId
	ORDER BY orgId, groupId

