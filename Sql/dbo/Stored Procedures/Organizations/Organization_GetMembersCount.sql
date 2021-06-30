CREATE PROCEDURE [dbo].[Organization_GetMembersCount]
	@orgId int,
	@page int = 1,
	@length int = 10,
	@search nvarchar(64),
	@excludeTeamId int = NULL
AS
SELECT COUNT(*) FROM (
	SELECT DISTINCT tm.userId FROM Teams t
	JOIN TeamMembers tm ON tm.teamId=t.teamId
	JOIN Users u ON u.userId = tm.userId
	WHERE
	(
		(
			@search IS NOT NULL AND @search != '' AND (
				u.name LIKE '%' + @search + '%'
				OR u.email LIKE '%' + @search + '%'
			)
		)
		OR @search IS NULL OR @search = ''
	)
	AND
	(
		(
			@excludeTeamId IS NOT NULL AND @excludeTeamId > 0 AND (
				t.teamId != @excludeTeamId
			)
			OR @excludeTeamId IS NULL OR @excludeTeamId <= 0
		)
	)
) AS tbl