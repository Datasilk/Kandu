CREATE PROCEDURE [dbo].[Organization_UpdateSettings]
	@orgId int,
	@groupId int,
	@cardtype varchar(16)
AS
	UPDATE Organizations SET groupId=@groupId, cardtype=@cardtype
	WHERE orgId=@orgId
