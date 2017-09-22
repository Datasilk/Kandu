CREATE PROCEDURE [dbo].[Team_Get]
	@teamId int
AS
	SELECT * FROM Teams WHERE teamId=@teamId