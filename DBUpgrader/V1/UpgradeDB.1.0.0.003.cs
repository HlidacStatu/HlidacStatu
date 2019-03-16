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

            [DatabaseUpgradeMethod("1.0.0.3")]
            public static void Init_1_0_0_3(IDatabaseUpgrader du)
            {
                string sql = @"

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Orders_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [DF_Orders_Created]
END

GO

/****** Object:  Table [dbo].[Orders]    Script Date: 26/10/16 13:18:41 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
DROP TABLE [dbo].[Orders]
GO

/****** Object:  Table [dbo].[Orders]    Script Date: 26/10/16 13:18:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(2016,1) NOT NULL,
	[ICO] [nvarchar](20) NULL,
	[DIC] [nvarchar](50) NULL,
	[Jmeno] [nvarchar](150) NULL,
	[Created] [datetime] NOT NULL,
	[PaidUntil] [date] NULL,
	[Deleted] [datetime] NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Orders_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_Created]  DEFAULT (getdate()) FOR [Created]
END

GO


";
                du.RunDDLCommands(sql);


            }

    


        }

    }
}
