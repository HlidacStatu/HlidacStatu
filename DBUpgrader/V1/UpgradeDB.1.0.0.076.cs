using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.76")]
			public static void Init_1_0_0_76(IDatabaseUpgrader du)
			{

				string sql = @"ALTER TABLE OsobaEvent ADD 
					Ico NVARCHAR(20) NULL,
					CEO int NULL;";
				du.RunDDLCommands(sql);

			}
		}
	}
}
