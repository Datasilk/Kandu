CREATE PROCEDURE [dbo].[Board_MemberExists]
	@userId int,
	@boardId int,
	@security nvarchar(MAX) = ''
AS
	IF 
		EXISTS (SELECT * FROM BoardMembers
		WHERE userId=@userId AND boardId=@boardId)
	OR
		EXISTS (SELECT * FROM BoardTeams bt
		JOIN TeamMembers tm ON tm.teamId = bt.teamId
		WHERE tm.userId=@userId AND bt.boardId=@boardId)
	OR
		EXISTS(SELECT * FROM [Security] s
		JOIN SecurityGroups sg ON sg.groupId= s.groupId
		JOIN SecurityUsers su ON su.groupId = sg.groupId
		JOIN Boards b ON b.orgId=sg.orgId AND b.boardId=@boardId
		WHERE su.userId = @userId
		AND (s.[key] IN ('Owner', 'BoardsCanViewAll') OR (@security != '' AND s.[key] = @security)))
		
	BEGIN
		SELECT 1
	END ELSE BEGIN
		SELECT 0
	END
