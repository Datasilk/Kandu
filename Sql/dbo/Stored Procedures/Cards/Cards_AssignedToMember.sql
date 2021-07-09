CREATE PROCEDURE [dbo].[Cards_AssignedToMember]
	@userId int,
	@orgId int,
	@start int = 1,
	@length int = 20,
	@archivedOnly bit = 0
AS
BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM (
		SELECT 
		ROW_NUMBER() OVER (ORDER BY datemodified DESC) AS rownum, 
		c.*
		FROM Cards c
		JOIN Boards b ON b.boardId=c.boardId AND (@orgId IS NULL OR @orgId = 0 OR b.orgId=@orgId)
		WHERE c.userId=@userId
		AND c.archived=@archivedOnly
	) as myTable
	WHERE rownum >= @start AND  rownum <= @start + @length
END