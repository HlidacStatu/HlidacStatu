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

			[DatabaseUpgradeMethod("1.0.0.60")]
			public static void Init_1_0_0_60(IDatabaseUpgrader du)
			{
                string sql = @"

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.DeleteFakeAccounts
@daysBack int = 30
AS
BEGIN

	set nocount on

	DECLARE @id nvarchar(128) 
 
	DECLARE cursorT CURSOR
	--LOCAL STATIC
	--LOCAL FAST_FORWARD
	--LOCAL READ_ONLY FORWARD_ONLY
	FOR
	SELECT u.id
	FROM aspnetUsers u inner join aspnetusertokens ut on u.id = ut.id
	WHERE ut.created < DateAdd(dy,-1 * @daysBack,getdate()) and u.emailconfirmed=0
 
	OPEN cursorT 
	FETCH NEXT FROM cursorT INTO @id
	WHILE @@FETCH_STATUS = 0
	BEGIN
	   Print @id
	   exec DELETE_ASPNETUser @id

	   FETCH NEXT FROM cursorT INTO @id

	END
	CLOSE cursorT 
	DEALLOCATE cursorT 


END
GO

";
    du.RunDDLCommands(sql);

            }




        }

	}
}
