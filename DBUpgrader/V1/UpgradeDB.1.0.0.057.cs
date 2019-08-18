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

			[DatabaseUpgradeMethod("1.0.0.57")]
			public static void Init_1_0_0_57(IDatabaseUpgrader du)
			{
                string sql = @"

/****** Object:  Table [dbo].[ItemToOcrQueue]    Script Date: 24/03/2019 11:17:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemToOcrQueue]') AND type in (N'U'))
DROP TABLE [dbo].[ItemToOcrQueue]
GO

/****** Object:  Table [dbo].[ItemToOcrQueue]    Script Date: 24/03/2019 11:17:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemToOcrQueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemToOcrQueue](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[itemType] [nvarchar](100) NOT NULL,
	[itemSubType] [nvarchar](100) NULL,
	[itemId] [nvarchar](150) NOT NULL,
	[created] [datetime] NOT NULL CONSTRAINT [DF_ItemToOcrQueue_created]  DEFAULT (getdate()),
	[done] [datetime] NULL,
	[status] [int] NOT NULL,
	[result] [nvarchar](max) NULL,
 CONSTRAINT [PK_ItemToOcrQueue] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO





";

                du.RunDDLCommands(sql);
            }




        }

	}
}
