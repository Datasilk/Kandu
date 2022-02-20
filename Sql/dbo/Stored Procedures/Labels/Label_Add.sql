CREATE PROCEDURE [dbo].[Label_Add]
	@cardId int,
	@label nvarchar(32)
AS
	DECLARE @id int
	SELECT @id = labelId FROM Labels WHERE [label]=@label
	IF @id IS NULL BEGIN
		SET @id = NEXT VALUE FOR SequenceLabels
		INSERT INTO Labels (labelId, [label]) VALUES (@id, @label)
	END
	INSERT INTO CardLabels (cardId, labelId) VALUES (@cardId, @id)
