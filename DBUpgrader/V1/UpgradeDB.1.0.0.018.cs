using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseUpgrader;
using System.Configuration;


namespace HlidacSmluv.DBUpgrades
{
	public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.18")]
			public static void Init_1_0_0_18(IDatabaseUpgrader du)
			{

                //fix of 1.0.0.14 script

                string sql = @"
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

GO
CREATE TABLE dbo.Tmp_OsobaEvent
	(
	pk int NOT NULL,
	OsobaId int NOT NULL,
	Title nvarchar(200) NULL,
	Description nvarchar(MAX) NULL,
	DatumOd date NULL,
	DatumDo date NULL,
	Type int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_OsobaEvent SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.OsobaEvent)
	 EXEC('INSERT INTO dbo.Tmp_OsobaEvent (pk, OsobaId, Title, Description, DatumOd, DatumDo, Type)
		SELECT pk, OsobaId, Title, Description, DatumOd, DatumDo, Type FROM dbo.OsobaEvent WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.OsobaEvent
GO
EXECUTE sp_rename N'dbo.Tmp_OsobaEvent', N'OsobaEvent', 'OBJECT' 
GO
ALTER TABLE dbo.OsobaEvent ADD CONSTRAINT
	PK_OsobaEvent_1 PRIMARY KEY CLUSTERED 
	(
	pk
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_OsobaEvent_Osoba ON dbo.OsobaEvent
	(
	OsobaId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
