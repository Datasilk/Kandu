CREATE PROCEDURE [dbo].[Card_UpdateJson]
	@boardId int,
	@cardId int,
	@json nvarchar(MAX)
AS
	IF EXISTS(SELECT * FROM CardJson WHERE cardId=@cardId) BEGIN
		UPDATE CardJson SET [json]=@json WHERE cardId=@cardId
		UPDATE Cards SET datemodified=GETUTCDATE() WHERE cardId=@cardId
	END
	
	EXEC Board_Modified @boardId=@boardId
