CREATE PROCEDURE [dbo].[Card_Create]
	@listId int,
	@boardId int,
	@type int = 0,
	@colors nvarchar(128),
	@name nvarchar(64),
	@datedue datetime = NULL,
	@description nvarchar(MAX) = ''
AS
	DECLARE @id int = NEXT VALUE FOR SequenceCards

	IF(YEAR(@datedue) < YEAR(GETDATE()) - 99) BEGIN
		SET @datedue = NULL
	END
	INSERT INTO Cards (cardId, listId, boardId, [type], colors, [name], datecreated, datedue, [description])
	VALUES (@id, @listId, @boardId, @type, @colors, @name, GETDATE(), @datedue, @description)

	EXEC Board_Modified @boardId=@boardId

	SELECT @id
