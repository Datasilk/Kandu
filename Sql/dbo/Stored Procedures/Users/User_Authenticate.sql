CREATE PROCEDURE [dbo].[User_Authenticate] 
	@email nvarchar(64) = '',
	@password nvarchar(255) = ''
AS
BEGIN
	SELECT u.*, (
		SELECT TOP 1 orgId FROM Organizations o
		WHERE o.ownerId=u.userId ORDER BY datecreated
	) AS orgId,
	b.name AS lastboardName
	FROM Users u
	LEFT JOIN Boards b ON b.boardId = u.lastboard
	WHERE email=@email AND [password]=@password
END