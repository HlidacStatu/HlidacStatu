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

			[DatabaseUpgradeMethod("1.0.0.48")]
			public static void Init_1_0_0_48(IDatabaseUpgrader du)
			{

                string sql = @"
/****** Object:  UserDefinedFunction [dbo].[IsSomehowInInterval]    Script Date: 16/08/2018 12:57:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsSomehowInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'ALTER FUNCTION [dbo].[IsSomehowInInterval]
(
	@dateIntervalFrom date,
	@dateIntervalTo date,
	@dateRelFrom date,
	@dateRelTo date
)
RETURNS bit
AS
BEGIN
	DECLARE @res bit

	declare @oks int; set @oks = 0;

	if (@dateIntervalFrom is null AND @dateIntervalTo is null)
		return 1;
	if (@dateRelFrom is null AND @dateRelTo is null)
		return 1;

	if (@dateRelFrom is null)
		set @dateRelFrom = ''1900-01-01'';
	if (@dateRelTo is null)
		set @dateRelto = DATEADD(dy,10,GetDate());


	if (dbo.IsDateInInterval(@dateRelFrom, @dateRelTo, @dateIntervalFrom) = 1)
		set @oks = @oks +1;

	if (dbo.IsDateInInterval(@dateRelFrom, @dateRelTo, @dateIntervalTo) = 1)
		set @oks = @oks +1;

	if (@dateIntervalFrom <= @dateRelFrom and @dateRelTo <= @dateIntervalTo
		and @dateIntervalFrom is not null and  @dateRelFrom is not null and @dateRelTo is not null and @dateIntervalTo is not null
		)
		set @oks = @oks +1;

	if (@oks=0
		and @dateIntervalFrom is null 
		and @dateIntervalTo >@dateRelTo
		)
		set @oks = @oks +1;

	if (@oks=0
		and @dateIntervalTo is null 
		and @dateIntervalFrom <@dateRelFrom
		)
		set @oks = @oks +1;


	if (@oks > 0)
		set @res= 1;
	else
		set @res = 0;


	RETURN @res

END


' 
END

";
                du.RunDDLCommands(sql);



            }




        }

	}
}
