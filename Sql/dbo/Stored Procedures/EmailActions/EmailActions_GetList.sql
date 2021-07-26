CREATE PROCEDURE [dbo].[EmailActions_GetList]
AS
	SELECT ea.*, ec.[key], ec.[label], ec.config_json
	FROM EmailActions ea
	JOIN EmailClients ec ON ec.clientId=ea.clientId
	ORDER BY ec.[key] ASC