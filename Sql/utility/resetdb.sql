/* Clear initial data (if you so desire) */
DECLARE @cursor CURSOR, @sql nvarchar(MAX)
SET @cursor = CURSOR FOR
SELECT
	'ALTER SEQUENCE '
+  QUOTENAME(schema_name(schema_id))
+  '.'
+  QUOTENAME(name)
+  ' RESTART WITH '
+  TRY_CONVERT(nvarchar(50),[start_value])
AS [QUERY]
FROM sys.sequences
OPEN @cursor
FETCH FROM @cursor INTO @sql
WHILE @@FETCH_STATUS = 0 BEGIN
	EXEC sp_executesql @sql
	FETCH FROM @cursor INTO @sql
END
CLOSE @cursor
DEALLOCATE @cursor
	
EXEC sp_MSForEachTable 'TRUNCATE TABLE ?'