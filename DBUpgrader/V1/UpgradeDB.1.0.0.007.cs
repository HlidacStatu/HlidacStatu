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

            [DatabaseUpgradeMethod("1.0.0.7")]
            public static void Init_1_0_0_7(IDatabaseUpgrader du)
            {
                string sql = @"
ALTER TABLE dbo.WatchDog ADD
	Name nvarchar(50) NULL




";
                du.RunDDLCommands(sql);


            }

    


        }

    }
}
