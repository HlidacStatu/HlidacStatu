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

			[DatabaseUpgradeMethod("1.0.0.13")]
			public static void Init_1_0_0_13(IDatabaseUpgrader du)
			{
				string sql = @"

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_AspNetUserTokens_Count]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AspNetUserTokens] DROP CONSTRAINT [DF_AspNetUserTokens_Count]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_AspNetUserTokens_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AspNetUserTokens] DROP CONSTRAINT [DF_AspNetUserTokens_Created]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_AspNetUserTokens_Token]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AspNetUserTokens] DROP CONSTRAINT [DF_AspNetUserTokens_Token]
END

GO

/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 28.03.2017 11:49:29 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]') AND type in (N'U'))
DROP TABLE [dbo].[AspNetUserTokens]
GO

/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 28.03.2017 11:49:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserTokens](
	[Id] [nvarchar](128) NOT NULL,
	[Token] [uniqueidentifier] NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastAccess] [datetime] NULL,
	[Count] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_AspNetUserTokens_Token]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AspNetUserTokens] ADD  CONSTRAINT [DF_AspNetUserTokens_Token]  DEFAULT (newid()) FOR [Token]
END

GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_AspNetUserTokens_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AspNetUserTokens] ADD  CONSTRAINT [DF_AspNetUserTokens_Created]  DEFAULT (getdate()) FOR [Created]
END

GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_AspNetUserTokens_Count]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AspNetUserTokens] ADD  CONSTRAINT [DF_AspNetUserTokens_Count]  DEFAULT ((0)) FOR [Count]
END

GO


insert into AspNetUserTokens
select id, NEWID() as token, getdate() as created, null as lastaccess, 0 as [count] from AspNetUsers
";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
