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

			[DatabaseUpgradeMethod("1.0.0.31")]
			public static void Init_1_0_0_31(IDatabaseUpgrader du)
			{

                string sql = @"
ALTER TABLE dbo.WatchDog ADD
	FocusId int NOT NULL CONSTRAINT DF_WatchDog_FocusId DEFAULT 0

";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
