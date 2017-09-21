CREATE TABLE [dbo].[Users]
(
	[userId] INT NOT NULL PRIMARY KEY, 
    [email] NVARCHAR(50) NOT NULL, 
    [password] NVARCHAR(255) NOT NULL DEFAULT '', 
    [photo] BIT NOT NULL DEFAULT 0, 
    [datecreated] DATETIME NOT NULL DEFAULT GETDATE()
)
