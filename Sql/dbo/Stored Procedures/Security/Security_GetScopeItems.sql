CREATE PROCEDURE [dbo].[Security_GetScopeItems]
	@orgId int,
	@groupId int,
	@key varchar(32),
	@scope int
AS
	IF @scope = 1 BEGIN -- Organization
		SELECT orgId AS id, [name] AS title FROM Organizations WHERE orgId = @orgId
	END
	ELSE IF @scope = 2 BEGIN -- Security Group
		SELECT groupId AS id, [name] AS title FROM SecurityGroups WHERE orgId = @orgId AND personal = 0
	END
	ELSE IF @scope = 3 BEGIN -- Team
		SELECT teamId AS id, [name] AS title FROM Teams WHERE orgId = @orgId
	END
	ELSE IF @scope = 4 BEGIN -- Board
		SELECT boardId AS id, [name] AS title FROM Boards WHERE orgId = @orgId
	END
	ELSE IF @scope = 5 BEGIN -- List
		SELECT l.listId AS id, b.[name] + ': ' + l.[name] AS title FROM Lists l
		JOIN Boards b ON b.boardId = l.boardId
		WHERE b.orgId = @orgId
		ORDER BY b.[name] ASC, l.[name] ASC
	END
	ELSE IF @scope = 6 BEGIN -- Card
		SELECT c.cardId AS id, b.[name] + ': ' + c.[name] AS title FROM Cards c
		JOIN Boards b ON b.boardId = c.boardId
		WHERE b.orgId = @orgId
		ORDER BY b.[name] ASC, c.[name] ASC
	END
