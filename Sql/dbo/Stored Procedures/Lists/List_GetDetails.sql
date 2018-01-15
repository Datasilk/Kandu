CREATE PROCEDURE [dbo].[List_GetDetails]
	@listId int
AS
	SELECT * FROM Lists WHERE listId=@listId
