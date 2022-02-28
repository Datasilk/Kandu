CREATE PROCEDURE [dbo].[SecurityGroups_GetListForUser]
	@orgId int,
	@spUserId int, -- user who is executing stored proc
	@userId int
AS
	SELECT DISTINCT sg.*, o.[name] AS orgName, o.ownerId
	FROM [Security] s 
	JOIN SecurityGroups sg ON sg.groupId=s.groupId
	JOIN SecurityUsers su ON su.groupId = sg.groupId
	JOIN Organizations o ON o.orgId=sg.orgId
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
	AND sg.personal = 0
	
	