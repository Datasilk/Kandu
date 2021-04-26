CREATE PROCEDURE [dbo].[Team_Get]
	@orgId int,
	@teamId int
AS
	SELECT * FROM Teams WHERE teamId=@teamId AND orgId=@orgId