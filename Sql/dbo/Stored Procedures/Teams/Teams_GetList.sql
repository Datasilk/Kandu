CREATE PROCEDURE [dbo].[Teams_GetList]
	@orgId int,
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
		CASE WHEN @orderby = 0 THEN teamId END ASC,
		CASE WHEN @orderby = 1 THEN [name] END DESC,
		CASE WHEN @orderby = 2 THEN datecreated END ASC
		) as rownum, *
		FROM Teams
		WHERE 
		(
			(@orgId > 0 AND orgId = @orgId)
			OR @orgId <= 0
		)
		AND
		(
			(@search <> '' AND [name] LIKE '%' + @search + '%')
			OR @search = ''
		)
		AND
		(
			(@search <> '' AND [description] LIKE '%' + @search + '%')
			OR @search = ''
		)
	) as myTable
	WHERE rownum >= @start AND  rownum <= @start + @length
END