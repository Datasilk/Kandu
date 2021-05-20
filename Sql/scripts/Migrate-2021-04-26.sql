DELETE FROM OrgSecurity
DECLARE @userId int, @name nvarchar(255),
@orgId int, @teamId int, @boardId int
DECLARE @tmp TABLE (id int)
DECLARE @cursor CURSOR, @cursor2 CURSOR
DECLARE @used bit = 0
SET @cursor = CURSOR FOR
SELECT userId, [name] FROM Users
OPEN @cursor
FETCH NEXT FROM @cursor INTO @userId, @name
WHILE @@FETCH_STATUS = 0 BEGIN
	-- create organization for user
	IF NOT EXISTS(SELECT * FROM Organizations WHERE ownerId=@userId AND isprivate = 1) BEGIN
		INSERT INTO @tmp EXEC Organization_Create @ownerId=@userId, @name='Personal', @website='', @description='Private Organization for myself', @isprivate=1
		SELECT @orgId = id FROM @tmp
		DELETE FROM @tmp
	PRINT 'Created Organization for ' + CONVERT(nvarchar(MAX), @userId)
	END ELSE BEGIN
		SELECT @orgId=orgId FROM Organizations WHERE ownerId=@userId AND isprivate = 1
		SET @used = 1
	PRINT 'Found Organization for ' + CONVERT(nvarchar(MAX), @userId)
	END
	
	--user should already have a team & team members & board members
	SELECT TOP 1 @teamId=teamId FROM Teams WHERE ownerId=@userId
	UPDATE Teams SET orgId=@orgId WHERE ownerId=@userId
	PRINT 'Found Team ID ' + CONVERT(nvarchar(MAX), @teamId)

	--update boards
	UPDATE Boards SET orgId=@orgId WHERE teamId=@teamId

	IF @used = 0 BEGIN
		--update board teams
		SET @cursor2 = CURSOR FOR
		SELECT boardId FROM Boards WHERE teamId=@teamId
		OPEN @cursor2
		FETCH NEXT FROM @cursor2 INTO @boardId
		WHILE @@FETCH_STATUS = 0 BEGIN
			INSERT INTO BoardTeams (boardId, teamId) VALUES (@boardId, @teamId)
			PRINT 'New Board Team for Board ID ' + CONVERT(nvarchar(MAX), @boardId)
			FETCH NEXT FROM @cursor2 INTO @boardId
		END
		CLOSE @cursor2
		DEALLOCATE @cursor2
	END
	FETCH NEXT FROM @cursor INTO @userId, @name
END
CLOSE @cursor
DEALLOCATE @cursor