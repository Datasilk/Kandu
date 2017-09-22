CREATE PROCEDURE [dbo].[Card_Archive]
	@cardId int
AS
UPDATE Cards SET archived=1 WHERE cardId=@cardId
