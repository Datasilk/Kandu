CREATE PROCEDURE [dbo].[Card_GetMembers]
	@cardId int
AS
	DECLARE @orgId int, @boardId int
	SELECT @orgId = b.orgId, @boardId = c.boardId FROM Cards c
	JOIN Boards b ON b.boardId = c.boardId 
	WHERE c.cardId=@cardId

	SELECT DISTINCT u.userId, u.[name], u.photo, k.sort
	FROM [Security] s
	JOIN SecurityUsers su ON su.groupId = s.groupId
	JOIN Users u ON u.userId = su.userId 
	CROSS APPLY (
		SELECT CASE 
			WHEN s.[key] = 'AppOwner' OR s.[key] = 'AppFullAccess' THEN 1
			WHEN s.[key] = 'Owner' THEN 2
			WHEN s.[key] = 'OrgFullAccess' THEN 3
			WHEN s.[key] = 'BoardsFullAccess' THEN 4
			WHEN s.[key] = 'BoardCanUpdate' THEN 5
			WHEN s.[key] = 'CardFullAccess' THEN 6
			WHEN s.[key] = 'CardCanUpdate' THEN 7
			ELSE 9 END AS sort
	) AS k
	WHERE (s.[key] = 'AppOwner' OR s.[key] = 'AppFullAccess')
	OR (s.[key] = 'Owner' AND s.orgId=@orgId)
	OR (s.[key] = 'OrgFullAccess' AND s.orgId=@orgId)
	OR (s.[key] = 'BoardsFullAccess' AND s.scopeId=@boardId)
	OR (s.[key] = 'BoardCanUpdate' AND s.scopeId=@boardId)
	OR (s.[key] = 'CardFullAccess' AND s.scopeId=@cardId)
	OR (s.[key] = 'CardCanUpdate' AND s.scopeId=@cardId)
	ORDER BY k.sort
	
