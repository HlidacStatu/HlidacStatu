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

            [DatabaseUpgradeMethod("1.0.0.5")]
            public static void Init_1_0_0_5(IDatabaseUpgrader du)
            {
                string sql = @"

INSERT INTO [dbo].[Firma]
           ([ICO]
           ,[DIC]
           ,[Datum_zapisu_OR]
           ,[Stav_subjektu]
           ,[Jmeno]
           ,[Kod_PF]
           ,[VersionUpdate]
           ,[Esa2010]
           ,[Source]
           ,[Popis])
     VALUES
           ('00026000'
           ,''
           ,'1981/01/01'
           ,1
           ,N'Městské státní zastupitelství v Praze'
           ,325
           ,0
           ,13000
           ,''
           ,'')
GO

INSERT INTO [dbo].[Firma_DS]
           ([ICO]
           ,[DatovaSchranka])
     VALUES
           ('00026000'
           ,'ijeabe3')
GO





";
                du.RunDDLCommands(sql);


            }

    


        }

    }
}
