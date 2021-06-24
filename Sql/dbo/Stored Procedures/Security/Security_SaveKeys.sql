CREATE PROCEDURE [dbo].[Security_SaveKeys]
	@orgId int,
	@groupId int,
	@keys XML 
	/* example:	
		<keys>
			<key name="manage-team" value="1" scope="3" scopeid="15"></key>
			<key name="manage-security" value="1"></key>
			<key name="upload" value="0"></key>
		</keys>
	*/
AS
	DECLARE @hdoc INT
	DECLARE @newkeys TABLE (
		[key] varchar(32),
		[value] bit,
		[scope] int,
		[scopeId] int
	)
	EXEC sp_xml_preparedocument @hdoc OUTPUT, @keys;

	INSERT INTO @newkeys
	SELECT x.[key], x.[value], x.[scope], x.[scopeId]
	FROM (
		SELECT * FROM OPENXML( @hdoc, '//key', 2)
		WITH (
			[key] varchar(32) '@name',
			[value] bit '@value',
			[scope] int '@scope',
			[scopeId] int '@scopeid'
		)
	) AS x
	
	DECLARE @cursor CURSOR 
	DECLARE @key varchar(32), @value bit, @scope int, @scopeId int
	SET @cursor = CURSOR FOR
	SELECT * FROM @newkeys
	FETCH NEXT FROM @cursor INTO @key, @value, @scope, @scopeId
	WHILE @@FETCH_STATUS = 0 BEGIN
		IF EXISTS(SELECT * FROM Security WHERE orgId=@orgId AND groupId=@groupId AND [key]=@key) BEGIN
			UPDATE Security SET [enabled] = @value, scope=@scope, scopeId=@scopeId WHERE orgId=orgId AND groupId=@groupId AND [key]=@key
		END ELSE BEGIN
			INSERT INTO Security (orgId, groupId, [key], [enabled], scope, scopeId) 
			VALUES (@orgId, @groupId, @key, @value, @scope, @scopeId)
		END
		FETCH NEXT FROM @cursor INTO @key, @value, @scope, @scopeId
	END
	CLOSE @cursor
	DEALLOCATE @cursor
