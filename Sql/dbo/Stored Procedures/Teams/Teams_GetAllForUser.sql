CREATE PROCEDURE [dbo].[Teams_GetAllForUser]
	@spUserId int,
	@userId int
AS
	SET NOCOUNT ON;

	--check security for user who is executing this SP
	SELECT su.groupId, sg.orgId, s.scope, s.scopeId
	INTO #sec
	FROM [SecurityUsers] su
	JOIN SecurityGroups sg ON sg.groupId = su.groupId
	JOIN Security s ON s.groupId = sg.groupId
	WHERE su.userId = @spUserId
	AND (
		s.[key] = 'Owner'
		OR s.[key] = 'TeamsCanViewAll'
	)




	SELECT DISTINCT t.*, o.[name] AS orgName,
	(SELECT COUNT(*) FROM TeamMembers tm2 WHERE tm2.teamId=t.teamId) AS totalMembers
	FROM Teams t
	LEFT JOIN TeamMembers tm ON tm.teamId=t.teamId AND tm.userId=@userId
	LEFT JOIN Organizations o ON o.orgId=t.orgId
	WHERE t.orgId IN (SELECT orgId FROM #sec WHERE (scopeId IS NULL OR scopeId < 1))
	OR t.teamId IN (SELECT scopeId FROM #sec WHERE scopeId IS NOT NULL)
	