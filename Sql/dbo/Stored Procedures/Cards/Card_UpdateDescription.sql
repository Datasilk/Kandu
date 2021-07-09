CREATE PROCEDURE [dbo].[Card_UpdateDescription]
	@boardId int,
	@cardId int,
	@description nvarchar(MAX)
AS
	UPDATE Cards SET [description]=@description, datemodified=GETUTCDATE() WHERE cardId=@cardId AND boardId=@boardId
	EXEC Board_Modified @boardId=@boardId
