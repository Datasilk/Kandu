CREATE TABLE [dbo].[Invitations]
(
	[userId] INT NOT NULL DEFAULT 0 , 
    [scopeId] INT NOT NULL DEFAULT 0, 
    [scope] INT NOT NULL DEFAULT 0, 
    [email] NVARCHAR(64) NOT NULL DEFAULT '',
    [publickey] VARCHAR(16) NULL , --if the invitation requires a key to authenticate the validity of the invitation
    [datecreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [invitedBy] INT NOT NULL, 
    [message] NVARCHAR(MAX) NULL, 
    PRIMARY KEY ([userId], [scopeId], [scope], [email])
)
