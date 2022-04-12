CREATE PROCEDURE [dbo].[Card_Checklist_UpdateItemChecked]
	@itemId int,
	@userId int,
	@cardId int,
	@ischecked bit
AS
	UPDATE CardChecklistItems 
	SET [checked]=@ischecked
	WHERE itemId=@itemId AND cardId=@cardId