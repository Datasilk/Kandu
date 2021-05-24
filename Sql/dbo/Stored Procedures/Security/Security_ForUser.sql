CREATE PROCEDURE [dbo].[Security_ForUser]
	@orgId int,
	@userId int
AS
	SELECT s.orgId, s.[key], s.[enabled], s.groupId
	FROM SecurityUsers su
	JOIN SecurityGroups sg ON sg.groupId = su.groupId
	JOIN [Security] s ON s.groupId = sg.groupId
	WHERE su.userId = @userId
	AND sg.orgId=@orgId
	ORDER BY sg.groupId
