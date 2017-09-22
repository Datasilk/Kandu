CREATE PROCEDURE [dbo].[Users_GetList]
	@userId int = 0, 
	@start int = 1,
	@length int = 10,
	@search nvarchar(MAX) = '',
	@orderby int = 1
AS
BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM (
		SELECT ROW_NUMBER() 
		OVER (ORDER BY
		CASE WHEN @orderby = 0 THEN [name] END DESC,
		CASE WHEN @orderby = 1 THEN email END DESC,
		CASE WHEN @orderby = 2 THEN datecreated END ASC
		) as rownum, *
		FROM Users
		WHERE 
		userId = CASE WHEN @userId > 0 THEN @userId ELSE userId END
		AND [name] LIKE CASE WHEN @search <> '' THEN '%' + @search + '%' ELSE [name] END
		AND email  LIKE CASE WHEN @search <> '' THEN '%' + @search + '%' ELSE email END
	) as myTable
	WHERE rownum >= @start AND  rownum <= @start + @length
END