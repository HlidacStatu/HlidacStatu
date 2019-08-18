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

			[DatabaseUpgradeMethod("1.0.0.40")]
			public static void Init_1_0_0_40(IDatabaseUpgrader du)
			{

                du.AddColumnToTable("LatestRec", "datetime", "watchdog", true);

                string sql = @"
update watchdog 
set LatestRec = '2018-02-22'
where LatestRec is null
";

                du.RunDDLCommands(sql);



            }




        }

	}
}
