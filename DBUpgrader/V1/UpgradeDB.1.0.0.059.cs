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

			[DatabaseUpgradeMethod("1.0.0.59")]
			public static void Init_1_0_0_59(IDatabaseUpgrader du)
			{
                string sql = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DELETE_ASPNETUser
	@id nvarchar(128)
AS
BEGIN


begin tran
  BEGIN TRY

	delete from AspNetUserClaims where userid=@id;
	delete from AspNetUserRoles where userid=@id;

	delete from AspNetUserlogins where userid=@id;
	--delete from useroptions where userid = @id;
	delete from WatchDog where userid = @id;
	delete from AspNetUsers where id=@id;
	commit tran
END TRY

BEGIN CATCH

    ROLLBACK TRANSACTION 

END CATCH  


END
GO
";
    du.RunDDLCommands(sql);

            }




        }

	}
}
