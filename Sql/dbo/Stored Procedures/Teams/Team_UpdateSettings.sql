CREATE PROCEDURE [dbo].[Team_UpdateSettings]
	@teamId int,
	@orgId int,
	@groupId int
AS
	UPDATE Teams SET
	[groupId] = @groupId
	WHERE teamId=@teamId AND orgId=@orgId
