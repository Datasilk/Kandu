CREATE PROCEDURE [dbo].[Boards_GetByOrgId]
	@orgId int
AS
	SELECT * FROM Boards WHERE orgId=@orgId
