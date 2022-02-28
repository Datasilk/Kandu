DECLARE @cursor1 CURSOR, @orgId int, @userId int, @groupId int, @name nvarchar(64)
DECLARE @tbl TABLE (
	groupId int
)
SELECT DISTINCT sg.orgId, su.userId INTO #tbl 
FROM SecurityUsers su
JOIN SecurityGroups sg ON sg.groupId=su.groupId
JOIN Organizations o ON o.orgId=sg.orgId AND o.isprivate=0

SELECT DISTINCT o.orgId, o.ownerId AS userId
INTO #tbl2
FROM Organizations o
LEFT JOIN #tbl t ON NOT (t.orgId=o.orgId AND t.userId = o.ownerId)

SET @cursor1 = CURSOR FOR
SELECT * FROM #tbl UNION SELECT * FROM #tbl2
OPEN @cursor1
FETCH NEXT FROM @cursor1 INTO @orgId, @userId
WHILE @@FETCH_STATUS = 0 BEGIN
	SET @groupId = NEXT VALUE FOR SequenceSecurityGroups
	PRINT @groupId
	INSERT INTO SecurityGroups (groupId, orgId, [name], personal) VALUES (@groupId, @orgId, 'User', 1)
	INSERT INTO SecurityUsers (groupId, userId) VALUES (@groupId, @userId)
	FETCH NEXT FROM @cursor1 INTO @orgId, @userId
END
CLOSE @cursor1
DEALLOCATE @cursor1
DROP TABLE #tbl