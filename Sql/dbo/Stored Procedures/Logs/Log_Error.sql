CREATE PROCEDURE [dbo].[Log_Error]
	@userId int,
	@url nvarchar(255),
	@area varchar(64),
	@message varchar(512),
	@stacktrace varchar(MAX)
AS
	INSERT INTO Log_Errors (datecreated, userId, url, area, message, stacktrace)
	VALUES (GETUTCDATE(), @userId, @url, @area, @message, @stacktrace)
