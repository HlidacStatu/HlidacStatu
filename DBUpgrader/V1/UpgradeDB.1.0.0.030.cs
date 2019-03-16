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

			[DatabaseUpgradeMethod("1.0.0.30")]
			public static void Init_1_0_0_30(IDatabaseUpgrader du)
			{

                string sql = @"
update OsobaEvent 
set title=''
where title = 'Popis'


GO

IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPaneCount' , N'SCHEMA',N'dbo', N'VIEW',N'vw_VypisFlat', NULL,NULL))
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPaneCount' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_VypisFlat'

GO

IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane1' , N'SCHEMA',N'dbo', N'VIEW',N'vw_VypisFlat', NULL,NULL))
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_VypisFlat'

GO

/****** Object:  View [dbo].[vw_VypisFlat]    Script Date: 09.06.2017 15:50:46 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_VypisFlat]'))
DROP VIEW [dbo].[vw_VypisFlat]
GO

/****** Object:  View [dbo].[vw_VypisFlat]    Script Date: 09.06.2017 15:50:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_VypisFlat]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[vw_VypisFlat]
AS
SELECT        f.ICO, f.Jmeno AS JmenoFirmy, f.Datum_zapisu_OR, 
ov.PojmenovaniVazby, 
case ov.TypVazby 
when 00 then 'Podnikatel_z_OR'		
when 01 then 'Clen_statutarniho_organu'	
when 02 then 'Likvidator'		
when 03 then 'Prokurista'		
when 04 then 'Clen_dozorci_rady'		
when 05 then 'Jediny_akcionar'		
when 06 then 'Clen_druzstva_s_vkladem'	
when 07 then 'Clen_dozorci_rady_v_zastoupeni'
when 08 then 'Clen_kontrolni_komise_v_zastoupeni'
when 09 then 'Komplementar'	
when 10 then 'Komanditista'		
when 11 then 'Spravce_konkursu'		
when 12 then 'Likvidator_v_zastoupeni'	
when 13 then 'Oddeleny_insolvencni_spravce'
when 14 then 'Pobocny_spolek'		
when 15 then 'Podnikatel'		
when 16 then 'Predbezny_insolvencni_spravce'
when 17 then 'Predbezny_spravce'		
when 18 then 'Predstavenstvo'		
when 19 then 'Podilnik'
when 20 then 'Revizor'
when 21 then 'Revizor_v_zastoupeni'	
when 22 then 'Clen_rozhodci_komise'	
when 23 then 'Vedouci_odstepneho_zavodu'	
when 24 then 'Spolecnik'
when 25 then 'Clen_spravni_rady_v_zastoupeni'
when 26 then 'Clen_statutarniho_organu_zrizovatele'
when 28 then 'Clen_statutarniho_organu_v_zastoupeni'
when 29 then 'Insolvencni_spravce_vyrovnavaci'
when 31 then 'Clen_spravni_rady'		
when 32 then 'Statutarni_organ_zrizovatele_v_zastoupeni'
when 33 then 'Zakladatel'		
when 34 then 'Nastupce_zrizovatele'	
when 35 then 'Zakladatel_s_vkladem'	
when 36 then 'Clen_sdruzeni'		
when 37 then 'Zastupce_insolvencniho_spravce'
when 38 then 'Clen_kontrolni_komise'	
when 39 then 'Insolvencni_spravce'	
when 40 then 'Zastupce_spravce'		
when 41 then 'Zvlastni_insolvencni_spravce'
when 42 then 'Zvlastni_spravce'		
end AS vazba, 
ov.podil, ov.DatumOd AS vztah_osoby_k_firme_Od, ov.DatumDo AS vztah_osoby_k_firme_Do, 
                         o.Jmeno, o.Prijmeni, o.Narozeni, o.Mesto, o.CountryCode
FROM            dbo.Firma AS f INNER JOIN
                         dbo.OsobaVazby AS ov ON f.ICO = ov.VazbakICO INNER JOIN
                         dbo.Osoba AS o ON ov.OsobaID = o.InternalId
' 
GO


IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPaneCount' , N'SCHEMA',N'dbo', N'VIEW',N'vw_VypisFlat', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_VypisFlat'
GO




";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
