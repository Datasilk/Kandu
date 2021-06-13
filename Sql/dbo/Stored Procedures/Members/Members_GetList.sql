CREATE PROCEDURE [dbo].[Members_GetList]
	@orgId int
AS
	SELECT DISTINCT u.*
	FROM Teams t
	JOIN TeamMembers tm ON tm.teamId = t.teamId
	JOIN Users u ON u.userId = tm.userId
	WHERE t.orgId=@orgId
	UNION
	SELECT u.*
	FROM Users u
	JOIN Organizations o ON o.ownerId=u.userId AND o.ownerId=@orgId
