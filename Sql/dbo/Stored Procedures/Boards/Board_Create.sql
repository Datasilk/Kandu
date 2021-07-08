CREATE PROCEDURE [dbo].[Board_Create]
	@orgId int,
	@teamId int,
	@userId int,
	@name nvarchar(64),
	@favorite bit = 0,
	@color nvarchar(6) = '',
	@cardtype varchar(16)
AS
	DECLARE @boardId int = NEXT VALUE FOR SequenceBoards
	INSERT INTO Boards (boardId, orgId, [name], color, cardtype)
	VALUES (@boardId, @orgId, @name, @color, @cardtype)

	IF (@teamId IS NOT NULL AND @teamId > 0) BEGIN
		INSERT INTO BoardTeams (boardId, teamId) VALUES (@boardId, @teamId)
	END

	IF @favorite = 1 BEGIN
		EXEC Board_Favorite @boardId=@boardId, @userId=@userId
	END ELSE BEGIN
		EXEC Board_Unfavorite @boardId=@boardId, @userId=@userId
	END

	SELECT @boardId