CREATE PROCEDURE [dbo].[Organization_Update]
	@orgId int,
	@name nvarchar(64),
	@website nvarchar(255) = '',
	@description nvarchar(MAX) = ''
AS
	UPDATE Organizations SET [name]=@name, [description]=@description, website=@website
	WHERE orgId=@orgId
