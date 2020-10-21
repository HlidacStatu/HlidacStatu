using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.74")]
			public static void Init_1_0_0_74(IDatabaseUpgrader du)
			{

				string sql = @"ALTER TABLE osoba ADD WikiId NVARCHAR(20);";
				du.RunDDLCommands(sql);

			}
		}
	}
}
