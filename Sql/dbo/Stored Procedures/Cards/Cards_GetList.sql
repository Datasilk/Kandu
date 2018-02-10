CREATE PROCEDURE [dbo].[Cards_GetList]
	@boardId int = 0,
	@listId int = 0,
	@start int = 1,
	@length int = 2000,
	@archivedOnly bit = 0
AS
BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM (
		SELECT ROW_NUMBER() 
		OVER (ORDER BY listId, sort ASC) 
		AS rownum, *
		FROM Cards
		WHERE boardId=@boardId
		AND listId = CASE WHEN @listId > 0 THEN @listId ELSE listId END
		AND archived=@archivedOnly
	) as myTable
	WHERE rownum >= @start AND  rownum <= @start + @length
END