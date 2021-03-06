﻿/*
Deployment script for Kandu

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;




GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET RECOVERY FULL 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET TEMPORAL_HISTORY_RETENTION ON 
            WITH ROLLBACK IMMEDIATE;
    END


GO
PRINT N'Dropping unnamed constraint on [dbo].[Boards]...';


GO
ALTER TABLE [dbo].[Boards] DROP CONSTRAINT [DF__Boards__favorite__44FF419A];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Boards]...';


GO
ALTER TABLE [dbo].[Boards] DROP CONSTRAINT [DF__Boards__archived__45F365D3];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Boards]...';


GO
ALTER TABLE [dbo].[Boards] DROP CONSTRAINT [DF__Boards__security__46E78A0C];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Boards]...';


GO
ALTER TABLE [dbo].[Boards] DROP CONSTRAINT [DF__Boards__color__47DBAE45];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Boards]...';


GO
ALTER TABLE [dbo].[Boards] DROP CONSTRAINT [DF__Boards__datecrea__48CFD27E];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Boards]...';


GO
ALTER TABLE [dbo].[Boards] DROP CONSTRAINT [DF__Boards__lastmodi__49C3F6B7];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Boards]...';


GO
ALTER TABLE [dbo].[Boards] DROP CONSTRAINT [DF__Boards__type__4AB81AF0];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Teams]...';


GO
ALTER TABLE [dbo].[Teams] DROP CONSTRAINT [DF__Teams__ownerId__5812160E];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Teams]...';


GO
ALTER TABLE [dbo].[Teams] DROP CONSTRAINT [DF__Teams__security__59063A47];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Teams]...';


GO
ALTER TABLE [dbo].[Teams] DROP CONSTRAINT [DF__Teams__name__59FA5E80];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Teams]...';


GO
ALTER TABLE [dbo].[Teams] DROP CONSTRAINT [DF__Teams__datecreat__5AEE82B9];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Teams]...';


GO
ALTER TABLE [dbo].[Teams] DROP CONSTRAINT [DF__Teams__website__5BE2A6F2];


GO
PRINT N'Dropping unnamed constraint on [dbo].[Teams]...';


GO
ALTER TABLE [dbo].[Teams] DROP CONSTRAINT [DF__Teams__descripti__5CD6CB2B];


GO
PRINT N'Dropping unnamed constraint on [dbo].[TeamMembers]...';


GO
ALTER TABLE [dbo].[TeamMembers] DROP CONSTRAINT [DF__TeamMembe__secur__571DF1D5];

GO
/*
The column [dbo].[Boards].[favorite] is being dropped, data loss could occur.

The column [dbo].[Boards].[security] is being dropped, data loss could occur.

The column [dbo].[Boards].[teamId] on table [dbo].[Boards] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/
GO
PRINT N'Starting rebuilding table [dbo].[Boards]...';

BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Boards] (
    [boardId]      INT           NOT NULL,
    [teamId]       INT           NOT NULL,
    [orgId]        INT           DEFAULT 0 NOT NULL,
    [archived]     BIT           DEFAULT 0 NOT NULL,
    [name]         NVARCHAR (64) NOT NULL,
    [color]        NVARCHAR (6)  DEFAULT '' NOT NULL,
    [datecreated]  DATETIME      DEFAULT GETDATE() NOT NULL,
    [lastmodified] DATETIME      DEFAULT GETDATE() NOT NULL,
    [type]         INT           DEFAULT 0 NOT NULL,
    PRIMARY KEY CLUSTERED ([boardId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Boards])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Boards] ([boardId], [teamId], [archived], [name], [color], [datecreated], [lastmodified], [type])
        SELECT   [boardId],
                 [teamId],
                 [archived],
                 [name],
                 [color],
                 [datecreated],
                 [lastmodified],
                 [type]
        FROM     [dbo].[Boards]
        ORDER BY [boardId] ASC;
    END

DROP TABLE [dbo].[Boards];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Boards]', N'Boards';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Altering [dbo].[TeamMembers]...';


GO
ALTER TABLE [dbo].[TeamMembers] DROP COLUMN [security];
GO
ALTER TABLE [dbo].[BoardMembers] ADD [favorite] BIT;
GO
/*
The column [dbo].[Teams].[security] is being dropped, data loss could occur.

The column [dbo].[Teams].[website] is being dropped, data loss could occur.

The column [dbo].[Teams].[ownerId] on table [dbo].[Teams] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/
GO
PRINT N'Starting rebuilding table [dbo].[Teams]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Teams] (
    [teamId]      INT            NOT NULL,
    [orgId]       INT            DEFAULT 0 NOT NULL,
    [ownerId]     INT            NOT NULL,
    [name]        NVARCHAR (64)  DEFAULT '' NOT NULL,
    [datecreated] DATETIME       DEFAULT GETDATE() NOT NULL,
    [description] NVARCHAR (MAX) DEFAULT '' NOT NULL,
    PRIMARY KEY CLUSTERED ([teamId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Teams])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Teams] ([teamId], [ownerId], [name], [datecreated], [description])
        SELECT   [teamId],
                 [ownerId],
                 [name],
                 [datecreated],
                 [description]
        FROM     [dbo].[Teams]
        ORDER BY [teamId] ASC;
    END

DROP TABLE [dbo].[Teams];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Teams]', N'Teams';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BoardTeams]...';


GO
CREATE TABLE [dbo].[BoardTeams] (
    [boardId] INT NOT NULL,
    [teamId]  INT NOT NULL
);


GO
PRINT N'Creating [dbo].[Organizations]...';


GO
CREATE TABLE [dbo].[Organizations] (
    [orgId]       INT            NOT NULL,
    [ownerId]     INT            NOT NULL,
    [name]        NVARCHAR (64)  NOT NULL,
    [datecreated] DATETIME2 (7)  NOT NULL,
    [website]     NVARCHAR (255) NOT NULL,
    [description] NVARCHAR (MAX) NOT NULL,
    [banner]      BIT            NOT NULL,
    [photo]       BIT            NOT NULL,
    [enabled]     BIT            NOT NULL,
    [isprivate]   BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([orgId] ASC)
);


GO
PRINT N'Creating [dbo].[Security]...';


GO
CREATE TABLE [dbo].[Security] (
    [orgId]   INT          NOT NULL,
    [groupId]  INT          NOT NULL,
    [key]     VARCHAR (32) NOT NULL,
    [enabled] BIT          NOT NULL,
    PRIMARY KEY CLUSTERED ([orgId] ASC, [groupId] ASC, [key] ASC)
);


GO
PRINT N'Creating unnamed constraint on [dbo].[Organizations]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD DEFAULT GETUTCDATE() FOR [datecreated];


GO
PRINT N'Creating unnamed constraint on [dbo].[Organizations]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD DEFAULT '' FOR [website];


GO
PRINT N'Creating unnamed constraint on [dbo].[Organizations]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD DEFAULT '' FOR [description];


GO
PRINT N'Creating unnamed constraint on [dbo].[Organizations]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD DEFAULT 0 FOR [banner];


GO
PRINT N'Creating unnamed constraint on [dbo].[Organizations]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD DEFAULT 0 FOR [photo];
	
GO
ALTER TABLE [dbo].[BoardMembers]
    ADD DEFAULT 0 FOR [favorite];


GO
PRINT N'Creating unnamed constraint on [dbo].[Organizations]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD DEFAULT 1 FOR [enabled];


GO
PRINT N'Creating unnamed constraint on [dbo].[Organizations]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD DEFAULT 1 FOR [isprivate];


GO
PRINT N'Creating unnamed constraint on [dbo].[Security]...';


GO
ALTER TABLE [dbo].[Security]
    ADD DEFAULT 0 FOR [enabled];

GO
PRINT N'Creating [dbo].[SequenceOrganizations]...';


GO
CREATE SEQUENCE [dbo].[SequenceOrganizations]
    AS BIGINT
    START WITH 1
    INCREMENT BY 1
    NO CACHE;


GO

GO
PRINT N'Update complete.';


GO
