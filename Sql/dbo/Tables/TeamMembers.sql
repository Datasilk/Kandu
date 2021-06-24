CREATE TABLE [dbo].[TeamMembers]
(
	[teamId] INT NOT NULL, 
    [userId] INT NOT NULL, 
    [title] NVARCHAR(64) NOT NULL DEFAULT ''
)
