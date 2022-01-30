CREATE PROCEDURE [dbo].[Card_UpdateDescription]
	@boardId int,
	@cardId int,
	@description nvarchar(MAX)
AS
	UPDATE CardDescriptions SET [description]=@description WHERE cardId=@cardId
	UPDATE Cards SET datemodified=GETUTCDATE() WHERE cardId=@cardId
	EXEC Board_Modified @boardId=@boardId
