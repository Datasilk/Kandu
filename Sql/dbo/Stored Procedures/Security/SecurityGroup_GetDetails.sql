CREATE PROCEDURE [dbo].[SecurityGroup_GetDetails]
	@groupId int
AS
	SELECT * FROM SecurityGroups WHERE groupId=@groupId
	SELECT * FROM [Security] WHERE groupId=@groupId
	SELECT u.* FROM SecurityUsers su
	JOIN Users u ON u.userId = su.userId
	WHERE su.groupId=@groupId
