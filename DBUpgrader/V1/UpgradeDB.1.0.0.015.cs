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

			[DatabaseUpgradeMethod("1.0.0.15")]
			public static void Init_1_0_0_15(IDatabaseUpgrader du)
			{

                //fix of 1.0.0.14 script

                string sql = @"
/****** Object:  StoredProcedure [dbo].[SmlouvaId_Save]    Script Date: 15.04.2017 10:17:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmlouvaId_Save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SmlouvaId_Save]
GO

/****** Object:  StoredProcedure [dbo].[SmlouvaId_Save]    Script Date: 15.04.2017 10:17:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmlouvaId_Save]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SmlouvaId_Save] AS' 
END
GO


ALTER PROCEDURE [dbo].[SmlouvaId_Save] 
	@id nvarchar(50),
    @active int,
	@created datetime = null,
	@updated datetime = null
AS
BEGIN
	SET NOCOUNT ON;
	declare @dt datetime;
	set @dt = getdate();

if (@created is null)
	set @created=@dt;
if (@updated is null)
	set @updated=@dt;

IF EXISTS(SELECT id FROM SmlouvyIds 
          WHERE id=@id)
    BEGIN
		update SmlouvyIds
		set updated = @updated, active = @active
		where id=@id
	END
	ELSE
    BEGIN
		insert into SmlouvyIds(id, created, updated, active) values(@id, @created,@updated,@active)
	END


END
GO


";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
