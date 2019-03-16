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

            [DatabaseUpgradeMethod("1.0.0.8")]
            public static void Init_1_0_0_8(IDatabaseUpgrader du)
            {
                string sql = @"


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DumpData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DumpData](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Processed] [datetime] NULL,
	[mesic] [int] NOT NULL,
	[rok] [int] NOT NULL,
	[hash] [nvarchar](65) NOT NULL,
	[velikost] [bigint] NOT NULL,
	[casGenerovani] [datetime] NOT NULL,
 CONSTRAINT [PK_DumpData] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO




";
                du.RunDDLCommands(sql);


            }

    


        }

    }
}
