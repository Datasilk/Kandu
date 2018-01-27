CREATE PROCEDURE [dbo].[Card_Import]
	@listId int,
	@boardId int,
	@type int = 0,
	@colors nvarchar(128),
	@name nvarchar(255),
	@datedue datetime = NULL,
	@description nvarchar(MAX) = '',
	@merge bit = 0
AS
	DECLARE @oldId int = 0, @create bit = 1
	SELECT @oldId = cardId FROM Cards WHERE boardId=@boardId AND [name]=@name
	CREATE TABLE #tmp (id int)

	IF @oldId IS NOT NULL AND @oldId > 0 BEGIN
		/* card already exists */
		IF @merge = 1 BEGIN
			/* merge */
			UPDATE Cards SET colors=@colors, datedue=@datedue, description=@description
			SET @create = 0
		END
	END

	IF @create = 1 BEGIN
		INSERT INTO #tmp EXEC Card_Create @listId=@listId, @boardId=@boardId, @type=@type, @colors=@colors, @name=@name, @datedue=@datedue, @description=@description
		SELECT @oldId=id FROM #tmp
	END

	SELECT @oldId
