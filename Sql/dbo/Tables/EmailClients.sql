CREATE TABLE [dbo].[EmailClients]
(
	[clientId] INT NOT NULL PRIMARY KEY, 
	[key] VARCHAR(32) NOT NULL, 
    [label] NVARCHAR(64) NOT NULL, 
    [config_json] NVARCHAR(MAX) NOT NULL
)
