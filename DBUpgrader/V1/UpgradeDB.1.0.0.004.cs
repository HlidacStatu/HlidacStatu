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

            [DatabaseUpgradeMethod("1.0.0.4")]
            public static void Init_1_0_0_4(IDatabaseUpgrader du)
            {
                string sql = @"
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 26/10/16 22:57:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__MigrationHistory]') AND type in (N'U'))
DROP TABLE [dbo].[__MigrationHistory]
GO


/****** Object:  Table [dbo].[DatoveSchranky]    Script Date: 26/10/16 23:11:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DatoveSchranky]') AND type in (N'U'))
DROP TABLE [dbo].[DatoveSchranky]
GO

/****** Object:  Table [dbo].[DatoveSchranky]    Script Date: 26/10/16 23:11:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DatoveSchranky]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DatoveSchranky](
	[DS] [nvarchar](50) NOT NULL,
	[Identifikator] [nvarchar](50) NOT NULL,
	[DetailJson] [nvarchar](max) NULL,
	[Url] [nvarchar](100) NULL,
 CONSTRAINT [PK_DatoveSchranky] PRIMARY KEY CLUSTERED 
(
	[DS] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


/****** Object:  Index [IX_DatoveSchranky_url]    Script Date: 27/10/16 09:52:52 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DatoveSchranky]') AND name = N'IX_DatoveSchranky_url')
DROP INDEX [IX_DatoveSchranky_url] ON [dbo].[DatoveSchranky]
GO

/****** Object:  Index [IX_DatoveSchranky_url]    Script Date: 27/10/16 09:52:52 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DatoveSchranky]') AND name = N'IX_DatoveSchranky_url')
CREATE NONCLUSTERED INDEX [IX_DatoveSchranky_url] ON [dbo].[DatoveSchranky]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO




";
                du.RunDDLCommands(sql);


            }

    


        }

    }
}
