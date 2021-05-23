CREATE PROCEDURE [dbo].[Security_SaveKeys]
	@orgId int,
	@userId int,
	@keys XML 
	/* example:	
		<keys>
			<key name="manage-team" value="1"></key>
			<key name="manage-security" value="1"></key>
			<key name="upload" value="0"></key>
		</keys>
	*/
AS
	DECLARE @hdoc INT
	DECLARE @newkeys TABLE (
		[key] varchar(32),
		[value] bit
	)
	EXEC sp_xml_preparedocument @hdoc OUTPUT, @keys;

	INSERT INTO @newkeys
	SELECT x.[key], x.[value]
	FROM (
		SELECT * FROM OPENXML( @hdoc, '//key', 2)
		WITH (
			[key] varchar(32) '@name',
			[value] bit '@value'
		)
	) AS x
	
	DECLARE @cursor CURSOR 
	DECLARE @key varchar(32), @value bit
	SET @cursor = CURSOR FOR
	SELECT * FROM @newkeys
	FETCH NEXT FROM @cursor INTO @key, @value
	WHILE @@FETCH_STATUS = 0 BEGIN
		IF EXISTS(SELECT * FROM Security WHERE orgId=@orgId AND userId=@userId AND [key]=@key) BEGIN
			UPDATE Security SET [enabled] = @value WHERE orgId=orgId AND userId=@userId AND [key]=@key
		END ELSE BEGIN
			INSERT INTO Security (orgId, userId, [key], [enabled]) 
			VALUES (@orgId, @userId, @key, @value)
		END
		FETCH NEXT FROM @cursor INTO @key, @value
	END
	CLOSE @cursor
	DEALLOCATE @cursor
