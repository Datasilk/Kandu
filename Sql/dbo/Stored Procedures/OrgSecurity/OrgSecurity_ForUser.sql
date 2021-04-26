CREATE PROCEDURE [dbo].[OrgSecurity_ForUser]
	@orgId int,
	@userId int
AS
	SELECT [key], [enabled] FROM OrgSecurity
	WHERE orgId=@orgId AND userId=@userId
