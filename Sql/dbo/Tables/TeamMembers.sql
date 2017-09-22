CREATE TABLE [dbo].[TeamMembers]
(
	[teamId] INT NOT NULL, 
    [userId] INT NOT NULL, 
    [security] INT NOT NULL DEFAULT 0
)
