CREATE PROCEDURE [dbo].[Card_Create]
	@listId int,
	@boardId int,
	@color int,
	@name nvarchar(64),
	@datedue datetime = NULL,
	@description nvarchar(MAX) = ''
AS
	DECLARE @id int = NEXT VALUE FOR SequenceCards

	INSERT INTO Cards (cardId, listId, boardId, color, [name], datecreated, datedue, [description])
	VALUES (@id, @listId, @boardId, @color, @name, GETDATE(), @datedue, @description)
