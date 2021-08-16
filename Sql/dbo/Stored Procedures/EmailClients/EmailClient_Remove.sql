CREATE PROCEDURE [dbo].[EmailClient_Remove]
	@clientId int
AS
	DELETE FROM EmailActions WHERE clientId=@clientId
	DELETE FROM EmailClients WHERE clientId=@clientId
