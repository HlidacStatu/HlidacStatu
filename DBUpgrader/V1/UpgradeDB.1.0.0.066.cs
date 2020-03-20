using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.66")]
			public static void Init_1_0_0_66(IDatabaseUpgrader du)
			{

				string sql = @"
ALTER TABLE dbo.OsobaEvent ADD Status int;
GO
COMMIT
";
				du.RunDDLCommands(sql);

			}
		}
	}
}
