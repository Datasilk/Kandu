CREATE PROCEDURE [dbo].[Orginization_GetInfo]
	@orgId int
AS
	SELECT * FROM Organizations WHERE orgId=@orgId
