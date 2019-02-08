CREATE PROCEDURE [dbo].[Card_Create]
	@listId int,
	@boardId int,
	@layout int = 0,
	@colors nvarchar(128),
	@name nvarchar(MAX),
	@datedue datetime = NULL,
	@description nvarchar(MAX) = ''
AS
	DECLARE @id int = NEXT VALUE FOR SequenceCards

	IF(YEAR(@datedue) < YEAR(GETDATE()) - 99) BEGIN
		SET @datedue = NULL
	END
	INSERT INTO Cards (cardId, listId, boardId, layout, colors, [name], datecreated, datedue, [description])
	VALUES (@id, @listId, @boardId, @layout, @colors, @name, GETDATE(), @datedue, @description)

	EXEC Board_Modified @boardId=@boardId

	SELECT @id
