using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{
		private partial class UpgradeDB
		{
			[DatabaseUpgradeMethod("1.0.0.84")]
			public static void Init_1_0_0_84(IDatabaseUpgrader du)
			{
				string sql = @"ALTER TABLE osoba ADD OriginalId int NULL;";
				du.RunDDLCommands(sql);
			}
		}
	}
}
