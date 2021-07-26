CREATE PROCEDURE [dbo].[EmailClient_GetConfig]
	@clientId varchar(32)
AS
	SELECT * FROM EmailClients WHERE clientId=@clientId
