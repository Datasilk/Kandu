CREATE TABLE [dbo].[TeamMembers]
(
	[teamId] INT NOT NULL, 
    [userId] INT NOT NULL,
	PRIMARY KEY (teamId, userId)
)
