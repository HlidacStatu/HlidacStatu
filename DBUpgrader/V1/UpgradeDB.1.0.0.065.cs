using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.65")]
			public static void Init_1_0_0_65(IDatabaseUpgrader du)
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
CREATE TABLE dbo.BannedIPs
	(
	IP nvarchar(50) NOT NULL,
	Expiration datetime NOT NULL,
	Created datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.BannedIPs ADD CONSTRAINT
	DF_BannedIPs_Created DEFAULT GetDate() FOR Created
GO
ALTER TABLE dbo.BannedIPs ADD CONSTRAINT
	PK_BannedIPs PRIMARY KEY CLUSTERED 
	(
	IP
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.BannedIPs SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


";
				du.RunDDLCommands(sql);

			}
		}
	}
}
