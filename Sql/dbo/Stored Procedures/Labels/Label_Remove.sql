CREATE PROCEDURE [dbo].[Label_Remove]
	@cardId int,
	@labelId int
AS
	DELETE FROM CardLabels WHERE cardId=@cardId AND labelId=@labelId
