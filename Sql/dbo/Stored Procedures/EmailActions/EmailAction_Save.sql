CREATE PROCEDURE [dbo].[EmailAction_Save]
	@action varchar(32),
	@clientId int,
	@subject nvarchar(255),
	@fromName nvarchar(64),
	@fromAddress nvarchar(64),
	@bodyText nvarchar(MAX),
	@bodyHtml nvarchar(MAX)
AS
	IF EXISTS(SELECT * FROM EmailActions WHERE [action]=@action) BEGIN
		UPDATE EmailActions 
		SET clientId=@clientId, [subject]=@subject, fromName=@fromName, fromAddress=@fromAddress, bodyText=@bodyText, bodyHtml=@bodyHtml
		WHERE [action]=@action
	END ELSE BEGIN
		INSERT INTO EmailActions ([action], clientId, [subject], fromName, fromAddress, bodyText, bodyHtml) 
		VALUES (@action, @clientId, @subject, @fromName, @fromAddress, @bodyText, @bodyHtml)
	END