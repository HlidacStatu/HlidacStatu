using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.69")]
			public static void Init_1_0_0_69(IDatabaseUpgrader du)
			{

				string sql = @"
/****** Object:  Table [dbo].[TipUrl]    Script Date: 19.04.2020 16:18:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TipUrl]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TipUrl](
	[Name] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Url] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_TipUrl] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_TipUrl_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TipUrl] ADD  CONSTRAINT [DF_TipUrl_Created]  DEFAULT (getdate()) FOR [Created]
END
GO



";
				du.RunDDLCommands(sql);

			}
		}
	}
}
