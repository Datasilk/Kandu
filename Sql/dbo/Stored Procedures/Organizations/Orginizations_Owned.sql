CREATE PROCEDURE [dbo].[Organizations_Owned]
	@ownerId int
AS
	SELECT * FROM Organizations WHERE ownerId=@ownerId
