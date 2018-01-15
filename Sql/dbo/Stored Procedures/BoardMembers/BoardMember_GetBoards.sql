CREATE PROCEDURE [dbo].[BoardMember_GetBoards]
	@userId int
AS
	SELECT boardId FROM BoardMembers WHERE userId=@userId
