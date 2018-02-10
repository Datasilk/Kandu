CREATE PROCEDURE [dbo].[Card_UpdateDescription]
	@boardId int,
	@cardId int,
	@description nvarchar(MAX)
AS
	UPDATE Cards SET description=@description WHERE cardId=@cardId AND boardId=@boardId
