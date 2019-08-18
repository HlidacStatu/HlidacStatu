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

			[DatabaseUpgradeMethod("1.0.0.47")]
			public static void Init_1_0_0_47(IDatabaseUpgrader du)
			{

                string sql = @"
/****** Object:  UserDefinedFunction [dbo].[IsDateInInterval]    Script Date: 29/04/2018 19:48:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsDateInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[IsDateInInterval]
GO

/****** Object:  UserDefinedFunction [dbo].[IsSomehowInInterval]    Script Date: 29/04/2018 19:48:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsSomehowInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[IsSomehowInInterval]
GO

/****** Object:  UserDefinedFunction [dbo].[IsSomehowInInterval]    Script Date: 29/04/2018 19:48:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsSomehowInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [dbo].[IsSomehowInInterval]
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

GO

/****** Object:  UserDefinedFunction [dbo].[IsDateInInterval]    Script Date: 29/04/2018 19:48:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsDateInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [dbo].[IsDateInInterval]
(
	@from date,
	@to date,
	@date date
)
RETURNS bit
AS
BEGIN
	DECLARE @res bit

	if @date is null
		set @res = 0
	else if (@from <= @date and @date <= @to)
		or (@from is null and @date <= @to)
		or (@date <= @to and @to is null)
		set @res = 1
	else
		set @res = 0

	--print (@from + '' < '' + @date + '' < '' + @to)


	return @res
END

' 
END

GO


/****** Object:  UserDefinedFunction [dbo].[IsSomehowInInterval]    Script Date: 29/04/2018 19:48:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsSomehowInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[IsSomehowInInterval]
GO

/****** Object:  UserDefinedFunction [dbo].[IsSomehowInInterval]    Script Date: 29/04/2018 19:48:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsSomehowInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [dbo].[IsSomehowInInterval]
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

GO


";
                du.RunDDLCommands(sql);



            }




        }

	}
}
