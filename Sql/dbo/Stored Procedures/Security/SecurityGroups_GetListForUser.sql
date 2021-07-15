CREATE PROCEDURE [dbo].[SecurityGroups_GetListForUser]
	@orgId int,
	@spUserId int, -- user who is executing stored proc
	@userId int
AS
	-- first, get security keys for user related to security groups & admin privilages
	SELECT s.groupId, s.orgId
	INTO #sec 
	FROM [Security] s 
	JOIN SecurityGroups sg ON sg.groupId=s.groupId
	JOIN SecurityUsers su ON su.groupId = sg.groupId
	WHERE su.userId=@spUserId 
	AND (
		(@orgId IS NOT NULL AND @orgId > 0 AND sg.orgId=@orgId)
		OR @orgId IS NULL OR @orgId = 0
	)
	AND (s.scope IS NULL OR s.scope <= 2) --0 = all, 1 = organization, 2 = security group
	AND (
		[key]='Owner' 
		OR ([key] = 'SecGroupsCanViewAll' AND scope IS NULL)
		OR (
			s.scope = 2 
			AND s.scopeId = sg.groupId 
			AND s.[key] IN ('SecGroupCanView')
		)
	)

	-- next, check if user has permission to view all security groups

	-- finally, get security groups that the user has access to
	SELECT DISTINCT sg.*, o.[name] AS orgName, o.ownerId --, (SELECT COUNT(*) FROM [Security] WHERE groupId=sg.groupId) AS totalkeys
	FROM SecurityUsers su
	JOIN #sec s ON s.groupId = su.groupId
	JOIN SecurityGroups sg ON sg.groupId = s.groupId
	JOIN Organizations o ON o.orgId=sg.orgId
	WHERE su.userId = @userId
	AND (
		(@orgId IS NOT NULL AND @orgId > 0 AND sg.orgId=@orgId)
		OR @orgId IS NULL OR @orgId = 0
	)
	