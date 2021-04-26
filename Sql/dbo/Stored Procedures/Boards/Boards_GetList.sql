CREATE PROCEDURE [dbo].[Boards_GetList]
	@userId int
AS
	SELECT DISTINCT b.*, org.sort, org.[name] AS orgName, bm.favorite 
	FROM TeamMembers tm
	JOIN BoardTeams bt ON bt.teamId = tm.teamId
	JOIN Boards b ON b.boardId = bt.boardId
	JOIN Teams t ON t.teamId = tm.teamId
	LEFT JOIN BoardMembers bm ON bm.boardId=bt.boardId AND bm.userId=@userId
	CROSS APPLY (
		SELECT 
		CASE WHEN o.ownerId=@userId THEN 0 ELSE 1 END AS sort,
		o.orgId, o.[name]
		FROM Organizations o
		WHERE o.orgId = t.orgId
	) AS org
	WHERE tm.userId=@userId
	ORDER BY org.sort ASC, org.[name] ASC, bm.favorite DESC, b.lastmodified DESC