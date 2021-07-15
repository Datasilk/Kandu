CREATE PROCEDURE [dbo].[Teams_GetAllForUser]
	@spUserId int,
	@userId int
AS
	SET NOCOUNT ON;

	--check security for user who is executing this SP
	SELECT su.groupId 
	INTO #sec
	FROM [SecurityUsers] su
	JOIN SecurityGroups sg ON sg.groupId = su.groupId
	JOIN Security s ON s.groupId = sg.groupId
	WHERE su.userId = @spUserId
	AND (
		s.[key] = 'Owner'
		OR s.[key] = 'TeamsCanViewAll'
	)


	SELECT DISTINCT t.*, o.[name] AS orgName, sg.[name] AS groupName,
	(SELECT COUNT(*) FROM TeamMembers tm2 WHERE tm2.teamId=t.teamId) AS totalMembers
	FROM Teams t
	LEFT JOIN TeamMembers tm ON tm.teamId=t.teamId AND tm.userId=@userId
	LEFT JOIN Organizations o ON o.orgId=t.orgId
	LEFT JOIN SecurityGroups sg ON sg.groupId = t.groupId
	WHERE sg.groupId IN (SELECT groupId FROM #sec)
	