CREATE PROCEDURE [dbo].[Organization_UpdateCustomJs]
	@orgId int,
	@customJs bit
AS
	UPDATE Organizations SET customJs=@customJs WHERE orgId=@orgId
