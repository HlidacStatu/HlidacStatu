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

			[DatabaseUpgradeMethod("1.0.0.12")]
			public static void Init_1_0_0_12(IDatabaseUpgrader du)
			{
				string sql = @"

ALTER TABLE dbo.AngazovanostFirma ADD
	podil decimal(18,9) NULL
GO

ALTER TABLE dbo.AngazovanostFirma ADD
	created datetime NULL
GO

ALTER TABLE dbo.AngazovanostFirma ADD
	updated datetime NULL

GO
";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
