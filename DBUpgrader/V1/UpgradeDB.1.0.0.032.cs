using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseUpgrader;
using System.Configuration;


namespace HlidacStatu.DBUpgrades
{
	public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.32")]
			public static void Init_1_0_0_32(IDatabaseUpgrader du)
			{

                string sql = @"

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.WatchDog
	DROP CONSTRAINT DF_WatchDog_RunCount
GO
ALTER TABLE dbo.WatchDog
	DROP CONSTRAINT DF_WatchDog_SentCount
GO
ALTER TABLE dbo.WatchDog
	DROP CONSTRAINT DF_WatchDog_ToEmail
GO
ALTER TABLE dbo.WatchDog
	DROP CONSTRAINT DF_WatchDog_Public
GO
ALTER TABLE dbo.WatchDog
	DROP CONSTRAINT DF_WatchDog_FocusId
GO
CREATE TABLE dbo.Tmp_WatchDog
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NOT NULL,
	Created datetime NOT NULL,
	Expires datetime NULL,
	StatusId int NOT NULL,
	SearchTerm nvarchar(MAX) NOT NULL,
	SearchRawQuery nvarchar(MAX) NULL,
	RunCount int NOT NULL,
	SentCount int NOT NULL,
	PeriodId int NOT NULL,
	LastSent datetime NULL,
	LastSearched datetime NULL,
	ToEmail int NOT NULL,
	ShowPublic int NOT NULL,
	Name nvarchar(50) NULL,
	FocusId int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_WatchDog SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_WatchDog ADD CONSTRAINT
	DF_WatchDog_RunCount DEFAULT ((0)) FOR RunCount
GO
ALTER TABLE dbo.Tmp_WatchDog ADD CONSTRAINT
	DF_WatchDog_SentCount DEFAULT ((0)) FOR SentCount
GO
ALTER TABLE dbo.Tmp_WatchDog ADD CONSTRAINT
	DF_WatchDog_ToEmail DEFAULT ((1)) FOR ToEmail
GO
ALTER TABLE dbo.Tmp_WatchDog ADD CONSTRAINT
	DF_WatchDog_Public DEFAULT ((0)) FOR ShowPublic
GO
ALTER TABLE dbo.Tmp_WatchDog ADD CONSTRAINT
	DF_WatchDog_FocusId DEFAULT ((0)) FOR FocusId
GO
SET IDENTITY_INSERT dbo.Tmp_WatchDog ON
GO
IF EXISTS(SELECT * FROM dbo.WatchDog)
	 EXEC('INSERT INTO dbo.Tmp_WatchDog (Id, UserId, Created, Expires, StatusId, SearchTerm, SearchRawQuery, RunCount, SentCount, PeriodId, LastSent, LastSearched, ToEmail, ShowPublic, Name, FocusId)
		SELECT Id, UserId, Created, Expires, StatusId, CONVERT(nvarchar(MAX), SearchTerm), SearchRawQuery, RunCount, SentCount, PeriodId, LastSent, LastSearched, ToEmail, ShowPublic, Name, FocusId FROM dbo.WatchDog WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_WatchDog OFF
GO
DROP TABLE dbo.WatchDog
GO
EXECUTE sp_rename N'dbo.Tmp_WatchDog', N'WatchDog', 'OBJECT' 
GO
ALTER TABLE dbo.WatchDog ADD CONSTRAINT
	PK_WatchDog PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_WatchDog_UserId ON dbo.WatchDog
	(
	UserId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT

";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
