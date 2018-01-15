CREATE VIEW [dbo].[View_Boards]
	AS 
	SELECT b.*, 
	t.[name] AS teamName, t.[description] AS teamDescription, t.[security] AS teamSecurity, t.website AS teamWebsite,
	u.userId AS ownerId, u.[name] AS ownerName

	FROM Boards b
	LEFT JOIN Teams t ON t.teamId = b.teamId
	LEFT JOIN Users u ON u.userId = t.ownerId
