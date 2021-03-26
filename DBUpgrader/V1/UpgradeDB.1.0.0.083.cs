using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{
		private partial class UpgradeDB
		{
			[DatabaseUpgradeMethod("1.0.0.83")]
			public static void Init_1_0_0_83(IDatabaseUpgrader du)
			{
				string sql = @"

/****** Object:  Table [dbo].[AutocompleteSynonyms]    Script Date: 26.03.2021 10:25:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AutocompleteSynonyms]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AutocompleteSynonyms](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[text] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[query] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[type] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[priority] [int] NOT NULL,
	[imageElement] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_AutocompleteSynonyms] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_AutocompleteSynonyms_type]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AutocompleteSynonyms] ADD  CONSTRAINT [DF_AutocompleteSynonyms_type]  DEFAULT ('') FOR [type]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_AutocompleteSynonyms_priority]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AutocompleteSynonyms] ADD  CONSTRAINT [DF_AutocompleteSynonyms_priority]  DEFAULT ((0)) FOR [priority]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_AutocompleteSynonyms_imageElement]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AutocompleteSynonyms] ADD  CONSTRAINT [DF_AutocompleteSynonyms_imageElement]  DEFAULT ('') FOR [imageElement]
END
GO


";
				du.RunDDLCommands(sql);
			}
		}
	}
}
