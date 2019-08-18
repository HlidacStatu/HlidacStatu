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

			[DatabaseUpgradeMethod("1.0.0.21")]
			public static void Init_1_0_0_21(IDatabaseUpgrader du)
			{

                //fix of 1.0.0.14 script

                string sql = @"
ALTER TABLE dbo.OsobaEvent ADD
	AddInfo nvarchar(MAX) NULL


";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
