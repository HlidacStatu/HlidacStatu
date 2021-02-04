using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.81")]
			public static void Init_1_0_0_81(IDatabaseUpgrader du)
			{

				string sql = @"
CREATE INDEX idx_sponzoring_OsobaIdDarce ON Sponzoring (OsobaIdDarce);
CREATE INDEX idx_sponzoring_IcoDarce ON Sponzoring (IcoDarce);
CREATE INDEX idx_sponzoring_OsobaIdPrijemce ON Sponzoring (OsobaIdPrijemce);
CREATE INDEX idx_sponzoring_IcoPrijemce ON Sponzoring (IcoPrijemce);
";
				du.RunDDLCommands(sql);

			}
		}
	}
}
