

CREATE   function SplitArray 
 (
 @text  varchar(8000)
 ,@delimiter varchar(100) = ',' --default to comma delimited.
 )
RETURNS @retTable TABLE 
 (
  Position  int identity(1,1)
 ,valueInt int 
 ,valueNum Numeric(18,3)
 ,value varchar(2000)
 )
AS
/*
********************************************************************************
Purpose: Parse values from a delimited string
  & return the result as an indexed table
Copyright 1996, 1997, 2000, 2003 Clayton Groom (<A href="mailto:Clayton_Groom@hotmail.com">Clayton_Groom@hotmail.com</A>)
Posted to the public domain Aug, 2004
06-17-03 Rewritten as SQL 2000 function.
 Reworked to allow for delimiters > 1 character in length 
 and to convert Text values to numbers
********************************************************************************
*/
BEGIN
 DECLARE @w_Continue  int
  ,@w_StartPos  int
  ,@w_Length  int
  ,@w_Delimeter_pos int
  ,@w_tmp_int  int
  ,@w_tmp_num  numeric(18,3)
  ,@w_tmp_txt   varchar(2000)
  ,@w_Delimeter_Len tinyint
 if len(@text) = 0
 begin
  SET  @w_Continue = 0 -- force early exit
 end 
 else
 begin
 -- parse the original @text array into a temp table
  SET  @w_Continue = 1
  SET @w_StartPos = 1
  SET @text = RTRIM( LTRIM( @text))
  SET @w_Length   = DATALENGTH( RTRIM( LTRIM( @text)))
  SET @w_Delimeter_Len = len(@delimiter)
 end
 WHILE @w_Continue = 1
 BEGIN
  SET @w_Delimeter_pos = CHARINDEX( @delimiter
      ,(SUBSTRING( @text, @w_StartPos
      ,((@w_Length - @w_StartPos) + @w_Delimeter_Len)))
      )
 
  IF @w_Delimeter_pos > 0  -- delimeter(s) found, get the value
  BEGIN
   SET @w_tmp_txt = LTRIM(RTRIM( SUBSTRING( @text, @w_StartPos 
        ,(@w_Delimeter_pos - 1)) ))
   if isnumeric(@w_tmp_txt) = 1
   begin
    set @w_tmp_int = cast( cast(@w_tmp_txt as numeric) as int)
    set @w_tmp_num = cast( @w_tmp_txt as numeric(18,3))
   end
   else
   begin
    set @w_tmp_int =  null
    set @w_tmp_num =  null
   end
   SET @w_StartPos = @w_Delimeter_pos + @w_StartPos + (@w_Delimeter_Len- 1)
  END
  ELSE -- No more delimeters, get last value
  BEGIN
   SET @w_tmp_txt = LTRIM(RTRIM( SUBSTRING( @text, @w_StartPos 
      ,((@w_Length - @w_StartPos) + @w_Delimeter_Len)) ))
   if isnumeric(@w_tmp_txt) = 1
   begin
    set @w_tmp_int = cast( cast(@w_tmp_txt as numeric) as int)
    set @w_tmp_num = cast( @w_tmp_txt as numeric(18,3))
   end
   else
   begin
    set @w_tmp_int =  null
    set @w_tmp_num =  null
   end
   SELECT @w_Continue = 0
  END
  INSERT INTO @retTable VALUES( @w_tmp_int, @w_tmp_num, @w_tmp_txt )
 END
RETURN
END
