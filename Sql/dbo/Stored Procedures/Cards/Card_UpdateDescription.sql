CREATE PROCEDURE [dbo].[Card_UpdateDescription]
	@boardId int,
	@cardId int,
	@description nvarchar(MAX)
AS
	IF NOT EXISTS(SELECT * FROM CardDescriptions WHERE cardId=@cardId) BEGIN
		INSERT INTO CardDescriptions ([description], cardId) VALUES (@description, @cardId)
	END ELSE BEGIN
		UPDATE CardDescriptions SET [description]=@description WHERE cardId=@cardId
	END
	UPDATE Cards SET datemodified=GETUTCDATE() WHERE cardId=@cardId
	EXEC Board_Modified @boardId=@boardId
