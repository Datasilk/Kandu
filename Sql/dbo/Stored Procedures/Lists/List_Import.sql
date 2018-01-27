CREATE PROCEDURE [dbo].[List_Import]
	@boardId int,
	@name nvarchar(64),
	@sort int = 0,
	@merge bit = 0
AS
	DECLARE @oldId int = 0, @create bit = 1
	SELECT @oldId = listId FROM Lists WHERE boardId=@boardId AND [name]=@name
	CREATE TABLE #tmp (id int)

	IF @oldId IS NOT NULL AND @oldId > 0 BEGIN
		/* card already exists */
		IF @merge = 1 BEGIN
			/* merge */
			SET @create = 0
		END
	END

	IF @create = 1 BEGIN
		INSERT INTO #tmp EXEC List_Create @boardId=@boardId, @name=@name, @sort=@sort
		SELECT @oldId=id FROM #tmp
	END

	SELECT @oldId
