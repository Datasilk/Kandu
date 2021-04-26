CREATE PROCEDURE [dbo].[Board_Update]
	@boardId int,
	@orgId int,
	@name nvarchar(64),
	@color nvarchar(6) = ''
AS
	UPDATE Boards SET [name]=@name, [color]=@color
	WHERE boardId=@boardId AND orgId=@orgId