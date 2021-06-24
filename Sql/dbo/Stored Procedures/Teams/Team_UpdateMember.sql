CREATE PROCEDURE [dbo].[Team_UpdateMember]
	@teamId int,
	@userId int,
	@title nvarchar(64)
AS
	UPDATE TeamMembers SET title=@title WHERE teamId=@teamId AND userId=@userId
