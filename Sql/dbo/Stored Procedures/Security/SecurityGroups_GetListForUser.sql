CREATE PROCEDURE [dbo].[SecurityGroups_GetListForUser]
	@orgId int,
	@userId int
AS
	-- first, get security keys for user related to security groups & admin privilages
	DECLARE @isAdmin bit = 0
	SELECT s.[key], s.scope, s.scopeId INTO #sec FROM [Security] s 
	JOIN SecurityGroups sg ON sg.groupId=s.groupId
	JOIN SecurityUsers su ON su.groupId = sg.groupId
	WHERE su.userId=@userId AND sg.orgId=@orgId
	AND (s.scope IS NULL OR s.scope <= 2) --0 = all, 1 = organization, 2 = security group

	-- next, check if user has permission to view all security groups
	SET @isAdmin = CASE WHEN 
		EXISTS(
			SELECT * FROM #sec 
			WHERE [key]='Owner' 
			OR ([key] = 'SecGroupsCanViewAll' AND scope IS NULL)
		) 
		THEN 1 ELSE 0 END
	
	-- finally, get security groups that the user has access to
	SELECT sg.*--, (SELECT COUNT(*) FROM [Security] WHERE groupId=sg.groupId) AS totalkeys
	FROM SecurityGroups sg 
	WHERE orgId=@orgId
	AND (@isAdmin = 1 OR
		EXISTS(
			SELECT * FROM #sec sec 
			WHERE sec.scope = 2 AND sec.scopeId = sg.groupId 
			AND sec.[key] IN ('SecGroupCanView')
		)
	)
	