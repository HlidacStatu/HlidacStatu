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

			[DatabaseUpgradeMethod("1.0.0.54")]
			public static void Init_1_0_0_54(IDatabaseUpgrader du)
			{

                string sql = @"
/****** Object:  StoredProcedure [dbo].[SmlouvaId_Save]    Script Date: 04/01/2019 16:21:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmlouvaId_Save]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'ALTER PROCEDURE [dbo].[SmlouvaId_Save] 
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

set transaction isolation level serializable
begin transaction

	IF EXISTS(SELECT id FROM SmlouvyIds with (updlock) 
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
commit

END' 
END
";
                du.RunDDLCommands(sql);


            }




        }

	}
}
