CREATE TABLE [dbo].[Boards]
(
	[boardId] INT NOT NULL PRIMARY KEY, 
    [teamId] INT NOT NULL, 
    [favorite] BIT NOT NULL DEFAULT 0, 
    [archived] BIT NOT NULL DEFAULT 0, 
    [name] NVARCHAR(64) NOT NULL, 
    [security] SMALLINT NOT NULL DEFAULT 0, 
    [color] NVARCHAR(6) NOT NULL DEFAULT '', 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)
