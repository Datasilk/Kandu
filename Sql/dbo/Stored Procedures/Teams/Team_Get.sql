CREATE PROCEDURE [dbo].[Team_Get]
	@ownerId int,
	@teamId int
AS
	SELECT * FROM Teams WHERE teamId=@teamId AND ownerId=@ownerId