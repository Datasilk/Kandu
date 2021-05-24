CREATE PROCEDURE [dbo].[Team_UpdateMember]
	@teamId int,
	@userId int,
	@roleId int
AS
	UPDATE TeamMembers SET roleId=@roleId WHERE teamId=@teamId AND userId=@userId
