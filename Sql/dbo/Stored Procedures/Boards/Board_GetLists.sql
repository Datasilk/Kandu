CREATE PROCEDURE [dbo].[Board_GetLists]
	@boardId int
AS
	SELECT * FROM Boards WHERE boardId=@boardId
	
	/* create temp tables */
	DECLARE @lists TABLE (
		listId int,
		boardId int,
		[name] nvarchar(64),
		sort int
	) 
	DECLARE @cards TABLE (
		[cardId] INT NOT NULL PRIMARY KEY, 
		[listId] INT NOT NULL,
		[boardId] INT NOT NULL, 
		[sort] INT NOT NULL DEFAULT 999, 
		[type] INT NOT NULL DEFAULT 0, 
		[archived] BIT NOT NULL DEFAULT 0,  
		[datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
		[datedue] DATETIME NULL , 
		[name] NVARCHAR(64) NOT NULL, 
		[colors] NVARCHAR(128) NOT NULL DEFAULT '',
		[description] NVARCHAR(MAX) NOT NULL DEFAULT ''
	) 

	/* declare variables */
	DECLARE @cursor CURSOR,
	@listId int, @name nvarchar(MAX), @sort int

	/* get lists for board */
	SET @cursor = CURSOR FOR
	SELECT listId, [name], sort FROM Lists WHERE boardId=@boardId ORDER BY sort
	OPEN @cursor
	FETCH FROM @cursor INTO @listId, @name, @sort
	WHILE @@FETCH_STATUS = 0 BEGIN
		INSERT INTO @lists (listId, boardId, [name], sort) VALUES (@listId, @boardId, @name, @sort)
		/* get cards for list */
		INSERT INTO @cards SELECT * FROM Cards WHERE listId=@listId

		FETCH FROM @cursor INTO @listId, @name, @sort
	END
	CLOSE @cursor
	DEALLOCATE @cursor

	SELECT * FROM @lists

	SELECT * FROM @cards