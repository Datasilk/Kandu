CREATE PROCEDURE [dbo].[Teams_GetList]
	@orgId int,
	@userId int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @hasAllAccess bit = 0
	IF EXISTS(
		SELECT * FROM [SecurityUsers] su
		JOIN SecurityGroups sg ON sg.groupId = su.groupId
		JOIN Security s ON s.groupId = sg.groupId
		WHERE su.userId = @userId AND sg.orgId=@orgId
		AND (
			s.[key] = 'Owner'
			OR s.[key] = 'TeamsCanViewAll'
		)
	) SET @hasAllAccess = 1


	SELECT DISTINCT t.*, o.[name] AS orgName, sg.[name] AS groupName,
	(SELECT COUNT(*) FROM TeamMembers tm2 WHERE tm2.teamId=t.teamId) AS totalMembers
	FROM Teams t
	LEFT JOIN TeamMembers tm ON tm.teamId=t.teamId AND tm.userId=@userId
	LEFT JOIN Organizations o ON o.orgId=t.orgId
	LEFT JOIN SecurityGroups sg ON sg.groupId = t.groupId
	WHERE 
	(
		(@orgId > 0 AND t.orgId = @orgId)
		OR @orgId <= 0
	)
	AND
	(
		(@hasAllAccess = 0 AND tm.userId IS NOT NULL)
		OR @hasAllAccess = 1
	)
END