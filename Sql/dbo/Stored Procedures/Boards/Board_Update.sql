CREATE PROCEDURE [dbo].[Board_Update]
	@boardId int,
	@teamId int,
	@name nvarchar(64),
	@color nvarchar(6) = ''
AS
	UPDATE Boards SET teamId=@teamId, [name]=@name, [color]=@color
	WHERE boardId=@boardId