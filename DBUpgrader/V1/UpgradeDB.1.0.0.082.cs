using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{
		private partial class UpgradeDB
		{
			[DatabaseUpgradeMethod("1.0.0.82")]
			public static void Init_1_0_0_82(IDatabaseUpgrader du)
			{
				string sql = @"ALTER TABLE firma ADD Typ int NULL;";
				du.RunDDLCommands(sql);
			}
		}
	}
}
