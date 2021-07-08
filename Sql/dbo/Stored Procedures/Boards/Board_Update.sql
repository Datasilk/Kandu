CREATE PROCEDURE [dbo].[Board_Update]
	@boardId int,
	@orgId int,
	@name nvarchar(64),
	@color nvarchar(6) = '',
	@cardtype varchar(16) = ''
AS
	UPDATE Boards SET [name]=@name, [color]=@color, cardtype=@cardtype
	WHERE boardId=@boardId AND orgId=@orgId