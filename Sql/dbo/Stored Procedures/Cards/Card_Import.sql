CREATE PROCEDURE [dbo].[Card_Import]
	@listId int,
	@boardId int,
	@layout int = 0,
	@colors nvarchar(128),
	@name nvarchar(MAX),
	@type varchar(16),
	@datedue datetime = NULL,
	@description nvarchar(MAX) = '',
	@json nvarchar(MAX) = '',
	@merge bit = 0
AS
	DECLARE @oldId int = 0, @create bit = 1
	SELECT @oldId = cardId FROM Cards WHERE boardId=@boardId AND [name]=@name
	CREATE TABLE #tmp (id int)

	IF @oldId IS NOT NULL AND @oldId > 0 BEGIN
		/* card already exists */
		IF @merge = 1 BEGIN
			/* merge */
			UPDATE Cards SET [type]=@type, colors=@colors, datedue=@datedue WHERE cardId=@oldId
			UPDATE CardDescriptions SET [description]=@description WHERE cardId=@oldId
			SET @create = 0
		END
	END

	IF @create = 1 BEGIN
		INSERT INTO #tmp EXEC Card_Create @listId=@listId, @boardId=@boardId, @layout=@layout, @colors=@colors, @name=@name, @type=@type, @datedue=@datedue, @description=@description, @json=@json
		SELECT @oldId=id FROM #tmp
	END

	EXEC Board_Modified @boardId=@boardId

	SELECT @oldId
