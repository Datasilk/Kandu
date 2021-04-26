CREATE PROCEDURE [dbo].[Board_Import]
	@orgId int,
	@teamId int,
	@userId int,
	@name nvarchar(64),
	@favorite bit = 0,
	@color nvarchar(6) = '',
	@merge bit = 0
AS
	DECLARE @oldId int = 0, @create bit = 1
	SELECT @oldId = boardId FROM Boards WHERE boardId IN (SELECT boardId FROM BoardMembers WHERE userId=@userId) AND [name]=@name
	CREATE TABLE #tmp (id int)

	IF @oldId IS NOT NULL AND @oldId > 0 BEGIN
		/* board already exists */
		IF @merge = 0 BEGIN
			/* do not merge. Instead, delete board and all associated lists, cards, comments, checklists, and activity */
			SELECT listId INTO #lists FROM Lists WHERE boardId=@oldId
			SELECT cardId INTO #cards FROM Cards WHERE listId IN (SELECT * FROM #lists)
			DELETE FROM CardMembers WHERE cardId IN (SELECT * FROM #cards)
			DELETE FROM CardComments WHERE cardId IN (SELECT * FROM #cards)
			DELETE FROM CardChecklistItems WHERE cardId IN (SELECT * FROM #cards)
			DELETE FROM CardChecklists WHERE cardId IN (SELECT * FROM #cards)
			DELETE FROM Cards WHERE boardId=@oldId
			DELETE FROM Lists WHERE boardId=@oldId
		END
		/* merge */
		UPDATE Boards SET @color=@color WHERE boardId=@oldId
		SET @create = 0
	END

	IF @create = 1 BEGIN
		INSERT INTO #tmp EXEC Board_Create @orgId=@orgId, @teamId=@teamId, @userId=@userId, @name=@name, @favorite=@favorite, @color=@color
		SELECT @oldId=id FROM #tmp
	END

	IF @favorite = 1 BEGIN
		EXEC Board_Favorite @boardId=@oldId, @userId=@userId
	END ELSE BEGIN
		EXEC Board_Unfavorite @boardId=@oldId, @userId=@userId
	END

	SELECT @oldId