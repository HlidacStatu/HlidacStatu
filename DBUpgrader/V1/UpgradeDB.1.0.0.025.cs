using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseUpgrader;
using System.Configuration;


namespace HlidacStatu.DBUpgrades
{
	public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.25")]
			public static void Init_1_0_0_25(IDatabaseUpgrader du)
			{

                string sql = @"
ALTER TABLE dbo.OsobaEvent ADD
	AddInfoNum decimal(18, 9) NULL


";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
