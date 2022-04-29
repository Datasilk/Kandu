CREATE PROCEDURE [dbo].[Card_Checklist_UpdateItemChecked]
	@itemId int,
	@userId int,
	@cardId int,
	@ischecked bit
AS
	UPDATE CardChecklistItems 
	SET [ischecked]=@ischecked
	WHERE itemId=@itemId AND cardId=@cardId