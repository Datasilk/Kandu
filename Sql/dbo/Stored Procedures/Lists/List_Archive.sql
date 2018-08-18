CREATE PROCEDURE [dbo].[List_Archive]
	@listId int
AS
	UPDATE Lists SET archived=1 WHERE listId=@listId
