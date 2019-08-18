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

			[DatabaseUpgradeMethod("1.0.0.42")]
			public static void Init_1_0_0_42(IDatabaseUpgrader du)
			{

                string sql = @"
/****** Object:  Table [dbo].[Review]    Script Date: 13/04/2018 14:43:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Review]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Review](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[itemType] [nvarchar](150) NOT NULL,
	[oldValue] [nvarchar](max) NULL,
	[newValue] [nvarchar](max) NOT NULL,
	[created] [datetime] NOT NULL,
	[createdBy] [nvarchar](150) NOT NULL,
	[reviewed] [datetime] NULL,
	[reviewedBy] [nvarchar](150) NULL,
	[reviewResult] [int] NULL,
 CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Review_created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Review] ADD  CONSTRAINT [DF_Review_created]  DEFAULT (getdate()) FOR [created]
END

GO





";

                du.RunDDLCommands(sql);



            }




        }

	}
}
