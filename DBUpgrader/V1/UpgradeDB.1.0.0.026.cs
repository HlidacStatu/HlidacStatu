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

			[DatabaseUpgradeMethod("1.0.0.26")]
			public static void Init_1_0_0_26(IDatabaseUpgrader du)
			{

                string sql = @"

/****** Object:  Table [dbo].[FirmaEvent]    Script Date: 27.05.2017 17:41:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FirmaEvent]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FirmaEvent](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[ICO] [nvarchar](30) NOT NULL,
	[Title] [nvarchar](200) NULL,
	[Description] [nvarchar](max) NULL,
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Type] [int] NOT NULL,
	[AddInfo] [nvarchar](max) NULL,
	[AddInfoNum] [decimal](18, 9) NULL,
 CONSTRAINT [PK_FirmaEvent_1] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

/****** Object:  Index [IX_FirmaEvent_Osoba]    Script Date: 27.05.2017 17:41:31 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FirmaEvent]') AND name = N'IX_FirmaEvent_Osoba')
CREATE NONCLUSTERED INDEX [IX_FirmaEvent_Osoba] ON [dbo].[FirmaEvent]
(
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
