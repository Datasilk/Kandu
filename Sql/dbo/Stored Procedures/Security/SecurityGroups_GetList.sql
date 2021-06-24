CREATE PROCEDURE [dbo].[SecurityGroups_GetList]
	@orgId int
AS
	SELECT sg.*, (SELECT COUNT(*) FROM [Security] WHERE groupId=sg.groupId) AS totalkeys
	FROM SecurityGroups sg WHERE orgId=@orgId