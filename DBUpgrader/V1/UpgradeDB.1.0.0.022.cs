using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseUpgrader;
using System.Configuration;


namespace HlidacSmluv.DBUpgrades
{
	public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.22")]
			public static void Init_1_0_0_22(IDatabaseUpgrader du)
			{

                //fix of 1.0.0.14 script

                string sql = @"
ALTER TABLE dbo.OsobaVazby ADD
	VazbakOsobaId int NULL

GO

ALTER TABLE dbo.OsobaVazby ADD
	RucniZapis int NULL
GO
ALTER TABLE dbo.OsobaVazby ADD
	Poznamka nvarchar(500) NULL
GO

ALTER TABLE dbo.FirmaVazby ADD
	RucniZapis int NULL
GO
ALTER TABLE dbo.FirmaVazby ADD
	Poznamka nvarchar(500) NULL
GO
";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
