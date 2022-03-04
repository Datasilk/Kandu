CREATE TABLE [dbo].[Log_Cards_Data]
(
	[logId] INT NOT NULL PRIMARY KEY,
	[data] NVARCHAR(MAX) NOT NULL -- name, card type, colors, description
)
