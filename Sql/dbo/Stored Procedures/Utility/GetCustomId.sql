CREATE PROCEDURE [dbo].[GetCustomID]
	@length int = 5,
	@chars nvarchar(MAX) = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789',
	@pattern nvarchar(MAX) = '?????',
	@id nvarchar(MAX) OUTPUT
AS
	DECLARE 
	@count int = 0,
	@result nvarchar(MAX) = '',
	@charlen int = Len(@chars),
	@found bit = 0,
	@patternchar nvarchar(1),
	@newchar nvarchar(1)

	WHILE (@count < @length) BEGIN
		SET @found = 0
		WHILE(@found = 0) BEGIN
			SELECT @patternchar = SUBSTRING(@pattern, @count + 1, 1)
			SELECT @newchar = SUBSTRING(@chars, CONVERT(int, RAND() * @charlen), 1)
			if(@patternchar = '?') BEGIN SET @found = 1 END
			if(@patternchar = 'A') BEGIN 
				IF(@newchar LIKE '[A-Za-z]') BEGIN SET @found = 1 END
			END
			if(@patternchar = '#') BEGIN 
				IF(@newchar LIKE '[0-9]') BEGIN SET @found = 1 END
			END
		END
		SELECT @result = @result + @newchar
		SELECT @count = @count + 1
	END

	SELECT @id = @result
