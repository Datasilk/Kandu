CREATE PROCEDURE [dbo].[Card_Attachment_Add]
	@userId int,
	@cardId int,
	@files XML 
	/* example:	
		<attachments>
			<file filename="my-new-puppy-2022-02-22-02787.jpg"></file>
			<file filename="vr-assetto-corsa_0289733.jpg"></file>
			<file filename="artwork-rem-rezero-2022.png"></file>
		</attachments>
	*/
AS
	DECLARE @hdoc INT
	DECLARE @cols TABLE (
		[filename] nvarchar(64)
	)

	EXEC sp_xml_preparedocument @hdoc OUTPUT, @files;

	INSERT INTO @cols
	SELECT x.[filename]
	FROM (
		SELECT * FROM OPENXML( @hdoc, '//file', 2)
		WITH (
			[filename] nvarchar(64) '@filename'
		)
	) AS x

	DECLARE @cursor CURSOR, @filename nvarchar(64)
	DECLARE @attachmentId int
	SET @cursor = CURSOR FOR
	SELECT [filename] FROM @cols
	OPEN @cursor
	FETCH FROM @cursor INTO @filename
	WHILE @@FETCH_STATUS = 0 BEGIN
		SET @attachmentId = NEXT VALUE FOR SequenceAttachments
		INSERT INTO CardAttachments (attachmentId, cardId, userId, [filename], datecreated)
		VALUES (@attachmentId, @cardId, @userId, @filename, GETUTCDATE())
		FETCH FROM @cursor INTO @filename
	END
	CLOSE @cursor
	DEALLOCATE @cursor