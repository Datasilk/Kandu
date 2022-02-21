CREATE PROCEDURE [dbo].[Card_UpdateDueDate]
	@cardId int,
	@userId int,
	@duedate datetime2(7) = NULL
AS
	UPDATE Cards SET datedue=@duedate WHERE cardId=@cardId
