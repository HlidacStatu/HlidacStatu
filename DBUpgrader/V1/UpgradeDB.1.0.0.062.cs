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

			[DatabaseUpgradeMethod("1.0.0.62")]
			public static void Init_1_0_0_62(IDatabaseUpgrader du)
			{
                string sql = @"
/****** Object:  StoredProcedure [dbo].[DELETE_ASPNETUser]    Script Date: 14. 9. 2019 12:13:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DELETE_ASPNETUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'ALTER PROCEDURE [dbo].[DELETE_ASPNETUser]
	@id nvarchar(128) = null,
	@email nvarchar(256) = null
AS
BEGIN

if (@id is null and @email is null)
	return;

if (@id is null and @email is not null)
	set @id = select id from AspNetUsers where email like @email


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


END' 
END

";
    du.RunDDLCommands(sql);

            }




        }

	}
}
