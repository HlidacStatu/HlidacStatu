using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.80")]
			public static void Init_1_0_0_80(IDatabaseUpgrader du)
			{

				string sql = @"
BEGIN
INSERT INTO Sponzoring ( OsobaIdDarce, IcoPrijemce, Typ, Hodnota, DarovanoDne, Zdroj, Created, Edited)
SELECT oe.OsobaId, oe.AddInfo, 0, oe.AddInfoNum, oe.DatumOd, oe.Zdroj, oe.Created, GETDATE()
  FROM OsobaEvent oe
 where oe.Type = 3;

INSERT INTO Sponzoring ( IcoDarce, IcoPrijemce, Typ, Hodnota, DarovanoDne, Zdroj, Created, Edited)
SELECT fe.ICO, fe.Description, 0, fe.AddInfoNum, fe.DatumOd, fe.Zdroj, fe.Created, GETDATE()
  FROM FirmaEvent fe
 where fe.type = 3;

Delete from OsobaEvent where Type = 3;
Delete from FirmaEvent where Type = 3;
Delete from FirmaEvent where Type = 33;
END
GO
COMMIT
";
				du.RunDDLCommands(sql);

			}
		}
	}
}
