CREATE PROCEDURE [dbo].[Board_MemberExists]
	@userId int,
	@boardId int
AS
	IF (SELECT COUNT(*) FROM BoardMembers WHERE boardId=@boardId AND userId=@userId) = 1 BEGIN
		SELECT 1
	END ELSE BEGIN
		IF (SELECT COUNT(*) FROM TeamMembers 
			WHERE teamId IN (SELECT teamId FROM Boards WHERE boardId=@boardId) 
			AND userId=@userId) = 1 
		BEGIN
			SELECT 1
		END ELSE BEGIN
			SELECT 0
		END
	END
