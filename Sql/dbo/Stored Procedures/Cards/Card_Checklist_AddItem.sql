CREATE PROCEDURE [dbo].[Card_Checklist_AddItem]
	@cardId int,
	@sort int,
	@label nvarchar(255)
AS
	DECLARE @id int = NEXT VALUE FOR SequenceChecklistItems
	INSERT INTO CardChecklistItems (itemId, cardId, [sort], [label])
	VALUES (@id, @cardId, @sort, @label)
	SELECT @id
