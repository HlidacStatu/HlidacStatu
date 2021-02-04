using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.79")]
			public static void Init_1_0_0_79(IDatabaseUpgrader du)
			{

				string sql = @"
Create table Sponzoring(
    Id int identity(1,1) Primary key,
    OsobaIdDarce int,
    IcoDarce nvarchar(20),
    OsobaIdPrijemce int,
    IcoPrijemce nvarchar(20),
    Typ int not null,
    Hodnota decimal(18,9),
    Popis nvarchar(max),
    DarovanoDne date,
    Zdroj nvarchar(max),
    Created date,
    Edited date,
    UpdatedBy nvarchar(150)
);
";
				du.RunDDLCommands(sql);

			}
		}
	}
}
