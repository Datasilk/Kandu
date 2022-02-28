CREATE PROCEDURE [dbo].[Invite_Accept]
	@email nvarchar(MAX),
	@publickey varchar(16)
AS
	SELECT * FROM Invitations WHERE email=@email AND publickey=@publickey
	UPDATE Invitations SET accepted=GETUTCDATE() WHERE email=@email AND publickey=@publickey
	DELETE FROM Invitations WHERE accepted IS NOT NULL AND accepted > DATEADD(DAY, 1, GETUTCDATE())