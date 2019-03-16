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

			[DatabaseUpgradeMethod("1.0.0.53")]
			public static void Init_1_0_0_53(IDatabaseUpgrader du)
			{

                string sql = @"
Create PROCEDURE [dbo].[Firma_IsInRS_Save] 
	@ico nvarchar(30)

AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT ico FROM firma
          WHERE ico=@ico and IsInRs is null)
	begin
	update firma set IsInRS = 1 where ico=@ico
	end

END
";
                du.RunDDLCommands(sql);


            }




        }

	}
}
