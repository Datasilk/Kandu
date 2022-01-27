CREATE PROCEDURE [dbo].[Security_GetScopesForKeys]
	@groupId int
AS
	DECLARE @cursor CURSOR, @key varchar(32), @scope int, @scopeId int, @scopeType varchar(32), @scopeItem nvarchar(64)
	DECLARE @tbl table (
		[key] varchar(32),
		scope int,
		scopeId int,
		scopeType varchar(32),
		scopeItem nvarchar(64)
	)
	SET @cursor = CURSOR FOR
	SELECT [key], scope, scopeId FROM [Security]
	WHERE groupId=@groupId
	OPEN @cursor
	FETCH FROM @cursor INTO @key, @scope, @scopeId
	WHILE @@FETCH_STATUS = 0 BEGIN
		--check different tables based on scope value
		If @scope = 1 BEGIN
			SET @scopeType = 'Organization'
			SELECT @scopeItem = name FROM Organizations WHERE orgId=@scopeId
			INSERT INTO @tbl ([key], scope, scopeId, scopeType, scopeItem)
			VALUES (@key, @scope, @scopeId, @scopeType, @scopeItem)
		END
		Else If @scope = 2 BEGIN
			SET @scopeType = 'Security Group'
			SELECT @scopeItem = name FROM SecurityGroups WHERE groupId=@scopeId
			INSERT INTO @tbl ([key], scope, scopeId, scopeType, scopeItem)
			VALUES (@key, @scope, @scopeId, @scopeType, @scopeItem)
		END
		Else If @scope = 3 BEGIN
			SET @scopeType = 'Team'
			SELECT @scopeItem = name FROM Teams WHERE teamId=@scopeId
			INSERT INTO @tbl ([key], scope, scopeId, scopeType, scopeItem)
			VALUES (@key, @scope, @scopeId, @scopeType, @scopeItem)
		END
		Else If @scope = 4 BEGIN
			SET @scopeType = 'Board'
			SELECT @scopeItem = name FROM Boards WHERE boardId=@scopeId
			INSERT INTO @tbl ([key], scope, scopeId, scopeType, scopeItem)
			VALUES (@key, @scope, @scopeId, @scopeType, @scopeItem)
		END
		Else If @scope = 5 BEGIN
			SET @scopeType = 'List'
			SELECT @scopeItem = name FROM Lists WHERE listId=@scopeId
			INSERT INTO @tbl ([key], scope, scopeId, scopeType, scopeItem)
			VALUES (@key, @scope, @scopeId, @scopeType, @scopeItem)
		END
		Else If @scope = 6 BEGIN
			SET @scopeType = 'Card'
			SELECT @scopeItem = name FROM Cards WHERE cardId=@scopeId
			INSERT INTO @tbl ([key], scope, scopeId, scopeType, scopeItem)
			VALUES (@key, @scope, @scopeId, @scopeType, @scopeItem)
		END
		FETCH FROM @cursor INTO @key, @scope, @scopeId
	END
	CLOSE @cursor
	DEALLOCATE @cursor

	SELECT * FROM @tbl
