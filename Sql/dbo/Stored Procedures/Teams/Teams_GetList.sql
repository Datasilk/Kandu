CREATE PROCEDURE [dbo].[Teams_GetList]
	@ownerId int = 0,
	@start int = 1,
	@length int = 20,
	@search nvarchar(MAX) = '',
	@orderby int = 0
AS
BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM (
		SELECT ROW_NUMBER() 
		OVER (ORDER BY
		CASE WHEN @orderby = 0 THEN [name] END DESC,
		CASE WHEN @orderby = 1 THEN [security] END DESC,
		CASE WHEN @orderby = 2 THEN datecreated END ASC
		) as rownum, *
		FROM Teams
		WHERE 
		ownerId = CASE WHEN @ownerId > 0 THEN @ownerId ELSE ownerId END
		AND [name] LIKE CASE WHEN @search <> '' THEN '%' + @search + '%' ELSE [name] END
		AND [website]  LIKE CASE WHEN @search <> '' THEN '%' + @search + '%' ELSE [website] END
		AND [description]  LIKE CASE WHEN @search <> '' THEN '%' + @search + '%' ELSE [description] END
	) as myTable
	WHERE rownum >= @start AND  rownum <= @start + @length
END