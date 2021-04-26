CREATE PROCEDURE [dbo].[Orginizations_Owned]
	@ownerId int
AS
	SELECT * FROM Organizations WHERE ownerId=@ownerId
