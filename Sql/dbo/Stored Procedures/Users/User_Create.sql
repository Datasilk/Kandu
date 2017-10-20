CREATE PROCEDURE [dbo].[User_Create]
	@name nvarchar(64),
	@email nvarchar(64),
	@password nvarchar(255),
	@photo bit = 0
AS
	DECLARE @id int = NEXT VALUE FOR SequenceUsers
	INSERT INTO Users (userId, [name], email, [password], photo, datecreated)
	VALUES (@id, @name, @email, @password, @photo, GETDATE())

	/* Create a Team for new User */
	IF OBJECT_ID('tempdb.dbo.#tmp') IS NOT NULL DROP TABLE #tmp
	CREATE TABLE #tmp (id int)
	DECLARE @teamId int = 0
	INSERT INTO #tmp EXEC Team_Create @ownerId=@id, @name=@name, @security=1
	SELECT @teamId = id FROM #tmp
	DELETE FROM #tmp

	/* Create record for Team Member */
	INSERT INTO TeamMembers (teamId, userId, [security])
	VALUES (@teamId, @id, 1)

	DROP TABLE #tmp
	
	SELECT @id