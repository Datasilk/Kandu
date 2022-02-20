CREATE PROCEDURE [dbo].[Card_UpdateAssignedTo]
	@cardId int,
	@userId int, --user who assigned the user to the card
	@userIdAssigned int
AS
	UPDATE cards SET userIdAssigned=@userIdAssigned WHERE cardId=@cardId