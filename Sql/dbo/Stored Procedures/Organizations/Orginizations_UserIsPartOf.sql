CREATE PROCEDURE [dbo].[Organizations_UserIsPartOf]
	@userId int
AS
	SELECT DISTINCT * FROM Organizations o
	JOIN TeamMembers tm ON tm.userId=@userId
	JOIN Teams t ON t.orgId = o.orgId AND t.teamId=tm.teamId
	WHERE o.orgId=t.orgId
