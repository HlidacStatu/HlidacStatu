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

			[DatabaseUpgradeMethod("1.0.0.23")]
			public static void Init_1_0_0_23(IDatabaseUpgrader du)
			{

                //fix of 1.0.0.14 script

                string sql = @"


insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1486017, '26014343',99, 'Spravce AB private trust I', '2017-02-03',1, 'bit.ly/AB_private_trust')

insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (800730, '26014343',99, 'Protektor AB private trust I', '2017-02-03',1, 'bit.ly/AB_private_trust')
insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1461247, '26014343',99, 'Protektor AB private trust I', '2017-02-03',1, 'bit.ly/AB_private_trust')
insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1032422, '26014343',99, 'Protektor AB private trust I', '2017-02-03',1, 'bit.ly/AB_private_trust')

insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1486017, '26185610',99, 'Spravce AB private trust I', '2017-02-03',1, 'bit.ly/AB_private_trust')

insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (800730, '26185610',99, 'Protektor AB private trust I', '2017-02-03',1, 'bit.ly/AB_private_trust')
insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1461247, '26185610',99, 'Protektor AB private trust I', '2017-02-03',1, 'bit.ly/AB_private_trust')
insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1032422, '26185610',99, 'Protektor AB private trust I', '2017-02-03',1, 'bit.ly/AB_private_trust')


insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1032422, '26185610',99, 'Spravce AB private trust II', '2017-02-03',1, 'bit.ly/AB_private_trust')

insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (800730, '26185610',99, 'Protektor AB private trust II', '2017-02-03',1, 'bit.ly/AB_private_trust')
insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1461247, '26185610',99, 'Protektor AB private trust II', '2017-02-03',1, 'bit.ly/AB_private_trust')
insert into OsobaVazby(OsobaID, VazbakICO, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis, Zdroj)
values (1486017, '26185610',99, 'Protektor AB private trust II', '2017-02-03',1, 'bit.ly/AB_private_trust')


insert into OsobaVazby(OsobaID, VazbakICO, VazbakOsobaId, TypVazby, PojmenovaniVazby, DatumOd, RucniZapis)
values (2097081,'', 1461247,-3, 'Partnerka/manželka', null,1 )


insert into OsobaEvent(OsobaId, Title, DatumOd, [Type], AddInfo)
values (1461247, 'Partnerka', '1994-01-01',4,'2097081')

insert into OsobaEvent(OsobaId, Title, DatumOd, [Type], AddInfo)
values (2097081, 'Partner', '1994-01-01',4,'1461247')


";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
