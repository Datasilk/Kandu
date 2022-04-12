CREATE PROCEDURE [dbo].[Card_Checklist_GetItem]
	@itemId int,
	@cardId int
AS
	SELECT * FROM CardChecklistItems WHERE itemId=@itemId AND cardId=@cardId
