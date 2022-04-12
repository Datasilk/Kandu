CREATE PROCEDURE [dbo].[Card_Checklist_UpdateItemLabel]
	@itemId int,
	@userId int,
	@cardId int,
	@label nvarchar(255)
AS
	IF (@label = '') BEGIN
		DECLARE @index int = 1
		SELECT @index = CASE WHEN COUNT(*) > ISNULL(MAX(sort), 0) THEN COUNT(*) ELSE ISNULL(MAX(sort), 0) END + 1
		FROM CardChecklistItems
		WHERE cardId=@cardId
		SET @label = 'Checklist Item #' + CAST(@index AS varchar(32))
	END
	UPDATE CardChecklistItems 
	SET [label]=@label
	WHERE itemId=@itemId AND cardId=@cardId
