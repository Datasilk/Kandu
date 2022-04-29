CREATE PROCEDURE [dbo].[Card_Checklist_SortItems]
	@userId int,
	@cardId int,
	@ids varchar(MAX)
AS
	SELECT * INTO #items FROM dbo.SplitArray(@ids,',')

	DECLARE @id int,
		@cursor CURSOR,
		@inc int = 0

	SET @cursor = CURSOR FOR
	SELECT valueInt FROM #items ORDER BY Position ASC
	OPEN @cursor
	FETCH FROM @cursor INTO @id
	WHILE @@FETCH_STATUS = 0 BEGIN
		UPDATE CardChecklistItems SET sort=@inc WHERE itemId=@id AND cardId=@cardId
		SET @inc = @inc + 1
		FETCH FROM @cursor INTO @id
	END
	CLOSE @cursor
	DEALLOCATE @cursor
