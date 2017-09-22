CREATE PROCEDURE [dbo].[Card_Restore]
	@cardId int
AS
UPDATE Cards SET archived=0 WHERE cardId=@cardId
