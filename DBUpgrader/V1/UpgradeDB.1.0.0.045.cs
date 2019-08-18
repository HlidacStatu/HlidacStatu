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

			[DatabaseUpgradeMethod("1.0.0.45")]
			public static void Init_1_0_0_45(IDatabaseUpgrader du)
			{

                string sql = @"


SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_FirmaVazby_9_1701581100__K2_K3_K1_4_5_6_7_8] ON [dbo].[FirmaVazby]
(
	[ICO] ASC,
	[VazbakICO] ASC,
	[pk] ASC
)
INCLUDE ( 	[TypVazby],
	[PojmenovaniVazby],
	[podil],
	[DatumOd],
	[DatumDo]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

SET ANSI_PADDING ON

go

CREATE NONCLUSTERED INDEX [_dta_index_FirmaVazby_9_1701581100__K3_K1_K2_4_5_6_7_8] ON [dbo].[FirmaVazby]
(
	[VazbakICO] ASC,
	[pk] ASC,
	[ICO] ASC
)
INCLUDE ( 	[TypVazby],
	[PojmenovaniVazby],
	[podil],
	[DatumOd],
	[DatumDo]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE STATISTICS [_dta_stat_1701581100_1_2] ON [dbo].[FirmaVazby]([pk], [ICO])
go



";
                du.RunDDLCommands(sql);



            }




        }

	}
}
