CREATE PROCEDURE [dbo].[Cards_GetFlaggedComments]
	@userId int
AS
	
	IF EXISTS(SELECT * FROM [Security] s JOIN SecurityUsers su ON su.groupId = s.groupId WHERE su.userId=@userId AND s.[key] = 'AppOwner' OR s.[key] = 'AppFullAccess' OR s.[key] = 'AppFullAccess') BEGIN
		SELECT c.* FROM CardComments c JOIN Users u ON u.userId = c.userId WHERE c.flagged = 1
	END
	SELECT DISTINCT cc.*, c.[name], b.orgId
	FROM CardComments cc 
	JOIN Cards c ON c.cardId = cc.cardId
	JOIN Boards b ON b.boardId = c.boardId
	JOIN Users u ON u.userId = cc.userId 
	JOIN [SecurityUsers] su ON su.userId = cc.userId
	JOIN [Security] s ON s.groupId = su.groupId
	WHERE cc.flagged = 1
	AND (
		(
			(s.[key] IN ('Owner', 'OrgFullAccess', 'BoardsFullAccess') 
			AND b.orgId = s.scopeId)
		)
		OR (
			(s.[key] IN ('BoardCanRemoveComments') 
			AND c.boardId = s.scopeId)
		)
	)