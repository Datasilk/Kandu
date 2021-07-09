﻿CREATE PROCEDURE [dbo].[Card_UpdateJson]
	@boardId int,
	@cardId int,
	@json nvarchar(MAX)
AS
	UPDATE Cards SET [json]=@json, datemodified=GETUTCDATE() WHERE cardId=@cardId AND boardId=@boardId
	EXEC Board_Modified @boardId=@boardId
