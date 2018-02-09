CREATE PROCEDURE [dbo].[Card_Move]
	@boardId int,
	@listId int,
	@cardId int,
	@ids nvarchar(MAX)
AS
	/* first, update card listId */
	UPDATE Cards SET listId=@listId WHERE cardId=@cardId

	/* next, sort cards for list */
	SELECT * INTO #cards FROM dbo.SplitArray(@ids,',')

	DECLARE @id int,
		@cursor CURSOR,
		@inc int = 0

	SET @cursor = CURSOR FOR
	SELECT valueInt FROM #cards ORDER BY Position ASC
	OPEN @cursor
	FETCH FROM @cursor INTO @id
	WHILE @@FETCH_STATUS = 0 BEGIN
		UPDATE Cards SET sort=@inc WHERE cardId=@id AND listId=@listId
		SET @inc = @inc + 1
		FETCH FROM @cursor INTO @id
	END
	CLOSE @cursor
	DEALLOCATE @cursor

	EXEC Board_Modified @boardId=@boardId
