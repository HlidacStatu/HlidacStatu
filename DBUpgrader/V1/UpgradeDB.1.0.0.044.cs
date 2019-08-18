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

			[DatabaseUpgradeMethod("1.0.0.44")]
			public static void Init_1_0_0_44(IDatabaseUpgrader du)
			{

                string sql = @"
";
                du.AddColumnToTable("comment", "nvarchar(500)", "review", true);
                //du.RunDDLCommands(sql);



            }




        }

	}
}
