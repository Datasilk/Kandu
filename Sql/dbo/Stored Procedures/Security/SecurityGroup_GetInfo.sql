CREATE PROCEDURE [dbo].[SecurityGroup_GetInfo]
	@groupId int
AS
	SELECT * FROM SecurityGroups WHERE groupId=@groupId
