CREATE PROCEDURE [dbo].[Card_UpdateName]
	@boardId int,
	@cardId int,
	@name nvarchar(MAX)
AS
	UPDATE Cards SET name=@name WHERE cardId=@cardId AND boardId=@boardId
