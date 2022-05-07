CREATE PROCEDURE [dbo].[Organization_UpdateCustomCss]
	@orgId int,
	@customCss bit
AS
	UPDATE Organizations SET customCss=@customCss WHERE orgId=@orgId
