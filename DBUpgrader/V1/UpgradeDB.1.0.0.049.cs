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

			[DatabaseUpgradeMethod("1.0.0.49")]
			public static void Init_1_0_0_49(IDatabaseUpgrader du)
			{

                string sql = @"
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Bookmarks_Folder]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Bookmarks] DROP CONSTRAINT [DF_Bookmarks_Folder]
END

GO

/****** Object:  Table [dbo].[Bookmarks]    Script Date: 26/09/2018 09:12:47 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Bookmarks]') AND type in (N'U'))
DROP TABLE [dbo].[Bookmarks]
GO

/****** Object:  Table [dbo].[Bookmarks]    Script Date: 26/09/2018 09:12:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Bookmarks]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Bookmarks](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](256) NOT NULL,
	[Folder] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Url] [varchar](4000) NOT NULL,
	[ItemType] [int] NOT NULL,
	[ItemId] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Bookmarks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Bookmarks_Folder]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Bookmarks] ADD  CONSTRAINT [DF_Bookmarks_Folder]  DEFAULT ('') FOR [Folder]
END

GO


";
                du.RunDDLCommands(sql);



            }




        }

	}
}
