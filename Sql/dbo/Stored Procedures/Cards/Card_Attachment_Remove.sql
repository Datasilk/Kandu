CREATE PROCEDURE [dbo].[Card_Attachment_Remove]
	@cardId int,
	@userId int,
	@attachmentId int
AS
	DELETE FROM CardAttachments 
	WHERE cardId = @cardId AND attachmentId = @attachmentId
