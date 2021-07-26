CREATE PROCEDURE [dbo].[EmailClients_GetList]
AS
	SELECT * FROM EmailClients ORDER BY [key] ASC