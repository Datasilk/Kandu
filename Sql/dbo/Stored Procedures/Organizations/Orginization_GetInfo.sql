CREATE PROCEDURE [dbo].[Organization_GetInfo]
	@orgId int
AS
	SELECT * FROM Organizations WHERE orgId=@orgId
