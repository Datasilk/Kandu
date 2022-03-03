CREATE PROCEDURE [dbo].[Invite_People_Batch]
	@invitedBy int = 0, -- userId
	@scopeId int = 0,
	@scope int = 0,
	@keys varchar(MAX) = '',
	@invites XML 
	/* example:	
		<invites>
			<invite id="837" email="" publickey="98efkjhe4r7itfwkgjvmnbv98ywe"></invite>
			<invite id="1936" email="" publickey="hgjdsfv78y6r43tpioefwkjhvfbnjbf"></invite>
			<invite id="" email="samantha.dreamer@mars.com" publickey="78y64hkjsvd098wjhgvtu76"></invite>
		</invites>
	*/
AS
SET NOCOUNT ON

	DECLARE @hdoc INT
	DECLARE @cols TABLE (
		[userId] int,
		[email] nvarchar(64),
		publickey varchar(16)
	)

	EXEC sp_xml_preparedocument @hdoc OUTPUT, @invites;

	/* create new addressbook entries based on email list */
	INSERT INTO @cols
	SELECT x.[id] AS userId, x.[email], x.publickey
	FROM (
		SELECT * FROM OPENXML( @hdoc, '//invite', 2)
		WITH (
			[id] int '@id',
			[email] nvarchar(64) '@email',
			[publickey] varchar(16) '@publickey'
		)
	) AS x

	DECLARE @cursor CURSOR,
	@userId int, @email nvarchar(64), @publickey varchar(16)
	DECLARE @failed TABLE (
		email nvarchar(64),
		[name] nvarchar(64)
	)
	SET @cursor = CURSOR FOR
	SELECT userId, email, publickey FROM @cols
	OPEN @cursor
	FETCH FROM @cursor INTO @userId, @email, @publickey
	WHILE @@FETCH_STATUS = 0 BEGIN
		BEGIN TRY
			INSERT INTO Invitations (userId, scopeId, scope, email, publickey, invitedBy, keys) 
			VALUES (@userId, @scopeId, @scope, @email, @publickey, @invitedBy, @keys)
		END TRY
		BEGIN CATCH
			INSERT INTO @failed (email, [name]) VALUES (
				CASE WHEN @userId > 0 THEN (SELECT email FROM Users WHERE userId=@userId) ELSE @email END,
				CASE WHEN @userId > 0 THEN (SELECT [name] FROM Users WHERE userId=@userId) ELSE @email END
			)
		END CATCH
		FETCH FROM @cursor INTO @userId, @email, @publickey
	END
	CLOSE @cursor
	DEALLOCATE @cursor

	SELECT * FROM @failed
