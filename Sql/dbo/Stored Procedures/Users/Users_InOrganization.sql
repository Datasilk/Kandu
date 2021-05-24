CREATE PROCEDURE [dbo].[Users_InOrganization]
	@orgId int
AS
	SELECT u.* FROM Teams t
	JOIN TeamMembers tm ON tm.teamId = t.teamId
	JOIN Users u ON u.userId = tm.userId
	WHERE t.orgId = @orgId
	ORDER BY u.[name] ASC
