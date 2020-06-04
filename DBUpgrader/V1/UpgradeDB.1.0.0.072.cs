using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.72")]
			public static void Init_1_0_0_72(IDatabaseUpgrader du)
			{

				string sql = @"
ALTER TABLE dbo.FirmaEvent ADD Note nvarchar(max);
GO
COMMIT
";
				du.RunDDLCommands(sql);

			}
		}
	}
}
