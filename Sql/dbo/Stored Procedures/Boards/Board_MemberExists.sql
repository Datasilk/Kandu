CREATE PROCEDURE [dbo].[Board_MemberExists]
	@userId int,
	@boardId int
AS
	IF (SELECT COUNT(*) FROM TeamMembers tm
		JOIN BoardTeams bt ON bt.teamId=tm.teamId AND bt.boardId=@boardId
	WHERE userId=@userId) = 1 BEGIN
		SELECT 1
	END ELSE BEGIN
		SELECT 0
	END
