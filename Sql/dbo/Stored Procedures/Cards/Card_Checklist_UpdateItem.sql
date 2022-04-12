CREATE PROCEDURE [dbo].[Card_Checklist_UpdateItem]
	@itemId int,
	@userId int,
	@cardId int,
	@sort int,
	@label nvarchar(255),
	@ischecked bit
AS
	IF (@sort < 0) BEGIN
		SELECT @sort = CASE WHEN COUNT(*) > ISNULL(MAX(sort), 0) THEN COUNT(*) ELSE ISNULL(MAX(sort), 0) END + 1
		FROM CardChecklistItems
		WHERE cardId=@cardId
	END
	IF (@label = '') BEGIN
		DECLARE @index int = 1
		SELECT @index = CASE WHEN COUNT(*) > ISNULL(MAX(sort), 0) THEN COUNT(*) ELSE ISNULL(MAX(sort), 0) END + 1
		FROM CardChecklistItems
		WHERE cardId=@cardId
		SET @label = 'Checklist Item #' + CAST(@index AS varchar(32))
	END
	UPDATE CardChecklistItems 
	SET [sort] = @sort, [label]=@label, checked=@ischecked
	WHERE itemId=@itemId AND cardId=@cardId
