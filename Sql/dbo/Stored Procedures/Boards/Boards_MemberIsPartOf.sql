CREATE PROCEDURE [dbo].[Boards_MemberIsPartOf]
	@userId int
AS
	SELECT boardId FROM BoardTeams bt
	JOIN TeamMembers tm ON tm.teamId = bt.teamId
	WHERE tm.userId=@userId
