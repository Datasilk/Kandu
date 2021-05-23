CREATE PROCEDURE [dbo].[Security_ForUser]
	@orgId int,
	@userId int
AS
	SELECT [key], [enabled] FROM Security
	WHERE orgId=@orgId AND userId=@userId
