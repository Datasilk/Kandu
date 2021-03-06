﻿CREATE TABLE [dbo].[Teams]
(
	[teamId] INT NOT NULL PRIMARY KEY, 
    [orgId] INT NOT NULL DEFAULT 0, 
    [ownerId] INT NULL, 
    [groupId] INT NULL, 
    [name] NVARCHAR(64) NOT NULL DEFAULT '', 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [description] NVARCHAR(MAX) NOT NULL DEFAULT ''
)
