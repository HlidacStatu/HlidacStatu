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

			[DatabaseUpgradeMethod("1.0.0.37")]
			public static void Init_1_0_0_37(IDatabaseUpgrader du)
			{

                string sql = @"
/****** Object:  Table [dbo].[NespolehlivyPlatceDPH]    Script Date: 04.01.2018 23:49:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NespolehlivyPlatceDPH]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NespolehlivyPlatceDPH](
	[Ico] [nvarchar](50) NOT NULL,
	[FromDate] [date] NULL,
	[ToDate] [date] NULL,
 CONSTRAINT [PK_NespolehlivyPlatceDPH] PRIMARY KEY CLUSTERED 
(
	[Ico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO





";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
