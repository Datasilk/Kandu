CREATE VIEW [dbo].[View_LogBoard]
AS
SELECT l.*, u.[name], u.email, u.photo
FROM LogBoard l
LEFT JOIN Users u ON u.userId=l.userId
