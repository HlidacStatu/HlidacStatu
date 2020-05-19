using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.71")]
			public static void Init_1_0_0_71(IDatabaseUpgrader du)
			{

				string sql = @"
ALTER TABLE dbo.Osoba ADD ManuallyUpdated DateTime;
ALTER TABLE dbo.Osoba ADD ManuallyUpdatedBy nvarchar(150);
GO
COMMIT
";
				du.RunDDLCommands(sql);

			}
		}
	}
}
