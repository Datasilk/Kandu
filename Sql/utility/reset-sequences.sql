DECLARE @maxid int
DECLARE @sql nvarchar(MAX) = 'ALTER SEQUENCE #sequence#
RESTART WITH #maxid# INCREMENT BY 1', @altered nvarchar(MAX)


SET @maxId = 0
SELECT TOP 1 @maxid = attachmentId + 1 FROM CardAttachments ORDER BY attachmentId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceAttachments')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = boardId + 1 FROM Boards ORDER BY boardId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceBoards')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = cardId + 1 FROM Cards ORDER BY cardId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceCards')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = itemId + 1 FROM CardChecklistItems ORDER BY itemId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceChecklistItems')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = commentId + 1 FROM CardComments ORDER BY commentId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceComments')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = clientId + 1 FROM EmailClients ORDER BY clientId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceEmailClients')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = labelId + 1 FROM CardLabels ORDER BY labelId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceLabels')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = listId + 1 FROM Lists ORDER BY listId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceLists')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = orgId + 1 FROM Organizations ORDER BY orgId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceOrganizations')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = groupId + 1 FROM SecurityGroups ORDER BY groupId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceSecurityGroups')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = teamId + 1 FROM Teams ORDER BY teamId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceTeams')
	PRINT @altered
	EXEC sp_executesql @altered
END

SET @maxId = 0
SELECT TOP 1 @maxid = userId + 1 FROM Users ORDER BY userId DESC
IF @maxId > 0 BEGIN
	SET @altered = REPLACE(REPLACE(@sql, '#maxid#', @maxid), '#sequence#', 'SequenceUsers')
	PRINT @altered
	EXEC sp_executesql @altered
END