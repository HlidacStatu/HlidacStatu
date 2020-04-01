using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.68")]
			public static void Init_1_0_0_68(IDatabaseUpgrader du)
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
CREATE TABLE dbo.FirmaHint
	(
	Ico nvarchar(30) NOT NULL,
	PocetDni_k_PrvniSmlouve int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.FirmaHint ADD CONSTRAINT
	PK_FirmaHint PRIMARY KEY CLUSTERED 
	(
	Ico
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.FirmaHint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

";
				du.RunDDLCommands(sql);

			}
		}
	}
}
