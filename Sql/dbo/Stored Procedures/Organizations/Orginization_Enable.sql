CREATE PROCEDURE [dbo].[Organization_Enable]
	@orgId int
AS
	UPDATE Organizations SET enabled=1 WHERE orgId=@orgId