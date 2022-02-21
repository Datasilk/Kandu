CREATE PROCEDURE [dbo].[Card_Comment_Add]
	@cardId int,
	@userId int,
	@comment nvarchar(MAX)
AS
	DECLARE @id int = NEXT VALUE FOR SequenceComments
	INSERT INTO CardComments (commentId, cardId, userId, datecreated, comment)
	VALUES (@id, @cardId, @userId, GETUTCDATE(), @comment)

	SELECT @id