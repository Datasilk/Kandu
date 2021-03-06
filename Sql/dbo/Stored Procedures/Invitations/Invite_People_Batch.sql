﻿CREATE PROCEDURE [dbo].[Invite_People_Batch]
	@invitedBy int = 0, -- userId
	@scopeId int = 0,
	@scope int = 0,
	@message nvarchar(MAX),
	@invites XML 
	/* example:	
		<invites>
			<invite id="837" email="" publickey="98efkjhe4r7itfwkgjvmnbv98ywe"></invite>
			<invite id="1936" email="" publickey="hgjdsfv78y6r43tpioefwkjhvfbnjbf"></invite>
			<invite id="" email="samantha.dreamer@mars.com" publickey="78y64hkjsvd098wjhgvtu76"></invite>
		</invites>
	*/
AS
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
	@userId int, @email nvarchar(64), @publickey varchar(16), @failed varchar(MAX)
	SET @cursor = CURSOR FOR
	SELECT userId, email, publickey FROM @cols
	OPEN @cursor
	FETCH FROM @cursor INTO @userId, @email, @publickey
	WHILE @@FETCH_STATUS = 0 BEGIN
		BEGIN TRY
			INSERT INTO Invitations (userId, scopeId, scope, email, publickey, [message], invitedBy) 
			VALUES (@userId, @scopeId, @scope, @email, @publickey, @message, @invitedBy)
		END TRY
		BEGIN CATCH
			SET @failed = @failed + (CASE WHEN @userId > 0 THEN CONVERT(varchar(16), @userId) ELSE @email END) + ','
		END CATCH
		FETCH FROM @cursor INTO @userId, @email, @publickey
	END
	CLOSE @cursor
	DEALLOCATE @cursor

	SELECT @failed
