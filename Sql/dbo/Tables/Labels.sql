﻿CREATE TABLE [dbo].[Labels]
(
	[labelId] INT NOT NULL PRIMARY KEY, 
    [label] NVARCHAR(32) NOT NULL, 
    [datecreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
