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

			[DatabaseUpgradeMethod("1.0.0.50")]
			public static void Init_1_0_0_50(IDatabaseUpgrader du)
			{

                
                //du.RunDDLCommands(sql);
                du.AddColumnToTable("Created", "datetime", "Bookmarks", false);


            }




        }

	}
}
