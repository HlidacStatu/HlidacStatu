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

			[DatabaseUpgradeMethod("1.0.0.27")]
			public static void Init_1_0_0_27(IDatabaseUpgrader du)
			{

                string sql = @"

ALTER TABLE dbo.OsobaEvent ADD
	Created datetime NOT NULL CONSTRAINT DF_OsobaEvent_Created DEFAULT getDate(),
	Zdroj nvarchar(MAX) NULL
GO
ALTER TABLE dbo.OsobaEvent SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.FirmaEvent ADD
	Created datetime NOT NULL CONSTRAINT DF_FirmaEvent_Created DEFAULT getDate(),
	Zdroj nvarchar(MAX) NULL
GO
ALTER TABLE dbo.FirmaEvent SET (LOCK_ESCALATION = TABLE)
GO


";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
