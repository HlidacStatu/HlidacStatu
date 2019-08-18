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

            [DatabaseUpgradeMethod("1.0.0.6")]
            public static void Init_1_0_0_6(IDatabaseUpgrader du)
            {
                string sql = @"
/****** Object:  Table [dbo].[WatchDog]    Script Date: 29.01.2017 18:28:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WatchDog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WatchDog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Expires] [datetime] NULL,
	[StatusId] [int] NOT NULL,
	[SearchTerm] [nvarchar](1000) NOT NULL,
	[SearchRawQuery] [nvarchar](max) NULL,
	[RunCount] [int] NOT NULL CONSTRAINT [DF_WatchDog_RunCount]  DEFAULT ((0)),
	[SentCount] [int] NOT NULL CONSTRAINT [DF_WatchDog_SentCount]  DEFAULT ((0)),
	[PeriodId] [int] NOT NULL,
	[LastSent] [datetime] NULL,
	[LastSearched] [datetime] NULL,
	[ToEmail] [int] NOT NULL CONSTRAINT [DF_WatchDog_ToEmail]  DEFAULT ((1)),
	[ShowPublic] [int] NOT NULL CONSTRAINT [DF_WatchDog_Public]  DEFAULT ((0)),
 CONSTRAINT [PK_WatchDog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_PADDING ON

GO

/****** Object:  Index [IX_WatchDog_UserId]    Script Date: 29.01.2017 18:28:48 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WatchDog]') AND name = N'IX_WatchDog_UserId')
CREATE NONCLUSTERED INDEX [IX_WatchDog_UserId] ON [dbo].[WatchDog]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO







";
                du.RunDDLCommands(sql);


            }

    


        }

    }
}
