CREATE PROCEDURE [dbo].[Cards_Sort]
	@listId int,
	@ids nvarchar(MAX)
AS
	SELECT * INTO #cards FROM dbo.SplitArray(@ids,',')

	DECLARE @id int,
		@cursor CURSOR,
		@inc int = 0

	SET @cursor = CURSOR FOR
	SELECT valueInt FROM #cards
	OPEN @cursor
	FETCH FROM @cursor INTO @id
	WHILE @@FETCH_STATUS = 0 BEGIN
		UPDATE Cards SET sort=@inc WHERE cardId=@id AND listId=@listId
		FETCH FROM @cursor INTO @id
	END
	CLOSE @cursor
	DEALLOCATE @cursor


