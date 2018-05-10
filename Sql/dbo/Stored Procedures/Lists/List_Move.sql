CREATE PROCEDURE [dbo].[List_Move]
	@boardId int,
	@ids nvarchar(MAX)
AS
	/* sort lists for board */
	SELECT * INTO #lists FROM dbo.SplitArray(@ids,',')

	DECLARE @id int,
		@cursor CURSOR,
		@inc int = 0

	SET @cursor = CURSOR FOR
	SELECT valueInt FROM #lists ORDER BY Position ASC
	OPEN @cursor
	FETCH FROM @cursor INTO @id
	WHILE @@FETCH_STATUS = 0 BEGIN
		UPDATE Lists SET sort=@inc WHERE listId=@id
		SET @inc = @inc + 1
		FETCH FROM @cursor INTO @id
	END
	CLOSE @cursor
	DEALLOCATE @cursor

	EXEC Board_Modified @boardId=@boardId