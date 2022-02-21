CREATE PROCEDURE [dbo].[Card_Comment_Update]
	@commentId int,
	@cardId int,
	@userId int,
	@comment nvarchar(MAX)
AS
	UPDATE CardComments SET comment=@comment WHERE commentId=@commentId AND cardId=@cardId
