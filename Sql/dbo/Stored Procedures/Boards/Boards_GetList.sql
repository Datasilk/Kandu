CREATE PROCEDURE [dbo].[Boards_GetList]
	@userId int,
	@orgId int = 0,
	@sort int
AS
	SELECT DISTINCT sg.orgId INTO #orgs FROM SecurityGroups sg
	JOIN SecurityUsers su ON su.groupId=sg.groupId
	

	SELECT b.*, 
	org.sort, org.[name] AS orgName, org.isprivate, bm.favorite 
	FROM Boards b
	LEFT JOIN BoardMembers bm ON bm.boardId=b.boardId AND bm.userId=@userId
	CROSS APPLY (
		SELECT 
		CASE WHEN o.ownerId=@userId THEN 0 ELSE 1 END AS sort,
		o.orgId, o.[name], o.isprivate
		FROM Organizations o
		WHERE o.orgId = b.orgId
	) AS org
	CROSS APPLY (
		SELECT CASE WHEN EXISTS(
		SELECT * FROM [SecurityUsers] su
		JOIN SecurityGroups sg ON sg.groupId = su.groupId
		JOIN Security s ON s.groupId = sg.groupId
		WHERE su.userId = @userId AND sg.orgId=b.orgId
		AND (
			s.[key] = 'Owner'
			OR s.[key] = 'BoardsCanViewAll'
		))
		THEN 1 ELSE 0 END AS viewall
	) AS sec
	WHERE (
		(sec.viewall = 1) OR
		(sec.viewall = 0 AND bm.userId=@userId)
	) 
	AND
	(
		(@orgId > 0 AND b.orgId=@orgId)
		OR @orgId <= 0
	)
	AND b.orgId IN (SELECT * FROM #orgs)
	ORDER BY 
	org.isprivate DESC,
	org.sort ASC,
	org.[name] ASC,
	CASE WHEN @sort = 1 THEN b.[name] END ASC,
	CASE WHEN @sort = 0 THEN bm.favorite END ASC,
	CASE WHEN @sort = 0 OR @sort = 2 THEN b.lastmodified END DESC