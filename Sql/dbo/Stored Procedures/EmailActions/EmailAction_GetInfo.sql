CREATE PROCEDURE [dbo].[EmailAction_GetInfo]
	@action varchar(32)
AS
	SELECT ea.*, ec.[key], ec.[label], ec.config_json
	FROM EmailActions ea
	JOIN EmailClients ec ON ec.clientId=ea.clientId
	WHERE ea.[action]=@action