CREATE PROCEDURE [dbo].[Board_Create]
	@teamId int,
	@name nvarchar(64),
	@favorite bit = 0,
	@security smallint = 0,
	@color nvarchar(6) = ''
AS
	DECLARE @id int = NEXT VALUE FOR SequenceBoards
	INSERT INTO Boards (boardId, teamId, [name], favorite, [security], color)
	VALUES (@id, @teamId, @name, @favorite, @security, @color)

	SELECT @id