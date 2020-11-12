using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.75")]
			public static void Init_1_0_0_75(IDatabaseUpgrader du)
			{

				string sql = @"BEGIN TRANSACTION
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
ALTER TABLE dbo.Firma_DS ADD
	dsParent nvarchar(50) NULL,
	dsSubjName nvarchar(50) NULL
GO
ALTER TABLE dbo.Firma_DS SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
";
				du.RunDDLCommands(sql);

			}
		}
	}
}
