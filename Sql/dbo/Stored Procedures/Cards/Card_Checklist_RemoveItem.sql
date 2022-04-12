CREATE PROCEDURE [dbo].[Card_Checklist_RemoveItem]
	@itemId int,
	@cardId int,
	@userId int
AS
	DELETE FROM CardChecklistItems WHERE itemId=@itemId AND cardId=@cardId
