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

			[DatabaseUpgradeMethod("1.0.0.34")]
			public static void Init_1_0_0_34(IDatabaseUpgrader du)
			{

                string sql = @"
    IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Server_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Server] DROP CONSTRAINT [DF_Media_Server_Created]
END

GO

/****** Object:  Table [dbo].[Media_Server]    Script Date: 18.09.2017 0:12:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_Server]') AND type in (N'U'))
DROP TABLE [dbo].[Media_Server]
GO

/****** Object:  Table [dbo].[Media_Server]    Script Date: 18.09.2017 0:12:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_Server]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Media_Server](
	[ServerId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[URL] [nvarchar](150) NOT NULL,
	[LogoUrl] [nvarchar](150) NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_Media_Server] PRIMARY KEY CLUSTERED 
(
	[ServerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Server_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Server] ADD  CONSTRAINT [DF_Media_Server_Created]  DEFAULT (getdate()) FOR [Created]
END

GO



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Article_LastChecked]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Article] DROP CONSTRAINT [DF_Media_Article_LastChecked]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Article_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Article] DROP CONSTRAINT [DF_Media_Article_Created]
END

GO

/****** Object:  Table [dbo].[Media_Article]    Script Date: 18.09.2017 0:12:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_Article]') AND type in (N'U'))
DROP TABLE [dbo].[Media_Article]
GO

/****** Object:  Table [dbo].[Media_Article]    Script Date: 18.09.2017 0:12:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_Article]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Media_Article](
	[ArticleId] [int] IDENTITY(1,1) NOT NULL,
	[serverId] [int] NOT NULL,
	[ArticleUrl] [nvarchar](800) NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastChecked] [datetime] NOT NULL,
 CONSTRAINT [PK_Media_Article] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Article_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Article] ADD  CONSTRAINT [DF_Media_Article_Created]  DEFAULT (getdate()) FOR [Created]
END

GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Article_LastChecked]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Article] ADD  CONSTRAINT [DF_Media_Article_LastChecked]  DEFAULT (getdate()) FOR [LastChecked]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_ArticleHistory_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_ArticleHistory] DROP CONSTRAINT [DF_Media_ArticleHistory_Created]
END

GO

/****** Object:  Table [dbo].[Media_ArticleHistory]    Script Date: 18.09.2017 0:13:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_ArticleHistory]') AND type in (N'U'))
DROP TABLE [dbo].[Media_ArticleHistory]
GO

/****** Object:  Table [dbo].[Media_ArticleHistory]    Script Date: 18.09.2017 0:13:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_ArticleHistory]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Media_ArticleHistory](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[ArticleId] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Title] [nvarchar](800) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Published] [datetime] NULL,
	[TitleDiff] [nvarchar](max) NULL,
	[BodyDiff] [nvarchar](max) NULL,
 CONSTRAINT [PK_Media_ArticleHistory] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_ArticleHistory_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_ArticleHistory] ADD  CONSTRAINT [DF_Media_ArticleHistory_Created]  DEFAULT (getdate()) FOR [Created]
END

GO






";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
