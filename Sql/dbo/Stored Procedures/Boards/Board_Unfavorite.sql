CREATE PROCEDURE [dbo].[Board_Unfavorite]
	@boardId int,
	@userId int
AS
	IF EXISTS(SELECT * FROM BoardMembers WHERE boardId=@boardId AND userId=@userId) BEGIN
		UPDATE BoardMembers SET favorite=0 WHERE boardId=@boardId AND userId=@userId
	END ELSE BEGIN
		INSERT INTO BoardMembers (boardId, userId, favorite) VALUES (@boardId, @userId, 0)
	END

