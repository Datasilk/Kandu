CREATE PROCEDURE [dbo].[Orginization_Disable]
	@orgId int
AS
	UPDATE Organizations SET enabled=0 WHERE orgId=@orgId
