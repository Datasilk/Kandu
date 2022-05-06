CREATE PROCEDURE [dbo].[Card_Attachment_GetInfo]
	@attachmentId int
AS
	SELECT * FROM CardAttachments WHERE attachmentId=@attachmentId
