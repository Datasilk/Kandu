CREATE TABLE [dbo].[Boards]
(
	[boardId] INT NOT NULL PRIMARY KEY, 
    [orgId] INT NOT NULL, 
    [archived] BIT NOT NULL DEFAULT 0, 
    [name] NVARCHAR(64) NOT NULL, 
    [color] NVARCHAR(6) NOT NULL DEFAULT '', 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [lastmodified] DATETIME NOT NULL DEFAULT GETDATE(), 
    [type] INT NOT NULL DEFAULT 0 /* 0 = kanban, 1 = timeline, 2 = photo gallery */
)
