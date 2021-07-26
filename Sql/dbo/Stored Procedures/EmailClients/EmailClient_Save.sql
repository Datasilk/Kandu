CREATE PROCEDURE [dbo].[EmailClient_Save]
	@clientId int,
	@key varchar(32),
	@label nvarchar(32),
	@config nvarchar(MAX)
AS
	IF @clientId IS NOT NULL AND @clientId > 0 AND EXISTS(SELECT * FROM EmailClients WHERE clientId=@clientId) BEGIN
		UPDATE EmailClients SET [key]=@key, [label]=@label, config_json=@config WHERE clientId=@clientId
	END ELSE BEGIN
		SET @clientId = NEXT VALUE FOR SequenceEmailClients
		INSERT INTO EmailClients (clientId, [key], [label], config_json) VALUES (@clientId, @key, @label, @config)
	END