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

			[DatabaseUpgradeMethod("1.0.0.58")]
			public static void Init_1_0_0_58(IDatabaseUpgrader du)
			{
                du.AddColumnToTable("priority", "int", "ItemToOcrQueue", true);
                //string sql = @"update ItemToOcrQueue set priority = 10";
                //du.RunDDLCommands(sql);

            }




        }

	}
}
