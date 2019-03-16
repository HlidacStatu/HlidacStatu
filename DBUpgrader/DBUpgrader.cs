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
        public static void UpgradeDatabases()
        {

            MsSqlDatabaseUpgrader core = new MsSqlDatabaseUpgrader(
                Devmasters.Core.Util.Config.GetConfigValue("CnnString"),
                typeof(UpgradeDB),
                MsSqlDatabaseObjectTypeForDatabaseVersion.ExtendedProperty
            );
            
            core.Upgrade();


        }

    }
}
