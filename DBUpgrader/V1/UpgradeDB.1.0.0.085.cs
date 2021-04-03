using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{
		private partial class UpgradeDB
		{
			[DatabaseUpgradeMethod("1.0.0.85")]
			public static void Init_1_0_0_85(IDatabaseUpgrader du)
			{
				string sql = @"ALTER TABLE dbo.AutocompleteSynonyms ADD	active int NOT NULL CONSTRAINT DF_AutocompleteSynonyms_active DEFAULT 1
";
				du.RunDDLCommands(sql);
			}
		}
	}
}
