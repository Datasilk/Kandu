CREATE PROCEDURE [dbo].[Card_Checklist_AddItem]
	@userId int,
	@cardId int,
	@sort int,
	@label nvarchar(255),
	@ischecked bit
AS
	DECLARE @id int = NEXT VALUE FOR SequenceChecklistItems
	IF (@sort < 0) BEGIN
		-- get sort index
		SELECT @sort = CASE WHEN COUNT(*) > ISNULL(MAX(sort), 0) THEN COUNT(*) ELSE ISNULL(MAX(sort), 0) END + 1
		FROM CardChecklistItems
		WHERE cardId=@cardId
	END
	IF (@label = '') BEGIN
		-- get default label
		DECLARE @index int = 1
		SELECT @index = CASE WHEN COUNT(*) > ISNULL(MAX(sort), 0) THEN COUNT(*) ELSE ISNULL(MAX(sort), 0) END + 1
		FROM CardChecklistItems
		WHERE cardId=@cardId
		SET @label = 'Checklist Item #' + CAST(@index AS varchar(32))
	END
	-- create checklist item record
	INSERT INTO CardChecklistItems (itemId, cardId, [sort], [label], ischecked)
	VALUES (@id, @cardId, @sort, @label, @ischecked)
	SELECT * FROM CardCheckListItems WHERE itemId=@id
