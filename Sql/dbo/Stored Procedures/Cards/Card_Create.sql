CREATE PROCEDURE [dbo].[Card_Create]
	@listId int,
	@boardId int,
	@layout int = 0,
	@colors nvarchar(128),
	@name nvarchar(MAX),
	@type varchar(16),
	@datedue datetime = NULL,
	@description nvarchar(MAX) = '',
	@json nvarchar(MAX) = ''
AS
	DECLARE @id int = NEXT VALUE FOR SequenceCards

	IF(YEAR(@datedue) < YEAR(GETDATE()) - 99) BEGIN
		SET @datedue = NULL
	END
	INSERT INTO Cards (cardId, listId, boardId, layout, colors, [name], [type], datecreated, datedue)
	VALUES (@id, @listId, @boardId, @layout, @colors, @name, @type, GETDATE(), @datedue)

	INSERT INTO CardDescriptions (cardId, [description]) VALUES (@id, @description)

	IF @json IS NOT NULL AND @json <> '' BEGIN
		INSERT INTO CardJson (cardId, [json]) VALUES (@id, @json)
	END

	EXEC Board_Modified @boardId=@boardId

	SELECT @id
