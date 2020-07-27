using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.73")]
			public static void Init_1_0_0_73(IDatabaseUpgrader du)
			{

				string sql = @"

--investice hartenberg holding
insert into FirmaVazby(ico, VazbakICO, datumOd, TypVazby, PojmenovaniVazby, zdroj, RucniZapis)
values 
('01801261','02625067','2019-06-19',19,'akcionář','výroční zpráva Hartenberg Holding',1),
('01801261','04226917','2015-08-10',19,'akcionář','výroční zpráva Hartenberg Holding',1),
('01801261','26951070','2019-06-19',19,'akcionář','výroční zpráva Hartenberg Holding',1),
('01801261','25944355','2018-05-21',19,'akcionář','výroční zpráva Hartenberg Holding',1),
('01801261','27528791','2018-05-21',19,'akcionář','výroční zpráva Hartenberg Holding',1)
";
				du.RunDDLCommands(sql);

			}
		}
	}
}
