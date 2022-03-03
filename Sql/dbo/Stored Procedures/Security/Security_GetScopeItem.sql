CREATE PROCEDURE [dbo].[Security_GetScopeItem]
	@scopeId int,
	@scope int
AS
	IF @scope = 1 BEGIN -- Organization
		SELECT orgId AS id, [name] AS title, [name] AS orgName 
		FROM Organizations WHERE orgId = @scopeId
	END
	ELSE IF @scope = 2 BEGIN -- Security Group
		SELECT sg.groupId AS id, sg.[name] AS title, o.[name] AS orgName 
		FROM SecurityGroups sg
		JOIN Organizations o ON o.orgId = sg.orgId
		WHERE sg.groupId = @scopeId AND sg.personal = 0
	END
	ELSE IF @scope = 3 BEGIN -- Team
		SELECT t.teamId AS id, t.[name] AS title, o.[name] AS orgName 
		FROM Teams t
		JOIN Organizations o ON o.orgId = t.orgId
		WHERE t.teamId = @scopeId
	END
	ELSE IF @scope = 4 BEGIN -- Board
		SELECT b.boardId AS id, b.[name] AS title, o.[name] AS orgName 
		FROM Boards b
		JOIN Organizations o ON o.orgId = b.orgId
		WHERE boardId = @scopeId
	END
	ELSE IF @scope = 5 BEGIN -- List
		SELECT l.listId AS id, b.[name] + ': ' + l.[name] AS title, o.[name] AS orgName 
		FROM Lists l
		JOIN Boards b ON b.boardId = l.boardId
		JOIN Organizations o ON o.orgId = b.orgId
		WHERE l.listId = @scopeId
	END
	ELSE IF @scope = 6 BEGIN -- Card
		SELECT c.cardId AS id, b.[name] + ': ' + c.[name] AS title, o.[name] AS orgName  
		FROM Cards c
		JOIN Boards b ON b.boardId = c.boardId
		JOIN Organizations o ON o.orgId = b.orgId
		WHERE c.cardId = @scopeId
	END
