CREATE PROCEDURE [dbo].[Board_Create]
	@orgId int,
	@teamId int,
	@userId int,
	@name nvarchar(64),
	@favorite bit = 0,
	@color nvarchar(6) = ''
AS
	DECLARE @boardId int = NEXT VALUE FOR SequenceBoards
	INSERT INTO Boards (boardId, orgId, [name], color)
	VALUES (@boardId, @orgId, @name, @color)

	INSERT INTO BoardTeams (boardId, teamId) VALUES (@boardId, @teamId)

	IF @favorite = 1 BEGIN
		EXEC Board_Favorite @boardId=@boardId, @userId=@userId
	END ELSE BEGIN
		EXEC Board_Unfavorite @boardId=@boardId, @userId=@userId
	END

	SELECT @boardId