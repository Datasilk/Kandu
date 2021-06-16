CREATE PROCEDURE [dbo].[Security_UpdateGroup]
	@groupId int,
	@name nvarchar(32)
AS
	UPDATE SecurityGroups SET [name]=@name WHERE groupId=@groupId
