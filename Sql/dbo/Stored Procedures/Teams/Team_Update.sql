CREATE PROCEDURE [dbo].[Team_Update]
	@teamId int,
	@orgId int,
	@name nvarchar(64),
	@description nvarchar(MAX)
AS
	UPDATE Teams SET
	[name]=@name,
	[description]=@description
	WHERE teamId=@teamId AND orgId=@orgId