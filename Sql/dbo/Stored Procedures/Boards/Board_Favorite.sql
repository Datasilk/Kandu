CREATE PROCEDURE [dbo].[Board_Favorite]
	@boardId int,
	@userId int
AS
	IF EXISTS(SELECT * FROM BoardMembers WHERE boardId=@boardId AND userId=@userId) BEGIN
		UPDATE BoardMembers SET favorite=1 WHERE boardId=@boardId AND userId=@userId
	END ELSE BEGIN
		INSERT INTO BoardMembers (boardId, userId, favorite) VALUES (@boardId, @userId, 1)
	END

