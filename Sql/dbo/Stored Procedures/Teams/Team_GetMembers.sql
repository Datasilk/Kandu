CREATE PROCEDURE [dbo].[Team_GetMembers]
	@teamId int
AS
	SELECT u.*, tm.title FROM TeamMembers tm
	JOIN Users u ON u.userId=tm.userId
	WHERE tm.teamId=@teamId
