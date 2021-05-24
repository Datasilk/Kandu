CREATE TABLE [dbo].[TeamMembers]
(
	[teamId] INT NOT NULL, 
    [userId] INT NOT NULL, 
    [roleId] INT NOT NULL DEFAULT 0
)
