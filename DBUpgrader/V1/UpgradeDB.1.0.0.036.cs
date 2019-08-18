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

			[DatabaseUpgradeMethod("1.0.0.36")]
			public static void Init_1_0_0_36(IDatabaseUpgrader du)
			{

                string sql = @"
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Audit_date]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Audit] DROP CONSTRAINT [DF_Audit_date]
END

GO

/****** Object:  Table [dbo].[Audit]    Script Date: 06.11.2017 22:43:41 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Audit]') AND type in (N'U'))
DROP TABLE [dbo].[Audit]
GO

/****** Object:  Table [dbo].[Audit]    Script Date: 06.11.2017 22:43:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Audit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Audit](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NOT NULL,
	[userId] [nvarchar](50) NOT NULL,
	[operation] [nvarchar](50) NOT NULL,
	[objectType] [nvarchar](150) NOT NULL,
	[objectId] [nvarchar](150) NOT NULL,
	[valueBefore] [nvarchar](max) NULL,
	[valueAfter] [nvarchar](max) NULL,
 CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Audit_date]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Audit] ADD  CONSTRAINT [DF_Audit_date]  DEFAULT (getdate()) FOR [date]
END

GO




";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
