CREATE PROCEDURE [dbo].[Team_Update]
	@teamId int,
	@ownerId int,
	@security bit,
	@name nvarchar(64),
	@website nvarchar(255),
	@description nvarchar(MAX)
AS
	UPDATE Teams SET
	[security]=@security,
	[name]=@name,
	website=@website,
	[description]=@description
	WHERE teamId=@teamId AND ownerId=@ownerId