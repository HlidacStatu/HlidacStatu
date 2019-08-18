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

			[DatabaseUpgradeMethod("1.0.0.10")]
			public static void Init_1_0_0_10(IDatabaseUpgrader du)
			{
				string sql = @"

ALTER TABLE dbo.DumpData ADD
	exception nvarchar(MAX) NULL

GO


";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
